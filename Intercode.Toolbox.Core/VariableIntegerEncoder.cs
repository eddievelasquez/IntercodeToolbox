// Module Name: VariableIntegerEncoder.cs
// Author:      Eduardo Velasquez
// Copyright (c) 2024, Intercode Consulting, Inc.

namespace Intercode.Toolbox.Core;

using System.Diagnostics;
using System.Runtime.InteropServices;

/// <summary>
///   Provides methods for encoding and decoding integers using the Variable Length Quantity (VLQ) encoding scheme.
///   This method is used in several protocols to encode integers in a compact form, such as MIDI and Google Protocol
///   Buffers.
/// </summary>
public static class VariableIntegerEncoder
{
  #region Constants

  // The maximum size in bytes of the buffer required for encoding an 32-bit integer
  private static readonly int s_bufferSize = Marshal.SizeOf<int>() + 1;

  #endregion

  #region Public Methods

  /// <summary>
  ///   Tries to encode the specified integer value into a span of bytes.
  /// </summary>
  /// <param name="buffer">The span of bytes to store the encoded value.</param>
  /// <param name="value">The integer value to encode.</param>
  /// <param name="bytesWritten">The number of bytes written to the buffer.</param>
  /// <returns><c>true</c> if the encoding succeeds; otherwise, <c>false</c>.</returns>
  /// <remarks>
  ///   Encoding will only fail if the provided buffer is too small for the encoded value.
  /// </remarks>
  public static bool TryEncode(
    Span<byte> buffer,
    int value,
    out int bytesWritten )
  {
    // Convert to unsigned int to handle the sign bit correctly
    var v = ( uint ) value;
    var pos = 0;

    while( v >= 0x80 )
    {
      // Set the high bit to indicate more bytes to follow
      if( !AddByte( buffer, ref pos, ( byte ) ( v | 0x80 ) ) )
      {
        bytesWritten = pos;
        return false;
      }

      v >>= 7; // Process the next 7 bits
    }

    // Add the final byte
    if( !AddByte( buffer, ref pos, ( byte ) v ) )
    {
      bytesWritten = pos;
      return false;
    }

    bytesWritten = pos;
    return true;

    static bool AddByte(
      Span<byte> buffer,
      ref int pos,
      byte b )
    {
      if( pos >= buffer.Length )
      {
        return false;
      }

      buffer[pos++] = b;
      return true;
    }
  }

  /// <summary>
  ///   Encodes the specified integer value into a <see cref="Stream" />.
  /// </summary>
  /// <param name="stream">The stream to write the encoded value to.</param>
  /// <param name="value">The integer value to encode.</param>
  /// <returns>The number of bytes written to the stream.</returns>
  public static int WriteEncoded(
    Stream stream,
    int value )
  {
    // Convert to unsigned int to handle the sign bit correctly
    var v = ( uint ) value;
    var currentPos = stream.Position;

    while( v >= 0x80 )
    {
      // Set the high bit to indicate more bytes to follow
      stream.WriteByte( ( byte ) ( v | 0x80 ) );
      v >>= 7; // Process the next 7 bits
    }

    // Add the final byte
    stream.WriteByte( ( byte ) v );

    var bytesWritten = stream.Position - currentPos;
    return ( int ) bytesWritten;
  }

  /// <summary>
  ///   Encodes the specified integer value into a byte array.
  /// </summary>
  /// <param name="value">The integer value to encode.</param>
  /// <returns>The encoded byte array.</returns>
  public static byte[] Encode(
    int value )
  {
    Span<byte> bytes = stackalloc byte[s_bufferSize];

    var succeeded = TryEncode( bytes, value, out var bytesWritten );
    Debug.Assert( succeeded ); // The buffer should never be too small here!

    return bytes[..bytesWritten]
      .ToArray();
  }

  /// <summary>
  ///   Decodes a span of bytes into an integer value.
  /// </summary>
  /// <param name="span">The span of bytes to decode.</param>
  /// <returns>The decoded integer value.</returns>
  public static int Decode(
    ReadOnlySpan<byte> span )
  {
    var value = 0;
    var shift = 0;

    foreach( var b in span )
    {
      value |= ( b & 0x7F ) << shift; // Add the lower 7 bits of the byte to the result
      if( ( b & 0x80 ) == 0 ) // If the high bit is not set, this is the last byte
      {
        break;
      }

      shift += 7; // Move to the next 7 bits
    }

    return value;
  }

  /// <summary>
  ///   Reads an encoded integer value from the specified <see cref="Stream" />.
  /// </summary>
  /// <param name="stream">The stream to read the encoded value from.</param>
  /// <returns>The decoded integer value.</returns>
  public static int ReadEncoded(
    Stream stream )
  {
    var value = 0;
    var shift = 0;

    while( true )
    {
      var b = stream.ReadByte();
      if( b == -1 )
      {
        throw new EndOfStreamException();
      }

      value |= ( b & 0x7F ) << shift; // Add the lower 7 bits of the byte to the result
      if( ( b & 0x80 ) == 0 ) // If the high bit is not set, this is the last byte
      {
        break;
      }

      shift += 7; // Move to the next 7 bits
    }

    return value;
  }

  #endregion
}
