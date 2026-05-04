using System.Net;
using System.Net.Http.Json;

namespace Opt.Api.Infrastructure.Clients;

internal static class HttpRetryHelper
{
    private static readonly int[] BackoffMs = [1000, 3000, 10000, 30000, 60000];

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
                await Task.Delay(delay, cancellationToken);
                continue;
            }

            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<T>(cancellationToken);
        }

        return default;
    }
}
