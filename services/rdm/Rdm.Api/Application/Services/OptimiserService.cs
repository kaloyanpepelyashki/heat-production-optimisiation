namespace Rdm.Api.Application.Services;

using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Options;
using Rdm.Api.Application.Interfaces;
using Rdm.Api.Inrastructure.Configuration;
using Rdm.Api.Inrastructure.DTOs;

public class OptimiserService : IOptimiserService
{
    private ILogger<OptimiserService> _logger;
    private string? OptimiserUrl;

    public OptimiserService(IOptions<ServiceUrlProvider> serviceUrlProvider, ILogger<OptimiserService> logger)
    {
        this.OptimiserUrl = serviceUrlProvider.Value.OptimiserUrl;
        this._logger = logger;
    }

    public async Task<OptimisationWrapperDto> RequestOptimisation(OptimisationRequestDto optimisationRequestDto)
    {
        try
        {
            bool wakeUpCallResponse = await this.WakeUpService();

            if (!wakeUpCallResponse)
            {
                throw new Exception("Wake Up Failed");
            }

            HttpClient client = new HttpClient();

            var json = JsonSerializer.Serialize(optimisationRequestDto);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await client.PostAsync($"{this.OptimiserUrl}/api/OptimizationResults/optimize", content);

            if ((int)response.StatusCode == 422)
            {
                var body = await response.Content.ReadAsStringAsync();
                throw new InvalidOperationException(body);
            }

            if (!response.IsSuccessStatusCode)
            {
                this._logger.LogError($"Optimiser responded with status code: {response.StatusCode}, {response.Content}, {response.ReasonPhrase}");
            }

            response.EnsureSuccessStatusCode();
            var responseBody = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<OptimisationWrapperDto>(responseBody);

            return result;
        }
        catch (HttpRequestException e)
        {
            this._logger.LogError(
                $"Error in OptimiserService. Connectivity error when requesting optimisation: {e.Message} {e.GetType()} Status Code: {e.StatusCode}, {e.HttpRequestError}");
            throw;
        }
        catch (TaskCanceledException e)
        {
            this._logger.LogError($"Error in OptimiserService. Request timed out: {e.Message} {e.GetType()}");
            throw;
        }
        catch (JsonException e)
        {
            this._logger.LogError($"Error in OptimiserService. Json serialization exception: {e.Message} {e.GetType()}");
            throw;
        }
        catch (Exception e)
        {
            this._logger.LogError($"Error in OptimiserService: {e.Message} {e.GetType()}");
            throw;
        }
    }

    public async Task<bool> WakeUpService()
    {
        try
        {
            HttpClient client = new HttpClient();

            var response = await client.GetAsync($"{this.OptimiserUrl}/api/Health/wakeup");

            if (response.IsSuccessStatusCode)
            {
                return true;
            }

            return false;
        }
        catch (HttpRequestException e)
        {
            this._logger.LogError($"Error in OptimiserService. Connectivity error when waking up optimiser: {e.Message} {e.GetType()} Status Code: {e.StatusCode}, {e.HttpRequestError}");
            throw;
        }
        catch (TaskCanceledException e)
        {
            this._logger.LogError($"Error in OptimiserService when waking up optimiser service. Request timed out: {e.Message} {e.GetType()}");
            return false;
        }
        catch (Exception e)
        {
            this._logger.LogError($"Error in OptimiserService in wake up call: {e.Message} {e.GetType()}");
            throw;
        }
    }
}