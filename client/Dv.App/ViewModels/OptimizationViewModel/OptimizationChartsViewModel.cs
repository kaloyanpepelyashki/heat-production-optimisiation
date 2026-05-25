using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia.Controls;
using CommunityToolkit.Mvvm.Input;
using Dv.App.Models;
using LiveChartsCore;
using LiveChartsCore.Defaults;
using LiveChartsCore.Kernel.Sketches;
using LiveChartsCore.Measure;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Avalonia;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;

namespace Dv.App.ViewModels;

public sealed class OptimizationChartsViewModel : ViewModelBase
{
    private CartesianChart? heatDemandChart;
    private CartesianChart? electricityPriceChart;
    private CartesianChart? optimizationResultsChart;
    private CartesianChart? expensesChart;
    private CartesianChart? co2EmissionsChart;

    private DateTimePoint[] cachedElectricityPrices = [];

    public void AttachCharts(
        CartesianChart heatDemandChart,
        CartesianChart electricityPriceChart,
        CartesianChart optimizationResultsChart,
        CartesianChart expensesChart,
        CartesianChart co2EmissionsChart)
    {
        this.heatDemandChart = heatDemandChart;
        this.electricityPriceChart = electricityPriceChart;
        this.optimizationResultsChart = optimizationResultsChart;
        this.expensesChart = expensesChart;
        this.co2EmissionsChart = co2EmissionsChart;
    }

    public void LoadSourceData(IEnumerable<SourceDataDto> sourceData, OptimizationContext context)
    {
        var filtered =
            sourceData
                .Where(x =>
                    x.TimeFrom >= context.StartDate &&
                    x.TimeTo <= context.EndDate)
                .OrderBy(x => x.TimeFrom)
                .ToList();

        var heatDemand =
            filtered
                .Select(x => new DateTimePoint(x.TimeFrom, (double)x.HeatDemand))
                .ToArray();

        this.cachedElectricityPrices =
            filtered
                .Select(x => new DateTimePoint(x.TimeFrom, (double)x.ElectricityPrice))
                .ToArray();

        ConfigureSingleChart(this.heatDemandChart, "Heat Demand", "#F59E0B", "MWh", heatDemand);
        ConfigureDualAxisChart(this.electricityPriceChart, this.cachedElectricityPrices, null);
    }

    public void ClearOptimizationCharts()
    {
        if (this.optimizationResultsChart is not null)
            this.optimizationResultsChart.Series = Array.Empty<ISeries>();
        if (this.expensesChart is not null)
            this.expensesChart.Series = Array.Empty<ISeries>();
        if (this.co2EmissionsChart is not null)
            this.co2EmissionsChart.Series = Array.Empty<ISeries>();

        if (this.cachedElectricityPrices.Length > 0)
            ConfigureDualAxisChart(this.electricityPriceChart, this.cachedElectricityPrices, null);
    }

    public void LoadOptimizationResult(OptimisationRunDto optimizationResult, OptimizationContext context)
    {
        if (optimizationResult.optimisationResultsHourly is null || optimizationResult.optimisationResultsHourly.Count == 0)
        {
            if (this.optimizationResultsChart is not null)
                this.optimizationResultsChart.Series = new List<ISeries>();
            if (this.expensesChart is not null)
                this.expensesChart.Series = new List<ISeries>();
            if (this.co2EmissionsChart is not null)
                this.co2EmissionsChart.Series = new List<ISeries>();
            return;
        }

        var orderedResults = optimizationResult.optimisationResultsHourly
            .OrderBy(x => x.TimeFrom)
            .ToList();

        var electricityConsumption = orderedResults
            .Select(x => new DateTimePoint(x.TimeFrom, (double)x.ElectricityConsumption))
            .ToArray();

        var expenses = orderedResults
            .Select(x => new DateTimePoint(x.TimeFrom, x.Expenses))
            .ToArray();

        var co2Emissions = orderedResults
            .Select(x => new DateTimePoint(x.TimeFrom, x.Co2Emissions))
            .ToArray();

        ConfigureDualAxisChart(this.electricityPriceChart, this.cachedElectricityPrices, electricityConsumption);
        ConfigureSingleChart(this.expensesChart, "Expenses", "#ea8080", "DKK", expenses);
        ConfigureSingleChart(this.co2EmissionsChart, "CO2 Emissions", "#DC2626", "kg / MWh", co2Emissions);

        BuildOptimizationStackedChart(orderedResults);
    }

    private void BuildOptimizationStackedChart(List<OptimisationResultsHourlyDto> orderedResults)
    {
        var allUnits = orderedResults
            .SelectMany(h => h.ProductionUnits)
            .GroupBy(u => new { u.ProductionUnitType, u.ProductionUnitId })
            .Select(g => new
            {
                g.Key.ProductionUnitType,
                g.Key.ProductionUnitId,
                MaxHeatProduction = g.Where(u => u.Capacity > 0).Max(u => u.Capacity),
            })
            .OrderBy(u => u.ProductionUnitId)
            .OrderByDescending(u => u.MaxHeatProduction)
            .ToList();

        var colors = new[]
        {
            SKColor.Parse("#FF0000"),
            SKColor.Parse("#00FF00"),
            SKColor.Parse("#0000FF"),
            SKColor.Parse("#000000"),
            SKColor.Parse("#FF00FF"),
            SKColor.Parse("#A855F7"),
        };

        var seriesList = new List<ISeries>();
        var colorIdx = 0;

        foreach (var unit in allUnits)
        {
            var color = colors[colorIdx % colors.Length];
            colorIdx++;

            var unitPoints = orderedResults.Select(h =>
            {
                var currentUnit = h.ProductionUnits
                    .FirstOrDefault(u =>
                        u.ProductionUnitId == unit.ProductionUnitId &&
                        u.ProductionUnitType == unit.ProductionUnitType);

                if (currentUnit == null || currentUnit.HeatProduction <= 0 || h.HeatProduction <= 0)
                    return new DateTimePoint(h.TimeFrom, 0);

                return new DateTimePoint(h.TimeFrom, currentUnit.HeatProduction);
            }).ToList();

            seriesList.Add(new StackedAreaSeries<DateTimePoint>
            {
                Name = $"{unit.ProductionUnitType} {unit.ProductionUnitId}",
                Values = unitPoints,
                LineSmoothness = 0.15,
                GeometrySize = 0,
                Stroke = new SolidColorPaint(color) { StrokeThickness = 3 },
                Fill = new SolidColorPaint(color.WithAlpha(150)),
            });
        }

        if (this.optimizationResultsChart is not null)
        {
            this.optimizationResultsChart.Series = seriesList;
            this.optimizationResultsChart.LegendPosition = LegendPosition.Bottom;

            this.optimizationResultsChart.XAxes = new ICartesianAxis[]
            {
                new DateTimeAxis(TimeSpan.FromHours(24), date => date.ToString("dd MMM"))
                {
                    LabelsPaint = GetAxisNamePaint(),
                    SeparatorsPaint = new SolidColorPaint(SKColor.Parse("#CBD5E1")) { StrokeThickness = 1 },
                    TextSize = 12,
                },
            };

            this.optimizationResultsChart.YAxes = new ICartesianAxis[]
            {
                new Axis
                {
                    Name = "MWh",
                    NamePaint = GetAxisNamePaint(),
                    LabelsPaint = GetAxisNamePaint(),
                    SeparatorsPaint = new SolidColorPaint(SKColor.Parse("#CBD5E1")) { StrokeThickness = 1 },
                    TextSize = 12,
                },
            };
        }
    }

    private void ConfigureDualAxisChart(
        CartesianChart? chart,
        DateTimePoint[] prices,
        DateTimePoint[]? consumption)
    {
        if (chart is null) return;

        var priceColor = SKColor.Parse("#3B82F6");
        var consumptionColor = SKColor.Parse("#10B981");

        var series = new List<ISeries>
        {
            new LineSeries<DateTimePoint>
            {
                Name = "Electricity Price",
                Values = prices,
                LineSmoothness = 0.15,
                GeometrySize = 6,
                Stroke = new SolidColorPaint(priceColor) { StrokeThickness = 3 },
                Fill = new SolidColorPaint(priceColor.WithAlpha(35)),
                GeometryFill = new SolidColorPaint(priceColor),
                GeometryStroke = new SolidColorPaint(priceColor),
                ScalesYAt = 0,
            },
        };

        if (consumption is not null)
        {
            series.Add(new LineSeries<DateTimePoint>
            {
                Name = "Electricity Consumption",
                Values = consumption,
                LineSmoothness = 0.15,
                GeometrySize = 6,
                Stroke = new SolidColorPaint(consumptionColor) { StrokeThickness = 3 },
                Fill = new SolidColorPaint(consumptionColor.WithAlpha(35)),
                GeometryFill = new SolidColorPaint(consumptionColor),
                GeometryStroke = new SolidColorPaint(consumptionColor),
                ScalesYAt = 1,
            });
        }

        chart.Series = series;

        chart.XAxes = new ICartesianAxis[]
        {
            new DateTimeAxis(TimeSpan.FromHours(24), date => date.ToString("dd MMM"))
            {
                LabelsPaint = GetAxisNamePaint(),
                SeparatorsPaint = new SolidColorPaint(SKColor.Parse("#CBD5E1")) { StrokeThickness = 1 },
                TextSize = 12,
            },
        };

        double priceMin = prices.Length > 0 ? Math.Floor(prices.Min(p => p.Value ?? 0) * 1.1) : 0;
        double priceMax = prices.Length > 0 ? Math.Ceiling(prices.Max(p => p.Value ?? 0) * 1.1) : 100;

        var yAxes = new List<ICartesianAxis>
        {
            new Axis
            {
                Name = "DKK / MWh",
                MinLimit = Math.Min(0, priceMin),
                MaxLimit = priceMax,
                NamePaint = GetAxisNamePaint(),
                LabelsPaint = GetAxisNamePaint(),
                SeparatorsPaint = new SolidColorPaint(SKColor.Parse("#CBD5E1")) { StrokeThickness = 1 },
                TextSize = 12,
            },
        };

        if (consumption is not null)
        {
            double consMin = consumption.Length > 0 ? Math.Floor(consumption.Min(p => p.Value ?? 0) * 1.1) : 0;
            double consMax = consumption.Length > 0 ? Math.Ceiling(consumption.Max(p => p.Value ?? 0) * 1.1) : 100;

            yAxes.Add(new Axis
            {
                Name = "MWh",
                MinLimit = Math.Min(0, consMin),
                MaxLimit = consMax,
                Position = AxisPosition.End,
                NamePaint = GetAxisNamePaint(),
                LabelsPaint = GetAxisNamePaint(),
                SeparatorsPaint = new SolidColorPaint(SKColors.Transparent),
                TextSize = 12,
            });
        }

        chart.YAxes = yAxes.ToArray();
        chart.LegendPosition = consumption is not null ? LegendPosition.Bottom : LegendPosition.Hidden;
    }

    private void ConfigureSingleChart(
        CartesianChart? chart,
        string seriesName,
        string colorHex,
        string axisName,
        DateTimePoint[] data)
    {
        if (chart is null) return;

        var color = SKColor.Parse(colorHex);

        chart.Series = new List<ISeries>
        {
            new LineSeries<DateTimePoint>
            {
                Values = data,
                Name = seriesName,
                LineSmoothness = 0.15,
                GeometrySize = 6,
                Stroke = new SolidColorPaint(color) { StrokeThickness = 3 },
                Fill = new SolidColorPaint(color.WithAlpha(35)),
                GeometryFill = new SolidColorPaint(color),
                GeometryStroke = new SolidColorPaint(color),
            },
        };

        double minValue = data.Length > 0 ? data.Min(p => p.Value ?? 0) : 0;
        double maxValue = data.Length > 0 ? data.Max(p => p.Value ?? 0) : 100;

        chart.XAxes = new ICartesianAxis[]
        {
            new DateTimeAxis(TimeSpan.FromHours(24), date => date.ToString("dd MMM"))
            {
                LabelsPaint = GetAxisNamePaint(),
                SeparatorsPaint = new SolidColorPaint(SKColor.Parse("#CBD5E1")) { StrokeThickness = 1 },
                TextSize = 12,
            },
        };

        chart.YAxes = new ICartesianAxis[]
        {
            new Axis
            {
                MinLimit = Math.Floor(Math.Min(0, minValue) * 1.1),
                MaxLimit = Math.Ceiling(Math.Max(0, maxValue) * 1.1),
                Name = axisName,
                NamePaint = GetAxisNamePaint(),
                LabelsPaint = GetAxisNamePaint(),
                SeparatorsPaint = new SolidColorPaint(SKColor.Parse("#CBD5E1")) { StrokeThickness = 1 },
                TextSize = 12,
            },
        };
    }

    // ── Maximize overlay ──────────────────────────────────────────────────────

    private IEnumerable<ISeries>? maximizedSeries;
    private IEnumerable<ICartesianAxis>? maximizedXAxes;
    private IEnumerable<ICartesianAxis>? maximizedYAxes;
    private string maximizedTitle = string.Empty;
    private bool isMaximized;
    private IRelayCommand? closeMaximizedCommand;

    public IEnumerable<ISeries>? MaximizedSeries
    {
        get => this.maximizedSeries;
        private set => this.SetProperty(ref this.maximizedSeries, value);
    }

    public IEnumerable<ICartesianAxis>? MaximizedXAxes
    {
        get => this.maximizedXAxes;
        private set => this.SetProperty(ref this.maximizedXAxes, value);
    }

    public IEnumerable<ICartesianAxis>? MaximizedYAxes
    {
        get => this.maximizedYAxes;
        private set => this.SetProperty(ref this.maximizedYAxes, value);
    }

    public string MaximizedTitle
    {
        get => this.maximizedTitle;
        private set => this.SetProperty(ref this.maximizedTitle, value);
    }

    public bool IsMaximized
    {
        get => this.isMaximized;
        private set => this.SetProperty(ref this.isMaximized, value);
    }

    public IRelayCommand CloseMaximizedCommand =>
        this.closeMaximizedCommand ??= new RelayCommand(() =>
        {
            this.IsMaximized = false;
            this.MaximizedSeries = null;
        });

    public void Maximize(CartesianChart? chart, string title)
    {
        if (chart is null) return;

        this.MaximizedSeries = chart.Series;
        this.MaximizedXAxes = chart.XAxes;
        this.MaximizedYAxes = chart.YAxes;
        this.MaximizedTitle = title;
        this.IsMaximized = true;
    }

    // ─────────────────────────────────────────────────────────────────────────

    private static SolidColorPaint GetAxisNamePaint()
    {
        var isDarkMode = Avalonia.Application.Current?.ActualThemeVariant == Avalonia.Styling.ThemeVariant.Dark;
        var axisTitleColor = isDarkMode ? SKColor.Parse("#E2E8F0") : SKColor.Parse("#334155");

        return new SolidColorPaint(axisTitleColor);
    }
}
