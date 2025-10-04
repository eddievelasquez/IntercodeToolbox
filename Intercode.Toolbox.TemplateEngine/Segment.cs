// Module Name: Segment.cs
// Author:      Eduardo Velasquez
// Copyright (c) 2025, Intercode Consulting, Inc.

namespace Intercode.Toolbox.TemplateEngine;

using System.Diagnostics;
using System.Runtime.CompilerServices;

/// <summary>
///   Represents a text segment in a <see cref="Template" />.
/// </summary>
[DebuggerDisplay( "{GetDebuggerString()}" )]
internal readonly record struct Segment
{
  #region Constants

  public static readonly Segment Empty = new ( -1, 0 );

  #endregion

  #region Fields

  private readonly int _textStart;
  private readonly int _argumentStart;
  private readonly ushort _textLength;
  private readonly ushort _argumentLength;
  private readonly short _slot;

  #endregion

  #region Constructors

  private Segment(
    int start,
    int length,
    int argumentStart,
    int argumentLength,
    int slot )
  {
    if( length > ushort.MaxValue )
    {
      throw new ArgumentOutOfRangeException(
        nameof( length ),
        "Length cannot exceed " + ushort.MaxValue
      );
    }

    if( argumentLength > ushort.MaxValue )
    {
      throw new ArgumentOutOfRangeException(
        nameof( argumentLength ),
        "Argument length cannot exceed " + ushort.MaxValue
      );
    }

    if( slot > short.MaxValue )
    {
      throw new ArgumentOutOfRangeException(
        nameof( slot ),
        "Slot cannot exceed " + short.MaxValue
      );
    }

    _textStart = start;
    _textLength = ( ushort ) length;
    _argumentStart = argumentStart;
    _argumentLength = ( ushort ) argumentLength;
    _slot = ( short ) slot;
  }

  private Segment(
    int start,
    int length )
  {
    if( length > ushort.MaxValue )
    {
      throw new ArgumentOutOfRangeException(
        nameof( length ),
        "Length cannot exceed " + ushort.MaxValue
      );
    }

    _textStart = start;
    _textLength = ( ushort ) length;
    _argumentStart = -1;
    _argumentLength = 0;
    _slot = MacroTable.MacroNotFoundSlot;
  }

  #endregion

  #region Properties

  /// <summary>
  ///   Gets the slot identifier associated with the segment.
  /// </summary>
  /// <remarks>
  ///   The slot is used to identify and retrieve the value of a macro during template processing.
  ///   A non-negative value indicates that the segment represents a macro.
  /// </remarks>
  public int Slot => _slot;

  /// <summary>
  ///   Gets a value indicating whether the segment represents a macro.
  /// </summary>
  /// <value>
  ///   <see langword="true" /> if the segment is a macro; otherwise, <see langword="false" />.
  /// </value>
  public bool IsMacro => _slot != MacroTable.MacroNotFoundSlot;

  /// <summary>
  ///   Gets a value indicating whether the segment is constant.
  /// </summary>
  /// <value>
  ///   <c>true</c> if the segment is constant; otherwise, <c>false</c>.
  ///   A segment is considered constant if it is not a macro.
  /// </value>
  public bool IsConstant => !IsMacro;

  #endregion

  #region Public Methods

  /// <summary>
  ///   Retrieves the text content of the segment from the specified <see cref="Template" />.
  /// </summary>
  /// <param name="template">
  ///   The <see cref="Template" /> instance containing the text from which the segment's content is extracted.
  /// </param>
  /// <returns>
  ///   A <see cref="string" /> representing the text content of the segment. Returns an empty string if the segment has no
  ///   content.
  /// </returns>
  public string GetText(
    Template template )
  {
    return _textLength != 0 ? template.Text.Substring( _textStart, _textLength ) : string.Empty;
  }

  /// <summary>
  ///   Retrieves the text span of the segment from the specified <see cref="Template" />.
  /// </summary>
  /// <param name="template">
  ///   The <see cref="Template" /> instance containing the text from which the segment's span is extracted.
  /// </param>
  /// <returns>
  ///   A <see cref="ReadOnlySpan{Char}" /> representing the text span of the segment. Returns an empty span if the segment
  ///   has no content.
  /// </returns>
  public ReadOnlySpan<char> GetTextSpan(
    Template template )
  {
    return _textLength != 0
      ? template.Text.AsSpan( _textStart, _textLength )
      : ReadOnlySpan<char>.Empty;
  }

  /// <summary>
  ///   Retrieves the optional argument of a macro segment from the specified <see cref="Template" />.
  /// </summary>
  /// <param name="template">
  ///   The <see cref="Template" /> instance containing the text from which the macro's argument is extracted.
  /// </param>
  /// <returns>
  ///   A <see cref="ReadOnlySpan{Char}" /> representing the macro's argument. Returns an empty span if the macro has no
  ///   argument.
  /// </returns>
  public ReadOnlySpan<char> GetArgumentSpan(
    Template template )
  {
    return _argumentLength != 0
      ? template.Text.AsSpan( _argumentStart, _argumentLength )
      : ReadOnlySpan<char>.Empty;
  }

  /// <summary>
  ///   Creates a constant segment with the specified starting position and length.
  /// </summary>
  /// <param name="start">
  ///   The starting position of the segment within the text.
  /// </param>
  /// <param name="length">
  ///   The length of the segment.
  /// </param>
  /// <returns>
  ///   A new <see cref="Segment" /> instance representing a constant segment.
  /// </returns>
  [MethodImpl( MethodImplOptions.AggressiveInlining )]
  public static Segment CreateConstant(
    int start,
    int length )
  {
    return new Segment( start, length );
  }

  /// <summary>
  ///   Creates a macro segment with the specified parameters.
  /// </summary>
  /// <param name="start">
  ///   The starting position of the macro segment within the text.
  /// </param>
  /// <param name="length">
  ///   The length of the macro segment.
  /// </param>
  /// <param name="argumentStart">
  ///   The starting position of the macro's argument within the text.
  /// </param>
  /// <param name="argumentLength">
  ///   The length of the macro's argument.
  /// </param>
  /// <param name="slot">
  ///   The slot index associated with the macro's value.
  /// </param>
  /// <returns>
  ///   A new <see cref="Segment" /> representing a macro segment.
  /// </returns>
  [MethodImpl( MethodImplOptions.AggressiveInlining )]
  public static Segment CreateMacro(
    int start,
    int length,
    int argumentStart,
    int argumentLength,
    int slot )
  {
    return new Segment( start, length, argumentStart, argumentLength, slot );
  }

  #endregion

  #region Implementation

  private string GetDebuggerString()
  {
    var builder = StringBuilderPool.Default.Get();

    try
    {
      if( IsMacro )
      {
        builder.Append( "Macro { " );
        builder.Append( "Slot: " );
        builder.Append( Slot );
        builder.Append( ", NameStart: " );
        builder.Append( _textStart );
        builder.Append( ", NameLength: " );
        builder.Append( _textLength );

        if( _argumentLength > 0 )
        {
          builder.Append( ", ArgumentStart: " );
          builder.Append( _argumentStart );
          builder.Append( ", ArgumentLength: " );
          builder.Append( _argumentLength );
        }

        builder.Append( " }" );
      }
      else
      {
        builder.Append( "Constant { " );
        builder.Append( "TextStart: " );
        builder.Append( _textStart );
        builder.Append( ", TextLength: " );
        builder.Append( _textLength );

        builder.Append( " }" );
      }

      return builder.ToString();
    }
    finally
    {
      StringBuilderPool.Default.Return( builder );
    }
  }

  #endregion
}
