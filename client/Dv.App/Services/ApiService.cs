using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;

namespace Dv.App.Services;

public enum BackendService
{
    Rdm,
    Sdm,
    Opt
}

/// <summary>
/// Interface for the main data retrieval layer that interacts with the backend services.
/// </summary>
public interface IApiService
{
    Task<T?> GetAsync<T>(BackendService service, string endpoint);
    Task<TResponse?> PostAsync<TRequest, TResponse>(BackendService service, string endpoint, TRequest data);
    Task<TResponse?> PutAsync<TRequest, TResponse>(BackendService service, string endpoint, TRequest data);
    Task<bool> DeleteAsync(BackendService service, string endpoint);
}

/// <summary>
/// The data access layer of the Data Visualization tool (client side).
/// This service acts like an HTTP client (e.g., Postman) to call the different backend services.
/// </summary>
public class ApiService : IApiService
{
    private readonly HttpClient _httpClient;
    private readonly JsonSerializerOptions _jsonOptions;
    
    // Map your different Render microservices to their root URLs
    private readonly Dictionary<BackendService, string> _serviceUrls = new()
    {
        { BackendService.Rdm, "https://rdm-api.onrender.com/" },
        { BackendService.Sdm, "https://sdm-api.onrender.com/" },
        { BackendService.Opt, "https://heat-production-optimisiation.onrender.com/" }
    };

    public ApiService()
    {
        _httpClient = new HttpClient();
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
    }

    private string BuildUrl(BackendService service, string endpoint)
    {
        var baseUrl = _serviceUrls[service];
        // Ensure we don't double up on slashes
        return $"{baseUrl.TrimEnd('/')}/{endpoint.TrimStart('/')}";
    }

    public async Task<T?> GetAsync<T>(BackendService service, string endpoint)
    {
        var url = BuildUrl(service, endpoint);
        var response = await _httpClient.GetAsync(url);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<T>(_jsonOptions);
    }

    public async Task<TResponse?> PostAsync<TRequest, TResponse>(BackendService service, string endpoint, TRequest data)
    {
        var url = BuildUrl(service, endpoint);
        var response = await _httpClient.PostAsJsonAsync(url, data, _jsonOptions);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<TResponse>(_jsonOptions);
    }

    public async Task<TResponse?> PutAsync<TRequest, TResponse>(BackendService service, string endpoint, TRequest data)
    {
        var url = BuildUrl(service, endpoint);
        var response = await _httpClient.PutAsJsonAsync(url, data, _jsonOptions);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<TResponse>(_jsonOptions);
    }

    public async Task<bool> DeleteAsync(BackendService service, string endpoint)
    {
        var url = BuildUrl(service, endpoint);
        var response = await _httpClient.DeleteAsync(url);
        return response.IsSuccessStatusCode;
    }
}
