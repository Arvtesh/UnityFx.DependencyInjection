// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Collections;
#if !NET35
using System.Collections.Concurrent;
#endif
using System.Collections.Generic;
using System.Diagnostics;

namespace UnityFx.DependencyInjection
{
	internal class ServiceScope : IServiceScope, IServiceScopeFactory, IServiceProvider
	{
		#region data

		private readonly ServiceProvider _serviceProvider;
#if NET35
		private readonly Dictionary<Type, object> _resolvedServices = new Dictionary<Type, object>();
#else
		private readonly ConcurrentDictionary<Type, object> _resolvedServices = new ConcurrentDictionary<Type, object>();
#endif
		private bool _disposed;

		#endregion

		#region interface

		internal ServiceScope(ServiceProvider serviceProvider)
		{
			_serviceProvider = serviceProvider;
		}

		internal ServiceScope(ServiceProvider serviceProvider, IEnumerable<KeyValuePair<Type, object>> resolvedServices)
		{
			_serviceProvider = serviceProvider;

			foreach (var kvp in resolvedServices)
			{
#if NET35
				_resolvedServices.Add(kvp.Key, kvp.Value);
#else
				_resolvedServices.TryAdd(kvp.Key, kvp.Value);
#endif
			}
		}

		internal void AddResolvedService(Type serviceType, object serviceInstance)
		{
			Debug.Assert(!_disposed);

#if NET35
			lock (_resolvedServices)
			{
				_resolvedServices.Add(serviceType, serviceInstance);
			}
#else
			_resolvedServices.TryAdd(serviceType, serviceInstance);
#endif
		}

		internal bool TryGetResolvedService(Type serviceType, out object serviceInstance)
		{
			Debug.Assert(!_disposed);

#if NET35
			lock (_resolvedServices)
			{
				return _resolvedServices.TryGetValue(serviceType, out serviceInstance);
			}
#else
			return _resolvedServices.TryGetValue(serviceType, out serviceInstance);
#endif
		}

		#endregion

		#region IServiceScope

		public IServiceProvider ServiceProvider => this;

		#endregion

		#region IServiceScopeFactory

		public IServiceScope CreateScope()
		{
			if (_disposed)
			{
				throw new ObjectDisposedException(GetType().Name);
			}

			return new ServiceScope(_serviceProvider);
		}

		#endregion

		#region IServiceProvider

		public object GetService(Type serviceType)
		{
			return _serviceProvider.GetService(serviceType, this);
		}

		#endregion

		#region IDisposable

		public void Dispose()
		{
			if (!_disposed)
			{
				_disposed = true;

				foreach (var service in _resolvedServices.Values)
				{
					if (service is IDisposable d)
					{
						d.Dispose();
					}
				}

				_resolvedServices.Clear();
			}
		}

		#endregion
	}
}
