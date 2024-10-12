// Module Name: Segment.cs
// Author:      Eduardo Velasquez
// Copyright (c) 2024, Intercode Consulting, Inc.

namespace Intercode.Toolbox.TypedPrimitives.TemplateEngine;

using System.Diagnostics;

[DebuggerDisplay( "Kind = {Kind}, Text = {Memory}" )]
public readonly record struct Segment(
  SegmentKind Kind,
  ReadOnlyMemory<char> Memory )
{
  #region Properties

  public string Text => Memory.ToString();

  #endregion
}
