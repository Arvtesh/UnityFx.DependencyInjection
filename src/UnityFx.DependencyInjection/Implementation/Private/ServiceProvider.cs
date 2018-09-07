// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;

namespace UnityFx.DependencyInjection
{
	internal class ServiceProvider : IServiceProvider, IServiceScopeFactory, IDisposable
	{
		#region data

		private readonly ServiceScope _rootScope;
		private readonly Dictionary<Type, ServiceDescriptor> _services = new Dictionary<Type, ServiceDescriptor>();
		private bool _disposed;

		#endregion

		#region interface

		internal ServiceProvider(IEnumerable<ServiceDescriptor> serviceDescriptors)
		{
			_rootScope = new ServiceScope(this);

			foreach (var descriptor in serviceDescriptors)
			{
				// TODO: validate service descriptors
				_services.Add(descriptor.ServiceType, descriptor);
			}
		}

		internal object GetService(Type serviceType, ServiceScope scope)
		{
			Debug.Assert(serviceType != null);
			Debug.Assert(scope != null);

			if (_services.TryGetValue(serviceType, out var item))
			{
				switch (item.Lifetime)
				{
					case ServiceLifetime.Singleton:
						return GetSingletonService(item, scope);

					case ServiceLifetime.Scoped:
						return GetScopedService(item, scope);

					case ServiceLifetime.Transient:
						return GetTransientService(item, scope);

					default:
						throw new NotSupportedException(item.Lifetime.ToString());
				}
			}
			else if (serviceType == typeof(IServiceProvider) || serviceType == typeof(IServiceScope) || serviceType == typeof(IServiceScopeFactory))
			{
				return this;
			}

			return null;
		}

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

			return GetService(serviceType, _rootScope);
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

		#region IDisposable

		public void Dispose()
		{
			if (!_disposed)
			{
				_disposed = true;
				_services.Clear();
				_rootScope.Dispose();
			}
		}

		#endregion

		#region implementation

		private object CreateInstance(Type serviceType, ServiceScope scope)
		{
			Debug.Assert(serviceType != null);

			try
			{
				var constructors = serviceType.GetConstructors(BindingFlags.Instance | BindingFlags.Public);

				if (constructors.Length > 0)
				{
					////var ctor = GetConstructor(constructors, callerTypes);

					////if (ctor != null)
					////{
					////	var parameters = ctor.GetParameters();
					////	var args = new object[parameters.Length];

					////	for (var i = 0; i < args.Length; ++i)
					////	{
					////		args[i] = GetService(parameters[i].ParameterType, scope);
					////	}

					////	return ctor.Invoke(args);
					////}
					////else
					////{
					////	throw new ServiceConstructorResolutionException(serviceType);
					////}

					throw new NotImplementedException();
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

		private object[] GetArguments(MethodBase method, ServiceScope scope)
		{
			Debug.Assert(method != null);

			var parameters = method.GetParameters();
			var args = new object[parameters.Length];

			for (var i = 0; i < args.Length; ++i)
			{
				args[i] = GetService(parameters[i].ParameterType, scope);
			}

			return args;
		}

		private object GetSingletonService(ServiceDescriptor serviceDescriptor, ServiceScope scope)
		{
			Debug.Assert(serviceDescriptor != null);

			// NOTE: Singleton is always created in the root scope.
			if (_rootScope.TryGetResolvedService(serviceDescriptor.ServiceType, out var instance))
			{
				return instance;
			}
			else if (serviceDescriptor.ImplementationType != null)
			{
				var service = CreateInstance(serviceDescriptor.ImplementationType, scope);
				_rootScope.AddResolvedService(serviceDescriptor.ServiceType, service);
				return service;
			}
			else if (serviceDescriptor.ImplementationFactory != null)
			{
				var service = serviceDescriptor.ImplementationFactory(this);
				_rootScope.AddResolvedService(serviceDescriptor.ServiceType, service);
				return service;
			}

			// Should not get here.
			Debug.Fail("Invalid service descriptor.");
			throw new InvalidOperationException();
		}

		private object GetScopedService(ServiceDescriptor serviceDescriptor, ServiceScope scope)
		{
			Debug.Assert(serviceDescriptor != null);

			// NOTE: Scoped services are created in the caller scope.
			if (_rootScope.TryGetResolvedService(serviceDescriptor.ServiceType, out var instance))
			{
				return instance;
			}
			else if (serviceDescriptor.ImplementationType != null)
			{
				var service = CreateInstance(serviceDescriptor.ImplementationType, scope);
				scope.AddResolvedService(serviceDescriptor.ServiceType, service);
				return service;
			}
			else if (serviceDescriptor.ImplementationFactory != null)
			{
				var service = serviceDescriptor.ImplementationFactory(this);
				scope.AddResolvedService(serviceDescriptor.ServiceType, service);
				return service;
			}

			// Should not get here.
			Debug.Fail("Invalid service descriptor.");
			throw new InvalidOperationException();
		}

		private object GetTransientService(ServiceDescriptor serviceDescriptor, ServiceScope scope)
		{
			Debug.Assert(serviceDescriptor != null);

			if (serviceDescriptor.ImplementationType != null)
			{
				return CreateInstance(serviceDescriptor.ImplementationType, scope);
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
