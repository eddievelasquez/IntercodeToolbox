// Module Name: Template.cs
// Author:      Eduardo Velasquez
// Copyright (c) 2024, Intercode Consulting, Inc.

namespace Intercode.Toolbox.TemplateEngine;

using System.Collections.Frozen;
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
  /// <param name="segments">The text segments that have been identified by the <see cref="TemplateCompiler" />.</param>
  /// <param name="macroTable">The dictionary containing macro names and their corresponding slot indices.</param>
  internal Template(
    Segment[] segments,
    IDictionary<string, int> macroTable )
  {
    if( segments == null )
    {
      throw new ArgumentNullException( nameof( segments ) );
    }

    if( segments.Length == 0 )
    {
      throw new ArgumentException( "The template must have at least one segment.", nameof( segments ) );
    }

    if( macroTable == null )
    {
      throw new ArgumentNullException( nameof( macroTable ) );
    }

    Segments = segments;
    MacroTable = macroTable.ToFrozenDictionary( StringComparer.OrdinalIgnoreCase );
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

  internal FrozenDictionary<string, int> MacroTable { get; init; }

  #endregion

  #region Public Methods

  /// <summary>
  ///   Creates an instance of <see cref="TemplateMacroValues" /> for the current template.
  /// </summary>
  /// <returns>
  ///   A new <see cref="TemplateMacroValues" /> object that provides functionality
  ///   to manage and retrieve macro values associated with the template.
  /// </returns>
  public TemplateMacroValues CreateMacroValues()
  {
    return new TemplateMacroValues( this );
  }

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
