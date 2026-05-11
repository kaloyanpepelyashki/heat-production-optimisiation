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
            bool wakeUpCallResponse = await WakeUpService();

            if (!wakeUpCallResponse)
            {
                throw new Exception("Wake Up Failed");
            }
            
            HttpClient client = new HttpClient();

            var json = JsonSerializer.Serialize(optimisationRequestDto);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await client.PostAsync($"{OptimiserUrl}/api/OptimizationResults/optimize", content);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError($"Optimiser responded with status code: {response.StatusCode}, {response.Content}, {response.ReasonPhrase}");
            }

            response.EnsureSuccessStatusCode();
            var responseBody = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<OptimisationWrapperDto>(responseBody);

            return result;
            
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