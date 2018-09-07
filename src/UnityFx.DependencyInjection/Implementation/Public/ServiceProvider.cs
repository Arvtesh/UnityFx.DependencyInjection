// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;

namespace UnityFx.DependencyInjection
{
	/// <summary>
	/// Default implementatino for <see cref="IServiceProvider"/>.
	/// </summary>
	public sealed class ServiceProvider : IServiceProvider, IDisposable
	{
		#region data

		private readonly ServiceScope _rootScope;
		private readonly Dictionary<Type, IServiceFactory> _services = new Dictionary<Type, IServiceFactory>();
		private bool _disposed;

		#endregion

		#region interface

		internal ServiceProvider(IEnumerable<ServiceDescriptor> serviceDescriptors, bool validate)
		{
			_rootScope = new ServiceScope(this);
			InitServices(serviceDescriptors, validate);
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
						return item.CreateInstance(scope);

					default:
						throw new NotSupportedException(item.Lifetime.ToString());
				}
			}
			else if (serviceType == typeof(IServiceProvider) || serviceType == typeof(IServiceScope) || serviceType == typeof(IServiceScopeFactory))
			{
				return scope;
			}

			return null;
		}

		#endregion

		#region IServiceProvider

		/// <inheritdoc/>
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

		#region IDisposable

		/// <inheritdoc/>
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

		private void InitServices(IEnumerable<ServiceDescriptor> serviceDescriptors, bool validate)
		{
			var knownTypes = new Dictionary<Type, ServiceDescriptor>();
			var constructorTypes = new Dictionary<Type, ConstructorInfo>();

			foreach (var descriptor in serviceDescriptors)
			{
				knownTypes.Add(descriptor.ServiceType, descriptor);
			}

			foreach (var descriptor in serviceDescriptors)
			{
				var serviceType = descriptor.ServiceType;

				// Make sure required services are not registered.
				if (serviceType == typeof(IServiceProvider) ||
					serviceType == typeof(IServiceScope) ||
					serviceType == typeof(IServiceScopeFactory))
				{
					throw new InvalidOperationException();
				}

				if (descriptor.ImplementationInstance != null)
				{
					_services.Add(serviceType, new InstanceServiceFactory(serviceType, descriptor.ImplementationInstance));
				}
				else if (descriptor.ImplementationType != null)
				{
					var ctor = GetPreferredCtor(descriptor.ImplementationType, knownTypes);

					if (ctor != null)
					{
						constructorTypes.Add(serviceType, ctor);
					}

					_services.Add(serviceType, new ConstructorServiceFactory(serviceType, ctor, descriptor.Lifetime));
				}
				else if (descriptor.ImplementationFactory != null)
				{
					_services.Add(serviceType, new DelegateServiceFactory(serviceType, descriptor.ImplementationFactory, descriptor.Lifetime));
				}
				else
				{
					throw new InvalidOperationException();
				}
			}

			if (validate)
			{
				ValidateConstructors(knownTypes, constructorTypes);
			}
		}

		private void ValidateConstructors(Dictionary<Type, ServiceDescriptor> knownTypes, Dictionary<Type, ConstructorInfo> constructorTypes)
		{
			var callStack = new HashSet<Type>();

			foreach (var kvp in constructorTypes)
			{
				callStack.Clear();

				// Make sure there are no dependency loops, i.e. no situations like A depends on B, B depends on A.
				ValidateCtorLoops(kvp.Key, kvp.Value, constructorTypes, callStack);

				// Make sure singleton constructor do not depend on scoped services.
				if (knownTypes[kvp.Key].Lifetime == ServiceLifetime.Singleton)
				{
					ValidateSingletonArguments(kvp.Value, knownTypes);
				}
			}
		}

		private void ValidateCtorLoops(Type serviceType, ConstructorInfo ctor, Dictionary<Type, ConstructorInfo> ctorTypes, HashSet<Type> callStack)
		{
			callStack.Add(serviceType);

			foreach (var arg in ctor.GetParameters())
			{
				var argType = arg.ParameterType;

				if (ctorTypes.TryGetValue(argType, out var argCtor))
				{
					if (callStack.Contains(argType))
					{
						throw new InvalidOperationException();
					}

					ValidateCtorLoops(argType, argCtor, ctorTypes, callStack);
				}
			}
		}

		private void ValidateSingletonArguments(ConstructorInfo ctor, Dictionary<Type, ServiceDescriptor> knownTypes)
		{
			foreach (var args in ctor.GetParameters())
			{
				var descriptor = knownTypes[args.ParameterType];

				if (descriptor.Lifetime == ServiceLifetime.Scoped)
				{
					throw new InvalidOperationException();
				}
			}
		}

		private ConstructorInfo GetPreferredCtor(Type serviceType, Dictionary<Type, ServiceDescriptor> knownTypes)
		{
			var constructors = serviceType.GetConstructors(BindingFlags.Instance | BindingFlags.Public);

			// Select the first public non-static ctor having all arguments registered.
			foreach (var ctor in constructors)
			{
				if (!ctor.IsStatic)
				{
					var argumentsValidated = true;

					foreach (var arg in ctor.GetParameters())
					{
						// Make sure the argument type is registered in _services (so we can create it).
						if (!knownTypes.ContainsKey(arg.ParameterType))
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
			}

			throw new InvalidOperationException();
		}

		private object GetSingletonService(IServiceFactory serviceFactory, ServiceScope scope)
		{
			Debug.Assert(serviceFactory != null);

			// NOTE: Singleton is always created in the root scope.
			if (_rootScope.TryGetResolvedService(serviceFactory.ServiceType, out var instance))
			{
				return instance;
			}
			else
			{
				var service = serviceFactory.CreateInstance(scope);
				_rootScope.AddResolvedService(serviceFactory.ServiceType, service);
				return service;
			}
		}

		private object GetScopedService(IServiceFactory serviceFactory, ServiceScope scope)
		{
			Debug.Assert(serviceFactory != null);

			// NOTE: Scoped services are created in the caller scope.
			if (scope.TryGetResolvedService(serviceFactory.ServiceType, out var instance))
			{
				return instance;
			}
			else
			{
				var service = serviceFactory.CreateInstance(scope);
				scope.AddResolvedService(serviceFactory.ServiceType, service);
				return service;
			}
		}

		#endregion
	}
}
