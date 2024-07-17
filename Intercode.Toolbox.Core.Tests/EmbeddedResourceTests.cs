// Module Name: EmbeddedResourceTests.cs
// Author:      Eduardo Velasquez
// Copyright (c) 2024, Intercode Consulting, Inc.

namespace Intercode.Toolbox.Core.Tests;

using FluentAssertions;
using Intercode.Toolbox.Core.IO;

public class EmbeddedResourceTests
{
  #region Tests

  [Fact]
  public void LoadBytesFromResource_ShouldSucceed_WhenResourceExists()
  {
    var expected = "Test resource"u8.ToArray();

    var actual = EmbeddedResource.LoadBytesFromResource( "TestResource.txt" );

    actual.Should()
          .BeEquivalentTo( expected );
  }

  [Fact]
  public void LoadBytesFromResource_ShouldThrow_WhenFileNameIsEmpty()
  {
    var act = () => EmbeddedResource.LoadBytesFromResource( string.Empty );

    act.Should()
       .Throw<ArgumentException>()
       .WithMessage( "The value cannot be an empty string. (Parameter 'fileName')" );
  }

  [Fact]
  public void LoadBytesFromResource_ShouldThrow_WhenFileNameIsNull()
  {
    var act = () => EmbeddedResource.LoadBytesFromResource( null! );

    act.Should()
       .Throw<ArgumentException>()
       .WithMessage( "Value cannot be null. (Parameter 'fileName')" );
  }

  [Fact]
  public void LoadBytesFromResource_ShouldThrow_WhenResourceDoesNotExist()
  {
    var act = () => EmbeddedResource.LoadBytesFromResource( "NonExistentResource.txt" );

    act.Should()
       .Throw<FileNotFoundException>()
       .WithMessage( "The file NonExistentResource.txt does not exist." );
  }

  [Fact]
  public void LoadFromResource_ShouldSucceed_WhenResourceExists()
  {
    const string Expected = "Test resource";

    var (content, contentType) = EmbeddedResource.LoadFromResource( "TestResource.txt" );

    contentType.Should()
               .Be( "text/plain" );

    content.Should()
           .NotBeNull();

    using var reader = new StreamReader( content );
    var actual = reader.ReadToEnd();

    actual.Should()
          .BeEquivalentTo( Expected );
  }

  [Fact]
  public void LoadFromResource_ShouldThrow_WhenFileNameIsEmpty()
  {
    var act = () => EmbeddedResource.LoadFromResource( string.Empty );

    act.Should()
       .Throw<ArgumentException>()
       .WithMessage( "The value cannot be an empty string. (Parameter 'fileName')" );
  }

  [Fact]
  public void LoadFromResource_ShouldThrow_WhenFileNameIsNull()
  {
    var act = () => EmbeddedResource.LoadFromResource( null! );

    act.Should()
       .Throw<ArgumentException>()
       .WithMessage( "Value cannot be null. (Parameter 'fileName')" );
  }

  [Fact]
  public void LoadFromResource_ShouldThrow_WhenResourceDoesNotExist()
  {
    var act = () => EmbeddedResource.LoadFromResource( "NonExistentResource.txt" );

    act.Should()
       .Throw<FileNotFoundException>()
       .WithMessage( "The file NonExistentResource.txt does not exist." );
  }

  #endregion
}
