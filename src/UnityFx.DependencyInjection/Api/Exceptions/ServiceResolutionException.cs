// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Runtime.Serialization;

namespace UnityFx.DependencyInjection
{
	/// <summary>
	/// Represents an error that occurs when a service requested cannot be resolved.
	/// </summary>
	/// <seealso cref="IServiceProvider"/>
	public class ServiceResolutionException : Exception
	{
		#region data

		private const string _serviceTypeName = "_serviceType";

		#endregion

		#region interface

		/// <summary>
		/// Gets service type.
		/// </summary>
		public Type ServiceType { get; }

		/// <summary>
		/// Initializes a new instance of the <see cref="ServiceResolutionException"/> class.
		/// </summary>
		public ServiceResolutionException(Type serviceType)
			: base(GetDefaultMessage(serviceType))
		{
			ServiceType = serviceType;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ServiceResolutionException"/> class.
		/// </summary>
		public ServiceResolutionException(Type serviceType, Exception innerException)
			: base(GetDefaultMessage(serviceType), innerException)
		{
			ServiceType = serviceType;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ServiceResolutionException"/> class.
		/// </summary>
		public ServiceResolutionException(Type serviceType, string message)
			: base(message)
		{
			ServiceType = serviceType;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ServiceResolutionException"/> class.
		/// </summary>
		public ServiceResolutionException(Type serviceType, string message, Exception innerException)
			: base(message, innerException)
		{
			ServiceType = serviceType;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ServiceResolutionException"/> class.
		/// </summary>
		protected ServiceResolutionException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
			ServiceType = (Type)info.GetValue(_serviceTypeName, typeof(Type));
		}

		#endregion

		#region ISerializable

		/// <inheritdoc/>
		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue(_serviceTypeName, ServiceType);
		}

		#endregion

		#region implementation

		private static string GetDefaultMessage(Type serviceType)
		{
			return $"Service {serviceType.Name} cannot be resolved.";
		}

		#endregion
	}
}
