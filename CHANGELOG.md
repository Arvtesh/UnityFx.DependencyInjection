# UnityFx.DependencyInjection changelog
All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](http://keepachangelog.com/); this project adheres to [Semantic Versioning](http://semver.org/).

-----------------------
## [0.3.0] - unreleased

### Added
- Added `InjectProperties` extension for `IServiceProvider`.

-----------------------
## [0.2.0] - 2018.09.10

### Added
- Added scopes support (`IServiceScope`, `IServiceScopeFactory`).
- Added `CreateInstance()` extensions to `IServiceProvider`.
- Added `GetRequiredService()` extensions to `IServiceProvider`.
- Added `Contains()` and `Remove()` overloads to `IServiceContainer`.

### Changed
- Changed namespace to `UnityFx.DependencyInjection`.
- Changed `IServiceProvider.GetService()` implementation to return `null` on resolve errors (instead of throwing an exception).
- Changed `ServiceProvider` to have internal constructor. `ServiceProvider` instanced should be created with `BuildServiceProvider` extension of `IServiceCollection`.
- Changed resolve validation to run on `ServiceProvider` construction (not on resolve).

### Removed
- Removed all assembly-specific exceptions. `InvalidOperationException` is thrown on resolve errors instead.

-----------------------
## [0.1.0] - 2018.09.03

### Added
- Initial release.

