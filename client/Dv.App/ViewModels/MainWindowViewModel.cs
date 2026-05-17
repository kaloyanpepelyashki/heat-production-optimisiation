using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using Dv.App.Models;
using Dv.App.Services;
using Microsoft.Extensions.Logging;
using Dv.App.Interfaces;

namespace Dv.App.ViewModels;

public sealed partial class MainWindowViewModel : ViewModelBase
{
	private readonly ILogger<MainWindowViewModel> _logger;
	
	private readonly IApiService _apiService; 
	
	private readonly Dictionary<string, ViewModelBase> viewMap;
	private ViewModelBase currentViewModel;
	private NavigationItem? selectedNavigationItem;
	
	private readonly CancellationTokenSource _startupCts = new CancellationTokenSource();
	private readonly Task _wakeUpServicesTask;


	[ObservableProperty] 
	private bool _isWakingUp;
	[ObservableProperty]
	private bool _allServicesWokeUp;

	[ObservableProperty] 
	private string _startUpError;

	public MainWindowViewModel(IApiService apiService, ILogger<MainWindowViewModel> logger, 
		DashboardViewModel dashboardViewModel,
		ProductionUnitsViewModel productionUnitsViewModel,
		OptimizationViewModel optimizationViewModel,
		SettingsViewModel settingsViewModel)
	{
		_logger = logger;
		
		_apiService = apiService;
		_wakeUpServicesTask = WakeUpServices(_startupCts.Token);
		 
		this.NavigationItems = new ObservableCollection<NavigationItem>
		{
			new NavigationItem { Title = "Dashboard", ViewKey = "dashboard" },
			new NavigationItem { Title = "Production Units", ViewKey = "production-units" },
			new NavigationItem { Title = "Optimization", ViewKey = "optimization" },
			new NavigationItem { Title = "Settings", ViewKey = "settings" },
		};

		this.viewMap = new Dictionary<string, ViewModelBase>
		{
			["dashboard"] = dashboardViewModel,
			["production-units"] = productionUnitsViewModel,
			["optimization"] = optimizationViewModel,
			["settings"] = settingsViewModel,
		};

		this.currentViewModel = this.viewMap["dashboard"];
		this.selectedNavigationItem = this.NavigationItems[0];
	}
	
	
	//Calls the WakeUpAllServices from the ApiClient, to send a health check to all the services and wake them up
	private async Task WakeUpServices(CancellationToken ct)
	{
		_isWakingUp = true;
		try
		{
			var response = await _apiService.WakeUpAllServices(ct);

			if (!response)
			{
				_allServicesWokeUp = false;
				_isWakingUp = false;
			}
		}
		catch (OperationCanceledException)
		{
			_allServicesWokeUp = false;
			_startUpError = "Waking up services process cancelled";
			_logger.LogError("Failed to wake up all services in MainWindowViewModel. Waking up services process cancelled");
		}
		catch (Exception e)
		{
			_isWakingUp = false;
			_startUpError = e.Message;
			Debug.WriteLine($"Error waking up all services {e.Message}, {e.GetType()}: {e.StackTrace}");
			_logger.LogError($"Error waking up all services {e.Message}, {e.GetType()}: {e.StackTrace}");
		}
		finally
		{
			_isWakingUp = false;
			_allServicesWokeUp = true;
			Debug.WriteLine("All services woke up successfully in MainWindowViewModel");
			_logger.LogInformation("All services woke up successfully in MainWindowViewModel");
		}
	}

	public ObservableCollection<NavigationItem> NavigationItems { get; }

	public NavigationItem? SelectedNavigationItem
	{
		get => this.selectedNavigationItem;
		set
		{
			if (!this.SetProperty(ref this.selectedNavigationItem, value) || value is null)
			{
				return;
			}

			this.NavigateTo(value.ViewKey);
		}
	}

	public ViewModelBase CurrentViewModel
	{
		get => this.currentViewModel;
		private set => this.SetProperty(ref this.currentViewModel, value);
	}

	private void NavigateTo(string viewKey)
	{
		if (this.viewMap.TryGetValue(viewKey, out var targetViewModel))
		{
			this.CurrentViewModel = targetViewModel;
		}
	}
}
