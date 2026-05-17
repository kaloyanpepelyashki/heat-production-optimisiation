namespace Dv.App.ViewModels;

using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Dv.App.Models;
using Dv.App.Services;
using Dv.App.Interfaces;

// holds the maintenance logic based on the period and scenario
public partial class MaintenancePeriodViewModel : ViewModelBase
{
    private readonly MaintenanceService maintenanceService;
    private readonly IDialogService dialogService;

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

    // Exposes MaintenanceStore schedules for XAML binding
    public ObservableCollection<MaintenanceEvent> Schedules => MaintenanceStore.MaintenanceSchedules;

    // setting up the instance
    public MaintenancePeriodViewModel(MaintenanceService maintenanceService, IDialogService dialogService, string periodName, string scenario, DateTime start, DateTime end, ObservableCollection<BoilerStatusViewModel> initialBoilers)
    {
        this.maintenanceService = maintenanceService;
        this.dialogService = dialogService;
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
