namespace Dv.App.Views.Optimization.Components;

using Avalonia.Controls;
using Dv.App.ViewModels;

public partial class GraphsCard : UserControl
{
    private bool attached;

    public GraphsCard()
    {
        this.InitializeComponent();

        this.DataContextChanged += (_, _) => this.TryAttachCharts();
        this.AttachedToVisualTree += (_, _) => this.TryAttachCharts();
    }

    private void TryAttachCharts()
    {
        if (this.attached || this.DataContext is not OptimizationViewModel vm)
        {
            return;
        }

        this.attached = true;

        vm.ChartsVM.AttachCharts(
            this.HeatDemandChart,
            this.ElectricityPriceChart,
            this.OptimizationResultsChart,
            this.ExpensesChart,
            this.Co2EmissionsChart);

        this.MaximizeHeatDemandBtn.Click += (_, _) =>
            vm.ChartsVM.Maximize(this.HeatDemandChart, "Heat Demand");
        this.MaximizeElectricityPriceBtn.Click += (_, _) =>
            vm.ChartsVM.Maximize(this.ElectricityPriceChart, "Electricity Price & Consumption");
        this.MaximizeOptimizationResultsBtn.Click += (_, _) =>
            vm.ChartsVM.Maximize(this.OptimizationResultsChart, "Optimization Results");
        this.MaximizeExpensesBtn.Click += (_, _) =>
            vm.ChartsVM.Maximize(this.ExpensesChart, "Expenses");
        this.MaximizeCo2Btn.Click += (_, _) =>
            vm.ChartsVM.Maximize(this.Co2EmissionsChart, "CO2 Emissions");

        vm.PropertyChanged += (_, e) =>
        {
            if (e.PropertyName == nameof(OptimizationViewModel.HasOptimizationResults))
            {
                this.SetOptimizationSectionVisible(vm.HasOptimizationResults);
            }
        };

        this.SetOptimizationSectionVisible(vm.HasOptimizationResults);
    }

    private void SetOptimizationSectionVisible(bool visible)
    {
        this.OptimizationSection.MaxHeight = visible ? 2000 : 0;
        this.OptimizationSection.Opacity = visible ? 1.0 : 0.0;
    }
}
