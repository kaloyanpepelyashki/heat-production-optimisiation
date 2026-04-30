using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Layout;
using LiveChartsCore;
using LiveChartsCore.Defaults;
using LiveChartsCore.Kernel.Sketches;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Avalonia;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;
using Dv.App.Models;
using Dv.App.Services;
using Dv.App.ViewModels;

namespace Dv.App.Views;

public partial class OptimizationView : UserControl
{
    private readonly IApiService apiService;
    private readonly MaintenanceService maintenanceService;
    private readonly ObservableCollection<BoilerStatusViewModel> summerBoilersScenario1;
    private readonly ObservableCollection<BoilerStatusViewModel> winterBoilersScenario1;
    private readonly ObservableCollection<BoilerStatusViewModel> summerBoilersScenario2;
    private readonly ObservableCollection<BoilerStatusViewModel> winterBoilersScenario2;

    public OptimizationView()
    {
        this.InitializeComponent();
        this.apiService = new ApiService();
        this.maintenanceService = new MaintenanceService();
        this.summerBoilersScenario1 = this.CreateBoilerRows("Summer");
        this.winterBoilersScenario1 = this.CreateBoilerRows("Winter");
        this.summerBoilersScenario2 = this.CreateBoilerRowsScenario2("Summer");
        this.winterBoilersScenario2 = this.CreateBoilerRowsScenario2("Winter");
        
        this.BoilersListBoxSummerScenario1.ItemsSource = this.summerBoilersScenario1;
        this.BoilersListBoxWinterScenario1.ItemsSource = this.winterBoilersScenario1;
        
        this.BoilersListBoxSummerScenario2.ItemsSource = this.summerBoilersScenario2;
        this.BoilersListBoxWinterScenario2.ItemsSource = this.winterBoilersScenario2;
        this.MaintenanceScheduleItemsControlSummerScenario2.ItemsSource = MaintenanceStore.MaintenanceSchedules;
        this.MaintenanceScheduleItemsControlWinterScenario2.ItemsSource = MaintenanceStore.MaintenanceSchedules;
        
        this.MaintenanceScheduleItemsControlSummerScenario1.ItemsSource = MaintenanceStore.MaintenanceSchedules;
        this.MaintenanceScheduleItemsControlWinterScenario1.ItemsSource = MaintenanceStore.MaintenanceSchedules;
        MaintenanceStore.MaintenanceSchedules.CollectionChanged += this.MaintenanceSchedules_CollectionChanged;
        this.UpdateBoilerStatuses();
        _ = this.LoadSeasonChartsAsync();
    }

    private async void SelectMaintenanceSummerScenario1_Click(object? sender, RoutedEventArgs e)
    {
        var selectedItem = this.BoilersListBoxSummerScenario1.SelectedItem as BoilerStatusViewModel;
        string boilerId = selectedItem?.BoilerId ?? "No Boiler Selected";

        if (!await this.EnsureMaintenanceCapacityAvailableAsync("Summer", "1"))
        {
            return;
        }

        var startDate = MaintenanceStartDatePickerSummerScenario1.SelectedDate;
        var startHour = MaintenanceStartHourSummerScenario1.Value;

        if (startDate.HasValue && startHour.HasValue)
        {
            var startDateTime = startDate.Value.Date.AddHours((double)startHour.Value);
            double durationHours = MaintenanceDurationSliderSummerScenario1.Value;
            var endDateTime = startDateTime.AddHours(durationHours);

            // YYYY/MM/DD + H/MM
            var periodStart = new DateTime(2025, 9, 8, 0, 0, 0);
            var periodEnd = new DateTime(2025, 9, 21, 23, 59, 59);

            if (startDateTime < periodStart || endDateTime > periodEnd)
            {
                await ShowValidationDialog($"Maintenance interval must be within the Summer period:\n{periodStart:dd.MM.yyyy HH:mm} to {periodEnd:dd.MM.yyyy HH:mm}");
                return;
            }

            var confirmed = await ShowMaintenanceConfirmationDialog(
                "Summer",
                boilerId,
                startDateTime,
                endDateTime,
                periodStart,
                periodEnd);

            if (confirmed)
            {
                var selectedScenario = this.GetSelectedScenario();
                this.maintenanceService.SaveMaintenance(boilerId, startDateTime, endDateTime, "Summer", selectedScenario);
            
            }
        }
        else
        {
            await ShowValidationDialog("Please completely fill out the Date/Time fields.");
        }
    }

    private async void SelectMaintenanceWinterScenario1_Click(object? sender, RoutedEventArgs e)
    {
        var selectedItem = this.BoilersListBoxWinterScenario1.SelectedItem as BoilerStatusViewModel;
        string boilerId = selectedItem?.BoilerId ?? "No Boiler Selected";

        if (!await this.EnsureMaintenanceCapacityAvailableAsync("Winter", "1"))
        {
            return;
        }

        var startDate = MaintenanceStartDatePickerWinterScenario1.SelectedDate;
        var startHour = MaintenanceStartHourWinterScenario1.Value;

        if (startDate.HasValue && startHour.HasValue)
        {
            var startDateTime = startDate.Value.Date.AddHours((double)startHour.Value);
            double durationHours = MaintenanceDurationSliderWinterScenario1.Value;
            var endDateTime = startDateTime.AddHours(durationHours);

            // YYYY/MM/DD + H/MM
            var periodStart = new DateTime(2026, 1, 5, 0, 0, 0);
            var periodEnd = new DateTime(2026, 1, 18, 23, 59, 59);

            if (startDateTime < periodStart || endDateTime > periodEnd)
            {
                await ShowValidationDialog($"Maintenance interval must be within the Winter period:\n{periodStart:dd.MM.yyyy HH:mm} to {periodEnd:dd.MM.yyyy HH:mm}");
                return;
            }

            var confirmed = await ShowMaintenanceConfirmationDialog(
                "Winter",
                boilerId,
                startDateTime,
                endDateTime,
                periodStart,
                periodEnd);

            if (confirmed)
            {
                var selectedScenario = this.GetSelectedScenario();
                this.maintenanceService.SaveMaintenance(boilerId, startDateTime, endDateTime, "Winter", selectedScenario);
            }
        }
        else
        {
            await ShowValidationDialog("Please completely fill out the Date/Time fields.");
        }
    }

    private async void SelectMaintenanceSummerScenario2_Click(object? sender, RoutedEventArgs e)
    {
        var selectedItem = this.BoilersListBoxSummerScenario2.SelectedItem as BoilerStatusViewModel;
        string boilerId = selectedItem?.BoilerId ?? "No Boiler Selected";

        if (!await this.EnsureMaintenanceCapacityAvailableAsync("Summer", "2"))
        {
            return;
        }

        var startDate = MaintenanceStartDatePickerSummerScenario2.SelectedDate;
        var startHour = MaintenanceStartHourSummerScenario2.Value;

        if (startDate.HasValue && startHour.HasValue && MaintenanceDurationSliderSummerScenario2 != null)
        {
            var startDateTime = startDate.Value.Date.AddHours((double)startHour.Value);
            double durationHours = MaintenanceDurationSliderSummerScenario2.Value;
            var endDateTime = startDateTime.AddHours(durationHours);

            var periodStart = new DateTime(2025, 9, 8, 0, 0, 0);
            var periodEnd = new DateTime(2025, 9, 21, 23, 59, 59);

            if (startDateTime < periodStart || endDateTime > periodEnd)
            {
                await ShowValidationDialog($"Maintenance interval must be within the Summer period:\n{periodStart:dd.MM.yyyy HH:mm} to {periodEnd:dd.MM.yyyy HH:mm}");
                return;
            }

            var confirmed = await ShowMaintenanceConfirmationDialog(
                "Summer",
                boilerId,
                startDateTime,
                endDateTime,
                periodStart,
                periodEnd);

            if (confirmed)
            {
                var selectedScenario = this.GetSelectedScenario();
                this.maintenanceService.SaveMaintenance(boilerId, startDateTime, endDateTime, "Summer", selectedScenario);
            }
        }
        else
        {
            await ShowValidationDialog("Please completely fill out the Date/Time fields.");
        }
    }

    private async void SelectMaintenanceWinterScenario2_Click(object? sender, RoutedEventArgs e)
    {
        var selectedItem = this.BoilersListBoxWinterScenario2.SelectedItem as BoilerStatusViewModel;
        string boilerId = selectedItem?.BoilerId ?? "No Boiler Selected";

        if (!await this.EnsureMaintenanceCapacityAvailableAsync("Winter", "2"))
        {
            return;
        }

        var startDate = MaintenanceStartDatePickerWinterScenario2.SelectedDate;
        var startHour = MaintenanceStartHourWinterScenario2.Value;

        if (startDate.HasValue && startHour.HasValue && MaintenanceDurationSliderSummerScenario2 != null)
        {
            var startDateTime = startDate.Value.Date.AddHours((double)startHour.Value);
            double durationHours = MaintenanceDurationSliderSummerScenario2.Value;
            var endDateTime = startDateTime.AddHours(durationHours);

            var periodStart = new DateTime(2026, 1, 5, 0, 0, 0);
            var periodEnd = new DateTime(2026, 1, 18, 23, 59, 59);

            if (startDateTime < periodStart || endDateTime > periodEnd)
            {
                await ShowValidationDialog($"Maintenance interval must be within the Winter period:\n{periodStart:dd.MM.yyyy HH:mm} to {periodEnd:dd.MM.yyyy HH:mm}");
                return;
            }

            var confirmed = await ShowMaintenanceConfirmationDialog(
                "Winter",
                boilerId,
                startDateTime,
                endDateTime,
                periodStart,
                periodEnd);

            if (confirmed)
            {
                var selectedScenario = this.GetSelectedScenario();
                this.maintenanceService.SaveMaintenance(boilerId, startDateTime, endDateTime, "Winter", selectedScenario);
            }
        }
        else
        {
            await ShowValidationDialog("Please completely fill out the Date/Time fields.");
        }
    }

    private string GetSelectedScenario()
    {
        var selectedScenarioItem = this.ScenarioComboBox.SelectedItem as ComboBoxItem;
        var selectedScenario = selectedScenarioItem?.Content?.ToString();

        return string.IsNullOrWhiteSpace(selectedScenario) ? "Scenario 1" : selectedScenario;
    }

    private void MaintenanceSchedules_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        this.UpdateBoilerStatuses();
    }

    private ObservableCollection<BoilerStatusViewModel> CreateBoilerRows(string period)
    {
        return new ObservableCollection<BoilerStatusViewModel>
        {
            new BoilerStatusViewModel("GB1", "Gas", period),
            new BoilerStatusViewModel("GB2", "Gas", period),
            new BoilerStatusViewModel("GB3", "Gas", period),
            new BoilerStatusViewModel("OB1", "Oil", period),
        };
    }

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

    private void UpdateBoilerStatuses()
    {
        this.UpdateBoilerStatuses(this.summerBoilersScenario1, "1");
        this.UpdateBoilerStatuses(this.winterBoilersScenario1, "1");
        this.UpdateBoilerStatuses(this.summerBoilersScenario2, "2");
        this.UpdateBoilerStatuses(this.winterBoilersScenario2, "2");
    }

    private void UpdateBoilerStatuses(IEnumerable<BoilerStatusViewModel> boilers, string scenario)
    {
        foreach (var boiler in boilers)
        {
            var boilerPeriodId = this.maintenanceService.GetPeriodId(boiler.Period);
            var isUnavailable = MaintenanceStore.MaintenanceSchedules.Any(schedule =>
                schedule.Period == boilerPeriodId && schedule.AssetName == boiler.BoilerId && schedule.Scenario == scenario);

            boiler.SetUnavailable(isUnavailable);
        }
    }

    // Makes it so only one maintenance can be reserved per period in each scenario
    private async Task<bool> EnsureMaintenanceCapacityAvailableAsync(string period, string scenario)
    {
        if (this.HasMaintenanceForPeriod(period, scenario))
        {
            await this.ShowValidationDialog($"The maximum amount of maintenances has been reached for the {period} period in Scenario {scenario}.");
            return false;
        }

        return true;
    }

    // checks if maintenance is already scheduled
    private bool HasMaintenanceForPeriod(string period, string scenario)
    {
        var periodId = this.maintenanceService.GetPeriodId(period);
        return MaintenanceStore.MaintenanceSchedules.Any(schedule => schedule.Period == periodId && schedule.Scenario == scenario);
    }

    // window for confirming maintenance with all relevant details
    private async Task<bool> ShowMaintenanceConfirmationDialog(
        string period,
        string boilerId,
        DateTime startDateTime,
        DateTime endDateTime,
        DateTime periodStart,
        DateTime periodEnd)
    {
        var dialog = new Window
        {
            Title = "Confirm Maintenance",
            Width = 460,
            Height = 320,
            WindowStartupLocation = WindowStartupLocation.CenterOwner,
            CanResize = false,
            RequestedThemeVariant = this.GetCurrentThemeVariant(),
        };

        var title = new TextBlock
        {
            Text = "Review selected maintenance",
            FontSize = 18,
            FontWeight = Avalonia.Media.FontWeight.SemiBold,
            Margin = new Avalonia.Thickness(0, 0, 0, 12)
        };

        var details = new StackPanel
        {
            Spacing = 8,
            Margin = new Avalonia.Thickness(0, 0, 0, 16)
        };

        details.Children.Add(CreateDetailRow("Boiler", boilerId));
        details.Children.Add(CreateDetailRow("Period", period));
        details.Children.Add(CreateDetailRow("Start", $"{startDateTime:dd.MM.yyyy HH:mm}"));
        details.Children.Add(CreateDetailRow("End", $"{endDateTime:dd.MM.yyyy HH:mm}"));
        details.Children.Add(CreateDetailRow("Allowed", $"{periodStart:dd.MM.yyyy HH:mm} - {periodEnd:dd.MM.yyyy HH:mm}"));

        var confirmTask = new TaskCompletionSource<bool>();

        var confirmButton = new Button
        {
            Content = "Confirm",
            FontWeight = Avalonia.Media.FontWeight.SemiBold,
            Padding = new Avalonia.Thickness(16, 8),
            HorizontalAlignment = HorizontalAlignment.Right,
            Margin = new Avalonia.Thickness(0, 0, 10, 0),
        };

        confirmButton.Click += (_, _) =>
        {
            confirmTask.TrySetResult(true);
            dialog.Close();
        };

        var cancelButton = new Button
        {
            Content = "Cancel",
            FontWeight = Avalonia.Media.FontWeight.SemiBold,
            Padding = new Avalonia.Thickness(16, 8),
            HorizontalAlignment = HorizontalAlignment.Right,
        };

        cancelButton.Click += (_, _) =>
        {
            confirmTask.TrySetResult(false);
            dialog.Close();
        };

        var buttonRow = new StackPanel
        {
            Orientation = Orientation.Horizontal,
            HorizontalAlignment = HorizontalAlignment.Right,
        };

        buttonRow.Children.Add(confirmButton);
        buttonRow.Children.Add(cancelButton);

        var panel = new StackPanel
        {
            Margin = new Avalonia.Thickness(20),
            Spacing = 12,
        };

        panel.Children.Add(title);
        panel.Children.Add(details);
        panel.Children.Add(buttonRow);

        dialog.Content = panel;

        dialog.Closed += (_, _) =>
        {
            confirmTask.TrySetResult(false);
        };

        if (this.VisualRoot is Window mainWindow)
        {
            await dialog.ShowDialog(mainWindow);
        }
        else
        {
            dialog.Show();
        }

        return await confirmTask.Task;
    }

    private static StackPanel CreateDetailRow(string label, string value)
    {
        var row = new StackPanel
        {
            Orientation = Orientation.Horizontal,
            Spacing = 8
        };

        row.Children.Add(new TextBlock
        {
            Text = $"{label}:",
            FontWeight = Avalonia.Media.FontWeight.SemiBold,
            Width = 80
        });

        row.Children.Add(new TextBlock
        {
            Text = value,
            TextWrapping = Avalonia.Media.TextWrapping.Wrap
        });

        return row;
    }

    // Summer and winter period charts
    private async Task LoadSeasonChartsAsync()
    {
        var summerHeatDemandChart = this.FindControl<CartesianChart>("SummerHeatDemandChartScenario1");
        var summerElectricityPriceChart = this.FindControl<CartesianChart>("SummerElectricityPriceChartScenario1");
        var winterHeatDemandChart = this.FindControl<CartesianChart>("WinterHeatDemandChartScenario1");
        var winterElectricityPriceChart = this.FindControl<CartesianChart>("WinterElectricityPriceChartScenario1");
        if (summerHeatDemandChart is null || summerElectricityPriceChart is null || winterHeatDemandChart is null || winterElectricityPriceChart is null)
        {
            return;
        }

        List<SourceDataDto> sourceData;
        try
        {
            sourceData = await this.apiService.GetAsync<List<SourceDataDto>>(BackendService.Sdm, "getAll") ?? new List<SourceDataDto>();
        }
        catch
        {
            sourceData = new List<SourceDataDto>();
        }

        this.ConfigureSeasonCharts(
            sourceData.Where(item => item.PeriodId == 1).OrderBy(item => item.TimeFrom).ToList(),
            summerHeatDemandChart,
            summerElectricityPriceChart,
            4.5,
            3500,
            "#F59E0B",
            "#3B82F6");

        this.ConfigureSeasonCharts(
            sourceData.Where(item => item.PeriodId == 2).OrderBy(item => item.TimeFrom).ToList(),
            winterHeatDemandChart,
            winterElectricityPriceChart,
            12,
            2500,
            "#F59E0B",
            "#3B82F6");

        var summerHeatDemandChartScenario2 = this.FindControl<CartesianChart>("SummerHeatDemandChartScenario2");
        var summerElectricityPriceChartScenario2 = this.FindControl<CartesianChart>("SummerElectricityPriceChartScenario2");
        var winterHeatDemandChartScenario2 = this.FindControl<CartesianChart>("WinterHeatDemandChartScenario2");
        var winterElectricityPriceChartScenario2 = this.FindControl<CartesianChart>("WinterElectricityPriceChartScenario2");

        if (summerHeatDemandChartScenario2 != null && summerElectricityPriceChartScenario2 != null)
        {
            this.ConfigureSeasonCharts(
                sourceData.Where(item => item.PeriodId == 1).OrderBy(item => item.TimeFrom).ToList(),
                summerHeatDemandChartScenario2,
                summerElectricityPriceChartScenario2,
                4.5,
                3500,
                "#F59E0B",
                "#3B82F6");
        }

        if (winterHeatDemandChartScenario2 != null && winterElectricityPriceChartScenario2 != null)
        {
            this.ConfigureSeasonCharts(
                sourceData.Where(item => item.PeriodId == 2).OrderBy(item => item.TimeFrom).ToList(),
                winterHeatDemandChartScenario2,
                winterElectricityPriceChartScenario2,
                12,
                2500,
                "#F59E0B",
                "#3B82F6");
        }
    }

    private void ConfigureSeasonCharts(
        IReadOnlyCollection<SourceDataDto> seasonData,
        CartesianChart heatDemandChart,
        CartesianChart electricityPriceChart,
        double heatDemandMaxLimit,
        double electricityPriceMaxLimit,
        string heatDemandColorHex,
        string electricityPriceColorHex)
    {
        var heatDemandColor = SKColor.Parse(heatDemandColorHex);
        var electricityPriceColor = SKColor.Parse(electricityPriceColorHex);

        var heatDemandPoints = seasonData
            .Select(item => new DateTimePoint(item.TimeFrom, item.HeatDemand))
            .ToList();

        var electricityPricePoints = seasonData
            .Select(item => new DateTimePoint(item.TimeFrom, item.ElectricityPrice))
            .ToList();

        heatDemandChart.Series = new ISeries[]
        {
            new LineSeries<DateTimePoint>
            {
                Values = heatDemandPoints,
                Name = "Heat demand",
                LineSmoothness = 0.15,
                GeometrySize = 6,
                Stroke = new SolidColorPaint(heatDemandColor) { StrokeThickness = 3 },
                Fill = new SolidColorPaint(heatDemandColor.WithAlpha(35)),
                GeometryFill = new SolidColorPaint(heatDemandColor),
                GeometryStroke = new SolidColorPaint(heatDemandColor),
            },
        };

        heatDemandChart.XAxes = new ICartesianAxis[]
        {
            new DateTimeAxis(TimeSpan.FromHours(24), date => date.ToString("dd MMM"))
            {
                LabelsPaint = new SolidColorPaint(heatDemandColor),
                SeparatorsPaint = new SolidColorPaint(SKColor.Parse("#CBD5E1")) { StrokeThickness = 1 },
                TextSize = 12,
            },
        };

        heatDemandChart.YAxes = new ICartesianAxis[]
        {
            new Axis
            {
                MinLimit = 0,
                MaxLimit = heatDemandMaxLimit,
                Name = "MWh",
                NamePaint = this.GetAxisNamePaint(),
                LabelsPaint = new SolidColorPaint(heatDemandColor),
                SeparatorsPaint = new SolidColorPaint(SKColor.Parse("#CBD5E1")) { StrokeThickness = 1 },
                TextSize = 12,
                Labeler = value => value.ToString("0.00"),
            },
        };

        electricityPriceChart.Series = new ISeries[]
        {
            new LineSeries<DateTimePoint>
            {
                Values = electricityPricePoints,
                Name = "Electricity price",
                LineSmoothness = 0,
                GeometrySize = 8,
                Stroke = new SolidColorPaint(electricityPriceColor) { StrokeThickness = 2 },
                GeometryFill = new SolidColorPaint(electricityPriceColor),
                GeometryStroke = new SolidColorPaint(electricityPriceColor),
            },
        };

        electricityPriceChart.XAxes = new ICartesianAxis[]
        {
            new DateTimeAxis(TimeSpan.FromHours(24), date => date.ToString("dd MMM"))
            {
                LabelsPaint = new SolidColorPaint(electricityPriceColor),
                SeparatorsPaint = new SolidColorPaint(SKColor.Parse("#CBD5E1")) { StrokeThickness = 1 },
                TextSize = 12,
            },
        };

        electricityPriceChart.YAxes = new ICartesianAxis[]
        {
            new Axis
            {
                MinLimit = 0,
                MaxLimit = electricityPriceMaxLimit,
                Name = "Price",
                NamePaint = this.GetAxisNamePaint(),
                LabelsPaint = new SolidColorPaint(electricityPriceColor),
                SeparatorsPaint = new SolidColorPaint(SKColor.Parse("#CBD5E1")) { StrokeThickness = 1 },
                TextSize = 12,
                Labeler = value => value.ToString("0.00"),
            },
        };
    }

    // Error message that pops up when the user enters an invalid time interval
    // await does not work without async (makes the program wait for the user to close the window)
    private async Task ShowValidationDialog(string message)
    {
        var dialog = new Window
        {
            Title = "Error Message",
            Width = 350,
            Height = 150,
            WindowStartupLocation = WindowStartupLocation.CenterOwner,
            CanResize = false,
            RequestedThemeVariant = this.GetCurrentThemeVariant(),
        };

        var text = new TextBlock
        {
            Text = message,
            TextWrapping = Avalonia.Media.TextWrapping.Wrap,
            Margin = new Avalonia.Thickness(20),
            HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center,
            VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center,
        };

        var button = new Button
        {
            Content = "OK",
            HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center,
            Margin = new Avalonia.Thickness(0, 10, 0, 0),
        };

        // Event handler
        button.Click += (_, _) => dialog.Close();

        var panel = new StackPanel { VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center };
        panel.Children.Add(text);
        panel.Children.Add(button);

        dialog.Content = panel;

        // The VisualRoot might not always be a window, the reason for this code
        if (this.VisualRoot is Window mainWindow)
        {
            await dialog.ShowDialog(mainWindow);
        }
        else
        {
            dialog.Show();
        }
    }

    // Getting the mode from the settingsview, so that the graphs also change based on the users preference
    private SolidColorPaint GetAxisNamePaint()
    {
        var isDarkMode = Avalonia.Application.Current?.ActualThemeVariant == Avalonia.Styling.ThemeVariant.Dark;
        var axisTitleColor = isDarkMode ? SKColor.Parse("#E2E8F0") : SKColor.Parse("#334155");

        return new SolidColorPaint(axisTitleColor);
    }

    private Avalonia.Styling.ThemeVariant GetCurrentThemeVariant()
    {
        return Avalonia.Application.Current?.ActualThemeVariant ?? Avalonia.Styling.ThemeVariant.Default;
    }
}
