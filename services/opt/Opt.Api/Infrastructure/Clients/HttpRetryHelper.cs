namespace Opt.Api.Infrastructure.Clients;

using System.Net;
using System.Net.Http.Json;

internal static class HttpRetryHelper
{
    private static readonly int[] BackoffMs = [1000, 5000, 30000, 60000];

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

                var retryAfterMs = response.Headers.RetryAfter?.Delta is { } delta
                    ? (int)delta.TotalMilliseconds
                    : 0;
                var delay = Math.Max(retryAfterMs, BackoffMs[attempt]);
                var jitter = Random.Shared.Next(0, delay / 5);
                await Task.Delay(delay + jitter, cancellationToken);
                continue;
            }

            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<T>(cancellationToken);
        }

        return default;
    }
}
