// Module Name: TemplateCompilerOptions.cs
// Author:      Eduardo Velasquez
// Copyright (c) 2024, Intercode Consulting, Inc.

namespace Intercode.Toolbox.TemplateEngine;

/// <summary>
///   Represents the options for the <see cref="TemplateCompiler" />.
/// </summary>
public sealed class TemplateCompilerOptions
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

  #region Constructors

  /// <summary>
  ///   Initializes a new instance of the <see cref="TemplateCompilerOptions" /> class
  ///   with default settings.
  /// </summary>
  /// <remarks>
  ///   This constructor sets the macro delimiter and argument separator to their default values.
  ///   The default macro delimiter is <see cref="DefaultMacroDelimiter" />.
  ///   The default argument separator is <see cref="DefaultArgumentSeparator" />.
  /// </remarks>
  public TemplateCompilerOptions()
  {
    MacroDelimiter = DefaultMacroDelimiter;
    ArgumentSeparator = DefaultArgumentSeparator;
  }

  /// <summary>
  ///   Initializes a new instance of the <see cref="TemplateCompilerOptions" /> class
  ///   with the specified macro delimiter and argument separator.
  /// </summary>
  /// <param name="macroDelimiter">
  ///   The character to use as the macro delimiter. This character must not be alphanumeric,
  ///   an underscore ('_'), a dash ('-'), or whitespace.
  /// </param>
  /// <param name="argumentSeparator">
  ///   The character to use as the argument separator. This character must not be alphanumeric,
  ///   an underscore ('_'), a dash ('-'), or whitespace.
  /// </param>
  /// <exception cref="ArgumentException">
  ///   Thrown when the <paramref name="macroDelimiter" /> and <paramref name="argumentSeparator" /> are the same,
  ///   or when either of them is invalid.
  /// </exception>
  /// <remarks>
  ///   Use this constructor to customize the macro delimiter and argument separator for the template compiler.
  /// </remarks>
  public TemplateCompilerOptions(
    char macroDelimiter,
    char argumentSeparator )
  {
    EnsureIsDelimiter( macroDelimiter, nameof( macroDelimiter ) );
    EnsureIsDelimiter( argumentSeparator, nameof( argumentSeparator ) );

    if( macroDelimiter == argumentSeparator )
    {
      throw new ArgumentException( "The macro delimiter and argument separator cannot be the same." );
    }

    MacroDelimiter = macroDelimiter;
    ArgumentSeparator = argumentSeparator;
    return;

    static void EnsureIsDelimiter(
      char c,
      string argName )
    {
      if( !c.IsDelimiterChar() )
      {
        throw new ArgumentException( "Cannot be alphanumeric, underscore, dash or whitespace.", argName );
      }
    }
  }

  #endregion

  #region Properties

  /// <summary>
  ///   Gets the character used as the macro delimiter in the template engine.
  /// </summary>
  /// <remarks>
  ///   The macro delimiter is used to identify the start and end of macros within templates.
  ///   By default, this is set to <see cref="DefaultMacroDelimiter" />.
  /// </remarks>
  public char MacroDelimiter { get; }

  /// <summary>
  ///   Gets the character used to separate arguments within a macro.
  /// </summary>
  /// <remarks>
  ///   The <see cref="ArgumentSeparator" /> is used to distinguish between the macro name
  ///   and its arguments in a template. By default, this is set to <see cref="DefaultArgumentSeparator" />.
  /// </remarks>
  public char ArgumentSeparator { get; }

  #endregion
}
