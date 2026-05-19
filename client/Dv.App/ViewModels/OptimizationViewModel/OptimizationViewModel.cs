using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Dv.App.Interfaces;
using Dv.App.Models;
using Dv.App.Services;

namespace Dv.App.ViewModels;

public sealed partial class OptimizationViewModel : ViewModelBase
{
    private readonly IApiService apiService;
    private readonly SemaphoreSlim loadLock = new(1, 1);
    private List<SourceDataDto> cachedSourceData = [];

    [ObservableProperty]
    private string selectedPeriod = "Winter";

    [ObservableProperty]
    private string selectedScenario = "Scenario 2";

    private OptimizationContext currentContext;
    private bool isLoading;
    private string? errorMessage;

    public OptimizationViewModel(
        IApiService apiService,
        MaintenanceService maintenanceService,
        IDialogService dialogService)
    {
        this.apiService = apiService;
        this.currentContext = OptimizationContextFactory.Create(this.selectedPeriod, this.selectedScenario);

        this.Maintenance = new OptimizationMaintenanceViewModel(maintenanceService, dialogService);
        this.Maintenance.ApplyContext(this.currentContext);

        MaintenanceStore.MaintenanceSchedules.CollectionChanged += (_, _) =>
            this.RunOptimizationCommand.NotifyCanExecuteChanged();
    }

    public OptimizationChartsViewModel ChartsVM { get; } = new();

    public OptimizationMaintenanceViewModel Maintenance { get; }

    public ObservableCollection<string> Periods { get; } = ["Summer", "Winter"];

    public ObservableCollection<string> Scenarios { get; } = ["Scenario 1", "Scenario 2"];

    public OptimizationContext CurrentContext
    {
        get => this.currentContext;
        private set
        {
            if (this.SetProperty(ref this.currentContext, value))
            {
                this.OnPropertyChanged(nameof(this.MaintenanceInstructions));
                this.RunOptimizationCommand.NotifyCanExecuteChanged();
            }
        }
    }

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

    public string MaintenanceInstructions =>
        $"Click on a unit to mark it for maintenance. " +
        $"Select a time interval lasting 30-60 hours. " +
        $"The interval must stay within the selected " +
        $"{this.CurrentContext.Period} period " +
        $"({this.CurrentContext.StartDate:dd.MM.yyyy HH:mm} " +
        $"to {this.CurrentContext.EndDate:dd.MM.yyyy HH:mm}).";

    partial void OnSelectedPeriodChanged(string value)
    {
        this.CurrentContext = OptimizationContextFactory.Create(value, this.SelectedScenario);
        this.RefreshSourceCharts();
        this.Maintenance.ApplyContext(this.CurrentContext);
    }

    partial void OnSelectedScenarioChanged(string value)
    {
        this.CurrentContext = OptimizationContextFactory.Create(this.SelectedPeriod, value);
        this.RefreshSourceCharts();
        this.Maintenance.ApplyContext(this.CurrentContext);
    }

    [RelayCommand]
    private async Task RefreshAsync()
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
                this.ErrorMessage = $"Failed to load source data: {ex.Message}";
            }

            this.cachedSourceData = sourceData;
            this.ChartsVM.LoadSourceData(this.cachedSourceData, this.CurrentContext);
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

    [RelayCommand(CanExecute = nameof(CanRunOptimization))]
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
                this.ErrorMessage = "Optimization returned no data.";
                return;
            }

            this.ChartsVM.LoadOptimizationResult(response.Data, this.CurrentContext);
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

    private bool CanRunOptimization()
    {
        var periodId = this.CurrentContext.PeriodId.ToString();
        var scenarioId = this.CurrentContext.ScenarioId.ToString();

        return MaintenanceStore.MaintenanceSchedules.Any(s =>
            s.Period == periodId && s.Scenario == scenarioId);
    }

    private OptimizationRequestDto BuildOptimizationRequest()
    {
        var periodId = this.CurrentContext.PeriodId.ToString();
        var scenarioId = this.CurrentContext.ScenarioId.ToString();

        var schedule = MaintenanceStore.MaintenanceSchedules
            .FirstOrDefault(s => s.Period == periodId && s.Scenario == scenarioId);

        return new OptimizationRequestDto
        {
            ScenarioId = this.CurrentContext.ScenarioId,
            PeriodId = this.CurrentContext.PeriodId,
            MaintenanceId = schedule?.MaintenanceId ?? 0,
            TimeFrom = this.CurrentContext.StartDate,
            TimeTo = this.CurrentContext.EndDate,
        };
    }

    private void RefreshSourceCharts()
    {
        if (this.cachedSourceData.Count == 0)
        {
            return;
        }

        this.ChartsVM.LoadSourceData(this.cachedSourceData, this.CurrentContext);
    }
}
