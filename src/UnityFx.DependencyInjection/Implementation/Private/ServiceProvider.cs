// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;

namespace UnityFx.DependencyInjection
{
	internal class ServiceProvider : IServiceProvider
	{
		#region data

		private readonly ServiceScope _scope;
		private readonly Dictionary<Type, ServiceDescriptor> _services = new Dictionary<Type, ServiceDescriptor>();

		#endregion

		#region interface

		/// <summary>
		/// Initializes a new instance of the <see cref="ServiceProvider"/> class.
		/// </summary>
		public ServiceProvider(ServiceScope scope)
		{
			_scope = scope;
		}

		internal object GetService(Type serviceType, ServiceScope sourceScope, ICollection<Type> callerTypes)
		{
			Debug.Assert(serviceType != null);
			Debug.Assert(sourceScope != null);

			if (serviceType == typeof(IServiceProvider))
			{
				return this;
			}
			else if (serviceType == typeof(IServiceScope) || serviceType == typeof(IServiceScopeFactory))
			{
				return _scope;
			}
			else if (_services.TryGetValue(serviceType, out var item))
			{
				switch (item.Lifetime)
				{
					case ServiceLifetime.Singleton:
						return GetScopedService(item, _scope, callerTypes);

					case ServiceLifetime.Scoped:
						return GetScopedService(item, sourceScope, callerTypes);

					case ServiceLifetime.Transient:
						return GetTransientService(item, callerTypes);

					default:
						throw new NotSupportedException(item.Lifetime.ToString());
				}
			}

			return _scope.GetService(serviceType);
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

			return GetService(serviceType, _scope, null);
		}

		#endregion

		#region implementation

		private object CreateInstance(Type serviceType, ServiceScope scope, ICollection<Type> callerTypes)
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
						var args = GetArguments(ctor, scope, callerTypes);
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

		private object[] GetArguments(MethodBase method, ServiceScope sourceScope, ICollection<Type> callerTypes)
		{
			Debug.Assert(method != null);

			var parameters = method.GetParameters();
			var args = new object[parameters.Length];

			for (var i = 0; i < args.Length; ++i)
			{
				args[i] = GetService(parameters[i].ParameterType, sourceScope, callerTypes);
			}

			return args;
		}

		private object GetScopedService(ServiceDescriptor serviceDescriptor, ServiceScope scope, ICollection<Type> callerTypes)
		{
			Debug.Assert(serviceDescriptor != null);

			if (_scope.TryGetServiceInstance(serviceDescriptor.ServiceType, out var instance))
			{
				return instance;
			}
			else if (serviceDescriptor.ImplementationType != null)
			{
				var service = CreateInstance(serviceDescriptor.ImplementationType, scope, callerTypes);
				scope.AddServiceInstance(serviceDescriptor.ServiceType, service);
				return service;
			}
			else if (serviceDescriptor.ImplementationFactory != null)
			{
				var service = serviceDescriptor.ImplementationFactory(this);
				scope.AddServiceInstance(serviceDescriptor.ServiceType, service);
				return service;
			}

			// Should not get here.
			Debug.Fail("Invalid service descriptor.");
			throw new InvalidOperationException();
		}

		private object GetTransientService(ServiceDescriptor serviceDescriptor, ICollection<Type> callerTypes)
		{
			Debug.Assert(serviceDescriptor != null);

			if (serviceDescriptor.ImplementationType != null)
			{
				return CreateInstance(serviceDescriptor.ImplementationType, callerTypes);
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
