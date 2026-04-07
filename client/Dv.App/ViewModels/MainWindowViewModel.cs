using System.Collections.Generic;
using System.Collections.ObjectModel;
using Dv.App.Models;

namespace Dv.App.ViewModels;

public sealed class MainWindowViewModel : ViewModelBase
{
	private readonly Dictionary<string, ViewModelBase> viewMap;
	private ViewModelBase currentViewModel;
	private NavigationItem? selectedNavigationItem;

	public MainWindowViewModel()
	{
		this.NavigationItems = new ObservableCollection<NavigationItem>
		{
			new NavigationItem { Title = "Dashboard", ViewKey = "dashboard" },
			new NavigationItem { Title = "Production Units", ViewKey = "production-units" },
			new NavigationItem { Title = "Source Data", ViewKey = "source-data" },
			new NavigationItem { Title = "Optimization", ViewKey = "optimization" },
			new NavigationItem { Title = "Result Data Manager", ViewKey = "result-data-manager" },
			new NavigationItem { Title = "Settings", ViewKey = "settings" },
		};

		this.viewMap = new Dictionary<string, ViewModelBase>
		{
			["dashboard"] = new DashboardViewModel(),
			["production-units"] = new ProductionUnitsViewModel(),
			["source-data"] = new SourceDataViewModel(),
			["optimization"] = new OptimizationViewModel(),
			["result-data-manager"] = new ResultDataManagerViewModel(),
			["settings"] = new SettingsViewModel(),
		};

		this.currentViewModel = this.viewMap["dashboard"];
		this.selectedNavigationItem = this.NavigationItems[0];
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
