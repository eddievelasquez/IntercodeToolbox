// Module Name: BaseConverter.cs
// Author:      Eduardo Velasquez
// Copyright (c) 2024, Intercode Consulting, Inc.

namespace Intercode.Toolbox.Core;

using System.Numerics;

/// <summary>
///   Provides methods for converting numbers between different number bases.
///   It supports bases from 2 to 62, unlike the built-in <see cref="System.Convert.ToString(int, int)" /> method,
///   which only supports the 2, 8, 10 or 16 bases.
/// </summary>
public static class BaseConverter
{
  #region Constants

  private const string DIGITS = "0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";

  /// <summary>
  ///   The minimum radix value allowed.
  /// </summary>
  public static readonly int MinRadix = 2;

  /// <summary>
  ///   The maximum radix value allowed.
  /// </summary>
  public static readonly int MaxRadix = DIGITS.Length;

  #endregion

  #region Public Methods

  /// <summary>
  ///   Converts the specified value to a string representation in the given radix.
  /// </summary>
  /// <typeparam name="T">The type of the value to convert.</typeparam>
  /// <param name="value">The value to convert.</param>
  /// <param name="radix">The radix to use for the conversion.</param>
  /// <returns>The string representation of the value in the specified radix.</returns>
  /// <exception cref="ArgumentOutOfRangeException">Thrown when the radix is outside the valid range.</exception>
  public static string ConvertToString<T>(
    T value,
    int radix )
    where T: IBinaryInteger<T>, IMinMaxValue<T>
  {
    ValidateRadix( radix );

    if( value == T.Zero )
    {
      return "0";
    }

    var maxDigits = ( value.GetByteCount() * 8 ) + 1;

    // Only use Abs() if T is a signed number type
    var currentNumber = T.MinValue < T.Zero ? T.Abs( value ) : value;
    var index = maxDigits - 1;
    var digitsUsed = 0;
    var radixAsT = T.CreateChecked( radix );

    Span<char> buffer = stackalloc char[maxDigits];
    while( currentNumber != T.Zero )
    {
      ( currentNumber, var remainder ) = T.DivRem( currentNumber, radixAsT );
      buffer[index--] = DIGITS[int.CreateChecked( remainder )];
      digitsUsed++;
    }

    if( value < T.Zero )
    {
      buffer[index] = '-';
      digitsUsed++;
    }

    var result = buffer[^digitsUsed..]
      .ToString();

    return result;
  }

  /// <summary>
  ///   Converts the specified string representation to a value of the specified type in the given radix.
  /// </summary>
  /// <typeparam name="T">The type of the value to convert.</typeparam>
  /// <param name="value">The string representation of the value.</param>
  /// <param name="radix">The radix used in the string representation.</param>
  /// <returns>The value represented by the string in the specified radix.</returns>
  /// <exception cref="ArgumentOutOfRangeException">Thrown when the radix is outside the valid range.</exception>
  /// <exception cref="FormatException">Thrown when the string contains an invalid character.</exception>
  public static T ConvertFromString<T>(
    string? value,
    int radix )
    where T: IBinaryInteger<T>
  {
    ValidateRadix( radix );

    if( string.IsNullOrEmpty( value ) )
    {
      return T.Zero;
    }

    if( radix <= 36 )
    {
      value = value.ToLowerInvariant();
    }

    var result = T.Zero;
    var multiplier = T.One;
    var radixAsT = T.CreateChecked( radix );

    for( var i = value.Length - 1; i >= 0; i-- )
    {
      var c = value[i];
      if( i == 0 && c == '-' )
      {
        result = -result;
        break;
      }

      var digit = DIGITS.IndexOf( c );
      if( digit == -1 )
      {
        throw new FormatException( $"Invalid character '{c}'" );
      }

      result += T.CreateChecked( digit ) * multiplier;
      multiplier *= radixAsT;
    }

    return result;
  }

  /// <summary>
  ///   Tries to convert the specified string representation to a value of the specified type in the given radix.
  /// </summary>
  /// <typeparam name="T">The type of the value to convert.</typeparam>
  /// <param name="value">The string representation of the value.</param>
  /// <param name="radix">The radix used in the string representation.</param>
  /// <param name="result">
  ///   When this method returns, contains the value represented by the string in the specified radix, if
  ///   the conversion succeeded, or zero if the conversion failed.
  /// </param>
  /// <returns><c>true</c> if the conversion succeeded; otherwise, <c>false</c>.</returns>
  /// <exception cref="ArgumentOutOfRangeException">Thrown when the radix is outside the valid range.</exception>
  public static bool TryConvertFromString<T>(
    string? value,
    int radix,
    out T result )
    where T: IBinaryInteger<T>
  {
    result = T.Zero;

    if( radix < 2 || radix > DIGITS.Length )
    {
      return false;
    }

    if( string.IsNullOrEmpty( value ) )
    {
      return true;
    }

    if( radix <= 36 )
    {
      value = value.ToLowerInvariant();
    }

    var multiplier = T.One;
    var radixAsT = T.CreateChecked( radix );

    for( var i = value.Length - 1; i >= 0; i-- )
    {
      var c = value[i];
      if( i == 0 && c == '-' )
      {
        result = -result;
        break;
      }

      var digit = DIGITS.IndexOf( c );
      if( digit == -1 )
      {
        result = T.Zero;
        return false;
      }

      result += T.CreateChecked( digit ) * multiplier;
      multiplier *= radixAsT;
    }

    return true;
  }

  #endregion

  #region Implementation

  private static void ValidateRadix(
    int radix )
  {
    if( radix < MinRadix || radix > MaxRadix )
    {
      throw new ArgumentOutOfRangeException(
        nameof( radix ),
        radix,
        $"The radix must be between {MinRadix} and {MaxRadix}"
      );
    }
  }

  #endregion
}
