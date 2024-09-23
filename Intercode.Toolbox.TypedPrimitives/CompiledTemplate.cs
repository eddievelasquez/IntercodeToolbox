// Module Name: CompiledTemplate.cs
// Author:      Eduardo Velasquez
// Copyright (c) 2024, Intercode Consulting, Inc.

namespace Intercode.Toolbox.TypedPrimitives;

public record CompiledTemplate(
  string Template,
  Segment[] Segments,
  int ConstantTextLength );
