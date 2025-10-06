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

  /// <summary>
  ///   Determines whether the specified string is a valid macro name.
  /// </summary>
  /// <param name="macroName">The string to evaluate as a macro name.</param>
  /// <returns>
  ///   <c>true</c> if the macro name is not null or empty and all characters in the span are valid macro name characters
  ///   (letters, digits, underscores ('_'), or dashes ('-')); otherwise, <c>false</c>.
  /// </returns>
  public static bool IsValidMacroName(
    this string macroName )
  {
    return macroName.AsSpan().IsValidMacroName();
  }

  /// <summary>
  ///   Determines whether the specified <see cref="ReadOnlySpan{T}" /> of characters represents a valid macro name.
  /// </summary>
  /// <param name="macroName">The span of characters to evaluate as a macro name.</param>
  /// <returns>
  ///   <c>true</c> if the macro name is not empty and all characters in the span are valid macro name characters (letters,
  ///   digits, underscores ('_'), or dashes ('-')); otherwise, <c>false</c>.
  /// </returns>
  public static bool IsValidMacroName(
    this ReadOnlySpan<char> macroName )
  {
    if( macroName.IsEmpty )
    {
      return false;
    }

    // NOTE: Use loop instead of LINQ for performance
    for( var index = 0; index < macroName.Length; index++ )
    {
      var c = macroName[index];

      if( !c.IsMacroNameChar() )
      {
        return false;
      }
    }

    return true;
  }

  /// <summary>
  ///   Validates the specified macro name to ensure it meets the requirements for a valid macro name.
  /// </summary>
  /// <param name="macroName">The macro name to validate.</param>
  /// <exception cref="ArgumentException">
  ///   Thrown if the <paramref name="macroName" /> is null, empty, or contains invalid characters.
  ///   Valid macro names must consist of letters, digits, underscores ('_'), or dashes ('-').
  /// </exception>
  public static void ValidateMacroName(
    string macroName )
  {
    ValidateMacroName( macroName.AsSpan() );
  }

  /// <summary>
  ///   Validates the specified macro name to ensure it adheres to the rules for valid macro names.
  /// </summary>
  /// <param name="macroName">The span of characters representing the macro name to validate.</param>
  /// <exception cref="ArgumentException">
  ///   Thrown when the macro name is null, empty, or contains invalid characters.
  ///   A valid macro name must consist only of alphanumeric characters, underscores ('_'), or dashes ('-').
  /// </exception>
  public static void ValidateMacroName(
    ReadOnlySpan<char> macroName )
  {
    if( !macroName.IsValidMacroName() )
    {
      throw new ArgumentException(
        "The macro name cannot be null or empty and must only contain alphanumeric, underscore, or dash characters",
        nameof( macroName )
      );
    }
  }

  #endregion
}
