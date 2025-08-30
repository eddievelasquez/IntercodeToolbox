// Module Name: MacroValueGenerator.cs
// Author:      Eduardo Velasquez
// Copyright (c) 2025, Intercode Consulting, Inc.

namespace Intercode.Toolbox.TemplateEngine;

/// <summary>
///   Callback for dynamic macros.
/// </summary>
/// <param name="argument">Optional data passed to the callback for generating a value.</param>
public delegate string MacroValueGenerator(
  ReadOnlySpan<char> argument );
