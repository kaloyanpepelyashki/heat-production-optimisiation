namespace Dv.App.Views;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Interactivity;
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

public partial class OptimizationView : UserControl
{
    private readonly IApiService apiService;

    public OptimizationView()
    {
        this.InitializeComponent();
        this.apiService = new ApiService();
        _ = this.LoadSeasonChartsAsync();
    }

    // Updates the view when the user changes it (Summer -> Winter), updates bindings
    protected override void OnDataContextChanged(EventArgs e)
    {
        base.OnDataContextChanged(e);

        if (this.DataContext is OptimizationViewModel viewModel)
        {
            this.BoilersListBoxSummerScenario1.ItemsSource = viewModel.SummerScenario1.Boilers;
            this.BoilersListBoxWinterScenario1.ItemsSource = viewModel.WinterScenario1.Boilers;
            this.BoilersListBoxSummerScenario2.ItemsSource = viewModel.SummerScenario2.Boilers;
            this.BoilersListBoxWinterScenario2.ItemsSource = viewModel.WinterScenario2.Boilers;

            this.MaintenanceScheduleItemsControlSummerScenario1.ItemsSource = Models.MaintenanceStore.MaintenanceSchedules;
            this.MaintenanceScheduleItemsControlWinterScenario1.ItemsSource = Models.MaintenanceStore.MaintenanceSchedules;
            this.MaintenanceScheduleItemsControlSummerScenario2.ItemsSource = Models.MaintenanceStore.MaintenanceSchedules;
            this.MaintenanceScheduleItemsControlWinterScenario2.ItemsSource = Models.MaintenanceStore.MaintenanceSchedules;
        }
    }

    // based on the view opened, this method selects the maintenance for the particular period/scenario
    private void ScheduleMaintenance_Click(object? sender, RoutedEventArgs e)
    {
        if (sender is Button button && button.Tag is string tag && this.DataContext is OptimizationViewModel viewModel)
        {
            var periodViewModel = tag switch
            {
                "SummerScenario1" => viewModel.SummerScenario1,
                "WinterScenario1" => viewModel.WinterScenario1,
                "SummerScenario2" => viewModel.SummerScenario2,
                "WinterScenario2" => viewModel.WinterScenario2,
                _ => null
            };

            if (periodViewModel == null) return;

            var listBox = this.FindControl<ListBox>($"BoilersListBox{tag}");
            var datePicker = this.FindControl<CalendarDatePicker>($"MaintenanceStartDatePicker{tag}");
            var startHour = this.FindControl<NumericUpDown>($"MaintenanceStartHour{tag}");
            var durationSlider = this.FindControl<Slider>($"MaintenanceDurationSlider{tag}");

            if (listBox != null && datePicker != null && startHour != null && durationSlider != null)
            {
                periodViewModel.SelectedBoiler = listBox.SelectedItem as BoilerStatusViewModel;
                periodViewModel.MaintenanceStartDate = datePicker.SelectedDate;
                periodViewModel.MaintenanceStartHour = (int)(startHour.Value ?? 12);
                periodViewModel.MaintenanceDuration = durationSlider.Value;
                periodViewModel.ScheduleMaintenanceCommand.Execute(null);
            }
        }
    }

    // Summer and winter period charts
    private async Task LoadSeasonChartsAsync()
    {
        List<SourceDataDto> sourceData;
        try
        {
            sourceData = await this.apiService.GetAsync<List<SourceDataDto>>(BackendService.Sdm, "getAll") ?? new List<SourceDataDto>();
        }
        catch
        {
            sourceData = new List<SourceDataDto>();
        }

        var summerData = sourceData.Where(item => item.PeriodId == 1).OrderBy(item => item.TimeFrom).ToList();
        var winterData = sourceData.Where(item => item.PeriodId == 2).OrderBy(item => item.TimeFrom).ToList();

        // We use separate chart instances for each scenario but they share the same summer/winter data and configuration
        this.ConfigureSeasonCharts(
            summerData,
            new[]
            {
                this.FindControl<CartesianChart>("SummerHeatDemandChartScenario1"),
                this.FindControl<CartesianChart>("SummerHeatDemandChartScenario2"),
            },
            new[]
            {
                this.FindControl<CartesianChart>("SummerElectricityPriceChartScenario1"),
                this.FindControl<CartesianChart>("SummerElectricityPriceChartScenario2"),
            },
            4.5,
            3500,
            "#F59E0B",
            "#3B82F6");

        this.ConfigureSeasonCharts(
            winterData,
            new[]
            {
                this.FindControl<CartesianChart>("WinterHeatDemandChartScenario1"),
                this.FindControl<CartesianChart>("WinterHeatDemandChartScenario2"),
            },
            new[]
            {
                this.FindControl<CartesianChart>("WinterElectricityPriceChartScenario1"),
                this.FindControl<CartesianChart>("WinterElectricityPriceChartScenario2"),
            },
            12,
            2500,
            "#F59E0B",
            "#3B82F6");
    }

    private void ConfigureSeasonCharts(
        IReadOnlyCollection<SourceDataDto> seasonData,
        IEnumerable<CartesianChart?> heatDemandCharts,
        IEnumerable<CartesianChart?> electricityPriceCharts,
        double heatDemandMaxLimit,
        double electricityPriceMaxLimit,
        string heatDemandColorHex,
        string electricityPriceColorHex)
    {
        foreach (var heatDemandChart in heatDemandCharts.OfType<CartesianChart>())
        {
            this.ConfigureSingleSeasonChart(
                heatDemandChart,
                seasonData,
                item => item.HeatDemand,
                "Heat demand",
                "MWh",
                heatDemandMaxLimit,
                heatDemandColorHex);
        }

        foreach (var electricityPriceChart in electricityPriceCharts.OfType<CartesianChart>())
        {
            this.ConfigureSingleSeasonChart(
                electricityPriceChart,
                seasonData,
                item => item.ElectricityPrice,
                "Electricity price",
                string.Empty,
                electricityPriceMaxLimit,
                electricityPriceColorHex);
        }
    }

    private void ConfigureSingleSeasonChart(
        CartesianChart chart,
        IReadOnlyCollection<SourceDataDto> seasonData,
        Func<SourceDataDto, double> valueSelector,
        string seriesName,
        string axisName,
        double maxLimit,
        string colorHex)
    {
        var color = SKColor.Parse(colorHex);

        var points = seasonData
            .Select(item => new DateTimePoint(item.TimeFrom, valueSelector(item)))
            .ToList();

        chart.Series = new ISeries[]
        {
            new LineSeries<DateTimePoint>
            {
                Values = points,
                Name = seriesName,
                LineSmoothness = 0.15,
                GeometrySize = 6,
                Stroke = new SolidColorPaint(color) { StrokeThickness = 3 },
                Fill = new SolidColorPaint(color.WithAlpha(35)),
                GeometryFill = new SolidColorPaint(color),
                GeometryStroke = new SolidColorPaint(color),
            },
        };

        chart.XAxes = new ICartesianAxis[]
        {
            new DateTimeAxis(TimeSpan.FromHours(24), date => date.ToString("dd MMM"))
            {
                LabelsPaint = this.GetAxisNamePaint(),
                SeparatorsPaint = new SolidColorPaint(SKColor.Parse("#CBD5E1")) { StrokeThickness = 1 },
                TextSize = 12,
            },
        };

        chart.YAxes = new ICartesianAxis[]
        {
            new Axis
            {
                MinLimit = 0,
                MaxLimit = maxLimit,
                Name = axisName,
                NamePaint = this.GetAxisNamePaint(),
                LabelsPaint = this.GetAxisNamePaint(),
                SeparatorsPaint = new SolidColorPaint(SKColor.Parse("#CBD5E1")) { StrokeThickness = 1 },
                TextSize = 12,
            },
        };
    }

    // gets the theme from settings - changes the tables based on that
    private SolidColorPaint GetAxisNamePaint()
    {
        var isDarkMode = Avalonia.Application.Current?.ActualThemeVariant == Avalonia.Styling.ThemeVariant.Dark;
        var axisTitleColor = isDarkMode ? SKColor.Parse("#E2E8F0") : SKColor.Parse("#334155");

        return new SolidColorPaint(axisTitleColor);
    }
}
