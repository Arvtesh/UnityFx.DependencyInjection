﻿// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;

namespace UnityFx.AppStates.DependencyInjection
{
	/// <summary>
	/// Specifies the lifetime of a service defined via <see cref="ServiceDescriptor"/>.
	/// </summary>
	public enum ServiceLifetime
	{
		/// <summary>
		/// Specifies that a single instance of the service will be created.
		/// </summary>
		Singleton,

		/// <summary>
		/// Specifies that a new instance of the service will be created for each scope. Not supported currently.
		/// </summary>
		Scoped,

		/// <summary>
		/// Specifies that a new instance of the service will be created every time it is requested.
		/// </summary>
		Transient
	}
}
