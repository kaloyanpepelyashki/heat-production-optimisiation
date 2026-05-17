using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Dv.App.Interfaces;
using Dv.App.Models;
using Dv.App.Services;

namespace Dv.App.ViewModels;

public sealed class OptimizationViewModel : ViewModelBase
{
    private readonly IApiService apiService;
    private readonly SemaphoreSlim loadLock = new(1, 1);
    private readonly OptimizationChartsViewModel chartsModule;
    private readonly OptimizationMaintenanceViewModel maintenanceModule;

    private string selectedPeriod = "Winter";
    private string selectedScenario = "Scenario 2";
    private bool isLoading;
    private string? errorMessage;

    private OptimizationContext currentContext =
        OptimizationContextFactory.Create("Winter", "Scenario 2");

    public bool HasSelectedBoiler =>
        this.SelectedBoiler is not null;

    public OptimizationViewModel(
        IApiService apiService,
        MaintenanceService maintenanceService,
        IDialogService dialogService)
    {
        this.apiService = apiService;

        this.chartsModule = new OptimizationChartsViewModel();

        this.maintenanceModule =
            new OptimizationMaintenanceViewModel(
                maintenanceService,
                dialogService);

        this.CurrentContext =
            OptimizationContextFactory.Create(
                this.selectedPeriod,
                this.selectedScenario);

        this.chartsModule.PropertyChanged += this.ModuleOnPropertyChanged;
        this.maintenanceModule.PropertyChanged += this.ModuleOnPropertyChanged;

        this.RefreshCommand =
            new AsyncRelayCommand(this.LoadSourceDataAsync);

        this.RunOptimizationCommand =
            new AsyncRelayCommand(this.RunOptimizationAsync);

        this.maintenanceModule.ApplyContext(this.CurrentContext);
    }

    public ChartCardViewModel HeatDemandChart =>
        this.chartsModule.HeatDemandChart;

    public ChartCardViewModel ElectricityPriceChart =>
        this.chartsModule.ElectricityPriceChart;

    public ChartCardViewModel OptimizationResultsChart =>
        this.chartsModule.OptimizationResultsChart;

    public ChartCardViewModel ElectricityConsumptionChart =>
        this.chartsModule.ElectricityConsumptionChart;

    public ChartCardViewModel ExpensesChart =>
        this.chartsModule.ExpensesChart;

    public ChartCardViewModel Co2EmissionsChart =>
        this.chartsModule.Co2EmissionsChart;

    public ObservableCollection<string> Periods { get; } =
    [
        "Summer",
        "Winter",
    ];

    public ObservableCollection<string> Scenarios { get; } =
    [
        "Scenario 1",
        "Scenario 2",
    ];

    public string SelectedPeriod
    {
        get => this.selectedPeriod;
        set
        {
            if (this.SetProperty(ref this.selectedPeriod, value))
            {
                this.CurrentContext =
                    OptimizationContextFactory.Create(
                        this.SelectedPeriod,
                        this.SelectedScenario);

                this.maintenanceModule.ApplyContext(
                    this.CurrentContext);
            }
        }
    }

    public string SelectedScenario
    {
        get => this.selectedScenario;
        set
        {
            if (this.SetProperty(ref this.selectedScenario, value))
            {
                this.CurrentContext =
                    OptimizationContextFactory.Create(
                        this.SelectedPeriod,
                        this.SelectedScenario);

                this.maintenanceModule.ApplyContext(
                    this.CurrentContext);
            }
        }
    }

    public OptimizationContext CurrentContext
    {
        get => this.currentContext;
        private set
        {
            if (this.SetProperty(ref this.currentContext, value))
            {
                this.OnPropertyChanged(nameof(this.MaintenanceInstructions));
            }
        }
    }

    public ObservableCollection<BoilerStatusViewModel> Boilers =>
        this.maintenanceModule.Boilers;

    public BoilerStatusViewModel? SelectedBoiler
    {
        get => this.maintenanceModule.SelectedBoiler;
        set => this.maintenanceModule.SelectedBoiler = value;
    }

    public DateTime? MaintenanceStartDate
    {
        get => this.maintenanceModule.MaintenanceStartDate;
        set => this.maintenanceModule.MaintenanceStartDate = value;
    }

    public int MaintenanceStartHour
    {
        get => this.maintenanceModule.MaintenanceStartHour;
        set => this.maintenanceModule.MaintenanceStartHour = value;
    }

    public int MaintenanceDuration
    {
        get => this.maintenanceModule.MaintenanceDuration;
        set => this.maintenanceModule.MaintenanceDuration = value;
    }

    public ObservableCollection<MaintenanceEvent> Schedules =>
        this.maintenanceModule.Schedules;

    public string MaintenanceInstructions =>
        $"Click on a unit to mark it for maintenance. " +
        $"Select a time interval lasting 30-60 hours. " +
        $"The interval must stay within the selected " +
        $"{this.CurrentContext.Period} period " +
        $"({this.CurrentContext.StartDate:dd.MM.yyyy HH:mm} " +
        $"to {this.CurrentContext.EndDate:dd.MM.yyyy HH:mm}).";

    public bool IsLoading
    {
        get => this.isLoading;
        private set => this.SetProperty(ref this.isLoading, value);
    }

    public string? ErrorMessage
    {
        get => this.errorMessage;
        private set => this.SetProperty(ref this.errorMessage, value);
    }

    public IAsyncRelayCommand RefreshCommand { get; }

    public IAsyncRelayCommand RunOptimizationCommand { get; }

    public ICommand ScheduleMaintenanceCommand =>
        this.maintenanceModule.ScheduleMaintenanceCommand;

    private async Task LoadSourceDataAsync()
    {
        await this.loadLock.WaitAsync();

        try
        {
            this.IsLoading = true;
            this.ErrorMessage = null;

            List<SourceDataDto> sourceData = [];

            try
            {
                sourceData =
                    await this.apiService.GetAsync<List<SourceDataDto>>(
                        BackendService.Sdm,
                        "getAll")
                    ?? [];
            }
            catch (Exception ex)
            {
                this.ErrorMessage =
                    $"Failed to load source data: {ex.Message}";
            }

            this.chartsModule.LoadSourceData(
                sourceData,
                this.CurrentContext);
        }
        catch (Exception ex)
        {
            this.ErrorMessage = ex.Message;
        }
        finally
        {
            this.IsLoading = false;
            this.loadLock.Release();
        }
    }

    private async Task RunOptimizationAsync()
    {
        try
        {
            this.IsLoading = true;
            this.ErrorMessage = null;

            var request = this.BuildOptimizationRequest();

            var response =
                await this.apiService.PostAsync<
                    OptimizationRequestDto,
                    ApiResponseModel<OptimisationRunDto>>(
                        BackendService.Rdm,
                        "optimisation",
                        request);

            if (response?.Data is null)
            {
                this.ErrorMessage =
                    "Optimization returned no data.";

                return;
            }

            this.chartsModule.LoadOptimizationResult(
                response.Data,
                this.CurrentContext);
        }
        catch (Exception ex)
        {
            this.ErrorMessage = ex.Message;
        }
        finally
        {
            this.IsLoading = false;
        }
    }

    private OptimizationRequestDto BuildOptimizationRequest()
    {
        var periodId = this.CurrentContext.PeriodId.ToString();
        var scenarioId = this.CurrentContext.ScenarioId.ToString();

        var schedule = MaintenanceStore.MaintenanceSchedules
            .FirstOrDefault(s => s.Period == periodId && s.Scenario == scenarioId);

        var maintenanceId = schedule?.MaintenanceId ?? 0;

        return new OptimizationRequestDto
        {
            ScenarioId = this.CurrentContext.ScenarioId,
            PeriodId = this.CurrentContext.PeriodId,
            MaintenanceId = maintenanceId,
            TimeFrom = this.CurrentContext.StartDate,
            TimeTo = this.CurrentContext.EndDate,
        };
    }

    private void ModuleOnPropertyChanged(
        object? sender,
        System.ComponentModel.PropertyChangedEventArgs e)
    {
        if (string.IsNullOrWhiteSpace(e.PropertyName))
        {
            return;
        }

        this.OnPropertyChanged(e.PropertyName);
    }
}