using Avalonia.Controls;
using Dv.App.ViewModels;

namespace Dv.App.Views.Optimization.Components;

public partial class CompareChartSlot : UserControl
{
    public CompareChartSlot()
    {
        InitializeComponent();
        this.SizeChanged += (_, _) =>
        {
            if (DataContext is ChartSlotViewModel vm)
                vm.Rebuild();
        };
    }
}
