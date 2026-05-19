using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Dv.App.Models;

namespace Dv.App.ViewModels;

public sealed partial class OptimizationCompareViewModel : ObservableObject
{
    private static readonly string[] SeriesColors =
    [
        "#3B82F6", "#F59E0B", "#10B981", "#EF4444",
        "#A855F7", "#F97316", "#06B6D4", "#84CC16",
    ];

    private readonly Dictionary<string, SeriesData> dataStore = new();
    private int colorIndex;

    public enum Layout { Single, TwoColumn, FourGrid }

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsSingleLayout), nameof(IsTwoColumnLayout), nameof(IsFourGridLayout))]
    private Layout selectedLayout = Layout.Single;

    public bool IsSingleLayout => this.SelectedLayout == Layout.Single;
    public bool IsTwoColumnLayout => this.SelectedLayout == Layout.TwoColumn;
    public bool IsFourGridLayout => this.SelectedLayout == Layout.FourGrid;

    [RelayCommand]
    private void SetSingleLayout() => this.SelectedLayout = Layout.Single;

    [RelayCommand]
    private void SetTwoColumnLayout() => this.SelectedLayout = Layout.TwoColumn;

    [RelayCommand]
    private void SetFourGridLayout() => this.SelectedLayout = Layout.FourGrid;

    public ChartSlotViewModel Slot0 { get; } = new();
    public ChartSlotViewModel Slot1 { get; } = new();
    public ChartSlotViewModel Slot2 { get; } = new();
    public ChartSlotViewModel Slot3 { get; } = new();

    public ObservableCollection<CompareSeriesEntry> AvailableResults { get; } = [];
    public bool HasNoResults => this.AvailableResults.Count == 0;

    public OptimizationCompareViewModel()
    {
        this.AvailableResults.CollectionChanged += (_, _) =>
            this.OnPropertyChanged(nameof(this.HasNoResults));
    }

    public void UpdateSourceData(IEnumerable<SourceDataDto> sourceData, OptimizationContext context)
    {
        var filtered = sourceData
            .Where(x => x.TimeFrom >= context.StartDate && x.TimeTo <= context.EndDate)
            .OrderBy(x => x.TimeFrom)
            .ToList();

        this.SetSeries(
            "Heat Demand",
            filtered.Select(x => new LiveChartsCore.Defaults.DateTimePoint(x.TimeFrom, (double)x.HeatDemand)).ToArray(),
            "MWh");

        this.SetSeries(
            "Electricity Price",
            filtered.Select(x => new LiveChartsCore.Defaults.DateTimePoint(x.TimeFrom, (double)x.ElectricityPrice)).ToArray(),
            "DKK / MWh");

        this.PushDataToSlots();
    }

    public void NotifyResultAvailable(
        int periodId,
        int scenarioId,
        OptimisationRunDto data,
        OptimizationContext context)
    {
        var ordered = data.optimisationResultsHourly.OrderBy(h => h.TimeFrom).ToList();

        this.SetSeries(
            "Heat Production",
            ordered.Select(h => new LiveChartsCore.Defaults.DateTimePoint(h.TimeFrom, h.HeatProduction)).ToArray(),
            "MWh");

        this.SetSeries(
            "Electricity Consumption",
            ordered.Select(h => new LiveChartsCore.Defaults.DateTimePoint(h.TimeFrom, (double)h.ElectricityConsumption)).ToArray(),
            "MWh");

        this.SetSeries(
            "Expenses",
            ordered.Select(h => new LiveChartsCore.Defaults.DateTimePoint(h.TimeFrom, h.Expenses)).ToArray(),
            "DKK");

        this.SetSeries(
            "CO2 Emissions",
            ordered.Select(h => new LiveChartsCore.Defaults.DateTimePoint(h.TimeFrom, h.Co2Emissions)).ToArray(),
            "kg / MWh");

        var existing = this.AvailableResults
            .FirstOrDefault(e => e.PeriodId == periodId && e.ScenarioId == scenarioId);
        if (existing is not null)
            this.AvailableResults.Remove(existing);

        this.AvailableResults.Add(new CompareSeriesEntry
        {
            Label = $"{context.Period} – {context.Scenario}",
            PeriodId = periodId,
            ScenarioId = scenarioId,
            Data = data,
            Context = context,
        });

        this.PushDataToSlots();
    }

    private void SetSeries(string key, LiveChartsCore.Defaults.DateTimePoint[] points, string yLabel)
    {
        if (!this.dataStore.ContainsKey(key))
        {
            var color = SeriesColors[this.colorIndex % SeriesColors.Length];
            this.colorIndex++;
            this.dataStore[key] = new SeriesData(points, yLabel, color);
        }
        else
        {
            var existing = this.dataStore[key];
            this.dataStore[key] = existing with { Points = points };
        }
    }

    private void PushDataToSlots()
    {
        var snapshot = (IReadOnlyDictionary<string, SeriesData>)this.dataStore;
        this.Slot0.UpdateDataStore(snapshot);
        this.Slot1.UpdateDataStore(snapshot);
        this.Slot2.UpdateDataStore(snapshot);
        this.Slot3.UpdateDataStore(snapshot);
    }
}
