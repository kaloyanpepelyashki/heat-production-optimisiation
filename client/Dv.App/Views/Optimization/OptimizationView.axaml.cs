namespace Dv.App.Views;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Controls;
using LiveChartsCore;
using LiveChartsCore.Defaults;
using LiveChartsCore.Kernel.Sketches;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Avalonia;
using LiveChartsCore.SkiaSharpView.Painting;
using LiveChartsCore.Measure;
using SkiaSharp;
using Dv.App.Models;
using Dv.App.Services;


public partial class OptimizationView : UserControl
{
    private readonly IApiService apiService;

    public OptimizationView()
    {
        this.InitializeComponent();
        this.apiService = new ApiService();
        _ = this.LoadSeasonChartsAsync();
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

        OptimisationRunDto? targetRun = null;
        try
        {
            var optRunsResponse = await this.apiService.GetAsync<ApiResponseModel<List<OptimisationRunDto>>>(BackendService.Rdm, "allOptimisationRuns");
            if (optRunsResponse?.Data != null)
            {
                targetRun = optRunsResponse.Data.FirstOrDefault(r => r.Id == 18);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Could not fetch optimisation runs: " + ex.Message);
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

        List<CartesianChart?> OptimizationResultsCharts = new List<CartesianChart?>
        {
            this.FindControl<CartesianChart>("SummerOptimizationResultsChartScenario1"),
            this.FindControl<CartesianChart>("WinterOptimizationResultsChartScenario1"),
            this.FindControl<CartesianChart>("SummerOptimizationResultsChartScenario2"),
            this.FindControl<CartesianChart>("WinterOptimizationResultsChartScenario2"),
        };

        foreach(CartesianChart chart in OptimizationResultsCharts)
        {
            if (chart != null && targetRun != null)
            {
                this.ConfigureOptimizationResultsChart(chart, targetRun);
            }
        }

        this.ConfigureSingleSeasonChart(
                electricityPriceChart,
                seasonData,
                item => item.ElectricityPrice,
                "Electricity price",
                string.Empty,
                electricityPriceMaxLimit,
                electricityPriceColorHex);
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

    private void ConfigureOptimizationResultsChart(CartesianChart chart, OptimisationRunDto run)
{
    var hourly = run.optimisationResultsHourly?
        .OrderBy(h => h.TimeFrom)
        .ToList() ?? new List<OptimisationResultsHourlyDto>();

    if (!hourly.Any())
    {
        return;
    }
    var allUnits = hourly
        .SelectMany(h => h.ProductionUnits)
        .GroupBy(u => new
        {
            u.ProductionUnitType,
            u.ProductionUnitId,
        })
        .Select(g => new
        {
            ProductionUnitType = g.Key.ProductionUnitType,
            ProductionUnitId = g.Key.ProductionUnitId,
            MaxHeatProduction = g
            .Where(u => u.Capacity > 0)
            .Max(u => u.Capacity),

        })
        .OrderByDescending(u => u.MaxHeatProduction)
        .ToList();

    var colors = new[]
    {
        SKColor.Parse("#FF0000"),
        SKColor.Parse("#00FF00"),
        SKColor.Parse("#0000FF"),
        SKColor.Parse("#000000"),
        SKColor.Parse("#ff0ff3"),
        SKColor.Parse("#A855F7")
    };

    var seriesList = new List<ISeries>();
    var colorIdx = 0;

    foreach (var unit in allUnits)
    {
        var color = colors[colorIdx % colors.Length];
        colorIdx++;

        var unitPoints = hourly.Select(h =>
        {
            var unitsThisHour = h.ProductionUnits.ToList();

            var currentUnit = unitsThisHour
                .FirstOrDefault(u =>
                    u.ProductionUnitId == unit.ProductionUnitId &&
                    u.ProductionUnitType == unit.ProductionUnitType);

            if (currentUnit == null || currentUnit.HeatProduction <= 0 || h.HeatProduction <= 0)
            {
                return new DateTimePoint(h.TimeFrom, 0);
            }

            return new DateTimePoint(h.TimeFrom, currentUnit.HeatProduction);
        }).ToList();

        seriesList.Add(new StackedAreaSeries<DateTimePoint>
        {
            Name = $"{unit.ProductionUnitType} {unit.ProductionUnitId}",
            Values = unitPoints,
            LineSmoothness = 0.15,
            GeometrySize = 0,
            Stroke = new SolidColorPaint(color)
            {
                StrokeThickness = 3
            },
            Fill = new SolidColorPaint(color.WithAlpha(150))
        });
    }

    var heatDemandColor = SKColor.Parse("#0EA5E9");

    var heatDemandPoints = hourly
        .Select(h => new DateTimePoint(h.TimeFrom, h.HeatProduction))
        .ToList();

    seriesList.Add(new LineSeries<DateTimePoint>
    {
        Name = "Heat demand",
        Values = heatDemandPoints,
        LineSmoothness = 0.15,
        GeometrySize = 6,
        Stroke = new SolidColorPaint(heatDemandColor)
        {
            StrokeThickness = 3,
        },
        Fill = null,
        GeometryFill = new SolidColorPaint(heatDemandColor),
        GeometryStroke = new SolidColorPaint(heatDemandColor),
    });

    chart.Series = seriesList.ToArray();

    chart.XAxes = new ICartesianAxis[]
    {
        new DateTimeAxis(TimeSpan.FromHours(24), date => date.ToString("dd MMM"))
        {
            LabelsPaint = this.GetAxisNamePaint(),
            SeparatorsPaint = new SolidColorPaint(SKColor.Parse("#CBD5E1"))
            {
                StrokeThickness = 1,
            },
            TextSize = 12,
        },
    };

    var maxHeatProduction = hourly
        .Select(h => h.HeatProduction)
        .DefaultIfEmpty(0)
        .Max();

    var yMax = Math.Ceiling(maxHeatProduction * 1.10);

    chart.YAxes = new ICartesianAxis[]
    {
        new Axis
        {
            MinLimit = 0,
            MaxLimit = yMax == 0 ? 12 : yMax,
            Name = "MWh",
            NamePaint = this.GetAxisNamePaint(),
            LabelsPaint = this.GetAxisNamePaint(),
            SeparatorsPaint = new SolidColorPaint(SKColor.Parse("#CBD5E1"))
            {
                StrokeThickness = 1,
            },
            TextSize = 12,
        },
    };

    chart.LegendPosition = LiveChartsCore.Measure.LegendPosition.Right;
    chart.LegendTextPaint = this.GetAxisNamePaint();
    chart.ZoomMode = LiveChartsCore.Measure.ZoomAndPanMode.None;
    }
}