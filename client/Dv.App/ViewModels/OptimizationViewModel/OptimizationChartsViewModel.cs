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
    }

    public void LoadOptimizationResult(
        OptimisationRunDto optimizationResult,
        OptimizationContext context)
    {
        if (optimizationResult.optimisationResultsHourly is null
            || optimizationResult.optimisationResultsHourly.Count == 0)
        {
            this.OptimizationResultsChart.Series =
                new ObservableCollection<ISeries>();

            this.ElectricityConsumptionChart.Series =
                new ObservableCollection<ISeries>();

            this.ExpensesChart.Series = new ObservableCollection<ISeries>();

            this.Co2EmissionsChart.Series = new ObservableCollection<ISeries>();

            return;
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

        var productionUnitSeries =
            orderedResults
                .Where(x => x.ProductionUnits is not null)
                .SelectMany(x => x.ProductionUnits.Select(unit => new
                {
                    Result = x,
                    Unit = unit,
                }))
                .GroupBy(x => x.Unit.ProductionUnitType)
                .Select(group =>
                    new LineSeries<DateTimePoint>
                        {
                            Name = group.Key.ToString(),

                            Values = group
                                    .OrderBy(x => x.Result.TimeFrom)
                                    .Select(x => new DateTimePoint(x.Result.TimeFrom, (double)x.Unit.HeatProduction))
                                    .ToArray(),

                            LineSmoothness = 0.15,
                            GeometrySize = 6,
                            Stroke = new SolidColorPaint(SKColors.LightGray) { StrokeThickness = 2 },
                            GeometryFill = new SolidColorPaint(SKColors.LightGray),
                            GeometryStroke = new SolidColorPaint(SKColors.LightGray),
                        })
                    .Cast<ISeries>()
                    .ToList();

        this.OptimizationResultsChart.Series =
            new ObservableCollection<ISeries>(
                productionUnitSeries);
    }

    private ChartCardViewModel CreateChart(
        string title)
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