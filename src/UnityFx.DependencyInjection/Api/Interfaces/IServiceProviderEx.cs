// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;

namespace UnityFx.DependencyInjection
{
	/// <summary>
	/// Defines optional helpers tools for <see cref="IServiceProvider"/>.
	/// </summary>
	/// <remarks>
	/// A specific <see cref="IServiceProvider"/> implementation might decide to implement this interface.
	/// </remarks>
	/// <seealso cref="IServiceProvider"/>
	public interface IServiceProviderEx : IServiceProvider
	{
		/// <summary>
		/// Creates an instance of the specified <paramref name="type"/>. The <paramref name="type"/> is not expected to be registered in the service provider.
		/// </summary>
		/// <param name="type">Type of the object to create. The type is not expected to be registered in the service provider.</param>
		/// <returns>An instance of the <paramref name="type"/> created.</returns>
		object CreateInstance(Type type);
	}
}
