// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;

namespace UnityFx.DependencyInjection
{
	internal class ServiceScope : IServiceScope, IServiceProvider
	{
		#region data

		private readonly ServiceProvider _serviceProvider;
		private readonly Dictionary<Type, object> _resolvedServices = new Dictionary<Type, object>();
		private bool _disposed;

		#endregion

		#region interface

		internal ServiceScope(ServiceProvider serviceProvider)
		{
			_serviceProvider = serviceProvider;
		}

		internal void AddResolvedService(Type serviceType, object serviceInstance)
		{
			_resolvedServices.Add(serviceType, serviceInstance);
		}

		internal bool TryGetResolvedService(Type serviceType, out object serviceInstance)
		{
			return _resolvedServices.TryGetValue(serviceType, out serviceInstance);
		}

		#endregion

		#region IServiceScope

		public IServiceProvider ServiceProvider => this;

		#endregion

		#region IServiceProvider

		public object GetService(Type serviceType)
		{
			if (serviceType == null)
			{
				throw new ArgumentNullException(nameof(serviceType));
			}

			if (_disposed)
			{
				throw new ObjectDisposedException(GetType().Name);
			}

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
