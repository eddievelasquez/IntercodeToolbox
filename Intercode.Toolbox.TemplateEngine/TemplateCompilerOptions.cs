// Module Name: TemplateCompilerOptions.cs
// Author:      Eduardo Velasquez
// Copyright (c) 2024, Intercode Consulting, Inc.

namespace Intercode.Toolbox.TemplateEngine;

/// <summary>
///   Represents the options for the <see cref="TemplateCompiler" />.
/// </summary>
public class TemplateCompilerOptions
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
  public static readonly TemplateCompilerOptions Default = new ();

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
