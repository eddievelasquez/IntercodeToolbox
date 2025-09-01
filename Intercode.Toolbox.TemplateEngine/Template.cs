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
  ///   Represents a template with text and segments within the text.
  /// </summary>
  /// <param name="context"></param>
  /// <param name="segments">The text segments that have been identified by the <see cref="TemplateCompiler" />.</param>
  internal Template(
    MacroProcessorContext context,
    Segment[] segments )
  {
    if( segments == null )
    {
      throw new ArgumentNullException( nameof( segments ) );
    }

    if( segments.Length == 0 )
    {
      throw new ArgumentException( "The template must have at least one segment.", nameof( segments ) );
    }

    Context = context ?? throw new ArgumentNullException( nameof( context ) );
    Segments = segments;
  }

  #endregion

  #region Properties

  /// <summary>
  ///   Gets the context associated with the template, which provides configuration
  ///   and macro definitions for processing the template.
  /// </summary>
  public MacroProcessorContext Context { get; }

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
