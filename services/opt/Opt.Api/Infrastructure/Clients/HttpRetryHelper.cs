using System.Net;
using System.Net.Http.Json;

namespace Opt.Api.Infrastructure.Clients;

internal static class HttpRetryHelper
{
    private static readonly int[] BackoffMs = [1000, 2000, 4000, 8000, 16000];

    internal static async Task<T?> GetWithRetryAsync<T>(HttpClient httpClient, string url, CancellationToken cancellationToken)
    {
        for (int attempt = 0; attempt <= BackoffMs.Length; attempt++)
        {
            var response = await httpClient.GetAsync(url, cancellationToken);
            if (response.StatusCode == HttpStatusCode.TooManyRequests)
            {
                if (attempt == BackoffMs.Length)
                {
                    response.EnsureSuccessStatusCode();
                }

                var retryAfter = response.Headers.RetryAfter?.Delta;
                var delay = retryAfter.HasValue
                    ? (int)retryAfter.Value.TotalMilliseconds
                    : BackoffMs[attempt];
                await Task.Delay(delay, cancellationToken);
                continue;
            }

            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<T>(cancellationToken);
        }

        return default;
    }
}
