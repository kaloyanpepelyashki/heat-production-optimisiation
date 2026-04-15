// <copyright file="OptimizationViewModel.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace Dv.App.ViewModels;

using System;
using System.Threading.Tasks;
using Dv.App.Services;

public sealed class OptimizationViewModel : ViewModelBase
{
    private readonly IApiService apiService;
    private string optData = "Data will appear here once loaded...";

    public OptimizationViewModel(IApiService apiService = null!)
    {
        this.apiService = apiService ?? new ApiService();
        _ = this.LoadOptDataAsync();
    }

    public string OptData
    {
        get => this.optData;
        set => this.SetProperty(ref this.optData, value);
    }

    private async Task LoadOptDataAsync()
    {
        try
        {
            this.OptData = $"Pinging OPT Render services...{Environment.NewLine}(Please wait, Render free tier can take up to 50s to wake up)";
            var response = await this.apiService.GetAsync<object>(BackendService.Opt, "WeatherForecast");
            this.OptData = $"Success! OPT API responded. Data parsed: {response != null}";
        }
        catch (Exception ex)
        {
            this.OptData = $"Failed to fetch OPT data: {ex.Message}";
        }
    }
}
