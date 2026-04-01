using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Input;
using Dv.App.Commands;
using Dv.App.Models;

namespace Dv.App.ViewModels;

public sealed class SourceDataViewModel : ViewModelBase
{
    private static readonly HttpClient HttpClient = new();
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
    };

    private readonly AsyncCommand loadCommand;
    private DateTimeOffset? fromDate;
    private DateTimeOffset? toDate;
    private bool isLoading;
    private string? errorMessage;

    public SourceDataViewModel()
    {
        this.loadCommand = new AsyncCommand(this.LoadAsync, this.CanLoad);
    }

    public ObservableCollection<SourceData> Data { get; } = new();

    public DateTimeOffset? FromDate
    {
        get => this.fromDate;
        set
        {
            if (this.SetProperty(ref this.fromDate, value))
            {
                this.loadCommand.RaiseCanExecuteChanged();
            }
        }
    }

    public DateTimeOffset? ToDate
    {
        get => this.toDate;
        set
        {
            if (this.SetProperty(ref this.toDate, value))
            {
                this.loadCommand.RaiseCanExecuteChanged();
            }
        }
    }

    public bool IsLoading
    {
        get => this.isLoading;
        private set
        {
            if (this.SetProperty(ref this.isLoading, value))
            {
                this.loadCommand.RaiseCanExecuteChanged();
            }
        }
    }

    public string? ErrorMessage
    {
        get => this.errorMessage;
        private set
        {
            if (this.SetProperty(ref this.errorMessage, value))
            {
                this.OnPropertyChanged(nameof(this.HasError));
            }
        }
    }

    public bool HasError => !string.IsNullOrWhiteSpace(this.ErrorMessage);

    public ICommand LoadCommand => this.loadCommand;

    private bool CanLoad()
    {
        return this.FromDate.HasValue && this.ToDate.HasValue && !this.IsLoading;
    }

    private async Task LoadAsync()
    {
        if (!this.FromDate.HasValue || !this.ToDate.HasValue)
        {
            return;
        }

        if (this.FromDate.Value.Date > this.ToDate.Value.Date)
        {
            this.ErrorMessage = "The from date must be before the to date.";
            this.Data.Clear();
            return;
        }

        this.IsLoading = true;
        this.ErrorMessage = null;

        try
        {
            var baseUrl = Environment.GetEnvironmentVariable("DV_FRONTEND_AGGREGATOR_URL") ?? "http://localhost:5005";
            var from = this.FromDate.Value.Date.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
            var to = this.ToDate.Value.Date.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
            var uri = $"{baseUrl.TrimEnd('/')}/api/source-data?from={from}&to={to}";

            using var response = await HttpClient.GetAsync(uri);
            response.EnsureSuccessStatusCode();

            var responseBody = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<List<SourceData>>(responseBody, JsonOptions) ?? new List<SourceData>();

            this.Data.Clear();
            foreach (var item in result.OrderBy(item => item.TimeFrom))
            {
                this.Data.Add(item);
            }
        }
        catch (Exception ex)
        {
            this.Data.Clear();
            this.ErrorMessage = $"Failed to load data: {ex.Message}";
        }
        finally
        {
            this.IsLoading = false;
        }
    }
}
