// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace UnityFx.DependencyInjection
{
	/// <summary>
	/// Extensions for <see cref="IServiceCollection"/>.
	/// </summary>
	[EditorBrowsable(EditorBrowsableState.Advanced)]
	public static class ServiceCollectionExtensions
	{
		/// <summary>
		/// Creates a <see cref="ServiceProvider"/> containing services from the provided <see cref="IServiceCollection"/>.
		/// </summary>
		/// <param name="services">The source <see cref="IServiceCollection"/> containing service descriptors.</param>
		public static ServiceProvider BuildServiceProvider(this IServiceCollection services)
		{
			return new ServiceProvider(services, true);
		}

		/// <summary>
		/// Creates a <see cref="ServiceProvider"/> containing services from the provided <see cref="IServiceCollection"/>.
		/// </summary>
		/// <param name="services">The source <see cref="IServiceCollection"/> containing service descriptors.</param>
		/// <param name="validate">If <see langword="true"/> all registered services are validated during <see cref="ServiceProvider"/> construction.</param>
		public static ServiceProvider BuildServiceProvider(this IServiceCollection services, bool validate)
		{
			return new ServiceProvider(services, validate);
		}
	}
}
