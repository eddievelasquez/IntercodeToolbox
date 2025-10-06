// Module Name: UriBuilderExtensionsTests.cs
// Author:      Eduardo Velasquez
// Copyright (c) 2024, Intercode Consulting, Inc.

namespace Intercode.Toolbox.Core.Tests;

using FluentAssertions;

public class UriBuilderExtensionsTests
{
  #region Constants

  private const string DEFAULT_URI = "http://example.com:80";

  #endregion

  #region Tests

  [Fact]
  public void AppendPath_ShouldAppend()
  {
    var builder = new UriBuilder( DEFAULT_URI );

    var result = builder.AppendPath( "api" )
                        .ToString();

    result.Should()
          .Be( "http://example.com:80/api" );
  }

  [Fact]
  public void AppendPath_ShouldAppend_WhenUriHasPath()
  {
    var builder = new UriBuilder( DEFAULT_URI );

    var result = builder.AppendPath( "api" )
                        .AppendPath( "v1" )
                        .ToString();

    result.Should()
          .Be( "http://example.com:80/api/v1" );
  }

  [Fact]
  public void AppendPath_ShouldReturnUnchanged_WhenPathIsEmpty()
  {
    var builder = new UriBuilder( DEFAULT_URI );

    var result = builder.AppendPath( "" )
                        .ToString();

    result.Should()
          .Be( "http://example.com:80/" );
  }

  [Fact]
  public void AppendPath_ShouldReturnUnchanged_WhenPathIsNull()
  {
    var builder = new UriBuilder( DEFAULT_URI );

    var result = builder.AppendPath( null )
                        .ToString();

    result.Should()
          .Be( "http://example.com:80/" );
  }

  [Fact]
  public void AppendPath_ShouldThrow_WhenBuilderIsNull()
  {
    UriBuilder builder = null!;

    var act = () => builder.AppendPath( "x" );

    act.Should()
       .Throw<ArgumentNullException>();
  }

  [Fact]
  public void AppendPath_ShouldUrlEncode_WhenPathHasSpecialChars()
  {
    var builder = new UriBuilder( DEFAULT_URI );

    var result = builder.AppendPath( "<api>" )
                        .ToString();

    result.Should()
          .Be( "http://example.com:80/%3Capi%3E" );
  }

  [Fact]
  public void AppendQuery_WithKey_ShouldAppend()
  {
    var builder = new UriBuilder( DEFAULT_URI );

    var result = builder.AppendQuery( "key" )
                        .ToString();

    result.Should()
          .Be( "http://example.com:80/?key" );
  }

  [Fact]
  public void AppendQuery_WithKey_ShouldAppend_WhenUriHasQuery()
  {
    var builder = new UriBuilder( DEFAULT_URI );

    var result = builder.AppendQuery( "key" )
                        .AppendQuery( "key2" )
                        .ToString();

    result.Should()
          .Be( "http://example.com:80/?key&key2" );
  }

  [Fact]
  public void AppendQuery_WithKey_ShouldReturnUnchanged_WhenKeyIsEmpty()
  {
    var builder = new UriBuilder( DEFAULT_URI );

    var result = builder.AppendQuery( "" )
                        .ToString();

    result.Should()
          .Be( "http://example.com:80/" );
  }

  [Fact]
  public void AppendQuery_WithKey_ShouldReturnUnchanged_WhenKeyIsNull()
  {
    var builder = new UriBuilder( DEFAULT_URI );

    var result = builder.AppendQuery( null )
                        .ToString();

    result.Should()
          .Be( "http://example.com:80/" );
  }

  [Fact]
  public void AppendQuery_WithKey_ShouldThrow_WhenBuilderIsNull()
  {
    UriBuilder builder = null!;

    var act = () => builder.AppendQuery( "x" );

    act.Should()
       .Throw<ArgumentNullException>();
  }

  [Fact]
  public void AppendQuery_WithKey_ShouldUrlEncode_WhenKeyChasSpecialChars()
  {
    var builder = new UriBuilder( DEFAULT_URI );

    var result = builder.AppendQuery( "<key>" )
                        .ToString();

    result.Should()
          .Be( "http://example.com:80/?%3Ckey%3E" );
  }

  [Fact]
  public void AppendQuery_WithKeyAndValue_ShouldAppend()
  {
    var builder = new UriBuilder( DEFAULT_URI );

    var result = builder.AppendQuery( "key", "value" )
                        .ToString();

    result.Should()
          .Be( "http://example.com:80/?key=value" );
  }

  [Fact]
  public void AppendQuery_WithKeyAndValue_ShouldAppend_WhenUriHasQuery()
  {
    var builder = new UriBuilder( DEFAULT_URI );

    var result = builder.AppendQuery( "key", "value" )
                        .AppendQuery( "key2", "value2" )
                        .ToString();

    result.Should()
          .Be( "http://example.com:80/?key=value&key2=value2" );
  }

  [Fact]
  public void AppendQuery_WithKeyAndValue_ShouldAppend_WhenValueIsEmpty()
  {
    var builder = new UriBuilder( DEFAULT_URI );

    var result = builder.AppendQuery( "key", "" )
                        .ToString();

    result.Should()
          .Be( "http://example.com:80/?key=" );
  }

  [Fact]
  public void AppendQuery_WithKeyAndValue_ShouldAppend_WhenValueIsNull()
  {
    var builder = new UriBuilder( DEFAULT_URI );

    var result = builder.AppendQuery( "key", ( string? ) null )
                        .ToString();

    result.Should()
          .Be( "http://example.com:80/?key=" );
  }

  [Fact]
  public void AppendQuery_WithKeyAndValue_ShouldReturnUnchanged_WhenKeyIsEmpty()
  {
    var builder = new UriBuilder( DEFAULT_URI );

    var result = builder.AppendQuery( "", "value" )
                        .ToString();

    result.Should()
          .Be( "http://example.com:80/" );
  }

  [Fact]
  public void AppendQuery_WithKeyAndValue_ShouldReturnUnchanged_WhenKeyIsNull()
  {
    var builder = new UriBuilder( DEFAULT_URI );

    var result = builder.AppendQuery( null, "value" )
                        .ToString();

    result.Should()
          .Be( "http://example.com:80/" );
  }

  [Fact]
  public void AppendQuery_WithKeyAndValue_ShouldThrow_WhenBuilderIsNull()
  {
    UriBuilder builder = null!;

    var act = () => builder.AppendQuery( "key", "value" );

    act.Should()
       .Throw<ArgumentNullException>();
  }

  [Fact]
  public void AppendQuery_WithKeyAndValue_ShouldUrlEncode_WhenKeyHasSpecialChars()
  {
    var builder = new UriBuilder( DEFAULT_URI );

    var result = builder.AppendQuery( "<key>", "value" )
                        .ToString();

    result.Should()
          .Be( "http://example.com:80/?%3Ckey%3E=value" );
  }

  [Fact]
  public void AppendQuery_WithKeyAndValue_ShouldUrlEncode_WhenValueHasSpecialChars()
  {
    var builder = new UriBuilder( DEFAULT_URI );

    var result = builder.AppendQuery( "key", "<value>" )
                        .ToString();

    result.Should()
          .Be( "http://example.com:80/?key=%3Cvalue%3E" );
  }

  #endregion
}
