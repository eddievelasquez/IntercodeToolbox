// Module Name: StreamExtensions.cs
// Author:      Eduardo Velasquez
// Copyright (c) 2024, Intercode Consulting, Inc.

namespace Intercode.Toolbox.Core.IO;

using System.Buffers;
using System.Text;

/// <summary>
///   Provides extension methods to read and write values to a <see cref="Stream" />.
/// </summary>
public static class StreamExtensions
{
  #region Constants

  private const byte NULL_STRING_SIZE = 0;
  private const byte EMPTY_STRING_SIZE = NULL_STRING_SIZE + 1;

  #endregion

  #region Public Methods

  /// <summary>
  ///   Converts a stream to a byte array.
  /// </summary>
  /// <param name="stream">The stream to convert.</param>
  /// <param name="bufferSize">
  ///   The buffer size to use for copying the stream. If not specified, the default buffer size will
  ///   be used.
  /// </param>
  /// <returns>The byte array representation of the stream.</returns>
  public static byte[] ToByteArray(
    this Stream stream,
    int? bufferSize = null )
  {
    ArgumentNullException.ThrowIfNull( stream );

    if( stream is MemoryStream ms )
    {
      return ms.ToArray();
    }

    using var destination = new MemoryStream();

    if( bufferSize != null )
    {
      stream.CopyTo( destination, ( int ) bufferSize );
    }
    else
    {
      stream.CopyTo( destination );
    }

    return destination.ToArray();
  }

  /// <summary>
  ///   Asynchronously converts a stream to a byte array.
  /// </summary>
  /// <param name="stream">The stream to convert.</param>
  /// <param name="bufferSize">
  ///   The buffer size to use for copying the stream. If not specified, the default buffer size will
  ///   be used.
  /// </param>
  /// <param name="cancellationToken">The cancellation token.</param>
  /// <returns>
  ///   A task representing the asynchronous operation. The task result contains the byte array representation of the
  ///   stream.
  /// </returns>
  public static async ValueTask<byte[]> ToByteArrayAsync(
    this Stream stream,
    int? bufferSize = null,
    CancellationToken cancellationToken = default )
  {
    ArgumentNullException.ThrowIfNull( stream );
    return await ToByteArrayAsyncCore( stream, bufferSize, cancellationToken );

    static async ValueTask<byte[]> ToByteArrayAsyncCore(
      Stream stream,
      int? bufferSize,
      CancellationToken cancellationToken )
    {
      if( stream is MemoryStream ms )
      {
        return ms.ToArray();
      }

      using var destination = new MemoryStream();

      if( bufferSize != null )
      {
        await stream.CopyToAsync( destination, ( int ) bufferSize, cancellationToken )
                    .ConfigureAwait( false );
      }
      else
      {
        await stream.CopyToAsync( destination, cancellationToken )
                    .ConfigureAwait( false );
      }

      return destination.ToArray();
    }
  }

  /// <summary>
  ///   Writes a size-prefixed (1 byte) short string to the stream.
  /// </summary>
  /// <param name="destination">The stream to write to.</param>
  /// <param name="value">The string value to write. If null, a null string will be written.</param>
  /// <param name="cancellationToken">The cancellation token.</param>
  /// <param name="encoding">
  ///   The encoding to use for converting the string to bytes. If not specified, UTF-8 encoding will be
  ///   used.
  /// </param>
  /// <returns>A task representing the asynchronous operation.</returns>
  public static async ValueTask WritePrefixedStringAsync(
    this Stream destination,
    string? value,
    CancellationToken cancellationToken = default,
    Encoding? encoding = null )
  {
    if( value is null )
    {
      destination.WriteByte( NULL_STRING_SIZE );
      return;
    }

    if( value.Length == 0 )
    {
      destination.WriteByte( EMPTY_STRING_SIZE );
      return;
    }

    var valueBytes = ( encoding ?? Encoding.UTF8 ).GetBytes( value );

    // Zero is reserved for null strings, so we add 1 to the length;
    // this means that the length of an empty string is 1 and so on.
    var length = valueBytes.Length + 1;

    // Write the length of the string
    VariableIntegerEncoder.WriteEncoded( destination, length );

    // Skip write for empty body
    if( length > EMPTY_STRING_SIZE )
    {
      await destination.WriteAsync( valueBytes, cancellationToken );
    }
  }

  /// <summary>
  ///   Reads a size-prefixed short string from the stream.
  /// </summary>
  /// <param name="source">The stream to read from.</param>
  /// <param name="encoding">
  ///   The encoding to use for converting the bytes to a string. If not specified, UTF-8 encoding will
  ///   be used.
  /// </param>
  /// <param name="cancellationToken">The cancellation token.</param>
  /// <returns>A task representing the asynchronous operation. The task result contains the read string value.</returns>
  public static async ValueTask<string?> ReadPrefixedStringAsync(
    this Stream source,
    Encoding? encoding = null,
    CancellationToken cancellationToken = default )
  {
    try
    {
      var length = VariableIntegerEncoder.ReadEncoded( source );
      if( length == NULL_STRING_SIZE )
      {
        return null;
      }

      if( length == EMPTY_STRING_SIZE )
      {
        return string.Empty;
      }

      --length; // Subtract 1 to get the actual length of the string

      var bytes = ArrayPool<byte>.Shared.Rent( length );

      try
      {
        await source.ReadExactlyAsync( bytes, 0, length, cancellationToken );
        var result = ( encoding ?? Encoding.UTF8 ).GetString( bytes, 0, length );
        return result;
      }
      finally
      {
        ArrayPool<byte>.Shared.Return( bytes );
      }
    }
    catch( Exception exception )
    {
      throw new InvalidDataException( "Corrupt data", exception );
    }
  }

  #endregion
}
