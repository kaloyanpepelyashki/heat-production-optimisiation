using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia.Controls;
using Dv.App.Models;
using LiveChartsCore;
using LiveChartsCore.Defaults;
using LiveChartsCore.Kernel.Sketches;
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
    private CartesianChart? electricityConsumptionChart;
    private CartesianChart? expensesChart;
    private CartesianChart? co2EmissionsChart;

    public void AttachCharts(
        CartesianChart heatDemandChart,
        CartesianChart electricityPriceChart,
        CartesianChart optimizationResultsChart,
        CartesianChart electricityConsumptionChart,
        CartesianChart expensesChart,
        CartesianChart co2EmissionsChart)
    {
        this.heatDemandChart = heatDemandChart;
        this.electricityPriceChart = electricityPriceChart;
        this.optimizationResultsChart = optimizationResultsChart;
        this.electricityConsumptionChart = electricityConsumptionChart;
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

        var electricityPrices =
            filtered
                .Select(x => new DateTimePoint(x.TimeFrom, (double)x.ElectricityPrice))
                .ToArray();

        var priceColor = SKColor.Parse("#3B82F6");

        ConfigureSingleChart(heatDemandChart,"Heat Demand", "#F59E0B", "MWh", heatDemand);
        ConfigureSingleChart(electricityPriceChart,"ElectricityPrice", "#3B82F6", "MW", electricityPrices);
    }

    public void LoadOptimizationResult(OptimisationRunDto optimizationResult, OptimizationContext context)
    {
        if (optimizationResult.optimisationResultsHourly is null || optimizationResult.optimisationResultsHourly.Count == 0)
        {
            this.optimizationResultsChart.Series = new List<ISeries>();
            this.electricityConsumptionChart.Series = new List<ISeries>();
            this.expensesChart.Series = new List<ISeries>();
            this.co2EmissionsChart.Series = new List<ISeries>();
            return;
        }

        var orderedResults = optimizationResult.optimisationResultsHourly
            .OrderBy(x => x.TimeFrom)
            .ToList();

        DateTimePoint[] electricityConsumption = orderedResults
            .Select(x => new DateTimePoint(x.TimeFrom, (double)x.ElectricityConsumption))
            .ToArray();

        DateTimePoint[] expenses = orderedResults
            .Select(x => new DateTimePoint(x.TimeFrom, x.Expenses))
            .ToArray();

        DateTimePoint[] co2Emissions = orderedResults
            .Select(x => new DateTimePoint(x.TimeFrom, x.Co2Emissions))
            .ToArray();

        DateTimePoint[] heatProduction = orderedResults
            .Select(x => new DateTimePoint(x.TimeFrom, x.HeatProduction))
            .ToArray();

        ConfigureSingleChart(electricityConsumptionChart,"Electricity Consumption", "#3B82F6", "MW", electricityConsumption);
        ConfigureSingleChart(expensesChart,"Expenses", "#F59E0B", "DKK", expenses);
        ConfigureSingleChart(co2EmissionsChart,"CO2 Emission", "#DC2626", "Kg/MWh", co2Emissions);
        ConfigureSingleChart(optimizationResultsChart,"Optimization Results", "#000000", "MWh", heatProduction);

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
        .OrderBy(u => u.ProductionUnitId)
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
                    StrokeThickness = 3,
                },
                Fill = new SolidColorPaint(color.WithAlpha(150)),
            });
        }

        this.optimizationResultsChart.Series = seriesList;
    }

    private void ConfigureSingleChart(CartesianChart chart, string seriesName, string colorName, string axisName, DateTimePoint[] data)
    {
        SKColor color= SKColor.Parse(colorName);
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


        double minPrice = data.Min(p => p.Value ?? 0);
        double maxPrice = data.Max(p => p.Value ?? 0);

        double minLimit = Math.Floor(Math.Min(0, minPrice) * 1.1);
        double maxLimit = Math.Ceiling(Math.Max(0, maxPrice) * 1.1);

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
                MinLimit = minLimit,
                MaxLimit = maxLimit,
                Name = axisName,
                NamePaint = this.GetAxisNamePaint(),
                LabelsPaint = this.GetAxisNamePaint(),
                SeparatorsPaint = new SolidColorPaint(SKColor.Parse("#CBD5E1")) { StrokeThickness = 1 },
                TextSize = 12,
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