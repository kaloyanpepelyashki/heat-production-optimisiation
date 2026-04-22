using System;
using System.Diagnostics;

// Used for await - for the dialogs
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Interactivity;

using Dv.App.Models;

namespace Dv.App.Views;

public partial class OptimizationView : UserControl
{
    public OptimizationView()
    {
        this.InitializeComponent();
    }

    private async void ConfirmMaintenanceSummer_Click(object? sender, RoutedEventArgs e)
    {
        var selectedItem = BoilersListBoxSummer.SelectedItem as ListBoxItem;
        string boilerId = selectedItem?.Tag?.ToString() ?? "No Boiler Selected";

        var startDate = MaintenanceStartDatePickerSummer.SelectedDate;
        var startTime = MaintenanceStartTimePickerSummer.SelectedTime;

        var endDate = MaintenanceEndDatePickerSummer.SelectedDate;
        var endTime = MaintenanceEndTimePickerSummer.SelectedTime;
        
        if (startDate.HasValue && startTime.HasValue && endDate.HasValue && endTime.HasValue)
        {
            var startDateTime = startDate.Value.Date + startTime.Value;
            var endDateTime = endDate.Value.Date + endTime.Value;

            // YYYY/MM/DD + H/MM
            var periodStart = new DateTime(2025, 9, 8, 0, 0, 0);
            var periodEnd = new DateTime(2025, 9, 21, 23, 59, 59);

            if (startDateTime >= endDateTime)
            {
                await ShowValidationDialog("The Maintenance cannot end before it starts.");
                return;
            }

            if (startDateTime < periodStart || endDateTime > periodEnd)
            {
                await ShowValidationDialog($"Maintenance interval must be within the Summer period:\n{periodStart:dd.MM.yyyy HH:mm} to {periodEnd:dd.MM.yyyy HH:mm}");
                return;
            }


            // Printing into the debug console to check if it works
            Debug.WriteLine("=== Summer Maintenance Scheduled ===");
            Debug.WriteLine($"Boiler: {boilerId}");
            Debug.WriteLine($"Start: {startDateTime:dd.MM.yyyy HH:mm}");
            Debug.WriteLine($"End:   {endDateTime:dd.MM.yyyy HH:mm}");
            Debug.WriteLine("=============================");

            MaintenanceStore.MaintenanceSchedules.Add(new MaintenanceEvent
            {
                AssetName = boilerId,
                StartDate = startDateTime,
                EndDate = endDateTime,
                Period = "Summer",
                Scenario = "1"
            });
        }
        else
        {
            await ShowValidationDialog("Please completely fill out the Date/Time fields.");
        }
    }

    private async void ConfirmMaintenanceWinter_Click(object? sender, RoutedEventArgs e)
    {
        var selectedItem = BoilersListBoxWinter.SelectedItem as ListBoxItem;
        string boilerId = selectedItem?.Tag?.ToString() ?? "No Boiler Selected";

        var startDate = MaintenanceStartDatePickerWinter.SelectedDate;
        var startTime = MaintenanceStartTimePickerWinter.SelectedTime;

        var endDate = MaintenanceEndDatePickerWinter.SelectedDate;
        var endTime = MaintenanceEndTimePickerWinter.SelectedTime;

        if (startDate.HasValue && startTime.HasValue && endDate.HasValue && endTime.HasValue)
        {
            var startDateTime = startDate.Value.Date + startTime.Value;
            var endDateTime = endDate.Value.Date + endTime.Value;

            // YYYY/MM/DD + H/MM
            var periodStart = new DateTime(2026, 1, 5, 0, 0, 0);
            var periodEnd = new DateTime(2026, 1, 18, 23, 59, 59);

            if (startDateTime >= endDateTime)
            {
                await ShowValidationDialog("The Maintenance cannot end before it starts.");
                return;
            }

            if (startDateTime < periodStart || endDateTime > periodEnd)
            {
                await ShowValidationDialog($"Maintenance interval must be within the Winter period:\n{periodStart:dd.MM.yyyy HH:mm} to {periodEnd:dd.MM.yyyy HH:mm}");
                return;
            }

            // Printing into the debug console to check if it works
            Debug.WriteLine("=== Winter Maintenance Scheduled ===");
            Debug.WriteLine($"Boiler: {boilerId}");
            Debug.WriteLine($"Start: {startDateTime:dd.MM.yyyy HH:mm}");
            Debug.WriteLine($"End:   {endDateTime:dd.MM.yyyy HH:mm}");
            Debug.WriteLine("=============================");

            MaintenanceStore.MaintenanceSchedules.Add(new MaintenanceEvent
            {
                AssetName = boilerId,
                StartDate = startDateTime,
                EndDate = endDateTime,
                Period = "Winter",
                Scenario = "1"
            });
        }
        else
        {
            await ShowValidationDialog("Please completely fill out the Date/Time fields.");
        }
    }

    // Error message that pops up when the user enters an invalid time interval
    // await does not work without async (makes the program wait for the user to close the window)
    private async Task ShowValidationDialog(string message)
    {
        var dialog = new Window
        {
            Title = "Error Message",
            Width = 350,
            Height = 150,
            WindowStartupLocation = WindowStartupLocation.CenterOwner,
            CanResize = false
        };

        var text = new TextBlock
        {
            Text = message,
            TextWrapping = Avalonia.Media.TextWrapping.Wrap,
            Margin = new Avalonia.Thickness(20),
            HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center,
            VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center
        };

        var button = new Button
        {
            Content = "OK",
            HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center,
            Margin = new Avalonia.Thickness(0, 10, 0, 0)
        };

        // Event handler
        button.Click += (_, _) => dialog.Close();

        var panel = new StackPanel { VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center };
        panel.Children.Add(text);
        panel.Children.Add(button);

        dialog.Content = panel;

        // The VisualRoot might not always be a window, the reason for this code
        if (this.VisualRoot is Window mainWindow)
        {
            await dialog.ShowDialog(mainWindow);
        }
        else
        {
            dialog.Show();
        }
    }
}
