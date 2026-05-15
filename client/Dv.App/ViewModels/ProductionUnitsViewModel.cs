using System;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using Dv.App.Models;
using Dv.App.Services;
using Microsoft.Extensions.Logging;
using Dv.App.Interfaces;

namespace Dv.App.ViewModels;

public sealed partial class ProductionUnitsViewModel : ViewModelBase
{
    private readonly ILogger<ProductionUnitsViewModel> _logger;
    
    private readonly IApiService apiService;
    private readonly CancellationTokenSource _ct = new CancellationTokenSource();
    
    
    private string productionData = "Data will appear here once loaded...";
   
    

    public Task InitializationTask { get; private set; }
    private readonly Task _wakingUpTask;

    [ObservableProperty] 
    private bool _isServiceWakingUp;
    [ObservableProperty] private bool _serviceWokeUp;
    
    // Gets data from MaintenanceStore.cs to make it visible in UI
    public ObservableCollection<MaintenanceEvent> MaintenanceSchedules => MaintenanceStore.MaintenanceSchedules;

    public ProductionUnitsViewModel(IApiService apiService, ILogger<ProductionUnitsViewModel> logger)
    {
        _logger = logger;
        
        this.apiService = apiService;
        _wakingUpTask = WakeUpService(_ct.Token);
        this.InitializationTask = this.LoadProductionDataAsync();
    }

    public string ProductionData
    {
        get => this.productionData;
        set => this.SetProperty(ref this.productionData, value);
    }

    private async Task WakeUpService(CancellationToken token)
    {
        _isServiceWakingUp = true;
        try
        {
            var response = await apiService.WakeUpService(BackendService.Am, token);

            if (!response)
            {
                _isServiceWakingUp = false;
                _serviceWokeUp = false;
            }
            
        }
        catch (OperationCanceledException)
        {
            _isServiceWakingUp = false;
            _serviceWokeUp = false;
            Debug.WriteLine("Waking up service in ProductionUnitsViewModel process cancelled");
            _logger.LogError("Waking up service in ProductionUnitsViewModel process cancelled");
        }
        catch (Exception e)
        {
            _isServiceWakingUp = false;
            _serviceWokeUp = false;
            Debug.WriteLine("Error waking up service in ProductionUnitsViewModel");
            _logger.LogError($"Error waking up service in ProductionUnitsViewModel: {e.Message}");
        }
        finally
        {
            _isServiceWakingUp = false;
            _serviceWokeUp = true;
            Debug.WriteLine("Woke up service in ProductionUnitsViewModel successfully");
        }
    }

    private async Task LoadProductionDataAsync()
    {
        try
        {
            this.ProductionData = $"Pinging AM Render services...\n(Please wait, Render free tier can take up to 50s to wake up)";
            var response = await this.apiService.GetAsync<object>(BackendService.Am, "api/GetProductionUnits/allGasBoilers");
            this.ProductionData = $"Success! AM API responded. Data parsed: {response != null}";
        }
        catch (Exception ex)
        {
            this.ProductionData = $"Failed to fetch AM data: {ex.Message}";
        }
    }
}
