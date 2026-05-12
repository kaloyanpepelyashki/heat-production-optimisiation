using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using Dv.App.Models;
using Dv.App.Services;

namespace Dv.App.ViewModels;

public sealed partial class MainWindowViewModel : ViewModelBase
{
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

	public MainWindowViewModel()
	{
		_apiService = new ApiService();
		_wakeUpServicesTask = WakeUpServices(_startupCts.Token);
		 
		this.NavigationItems = new ObservableCollection<NavigationItem>
		{
			new NavigationItem { Title = "Dashboard", ViewKey = "dashboard" },
			new NavigationItem { Title = "Production Units", ViewKey = "production-units" },
			new NavigationItem { Title = "Source Data", ViewKey = "source-data" },
			new NavigationItem { Title = "Optimization", ViewKey = "optimization" },
			new NavigationItem { Title = "Settings", ViewKey = "settings" },
		};

		this.viewMap = new Dictionary<string, ViewModelBase>
		{
			["dashboard"] = new DashboardViewModel(),
			["production-units"] = new ProductionUnitsViewModel(),
			["source-data"] = new SourceDataViewModel(),
			["optimization"] = new OptimizationViewModel(),
			["settings"] = new SettingsViewModel(),
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
		}
		catch (Exception e)
		{
			Debug.WriteLine($"Error waking up all services {e.Message}, {e.GetType()}: {e.StackTrace}");
			_isWakingUp = false;
			_startUpError = e.Message;
		}
		finally
		{
			_isWakingUp = false;
			_allServicesWokeUp = true;
			Debug.WriteLine("All services woke up");
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
