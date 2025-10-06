# Changelog

## 2.6.0 - 2025-10-04

- Updated to `Intercode.Toolbox.TemplateEngine` version `3.0.0`.
- **Breaking Change**: The generated value converter class name has changed from `{TypeName}ValueConverter` to `{TypeName}EFCoreValueConverter` to avoid conflicts with other value converters.

## 2.5.1 - 2025-03-09

- Typed primitives now implement `IComparable<T>` with the underlying primitive type.
- Added the `Empty` static readonly instance.
- Added the `HasValue` property. It replaces the `IsDefault` property, which may be deprecated in the future.
- Added the `GetValueOrDefault()` method. It replaces the `ValueOrDefault` property, which may be deprecated in the future.
- Added the `GetValueOrDefault(defaultValue)` method, which returns the provided default value if `HasValue` returns `false`.
- `CompareTo(object?)` now supports comparisons with the underlying primitive type as well.
- The conversion operator to underlying primitive type is now implicit.
- No longer need to change the `DateParseHandling` setting for the Newtonsoft Json deserialization.
- Internal cleanup and refactoring to enable better unit testing and future enhancements.

- **Bug Fix**: The EF Value converters were not properly supporting nullable fields.

## 2.5.0 - 2024-11-13

 - All primitives now implement the `CreateOrThrow` and `ValidateOrThrow` static methods for scenarios where handling a result value is not possible.
 - All primitives (except string and Uri) implement the `IFormatable` and `IParsable` interfaces. For .NET 7+, `ISpanFormattable` and `ISpanParsable` are implemented as well.

## 2.4.3 - 2024-11-05

- Added support for the following primitive types: **Byte**, **SByte**, **Int16**, **UInt16**, **UInt32**, **UInt64**, **Single**,
  **Double**, **Decimal**, **TimeSpan**, and **Uri**.

## 2.2 - 2024-10-12

- Moved the template processing engine to its own package: [Intercode.Toolbox.TemplateEngine](https://www.nuget.org/packages/Intercode.Toolbox.TemplateEngine/).
