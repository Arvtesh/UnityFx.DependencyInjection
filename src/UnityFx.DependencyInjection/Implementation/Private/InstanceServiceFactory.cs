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

		#endregion

		#region interface

		public InstanceServiceFactory(Type serviceType, object instance)
		{
			_serviceType = serviceType;
			_instance = instance;
		}

		#endregion

		#region IServiceFactory

		public Type ServiceType => _serviceType;

		public ServiceLifetime Lifetime => ServiceLifetime.Singleton;

		public object CreateInstance(IServiceProvider serviceProvider) => _instance;

		#endregion
	}
}
