﻿using System.Data;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace TomLonghurst.DependencyInjection.EnumerableServiceDecorator;

public static class DependencyInjectionExtensions
{
    public static IServiceCollection FlattenEnumerableToSingle<T>(this IServiceCollection services)
    {
        if (services.IsReadOnly)
        {
            throw new ReadOnlyException($"{nameof(services)} is read only");
        }

        if (!typeof(T).IsInterface)
        {
            throw new ArgumentException($"Please register {nameof(T)} as an interface");
        }

        var serviceDescriptors = services
            .Where(descriptor => descriptor.ServiceType == typeof(T))
            .ToList();

        services.RemoveAll<T>();
        
        var type = Assembly.GetCallingAssembly().GetType($"{typeof(T).Namespace}.{typeof(T).FullName.Replace(".", string.Empty)}EnumerableServiceDecorator");

        foreach (var serviceDescriptor in serviceDescriptors)
        {
            services.Add(new ServiceDescriptor(typeof(IDecoratorInterfaceWrapper<T>), GetServiceDecorator<T>(serviceDescriptor), serviceDescriptor.Lifetime));
        }

        services.AddTransient(typeof(T), type);

        return services;
    }

    private static Func<IServiceProvider, object> GetServiceDecorator<T>(ServiceDescriptor serviceDescriptor)
    {
        return sp =>
        {
            var constructedInstance = serviceDescriptor.ImplementationInstance ?? ActivatorUtilities.GetServiceOrCreateInstance(sp, serviceDescriptor.ImplementationType);

            return new DecoratorClassWrapper<T>((T)constructedInstance);
        };
    }
}