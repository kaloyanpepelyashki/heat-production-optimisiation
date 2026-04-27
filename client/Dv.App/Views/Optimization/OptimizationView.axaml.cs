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
    private readonly ObservableCollection<BoilerStatusViewModel> summerBoilers;
    private readonly ObservableCollection<BoilerStatusViewModel> winterBoilers;

    public OptimizationView()
    {
        this.InitializeComponent();
        this.apiService = new ApiService();
        this.maintenanceService = new MaintenanceService();
        this.summerBoilers = this.CreateBoilerRows("Summer");
        this.winterBoilers = this.CreateBoilerRows("Winter");
        this.BoilersListBoxSummer.ItemsSource = this.summerBoilers;
        this.BoilersListBoxWinter.ItemsSource = this.winterBoilers;
        this.MaintenanceScheduleItemsControlSummer.ItemsSource = MaintenanceStore.MaintenanceSchedules;
        this.MaintenanceScheduleItemsControlWinter.ItemsSource = MaintenanceStore.MaintenanceSchedules;
        MaintenanceStore.MaintenanceSchedules.CollectionChanged += this.MaintenanceSchedules_CollectionChanged;
        this.UpdateBoilerStatuses();
        _ = this.LoadSeasonChartsAsync();
    }

    private async void SelectMaintenanceSummer_Click(object? sender, RoutedEventArgs e)
    {
        var selectedItem = this.BoilersListBoxSummer.SelectedItem as BoilerStatusViewModel;
        string boilerId = selectedItem?.BoilerId ?? "No Boiler Selected";

        if (!await this.EnsureMaintenanceCapacityAvailableAsync("Summer"))
        {
            return;
        }

        var startDate = MaintenanceStartDatePickerSummer.SelectedDate;
        var startHour = MaintenanceStartHourSummer.Value;

        if (startDate.HasValue && startHour.HasValue)
        {
            var startDateTime = startDate.Value.Date.AddHours((double)startHour.Value);
            double durationHours = MaintenanceDurationSliderSummer.Value;
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

    private async void SelectMaintenanceWinter_Click(object? sender, RoutedEventArgs e)
    {
        var selectedItem = this.BoilersListBoxWinter.SelectedItem as BoilerStatusViewModel;
        string boilerId = selectedItem?.BoilerId ?? "No Boiler Selected";

        if (!await this.EnsureMaintenanceCapacityAvailableAsync("Winter"))
        {
            return;
        }

        var startDate = MaintenanceStartDatePickerWinter.SelectedDate;
        var startHour = MaintenanceStartHourWinter.Value;

        if (startDate.HasValue && startHour.HasValue)
        {
            var startDateTime = startDate.Value.Date.AddHours((double)startHour.Value);
            double durationHours = MaintenanceDurationSliderWinter.Value;
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

    private void UpdateBoilerStatuses()
    {
        this.UpdateBoilerStatuses(this.summerBoilers);
        this.UpdateBoilerStatuses(this.winterBoilers);
    }

    private void UpdateBoilerStatuses(IEnumerable<BoilerStatusViewModel> boilers)
    {
        foreach (var boiler in boilers)
        {
            var boilerPeriodId = this.maintenanceService.GetPeriodId(boiler.Period);
            var isUnavailable = MaintenanceStore.MaintenanceSchedules.Any(schedule =>
                schedule.Period == boilerPeriodId && schedule.AssetName == boiler.BoilerId);

            boiler.SetUnavailable(isUnavailable);
        }
    }

    // Makes it so only one maintenance can be reserved in a period (Case requirements, we dont need more)
    private async Task<bool> EnsureMaintenanceCapacityAvailableAsync(string period)
    {
        if (this.HasMaintenanceForPeriod(period))
        {
            await this.ShowValidationDialog($"The maximum amount of maintenances has been reached for the {period} period.");
            return false;
        }

        return true;
    }

    private bool HasMaintenanceForPeriod(string period)
    {
        var periodId = this.maintenanceService.GetPeriodId(period);
        return MaintenanceStore.MaintenanceSchedules.Any(schedule => schedule.Period == periodId);
    }

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
        var summerHeatDemandChart = this.FindControl<CartesianChart>("SummerHeatDemandChart");
        var summerElectricityPriceChart = this.FindControl<CartesianChart>("SummerElectricityPriceChart");
        var winterHeatDemandChart = this.FindControl<CartesianChart>("WinterHeatDemandChart");
        var winterElectricityPriceChart = this.FindControl<CartesianChart>("WinterElectricityPriceChart");
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
