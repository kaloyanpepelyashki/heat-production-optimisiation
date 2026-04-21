namespace Dv.App.ViewModels;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dv.App.Models;
using Dv.App.Services;

public sealed class DashboardViewModel : ViewModelBase
{
    private readonly IApiService apiService;
    private string dashboardData = "Data will appear here once loaded...";

    public Task InitializationTask { get; private set; }

    public DashboardViewModel(IApiService apiService = null!)
    {
        this.apiService = apiService ?? new ApiService();
        this.InitializationTask = this.LoadDashboardDataAsync();
    }

    public string DashboardData
    {
        get => this.dashboardData;
        set => this.SetProperty(ref this.dashboardData, value);
    }

    private async Task LoadDashboardDataAsync()
    {
        try
        {
            this.DashboardData = $"Pinging Render services...{Environment.NewLine}(Please wait, Render free tier can take up to 50s to wake up)";

            var sourceData = await this.apiService.GetAsync<List<SourceDataDto>>(BackendService.Sdm, "getAll");

            if (sourceData != null && sourceData.Any())
            {
                var first = sourceData.First();
                this.DashboardData = $"Success! SDM API responded. First Data entry: Heat Demand {first.HeatDemand}, Electricity Price {first.ElectricityPrice} from {first.TimeFrom}";
            }
            else
            {
                this.DashboardData = "Success! But SDM API returned an empty array.";
            }
        }
        catch (Exception ex)
        {
            this.DashboardData = $"Test Failed: {ex.Message} (Check if the Render URL is correct and the service is deployed)";
        }
    }
}
