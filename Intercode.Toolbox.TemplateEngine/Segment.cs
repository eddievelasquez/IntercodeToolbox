// Module Name: Segment.cs
// Author:      Eduardo Velasquez
// Copyright (c) 2025, Intercode Consulting, Inc.

namespace Intercode.Toolbox.TemplateEngine;

using System.Diagnostics;
using System.Runtime.CompilerServices;

/// <summary>
///   Represents a text segment in a <see cref="Template" />.
/// </summary>
[DebuggerDisplay( "Kind = {Kind}, Text = {Memory}, Slot = {ValueSlot}" )]
public readonly record struct Segment
{
  #region Constructors

  /// <summary>
  ///   Represents a text segment in a <see cref="Template" />.
  /// </summary>
  /// <param name="kind">The kind of text segment.</param>
  /// <param name="memory">The <see cref="Memory" /> that contains the segment's text.</param>
  /// <param name="argumentMemory">Optional <see cref="Memory" /> that contains a macro segment's argument.</param>
  /// <param name="valueSlot"></param>
  private Segment(
    SegmentKind kind,
    ReadOnlyMemory<char> memory,
    ReadOnlyMemory<char> argumentMemory,
    int valueSlot )
  {
    Debug.Assert( valueSlot >= -1 );

    Kind = kind;
    Memory = memory;
    ArgumentMemory = argumentMemory;
    ValueSlot = valueSlot;
  }

  #endregion

  #region Properties

  /// <summary>
  ///   Gets the text representation of the segment.
  /// </summary>
  public string Text => Memory.ToString();

  /// <summary>The kind of text segment.</summary>
  public SegmentKind Kind { get; init; }

  /// <summary>The <see cref="Memory" /> that contains the segment's text.</summary>
  public ReadOnlyMemory<char> Memory { get; init; }

  /// <summary>Optional <see cref="Memory" /> that contains a macro segment's argument.</summary>
  public ReadOnlyMemory<char> ArgumentMemory { get; init; }

  /// <summary>
  ///   Gets or initializes the slot index associated with the macro's value.
  /// </summary>
  /// <remarks>
  ///   A value of <c>-1</c> indicates that the segment is not associated with a macro.
  /// </remarks>
  public int ValueSlot { get; init; }

  #endregion

  #region Public Methods

  /// <summary>
  ///   Creates a constant segment.
  /// </summary>
  /// <param name="memory">The memory that contains the constant segment's text.</param>
  /// <returns></returns>
  [MethodImpl( MethodImplOptions.AggressiveInlining )]
  public static Segment CreateConstant(
    ReadOnlyMemory<char> memory )
  {
    return new Segment( SegmentKind.Constant, memory, ReadOnlyMemory<char>.Empty, -1 );
  }

  /// <summary>
  ///   Creates a delimiter segment.
  /// </summary>
  /// <returns>A new <see cref="Segment" /> representing a delimiter segment.</returns>
  [MethodImpl( MethodImplOptions.AggressiveInlining )]
  public static Segment CreateDelimiter()
  {
    return new Segment( SegmentKind.Delimiter, ReadOnlyMemory<char>.Empty, ReadOnlyMemory<char>.Empty, -1 );
  }

  /// <summary>
  ///   Creates a macro segment.
  /// </summary>
  /// <param name="memory">The memory that contains the macro segment's text.</param>
  /// <param name="valueSlot">The slot index associated with the macro's value.</param>
  /// <returns>A new <see cref="Segment" /> representing a macro segment.</returns>
  [MethodImpl( MethodImplOptions.AggressiveInlining )]
  public static Segment CreateMacro(
    ReadOnlyMemory<char> memory,
    int valueSlot )
  {
    return new Segment( SegmentKind.Macro, memory, ReadOnlyMemory<char>.Empty, valueSlot );
  }

  /// <summary>
  ///   Creates a macro segment.
  /// </summary>
  /// <param name="memory">The memory that contains the macro segment's text.</param>
  /// <param name="argument">The memory that contains the macro segment's argument.</param>
  /// <param name="valueSlot">The slot index associated with the macro's value.</param>
  /// <returns>A new <see cref="Segment" /> representing a macro segment.</returns>
  [MethodImpl( MethodImplOptions.AggressiveInlining )]
  public static Segment CreateMacro(
    ReadOnlyMemory<char> memory,
    ReadOnlyMemory<char> argument,
    int valueSlot )
  {
    return new Segment( SegmentKind.Macro, memory, argument, valueSlot );
  }

  /// <summary>
  ///   Deconstructs the <see cref="Segment" /> into its component properties.
  /// </summary>
  /// <param name="kind">The kind of text segment.</param>
  /// <param name="memory">The memory containing the segment's text.</param>
  /// <param name="argumentMemory">Optional memory containing a macro segment's argument.</param>
  /// <param name="valueSlot">The slot index associated with the macro's value, or -1 if not applicable.</param>
  public void Deconstruct(
    out SegmentKind kind,
    out ReadOnlyMemory<char> memory,
    out ReadOnlyMemory<char> argumentMemory,
    out int valueSlot )
  {
    kind = Kind;
    memory = Memory;
    argumentMemory = ArgumentMemory;
    valueSlot = ValueSlot;
  }

  #endregion
}
