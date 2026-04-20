namespace Dv.App;

using Avalonia.Controls;
using Dv.App.ViewModels;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        this.InitializeComponent();
        this.DataContext = new MainWindowViewModel();
    }
}