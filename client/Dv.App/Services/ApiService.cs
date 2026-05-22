namespace Dv.App.Services;

using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Linq;
using System.Threading;

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
    
    
    /// Polls service/api/Health/wakeup until it responds with a non-5xx status or 90 seconds elapse.
    /// Render.com free-tier services return 5xx during cold start; a single request is not enough.
    public async Task<bool> WakeUpService(BackendService service, CancellationToken token)
    {
        var endpoint = this.BuildUrl(service, "api/Health/wakeup");
        var deadline = DateTime.UtcNow.AddSeconds(90);

        while (DateTime.UtcNow < deadline)
        {
            try
            {
                var response = await SharedHttpClient.GetAsync(endpoint, token);

                if ((int)response.StatusCode >= 500)
                {
                    Debug.WriteLine($"WakeUp: {service} returned {(int)response.StatusCode}, retrying…");
                    await Task.Delay(5000, token);
                    continue;
                }

                // 2xx or 4xx — the server is handling requests
                return true;
            }
            catch (OperationCanceledException) when (token.IsCancellationRequested)
            {
                Debug.WriteLine($"WakeUp: {service} cancelled.");
                throw;
            }
            catch (Exception e)
            {
                Debug.WriteLine($"WakeUp: {service} error: {e.Message}, retrying…");
                await Task.Delay(5000, token);
            }
        }

        Debug.WriteLine($"WakeUp: {service} timed out after 90s.");
        return false;
    }

    public async Task<bool> WakeUpAllServices(CancellationToken token = default)
    {
        var tasks = ServiceUrls.Keys
            .ToDictionary(svc => svc, svc => WakeUpServiceSafe(svc, token));

        await Task.WhenAll(tasks.Values);

        return new[] { BackendService.Rdm, BackendService.Sdm, BackendService.Am }
            .All(svc => tasks[svc].Result);
    }

    private async Task<bool> WakeUpServiceSafe(BackendService service, CancellationToken token)
    {
        try
        {
            return await WakeUpService(service, token);
        }
        catch (OperationCanceledException) when (token.IsCancellationRequested)
        {
            throw;
        }
        catch (Exception e)
        {
            Debug.WriteLine($"WakeUp failed for {service}: {e.Message}");
            return false;
        }
    }
    
    
}
