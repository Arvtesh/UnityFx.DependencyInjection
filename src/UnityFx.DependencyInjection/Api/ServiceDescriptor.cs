﻿// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Diagnostics;

namespace UnityFx.AppStates.DependencyInjection
{
	/// <summary>
	/// Describes a service with its service type, implementation, and lifetime.
	/// </summary>
	/// <seealso cref="IServiceCollection"/>
	/// <seealso cref="IServiceProvider"/>
	public class ServiceDescriptor
	{
		#region data

		private readonly Type _serviceType;
		private readonly Type _implementationType;
		private readonly Func<IServiceProvider, object> _serviceFactory;
		private readonly ServiceLifetime _serviceLifetime;
		private readonly object _serviceInstance;

		#endregion

		#region interface

		/// <summary>
		/// Gets the service type.
		/// </summary>
		/// <seealso cref="ImplementationType"/>
		/// <seealso cref="ImplementationInstance"/>
		/// <seealso cref="ImplementationFactory"/>
		public Type ServiceType => _serviceType;

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
		/// Gets the service lifetime.
		/// </summary>
		/// <seealso cref="ServiceType"/>
		public ServiceLifetime Lifetime => _serviceLifetime;

		/// <summary>
		/// Initializes a new instance of the <see cref="ServiceDescriptor"/> class with the specified <paramref name="instance"/> as a <see cref="ServiceLifetime.Singleton"/>.
		/// </summary>
		/// <param name="serviceType">Service type.</param>
		/// <param name="instance">Service singleton instance.</param>
		/// <exception cref="ArgumentNullException">Thrown if either <paramref name="serviceType"/> or <paramref name="instance"/> is <see langword="null"/>.</exception>
		public ServiceDescriptor(Type serviceType, object instance)
		{
			_serviceType = serviceType ?? throw new ArgumentNullException(nameof(serviceType));
			_serviceInstance = instance ?? throw new ArgumentNullException(nameof(instance));
			_serviceLifetime = ServiceLifetime.Singleton;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ServiceDescriptor"/> class with the specified <paramref name="implementationFactory"/> and <paramref name="lifetime"/>.
		/// </summary>
		/// <param name="serviceType">Service type.</param>
		/// <param name="implementationFactory">Service factory.</param>
		/// <param name="lifetime">Service lifetime.</param>
		/// <exception cref="ArgumentNullException">Thrown if either <paramref name="serviceType"/> or <paramref name="implementationFactory"/> is <see langword="null"/>.</exception>
		public ServiceDescriptor(Type serviceType, Func<IServiceProvider, object> implementationFactory, ServiceLifetime lifetime)
		{
			_serviceType = serviceType ?? throw new ArgumentNullException(nameof(serviceType));
			_serviceFactory = implementationFactory ?? throw new ArgumentNullException(nameof(implementationFactory));
			_serviceLifetime = lifetime;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ServiceDescriptor"/> class with the specified <paramref name="implementationType"/> and <paramref name="lifetime"/>.
		/// </summary>
		/// <param name="serviceType">Service type.</param>
		/// <param name="implementationType">Service implementation type.</param>
		/// <param name="lifetime">Service lifetime.</param>
		/// <exception cref="ArgumentNullException">Thrown if either <paramref name="serviceType"/> or <paramref name="implementationType"/> is <see langword="null"/>.</exception>
		public ServiceDescriptor(Type serviceType, Type implementationType, ServiceLifetime lifetime)
		{
			_serviceType = serviceType ?? throw new ArgumentNullException(nameof(serviceType));
			_implementationType = implementationType ?? throw new ArgumentNullException(nameof(implementationType));
			_serviceLifetime = lifetime;
		}

		#endregion

		#region implementation
		#endregion
	}
}
