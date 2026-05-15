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
    private static readonly HttpClient SharedHttpClient = new HttpClient();
    private readonly JsonSerializerOptions jsonOptions;

    public static readonly IReadOnlyDictionary<BackendService, string> ServiceUrls = new Dictionary<BackendService, string>
    {
        { BackendService.Rdm, "https://rdm-api.onrender.com/" },
        { BackendService.Sdm, "https://sdm-api.onrender.com/" },
        { BackendService.Am, "https://heat-production-optimisiation.onrender.com/" },
        { BackendService.Opt, "https://opt-api-7dj4.onrender.com"}
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
    
    
    /// <summary>
    /// Used to wake up an API service based on a provided url from the BackendService enum.
    /// Performs a GET request to the Health endpoint for wakeup.
    /// Will fail if different than 200 status code is returned. 
    /// </summary>
    /// <param name="service">the url of the service taken from the BackendService enum</param>
    /// <returns>true if wake up was successful (200 was returned from the request), false if it was not</returns>
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
            Debug.WriteLine($"Wake-up cancelled for service {service} at {endpoint}.");
            throw;
        }
        catch (HttpRequestException e)
        {
            Debug.WriteLine($"Error in APIService. Connectivity error when waking up service at: {endpoint}: {e.Message} {e.GetType()} Status Code: {e.StatusCode}, {e.HttpRequestError}");
            throw;
        }
        catch (TaskCanceledException e)
        {
            Debug.WriteLine($"Error in APIService when waking up service at: {endpoint}. Request timed out: {e.Message} {e.GetType()}");
            throw;
        }
        catch (Exception e)
        {
            Debug.WriteLine($"Error waking up service at:  {endpoint}. {e.Message} {e.GetType()}");
            throw; 
        }
    }

    /// <summary>
    /// Used to wake up all services at ones. Iterates over the ServiceUrls defined in the APIService.
    /// For each service in the list schedules a task, calling the WakeUpService method - sends a request to the health/wakeup endpoint of each service
    /// Expects all of them to respond with status code 200, to complete with a success. 
    /// </summary>
    /// <param name="token"></param>
    /// <returns></returns>
    public async Task<bool> WakeUpAllServices(CancellationToken token = default)
    {
            List<Task<bool>> tasks = new List<Task<bool>>();
        
            
            foreach (KeyValuePair<BackendService, string> pair in ServiceUrls)
            { 
                tasks.Add(WakeUpService(pair.Key, token));
            }
            
            bool[] results = await Task.WhenAll(tasks);

            return results.All(success => success);
    }
    
    
}
