// Module Name: Segment.cs
// Author:      Eduardo Velasquez
// Copyright (c) 2024, Intercode Consulting, Inc.

namespace Intercode.Toolbox.TypedPrimitives;

public readonly record struct Segment(
  SegmentType Type,
  ReadOnlyMemory<char> Memory )
{
  #region Properties

  public string Text => Memory.Span.ToString();
  public ReadOnlySpan<char> Span => Memory.Span;

  #endregion
}
