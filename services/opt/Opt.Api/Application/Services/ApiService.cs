namespace Opt.Api.Application.Services;

using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using Opt.Api.Application.Interfaces;

/// <summary>
/// The data access layer of the Data Visualization tool (client side).
/// This service acts like an HTTP client (e.g., Postman) to call the different backend services.
/// </summary>
public class ApiService : IApiService
{
    private readonly HttpClient httpClient;
    private readonly JsonSerializerOptions jsonOptions;

    private readonly Dictionary<BackendService, string> serviceUrls = new()
    {
        { BackendService.Sdm, "https://sdm-api.onrender.com/" },
        { BackendService.Am, "https://am-api.onrender.com/" },
    };

    public ApiService()
    {
        this.httpClient = new HttpClient();
        this.jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
        };
    }

    /// <inheritdoc/>
    public async Task<T?> GetAsync<T>(BackendService service, string endpoint)
    {
        var url = this.BuildUrl(service, endpoint);
        var response = await this.httpClient.GetAsync(url);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<T>(this.jsonOptions);
    }

    private string BuildUrl(BackendService service, string endpoint)
    {
        var baseUrl = this.serviceUrls[service];

        // Ensure we don't double up on slashes
        return $"{baseUrl.TrimEnd('/')}/{endpoint.TrimStart('/')}";
    }
}