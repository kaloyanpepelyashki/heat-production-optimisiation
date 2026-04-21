using Avalonia.Controls;
using Dv.App.ViewModels;

namespace Dv.App;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        this.InitializeComponent();
        this.DataContext = new MainWindowViewModel();
    }
}