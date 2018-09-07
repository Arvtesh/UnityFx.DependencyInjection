// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;

namespace UnityFx.DependencyInjection
{
	/// <summary>
	/// Implementation of a default <see cref="IServiceScopeFactory"/>.
	/// </summary>
	public class DefaultServiceScopeFactory : IServiceScopeFactory
	{
		#region IServiceScopeFactory

		/// <inheritdoc/>
		public IServiceScope CreateScope()
		{
			return new ServiceProvider(null);
		}

		#endregion
	}
}
