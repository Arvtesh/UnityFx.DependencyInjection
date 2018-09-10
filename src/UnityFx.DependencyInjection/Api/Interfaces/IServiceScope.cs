// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;

namespace UnityFx.DependencyInjection
{
	/// <summary>
	/// A generic service scope.
	/// </summary>
	/// <remarks>
	/// The <see cref="IDisposable.Dispose"/> method ends the scope lifetime. Once it is called,
	/// any scoped services that have been resolved from <see cref="IServiceProvider"/> will be
	/// disposed.
	/// </remarks>
	/// <seealso cref="IServiceProvider"/>
	public interface IServiceScope : IDisposable
	{
		/// <summary>
		/// Gets <see cref="IServiceProvider"/> used to resolve dependencies from the scope.
		/// </summary>
		IServiceProvider ServiceProvider { get; }
	}
}
