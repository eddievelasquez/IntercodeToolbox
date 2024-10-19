// Module Name: TemplateEngineOptions.cs
// Author:      Eduardo Velasquez
// Copyright (c) 2024, Intercode Consulting, Inc.

namespace Intercode.Toolbox.TemplateEngine;

/// <summary>
///   Represents the options for the template engine.
/// </summary>
public class TemplateEngineOptions
{
  #region Constants

  /// <summary>
  ///   The default macro delimiter.
  /// </summary>
  public const char DefaultMacroDelimiter = '$';

  /// <summary>
  ///   The default template engine options.
  /// </summary>
  public static readonly TemplateEngineOptions Default = new ();

  #endregion

  #region Constructors

  /// <summary>
  ///   Initializes a new instance of the <see cref="TemplateEngineOptions" /> class.
  /// </summary>
  /// <param name="macroDelimiter">The macro delimiter. Will default to <see cref="DefaultMacroDelimiter" /> if <c>null</c>.</param>
  public TemplateEngineOptions(
    char? macroDelimiter = null )
  {
    MacroDelimiter = macroDelimiter ?? DefaultMacroDelimiter;
  }

  #endregion

  #region Properties

  /// <summary>
  ///   Gets the macro delimiter.
  /// </summary>
  public char MacroDelimiter { get; }

  #endregion

  #region Implementation

  internal string Delimit(
    string name )
  {
    return $"{MacroDelimiter}{name}{MacroDelimiter}";
  }

  #endregion
}
