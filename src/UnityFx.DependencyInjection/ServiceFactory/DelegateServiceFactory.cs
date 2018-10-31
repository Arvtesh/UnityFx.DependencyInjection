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
		private readonly ServiceOptions _serviceOptions;

		#endregion

		#region interface

		public DelegateServiceFactory(IServiceInfo serviceInfo, Func<IServiceProvider, object> serviceFactory)
		{
			_serviceType = serviceInfo.ServiceType;
			_factory = serviceFactory;
			_serviceLifetime = serviceInfo.Lifetime;
			_serviceOptions = serviceInfo.Options;
		}

		#endregion

		#region IServiceFactory

		public Type ServiceType => _serviceType;

		public ServiceLifetime Lifetime => _serviceLifetime;

		public ServiceOptions Options => _serviceOptions;

		public object CreateInstance(IServiceProvider serviceProvider) => _factory.Invoke(serviceProvider);

		#endregion
	}
}
