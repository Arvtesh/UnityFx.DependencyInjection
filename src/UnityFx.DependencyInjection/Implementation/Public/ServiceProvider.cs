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
	/// Default implementation of <see cref="IServiceProvider"/>.
	/// </summary>
	/// <seealso cref="ServiceCollection"/>
	/// <seealso cref="ServiceDescriptor"/>
	public class ServiceProvider : IServiceCollection, IServiceProvider, IDisposable
	{
		#region data

		private struct ServiceData
		{
			public ServiceDescriptor Descriptor;
			public object Instance;

			public ServiceData(ServiceDescriptor descriptor)
			{
				Descriptor = descriptor;
				Instance = descriptor.ImplementationInstance;
			}
		}

		private Dictionary<Type, ServiceData> _services = new Dictionary<Type, ServiceData>();
		private bool _disposed;

		#endregion

		#region interface

		/// <summary>
		/// Initializes a new instance of the <see cref="ServiceProvider"/> class.
		/// </summary>
		public ServiceProvider()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ServiceProvider"/> class.
		/// </summary>
		/// <param name="serviceDescriptors">A collection of service descriptors.</param>
		public ServiceProvider(IEnumerable<ServiceDescriptor> serviceDescriptors)
		{
			foreach (var serviceDescriptor in serviceDescriptors)
			{
				Add(serviceDescriptor);
			}
		}

		/// <summary>
		/// Creates an instance of the specified <paramref name="type"/>.
		/// </summary>
		/// <param name="type">Type of the object to create. The type is not expected to be registered in the service provider.</param>
		/// <returns>An instance of the <paramref name="type"/> created.</returns>
		public object CreateInstance(Type type)
		{
			if (type == null)
			{
				throw new ArgumentNullException(nameof(type));
			}

			if (_disposed)
			{
				throw new ObjectDisposedException(GetType().Name);
			}

			return CreateInstance(type, null);
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

			return GetService(serviceType, null);
		}

		#endregion

		#region ICollection

		/// <inheritdoc/>
		public int Count => _services.Count;

		/// <inheritdoc/>
		public bool IsReadOnly => false;

		/// <inheritdoc/>
		public void Add(ServiceDescriptor item)
		{
			if (item == null)
			{
				throw new ArgumentNullException(nameof(item));
			}

			if (_services.ContainsKey(item.ServiceType))
			{
				_services[item.ServiceType] = new ServiceData(item);
			}
			else
			{
				_services.Add(item.ServiceType, new ServiceData(item));
			}
		}

		/// <inheritdoc/>
		public bool Remove(ServiceDescriptor item)
		{
			if (item != null)
			{
				return _services.Remove(item.ServiceType);
			}

			return false;
		}

		/// <inheritdoc/>
		public bool Contains(ServiceDescriptor item)
		{
			if (item != null)
			{
				return _services.ContainsKey(item.ServiceType);
			}

			return false;
		}

		/// <inheritdoc/>
		public void CopyTo(ServiceDescriptor[] array, int arrayIndex)
		{
			if (array == null)
			{
				throw new ArgumentNullException(nameof(array));
			}

			var index = arrayIndex;

			foreach (var item in _services.Values)
			{
				array[arrayIndex++] = item.Descriptor;
			}
		}

		/// <inheritdoc/>
		public void Clear()
		{
			_services.Clear();
		}

		#endregion

		#region IEnumerable

		/// <inheritdoc/>
		public IEnumerator<ServiceDescriptor> GetEnumerator()
		{
			foreach (var serviceData in _services.Values)
			{
				yield return serviceData.Descriptor;
			}
		}

		/// <inheritdoc/>
		IEnumerator IEnumerable.GetEnumerator()
		{
			return (this as IEnumerable<ServiceDescriptor>).GetEnumerator();
		}

		#endregion

		#region IDisposable

		/// <inheritdoc/>
		public void Dispose()
		{
			if (!_disposed)
			{
				_disposed = true;

				foreach (var item in _services.Values)
				{
					if (item.Instance is IDisposable service)
					{
						service.Dispose();
					}
				}

				_services.Clear();
			}

			GC.SuppressFinalize(this);
		}

		#endregion

		#region implementation

		private object CreateInstance(Type serviceType, ICollection<Type> callerTypes)
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
						var args = GetArguments(ctor, callerTypes);
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

		private object[] GetArguments(MethodBase method, ICollection<Type> callerTypes)
		{
			Debug.Assert(method != null);

			var parameters = method.GetParameters();
			var args = new object[parameters.Length];

			for (var i = 0; i < args.Length; ++i)
			{
				args[i] = GetService(parameters[i].ParameterType, callerTypes);
			}

			return args;
		}

		private object GetService(Type serviceType, ICollection<Type> callerTypes)
		{
			Debug.Assert(serviceType != null);

			if (serviceType == typeof(IServiceProvider))
			{
				return this;
			}
			else if (_services.TryGetValue(serviceType, out var item))
			{
				var descriptor = item.Descriptor;

				switch (descriptor.Lifetime)
				{
					case ServiceLifetime.Singleton:
						return GetSingletonService(descriptor, item.Instance, callerTypes);

					case ServiceLifetime.Scoped:
						return GetScopedService(descriptor, item.Instance, callerTypes);

					case ServiceLifetime.Transient:
						return GetTransientService(descriptor, callerTypes);

					default:
						throw new NotSupportedException(descriptor.Lifetime.ToString());
				}
			}

			throw new ServiceNotFoundException(serviceType);
		}

		private object GetSingletonService(ServiceDescriptor serviceDescriptor, object instance, ICollection<Type> callerTypes)
		{
			Debug.Assert(serviceDescriptor != null);

			if (instance != null)
			{
				return instance;
			}
			else if (serviceDescriptor.ImplementationType != null)
			{
				var service = CreateInstance(serviceDescriptor.ImplementationType, callerTypes);
				_services[serviceDescriptor.ServiceType] = new ServiceData() { Descriptor = serviceDescriptor, Instance = service };
				return service;
			}
			else if (serviceDescriptor.ImplementationFactory != null)
			{
				var service = serviceDescriptor.ImplementationFactory(this);
				_services[serviceDescriptor.ServiceType] = new ServiceData() { Descriptor = serviceDescriptor, Instance = service };
				return service;
			}

			// Should not get here.
			Debug.Fail("Invalid service descriptor.");
			throw new InvalidOperationException();
		}

		private object GetScopedService(ServiceDescriptor serviceDescriptor, object instance, ICollection<Type> callerTypes)
		{
			Debug.Assert(serviceDescriptor != null);

			throw new NotSupportedException();
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
