// Module Name: TemplateCompilerOptionsBuilderTests.cs
// Author:      Eduardo Velasquez
// Copyright (c) 2025, Intercode Consulting, Inc.

namespace Intercode.Toolbox.TemplateEngine.Tests;

using FluentAssertions;

public class TemplateCompilerOptionsBuilderTests
{
  #region Tests

  [Fact]
  public void Build_ReturnsDefaultValues_WhenNoChangesMade()
  {
    var builder = new TemplateCompilerOptionsBuilder();
    var options = builder.Build();
    options.MacroDelimiter.Should().Be( TemplateCompilerOptionsBuilder.DefaultMacroDelimiter );
    options.ArgumentSeparator.Should().Be( TemplateCompilerOptionsBuilder.DefaultArgumentSeparator );
  }

  [Theory]
  [InlineData( 'B' )]
  [InlineData( '2' )]
  [InlineData( ' ' )]
  public void Build_ThrowsArgumentException_WhenArgumentSeparatorIsNotPunctuation(
    char invalid )
  {
    var builder = new TemplateCompilerOptionsBuilder().SetArgumentSeparator( invalid );
    Action act = () => builder.Build();

    act.Should()
       .Throw<ArgumentException>()
       .WithMessage( "*Cannot be alphanumeric, underscore, dash or whitespace*" );
  }

  [Fact]
  public void Build_ThrowsArgumentException_WhenDelimiterAndSeparatorAreSame()
  {
    var builder = new TemplateCompilerOptionsBuilder()
                  .SetMacroDelimiter( '!' )
                  .SetArgumentSeparator( '!' );
    Action act = () => builder.Build();

    act.Should()
       .Throw<ArgumentException>()
       .WithMessage( "*cannot be the same*" );
  }

  [Theory]
  [InlineData( 'A' )]
  [InlineData( '1' )]
  [InlineData( ' ' )]
  public void Build_ThrowsArgumentException_WhenMacroDelimiterIsNotPunctuation(
    char invalid )
  {
    var builder = new TemplateCompilerOptionsBuilder().SetMacroDelimiter( invalid );
    Action act = () => builder.Build();

    act.Should()
       .Throw<ArgumentException>()
       .WithMessage( "*Cannot be alphanumeric, underscore, dash or whitespace*" );
  }

  [Fact]
  public void SetArgumentSeparator_UpdatesArgumentSeparator()
  {
    var builder = new TemplateCompilerOptionsBuilder();
    var options = builder.SetArgumentSeparator( ';' ).Build();
    options.MacroDelimiter.Should().Be( TemplateCompilerOptionsBuilder.DefaultMacroDelimiter );
    options.ArgumentSeparator.Should().Be( ';' );
  }

  [Fact]
  public void SetMacroDelimiter_UpdatesMacroDelimiter()
  {
    var builder = new TemplateCompilerOptionsBuilder();
    var options = builder.SetMacroDelimiter( '#' ).Build();
    options.MacroDelimiter.Should().Be( '#' );
    options.ArgumentSeparator.Should().Be( TemplateCompilerOptionsBuilder.DefaultArgumentSeparator );
  }

  #endregion
}
