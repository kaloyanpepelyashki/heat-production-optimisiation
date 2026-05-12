using Avalonia.Controls;
using Dv.App.ViewModels;

namespace Dv.App;

public partial class MainWindow : Window
{
    public MainWindow(MainWindowViewModel viewModel)
    {
        this.InitializeComponent();
        this.DataContext = viewModel;
    }
}