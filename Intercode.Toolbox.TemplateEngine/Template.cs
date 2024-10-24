// Module Name: Template.cs
// Author:      Eduardo Velasquez
// Copyright (c) 2024, Intercode Consulting, Inc.

namespace Intercode.Toolbox.TemplateEngine;

using System.Runtime.InteropServices;

/// <summary>
///   Represents a template with text and segments within the text.
/// </summary>
public record Template
{
  #region Constructors

  /// <summary>
  ///   Represents a template with text and segments within the text.
  /// </summary>
  /// <param name="Segments">The text segments that have been identified by the <see cref="TemplateCompiler" />.</param>
  public Template(
    Segment[] Segments )
  {
    if( Segments == null )
    {
      throw new ArgumentNullException( nameof( Segments ) );
    }

    if( Segments.Length == 0 )
    {
      throw new ArgumentException( "The template must have at least one segment.", nameof( Segments ) );
    }

    this.Segments = Segments;
  }

  #endregion

  #region Properties

  /// <summary>
  ///   The template's text
  /// </summary>
  public string Text
  {
    get
    {
      if( MemoryMarshal.TryGetString( Segments[0].Memory, out var text, out var start, out var length ) )
      {
        return text;
      }

      throw new InvalidOperationException( "Cannot get the template text" );
    }
  }

  /// <summary>The text segments that have been identified by the <see cref="TemplateCompiler" />.</summary>
  public Segment[] Segments { get; init; }

  #endregion

  #region Public Methods

  /// <summary>
  ///   Deconstructs the <see cref="Template" /> into its segments.
  /// </summary>
  /// <param name="segments">The array of segments that make up the template.</param>
  public void Deconstruct(
    out Segment[] segments )
  {
    segments = Segments;
  }

  #endregion
}
