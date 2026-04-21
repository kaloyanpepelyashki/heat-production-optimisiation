namespace Dv.App.Services;

using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;

public class ApiService : IApiService
{
    private static readonly HttpClient SharedHttpClient = new HttpClient();
    private readonly JsonSerializerOptions jsonOptions;

    private readonly Dictionary<BackendService, string> serviceUrls = new()
    {
        { BackendService.Rdm, "https://rdm-api.onrender.com/" },
        { BackendService.Sdm, "https://sdm-api.onrender.com/" },
        { BackendService.Am, "https://heat-production-optimisiation.onrender.com/" },
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
        var baseUrl = this.serviceUrls[service];

        return $"{baseUrl.TrimEnd('/')}/{endpoint.TrimStart('/')}";
    }
}
