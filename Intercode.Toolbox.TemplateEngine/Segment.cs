// Module Name: Segment.cs
// Author:      Eduardo Velasquez
// Copyright (c) 2025, Intercode Consulting, Inc.

namespace Intercode.Toolbox.TemplateEngine;

using System.Diagnostics;
using System.Runtime.CompilerServices;

/// <summary>
///   Represents a text segment in a <see cref="Template" />.
/// </summary>
[DebuggerDisplay( "IsMacro = {IsMacro}, Text = {Memory}, Start = {Start}, Slot = {Slot}" )]
internal readonly record struct Segment
{
  // TODO: Consider using a custom "span" type to represent a start index and length of the segment
  //       within the template text, instead of using Memory<char> that doesn't provide a way to
  //       obtain the length directly; which is needed for merging segments.

  #region Constructors

  /// <summary>
  ///   Initializes a new instance of the <see cref="Segment" /> struct as a constant segment.
  /// </summary>
  /// <param name="start">The starting position of the segment.</param>
  /// <param name="memory">The memory that contains the constant segment's text.</param>
  private Segment(
    int start,
    ReadOnlyMemory<char> memory )
  {
    Start = start;
    IsMacro = false;
    Memory = memory;
    ArgumentMemory = ReadOnlyMemory<char>.Empty;
    Slot = -1;
  }

  /// <summary>
  ///   Initializes a new instance of the <see cref="Segment" /> struct as a macro segment.
  /// </summary>
  /// <param name="start">The starting position of the segment.</param>
  /// <param name="memory">The memory that contains the macro segment's text.</param>
  /// <param name="argumentMemory">The memory that contains the macro segment's argument.</param>
  /// <param name="slot">The slot index associated with the macro's value.</param>
  private Segment(
    int start,
    ReadOnlyMemory<char> memory,
    ReadOnlyMemory<char> argumentMemory,
    int slot )
  {
    Start = start;
    IsMacro = true;
    Memory = memory;
    ArgumentMemory = argumentMemory;
    Slot = slot;
  }

  #endregion

  #region Properties

  /// <summary>
  ///   Gets the starting position of the segment within the template text.
  /// </summary>
  /// <remarks>
  ///   This property indicates the zero-based index where the segment begins in the template text.
  ///   It is used to determine the position of the segment for operations such as merging or slicing.
  /// </remarks>
  public int Start { get; }

  /// <summary>
  ///   Gets a value indicating whether the segment represents a macro.
  /// </summary>
  /// <value>
  ///   <see langword="true" /> if the segment is a macro; otherwise, <see langword="false" />.
  /// </value>
  public bool IsMacro { get; }

  /// <summary>
  ///   Gets a value indicating whether the segment is constant.
  /// </summary>
  /// <value>
  ///   <c>true</c> if the segment is constant; otherwise, <c>false</c>.
  ///   A segment is considered constant if it is not a macro.
  /// </value>
  public bool IsConstant => !IsMacro;

  /// <summary>
  ///   Gets the text representation of the segment.
  /// </summary>
  public string Text => Memory.ToString();

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
  internal int Slot { get; init; }

  #endregion

  #region Public Methods

  /// <summary>
  ///   Creates a constant segment with the specified starting position and text content.
  /// </summary>
  /// <param name="start">The starting position of the segment within the template.</param>
  /// <param name="memory">The memory that contains the text content of the constant segment.</param>
  /// <returns>A new <see cref="Segment" /> representing a constant segment.</returns>
  [MethodImpl( MethodImplOptions.AggressiveInlining )]
  public static Segment CreateConstant(
    int start,
    ReadOnlyMemory<char> memory )
  {
    return new Segment( start, memory );
  }

  /// <summary>
  ///   Creates a macro segment with the specified starting position, text content, and value slot.
  /// </summary>
  /// <param name="start">The starting position of the segment within the template.</param>
  /// <param name="memory">The memory that contains the macro segment's text.</param>
  /// <param name="valueSlot">The slot index associated with the macro's value.</param>
  /// <returns>A new <see cref="Segment" /> representing a macro segment.</returns>
  [MethodImpl( MethodImplOptions.AggressiveInlining )]
  public static Segment CreateMacro(
    int start,
    ReadOnlyMemory<char> memory,
    int valueSlot )
  {
    return new Segment( start, memory, ReadOnlyMemory<char>.Empty, valueSlot );
  }

  /// <summary>
  ///   Creates a macro segment with the specified parameters.
  /// </summary>
  /// <param name="start">The starting position of the macro segment in the template.</param>
  /// <param name="memory">The memory that contains the macro segment's text.</param>
  /// <param name="argument">The memory that contains the macro segment's argument.</param>
  /// <param name="valueSlot">The slot index associated with the macro's value.</param>
  /// <returns>A new <see cref="Segment" /> representing a macro segment.</returns>
  [MethodImpl( MethodImplOptions.AggressiveInlining )]
  public static Segment CreateMacro(
    int start,
    ReadOnlyMemory<char> memory,
    ReadOnlyMemory<char> argument,
    int valueSlot )
  {
    return new Segment( start, memory, argument, valueSlot );
  }

  #endregion
}
