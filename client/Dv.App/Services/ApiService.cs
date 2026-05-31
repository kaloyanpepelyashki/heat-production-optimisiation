namespace Dv.App.Services;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Dv.App.Interfaces;

public class ApiService : IApiService
{
    private static readonly HttpClient SharedHttpClient = new HttpClient
    {
        Timeout = TimeSpan.FromMinutes(4),
    };

    private readonly JsonSerializerOptions jsonOptions;

    public static readonly IReadOnlyDictionary<BackendService, string> ServiceUrls = new Dictionary<BackendService, string>
    {
        { BackendService.Rdm, "https://rdm-api.onrender.com/" },
        { BackendService.Sdm, "https://sdm-api.onrender.com/" },
        { BackendService.Am, "https://heat-production-optimisiation.onrender.com/" },
        { BackendService.Opt, "https://opt-api-7dj4.onrender.com/" },
    };

    public ApiService()
    {
        this.jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
        };
    }

    public async Task<T?> GetAsync<T>(BackendService service, string endpoint)
    {
        var url = this.BuildUrl(service, endpoint);
        var response = await SharedHttpClient.GetAsync(url);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<T>(this.jsonOptions);
    }

    public async Task<TResponse?> PostAsync<TRequest, TResponse>(BackendService service, string endpoint, TRequest data)
    {
        var url = this.BuildUrl(service, endpoint);
        var response = await SharedHttpClient.PostAsJsonAsync(url, data, this.jsonOptions);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<TResponse>(this.jsonOptions);
    }

    public async Task<TResponse?> PutAsync<TRequest, TResponse>(BackendService service, string endpoint, TRequest data)
    {
        var url = this.BuildUrl(service, endpoint);
        var response = await SharedHttpClient.PutAsJsonAsync(url, data, this.jsonOptions);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<TResponse>(this.jsonOptions);
    }

    public async Task<bool> DeleteAsync(BackendService service, string endpoint)
    {
        var url = this.BuildUrl(service, endpoint);
        var response = await SharedHttpClient.DeleteAsync(url);
        return response.IsSuccessStatusCode;
    }

    private string BuildUrl(BackendService service, string endpoint)
    {
        var baseUrl = ServiceUrls[service];

        return $"{baseUrl.TrimEnd('/')}/{endpoint.TrimStart('/')}";
    }

    public async Task<bool> WakeUpService(BackendService service, CancellationToken token)
    {
        var endpoint = this.BuildUrl(service, "api/Health/wakeup");
        try
        {
            var response = await SharedHttpClient.GetAsync(endpoint, token);
            return response.IsSuccessStatusCode;
        }
        catch (OperationCanceledException) when (token.IsCancellationRequested)
        {
            throw;
        }
        catch (Exception e)
        {
            Debug.WriteLine($"WakeUp: {service} error: {e.Message}");
            return false;
        }
    }

    public async Task<bool> WakeUpAllServices(CancellationToken token = default)
    {
        var tasks = ServiceUrls.Keys.Select(svc => this.WakeUpService(svc, token));
        var results = await Task.WhenAll(tasks);
        return results.All(r => r);
    }
}
