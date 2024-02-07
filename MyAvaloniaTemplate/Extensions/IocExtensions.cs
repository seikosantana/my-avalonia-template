using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MyAvaloniaTemplate.Abstractions;

namespace MyAvaloniaTemplate.Extensions;

public static class IocExtensions
{
    private static bool _hasConfiguredIoc;
    private static bool _hasCreatedBackgroundServices;

    public static async Task ConfigureAsync(this Ioc ioc,
        Func<IServiceCollection, Task> configureAction)
    {
        if (_hasConfiguredIoc)
        {
            Console.WriteLine("Ioc has already been configured");
            return;
        }

        var serviceCollection = new ServiceCollection();
        await configureAction(serviceCollection);
        var serviceProvider = serviceCollection.BuildServiceProvider();
        ioc.ConfigureServices(serviceProvider);
        _hasConfiguredIoc = true;
    }

    public static void Configure(this Ioc ioc,
        Action<IServiceCollection> configureAction)
    {
        if (_hasConfiguredIoc)
        {
            Console.WriteLine("Ioc has already been configured");
            return;
        }

        var serviceCollection = new ServiceCollection();
        configureAction(serviceCollection);
        var serviceProvider = serviceCollection.BuildServiceProvider();
        ioc.ConfigureServices(serviceProvider);
        _hasConfiguredIoc = true;
    }

    private static readonly List<Type> BackgroundServiceTypes = new List<Type>();

    public static IServiceCollection AddBackgroundService<TInterface, TImplementation>(
        this IServiceCollection serviceCollection)
        where TInterface : class, IBackgroundService
        where TImplementation : class, TInterface
    {
        BackgroundServiceTypes.Add(typeof(TInterface));
        serviceCollection.AddSingleton<TInterface, TImplementation>();
        return serviceCollection;
    }

    public static IServiceCollection AddBackgroundService<TImplementation>(
        this IServiceCollection serviceCollection)
        where TImplementation : class, IBackgroundService
    {
        BackgroundServiceTypes.Add(typeof(TImplementation));
        serviceCollection.AddSingleton<TImplementation>();
        return serviceCollection;
    }

    public static void CreateBackgroundServices(this IServiceProvider serviceProvider)
    {
        if (_hasCreatedBackgroundServices)
        {
            Console.WriteLine("Background service has already been created");
            return;
        }

        ILogger<Ioc>? logger = serviceProvider.GetService<ILogger<Ioc>>();
        logger?.LogInformation("Creating background services");
        foreach (var type in BackgroundServiceTypes)
        {
            logger?.LogDebug("Creating background service {typeName}", type.Name);
            serviceProvider.GetService(type);
        }

        _hasCreatedBackgroundServices = true;
    }

    public static void CleanupBackgroundServices(this IServiceProvider serviceProvider)
    {
        ILogger<Ioc>? logger = serviceProvider.GetService<ILogger<Ioc>>();
        logger?.LogInformation("Cleaning up background services");
        foreach (var type in BackgroundServiceTypes)
        {
            var service = serviceProvider.GetService(type);
            if (service is null)
            {
                return;
            }

            if (service is IBackgroundService backgroundService)
            {
                try
                {
                    logger?.LogDebug("Cleaning background service {typeName}", type.Name);
                    backgroundService.Cleanup().GetAwaiter().GetResult();
                }
                catch (Exception ex)
                {
                    logger?.LogError(ex, "Error cleaning up background service {typeName}", type.Name);
                }

                try
                {
                    logger?.LogDebug("Disposing background service {typeName}", type.Name);
                    backgroundService.Dispose();
                }
                catch (Exception ex)
                {
                    logger?.LogError("Error disposing background service {typeName}", type.Name);
                }
            }
        }
    }
}