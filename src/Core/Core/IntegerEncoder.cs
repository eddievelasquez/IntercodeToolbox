// Module Name: IntegerEncoder.cs
// Author:      Eduardo Velasquez
// Copyright (c) 2024, Intercode Consulting, Inc.

namespace Intercode.Toolbox.Core;

using System.Diagnostics;

/// <summary>
///   Using multiplicative inverses to obfuscate the user ids and avoid exposing sequential
///   values that could potentially be exploited. The algorithm is not cryptographically strong,
///   but is very fast and cheap.
///   Taken from:
///   https://ericlippert.com/2013/11/14/a-practical-use-of-multiplicative-inverses/
/// </summary>
public static class IntegerEncoder
{
  #region Constants

  /// <summary>
  ///   The maximum value that can be encoded.
  /// </summary>
  public const int MaxValue = MODULUS - 1;

  /// <summary>
  ///   The minimum value that can be encoded.
  /// </summary>
  public const int MinValue = -MaxValue;

  private const int MODULUS = 1_000_000_000;
  private const ulong COPRIME = 387_420_489ul;
  private const ulong INVERSE = 513_180_409ul;

  #endregion

  #region Public Methods

  /// <summary>
  ///   Encodes an integer value using a multiplicative inverse algorithm.
  /// </summary>
  /// <param name="value">The integer value to encode.</param>
  /// <returns>The encoded integer value.</returns>
  public static int Encode(
    int value )
  {
    if( value is < -MaxValue or > MaxValue )
    {
      throw new ArgumentOutOfRangeException(
        nameof( value ),
        value,
        $"The identifier must be in the [{-MaxValue}, {MaxValue}) range"
      );
    }

    var encoded = ( ulong ) Math.Abs( value ) * COPRIME % MODULUS;
    encoded <<= 1;

    if( value < 0 )
    {
      encoded |= 1;
    }

    Debug.Assert( ( long ) encoded >= int.MinValue && encoded <= int.MaxValue );
    return ( int ) encoded;
  }

  /// <summary>
  ///   Decodes an encoded integer value using a multiplicative inverse algorithm.
  /// </summary>
  /// <param name="encoded">The encoded integer value to decode.</param>
  /// <returns>The decoded integer value.</returns>
  public static int Decode(
    int encoded )
  {
    var decoded = ( int ) ( ( ( ulong ) encoded >> 1 ) * INVERSE % MODULUS );
    if( ( encoded & 1 ) == 1 )
    {
      decoded = -decoded;
    }

    return decoded;
  }

  #endregion
}
