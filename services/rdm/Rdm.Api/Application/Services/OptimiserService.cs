using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Options;
using Rdm.Api.Application.Interfaces;
using Rdm.Api.Application.Model;
using Rdm.Api.Inrastructure.Configuration;
using Rdm.Api.Inrastructure.DTOs;

namespace Rdm.Api.Application.Services;


public class OptimiserService : IOptimiserService
{
    private ILogger<OptimiserService> _logger;
    private string? OptimiserUrl;
    
    public OptimiserService(IOptions<ServiceUrlProvider> serviceUrlProvider, ILogger<OptimiserService> logger)
    {
        OptimiserUrl = serviceUrlProvider.Value.OptimiserUrl;
        
        _logger = logger;
    }
    
    public async Task<OptimisationWrapperDto> RequestOptimisation(OptimisationRequestDto optimisationRequestDto)
    {
        try
        {
            if (!await WakeUpService())
                throw new Exception("Wake Up Failed");

            var json = JsonSerializer.Serialize(optimisationRequestDto);
            var url = $"{OptimiserUrl}/api/OptimizationResults/optimize";

            using var client = new HttpClient();
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await client.PostAsync(url, content);

            if ((int)response.StatusCode == 422)
            {
                var body = await response.Content.ReadAsStringAsync();
                throw new InvalidOperationException(body);
            }

            if (!response.IsSuccessStatusCode)
                _logger.LogError("Optimiser responded with status code: {StatusCode}, {ReasonPhrase}", response.StatusCode, response.ReasonPhrase);

            response.EnsureSuccessStatusCode();
            var responseBody = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<OptimisationWrapperDto>(responseBody);
            
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

    public async Task<bool> WakeUpService()
    {
        using var client = new HttpClient { Timeout = TimeSpan.FromSeconds(10) };
        var endpoint = $"{OptimiserUrl}/api/Health/wakeup";
        try
        {
            var response = await client.GetAsync(endpoint);
            return response.IsSuccessStatusCode;
        }
        catch (Exception e)
        {
            _logger.LogError("WakeUp: {Message}", e.Message);
            return false;
        }
    }
}