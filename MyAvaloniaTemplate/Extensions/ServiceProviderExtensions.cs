using System;
using Microsoft.Extensions.DependencyInjection;
using MyAvaloniaTemplate.ViewModels;

namespace MyAvaloniaTemplate.Extensions;

public static class ServiceProviderExtensions
{
    public static MainViewModel GetMainViewModel(this IServiceProvider serviceProvider)
    {
        return serviceProvider.GetRequiredService<MainViewModel>();
    }
}