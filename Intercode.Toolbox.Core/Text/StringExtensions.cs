// Module Name: StringExtensions.cs
// Author:      Eduardo Velasquez
// Copyright (c) 2024, Intercode Consulting, Inc.

namespace Intercode.Toolbox.Core.Text;

using System.Buffers;
using System.Globalization;
using System.Text;

/// <summary>
///   Provides extension methods for <see cref="string" /> objects.
/// </summary>
public static class StringExtensions
{
  #region Public Methods

  /// <summary>
  ///   Removes diacritics from a string.
  /// </summary>
  /// <param name="text">The string to process.</param>
  /// <returns>The string without diacritics.</returns>
  public static string? RemoveDiacritics(
    this string? text )
  {
    if( string.IsNullOrEmpty( text ) )
    {
      return text;
    }

    // Normalize the input string using the 'D' form of Unicode normalization
    // (full canonical decomposition)
    var normalized = text.Normalize( NormalizationForm.FormD )
                         .AsSpan();

    // Rent a buffer from the shared pool if the input string is larger than the stack limit
    const int StackLimit = 1024;
    var rentedBuffer = text.Length > StackLimit ? ArrayPool<char>.Shared.Rent( text.Length ) : null;
    var dest = rentedBuffer != null ? rentedBuffer.AsSpan() : stackalloc char[text.Length];

    try
    {
      var i = 0;

      // Iterate over each rune in the normalized string
      foreach( var rune in normalized.EnumerateRunes() )
      {
        // Determine the Unicode category for the current rune
        var category = Rune.GetUnicodeCategory( rune );

        // If the current rune is not a non-spacing mark, append it to the StringBuilder
        if( category != UnicodeCategory.NonSpacingMark )
        {
          dest[i++] = ( char ) rune.Value;
        }
      }

      return new string( dest ).Normalize( NormalizationForm.FormC );
    }
    finally
    {
      if( rentedBuffer != null )
      {
        // Return the rented buffer to the shared pool
        ArrayPool<char>.Shared.Return( rentedBuffer );
      }
    }
  }

  #endregion
}
