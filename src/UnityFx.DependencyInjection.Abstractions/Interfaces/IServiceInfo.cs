// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;

namespace UnityFx.DependencyInjection
{
	/// <summary>
	/// A generic service information.
	/// </summary>
	public interface IServiceInfo
	{
		/// <summary>
		/// Gets the service type.
		/// </summary>
		/// <seealso cref="Lifetime"/>
		/// <seealso cref="Options"/>
		Type ServiceType { get; }

		/// <summary>
		/// Gets the service lifetime.
		/// </summary>
		/// <seealso cref="Options"/>
		/// <seealso cref="ServiceType"/>
		ServiceLifetime Lifetime { get; }

		/// <summary>
		/// Gets the service options.
		/// </summary>
		/// <seealso cref="Lifetime"/>
		/// <seealso cref="ServiceType"/>
		ServiceOptions Options { get; }
	}
}
