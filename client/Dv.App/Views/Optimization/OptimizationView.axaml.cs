using Avalonia.Controls;
using Avalonia.VisualTree;
using Dv.App.ViewModels;

namespace Dv.App.Views;

public partial class OptimizationView : UserControl
{
    public OptimizationView()
    {
        InitializeComponent();

        this.AttachedToVisualTree += (_, _) =>
        {
            if (this.DataContext is OptimizationViewModel vm)
            {
                if (vm.RefreshCommand is not null && vm.RefreshCommand.CanExecute(null))
                {
                    vm.RefreshCommand.Execute(null);
                }
            }
        };
    }
    
}