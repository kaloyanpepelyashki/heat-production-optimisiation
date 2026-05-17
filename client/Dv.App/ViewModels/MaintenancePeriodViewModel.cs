namespace Dv.App.ViewModels;

using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Dv.App.Models;
using Dv.App.Services;

// holds the maintenance logic based on the period and scenario
public partial class MaintenancePeriodViewModel : ViewModelBase
{
    private readonly MaintenanceService maintenanceService;
    private readonly IDialogService dialogService;
    private readonly IApiService apiService;

    [ObservableProperty]
    private string periodName = string.Empty;

    [ObservableProperty]
    private string scenario = string.Empty;

    [ObservableProperty]
    private DateTime periodStart;

    [ObservableProperty]
    private DateTime periodEnd;

    [ObservableProperty]
    private ObservableCollection<BoilerStatusViewModel> boilers = new();

    [ObservableProperty]
    private BoilerStatusViewModel? selectedBoiler;

    [ObservableProperty]
    private DateTime? maintenanceStartDate;

    [ObservableProperty]
    private int maintenanceStartHour = 12;

    [ObservableProperty]
    private double maintenanceDuration = 30;

    [ObservableProperty]
    private bool isOptimising;

    [ObservableProperty]
    private bool hasOptimisationResults;

    [ObservableProperty]
    private string? optimisationStatusMessage;

    [ObservableProperty]
    private double totalHeatProduction;

    [ObservableProperty]
    private double totalExpenses;

    [ObservableProperty]
    private double totalCo2Emissions;

    public ObservableCollection<OptimisationResultsHourlyClient> OptimisationResults { get; } = new();

    public OptimizationChartsViewModel Charts { get; } = new();

    // Exposes MaintenanceStore schedules for XAML binding
    public ObservableCollection<MaintenanceEvent> Schedules => MaintenanceStore.MaintenanceSchedules;

    // setting up the instance
    public MaintenancePeriodViewModel(MaintenanceService maintenanceService, IDialogService dialogService, IApiService apiService, string periodName, string scenario, DateTime start, DateTime end, ObservableCollection<BoilerStatusViewModel> initialBoilers)
    {
        this.maintenanceService = maintenanceService;
        this.dialogService = dialogService;
        this.apiService = apiService;
        this.PeriodName = periodName;
        this.Scenario = scenario;
        this.PeriodStart = start;
        this.PeriodEnd = end;
        this.Boilers = initialBoilers;

        MaintenanceStore.MaintenanceSchedules.CollectionChanged += (s, e) => this.UpdateBoilerStatuses();
        this.UpdateBoilerStatuses();
    }

    // getting the selected boiler data here
    [RelayCommand]
    private async Task ScheduleMaintenanceAsync()
    {
        string boilerId = this.SelectedBoiler?.BoilerId ?? "No Boiler Selected";

        if (!await this.EnsureMaintenanceCapacityAvailableAsync())
        {
            return;
        }

        if (this.MaintenanceStartDate.HasValue)
        {
            var startDateTime = this.MaintenanceStartDate.Value.Date.AddHours(this.MaintenanceStartHour);
            var endDateTime = startDateTime.AddHours(this.MaintenanceDuration);

            if (startDateTime < this.PeriodStart || endDateTime > this.PeriodEnd)
            {
                await this.dialogService.ShowValidationDialogAsync($"Maintenance interval must be within the {this.PeriodName} period:\n{this.PeriodStart:dd.MM.yyyy HH:mm} to {this.PeriodEnd:dd.MM.yyyy HH:mm}");
                return;
            }

            // Confirmation window for the user to confirm, if confirm, we write the maintenance info
            bool confirmed = await this.dialogService.ShowConfirmationDialogAsync(this.PeriodName, boilerId, startDateTime, endDateTime, this.PeriodStart, this.PeriodEnd);

            if (confirmed)
            {
                this.maintenanceService.SaveMaintenance(boilerId, startDateTime, endDateTime, this.PeriodName, this.Scenario);
            }
        }

        // exception for when the time has not been filled in properly
        else
        {
            await this.dialogService.ShowValidationDialogAsync("Please completely fill out the Date/Time fields.");
        }
    }

    [RelayCommand]
    private async Task RunOptimisationAsync()
    {
        var periodId = this.maintenanceService.GetPeriodId(this.PeriodName);

        var schedule = MaintenanceStore.MaintenanceSchedules.FirstOrDefault(s =>
            s.Period == periodId && s.Scenario == this.Scenario);

        var request = new OptimisationRequestDto
        {
            ScenarioId = int.Parse(this.Scenario),
            PeriodId = int.Parse(periodId),
            MaintenanceId = schedule?.MaintenanceId ?? 0,
            TimeFrom = this.PeriodStart,
            TimeTo = this.PeriodEnd,
        };

        this.IsOptimising = true;
        this.OptimisationStatusMessage = null;
        this.HasOptimisationResults = false;

        try
        {
            var response = await this.apiService.PostAsync<OptimisationRequestDto, ApiResponseWrapper<OptimisationRunClient>>(
                BackendService.Rdm, "optimisation", request);

            if (response?.Data == null || response.Data.OptimisationResultsHourly.Count == 0)
            {
                this.OptimisationStatusMessage = "No results returned from the optimiser.";
                return;
            }

            this.OptimisationResults.Clear();
            foreach (var hourly in response.Data.OptimisationResultsHourly)
                this.OptimisationResults.Add(hourly);

            this.TotalHeatProduction = Math.Round(this.OptimisationResults.Sum(r => r.HeatProduction), 2);
            this.TotalExpenses = Math.Round(this.OptimisationResults.Sum(r => r.Expenses), 2);
            this.TotalCo2Emissions = Math.Round(this.OptimisationResults.Sum(r => r.Co2Emissions), 2);

            this.Charts.LoadOptimizationResult(response.Data.OptimisationResultsHourly);

            this.HasOptimisationResults = true;
        }
        catch (Exception ex)
        {
            this.OptimisationStatusMessage = $"Optimisation failed: {ex.Message}";
            await this.dialogService.ShowValidationDialogAsync($"Optimisation failed.\n{ex.Message}");
        }
        finally
        {
            this.IsOptimising = false;
        }
    }

    // updates the boiler availability in the maintenance schedule
    private void UpdateBoilerStatuses()
    {
        foreach (var boiler in this.Boilers)
        {
            string boilerPeriodId = this.maintenanceService.GetPeriodId(boiler.Period);
            bool isUnavailable = MaintenanceStore.MaintenanceSchedules.Any(schedule =>
                schedule.Period == boilerPeriodId && schedule.AssetName == boiler.BoilerId && schedule.Scenario == this.Scenario);

            boiler.SetUnavailable(isUnavailable);
        }
    }

    // warning dialog if there is already a boiler in maintenance in the current period & scenario
    private async Task<bool> EnsureMaintenanceCapacityAvailableAsync()
    {
        string periodId = this.maintenanceService.GetPeriodId(this.PeriodName);
        bool hasMaintenance = MaintenanceStore.MaintenanceSchedules.Any(schedule => schedule.Period == periodId && schedule.Scenario == this.Scenario);

        if (hasMaintenance)
        {
            await this.dialogService.ShowValidationDialogAsync($"The maximum amount of maintenances has been reached for the {this.PeriodName} period in Scenario {this.Scenario}.");
            return false;
        }

        return true;
    }
}
