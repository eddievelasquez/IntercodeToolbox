// Module Name: Template.cs
// Author:      Eduardo Velasquez
// Copyright (c) 2025, Intercode Consulting, Inc.

namespace Intercode.Toolbox.TemplateEngine;

using System.Runtime.InteropServices;

/// <summary>
///   Represents a template with text and segments within the text.
/// </summary>
public record Template
{
  #region Constructors

  /// <summary>
  ///   Initializes a new instance of the <see cref="Template" /> class with the specified macro table and segments.
  /// </summary>
  /// <param name="macroTable">
  ///   The <see cref="MacroTable" /> containing macro definitions used by the template.
  /// </param>
  /// <param name="segments">
  ///   An array of <see cref="Segment" /> objects representing the segments of the template.
  /// </param>
  /// <exception cref="ArgumentNullException">
  ///   Thrown when <paramref name="macroTable" /> or <paramref name="segments" /> is <c>null</c>.
  /// </exception>
  /// <exception cref="ArgumentException">
  ///   Thrown when <paramref name="segments" /> is empty.
  /// </exception>
  internal Template(
    MacroTable macroTable,
    Segment[] segments )
  {
    Segments = segments ?? throw new ArgumentNullException( nameof( segments ) );
    MacroTable = macroTable ?? throw new ArgumentNullException( nameof( macroTable ) );

    if( Segments.Length == 0 )
    {
      throw new ArgumentException( "The template must have at least one segment.", nameof( segments ) );
    }
  }

  #endregion

  #region Properties

  /// <summary>
  ///   Gets the <see cref="MacroTable" /> associated with this template.
  /// </summary>
  public MacroTable MacroTable { get; }

  /// <summary>
  ///   The template's text
  /// </summary>
  public string Text
  {
    get
    {
      var textMemory = Segments[0].Memory;

      if( textMemory.IsEmpty )
      {
        return string.Empty;
      }

      if( MemoryMarshal.TryGetString( textMemory, out var text, out var start, out var length ) )
      {
        return text;
      }

      throw new InvalidOperationException( "Cannot get the template text" );
    }
  }

  /// <summary>The text segments that have been identified by the <see cref="TemplateCompiler" />.</summary>
  public Segment[] Segments { get; init; }

  #endregion
}
