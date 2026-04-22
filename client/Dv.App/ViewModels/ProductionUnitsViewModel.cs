using System;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using Dv.App.Models;
using Dv.App.Services;

namespace Dv.App.ViewModels;

public sealed class ProductionUnitsViewModel : ViewModelBase
{
    private readonly IApiService apiService;
    private string productionData = "Data will appear here once loaded...";

    public Task InitializationTask { get; private set; }

    // Gets data from MaintenanceStore.cs to make it visible in UI
    public ObservableCollection<MaintenanceEvent> MaintenanceSchedules => MaintenanceStore.MaintenanceSchedules;

    public ProductionUnitsViewModel(IApiService apiService = null!)
    {
        this.apiService = apiService ?? new ApiService();
        this.InitializationTask = this.LoadProductionDataAsync();
    }

    public string ProductionData
    {
        get => this.productionData;
        set => this.SetProperty(ref this.productionData, value);
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
