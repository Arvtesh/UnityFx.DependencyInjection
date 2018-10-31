// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;

namespace UnityFx.DependencyInjection
{
	/// <summary>
	/// Describes a service with its service type, implementation, and lifetime.
	/// </summary>
	/// <seealso cref="IServiceCollection"/>
	/// <seealso cref="IServiceProvider"/>
	public class ServiceDescriptor : IServiceInfo
	{
		#region data

		private readonly Type _serviceType;
		private readonly Type _implementationType;
		private readonly Func<IServiceProvider, object> _serviceFactory;
		private readonly ServiceLifetime _serviceLifetime;
		private readonly ServiceOptions _serviceOptions;
		private readonly object _serviceInstance;

		#endregion

		#region interface

		/// <summary>
		/// Gets the service type.
		/// </summary>
		/// <seealso cref="ImplementationInstance"/>
		/// <seealso cref="ImplementationFactory"/>
		/// <seealso cref="ServiceType"/>
		public Type ImplementationType => _implementationType;

		/// <summary>
		/// Gets the service factory.
		/// </summary>
		/// <seealso cref="ImplementationInstance"/>
		/// <seealso cref="ImplementationType"/>
		/// <seealso cref="ServiceType"/>
		public Func<IServiceProvider, object> ImplementationFactory => _serviceFactory;

		/// <summary>
		/// Gets the singleton service instance.
		/// </summary>
		/// <seealso cref="ImplementationFactory"/>
		/// <seealso cref="ImplementationType"/>
		/// <seealso cref="ServiceType"/>
		public object ImplementationInstance => _serviceInstance;

		/// <summary>
		/// Initializes a new instance of the <see cref="ServiceDescriptor"/> class with the specified <paramref name="instance"/> as a <see cref="ServiceLifetime.Singleton"/>.
		/// </summary>
		/// <param name="serviceType">Service type.</param>
		/// <param name="instance">Service singleton instance.</param>
		/// <param name="options">Service options.</param>
		/// <exception cref="ArgumentNullException">Thrown if either <paramref name="serviceType"/> or <paramref name="instance"/> is <see langword="null"/>.</exception>
		public ServiceDescriptor(Type serviceType, object instance, ServiceOptions options)
		{
			_serviceType = serviceType ?? throw new ArgumentNullException(nameof(serviceType));
			_serviceInstance = instance ?? throw new ArgumentNullException(nameof(instance));
			_serviceLifetime = ServiceLifetime.Singleton;
			_serviceOptions = options;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ServiceDescriptor"/> class with the specified <paramref name="implementationFactory"/> and <paramref name="lifetime"/>.
		/// </summary>
		/// <param name="serviceType">Service type.</param>
		/// <param name="implementationFactory">Service factory.</param>
		/// <param name="lifetime">Service lifetime.</param>
		/// <param name="options">Service options.</param>
		/// <exception cref="ArgumentNullException">Thrown if either <paramref name="serviceType"/> or <paramref name="implementationFactory"/> is <see langword="null"/>.</exception>
		public ServiceDescriptor(Type serviceType, Func<IServiceProvider, object> implementationFactory, ServiceLifetime lifetime, ServiceOptions options)
		{
			_serviceType = serviceType ?? throw new ArgumentNullException(nameof(serviceType));
			_serviceFactory = implementationFactory ?? throw new ArgumentNullException(nameof(implementationFactory));
			_serviceLifetime = lifetime;
			_serviceOptions = options;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ServiceDescriptor"/> class with the specified <paramref name="implementationType"/> and <paramref name="lifetime"/>.
		/// </summary>
		/// <param name="serviceType">Service type.</param>
		/// <param name="implementationType">Service implementation type.</param>
		/// <param name="lifetime">Service lifetime.</param>
		/// <param name="options">Service options.</param>
		/// <exception cref="ArgumentNullException">Thrown if either <paramref name="serviceType"/> or <paramref name="implementationType"/> is <see langword="null"/>.</exception>
		public ServiceDescriptor(Type serviceType, Type implementationType, ServiceLifetime lifetime, ServiceOptions options)
		{
			if (serviceType == null)
			{
				throw new ArgumentNullException(nameof(serviceType));
			}

			if (implementationType == null)
			{
				throw new ArgumentNullException(nameof(implementationType));
			}

			if (implementationType.IsAbstract || implementationType.IsInterface)
			{
				throw new ArgumentException($"Implementation type registered for service {serviceType.Name} is not concrete ({implementationType.Name}).");
			}

			_serviceType = serviceType;
			_implementationType = implementationType;
			_serviceLifetime = lifetime;
			_serviceOptions = options;
		}

		#endregion

		#region IServiceInfo

		/// <summary>
		/// Gets the service type.
		/// </summary>
		/// <seealso cref="ImplementationType"/>
		/// <seealso cref="ImplementationInstance"/>
		/// <seealso cref="ImplementationFactory"/>
		public Type ServiceType => _serviceType;

		/// <summary>
		/// Gets the service lifetime.
		/// </summary>
		/// <seealso cref="ServiceType"/>
		/// <seealso cref="Options"/>
		public ServiceLifetime Lifetime => _serviceLifetime;

		/// <summary>
		/// Gets the service options.
		/// </summary>
		/// <seealso cref="ServiceType"/>
		/// <seealso cref="Lifetime"/>
		public ServiceOptions Options => _serviceOptions;

		#endregion

		#region implementation
		#endregion
	}
}
