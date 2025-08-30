// Module Name: TemplateCompilerOptionsBuilder.cs
// Author:      Eduardo Velasquez
// Copyright (c) 2025, Intercode Consulting, Inc.

namespace Intercode.Toolbox.TemplateEngine;

/// <summary>
///   Provides a builder for configuring and creating instances of <see cref="TemplateCompilerOptions" />.
/// </summary>
/// <remarks>
///   This class allows customization of the macro delimiter and argument separator used by the template compiler.
///   It ensures that the configured characters are valid punctuation and distinct from each other.
/// </remarks>
public class TemplateCompilerOptionsBuilder
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

  #endregion

  #region Fields

  private char _macroDelimiter = DefaultMacroDelimiter;
  private char _argumentSeparator = DefaultArgumentSeparator;

  #endregion

  #region Public Methods

  /// <summary>
  ///   Sets the macro delimiter to the specified character.
  /// </summary>
  /// <param name="value">The character to use as the macro delimiter.</param>
  /// <returns>The current instance of <see cref="TemplateCompilerOptionsBuilder" /> to allow method chaining.</returns>
  public TemplateCompilerOptionsBuilder SetMacroDelimiter(
    char value )
  {
    _macroDelimiter = value;
    return this;
  }

  /// <summary>
  ///   Sets the argument separator to the specified character.
  /// </summary>
  /// <param name="value">The character to use as the argument separator.</param>
  /// <returns>The current instance of <see cref="TemplateCompilerOptionsBuilder" /> to allow method chaining.</returns>
  public TemplateCompilerOptionsBuilder SetArgumentSeparator(
    char value )
  {
    _argumentSeparator = value;
    return this;
  }

  /// <summary>
  ///   Builds and returns an instance of <see cref="TemplateCompilerOptions" /> with the configured settings.
  /// </summary>
  /// <remarks>
  ///   This method validates the configured macro delimiter and argument separator to ensure they are distinct
  ///   and valid punctuation characters. If the validation fails, an <see cref="ArgumentException" /> is thrown.
  /// </remarks>
  /// <returns>
  ///   An instance of <see cref="TemplateCompilerOptions" /> with the specified macro delimiter and argument
  ///   separator.
  /// </returns>
  /// <exception cref="ArgumentException">
  ///   Thrown if the macro delimiter and argument separator are the same or if either of them is not a valid punctuation
  ///   character.
  /// </exception>
  public TemplateCompilerOptions Build()
  {
    EnsureIsDelimiter( _macroDelimiter, "MacroDelimiter" );
    EnsureIsDelimiter( _argumentSeparator, "ArgumentSeparator" );

    if( _macroDelimiter == _argumentSeparator )
    {
      throw new ArgumentException( "The macro delimiter and argument separator cannot be the same." );
    }

    return new TemplateCompilerOptions
    {
      MacroDelimiter = _macroDelimiter,
      ArgumentSeparator = _argumentSeparator
    };
  }

  #endregion

  #region Implementation

  private static void EnsureIsDelimiter(
    char c,
    string argName )
  {
    if( !c.IsDelimiterChar() )
    {
      throw new ArgumentException( "Cannot be alphanumeric, underscore, dash or whitespace.", argName );
    }

    return;
  }

  #endregion
}
