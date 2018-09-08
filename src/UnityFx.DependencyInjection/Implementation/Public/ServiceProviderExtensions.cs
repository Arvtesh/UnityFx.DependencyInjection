// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.ComponentModel;
using System.Reflection;
#if !NET35
using System.Runtime.ExceptionServices;
#endif

namespace UnityFx.DependencyInjection
{
	/// <summary>
	/// Extensions for <see cref="IServiceProvider"/>.
	/// </summary>
	[EditorBrowsable(EditorBrowsableState.Advanced)]
	public static class ServiceProviderExtensions
	{
		/// <summary>
		/// Returns an instance of a service for the type specified.
		/// </summary>
		/// <typeparam name="TService">Type of the requested service.</typeparam>
		/// <param name="serviceProvider">A service provider.</param>
		/// <returns>Returns service instance registered for the <typeparamref name="TService"/> type.</returns>
		public static TService GetService<TService>(this IServiceProvider serviceProvider)
		{
			return (TService)serviceProvider.GetService(typeof(TService));
		}

		/// <summary>
		/// Returns an instance of a service for the type specified.
		/// </summary>
		/// <param name="serviceProvider">A service provider.</param>
		/// <param name="serviceType">Type of the requested service.</param>
		/// <exception cref="ArgumentNullException">Thrown if <paramref name="serviceType"/> is <see langword="null"/>.</exception>
		/// <exception cref="InvalidOperationException">Thrown if service is not registered in the <paramref name="serviceProvider"/>.</exception>
		/// <returns>Returns service instance registered for the <paramref name="serviceType"/> type.</returns>
		public static object GetRequiredService(this IServiceProvider serviceProvider, Type serviceType)
		{
			return serviceProvider.GetService(serviceType) ?? throw new InvalidOperationException();
		}

		/// <summary>
		/// Returns an instance of a service for the type specified.
		/// </summary>
		/// <typeparam name="TService">Type of the requested service.</typeparam>
		/// <param name="serviceProvider">A service provider.</param>
		/// <exception cref="InvalidOperationException">Thrown if service is not registered in the <paramref name="serviceProvider"/>.</exception>
		/// <returns>Returns service instance registered for the <typeparamref name="TService"/> type.</returns>
		public static TService GetRequiredService<TService>(this IServiceProvider serviceProvider)
		{
			return (TService)(serviceProvider.GetService(typeof(TService)) ?? throw new InvalidOperationException());
		}

		/// <summary>
		/// Returns an instance of a service for the type specified.
		/// </summary>
		/// <param name="serviceProvider">A service provider.</param>
		/// <returns>Creates a new scope.</returns>
		public static IServiceScope CreateScope(this IServiceProvider serviceProvider)
		{
			return ((IServiceScopeFactory)serviceProvider.GetService(typeof(IServiceScopeFactory))).CreateScope();
		}

		/// <summary>
		/// Instantiate a type with constructor arguments provided directly and/or from an <see cref="IServiceProvider"/>.
		/// </summary>
		/// <param name="serviceProvider">The service provider used to resolve dependencies.</param>
		/// <param name="instanceType">The type to activate.</param>
		/// <param name="args">Constructor arguments not provided by the <paramref name="serviceProvider"/>.</param>
		/// <exception cref="ArgumentNullException">Thrown if <paramref name="instanceType"/> is <see langword="null"/>.</exception>
		/// <returns>An activated object of type <paramref name="instanceType"/>.</returns>
		public static object CreateInstance(this IServiceProvider serviceProvider, Type instanceType, params object[] args)
		{
			if (instanceType == null)
			{
				throw new ArgumentNullException(nameof(instanceType));
			}

			try
			{
				var constructors = instanceType.GetConstructors(BindingFlags.Instance | BindingFlags.Public);

				if (constructors.Length > 0)
				{
					// Select the first public non-static ctor with matching arguments.
					foreach (var ctor in constructors)
					{
						if (!ctor.IsStatic)
						{
							var argInfo = ctor.GetParameters();
							var argValues = new object[argInfo.Length];
							var matchCount = 0;

							for (var i = 0; i < argInfo.Length; ++i)
							{
								var argType = argInfo[i].ParameterType;

								for (var j = 0; j < args.Length; ++j)
								{
									if (argType.IsAssignableFrom(args[j].GetType()))
									{
										argValues[i] = args[j];
										++matchCount;
									}
								}
							}

							// If all arguments matched, use this constructor for activation.
							if (matchCount == argInfo.Length)
							{
								return ctor.Invoke(argValues);
							}
						}
					}

					throw new InvalidOperationException();
				}
				else
				{
					return Activator.CreateInstance(instanceType);
				}
			}
			catch (TargetInvocationException e)
			{
#if !NET35
				ExceptionDispatchInfo.Capture(e.InnerException).Throw();
#endif
				throw e.InnerException;
			}
		}

		/// <summary>
		/// Instantiate a type with constructor arguments provided directly and/or from an <see cref="IServiceProvider"/>.
		/// </summary>
		/// <typeparam name="T">The type to activate.</typeparam>
		/// <param name="serviceProvider">The service provider used to resolve dependencies.</param>
		/// <param name="args">Constructor arguments not provided by the <paramref name="serviceProvider"/>.</param>
		/// <returns>An activated object of type <typeparamref name="T"/>.</returns>
		public static T CreateInstance<T>(this IServiceProvider serviceProvider, params object[] args)
		{
			return (T)CreateInstance(serviceProvider, typeof(T), args);
		}
	}
}
