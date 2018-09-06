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
#if NET35
	public interface IServiceCollection : ICollection<ServiceDescriptor>
#else
	public interface IServiceCollection : ICollection<ServiceDescriptor>, IReadOnlyCollection<ServiceDescriptor>
#endif
	{
	}
}
