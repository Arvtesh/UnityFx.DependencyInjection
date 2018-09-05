# UnityFx.DependencyInjection

Channel  | UnityFx.DependencyInjection |
---------|---------------|
AppVeyor | [![Build status](https://ci.appveyor.com/api/projects/status/hfmq9vow53al7tpd/branch/master?svg=true)](https://ci.appveyor.com/project/Arvtesh/unityfx-dependencyinjection/branch/master) [![AppVeyor tests](https://img.shields.io/appveyor/tests/Arvtesh/unityFx-dependencyinjection.svg)](https://ci.appveyor.com/project/Arvtesh/unityfx-dependencyinjection/build/tests)
NuGet | [![NuGet](https://img.shields.io/nuget/v/UnityFx.DependencyInjection.svg)](https://www.nuget.org/packages/UnityFx.DependencyInjection)
Github | [![GitHub release](https://img.shields.io/github/release/Arvtesh/UnityFx.DependencyInjection.svg?logo=github)](https://github.com/Arvtesh/UnityFx.DependencyInjection/releases)

## Synopsis

*UnityFx.DependencyInjection* is a lightweight ASP.NET-like dependency injection framework for Unity. Basically it provides an implementation of [IServiceProvider](https://docs.microsoft.com/en-us/dotnet/api/system.iserviceprovider) and ways to initialize it. It only supports constructor injection.

## Getting Started
### Prerequisites
You may need the following software installed in order to build/use the library:
- [Microsoft Visual Studio 2017](https://www.visualstudio.com/vs/community/).
- [Unity3d](https://store.unity.com/).

### Getting the code
You can get the code by cloning the github repository using your preffered git client UI or you can do it from command line as follows:
```cmd
git clone https://github.com/Arvtesh/UnityFx.DependencyInjection.git
git submodule -q update --init
```
### Getting binaries
The binaries are available as a [NuGet package](https://www.nuget.org/packages/UnityFx.DependencyInjection). See [here](http://docs.nuget.org/docs/start-here/using-the-package-manager-console) for instructions on installing a package via nuget. One can also download them directly from [Github releases](https://github.com/Arvtesh/UnityFx.DependencyInjection/releases).

## Understanding the concepts
As outlined in Wikipedia, [dependency injection](https://en.wikipedia.org/wiki/Dependency_injection) is a technique whereby one object (or static method) supplies the dependencies of another object. A dependency is an object that can be used (a service). An injection is the passing of a dependency to a dependent object (a client) that would use it. The service is made part of the client's state. Passing the service to the client, rather than allowing a client to build or find the service, is the fundamental requirement of the pattern.

This fundamental requirement means that using values (services) produced within the class from new or static methods is prohibited. The client should accept values passed in from outside. This allows the client to make acquiring dependencies someone else's problem.

The intent behind dependency injection is to decouple objects to the extent that no client code has to be changed simply because an object it depends on needs to be changed to a different one.

Dependency injection is one form of the broader technique of [inversion of control](https://en.wikipedia.org/wiki/Inversion_of_control). As with other forms of inversion of control, dependency injection supports the [dependency inversion principle](https://en.wikipedia.org/wiki/Dependency_inversion_principle). The client delegates the responsibility of providing its dependencies to external code (the injector). The client is not allowed to call the injector code; it is the injecting code that constructs the services and calls the client to inject them. This means the client code does not need to know about the injecting code, how to construct the services or even which actual services it is using; the client only needs to know about the intrinsic interfaces of the services because these define how the client may use the services. This separates the responsibilities of use and construction. 

Dependency injection involves four roles:
* The *service* object(s) to be used.
* The *client* object that depends on the service(s) it uses.
* The *interfaces* that define how the client may use the services.
* The *injector* (aka DI container), which is responsible for constructing the services and injecting them into the client.

There are at least 3 ways an object can receive a reference to a service:
* Constructor injection.
* Setter (property) injection.
* Interface injection.

### Constructor injection
In constructor injection all client dependencies are provided through a class constructor. Preferred when all dependencies can be constructed first because it can be used to ensure the client object is always in a valid state, as opposed to having some of its dependency references be null (not be set). However, on its own, it lacks the flexibility to have its dependencies changed later. This can be a first step towards making the client immutable and therefore thread safe. In general, this is the recommended approach.

### Setter injection
In setter injection the client exposes a setter method (or property) that the injector (DI container) uses for injection of the dependencies. Requires clients to provide a setter method for each dependency. This gives the freedom to manipulate the state of the dependency references at any time. This offers flexibility, but if there is more than one dependency to be injected, it is difficult for the client to ensure that all dependencies are injected before the client could be provided for use.

### Interface injection
In interface injection the dependency provides an injector method that will inject the dependency into any client passed to it. Clients must implement an interface that exposes a setter method that accepts the dependency. The advantage of interface injection is that dependencies can be completely ignorant of their clients yet can still receive a reference to a new client and, using it, send a reference-to-self back to the client. In this way, the dependencies become injectors. The key is that the injecting method (which could just be a classic setter method) is provided through an interface.

## Using the library
Reference the DLL and import the namespace:
```csharp
using UnityFx.DependencyInjection;
```
TODO

## Motivation
TODO

## Documentation
Please see the links below for extended information on the product:
- [CHANGELOG](CHANGELOG.md).
- [SUPPORT](.github/SUPPORT.md).

## Useful links
- [Dependency injection](https://en.wikipedia.org/wiki/Dependency_injection).
- [Dependency injection in ASP.NET Core](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/dependency-injection).
- [TinyIoC](https://github.com/grumpydev/TinyIoC).
- [StrangeIoC](https://github.com/strangeioc/strangeioc).

## Contributing
Please see [contributing guide](.github/CONTRIBUTING.md) for details.

## Versioning
The project uses [SemVer](https://semver.org/) versioning pattern. For the versions available, see [tags in this repository](https://github.com/Arvtesh/UnityFx.DependencyInjection/tags).

## License
Please see the [![license](https://img.shields.io/github/license/Arvtesh/UnityFx.DependencyInjection.svg)](LICENSE.md) for details.

## Acknowledgments
Working on this project is a great experience. Please see below list of sources of my inspiration (in no particular order):
* [ASP.NET source](https://github.com/aspnet/DependencyInjection). A great source of knowledge and good programming practices.
* Everyone who ever commented or left any feedback on the project. It's always very helpful.
