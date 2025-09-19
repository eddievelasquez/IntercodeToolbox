// Module Name: MacroValueGenerator.cs
// Author:      Eduardo Velasquez
// Copyright (c) 2025, Intercode Consulting, Inc.

namespace Intercode.Toolbox.TemplateEngine;

/// <summary>
///   Represents a delegate that generates a string value for a macro dynamically.
/// </summary>
/// <param name="argument">
///   Optional data passed to the delegate to assist in generating the macro value.
/// </param>
/// <returns>
///   A string representing the generated value for the macro, or <c>null</c> if no value is generated.
/// </returns>
public delegate string? MacroValueGenerator(
  ReadOnlySpan<char> argument );
