// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Collections;
using Xunit;
using NSubstitute;

namespace UnityFx.DependencyInjection
{
	public class ServiceProviderTests
	{
		[Fact]
		public void GetService_ReturnsNullIfServiceNotFound()
		{
			// Arrange
			var sc = new ServiceCollection();
			var sp = sc.BuildServiceProvider();

			// Act
			var result = sp.GetService(typeof(IEnumerable));

			// Assert
			Assert.Null(result);
		}

		[Theory]
		[InlineData(typeof(SelfReferenceClass))]
		[InlineData(typeof(CrossReferenceClass1))]
		[InlineData(typeof(CrossReferenceClass2))]
		[InlineData(typeof(InvalidService))]
		public void GetService_Throws_ServiceConstructorResolutionException(Type type)
		{
			// Arrange
			var sc = new ServiceCollection();
			sc.Add(new ServiceDescriptor(type, type, ServiceLifetime.Transient));

			// Act/Assert
			Assert.Throws<InvalidOperationException>(() => sc.BuildServiceProvider());
		}

		[Fact]
		public void GetService_CreatesOnlyOneSingletonInstance()
		{
			// Arrange
			var sc = new ServiceCollection();
			sc.Add(new ServiceDescriptor(typeof(IEnumerable), typeof(ArrayList), ServiceLifetime.Singleton));

			var sp = sc.BuildServiceProvider();

			// Act
			var instance1 = sp.GetService(typeof(IEnumerable));
			var instance2 = sp.GetService(typeof(IEnumerable));

			// Assert
			Assert.NotNull(instance1);
			Assert.NotNull(instance2);
			Assert.Same(instance1, instance2);
		}

		[Fact]
		public void GetService_CreatesManyTransientInstances()
		{
			// Arrange
			var sc = new ServiceCollection();
			sc.Add(new ServiceDescriptor(typeof(IEnumerable), typeof(ArrayList), ServiceLifetime.Transient));

			var sp = sc.BuildServiceProvider();

			// Act
			var instance1 = sp.GetService(typeof(IEnumerable));
			var instance2 = sp.GetService(typeof(IEnumerable));

			// Assert
			Assert.NotNull(instance1);
			Assert.NotNull(instance2);
			Assert.NotSame(instance1, instance2);
		}

		[Fact]
		public void GetService_ResolvesMultipleCtors()
		{
			// Arrange
			var sc = new ServiceCollection();
			sc.Add(new ServiceDescriptor(typeof(MultiCtorClass), typeof(MultiCtorClass), ServiceLifetime.Transient));

			var sp = sc.BuildServiceProvider();

			// Act
			var instance = sp.GetService(typeof(MultiCtorClass));

			// Assert
			Assert.NotNull(instance);
		}
	}
}
