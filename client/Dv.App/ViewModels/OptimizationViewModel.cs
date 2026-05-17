using Microsoft.Extensions.Logging;

namespace Dv.App.ViewModels;

using System;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using Dv.App.Services;
using Dv.App.Models;
using Dv.App.Interfaces;   

// creates the four period view models / wires the optimization screen based on the period data
public sealed partial class OptimizationViewModel : ViewModelBase
{
    private readonly IApiService _apiService;
    private readonly ILogger<OptimizationViewModel> _logger;
    
    private readonly MaintenanceService maintenanceService;
    private readonly IDialogService dialogService;

    [ObservableProperty]
    private MaintenancePeriodViewModel summerScenario1;

    [ObservableProperty]
    private MaintenancePeriodViewModel winterScenario1;

    [ObservableProperty]
    private MaintenancePeriodViewModel summerScenario2;

    [ObservableProperty]
    private MaintenancePeriodViewModel winterScenario2;
    
    
    

    // building the four period panels/views exposing them to the view
    public OptimizationViewModel(IApiService apiService, ILogger<OptimizationViewModel> logger)
    {
        _logger = logger;
        
        _apiService = apiService;
        
        this.maintenanceService = new MaintenanceService();
        this.dialogService = new DialogService();

        this.summerScenario1 = new MaintenancePeriodViewModel(
            this.maintenanceService,
            this.dialogService,
            "Summer",
            "1",
            new DateTime(2025, 9, 8, 0, 0, 0),
            new DateTime(2025, 9, 21, 23, 59, 59),
            this.CreateBoilerRowsScenario1("Summer"));

        this.winterScenario1 = new MaintenancePeriodViewModel(
            this.maintenanceService,
            this.dialogService,
            "Winter",
            "1",
            new DateTime(2026, 1, 5, 0, 0, 0),
            new DateTime(2026, 1, 18, 23, 59, 59),
            this.CreateBoilerRowsScenario1("Winter"));

        this.summerScenario2 = new MaintenancePeriodViewModel(
            this.maintenanceService,
            this.dialogService,
            "Summer",
            "2",
            new DateTime(2025, 9, 8, 0, 0, 0),
            new DateTime(2025, 9, 21, 23, 59, 59),
            this.CreateBoilerRowsScenario2("Summer"));

        this.winterScenario2 = new MaintenancePeriodViewModel(
            this.maintenanceService,
            this.dialogService,
            "Winter",
            "2",
            new DateTime(2026, 1, 5, 0, 0, 0),
            new DateTime(2026, 1, 18, 23, 59, 59),
            this.CreateBoilerRowsScenario2("Winter"));
    }

    // boiler list for scenario 1
    private ObservableCollection<BoilerStatusViewModel> CreateBoilerRowsScenario1(string period)
    {
        return new ObservableCollection<BoilerStatusViewModel>
        {
            new BoilerStatusViewModel("GB1", "Gas", period),
            new BoilerStatusViewModel("GB2", "Gas", period),
            new BoilerStatusViewModel("GB3", "Gas", period),
            new BoilerStatusViewModel("OB1", "Oil", period),
        };
    }

    // boiler list for scenario 2
    private ObservableCollection<BoilerStatusViewModel> CreateBoilerRowsScenario2(string period)
    {
        return new ObservableCollection<BoilerStatusViewModel>
        {
            new BoilerStatusViewModel("GB1", "Gas", period),
            new BoilerStatusViewModel("GB2", "Gas", period),
            new BoilerStatusViewModel("GM1", "Gas", period),
            new BoilerStatusViewModel("EB1", "Electric", period),
        };
    }
}
