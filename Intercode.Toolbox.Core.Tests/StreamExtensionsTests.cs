// Module Name: StreamExtensionsTests.cs
// Author:      Eduardo Velasquez
// Copyright (c) 2024, Intercode Consulting, Inc.

namespace Intercode.Toolbox.Core.Tests;

using FluentAssertions;
using Intercode.Toolbox.Core.IO;

public class StreamExtensionsTests
{
  #region Tests

  [Fact]
  public void ToByteArray_ShouldSucceed_WhenStreamIsMemoryStream()
  {
    var expected = "Hello, World!"u8.ToArray();

    using var stream = new MemoryStream( expected );
    var actual = stream.ToByteArray();

    actual.Should()
          .BeEquivalentTo( expected );
  }

  [Fact]
  public void ToByteArray_ShouldSucceed_WhenStreamIsNotMemoryStream()
  {
    var expected = "Hello, World!"u8.ToArray();

    using var stream = new TestStream( expected );

    var actual = stream.ToByteArray();

    actual.Should()
          .BeEquivalentTo( expected );

    stream.CopyBufferSize.Should()
          .NotBeNull();

    stream.CopyBufferSize.Should()
          .BeGreaterThan( 0 );
  }

  [Fact]
  public void ToByteArray_ShouldThrow_WhenStreamIsNull()
  {
    Stream stream = null!;

    Action act = () => stream.ToByteArray();

    act.Should()
       .Throw<ArgumentNullException>();
  }

  [Fact]
  public void ToByteArray_ShouldUseBufferSize_WhenProvided()
  {
    var expected = "Hello, World!"u8.ToArray();
    const int BufferSize = 100;

    using var stream = new TestStream( expected );

    var actual = stream.ToByteArray( BufferSize );

    actual.Should()
          .BeEquivalentTo( expected );

    stream.CopyBufferSize.Should()
          .NotBeNull();

    stream.CopyBufferSize.Should()
          .Be( BufferSize );
  }

  [Fact]
  public async Task ToByteArrayAsync_ShouldSucceed_WhenStreamIsMemoryStream()
  {
    var expected = "Hello, World!"u8.ToArray();

    await using var stream = new MemoryStream( expected );
    var actual = await stream.ToByteArrayAsync();

    actual.Should()
          .BeEquivalentTo( expected );
  }

  [Fact]
  public async Task ToByteArrayAsync_ShouldSucceed_WhenStreamIsNotMemoryStream()
  {
    var expected = "Hello, World!"u8.ToArray();

    await using var stream = new TestStream( expected );

    var actual = await stream.ToByteArrayAsync();

    actual.Should()
          .BeEquivalentTo( expected );

    stream.CopyBufferSize.Should()
          .NotBeNull();

    stream.CopyBufferSize.Should()
          .BeGreaterThan( 0 );
  }

  [Fact]
  public async Task ToByteArrayAsync_ShouldThrow_WhenStreamIsNull()
  {
    Stream stream = null!;

    var act = async () => await stream.ToByteArrayAsync();

    await act.Should()
             .ThrowAsync<ArgumentNullException>();
  }

  [Fact]
  public async Task ToByteArrayAsync_ShouldUseBufferSize_WhenProvided()
  {
    var expected = "Hello, World!"u8.ToArray();
    const int BufferSize = 100;

    await using var stream = new TestStream( expected );

    var actual = await stream.ToByteArrayAsync( BufferSize );

    actual.Should()
          .BeEquivalentTo( expected );

    stream.CopyBufferSize.Should()
          .NotBeNull();

    stream.CopyBufferSize.Should()
          .Be( BufferSize );
  }

  [Fact]
  public async Task WriteReadPrefixedStringAsync_ShouldRoundTrip()
  {
    const string Expected = "Hello, World!";

    await using var stream = new MemoryStream();
    await stream.WritePrefixedStringAsync( Expected );

    stream.Position = 0;
    var actual = await stream.ReadPrefixedStringAsync();

    actual.Should()
          .Be( Expected );
  }

  [Fact]
  public async Task WriteReadPrefixedStringAsync_ShouldRoundTrip_WhenValueIsEmpty()
  {
    const string? Expected = "";

    await using var stream = new MemoryStream();
    await stream.WritePrefixedStringAsync( Expected );

    stream.Position = 0;
    var actual = await stream.ReadPrefixedStringAsync();

    actual.Should()
          .Be( Expected );
  }

  [Fact]
  public async Task WriteReadPrefixedStringAsync_ShouldRoundTrip_WhenValueIsLongString()
  {
    var expected = new string( 'A', 1024 );

    await using var stream = new MemoryStream();
    await stream.WritePrefixedStringAsync( expected );

    stream.Position = 0;
    var actual = await stream.ReadPrefixedStringAsync();

    actual.Should()
          .Be( expected );
  }

  [Fact]
  public async Task WriteReadPrefixedStringAsync_ShouldRoundTrip_WhenValueIsNull()
  {
    const string? Expected = null;

    await using var stream = new MemoryStream();
    await stream.WritePrefixedStringAsync( Expected );

    stream.Position = 0;
    var actual = await stream.ReadPrefixedStringAsync();

    actual.Should()
          .Be( Expected );
  }

  #endregion

  #region Implementation

  private class TestStream: Stream
  {
    #region Fields

    private readonly MemoryStream _stream;

    #endregion

    #region Constructors

    public TestStream()
    {
      _stream = new MemoryStream();
    }

    public TestStream(
      byte[] data )
    {
      _stream = new MemoryStream( data );
    }

    #endregion

    #region Properties

    public override bool CanRead => _stream.CanRead;
    public override bool CanSeek => _stream.CanSeek;
    public override bool CanWrite => _stream.CanWrite;
    public override long Length => _stream.Length;

    public override long Position
    {
      get => _stream.Position;
      set => _stream.Position = value;
    }

    public int? CopyBufferSize { get; private set; }

    #endregion

    #region Public Methods

    public override void CopyTo(
      Stream destination,
      int bufferSize )
    {
      CopyBufferSize = bufferSize;
      base.CopyTo( destination, bufferSize );
    }

    public override async Task CopyToAsync(
      Stream destination,
      int bufferSize,
      CancellationToken cancellationToken )
    {
      CopyBufferSize = bufferSize;
      await base.CopyToAsync( destination, bufferSize, cancellationToken );
    }

    public override void Flush()
    {
      _stream.Flush();
    }

    public override int Read(
      byte[] buffer,
      int offset,
      int count )
    {
      return _stream.Read( buffer, offset, count );
    }

    public override long Seek(
      long offset,
      SeekOrigin origin )
    {
      return _stream.Seek( offset, origin );
    }

    public override void SetLength(
      long value )
    {
      _stream.SetLength( value );
    }

    public override void Write(
      byte[] buffer,
      int offset,
      int count )
    {
      _stream.Write( buffer, offset, count );
    }

    #endregion

    #region Implementation

    protected override void Dispose(
      bool disposing )
    {
      _stream.Dispose();
      base.Dispose( disposing );
    }

    #endregion
  }

  #endregion
}
