namespace Dv.App.ViewModels;

using System;
using System.Net.Http;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Styling;
using Avalonia.Threading;
using Dv.App.Models;
using Dv.App.Services;

public sealed class SettingsViewModel : ViewModelBase
{
    private static readonly HttpClient PingClient = new() { Timeout = TimeSpan.FromSeconds(8) };

    private string selectedTheme;
    private bool rdmChecking = true;
    private bool rdmOnline;
    private bool sdmChecking = true;
    private bool sdmOnline;
    private bool amChecking = true;
    private bool amOnline;

    public SettingsViewModel()
    {
        var settings = SettingsService.Load();
        this.selectedTheme = settings.ThemeVariant;
        _ = this.CheckServicesAsync();
    }

    public bool IsLightTheme
    {
        get => this.selectedTheme == "Light";
        set
        {
            if (value && this.selectedTheme != "Light")
            {
                this.ApplyTheme("Light");
            }
        }
    }

    public bool IsSystemTheme
    {
        get => this.selectedTheme == "Default";
        set
        {
            if (value && this.selectedTheme != "Default")
            {
                this.ApplyTheme("Default");
            }
        }
    }

    public bool IsDarkTheme
    {
        get => this.selectedTheme == "Dark";
        set
        {
            if (value && this.selectedTheme != "Dark")
            {
                this.ApplyTheme("Dark");
            }
        }
    }

    public bool IsRdmChecking { get => this.rdmChecking; private set => this.SetProperty(ref this.rdmChecking, value); }

    public bool IsRdmOnline { get => this.rdmOnline; private set => this.SetProperty(ref this.rdmOnline, value); }

    public bool IsRdmOffline => !this.rdmChecking && !this.rdmOnline;

    public bool IsSdmChecking { get => this.sdmChecking; private set => this.SetProperty(ref this.sdmChecking, value); }

    public bool IsSdmOnline { get => this.sdmOnline; private set => this.SetProperty(ref this.sdmOnline, value); }

    public bool IsSdmOffline => !this.sdmChecking && !this.sdmOnline;

    public bool IsAmChecking { get => this.amChecking; private set => this.SetProperty(ref this.amChecking, value); }

    public bool IsAmOnline { get => this.amOnline; private set => this.SetProperty(ref this.amOnline, value); }

    public bool IsAmOffline => !this.amChecking && !this.amOnline;

    private void ApplyTheme(string themeVariant)
    {
        this.selectedTheme = themeVariant;

        this.OnPropertyChanged(nameof(this.IsLightTheme));
        this.OnPropertyChanged(nameof(this.IsSystemTheme));
        this.OnPropertyChanged(nameof(this.IsDarkTheme));

        if (Application.Current is not null)
        {
            Application.Current.RequestedThemeVariant = themeVariant switch
            {
                "Light" => ThemeVariant.Light,
                "Dark" => ThemeVariant.Dark,
                _ => ThemeVariant.Default,
            };
        }

        SettingsService.Save(new AppSettings { ThemeVariant = themeVariant });
    }

    private async Task CheckServicesAsync()
    {
        await Task.WhenAll(
            PingAndUpdate(
                "https://rdm-api.onrender.com/",
                online =>
                {
                    this.IsRdmChecking = false;
                    this.IsRdmOnline = online;
                    this.OnPropertyChanged(nameof(this.IsRdmOffline));
                }),
            PingAndUpdate(
                "https://sdm-api.onrender.com/",
                online =>
                {
                    this.IsSdmChecking = false;
                    this.IsSdmOnline = online;
                    this.OnPropertyChanged(nameof(this.IsSdmOffline));
                }),
            PingAndUpdate(
                "https://heat-production-optimisiation.onrender.com/",
                online =>
                {
                    this.IsAmChecking = false;
                    this.IsAmOnline = online;
                    this.OnPropertyChanged(nameof(this.IsAmOffline));
                }));
    }

    private static async Task PingAndUpdate(string url, Action<bool> onResult)
    {
        bool online;
        try
        {
            await PingClient.GetAsync(url, HttpCompletionOption.ResponseHeadersRead);
            online = true;
        }
        catch
        {
            online = false;
        }

        await Dispatcher.UIThread.InvokeAsync(() => onResult(online));
    }
}
