// Module Name: IncludesCollectionTests.cs
// Author:      Eduardo Velasquez
// Copyright (c) 2025, Intercode Consulting, Inc.

namespace Intercode.Toolbox.TemplateEngine.Tests;

public class IncludesCollectionTests
{
  #region Tests

  [Fact]
  public void AddInclude_WithGenerator_ShouldAllowNullContent_WhenNullGeneratorIsProvided()
  {
    var collection = new IncludesCollection();
    collection.AddInclude( "MacroNull", ( MacroValueGenerator? ) null );
    collection.TryGetIncludeContent( "MacroNull", out var content ).Should().BeTrue();
    content.Should().BeNull();
  }

  [Fact]
  public void AddInclude_WithGenerator_ShouldReturnGeneratedContent_WhenCustomGeneratorIsProvided()
  {
    var collection = new IncludesCollection();
    collection.AddInclude( "MacroGen", span => $"Hello {new string( span )}!" );
    collection.TryGetIncludeContent( "MacroGen", out var content ).Should().BeTrue();
    content.Should().Be( "Hello !" );
  }

  [Theory]
  [InlineData( "a" )]
  [InlineData( "1" )]
  public void AddInclude_WithString_ShouldAcceptValidMacroNames_WhenNameIsSingleCharacterOrDigit(
    string validName )
  {
    var collection = new IncludesCollection();
    collection.AddInclude( validName, "Content" );
    collection.TryGetIncludeContent( validName, out var content ).Should().BeTrue();
    content.Should().Be( "Content" );
  }

  [Theory]
  [InlineData( "macro_name" )]
  [InlineData( "macro-name" )]
  [InlineData( "_macro_" )]
  [InlineData( "macro123" )]
  public void AddInclude_WithString_ShouldAcceptValidMacroNames_WhenNameIsValid(
    string validName )
  {
    var collection = new IncludesCollection();
    collection.AddInclude( validName, "Content" );
    collection.TryGetIncludeContent( validName, out var content ).Should().BeTrue();
    content.Should().Be( "Content" );
  }

  [Fact]
  public void AddInclude_WithString_ShouldAddEntryAndIncreaseCount_WhenNewMacroIsAdded()
  {
    var collection = new IncludesCollection();
    collection.AddInclude( "Macro1", "Content1" );
    collection.Count.Should().Be( 1 );
    collection.TryGetIncludeContent( "Macro1", out var content ).Should().BeTrue();
    content.Should().Be( "Content1" );
  }

  [Fact]
  public void AddInclude_WithString_ShouldAllowNullContent_WhenNullContentIsProvided()
  {
    var collection = new IncludesCollection();
    collection.AddInclude( "MacroNull", ( string? ) null );
    collection.TryGetIncludeContent( "MacroNull", out var content ).Should().BeTrue();
    content.Should().BeNull();
  }

  [Fact]
  public void AddInclude_WithString_ShouldBeCaseInsensitive_WhenMacroNameHasDifferentCasing()
  {
    var collection = new IncludesCollection();
    collection.AddInclude( "Macro1", "Content1" );
    collection.TryGetIncludeContent( "macro1", out var content1 ).Should().BeTrue();
    content1.Should().Be( "Content1" );
    collection.TryGetIncludeContent( "MACRO1", out var content2 ).Should().BeTrue();
    content2.Should().Be( "Content1" );
  }

  [Fact]
  public void
    AddInclude_WithString_ShouldOverwriteWithNullContent_WhenNullContentIsProvidedForExistingMacro()
  {
    var collection = new IncludesCollection();
    collection.AddInclude( "Macro1", "Content1" );
    collection.AddInclude( "Macro1", ( string? ) null );
    collection.TryGetIncludeContent( "Macro1", out var content ).Should().BeTrue();
    content.Should().BeNull();
  }

  [Fact]
  public void AddInclude_WithString_ShouldReplaceContent_WhenNameExists()
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
  public void AddInclude_WithString_ShouldThrowArgumentException_WhenNameIsInvalid(
    string invalidName )
  {
    var collection = new IncludesCollection();
    var act = () => collection.AddInclude( invalidName, "Content" );
    act.Should().Throw<ArgumentException>();
  }

  [Fact]
  public void AddInclude_WithString_ShouldThrowArgumentException_WhenNameIsWhitespace()
  {
    var collection = new IncludesCollection();
    var act = () => collection.AddInclude( "   ", "Content" );
    act.Should().Throw<ArgumentException>();
  }

  [Fact]
  public void Constructor_ShouldStartWithZeroCount_WhenCollectionIsCreated()
  {
    var collection = new IncludesCollection();
    collection.Count.Should().Be( 0 );
  }

  [Fact]
  public void Count_ShouldReturnNumberOfUniqueNames_WhenMultipleMacrosAreAdded()
  {
    var collection = new IncludesCollection();
    collection.AddInclude( "A", "1" );
    collection.AddInclude( "B", "2" );
    collection.AddInclude( "C", "3" );
    collection.Count.Should().Be( 3 );
  }

  [Fact]
  public void TryGetIncludeContent_WithString_ShouldReturnFalse_WhenMacroIsNotFound()
  {
    var collection = new IncludesCollection();
    collection.TryGetIncludeContent( "NotFound", out var content ).Should().BeFalse();
    content.Should().BeNull();
  }

  [Fact]
  public void TryGetIncludeContent_WithString_ShouldThrowArgumentNullException_WhenNameIsNull()
  {
    var collection = new IncludesCollection();
    Action act = () => collection.TryGetIncludeContent( null!, out _ );
    act.Should().Throw<ArgumentNullException>();
  }

  #endregion

#if NET9_0_OR_GREATER
  [Fact]
  public void TryGetIncludeContent_WithSpan_ShouldReturnContent_WhenMacroExists()
  {
    var collection = new IncludesCollection();
    collection.AddInclude( "SpanMacro", "SpanContent" );
    var span = "SpanMacro".AsSpan();
    collection.TryGetIncludeContent( span, out var content ).Should().BeTrue();
    content.Should().Be( "SpanContent" );
  }

  [Fact]
  public void TryGetIncludeContent_WithSpan_ShouldReturnFalse_WhenMacroIsNotFound()
  {
    var collection = new IncludesCollection();
    var span = "NotFound".AsSpan();
    collection.TryGetIncludeContent( span, out var content ).Should().BeFalse();
    content.Should().BeNull();
  }

  [Fact]
  public void
    TryGetIncludeContent_WithSpan_ShouldBeCaseInsensitive_WhenMacroNameHasDifferentCasing()
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
