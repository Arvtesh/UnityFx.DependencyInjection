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
		#region interface

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

		#endregion

		#region implementation
		#endregion
	}
}
