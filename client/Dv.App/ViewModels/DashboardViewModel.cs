// <copyright file="DashboardViewModel.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

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

    // Allow parameterless initialization for now if desired, but ideally we inject it
    public DashboardViewModel(IApiService apiService = null!)
    {
        this.apiService = apiService ?? new ApiService();

        // Let's actually test it when the view model is created
        _ = this.LoadDashboardDataAsync();
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

            // Testing our Data Retrieval Layer by pinging the actual SDM service Render deployment
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
