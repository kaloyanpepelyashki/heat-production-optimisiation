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
        this.OptimiserUrl = serviceUrlProvider.Value.OptimiserUrl;
        this._logger = logger;
    }

    public async Task<OptimisationWrapperDto> RequestOptimisation(OptimisationRequestDto optimisationRequestDto)
    {
        try
        {
            if (!await this.WakeUpService())
            {
                throw new Exception("Wake Up Failed");
            }

            var json = JsonSerializer.Serialize(optimisationRequestDto);
            var url = $"{this.OptimiserUrl}/api/OptimizationResults/optimize";
            var deadline = DateTime.UtcNow.AddSeconds(60);

            using var client = new HttpClient();

            while (true)
            {
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await client.PostAsync(url, content);

                if ((int)response.StatusCode >= 500 && DateTime.UtcNow < deadline)
                {
                    this._logger.LogWarning("Optimiser returned {StatusCode}, retrying…", response.StatusCode);
                    await Task.Delay(5000);
                    continue;
                }

                if ((int)response.StatusCode == 422)
                {
                    var body = await response.Content.ReadAsStringAsync();
                    throw new InvalidOperationException(body);
                }

                if (!response.IsSuccessStatusCode)
                {
                    this._logger.LogError("Optimiser responded with status code: {StatusCode}, {ReasonPhrase}", response.StatusCode, response.ReasonPhrase);
                }

                response.EnsureSuccessStatusCode();
                var responseBody = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<OptimisationWrapperDto>(responseBody);
            }
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

    // Polls until the optimiser is ready — Render cold starts return 503 for up to 85 seconds.
    public async Task<bool> WakeUpService()
    {
        using var client = new HttpClient { Timeout = TimeSpan.FromSeconds(10) };
        var endpoint = $"{this.OptimiserUrl}/api/Health/wakeup";
        var deadline = DateTime.UtcNow.AddSeconds(85);

        while (DateTime.UtcNow < deadline)
        {
            try
            {
                var response = await client.GetAsync(endpoint);

                if ((int)response.StatusCode < 500)
                {
                    return true;
                }

                this._logger.LogInformation("WakeUp: Optimiser returned {StatusCode}, retrying…", response.StatusCode);
                await Task.Delay(5000);
            }
            catch (TaskCanceledException)
            {
                this._logger.LogWarning("WakeUp: Optimiser request timed out, retrying…");
                await Task.Delay(5000);
            }
            catch (Exception e)
            {
                this._logger.LogWarning("WakeUp: Optimiser error: {Message}, retrying…", e.Message);
                await Task.Delay(5000);
            }
        }

        this._logger.LogError("WakeUp: Optimiser timed out after 85s.");
        return false;
    }
}