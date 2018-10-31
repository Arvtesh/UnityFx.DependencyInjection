// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
#if !NET35
using System.Runtime.ExceptionServices;
#endif

namespace UnityFx.DependencyInjection
{
	internal class ConstructorServiceFactory : IServiceFactory
	{
		#region data

		private readonly Type _serviceType;
		private readonly ConstructorInfo _ctor;
		private readonly ServiceLifetime _serviceLifetime;
		private readonly ServiceOptions _serviceOptions;

		#endregion

		#region interface

		public ConstructorInfo Ctor => _ctor;

		public ConstructorServiceFactory(IServiceInfo serviceInfo, ConstructorInfo ctor)
		{
			_serviceType = serviceInfo.ServiceType;
			_ctor = ctor;
			_serviceLifetime = serviceInfo.Lifetime;
			_serviceOptions = serviceInfo.Options;
		}

		#endregion

		#region IServiceFactory

		public Type ServiceType => _serviceType;

		public ServiceLifetime Lifetime => _serviceLifetime;

		public ServiceOptions Options => _serviceOptions;

		public object CreateInstance(IServiceProvider serviceProvider)
		{
			try
			{
				if (_ctor != null)
				{
					var parameters = _ctor.GetParameters();
					var args = new object[parameters.Length];

					for (var i = 0; i < args.Length; ++i)
					{
						args[i] = serviceProvider.GetService(parameters[i].ParameterType);
					}

					return _ctor.Invoke(args);
				}
				else
				{
					return Activator.CreateInstance(_serviceType);
				}
			}
			catch (TargetInvocationException e)
			{
#if !NET35
				ExceptionDispatchInfo.Capture(e.InnerException).Throw();
#endif
				throw e.InnerException;
			}
		}

		#endregion

		#region implementation
		#endregion
	}
}
