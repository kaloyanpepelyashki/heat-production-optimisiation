using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Avalonia;
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
        this.HeatDemandChart =
            this.CreateChart("Heat Demand");

        this.ElectricityPriceChart =
            this.CreateChart("Electricity Price");

        this.OptimizationResultsChart =
            this.CreateChart("Optimization Results");

        this.ElectricityConsumptionChart =
            this.CreateChart("Electricity Consumption");

        this.ExpensesChart =
            this.CreateChart("Expenses");

        this.Co2EmissionsChart =
            this.CreateChart("CO2 Emissions");
    }

    public ChartCardViewModel HeatDemandChart { get; }

    public ChartCardViewModel ElectricityPriceChart { get; }

    public ChartCardViewModel OptimizationResultsChart { get; }

    public ChartCardViewModel ElectricityConsumptionChart { get; }

    public ChartCardViewModel ExpensesChart { get; }

    public ChartCardViewModel Co2EmissionsChart { get; }

    public void LoadSourceData(
        IEnumerable<SourceDataDto> sourceData,
        OptimizationContext context)
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

        var electricityPrices =
            filtered
                .Select(x => new DateTimePoint(x.TimeFrom, (double)x.ElectricityPrice))
                .ToArray();

        var heatColor = SKColor.Parse("#F59E0B");
        var priceColor = SKColor.Parse("#3B82F6");

        this.HeatDemandChart.Series = new ObservableCollection<ISeries>
        {
            new LineSeries<DateTimePoint>
            {
                Values = heatDemand,
                Name = "Heat Demand",
                LineSmoothness = 0.15,
                GeometrySize = 6,
                Stroke = new SolidColorPaint(heatColor) { StrokeThickness = 3 },
                Fill = new SolidColorPaint(heatColor.WithAlpha(35)),
                GeometryFill = new SolidColorPaint(heatColor),
                GeometryStroke = new SolidColorPaint(heatColor),
            },
        };

        // Adjust Y axis for heat demand to provide headroom and avoid clipping
        var maxHeat = heatDemand.Select(p => p.Value ?? 0d).DefaultIfEmpty(0d).Max();
        if (this.HeatDemandChart.YAxes.FirstOrDefault() is Axis heatAxis)
        {
            heatAxis.MinLimit = 0;
            heatAxis.MaxLimit = Math.Ceiling(Math.Max(1, maxHeat) * 1.1);
        }

        this.ElectricityPriceChart.Series = new ObservableCollection<ISeries>
        {
            new LineSeries<DateTimePoint>
            {
                Values = electricityPrices,
                Name = "Electricity Price",
                LineSmoothness = 0.15,
                GeometrySize = 6,
                Stroke = new SolidColorPaint(priceColor) { StrokeThickness = 3 },
                Fill = new SolidColorPaint(priceColor.WithAlpha(35)),
                GeometryFill = new SolidColorPaint(priceColor),
                GeometryStroke = new SolidColorPaint(priceColor),
            },
        };

        var minPrice = electricityPrices.Select(p => p.Value ?? 0d).DefaultIfEmpty(0d).Min();
        var maxPrice = electricityPrices.Select(p => p.Value ?? 0d).DefaultIfEmpty(0d).Max();
        if (this.ElectricityPriceChart.YAxes.FirstOrDefault() is Axis priceAxis)
        {
            priceAxis.MinLimit = Math.Floor(Math.Min(0, minPrice) * 1.1);
            priceAxis.MaxLimit = Math.Ceiling(Math.Max(1, maxPrice) * 1.1);
        }
    }

    public void LoadOptimizationResult(OptimisationRunDto optimizationResult, OptimizationContext context)
    {
        if (optimizationResult.optimisationResultsHourly is null || optimizationResult.optimisationResultsHourly.Count == 0)
        {
            this.OptimizationResultsChart.Series = new ObservableCollection<ISeries>();

            this.ElectricityConsumptionChart.Series = new ObservableCollection<ISeries>();

            this.ExpensesChart.Series = new ObservableCollection<ISeries>();

            this.Co2EmissionsChart.Series = new ObservableCollection<ISeries>();
        }

        var orderedResults =
            optimizationResult.optimisationResultsHourly
                .OrderBy(x => x.TimeFrom)
                .ToList();

        var electricityConsumption =
            orderedResults
                .Select(x => new DateTimePoint(x.TimeFrom, (double)x.ElectricityConsumption))
                .ToArray();

        var expenses =
            orderedResults
                .Select(x => new DateTimePoint(x.TimeFrom, x.Expenses))
                .ToArray();

        var co2Emissions =
            orderedResults
                .Select(x => new DateTimePoint(x.TimeFrom, x.Co2Emissions))
                .ToArray();

        var consumptionColor = SKColor.Parse("#3B82F6");
        var expensesColor = SKColor.Parse("#F59E0B");
        var emissionsColor = SKColor.Parse("#DC2626");

        this.ElectricityConsumptionChart.Series = new ObservableCollection<ISeries>
        {
            new LineSeries<DateTimePoint>
            {
                Values = electricityConsumption,
                Name = "Electricity Consumption",
                LineSmoothness = 0.15,
                GeometrySize = 6,
                Stroke = new SolidColorPaint(consumptionColor) { StrokeThickness = 3 },
                Fill = new SolidColorPaint(consumptionColor.WithAlpha(35)),
                GeometryFill = new SolidColorPaint(consumptionColor),
                GeometryStroke = new SolidColorPaint(consumptionColor),
            },
        };

        // set sensible axis limits for electricity consumption
        var minConsumption = electricityConsumption.Select(p => p.Value ?? 0d).DefaultIfEmpty(0d).Min();
        var maxConsumption = electricityConsumption.Select(p => p.Value ?? 0d).DefaultIfEmpty(0d).Max();
        if (this.ElectricityConsumptionChart.YAxes.FirstOrDefault() is Axis consAxis)
        {
            consAxis.MinLimit = Math.Floor(Math.Min(0, minConsumption) * 1.1);
            consAxis.MaxLimit = Math.Ceiling(Math.Max(1, maxConsumption) * 1.1);
        }

        this.ExpensesChart.Series = new ObservableCollection<ISeries>
        {
            new LineSeries<DateTimePoint>
            {
                Values = expenses,
                Name = "Expenses",
                LineSmoothness = 0.15,
                GeometrySize = 6,
                Stroke = new SolidColorPaint(expensesColor) { StrokeThickness = 3 },
                Fill = new SolidColorPaint(expensesColor.WithAlpha(35)),
                GeometryFill = new SolidColorPaint(expensesColor),
                GeometryStroke = new SolidColorPaint(expensesColor),
            },
        };

        var minExpenses = expenses.Select(p => p.Value ?? 0d).DefaultIfEmpty(0d).Min();
        var maxExpenses = expenses.Select(p => p.Value ?? 0d).DefaultIfEmpty(0d).Max();
        if (this.ExpensesChart.YAxes.FirstOrDefault() is Axis expensesAxis)
        {
            expensesAxis.MinLimit = Math.Floor(Math.Min(0, minExpenses) * 1.1);
            expensesAxis.MaxLimit = Math.Ceiling(Math.Max(1, maxExpenses) * 1.1);
        }

        this.Co2EmissionsChart.Series = new ObservableCollection<ISeries>
        {
            new LineSeries<DateTimePoint>
            {
                Values = co2Emissions,
                Name = "CO2 Emissions",
                LineSmoothness = 0.15,
                GeometrySize = 6,
                Stroke = new SolidColorPaint(emissionsColor) { StrokeThickness = 3 },
                Fill = new SolidColorPaint(emissionsColor.WithAlpha(35)),
                GeometryFill = new SolidColorPaint(emissionsColor),
                GeometryStroke = new SolidColorPaint(emissionsColor),
            },
        };

        var minEmissions = co2Emissions.Select(p => p.Value ?? 0d).DefaultIfEmpty(0d).Min();
        var maxEmissions = co2Emissions.Select(p => p.Value ?? 0d).DefaultIfEmpty(0d).Max();

        if (this.Co2EmissionsChart.YAxes.FirstOrDefault() is Axis emissionsAxis)
        {
            emissionsAxis.MinLimit = Math.Floor(Math.Min(0, minEmissions) * 1.1);
            emissionsAxis.MaxLimit = Math.Ceiling(Math.Max(1, maxEmissions) * 1.1);
        }

        // finding active untis and sorting them, by capacity for better graphical look
        var allUnits = orderedResults
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
            SKColor.Parse("#A855F7"),
        };

        var seriesList = new List<ISeries>();
        var colorIdx = 0;

        foreach (var unit in allUnits)
        {
            var color = colors[colorIdx % colors.Length];
            colorIdx++;

            // creating line for each of the units
            var unitPoints = orderedResults.Select(h =>
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

        this.OptimizationResultsChart.Series = new ObservableCollection<ISeries>(seriesList);

        this.OptimizationResultsChart.XAxes = new ObservableCollection<ICartesianAxis>
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

        var maxHeatProduction = orderedResults
            .Select(h => h.HeatProduction)
            .DefaultIfEmpty(0)
            .Max();

        var yMax = Math.Ceiling(maxHeatProduction * 1.10);

        this.OptimizationResultsChart.YAxes = new ObservableCollection<ICartesianAxis>
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

        // Compute max heat production across all production units and apply scaling
        var maxProduction = orderedResults
            .Where(x => x.ProductionUnits is not null)
            .SelectMany(x => x.ProductionUnits.Select(u => (double)u.HeatProduction))
            .DefaultIfEmpty(0)
            .Max();

        var minAllowed = context.Period.Equals("Winter", StringComparison.OrdinalIgnoreCase) ? 14.0 : 5.0;

        if (this.OptimizationResultsChart.YAxes.FirstOrDefault() is Axis resAxis)
        {
            resAxis.MinLimit = 0;
            resAxis.MaxLimit = yMax;
        }
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
                    LabelsPaint = this.GetAxisNamePaint(),
                    SeparatorsPaint = new SolidColorPaint(SKColor.Parse("#CBD5E1")) { StrokeThickness = 1 },
                    TextSize = 12,
                }
            },

            YAxes = new ObservableCollection<ICartesianAxis>
            {
                new Axis
                {
                    LabelsPaint = this.GetAxisNamePaint(),
                    NamePaint = this.GetAxisNamePaint(),
                    SeparatorsPaint = new SolidColorPaint(SKColor.Parse("#CBD5E1")) { StrokeThickness = 1 },
                    TextSize = 12,
                }
            },
        };
    }

    private SolidColorPaint GetAxisNamePaint()
    {
        var isDarkMode = Avalonia.Application.Current?.ActualThemeVariant == Avalonia.Styling.ThemeVariant.Dark;
        var axisTitleColor = isDarkMode ? SKColor.Parse("#E2E8F0") : SKColor.Parse("#334155");

        return new SolidColorPaint(axisTitleColor);
    }
}