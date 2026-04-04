using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dv.App.Services;

namespace Dv.App.ViewModels;

public class WeatherForecast
{
    public DateTime Date { get; set; }
    public int TemperatureC { get; set; }
    public int TemperatureF { get; set; }
    public string? Summary { get; set; }
}

public sealed class DashboardViewModel : ViewModelBase
{
    private readonly IApiService _apiService;
    private string _dashboardData = "Data will appear here once loaded...";

    public string DashboardData
    {
        get => _dashboardData;
        set => this.SetProperty(ref _dashboardData, value);
    }
    
    // Allow parameterless initialization for now if desired, but ideally we inject it
    public DashboardViewModel(IApiService apiService = null!)
    {
        _apiService = apiService ?? new ApiService();
        
        // Let's actually test it when the view model is created
        _ = LoadDashboardDataAsync();
    }

    private async Task LoadDashboardDataAsync()
    {
        try
        {
            DashboardData = $"Pinging Render services...{Environment.NewLine}(Please wait, Render free tier can take up to 50s to wake up)";
            
            // Testing our Data Retrieval Layer by pinging the Opt service Render deployment
            var forecasts = await _apiService.GetAsync<List<WeatherForecast>>(BackendService.Opt, "WeatherForecast");
            
            if (forecasts != null && forecasts.Any())
            {
                var first = forecasts.First();
                DashboardData = $"Success! Render responded. First forecast: {first.Summary} ({first.TemperatureC}°C) on {first.Date.ToShortDateString()}";
            }
            else
            {
                DashboardData = "Success! But Render returned an empty array.";
            }
        }
        catch (Exception ex)
        {
            DashboardData = $"Test Failed: {ex.Message} (Check if the Render URL is correct and the service is deployed)";
        }
    }
}
