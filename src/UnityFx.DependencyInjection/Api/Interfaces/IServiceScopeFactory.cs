// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Collections.Generic;

namespace UnityFx.DependencyInjection
{
	/// <summary>
	/// A factory for creating instances of <see cref="IServiceScope"/>, which is used to create services within a scope.
	/// </summary>
	/// <seealso cref="IServiceScope"/>
	/// <seealso cref="IServiceProvider"/>
	public interface IServiceScopeFactory
	{
		/// <summary>
		/// Creates an <see cref="IServiceScope"/> which contains an <see cref="IServiceProvider"/> used to resolve dependencies from a newly created scope.
		/// </summary>
		IServiceScope CreateScope();
	}
}
