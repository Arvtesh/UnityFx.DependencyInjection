// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Collections;
using System.Collections.Generic;

namespace UnityFx.DependencyInjection
{
	/// <summary>
	/// Default implementation of <see cref="IServiceCollection"/>.
	/// </summary>
	/// <seealso cref="ServiceDescriptor"/>
	public class ServiceCollection : List<ServiceDescriptor>, IServiceCollection
	{
		#region data
		#endregion

		#region interface
		#endregion

		#region IServiceCollection

		/// <inheritdoc/>
		public bool Contains(Type serviceType)
		{
			if (serviceType != null)
			{
				foreach (var item in this)
				{
					if (item.ServiceType == serviceType)
					{
						return true;
					}
				}
			}

			return false;
		}

		/// <inheritdoc/>
		public bool Remove(Type serviceType)
		{
			if (serviceType != null)
			{
				for (var i = 0; i < Count; ++i)
				{
					if (this[i].ServiceType == serviceType)
					{
						RemoveAt(i);
						return true;
					}
				}
			}

			return false;
		}

		#endregion

		#region implementation
		#endregion
	}
}
