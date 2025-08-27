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

  #region Fields

  private readonly char _macroDelimiter = DefaultMacroDelimiter;
  private readonly char _argumentSeparator = DefaultArgumentSeparator;

  #endregion

  #region Properties

  /// <summary>
  ///   Gets or initializes the macro delimiter. Will default to <see cref="DefaultMacroDelimiter" /> if <c>null</c>.
  /// </summary>
  /// <exception cref="ArgumentException">
  ///   Thrown if <paramref name="value" /> is not a punctuation character.
  /// </exception>
  public char MacroDelimiter
  {
    get => _macroDelimiter;
    init => _macroDelimiter = EnsureIsPunctuation( value, nameof( MacroDelimiter ) );
  }

  /// <summary>
  ///   Gets the macro's argument separator. Will default to <see cref="DefaultArgumentSeparator" /> if <c>null</c>.
  /// </summary>
  /// <exception cref="ArgumentException">
  ///   Thrown if <paramref name="value" /> is not a punctuation character.
  /// </exception>
  public char ArgumentSeparator
  {
    get => _argumentSeparator;
    init => _argumentSeparator = EnsureIsPunctuation( value, nameof( ArgumentSeparator ) );
  }

  /// <summary>
  ///   Gets or initializes a value indicating whether the standard macros should be registered
  ///   by the template engine.
  /// </summary>
  /// <value>
  ///   <c>true</c> if the standard macros should be registered; otherwise, <c>false</c>.
  /// </value>
  public bool RegisterStandardMacros { get; init; }

  #endregion

  #region Implementation

  private static char EnsureIsPunctuation(
    char c,
    string argName )
  {
    if( !char.IsPunctuation( c ) )
    {
      throw new ArgumentException( "Must be a punctuation character", argName );
    }

    return c;
  }

  #endregion
}
