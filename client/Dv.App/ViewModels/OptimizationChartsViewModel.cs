using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Avalonia.Styling;
using Dv.App.Models;
using LiveChartsCore;
using LiveChartsCore.Defaults;
using LiveChartsCore.Kernel.Sketches;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;

namespace Dv.App.ViewModels;

public sealed class OptimizationChartsViewModel : ViewModelBase
{
    public OptimizationChartsViewModel()
    {
        this.OptimizationResultsChart = this.CreateChart("Optimization Results");
        this.ElectricityConsumptionChart = this.CreateChart("Electricity Consumption");
        this.ExpensesChart = this.CreateChart("Expenses");
        this.Co2EmissionsChart = this.CreateChart("CO2 Emissions");
    }

    public ChartCardViewModel OptimizationResultsChart { get; }
    public ChartCardViewModel ElectricityConsumptionChart { get; }
    public ChartCardViewModel ExpensesChart { get; }
    public ChartCardViewModel Co2EmissionsChart { get; }

    public void LoadOptimizationResult(IReadOnlyList<OptimisationResultsHourlyClient> hourlyResults, string period)
    {
        if (hourlyResults is null || hourlyResults.Count == 0)
        {
            this.OptimizationResultsChart.Series = new ObservableCollection<ISeries>();
            this.ElectricityConsumptionChart.Series = new ObservableCollection<ISeries>();
            this.ExpensesChart.Series = new ObservableCollection<ISeries>();
            this.Co2EmissionsChart.Series = new ObservableCollection<ISeries>();
            return;
        }

        var ordered = hourlyResults.OrderBy(x => x.TimeFrom).ToList();

        var consumptionColor = SKColor.Parse("#3B82F6");
        var expensesColor = SKColor.Parse("#F59E0B");
        var emissionsColor = SKColor.Parse("#DC2626");

        this.ElectricityConsumptionChart.Series = new ObservableCollection<ISeries>
        {
            new LineSeries<DateTimePoint>
            {
                Values = ordered.Select(x => new DateTimePoint(x.TimeFrom, x.ElectricityConsumption)).ToArray(),
                Name = "Electricity Consumption",
                LineSmoothness = 0.15,
                GeometrySize = 6,
                Stroke = new SolidColorPaint(consumptionColor) { StrokeThickness = 3 },
                Fill = new SolidColorPaint(consumptionColor.WithAlpha(35)),
                GeometryFill = new SolidColorPaint(consumptionColor),
                GeometryStroke = new SolidColorPaint(consumptionColor),
            },
        };

        this.ExpensesChart.Series = new ObservableCollection<ISeries>
        {
            new LineSeries<DateTimePoint>
            {
                Values = ordered.Select(x => new DateTimePoint(x.TimeFrom, x.Expenses)).ToArray(),
                Name = "Expenses",
                LineSmoothness = 0.15,
                GeometrySize = 6,
                Stroke = new SolidColorPaint(expensesColor) { StrokeThickness = 3 },
                Fill = new SolidColorPaint(expensesColor.WithAlpha(35)),
                GeometryFill = new SolidColorPaint(expensesColor),
                GeometryStroke = new SolidColorPaint(expensesColor),
            },
        };

        this.Co2EmissionsChart.Series = new ObservableCollection<ISeries>
        {
            new LineSeries<DateTimePoint>
            {
                Values = ordered.Select(x => new DateTimePoint(x.TimeFrom, x.Co2Emissions)).ToArray(),
                Name = "CO2 Emissions",
                LineSmoothness = 0.15,
                GeometrySize = 6,
                Stroke = new SolidColorPaint(emissionsColor) { StrokeThickness = 3 },
                Fill = new SolidColorPaint(emissionsColor.WithAlpha(35)),
                GeometryFill = new SolidColorPaint(emissionsColor),
                GeometryStroke = new SolidColorPaint(emissionsColor),
            },
        };

        var allUnits = ordered
            .SelectMany(h => h.ProductionUnits)
            .GroupBy(u => new { u.ProductionUnitType, u.ProductionUnitId })
            .Select(g => new
            {
                g.Key.ProductionUnitType,
                g.Key.ProductionUnitId,
                MaxCapacity = g.Where(u => u.Capacity > 0).Select(u => u.Capacity).DefaultIfEmpty(0).Max(),
            })
            .OrderByDescending(u => u.MaxCapacity)
            .ToList();

        var colors = new[]
        {
            SKColor.Parse("#FF0000"),
            SKColor.Parse("#00FF00"),
            SKColor.Parse("#0000FF"),
            SKColor.Parse("#000000"),
            SKColor.Parse("#ff0ff3"),
            SKColor.Parse("#A855F7"),
        };

        var seriesList = new List<ISeries>();
        var colorIdx = 0;

        foreach (var unit in allUnits)
        {
            var color = colors[colorIdx++ % colors.Length];

            var points = ordered.Select(h =>
            {
                var pu = h.ProductionUnits.FirstOrDefault(u =>
                    u.ProductionUnitId == unit.ProductionUnitId &&
                    u.ProductionUnitType == unit.ProductionUnitType);

                return new DateTimePoint(h.TimeFrom, (pu is null || pu.HeatProduction <= 0) ? 0 : pu.HeatProduction);
            }).ToList();

            seriesList.Add(new StackedAreaSeries<DateTimePoint>
            {
                Name = $"{unit.ProductionUnitType} {unit.ProductionUnitId}",
                Values = points,
                LineSmoothness = 0.15,
                GeometrySize = 0,
                Stroke = new SolidColorPaint(color) { StrokeThickness = 3 },
                Fill = new SolidColorPaint(color.WithAlpha(150)),
            });
        }

        this.OptimizationResultsChart.Series = new ObservableCollection<ISeries>(seriesList);

        this.OptimizationResultsChart.XAxes = new ObservableCollection<ICartesianAxis>
        {
            new DateTimeAxis(TimeSpan.FromHours(24), date => date.ToString("dd MMM"))
            {
                LabelsPaint = this.GetAxisPaint(),
                SeparatorsPaint = new SolidColorPaint(SKColor.Parse("#CBD5E1")) { StrokeThickness = 1 },
                TextSize = 12,
            },
        };

        var maxHeat = ordered.Select(h => h.HeatProduction).DefaultIfEmpty(0).Max();
        var yMax = Math.Ceiling(maxHeat * 1.10);

        this.OptimizationResultsChart.YAxes = new ObservableCollection<ICartesianAxis>
        {
            new Axis
            {
                MinLimit = 0,
                MaxLimit = yMax == 0 ? 12 : yMax,
                Name = "MWh",
                NamePaint = this.GetAxisPaint(),
                LabelsPaint = this.GetAxisPaint(),
                SeparatorsPaint = new SolidColorPaint(SKColor.Parse("#CBD5E1")) { StrokeThickness = 1 },
                TextSize = 12,
            },
        };
    }

    private ChartCardViewModel CreateChart(string title)
    {
        return new ChartCardViewModel
        {
            Title = title,
            Series = new ObservableCollection<ISeries>(),
            XAxes = new ObservableCollection<ICartesianAxis>
            {
                new DateTimeAxis(TimeSpan.FromHours(24), date => date.ToString("dd MMM"))
                {
                    LabelsPaint = this.GetAxisPaint(),
                    SeparatorsPaint = new SolidColorPaint(SKColor.Parse("#CBD5E1")) { StrokeThickness = 1 },
                    TextSize = 12,
                },
            },
            YAxes = new ObservableCollection<ICartesianAxis>
            {
                new Axis
                {
                    LabelsPaint = this.GetAxisPaint(),
                    NamePaint = this.GetAxisPaint(),
                    SeparatorsPaint = new SolidColorPaint(SKColor.Parse("#CBD5E1")) { StrokeThickness = 1 },
                    TextSize = 12,
                },
            },
        };
    }

    private SolidColorPaint GetAxisPaint()
    {
        var isDark = Avalonia.Application.Current?.ActualThemeVariant == ThemeVariant.Dark;
        return new SolidColorPaint(isDark ? SKColor.Parse("#E2E8F0") : SKColor.Parse("#334155"));
    }
}
