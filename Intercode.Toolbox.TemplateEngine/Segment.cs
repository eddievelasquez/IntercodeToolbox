// Module Name: Segment.cs
// Author:      Eduardo Velasquez
// Copyright (c) 2024, Intercode Consulting, Inc.

namespace Intercode.Toolbox.TemplateEngine;

using System.Diagnostics;

/// <summary>
///   Represents a text segment in a <see cref="Template" />.
/// </summary>
/// <param name="Kind">The kind of text segment.</param>
/// <param name="Memory">The <see cref="Memory" /> that contains the segment's text.</param>
[DebuggerDisplay( "Kind = {Kind}, Text = {Memory}" )]
public readonly record struct Segment(
  SegmentKind Kind,
  ReadOnlyMemory<char> Memory )
{
  #region Properties

  /// <summary>
  ///   Gets a value indicating whether the segment is a macro.
  /// </summary>
  public bool IsMacro => Kind == SegmentKind.Macro;

  /// <summary>
  ///   Gets the text representation of the segment.
  /// </summary>
  public string Text => Memory.ToString();

  #endregion
}
