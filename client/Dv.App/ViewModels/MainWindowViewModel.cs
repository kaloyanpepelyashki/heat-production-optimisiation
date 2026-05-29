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
	private readonly SettingsViewModel _settingsViewModel;

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
		_settingsViewModel = settingsViewModel;
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
	

	private async Task WakeUpServices(CancellationToken ct)
	{
		IsWakingUp = true;
		StartUpError = string.Empty;
		try
		{
			var response = await _apiService.WakeUpAllServices(ct);

			if (!response)
			{
				AllServicesWokeUp = false;
				StartUpError = "One or more services failed to wake up.";
			}
			else
			{
				AllServicesWokeUp = true;
			}
		}
		catch (OperationCanceledException) when (ct.IsCancellationRequested)
		{
			AllServicesWokeUp = false;
			StartUpError = "Service wakeup was cancelled.";
			_logger.LogError("Failed to wake up all services in MainWindowViewModel. Waking up services process cancelled");
		}
		catch (Exception e)
		{
			AllServicesWokeUp = false;
			StartUpError = e.Message;
			_logger.LogError($"Error waking up all services {e.Message}, {e.GetType()}: {e.StackTrace}");
		}
		finally
		{
			IsWakingUp = false;
			_settingsViewModel.RefreshServices();
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
