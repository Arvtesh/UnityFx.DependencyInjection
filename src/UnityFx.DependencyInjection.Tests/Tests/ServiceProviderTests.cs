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
		[Theory]
		[InlineData(typeof(SelfReferenceClass))]
		[InlineData(typeof(CrossReferenceClass1))]
		[InlineData(typeof(CrossReferenceClass2))]
		[InlineData(typeof(InvalidService))]
		[InlineData(typeof(ArrayListDependentClass))]
		public void BuildServiceProvider_ThrowsOnError(Type type)
		{
			// Arrange
			var sc = new ServiceCollection();
			sc.AddTransient(type);

			// Act/Assert
			Assert.Throws<InvalidOperationException>(() => sc.BuildServiceProvider());
		}

		[Fact]
		public void BuildServiceProvider_ThrowsIfScopedServiceResolvedFromSingleton()
		{
			// Arrange
			var sc = new ServiceCollection();
			sc.AddSingleton<ArrayListDependentClass>();
			sc.AddScoped<ArrayList>();

			// Act/Assert
			Assert.Throws<InvalidOperationException>(() => sc.BuildServiceProvider());
		}

		[Fact]
		public void Dispose_IsCalledOnScopeDisposal()
		{
			// Arrange
			var sc = new ServiceCollection();
			var disposable = Substitute.For<IDisposable>();
			sc.AddSingleton(disposable);

			var sp = sc.BuildServiceProvider();

			// Act
			sp.Dispose();

			// Assert
			disposable.Received(1).Dispose();
		}

		[Theory]
		[InlineData(typeof(SelfReferenceClass))]
		[InlineData(typeof(CrossReferenceClass1))]
		[InlineData(typeof(CrossReferenceClass2))]
		[InlineData(typeof(InvalidService))]
		[InlineData(typeof(ArrayListDependentClass))]
		public void CreateInstance_ThrowsOnError(Type type)
		{
			// Arrange
			var sc = new ServiceCollection();
			var sp = sc.BuildServiceProvider();

			// Act/Assert
			Assert.Throws<InvalidOperationException>(() => sp.CreateInstance(type));
		}

		[Fact]
		public void CreateInstance_MatchesArguments()
		{
			// Arrange
			var sc = new ServiceCollection();
			var sp = sc.BuildServiceProvider();

			// Act
			var result = sp.CreateInstance<ArrayListDependentClass>(new ArrayList());

			// Assert
			Assert.NotNull(result);
		}

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

		[Fact]
		public void GetService_CreatesOnlyOneSingletonInstance()
		{
			// Arrange
			var sc = new ServiceCollection();
			sc.AddSingleton<IEnumerable, ArrayList>();

			var sp = sc.BuildServiceProvider();
			var sp2 = sp.CreateScope().ServiceProvider;

			// Act
			var instance1 = sp.GetService(typeof(IEnumerable));
			var instance2 = sp.GetService(typeof(IEnumerable));
			var instance3 = sp2.GetService(typeof(IEnumerable));
			var instance4 = sp2.GetService(typeof(IEnumerable));

			// Assert
			Assert.NotNull(instance1);
			Assert.NotNull(instance2);
			Assert.NotNull(instance3);
			Assert.NotNull(instance4);
			Assert.Same(instance1, instance2);
			Assert.Same(instance3, instance4);
			Assert.Same(instance1, instance3);
		}

		[Fact]
		public void GetService_CreatesOneScopedInstancePerScope()
		{
			// Arrange
			var sc = new ServiceCollection();
			sc.AddScoped<IEnumerable, ArrayList>();

			var sp = sc.BuildServiceProvider();
			var sp2 = sp.CreateScope().ServiceProvider;

			// Act
			var instance1 = sp.GetService(typeof(IEnumerable));
			var instance2 = sp.GetService(typeof(IEnumerable));
			var instance3 = sp2.GetService(typeof(IEnumerable));
			var instance4 = sp2.GetService(typeof(IEnumerable));

			// Assert
			Assert.NotNull(instance1);
			Assert.NotNull(instance2);
			Assert.NotNull(instance3);
			Assert.NotNull(instance4);
			Assert.Same(instance1, instance2);
			Assert.Same(instance3, instance4);
			Assert.NotSame(instance1, instance3);
		}

		[Fact]
		public void GetService_CreatesManyTransientInstances()
		{
			// Arrange
			var sc = new ServiceCollection();
			sc.AddTransient<IEnumerable, ArrayList>();

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
			sc.AddTransient<MultiCtorClass>();

			var sp = sc.BuildServiceProvider();

			// Act
			var instance = sp.GetService(typeof(MultiCtorClass));

			// Assert
			Assert.NotNull(instance);
		}
	}
}
