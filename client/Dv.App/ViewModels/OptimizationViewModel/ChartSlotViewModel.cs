using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LiveChartsCore;
using LiveChartsCore.Defaults;
using LiveChartsCore.Kernel.Sketches;
using LiveChartsCore.Measure;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;

namespace Dv.App.ViewModels;

public sealed record SeriesData(DateTimePoint[] Points, string YAxisLabel, string Color);

public sealed partial class ChartSlotViewModel : ObservableObject
{
    private IReadOnlyDictionary<string, SeriesData> dataStore =
        new Dictionary<string, SeriesData>();

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(CompareSeries), nameof(YAxes), nameof(HasData))]
    private string? primarySeriesKey;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(CompareSeries), nameof(YAxes), nameof(HasData))]
    private string? overlaySeriesKey;

    public ObservableCollection<string> AvailableSeriesKeys { get; } = [];

    public IEnumerable<ISeries> CompareSeries { get; private set; } = Array.Empty<ISeries>();
    public ICartesianAxis[] XAxes { get; private set; } = BuildDefaultXAxes();
    public ICartesianAxis[] YAxes { get; private set; } = BuildDefaultYAxes();
    public bool HasData => this.PrimarySeriesKey is not null;
    public LegendPosition ChartLegendPosition =>
        this.CompareSeries.Count() > 1 ? LegendPosition.Bottom : LegendPosition.Hidden;

    [RelayCommand]
    private void ClearOverlay() => this.OverlaySeriesKey = null;

    public void UpdateDataStore(IReadOnlyDictionary<string, SeriesData> store)
    {
        this.dataStore = store;

        var keys = store.Keys.OrderBy(k => k).ToList();
        this.AvailableSeriesKeys.Clear();
        foreach (var key in keys)
            this.AvailableSeriesKeys.Add(key);

        Rebuild();
    }

    partial void OnPrimarySeriesKeyChanged(string? value) => Rebuild();
    partial void OnOverlaySeriesKeyChanged(string? value) => Rebuild();

    public void Rebuild()
    {
        this.dataStore.TryGetValue(this.PrimarySeriesKey ?? "", out var primary);

        var hasOverlay = this.OverlaySeriesKey is not null
                         && this.OverlaySeriesKey != this.PrimarySeriesKey
                         && this.dataStore.TryGetValue(this.OverlaySeriesKey, out _);
        this.dataStore.TryGetValue(this.OverlaySeriesKey ?? "", out var overlay);

        // Only use a second Y-axis when both series exist with different units.
        var useDualAxis = hasOverlay
                          && primary is not null
                          && overlay is not null
                          && overlay.YAxisLabel != primary.YAxisLabel;

        var series = new List<ISeries>();

        if (primary is not null && this.PrimarySeriesKey is not null)
        {
            var color = SKColor.Parse(primary.Color);
            series.Add(new LineSeries<DateTimePoint>
            {
                Name = this.PrimarySeriesKey,
                Values = primary.Points,
                LineSmoothness = 0.15,
                GeometrySize = 5,
                Stroke = new SolidColorPaint(color) { StrokeThickness = 2 },
                Fill = new SolidColorPaint(color.WithAlpha(30)),
                GeometryFill = new SolidColorPaint(color),
                GeometryStroke = new SolidColorPaint(color),
                ScalesYAt = 0,
            });
        }

        if (hasOverlay && overlay is not null)
        {
            var color = SKColor.Parse(overlay.Color);
            series.Add(new LineSeries<DateTimePoint>
            {
                Name = this.OverlaySeriesKey,
                Values = overlay.Points,
                LineSmoothness = 0.15,
                GeometrySize = 5,
                Stroke = new SolidColorPaint(color) { StrokeThickness = 2 },
                Fill = new SolidColorPaint(color.WithAlpha(30)),
                GeometryFill = new SolidColorPaint(color),
                GeometryStroke = new SolidColorPaint(color),
                ScalesYAt = useDualAxis ? 1 : 0,
            });
        }

        this.CompareSeries = series;
        this.YAxes = BuildYAxes(
            primary?.YAxisLabel ?? string.Empty,
            useDualAxis ? overlay?.YAxisLabel : null);

        this.OnPropertyChanged(nameof(this.CompareSeries));
        this.OnPropertyChanged(nameof(this.YAxes));
        this.OnPropertyChanged(nameof(this.HasData));
        this.OnPropertyChanged(nameof(this.ChartLegendPosition));
    }

    private static ICartesianAxis[] BuildDefaultXAxes() =>
    [
        new DateTimeAxis(TimeSpan.FromHours(24), date => date.ToString("dd MMM"))
        {
            LabelsPaint = GetAxisPaint(),
            SeparatorsPaint = new SolidColorPaint(SKColor.Parse("#CBD5E1")) { StrokeThickness = 1 },
            TextSize = 12,
        },
    ];

    private static ICartesianAxis[] BuildDefaultYAxes() =>
    [
        new Axis
        {
            LabelsPaint = GetAxisPaint(),
            SeparatorsPaint = new SolidColorPaint(SKColor.Parse("#CBD5E1")) { StrokeThickness = 1 },
            TextSize = 12,
        },
    ];

    private static ICartesianAxis[] BuildYAxes(string primaryLabel, string? overlayLabel)
    {
        var axes = new List<ICartesianAxis>
        {
            new Axis
            {
                Name = primaryLabel,
                NamePaint = GetAxisPaint(),
                LabelsPaint = GetAxisPaint(),
                SeparatorsPaint = new SolidColorPaint(SKColor.Parse("#CBD5E1")) { StrokeThickness = 1 },
                TextSize = 12,
            },
        };

        if (overlayLabel is not null && overlayLabel != primaryLabel)
        {
            axes.Add(new Axis
            {
                Name = overlayLabel,
                NamePaint = GetAxisPaint(),
                LabelsPaint = GetAxisPaint(),
                SeparatorsPaint = new SolidColorPaint(SKColors.Transparent),
                Position = LiveChartsCore.Measure.AxisPosition.End,
                TextSize = 12,
            });
        }

        return axes.ToArray();
    }

    private static SolidColorPaint GetAxisPaint()
    {
        var isDark = Avalonia.Application.Current?.ActualThemeVariant == Avalonia.Styling.ThemeVariant.Dark;
        return new SolidColorPaint(isDark ? SKColor.Parse("#E2E8F0") : SKColor.Parse("#334155"));
    }
}
