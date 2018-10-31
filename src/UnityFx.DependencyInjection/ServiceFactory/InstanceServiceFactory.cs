// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;

namespace UnityFx.DependencyInjection
{
	internal class InstanceServiceFactory : IServiceFactory
	{
		#region data

		private readonly Type _serviceType;
		private readonly object _instance;
		private readonly ServiceOptions _serviceOptions;

		#endregion

		#region interface

		public InstanceServiceFactory(IServiceInfo serviceInfo, object instance)
		{
			_serviceType = serviceInfo.ServiceType;
			_instance = instance;
			_serviceOptions = serviceInfo.Options;
		}

		#endregion

		#region IServiceFactory

		public Type ServiceType => _serviceType;

		public ServiceLifetime Lifetime => ServiceLifetime.Singleton;

		public ServiceOptions Options => _serviceOptions;

		public object CreateInstance(IServiceProvider serviceProvider) => _instance;

		#endregion
	}
}
