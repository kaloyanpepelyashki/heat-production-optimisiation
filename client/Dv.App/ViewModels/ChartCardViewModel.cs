using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using LiveChartsCore;
using LiveChartsCore.Kernel.Sketches;

namespace Dv.App.ViewModels;

public class ChartCardViewModel : ObservableObject
{
    private ObservableCollection<ISeries> series = new();
    private ObservableCollection<ICartesianAxis> xAxes = new();
    private ObservableCollection<ICartesianAxis> yAxes = new();
    private string title = string.Empty;

    public ObservableCollection<ISeries> Series
    {
        get => series;
        set => SetProperty(ref series, value);
    }

    public ObservableCollection<ICartesianAxis> XAxes
    {
        get => xAxes;
        set => SetProperty(ref xAxes, value);
    }

    public ObservableCollection<ICartesianAxis> YAxes
    {
        get => yAxes;
        set => SetProperty(ref yAxes, value);
    }

    public string Title
    {
        get => title;
        set => SetProperty(ref title, value);
    }
}
