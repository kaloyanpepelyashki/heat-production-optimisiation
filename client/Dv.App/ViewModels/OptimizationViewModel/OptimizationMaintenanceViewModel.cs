using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Dv.App.Interfaces;
using Dv.App.Models;
using Dv.App.Services;
    
namespace Dv.App.ViewModels;

public sealed partial class OptimizationMaintenanceViewModel : ObservableObject
{
    private readonly MaintenanceService maintenanceService;
    private readonly IDialogService dialogService;

    [ObservableProperty]
    private OptimizationContext currentContext = OptimizationContextFactory.Create("Summer", "Scenario 1");

    [ObservableProperty]
    private ObservableCollection<BoilerStatusViewModel> boilers = new();

    [ObservableProperty]
    private BoilerStatusViewModel? selectedBoiler;

    [ObservableProperty]
    private DateTime? maintenanceStartDate = DateTime.Today;

    [ObservableProperty]
    private int maintenanceStartHour;

    [ObservableProperty]
    private int maintenanceDuration = 30;

    [ObservableProperty]
    private ObservableCollection<MaintenanceEvent> schedules = new();

    public bool HasSelectedBoiler => this.SelectedBoiler is not null;

    public OptimizationMaintenanceViewModel(MaintenanceService maintenanceService, IDialogService dialogService)
    {
        this.maintenanceService = maintenanceService;
        this.dialogService = dialogService;

        MaintenanceStore.MaintenanceSchedules.CollectionChanged += this.OnMaintenanceStoreChanged;
        this.ApplyContext(this.currentContext);
    }

    partial void OnSelectedBoilerChanged(BoilerStatusViewModel? value) =>
        this.OnPropertyChanged(nameof(this.HasSelectedBoiler));

    [RelayCommand]
    private async Task ScheduleMaintenanceAsync()
    {
        if (this.SelectedBoiler is null)
        {
            await this.dialogService.ShowValidationDialogAsync("Please select a boiler first.");
            return;
        }

        if (!this.MaintenanceStartDate.HasValue)
        {
            await this.dialogService.ShowValidationDialogAsync("Please completely fill out the Date/Time fields.");
            return;
        }

        if (!this.EnsureMaintenanceCapacityAvailable())
        {
            await this.dialogService.ShowValidationDialogAsync(
                $"The maximum amount of maintenances has been reached for the {this.CurrentContext.Period} period in {this.CurrentContext.Scenario}.");
            return;
        }

        var startDateTime = this.MaintenanceStartDate.Value.Date.AddHours(this.MaintenanceStartHour);
        var endDateTime = startDateTime.AddHours(this.MaintenanceDuration);

        if (startDateTime < this.CurrentContext.StartDate || endDateTime > this.CurrentContext.EndDate)
        {
            await this.dialogService.ShowValidationDialogAsync(
                $"Maintenance interval must be within the {this.CurrentContext.Period} period:\n{this.CurrentContext.StartDate:dd.MM.yyyy HH:mm} to {this.CurrentContext.EndDate:dd.MM.yyyy HH:mm}");
            return;
        }

        var confirmed = await this.dialogService.ShowConfirmationDialogAsync(
            this.CurrentContext.Period,
            this.SelectedBoiler.BoilerId,
            startDateTime,
            endDateTime,
            this.CurrentContext.StartDate,
            this.CurrentContext.EndDate);

        if (!confirmed)
        {
            return;
        }

        try
        {
            await this.maintenanceService.SaveMaintenanceAsync(
                this.SelectedBoiler.BoilerId,
                startDateTime,
                endDateTime,
                this.CurrentContext.Period,
                this.CurrentContext.Scenario);
        }
        catch (Exception ex)
        {
            await this.dialogService.ShowValidationDialogAsync($"Failed to save maintenance: {ex.Message}");
            return;
        }

        this.RefreshFromStore();
    }

    public void ApplyContext(OptimizationContext context)
    {
        this.CurrentContext = context;
        this.MaintenanceStartDate = context.StartDate.Date;
        this.MaintenanceStartHour = 0;
        this.MaintenanceDuration = 30;

        var previousBoilerId = this.SelectedBoiler?.BoilerId;
        this.Boilers = new ObservableCollection<BoilerStatusViewModel>(
            context.Boilers.Select(b => new BoilerStatusViewModel(b.BoilerId, b.FuelType, context.Period)));

        this.RefreshFromStore();

        this.SelectedBoiler = (!string.IsNullOrWhiteSpace(previousBoilerId)
            ? this.Boilers.FirstOrDefault(b => b.BoilerId == previousBoilerId)
            : null)
            ?? this.Boilers.FirstOrDefault();
    }

    private void OnMaintenanceStoreChanged(object? sender, NotifyCollectionChangedEventArgs e) =>
        this.RefreshFromStore();

    private void RefreshFromStore()
    {
        var periodId = this.CurrentContext.PeriodId.ToString();
        var scenarioId = this.CurrentContext.ScenarioId.ToString();

        this.Schedules = new ObservableCollection<MaintenanceEvent>(
            MaintenanceStore.MaintenanceSchedules
                .Where(s => s.Period == periodId && s.Scenario == scenarioId)
                .OrderBy(s => s.StartDate));

        foreach (var boiler in this.Boilers)
        {
            boiler.SetUnavailable(MaintenanceStore.MaintenanceSchedules.Any(s =>
                s.Period == periodId &&
                s.Scenario == scenarioId &&
                s.AssetName == boiler.BoilerId));
        }
    }

    private bool EnsureMaintenanceCapacityAvailable()
    {
        var periodId = this.CurrentContext.PeriodId.ToString();
        var scenarioId = this.CurrentContext.ScenarioId.ToString();

        return !MaintenanceStore.MaintenanceSchedules.Any(s =>
            s.Period == periodId && s.Scenario == scenarioId);
    }
}
