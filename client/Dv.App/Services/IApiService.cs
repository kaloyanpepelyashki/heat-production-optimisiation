// <copyright file="IApiService.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace Dv.App.Services;

using System.Threading.Tasks;

/// <summary>
/// Interface for the main data retrieval layer that interacts with the backend services.
/// </summary>
public interface IApiService
{
    Task<T?> GetAsync<T>(BackendService service, string endpoint);

    Task<TResponse?> PostAsync<TRequest, TResponse>(BackendService service, string endpoint, TRequest data);

    Task<TResponse?> PutAsync<TRequest, TResponse>(BackendService service, string endpoint, TRequest data);

    Task<bool> DeleteAsync(BackendService service, string endpoint);
}
