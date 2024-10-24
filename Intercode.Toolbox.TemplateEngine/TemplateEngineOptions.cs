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
  ///   Character used to separate the macro's name from its arguments.
  /// </summary>
  public const char DefaultArgumentSeparator = ':';

  /// <summary>
  ///   The default template engine options.
  /// </summary>
  public static readonly TemplateEngineOptions Default = new ();

  #endregion

  #region Constructors

  /// <summary>
  ///   Initializes a new instance of the <see cref="TemplateEngineOptions" /> class.
  /// </summary>
  /// <param name="macroDelimiter">
  ///   The macro delimiter. Will default to <see cref="DefaultMacroDelimiter" /> if <c>null</c>.
  /// </param>
  /// <param name="argumentSeparator">
  ///   The macro's argument separator. Will default to <see cref="DefaultArgumentSeparator" /> if <c>null</c>.
  /// </param>
  /// <exception cref="ArgumentException">
  ///   Thrown if <paramref name="macroDelimiter" /> or <paramref name="argumentSeparator" /> is not a punctuation character.
  /// </exception>
  public TemplateEngineOptions(
    char? macroDelimiter = null,
    char? argumentSeparator = null )
  {
    MacroDelimiter = EnsureIsPunctuation( macroDelimiter, DefaultMacroDelimiter, nameof( macroDelimiter ) );
    ArgumentSeparator = EnsureIsPunctuation( argumentSeparator, DefaultArgumentSeparator, nameof( argumentSeparator ) );
    return;

    static char EnsureIsPunctuation(
      char? c,
      char defaultValue,
      string argName )
    {
      if( c is null )
      {
        return defaultValue;
      }

      var p = c.Value;
      if( !char.IsPunctuation( p ) )
      {
        throw new ArgumentException( "Must be a punctuation character", argName );
      }

      return p;
    }
  }

  #endregion

  #region Properties

  /// <summary>
  ///   Gets the macro delimiter.
  /// </summary>
  public char MacroDelimiter { get; }

  /// <summary>
  ///   Gets the macro's argument separator
  /// </summary>
  public char ArgumentSeparator { get; }

  #endregion
}
