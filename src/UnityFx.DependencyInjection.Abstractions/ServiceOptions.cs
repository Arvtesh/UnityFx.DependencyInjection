// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;

namespace UnityFx.DependencyInjection
{
	/// <summary>
	/// Specifies flags for a service defined via <see cref="ServiceDescriptor"/>.
	/// </summary>
	[Flags]
	public enum ServiceOptions
	{
		/// <summary>
		/// No flags.
		/// </summary>
		None = 0,

		/// <summary>
		/// Specifies that the service instance should not be disposed by service provider.
		/// </summary>
		DoNotDispose = 1
	}
}
