// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;

namespace UnityFx.DependencyInjection
{
	internal static class Messages
	{
		public static string FormatImplementationTypeIsAbstract(Type serviceType, Type implementationType)
		{
			return $"Implementation type registered for service {serviceType.Name} is not concrete ({implementationType.Name}).";
		}

		public static string FormatRequiredServiceRegistration(Type serviceType)
		{
			return $"Attempt to register a required service {serviceType.Name}.";
		}

		public static string FormatInvalidServiceDescriptor(Type serviceType)
		{
			return $"Invalid descriptor specified for service {serviceType.Name}.";
		}

		public static string FormatDependencyLoop(Type serviceType, Type argType)
		{
			return $"Dependency loop detected in service constructor {serviceType.Name} (argument type {argType}).";
		}

		public static string FormatSingletonArgumentScope(Type serviceType, Type argType)
		{
			return $"A scoped service {argType.Name} is passed to a singleton constructor of service {serviceType.Name}.";
		}

		public static string FormatNoConstructorApplicable(Type serviceType)
		{
			return $"A suitable constructor for type '{serviceType}' could not be located. Ensure the type is concrete and services are registered for all parameters of a public constructor.";
		}
	}
}
