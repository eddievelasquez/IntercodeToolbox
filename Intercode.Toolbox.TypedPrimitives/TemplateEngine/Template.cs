// Module Name: Template.cs
// Author:      Eduardo Velasquez
// Copyright (c) 2024, Intercode Consulting, Inc.

namespace Intercode.Toolbox.TypedPrimitives.TemplateEngine;

public record Template(
  string Text,
  Segment[] Segments );
