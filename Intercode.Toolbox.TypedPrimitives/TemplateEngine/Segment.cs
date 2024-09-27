// Module Name: Segment.cs
// Author:      Eduardo Velasquez
// Copyright (c) 2024, Intercode Consulting, Inc.

namespace Intercode.Toolbox.TypedPrimitives.TemplateEngine;

using System.Diagnostics;

[DebuggerDisplay( "Type = {Type}, Text = {Memory}" )]
public readonly record struct Segment(
  SegmentType Type,
  ReadOnlyMemory<char> Memory )
{
  #region Properties

  public string Text => Memory.Span.ToString();

  #endregion
}
