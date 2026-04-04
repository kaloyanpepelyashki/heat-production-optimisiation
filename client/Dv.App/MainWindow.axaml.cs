// <copyright file="MainWindow.axaml.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

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