using Avalonia.Controls;
using Dv.App.ViewModels;

namespace Dv.App.Views.Optimization.Components;

public partial class GraphsCard : UserControl
{
    public GraphsCard()
    {
        InitializeComponent();

        this.DataContextChanged += (_, _) => this.TryAttachCharts();
        this.AttachedToVisualTree += (_, _) => this.TryAttachCharts();
    }

    private void TryAttachCharts()
    {
        if (this.DataContext is not OptimizationViewModel vm)
        {
            return;
        }

        vm.ChartsVM.AttachCharts(
            this.HeatDemandChart,
            this.ElectricityPriceChart,
            this.OptimizationResultsChart,
            this.ElectricityConsumptionChart,
            this.ExpensesChart,
            this.Co2EmissionsChart);
    }
}