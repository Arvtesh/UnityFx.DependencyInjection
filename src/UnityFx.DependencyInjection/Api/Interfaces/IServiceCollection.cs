// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Collections.Generic;

namespace UnityFx.DependencyInjection
{
	/// <summary>
	/// Specifies the contract for a collection of service descriptors.
	/// </summary>
	/// <seealso cref="ServiceDescriptor"/>
	/// <seealso cref="IServiceProvider"/>
	public interface IServiceCollection : ICollection<ServiceDescriptor>
	{
		/// <summary>
		/// Determines whether the collection contains a service having specific type.
		/// </summary>
		/// <param name="serviceType">The service type to check for.</param>
		/// <returns>Returns <see langword="true"/> if the collection contains service of type <paramref name="serviceType"/>; <see langword="false"/> otherwise.</returns>
		bool Contains(Type serviceType);

		/// <summary>
		/// Removes the first service of type <paramref name="serviceType"/> from the collection.
		/// </summary>
		/// <param name="serviceType">The service type to remove.</param>
		bool Remove(Type serviceType);
	}
}
