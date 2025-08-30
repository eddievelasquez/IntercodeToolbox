// Module Name: MacroExtensions.cs
// Author:      Eduardo Velasquez
// Copyright (c) 2025, Intercode Consulting, Inc.

namespace Intercode.Toolbox.TemplateEngine;

/// <summary>
///   Provides extension methods for handling macros within the template engine.
/// </summary>
/// <remarks>
///   This static class contains utility methods for determining the characteristics of characters
///   used in macro names and delimiters within the template engine.
/// </remarks>
internal static class MacroExtensions
{
  #region Public Methods

  /// <summary>
  ///   Determines whether the specified character is valid for use in a macro name.
  /// </summary>
  /// <param name="c">The character to evaluate.</param>
  /// <returns>
  ///   <c>true</c> if the character is a letter, digit, underscore ('_'), or dash ('-'); otherwise, <c>false</c>.
  /// </returns>
  public static bool IsMacroNameChar(
    this char c )
  {
    return char.IsLetterOrDigit( c ) || c is '_' or '-';
  }

  /// <summary>
  ///   Determines whether the specified character is a valid delimiter character.
  /// </summary>
  /// <param name="c">The character to evaluate.</param>
  /// <returns>
  ///   <c>true</c> if the character is not valid for use in a macro name (i.e., not a letter, digit, underscore ('_'), or
  ///   dash ('-'));
  ///   otherwise, <c>false</c>.
  /// </returns>
  public static bool IsDelimiterChar(
    this char c )
  {
    return !c.IsMacroNameChar() && !char.IsWhiteSpace( c );
  }

  #endregion
}
