// Module Name: StringBuilderPoolTests.cs
// Author:      Eduardo Velasquez
// Copyright (c) 2025, Intercode Consulting, Inc.

namespace Intercode.Toolbox.TemplateEngine.Tests;

using System.Text;
using FluentAssertions;

public class StringBuilderPoolTests
{
  #region Tests

  [Fact]
  public void Builder_ShouldBeClearedOnReturn()
  {
    var pool = new StringBuilderPool();
    var builder = new StringBuilder( "hello" );
    pool.Return( builder );
    builder.Length.Should().Be( 0 );
  }

  [Fact]
  public void Default_ShouldHaveExpectedMaxPoolSize()
  {
    StringBuilderPool.Default.MaxPoolSize.Should().Be( 100 );
  }

  [Fact]
  public void Get_ShouldReturnBuilderFromPool_WhenAvailable()
  {
    var pool = new StringBuilderPool( 128, 2 );
    var builder = new StringBuilder( "abc" );
    pool.Return( builder );
    var pooled = pool.Get();
    pooled.Should().BeSameAs( builder );
    pool.Size.Should().Be( 0 );
  }

  [Fact]
  public void Get_ShouldReturnNewStringBuilder_WhenPoolIsEmpty()
  {
    var pool = new StringBuilderPool( 256, 2 );
    var builder = pool.Get();
    builder.Should().NotBeNull();
    builder.Capacity.Should().BeGreaterThanOrEqualTo( 256 );
    pool.Size.Should().Be( 0 );
  }

  [Fact]
  public void Return_ShouldAddBuilderToPool_AndClearBuilder()
  {
    var pool = new StringBuilderPool( 128, 2 );
    var builder = new StringBuilder().Append( "test" );
    pool.Return( builder );
    pool.Size.Should().Be( 1 );
    var pooled = pool.Get();
    pooled.Length.Should().Be( 0 );
  }

  [Fact]
  public void Return_ShouldNotAddBuilder_WhenPoolIsFull()
  {
    var pool = new StringBuilderPool( 128, 1 );
    pool.Return( new StringBuilder( "a" ) );
    pool.Size.Should().Be( 1 );
    pool.Return( new StringBuilder( "b" ) );
    pool.Size.Should().Be( 1 );
  }

  [Fact]
  public void Return_ShouldThrowArgumentNullException_WhenBuilderIsNull()
  {
    var pool = new StringBuilderPool();
    var act = () => pool.Return( null! );
    act.Should().Throw<ArgumentNullException>();
  }

  [Fact]
  public void Size_ShouldReflectNumberOfBuildersInPool()
  {
    var pool = new StringBuilderPool( 128, 3 );
    pool.Size.Should().Be( 0 );
    pool.Return( new StringBuilder( "x" ) );
    pool.Size.Should().Be( 1 );
    pool.Return( new StringBuilder( "y" ) );
    pool.Size.Should().Be( 2 );
    pool.Get();
    pool.Size.Should().Be( 1 );
  }

  #endregion
}
