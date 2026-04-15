// <copyright file="ApiService.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace Dv.App.Services;

using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;

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
        { BackendService.Rdm, "https://rdm-api.onrender.com/" },
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

    /// <inheritdoc/>
    public async Task<TResponse?> PostAsync<TRequest, TResponse>(BackendService service, string endpoint, TRequest data)
    {
        var url = this.BuildUrl(service, endpoint);
        var response = await this.httpClient.PostAsJsonAsync(url, data, this.jsonOptions);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<TResponse>(this.jsonOptions);
    }

    /// <inheritdoc/>
    public async Task<TResponse?> PutAsync<TRequest, TResponse>(BackendService service, string endpoint, TRequest data)
    {
        var url = this.BuildUrl(service, endpoint);
        var response = await this.httpClient.PutAsJsonAsync(url, data, this.jsonOptions);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<TResponse>(this.jsonOptions);
    }

    /// <inheritdoc/>
    public async Task<bool> DeleteAsync(BackendService service, string endpoint)
    {
        var url = this.BuildUrl(service, endpoint);
        var response = await this.httpClient.DeleteAsync(url);
        return response.IsSuccessStatusCode;
    }

    private string BuildUrl(BackendService service, string endpoint)
    {
        var baseUrl = this.serviceUrls[service];

        // Ensure we don't double up on slashes
        return $"{baseUrl.TrimEnd('/')}/{endpoint.TrimStart('/')}";
    }
}
