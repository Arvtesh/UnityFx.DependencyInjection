// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;

namespace UnityFx.DependencyInjection
{
	internal class DelegateServiceFactory : IServiceFactory
	{
		#region data

		private readonly Type _serviceType;
		private readonly Func<IServiceProvider, object> _factory;
		private readonly ServiceLifetime _serviceLifetime;

		#endregion

		#region interface

		public DelegateServiceFactory(Type serviceType, Func<IServiceProvider, object> serviceFactory, ServiceLifetime serviceLifetime)
		{
			_serviceType = serviceType;
			_factory = serviceFactory;
			_serviceLifetime = serviceLifetime;
		}

		#endregion

		#region IServiceFactory

		public Type ServiceType => _serviceType;

		public ServiceLifetime Lifetime => _serviceLifetime;

		public object CreateInstance(IServiceProvider serviceProvider) => _factory.Invoke(serviceProvider);

		#endregion
	}
}
