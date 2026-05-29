namespace Dv.App;

using Avalonia.Controls;
using Dv.App.ViewModels;

public partial class MainWindow : Window
{
    public MainWindow(MainWindowViewModel viewModel)
    {
        this.InitializeComponent();
        this.DataContext = viewModel;
    }
}