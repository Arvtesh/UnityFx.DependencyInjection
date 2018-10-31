// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace UnityFx.DependencyInjection
{
	/// <summary>
	/// Extensions for <see cref="IServiceCollection"/>.
	/// </summary>
	[EditorBrowsable(EditorBrowsableState.Advanced)]
	public static class ServiceCollectionExtensions
	{
		/// <summary>
		/// Adds a sequence of <see cref="ServiceDescriptor"/> to the <paramref name="collection"/>.
		/// </summary>
		/// <param name="collection">The <see cref="IServiceCollection"/> instance.</param>
		/// <param name="descriptors">The <see cref="ServiceDescriptor"/>s to add.</param>
		public static void Add(this IServiceCollection collection, IEnumerable<ServiceDescriptor> descriptors)
		{
			if (descriptors == null)
			{
				throw new ArgumentNullException(nameof(descriptors));
			}

			foreach (var descriptor in descriptors)
			{
				collection.Add(descriptor);
			}
		}

		/// <summary>
		/// Removes the first service in <see cref="IServiceCollection"/> with the same service type as <paramref name="descriptor"/>
		/// and adds <paramef name="descriptor"/> to the collection.
		/// </summary>
		/// <param name="collection">The <see cref="IServiceCollection"/>.</param>
		/// <param name="descriptor">The <see cref="ServiceDescriptor"/> to replace with.</param>
		public static void Replace(this IServiceCollection collection, ServiceDescriptor descriptor)
		{
			if (descriptor == null)
			{
				throw new ArgumentNullException(nameof(descriptor));
			}

			collection.Remove(descriptor.ServiceType);
			collection.Add(descriptor);
		}

		/// <summary>
		/// Removes the first services of type <typeparamef name="T"/> in <see cref="IServiceCollection"/>.
		/// </summary>
		/// <param name="collection">The <see cref="IServiceCollection"/>.</param>
		public static bool Remove<T>(this IServiceCollection collection)
		{
			return collection.Remove(typeof(T));
		}

		/// <summary>
		/// Removes all services of type <typeparamef name="T"/> in <see cref="IServiceCollection"/>.
		/// </summary>
		/// <param name="collection">The <see cref="IServiceCollection"/>.</param>
		public static bool RemoveAll<T>(this IServiceCollection collection)
		{
			return RemoveAll(collection, typeof(T));
		}

		/// <summary>
		/// Removes all services of type <paramef name="serviceType"/> in <see cref="IServiceCollection"/>.
		/// </summary>
		/// <param name="collection">The <see cref="IServiceCollection"/>.</param>
		/// <param name="serviceType">The service type to remove.</param>
		public static bool RemoveAll(this IServiceCollection collection, Type serviceType)
		{
			var result = false;

			while (true)
			{
				if (collection.Remove(serviceType))
				{
					result = true;
				}
				else
				{
					break;
				}
			}

			return result;
		}

		/// <summary>
		/// Adds an instance of <see cref="ServiceDescriptor"/> with the specified <paramref name="serviceType"/> and <paramref name="implementationType"/> and the <see cref="ServiceLifetime.Singleton"/> lifetime.
		/// </summary>
		/// <param name="services">Target service collection.</param>
		/// <param name="serviceType">Type of the service.</param>
		/// <param name="implementationType">Type of the service implementation.</param>
		public static void AddSingleton(this IServiceCollection services, Type serviceType, Type implementationType)
		{
			services.Add(new ServiceDescriptor(serviceType, implementationType, ServiceLifetime.Singleton));
		}

		/// <summary>
		/// Adds an instance of <see cref="ServiceDescriptor"/> with the specified <paramref name="serviceType"/> and the <see cref="ServiceLifetime.Singleton"/> lifetime.
		/// </summary>
		/// <param name="services">Target service collection.</param>
		/// <param name="serviceType">Type of the service/implementation.</param>
		public static void AddSingleton(this IServiceCollection services, Type serviceType)
		{
			services.Add(new ServiceDescriptor(serviceType, serviceType, ServiceLifetime.Singleton));
		}

		/// <summary>
		/// Adds an instance of <see cref="ServiceDescriptor"/> with the specified <paramref name="serviceType"/> and <paramref name="instance"/> and the <see cref="ServiceLifetime.Singleton"/> lifetime.
		/// </summary>
		/// <param name="services">Target service collection.</param>
		/// <param name="serviceType">Type of the service.</param>
		/// <param name="instance">The service instance.</param>
		public static void AddSingleton(this IServiceCollection services, Type serviceType, object instance)
		{
			services.Add(new ServiceDescriptor(serviceType, instance));
		}

		/// <summary>
		/// Adds an instance of <see cref="ServiceDescriptor"/> with the specified <paramref name="serviceType"/> and <paramref name="implementationFactory"/> and the <see cref="ServiceLifetime.Singleton"/> lifetime.
		/// </summary>
		/// <param name="services">Target service collection.</param>
		/// <param name="serviceType">Type of the service.</param>
		/// <param name="implementationFactory">Factory delegate for the service instances.</param>
		public static void AddSingleton(this IServiceCollection services, Type serviceType, Func<IServiceProvider, object> implementationFactory)
		{
			services.Add(new ServiceDescriptor(serviceType, implementationFactory, ServiceLifetime.Singleton));
		}

		/// <summary>
		/// Adds an instance of <see cref="ServiceDescriptor"/> with the specified <typeparamref name="TService"/> and <typeparamref name="TImplementation"/> and the <see cref="ServiceLifetime.Singleton"/> lifetime.
		/// </summary>
		/// <param name="services">Target service collection.</param>
		/// <typeparam name="TService">Type of the service.</typeparam>
		/// <typeparam name="TImplementation">Type of the service implementation.</typeparam>
		public static void AddSingleton<TService, TImplementation>(this IServiceCollection services) where TService : class where TImplementation : class, TService
		{
			services.Add(new ServiceDescriptor(typeof(TService), typeof(TImplementation), ServiceLifetime.Singleton));
		}

		/// <summary>
		/// Adds an instance of <see cref="ServiceDescriptor"/> with the specified <typeparamref name="TService"/> and the <see cref="ServiceLifetime.Singleton"/> lifetime.
		/// </summary>
		/// <param name="services">Target service collection.</param>
		/// <typeparam name="TService">Type of the service/implementation.</typeparam>
		public static void AddSingleton<TService>(this IServiceCollection services) where TService : class
		{
			services.Add(new ServiceDescriptor(typeof(TService), typeof(TService), ServiceLifetime.Singleton));
		}

		/// <summary>
		/// Adds an instance of <see cref="ServiceDescriptor"/> with the specified <typeparamref name="TService"/> and <paramref name="instance"/> and the <see cref="ServiceLifetime.Singleton"/> lifetime.
		/// </summary>
		/// <param name="services">Target service collection.</param>
		/// <typeparam name="TService">Type of the service.</typeparam>
		/// <param name="instance">The singleton instance.</param>
		public static void AddSingleton<TService>(this IServiceCollection services, TService instance) where TService : class
		{
			services.Add(new ServiceDescriptor(typeof(TService), instance));
		}

		/// <summary>
		/// Adds an instance of <see cref="ServiceDescriptor"/> with the specified <paramref name="serviceType"/> and <paramref name="implementationType"/> and the <see cref="ServiceLifetime.Transient"/> lifetime.
		/// </summary>
		/// <param name="services">Target service collection.</param>
		/// <param name="serviceType">Type of the service.</param>
		/// <param name="implementationType">Type of the service implementation.</param>
		public static void AddTransient(this IServiceCollection services, Type serviceType, Type implementationType)
		{
			services.Add(new ServiceDescriptor(serviceType, implementationType, ServiceLifetime.Transient));
		}

		/// <summary>
		/// Adds an instance of <see cref="ServiceDescriptor"/> with the specified <paramref name="serviceType"/> and the <see cref="ServiceLifetime.Transient"/> lifetime.
		/// </summary>
		/// <param name="services">Target service collection.</param>
		/// <param name="serviceType">Type of the service/implementation.</param>
		public static void AddTransient(this IServiceCollection services, Type serviceType)
		{
			services.Add(new ServiceDescriptor(serviceType, serviceType, ServiceLifetime.Transient));
		}

		/// <summary>
		/// Adds an instance of <see cref="ServiceDescriptor"/> with the specified <paramref name="serviceType"/> and <paramref name="implementationFactory"/> and the <see cref="ServiceLifetime.Transient"/> lifetime.
		/// </summary>
		/// <param name="services">Target service collection.</param>
		/// <param name="serviceType">Type of the service.</param>
		/// <param name="implementationFactory">Factory delegate for the service instances.</param>
		public static void AddTransient(this IServiceCollection services, Type serviceType, Func<IServiceProvider, object> implementationFactory)
		{
			services.Add(new ServiceDescriptor(serviceType, implementationFactory, ServiceLifetime.Transient));
		}

		/// <summary>
		/// Adds an instance of <see cref="ServiceDescriptor"/> with the specified <typeparamref name="TService"/> and <typeparamref name="TImplementation"/> and the <see cref="ServiceLifetime.Transient"/> lifetime.
		/// </summary>
		/// <param name="services">Target service collection.</param>
		/// <typeparam name="TService">Type of the service.</typeparam>
		/// <typeparam name="TImplementation">Type of the service implementation.</typeparam>
		public static void AddTransient<TService, TImplementation>(this IServiceCollection services) where TService : class where TImplementation : class, TService
		{
			services.Add(new ServiceDescriptor(typeof(TService), typeof(TImplementation), ServiceLifetime.Transient));
		}

		/// <summary>
		/// Adds an instance of <see cref="ServiceDescriptor"/> with the specified <typeparamref name="TService"/> and the <see cref="ServiceLifetime.Transient"/> lifetime.
		/// </summary>
		/// <param name="services">Target service collection.</param>
		/// <typeparam name="TService">Type of the service/implementation.</typeparam>
		public static void AddTransient<TService>(this IServiceCollection services) where TService : class
		{
			services.Add(new ServiceDescriptor(typeof(TService), typeof(TService), ServiceLifetime.Transient));
		}

		/// <summary>
		/// Adds an instance of <see cref="ServiceDescriptor"/> with the specified <paramref name="serviceType"/> and <paramref name="implementationType"/> and the <see cref="ServiceLifetime.Scoped"/> lifetime.
		/// </summary>
		/// <param name="services">Target service collection.</param>
		/// <param name="serviceType">Type of the service.</param>
		/// <param name="implementationType">Type of the service implementation.</param>
		public static void AddScoped(this IServiceCollection services, Type serviceType, Type implementationType)
		{
			services.Add(new ServiceDescriptor(serviceType, implementationType, ServiceLifetime.Scoped));
		}

		/// <summary>
		/// Adds an instance of <see cref="ServiceDescriptor"/> with the specified <paramref name="serviceType"/> and the <see cref="ServiceLifetime.Scoped"/> lifetime.
		/// </summary>
		/// <param name="services">Target service collection.</param>
		/// <param name="serviceType">Type of the service/implementation.</param>
		public static void AddScoped(this IServiceCollection services, Type serviceType)
		{
			services.Add(new ServiceDescriptor(serviceType, serviceType, ServiceLifetime.Scoped));
		}

		/// <summary>
		/// Adds an instance of <see cref="ServiceDescriptor"/> with the specified <paramref name="serviceType"/> and <paramref name="implementationFactory"/> and the <see cref="ServiceLifetime.Scoped"/> lifetime.
		/// </summary>
		/// <param name="services">Target service collection.</param>
		/// <param name="serviceType">Type of the service.</param>
		/// <param name="implementationFactory">Factory delegate for the service instances.</param>
		public static void AddScoped(this IServiceCollection services, Type serviceType, Func<IServiceProvider, object> implementationFactory)
		{
			services.Add(new ServiceDescriptor(serviceType, implementationFactory, ServiceLifetime.Scoped));
		}

		/// <summary>
		/// Adds an instance of <see cref="ServiceDescriptor"/> with the specified <typeparamref name="TService"/> and <typeparamref name="TImplementation"/> and the <see cref="ServiceLifetime.Scoped"/> lifetime.
		/// </summary>
		/// <param name="services">Target service collection.</param>
		/// <typeparam name="TService">Type of the service.</typeparam>
		/// <typeparam name="TImplementation">Type of the service implementation.</typeparam>
		public static void AddScoped<TService, TImplementation>(this IServiceCollection services) where TService : class where TImplementation : class, TService
		{
			services.Add(new ServiceDescriptor(typeof(TService), typeof(TImplementation), ServiceLifetime.Scoped));
		}

		/// <summary>
		/// Adds an instance of <see cref="ServiceDescriptor"/> with the specified <typeparamref name="TService"/> and the <see cref="ServiceLifetime.Scoped"/> lifetime.
		/// </summary>
		/// <param name="services">Target service collection.</param>
		/// <typeparam name="TService">Type of the service.</typeparam>
		public static void AddScoped<TService>(this IServiceCollection services) where TService : class
		{
			services.Add(new ServiceDescriptor(typeof(TService), typeof(TService), ServiceLifetime.Scoped));
		}
	}
}
