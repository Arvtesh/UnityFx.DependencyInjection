// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;

namespace UnityFx.DependencyInjection
{
	internal class ServiceProvider : IServiceProvider, IServiceScope, IServiceScopeFactory
	{
		#region data

		private readonly ServiceProvider _parent;
		private readonly Dictionary<Type, ServiceDescriptor> _services = new Dictionary<Type, ServiceDescriptor>();
		private readonly Dictionary<Type, object> _serviceInstances = new Dictionary<Type, object>();
		private bool _disposed;

		#endregion

		#region interface

		internal ServiceProvider(ServiceProvider parent)
		{
			_parent = parent;
		}

		internal object GetService(Type serviceType, ServiceProvider callerScope, ICollection<Type> callerTypes)
		{
			Debug.Assert(serviceType != null);
			Debug.Assert(callerScope != null);

			if (serviceType == typeof(IServiceProvider) || serviceType == typeof(IServiceScope) || serviceType == typeof(IServiceScopeFactory))
			{
				return this;
			}
			else if (_services.TryGetValue(serviceType, out var item))
			{
				switch (item.Lifetime)
				{
					case ServiceLifetime.Singleton:
						return GetSingletonService(item, callerScope, callerTypes);

					case ServiceLifetime.Scoped:
						return GetScopedService(item, callerScope, callerTypes);

					case ServiceLifetime.Transient:
						return GetTransientService(item, callerScope, callerTypes);

					default:
						throw new NotSupportedException(item.Lifetime.ToString());
				}
			}

			return _parent.GetService(serviceType, callerScope, callerTypes);
		}

		#endregion

		#region IServiceProvider

		public object GetService(Type serviceType)
		{
			if (serviceType == null)
			{
				throw new ArgumentNullException(nameof(serviceType));
			}

			return GetService(serviceType, this, null);
		}

		#endregion

		#region IServiceScopeFactory

		public IServiceScope CreateScope()
		{
			if (_disposed)
			{
				throw new ObjectDisposedException(GetType().Name);
			}

			return new ServiceProvider(this);
		}

		#endregion

		#region IServiceScope

		IServiceProvider IServiceScope.ServiceProvider => this;

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
				_services.Clear();
			}
		}

		#endregion

		#region implementation

		private object CreateInstance(Type serviceType, ServiceProvider callerScope, ICollection<Type> callerTypes)
		{
			Debug.Assert(serviceType != null);

			try
			{
				var constructors = serviceType.GetConstructors(BindingFlags.Instance | BindingFlags.Public);

				if (constructors.Length > 0)
				{
					// Add the service type to a set of caller types. We have to maintain it to avoid loops like
					// A requires B, B requires A.
					if (callerTypes == null)
					{
						callerTypes = new HashSet<Type>() { serviceType };
					}
					else
					{
						callerTypes.Add(serviceType);
					}

					var ctor = GetConstructor(constructors, callerTypes);

					if (ctor != null)
					{
						var args = GetArguments(ctor, callerScope, callerTypes);
						return ctor.Invoke(args);
					}
					else
					{
						throw new ServiceConstructorResolutionException(serviceType);
					}
				}
				else
				{
					return Activator.CreateInstance(serviceType);
				}
			}
			catch (TargetInvocationException e)
			{
				throw e.InnerException;
			}
		}

		private ConstructorInfo GetConstructor(ConstructorInfo[] ctors, ICollection<Type> callerTypes)
		{
			Debug.Assert(ctors != null);

			// Select the first ctor having all arguments registered.
			foreach (var ctor in ctors)
			{
				var argumentsValidated = true;

				foreach (var arg in ctor.GetParameters())
				{
					// Make sure the argument type is registered in _services (so we can create it) and no construction loops are detected.
					if (!_services.ContainsKey(arg.ParameterType) || (callerTypes != null && callerTypes.Contains(arg.ParameterType)))
					{
						argumentsValidated = false;
						break;
					}
				}

				if (argumentsValidated)
				{
					return ctor;
				}
			}

			return null;
		}

		private object[] GetArguments(MethodBase method, ServiceProvider callerScope, ICollection<Type> callerTypes)
		{
			Debug.Assert(method != null);

			var parameters = method.GetParameters();
			var args = new object[parameters.Length];

			for (var i = 0; i < args.Length; ++i)
			{
				args[i] = GetService(parameters[i].ParameterType, callerScope, callerTypes);
			}

			return args;
		}

		private object GetSingletonService(ServiceDescriptor serviceDescriptor, ServiceProvider callerScope, ICollection<Type> callerTypes)
		{
			Debug.Assert(serviceDescriptor != null);

			// NOTE: Singleton is always created in the scope it was registered.
			if (_serviceInstances.TryGetValue(serviceDescriptor.ServiceType, out var instance))
			{
				return instance;
			}
			else if (serviceDescriptor.ImplementationType != null)
			{
				var service = CreateInstance(serviceDescriptor.ImplementationType, callerScope, callerTypes);
				_serviceInstances.Add(serviceDescriptor.ServiceType, service);
				return service;
			}
			else if (serviceDescriptor.ImplementationFactory != null)
			{
				var service = serviceDescriptor.ImplementationFactory(this);
				_serviceInstances.Add(serviceDescriptor.ServiceType, service);
				return service;
			}

			// Should not get here.
			Debug.Fail("Invalid service descriptor.");
			throw new InvalidOperationException();
		}

		private object GetScopedService(ServiceDescriptor serviceDescriptor, ServiceProvider callerScope, ICollection<Type> callerTypes)
		{
			Debug.Assert(serviceDescriptor != null);

			// NOTE: Scoped services are created in the caller scope.
			if (_serviceInstances.TryGetValue(serviceDescriptor.ServiceType, out var instance))
			{
				return instance;
			}
			else if (serviceDescriptor.ImplementationType != null)
			{
				var service = CreateInstance(serviceDescriptor.ImplementationType, callerScope, callerTypes);
				callerScope._serviceInstances.Add(serviceDescriptor.ServiceType, service);
				return service;
			}
			else if (serviceDescriptor.ImplementationFactory != null)
			{
				var service = serviceDescriptor.ImplementationFactory(this);
				callerScope._serviceInstances.Add(serviceDescriptor.ServiceType, service);
				return service;
			}

			// Should not get here.
			Debug.Fail("Invalid service descriptor.");
			throw new InvalidOperationException();
		}

		private object GetTransientService(ServiceDescriptor serviceDescriptor, ServiceProvider callerScope, ICollection<Type> callerTypes)
		{
			Debug.Assert(serviceDescriptor != null);

			if (serviceDescriptor.ImplementationType != null)
			{
				return CreateInstance(serviceDescriptor.ImplementationType, callerScope, callerTypes);
			}
			else if (serviceDescriptor.ImplementationFactory != null)
			{
				return serviceDescriptor.ImplementationFactory(this);
			}

			// Should not get here.
			Debug.Fail("Invalid service descriptor.");
			throw new InvalidOperationException();
		}

		#endregion
	}
}
