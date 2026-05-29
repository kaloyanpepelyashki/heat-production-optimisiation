namespace Dv.App.Services;

using System.Threading;
using System.Threading.Tasks;

public interface IApiService
{
    Task<T?> GetAsync<T>(BackendService service, string endpoint);

    Task<TResponse?> PostAsync<TRequest, TResponse>(BackendService service, string endpoint, TRequest data);

    Task<TResponse?> PutAsync<TRequest, TResponse>(BackendService service, string endpoint, TRequest data);

    Task<bool> DeleteAsync(BackendService service, string endpoint);

    /// <summary>
    /// Used to wake up an API service based on a provided url from the BackendService enum.
    /// Performs a GET request to the Health endpoint for wakeup.
    /// Will fail if different than 200 status code is returned.
    /// </summary>
    /// <param name="service">the url of the service taken from the BackendService enum</param>
    /// <returns>true if wake up was successful (200 was returned from the request), false if it was not</returns>
    Task<bool> WakeUpService(BackendService service, CancellationToken token);

    /// <summary>
    /// Used to wake up all services at ones. Iterates over the ServiceUrls defined in the APIService.
    /// For each service in the list schedules a task, calling the WakeUpService method - sends a request to the health/wakeup endpoint of each service
    /// Expects all of them to respond with status code 200, to complete with a success.
    /// </summary>
    /// <param name="token"></param>
    /// <returns></returns>
    Task<bool> WakeUpAllServices(CancellationToken token);
}
