// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;

namespace UnityFx.DependencyInjection
{
	/// <summary>
	/// A generic service factory.
	/// </summary>
	internal interface IServiceFactory
	{
		/// <summary>
		/// Gets the service type.
		/// </summary>
		/// <seealso cref="Lifetime"/>
		Type ServiceType { get; }

		/// <summary>
		/// Gets the service lifetime.
		/// </summary>
		/// <seealso cref="ServiceType"/>
		ServiceLifetime Lifetime { get; }

		/// <summary>
		/// Instantiates the service.
		/// </summary>
		object CreateInstance(IServiceProvider serviceProvider);
	}
}
