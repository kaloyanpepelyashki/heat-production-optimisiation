namespace Dv.App.Services;

using System;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Layout;
using Avalonia.Media;
using Dv.App.Interfaces;

public class DialogService : IDialogService
{
    public async Task ShowValidationDialogAsync(string message)
    {
        var dialog = new Window
        {
            Title = "Error Message",
            Width = 350,
            Height = 150,
            WindowStartupLocation = WindowStartupLocation.CenterOwner,
            CanResize = false,
            RequestedThemeVariant = this.GetCurrentThemeVariant(),
        };

        var text = new TextBlock
        {
            Text = message,
            TextWrapping = TextWrapping.Wrap,
            Margin = new Avalonia.Thickness(20),
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center,
        };

        var button = new Button
        {
            Content = "OK",
            HorizontalAlignment = HorizontalAlignment.Center,
            Margin = new Avalonia.Thickness(0, 10, 0, 0),
        };

        button.Click += (_, _) => dialog.Close();

        var panel = new StackPanel { VerticalAlignment = VerticalAlignment.Center };
        panel.Children.Add(text);
        panel.Children.Add(button);

        dialog.Content = panel;

        await this.ShowDialogAsync(dialog);
    }

    public async Task<bool> ShowConfirmationDialogAsync(
        string period,
        string boilerId,
        DateTime startDateTime,
        DateTime endDateTime,
        DateTime periodStart,
        DateTime periodEnd)
    {
        var dialog = new Window
        {
            Title = "Confirm Maintenance",
            Width = 460,
            Height = 320,
            WindowStartupLocation = WindowStartupLocation.CenterOwner,
            CanResize = false,
            RequestedThemeVariant = this.GetCurrentThemeVariant(),
        };

        var title = new TextBlock
        {
            Text = "Review selected maintenance",
            FontSize = 18,
            FontWeight = FontWeight.SemiBold,
            Margin = new Avalonia.Thickness(0, 0, 0, 12)
        };

        var details = new StackPanel
        {
            Spacing = 8,
            Margin = new Avalonia.Thickness(0, 0, 0, 16)
        };

        details.Children.Add(this.CreateDetailRow("Boiler", boilerId));
        details.Children.Add(this.CreateDetailRow("Period", period));
        details.Children.Add(this.CreateDetailRow("Start", $"{startDateTime:dd.MM.yyyy HH:mm}"));
        details.Children.Add(this.CreateDetailRow("End", $"{endDateTime:dd.MM.yyyy HH:mm}"));
        details.Children.Add(this.CreateDetailRow("Allowed", $"{periodStart:dd.MM.yyyy HH:mm} - {periodEnd:dd.MM.yyyy HH:mm}"));

        var confirmTask = new TaskCompletionSource<bool>();

        var confirmButton = new Button
        {
            Content = "Confirm",
            FontWeight = FontWeight.SemiBold,
            Padding = new Avalonia.Thickness(16, 8),
            HorizontalAlignment = HorizontalAlignment.Right,
            Margin = new Avalonia.Thickness(0, 0, 10, 0),
        };

        confirmButton.Click += (_, _) =>
        {
            confirmTask.TrySetResult(true);
            dialog.Close();
        };

        var cancelButton = new Button
        {
            Content = "Cancel",
            FontWeight = FontWeight.SemiBold,
            Padding = new Avalonia.Thickness(16, 8),
            HorizontalAlignment = HorizontalAlignment.Right,
        };

        cancelButton.Click += (_, _) =>
        {
            confirmTask.TrySetResult(false);
            dialog.Close();
        };

        var buttonRow = new StackPanel
        {
            Orientation = Orientation.Horizontal,
            HorizontalAlignment = HorizontalAlignment.Right,
        };

        buttonRow.Children.Add(confirmButton);
        buttonRow.Children.Add(cancelButton);

        var panel = new StackPanel
        {
            Margin = new Avalonia.Thickness(20),
            Spacing = 12,
        };

        panel.Children.Add(title);
        panel.Children.Add(details);
        panel.Children.Add(buttonRow);

        dialog.Content = panel;

        dialog.Closed += (_, _) =>
        {
            confirmTask.TrySetResult(false);
        };

        await this.ShowDialogAsync(dialog);

        return await confirmTask.Task;
    }

    private StackPanel CreateDetailRow(string label, string value)
    {
        var row = new StackPanel
        {
            Orientation = Orientation.Horizontal,
            Spacing = 8
        };

        row.Children.Add(new TextBlock
        {
            Text = $"{label}:",
            FontWeight = FontWeight.SemiBold,
            Width = 80
        });

        row.Children.Add(new TextBlock
        {
            Text = value,
            TextWrapping = TextWrapping.Wrap
        });

        return row;
    }

    private Avalonia.Styling.ThemeVariant GetCurrentThemeVariant()
    {
        return Avalonia.Application.Current?.ActualThemeVariant ?? Avalonia.Styling.ThemeVariant.Default;
    }

    private async Task ShowDialogAsync(Window dialog)
    {
        var lifetime = Avalonia.Application.Current?.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime;
        
        if (lifetime?.MainWindow != null)
        {
            await dialog.ShowDialog(lifetime.MainWindow);
        }
        else
        {
            dialog.Show();
        }
    }
}