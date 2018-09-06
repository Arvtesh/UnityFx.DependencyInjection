// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;

namespace UnityFx.DependencyInjection
{
	internal class ServiceScope : IServiceScope, IServiceScopeFactory
	{
		#region data

		private readonly ServiceScope _parentScope;
		private readonly ServiceProvider _serviceProvider;
		private readonly Dictionary<Type, object> _serviceInstances = new Dictionary<Type, object>();
		private bool _disposed;

		#endregion

		#region interface

		internal ServiceScope(ServiceScope parentScope)
		{
			_parentScope = parentScope;
			_serviceProvider = new ServiceProvider(this);
		}

		internal object GetService(Type serviceType)
		{
			return _parentScope?.ServiceProvider.GetService(serviceType);
		}

		internal void AddServiceInstance(Type serviceType, object instance)
		{
			_serviceInstances.Add(serviceType, instance);
		}

		internal bool TryGetServiceInstance(Type serviceType, out object instance)
		{
			return _serviceInstances.TryGetValue(serviceType, out instance);
		}

		#endregion

		#region IServiceScopeFactory

		public IServiceScope CreateScope()
		{
			if (_disposed)
			{
				throw new ObjectDisposedException(GetType().Name);
			}

			return new ServiceScope(this);
		}

		#endregion

		#region IServiceScope

		public IServiceProvider ServiceProvider => _serviceProvider;

		#endregion

		#region IDisposable

		public void Dispose()
		{
			if (!_disposed)
			{
				_disposed = true;

				foreach (var service in _serviceInstances.Values)
				{
					if (service is IDisposable d)
					{
						d.Dispose();
					}
				}

				_serviceInstances.Clear();
			}
		}

		#endregion
	}
}
