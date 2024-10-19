// Module Name: Template.cs
// Author:      Eduardo Velasquez
// Copyright (c) 2024, Intercode Consulting, Inc.

namespace Intercode.Toolbox.TemplateEngine;

/// <summary>
///   Represents a template with text and segments within the text.
/// </summary>
/// <param name="Text">The template's text.</param>
/// <param name="Segments">The text segments that have been identified by the <see cref="TemplateCompiler" />.</param>
public record Template(
  string Text,
  Segment[] Segments );
