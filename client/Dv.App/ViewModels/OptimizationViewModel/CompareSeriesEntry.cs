using CommunityToolkit.Mvvm.ComponentModel;
using Dv.App.Models;

namespace Dv.App.ViewModels;

public sealed partial class CompareSeriesEntry : ObservableObject
{
    [ObservableProperty]
    private bool isSelected;

    public string Label { get; init; } = string.Empty;
    public int PeriodId { get; init; }
    public int ScenarioId { get; init; }
    public OptimisationRunDto Data { get; init; } = null!;
    public OptimizationContext Context { get; init; } = null!;
}
