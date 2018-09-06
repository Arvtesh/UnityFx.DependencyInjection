// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;

namespace UnityFx.DependencyInjection
{
	/// <summary>
	/// Represents an error that occurs when a service requested is not registered.
	/// </summary>
	/// <seealso cref="IServiceProvider"/>
	public class ServiceNotFoundException : ServiceResolutionException
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="ServiceNotFoundException"/> class.
		/// </summary>
		public ServiceNotFoundException(Type serviceType)
			: base(serviceType, string.Format("Service {0} is not registered.", serviceType.Name))
		{
		}
	}
}
