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
/// <param name="ArgumentMemory">Optional <see cref="Memory" /> that contains a macro segment's argument.</param>
[DebuggerDisplay( "Kind = {Kind}, Text = {Memory}" )]
public readonly record struct Segment(
  SegmentKind Kind,
  ReadOnlyMemory<char> Memory,
  ReadOnlyMemory<char> ArgumentMemory )
{
  #region Properties

  /// <summary>
  ///   Gets the text representation of the segment.
  /// </summary>
  public string Text => Memory.ToString();

  #endregion

  #region Public Methods

  /// <summary>
  ///   Creates a constant segment.
  /// </summary>
  /// <param name="memory">The memory that contains the constant segment's text.</param>
  /// <returns></returns>
  public static Segment CreateConstant(
    ReadOnlyMemory<char> memory )
  {
    return new Segment( SegmentKind.Constant, memory, ReadOnlyMemory<char>.Empty );
  }

  /// <summary>
  ///   Creates a delimiter segment.
  /// </summary>
  /// <returns>A new <see cref="Segment" /> representing a delimiter segment.</returns>
  public static Segment CreateDelimiter()
  {
    return new Segment( SegmentKind.Delimiter, ReadOnlyMemory<char>.Empty, ReadOnlyMemory<char>.Empty );
  }

  /// <summary>
  ///   Creates a macro segment.
  /// </summary>
  /// <param name="memory">The memory that contains the macro segment's text.</param>
  /// <returns>A new <see cref="Segment" /> representing a macro segment.</returns>
  public static Segment CreateMacro(
    ReadOnlyMemory<char> memory )
  {
    return new Segment( SegmentKind.Macro, memory, ReadOnlyMemory<char>.Empty );
  }

  /// <summary>
  ///   Creates a macro segment.
  /// </summary>
  /// <param name="memory">The memory that contains the macro segment's text.</param>
  /// <param name="argument">The memory that contains the macro segment's argument.</param>
  /// <returns>A new <see cref="Segment" /> representing a macro segment.</returns>
  public static Segment CreateMacro(
    ReadOnlyMemory<char> memory,
    ReadOnlyMemory<char> argument )
  {
    return new Segment( SegmentKind.Macro, memory, argument );
  }

  #endregion
}
