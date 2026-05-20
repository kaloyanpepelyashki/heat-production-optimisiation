using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Options;
using Rdm.Api.Application.Interfaces;
using Rdm.Api.Application.Model;
using Rdm.Api.Inrastructure.Configuration;
using Rdm.Api.Inrastructure.DTOs;

namespace Rdm.Api.Application.Services;

/// <summary>
/// The serivce is responsible for handling all interactions with the Optimiser module.
/// </summary>
public class OptimiserService : IOptimiserService
{
    private ILogger<OptimiserService> _logger;
    private string? OptimiserUrl;
    
    public OptimiserService(IOptions<ServiceUrlProvider> serviceUrlProvider, ILogger<OptimiserService> logger)
    {
        OptimiserUrl = serviceUrlProvider.Value.OptimiserUrl;
        
        _logger = logger;
    }
    
    /// <summary>
    /// Sends an optimisation request to the optimiser module via a HTTP client and returns the optimisation result.
    /// Handles request serialization, HTTP communication, response deserialization, and logs any request or JSON errors.
    /// </summary>
    /// <param name="optimisationRequestDto">The optimisation input data used by the Optimiser.</param>
    /// <returns>The optimisation result returned from the Optimiser service.</returns>
    public async Task<OptimisationWrapperDto> RequestOptimisation(OptimisationRequestDto optimisationRequestDto)
    {
        try
        {
            if (!await WakeUpService())
                throw new Exception("Wake Up Failed");

            var json = JsonSerializer.Serialize(optimisationRequestDto);
            var url = $"{OptimiserUrl}/api/OptimizationResults/optimize";
            var deadline = DateTime.UtcNow.AddSeconds(60);

            using var client = new HttpClient();

            while (true)
            {
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await client.PostAsync(url, content);

                if ((int)response.StatusCode >= 500 && DateTime.UtcNow < deadline)
                {
                    _logger.LogWarning("Optimiser returned {StatusCode}, retrying…", response.StatusCode);
                    await Task.Delay(5000);
                    continue;
                }

                if (!response.IsSuccessStatusCode)
                    _logger.LogError("Optimiser responded with status code: {StatusCode}, {ReasonPhrase}", response.StatusCode, response.ReasonPhrase);

                response.EnsureSuccessStatusCode();
                var responseBody = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<OptimisationWrapperDto>(responseBody);
            }
            
        }
        catch (HttpRequestException e)
        {
            _logger.LogError(
                $"Error in OptimiserService. Connectivity error when requesting optimisation: {e.Message} {e.GetType()} Status Code: {e.StatusCode}, {e.HttpRequestError}");
            throw;
        }
        catch (TaskCanceledException e)
        {
            _logger.LogError($"Error in OptimiserService. Request timed out: {e.Message} {e.GetType()}");
            throw;
        }
        catch (JsonException e)
        {
            _logger.LogError($"Error in OptimiserService. Json serialization exception: {e.Message} {e.GetType()}");
            throw;
        }
        catch (Exception e)
        {
            _logger.LogError($"Error in OptimiserService: {e.Message} {e.GetType()}");
            throw e;
        }
    }

    /// <summary>
    /// Send a GET request to the optimiser service, to wake it up. 
    /// </summary>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    public async Task<bool> WakeUpService()
    {
        try
        {
            HttpClient client = new HttpClient();
            
            var response = await client.GetAsync($"{OptimiserUrl}/api/Health/wakeup");

            if (response.IsSuccessStatusCode)
            {
                return true;
            } 
            
            return false;
            
            
        }  catch (HttpRequestException e)
        {
            _logger.LogError($"Error in OptimiserService. Connectivity error when waking up optimiser: {e.Message} {e.GetType()} Status Code: {e.StatusCode}, {e.HttpRequestError}");
            throw;
        }
        catch (TaskCanceledException e)
        {
            _logger.LogError($"Error in OptimiserService when waking up optimiser service. Request timed out: {e.Message} {e.GetType()}");
            return false;
        }
        catch (Exception e)
        {
            _logger.LogError($"Error in OptimiserService in wake up call: {e.Message} {e.GetType()}");
            throw e;
        }
    }
}