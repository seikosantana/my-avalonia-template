using System;
using System.Collections.Generic;
using CommunityToolkit.Mvvm.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MyAvaloniaTemplate.Abstractions;
using MyAvaloniaTemplate.Models.Settings;
using MyAvaloniaTemplate.Services;
using MyAvaloniaTemplate.ViewModels;

namespace MyAvaloniaTemplate.Extensions;

public static class IocExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection serviceCollection,
        bool dummyMode = false)
    {
        if (dummyMode)
        {
            serviceCollection.AddSingleton<ISettingsService<SettingsModel>, DummySettingsService>();
        }
        else
        {
            serviceCollection.AddSingleton<ISettingsService<SettingsModel>, SettingsJsonService>();
        }

        serviceCollection.AddSingleton<MainViewModel>();
        return serviceCollection;
    }

    private static bool _hasConfiguredIoc;

    public static void ConfigureApplicationServices(this Ioc ioc, bool dummyMode = false)
    {
        if (_hasConfiguredIoc) return;

        ioc.ConfigureServices(new ServiceCollection()
            .AddApplicationServices(dummyMode)
            .AddLogging(logging =>
            {
                logging.ClearProviders();
                logging.AddConsole();
                if (OperatingSystem.IsWindows())
                {
                    logging.AddEventLog();
                }

                logging.SetMinimumLevel(LogLevel.Information);
            })
            .BuildServiceProvider());

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
        ILogger<Host> logger = serviceProvider.GetRequiredService<ILogger<Host>>();
        logger.LogInformation("Creating background services");
        foreach (var type in BackgroundServiceTypes)
        {
            logger.LogDebug("Creating background service {typeName}", type.Name);
            serviceProvider.GetService(type);
        }
    }

    public static void CleanupBackgroundServices(this IServiceProvider serviceProvider)
    {
        ILogger<Host> logger = serviceProvider.GetRequiredService<ILogger<Host>>();
        logger.LogInformation("Cleaning up background services");
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
                    logger.LogDebug("Cleaning background service {typeName}", type.Name);
                    backgroundService.Cleanup().GetAwaiter().GetResult();
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Error cleaning up background service {typeName}", type.Name);
                }

                try
                {
                    logger.LogDebug("Disposing background service {typeName}", type.Name);
                    backgroundService.Dispose();
                }
                catch (Exception ex)
                {
                    logger.LogError("Error disposing background service {typeName}", type.Name);
                }
            }
        }
    }
}