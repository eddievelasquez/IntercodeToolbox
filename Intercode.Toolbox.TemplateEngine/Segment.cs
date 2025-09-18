// Module Name: Segment.cs
// Author:      Eduardo Velasquez
// Copyright (c) 2025, Intercode Consulting, Inc.

namespace Intercode.Toolbox.TemplateEngine;

using System.Diagnostics;
using System.Runtime.CompilerServices;

/// <summary>
///   Represents a text segment in a <see cref="Template" />.
/// </summary>
[DebuggerDisplay( "IsMacro = {IsMacro}, Text = {Memory}, Slot = {Slot}" )]
internal readonly record struct Segment
{
  #region Constructors

  /// <summary>
  ///   Initializes a new instance of the <see cref="Segment" /> struct as a constant segment.
  /// </summary>
  /// <param name="memory">The memory that contains the constant segment's text.</param>
  private Segment(
    ReadOnlyMemory<char> memory )
  {
    IsMacro = false;
    Memory = memory;
    ArgumentMemory = ReadOnlyMemory<char>.Empty;
    Slot = -1;
  }

  /// <summary>
  ///   Initializes a new instance of the <see cref="Segment" /> struct as a macro segment.
  /// </summary>
  /// <param name="memory">The memory that contains the macro segment's text.</param>
  /// <param name="argumentMemory">The memory that contains the macro segment's argument.</param>
  /// <param name="slot">The slot index associated with the macro's value.</param>
  private Segment(
    ReadOnlyMemory<char> memory,
    ReadOnlyMemory<char> argumentMemory,
    int slot )
  {
    IsMacro = true;
    Memory = memory;
    ArgumentMemory = argumentMemory;
    Slot = slot;
  }

  #endregion

  #region Properties

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
  ///   Creates a constant segment.
  /// </summary>
  /// <param name="memory">The memory that contains the constant segment's text.</param>
  /// <returns></returns>
  [MethodImpl( MethodImplOptions.AggressiveInlining )]
  public static Segment CreateConstant(
    ReadOnlyMemory<char> memory )
  {
    return new Segment( memory );
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
    return new Segment( memory, ReadOnlyMemory<char>.Empty, valueSlot );
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
    return new Segment( memory, argument, valueSlot );
  }

  #endregion
}
