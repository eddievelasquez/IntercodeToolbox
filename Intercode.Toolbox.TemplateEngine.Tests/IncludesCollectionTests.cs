// Module Name: IncludesCollectionTests.cs
// Author:      Eduardo Velasquez
// Copyright (c) 2025, Intercode Consulting, Inc.

namespace Intercode.Toolbox.TemplateEngine.Tests;

using FluentAssertions;

public class IncludesCollectionTests
{
  #region Tests

  [Fact]
  public void AddInclude_ShouldAddEntryAndIncreaseCount()
  {
    var collection = new IncludesCollection();
    collection.AddInclude( "Macro1", "Content1" );
    collection.Count.Should().Be( 1 );
    collection.TryGetIncludeContent( "Macro1", out var content ).Should().BeTrue();
    content.Should().Be( "Content1" );
  }

  [Fact]
  public void AddInclude_ShouldAllowNullContent()
  {
    var collection = new IncludesCollection();
    collection.AddInclude( "MacroNull", null );
    collection.TryGetIncludeContent( "MacroNull", out var content ).Should().BeTrue();
    content.Should().BeNull();
  }

  [Fact]
  public void AddInclude_ShouldBeCaseInsensitive()
  {
    var collection = new IncludesCollection();
    collection.AddInclude( "Macro1", "Content1" );
    collection.TryGetIncludeContent( "macro1", out var content1 ).Should().BeTrue();
    content1.Should().Be( "Content1" );
    collection.TryGetIncludeContent( "MACRO1", out var content2 ).Should().BeTrue();
    content2.Should().Be( "Content1" );
  }

  [Fact]
  public void AddInclude_ShouldReplaceContentIfNameExists()
  {
    var collection = new IncludesCollection();
    collection.AddInclude( "Macro1", "Content1" );
    collection.AddInclude( "Macro1", "Content2" );
    collection.Count.Should().Be( 1 );
    collection.TryGetIncludeContent( "Macro1", out var content ).Should().BeTrue();
    content.Should().Be( "Content2" );
  }

  [Theory]
  [InlineData( "" )]
  [InlineData( " " )]
  [InlineData( "Invalid!" )]
  [InlineData( "Name*" )]
  public void AddInclude_ShouldThrowArgumentExceptionIfNameIsInvalid(
    string invalidName )
  {
    var collection = new IncludesCollection();
    var act = () => collection.AddInclude( invalidName, "Content" );
    act.Should().Throw<ArgumentException>();
  }

  [Fact]
  public void Constructor_ShouldStartWithZeroCount()
  {
    var collection = new IncludesCollection();
    collection.Count.Should().Be( 0 );
  }

  [Fact]
  public void Count_ShouldReflectNumberOfUniqueNames()
  {
    var collection = new IncludesCollection();
    collection.AddInclude( "A", "1" );
    collection.AddInclude( "B", "2" );
    collection.AddInclude( "C", "3" );
    collection.Count.Should().Be( 3 );
  }

  [Fact]
  public void TryGetIncludeContent_ShouldReturnFalseIfNotFound()
  {
    var collection = new IncludesCollection();
    collection.TryGetIncludeContent( "NotFound", out var content ).Should().BeFalse();
    content.Should().BeNull();
  }

  [Fact]
  public void TryGetIncludeContent_ShouldThrowArgumentNullExceptionIfNameIsNull()
  {
    var collection = new IncludesCollection();
    Action act = () => collection.TryGetIncludeContent( null!, out _ );
    act.Should().Throw<ArgumentNullException>();
  }

  #endregion

#if NET9_0_OR_GREATER
  [Fact]
  public void TryGetIncludeContent_SpanOverload_ShouldReturnContentIfExists()
  {
    var collection = new IncludesCollection();
    collection.AddInclude( "SpanMacro", "SpanContent" );
    var span = "SpanMacro".AsSpan();
    collection.TryGetIncludeContent( span, out var content ).Should().BeTrue();
    content.Should().Be( "SpanContent" );
  }

  [Fact]
  public void TryGetIncludeContent_SpanOverload_ShouldReturnFalseIfNotFound()
  {
    var collection = new IncludesCollection();
    var span = "NotFound".AsSpan();
    collection.TryGetIncludeContent( span, out var content ).Should().BeFalse();
    content.Should().BeNull();
  }

  [Fact]
  public void TryGetIncludeContent_SpanOverload_ShouldBeCaseInsensitive()
  {
    var collection = new IncludesCollection();
    collection.AddInclude( "SpanMacro", "Value" );
    collection.TryGetIncludeContent( "spanmacro".AsSpan(), out var content1 ).Should().BeTrue();
    content1.Should().Be( "Value" );
    collection.TryGetIncludeContent( "SPANMACRO".AsSpan(), out var content2 ).Should().BeTrue();
    content2.Should().Be( "Value" );
  }
#endif
}
