using Dv.App.Services;
using Dv.App.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Dv.App;

public static class ServiceCollectionExtension
{
    public static void AddCommonServices(this IServiceCollection services)
    {
        services.AddSingleton<IApiService, ApiService>();
        services.AddLogging(builder =>
        {
            builder.AddDebug();
            builder.AddConsole();
            builder.SetMinimumLevel(LogLevel.Debug);
        });
        
        services.AddTransient<IDialogService, DialogService>();

        services.AddSingleton<MainWindowViewModel>();
        services.AddSingleton<ProductionUnitsViewModel>();
        services.AddSingleton<DashboardViewModel>();
        services.AddSingleton<OptimizationViewModel>();
        services.AddSingleton<SourceDataViewModel>();
        services.AddSingleton<SettingsViewModel>();
        
        services.AddSingleton<MainWindow>();
        
    }
}