# Changelog

## 3.0.0 - 2025-10-04

- Simplified, static API and new object model. The new API is not backward compatible; see [README](README.md#migrating-from-2x-to-30) for migration instructions.
- Compile-time includes for pre-expanding static blocks before macro processing.
- Standard macros are always available; no explicit declaration required.
- Major performance gains. The new algorithm is 43% faster than the previous version.
- Requires .NET Standard 2.0, or .NET 8.0 or later.

## 2.5.1 - 2025-03-09

- Minor macro processing performance improvements

## 2.5.0 - 2025-09-28

- Dropped support for .NET 6.0 and .NET 7.0
- Migrated the solution to the slnx format and reorganized the projects
  into cleaner solution folders.

## 2.4.0 - 2024-10-23

- Added support for macros with dynamically generated values.

## 2.3.1 - 2024-10-19

- Extracted the Template Engine from `TypedPrimitives` project to a standalone package.
- Code cleanup and multiple performance and memory consumption improvements.

