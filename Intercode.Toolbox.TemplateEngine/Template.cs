// Module Name: Template.cs
// Author:      Eduardo Velasquez
// Copyright (c) 2025, Intercode Consulting, Inc.

namespace Intercode.Toolbox.TemplateEngine;

/// <summary>
///   Represents a template with text and segments within the text.
/// </summary>
public readonly record struct Template
{
  #region Constructors

  /// <summary>
  ///   Initializes a new instance of the <see cref="Template" /> class with the specified macro table and segments.
  /// </summary>
  /// <param name="text">The template's text.</param>
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
    string text,
    MacroTable macroTable,
    Segment[] segments )
  {
    Text = text ?? throw new ArgumentNullException( nameof( text ) );
    Segments = segments ?? throw new ArgumentNullException( nameof( segments ) );
    MacroTable = macroTable ?? throw new ArgumentNullException( nameof( macroTable ) );

    if( Segments.Length == 0 )
    {
      throw new ArgumentException(
        "The template must have at least one segment.",
        nameof( segments )
      );
    }
  }

  #endregion

  #region Properties

  /// <summary>
  ///   Gets the text content of the template.
  /// </summary>
  /// <value>
  ///   A <see cref="string" /> representing the text of the template.
  /// </value>
  /// <remarks>
  ///   The text serves as the base content of the template, which may include macro placeholders
  /// </remarks>
  public string Text { get; }

  /// <summary>
  ///   Gets the <see cref="MacroTable" /> associated with this template.
  /// </summary>
  public MacroTable MacroTable { get; }

  /// <summary>The text segments that have been identified by the <see cref="TemplateCompiler" />.</summary>
  internal Segment[] Segments { get; init; }

  #endregion

  #region Public Methods

  /// <summary>
  ///   Creates a new instance of <see cref="MacroValues" /> associated with the <see cref="MacroTable" /> of this template.
  /// </summary>
  public MacroValues CreateValues()
  {
    return MacroTable.CreateValues();
  }

  #endregion
}
