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
  ///   Initializes a new instance of the <see cref="Segment" /> struct.
  /// </summary>
  /// <param name="memory">
  ///   The memory that contains the text content of the segment.
  /// </param>
  /// <param name="argumentMemory">
  ///   The memory that contains the argument for the macro segment, if applicable.
  /// </param>
  /// <param name="slot">
  ///   The slot index associated with the macro's value, or -1 if not applicable.
  /// </param>
  private Segment(
    ReadOnlyMemory<char> memory,
    ReadOnlyMemory<char> argumentMemory = default,
    int slot = -1 )
  {
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
  public bool IsMacro => Slot >= 0;

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

  /// <summary>
  ///   The <see cref="Memory" /> that contains the segment's text.
  /// </summary>
  public ReadOnlyMemory<char> Memory { get; init; }

  /// <summary>
  ///   Optional <see cref="Memory" /> that contains a macro segment's argument.
  /// </summary>
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
  /// <param name="memory">The memory that contains the text content of the constant segment.</param>
  /// <returns>A new <see cref="Segment" /> representing a constant segment.</returns>
  [MethodImpl( MethodImplOptions.AggressiveInlining )]
  public static Segment CreateConstant(
    ReadOnlyMemory<char> memory )
  {
    return new Segment( memory );
  }

  /// <summary>
  ///   Creates a macro segment with the specified parameters.
  /// </summary>
  /// <param name="slot">The slot index associated with the macro's value.</param>
  /// <param name="memory">The memory that contains the macro segment's text.</param>
  /// <param name="argument">The memory that contains the macro segment's argument.</param>
  /// <returns>A new <see cref="Segment" /> representing a macro segment.</returns>
  [MethodImpl( MethodImplOptions.AggressiveInlining )]
  public static Segment CreateMacro(
    int slot,
    ReadOnlyMemory<char> memory,
    ReadOnlyMemory<char> argument = default )
  {
    return new Segment( memory, argument, slot );
  }

  #endregion
}
