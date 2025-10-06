// Module Name: TemplateCompilerOptionsTest.cs
// Author:      Eduardo Velasquez
// Copyright (c) 2025, Intercode Consulting, Inc.

namespace Intercode.Toolbox.TemplateEngine.Tests;

using FluentAssertions;

public class TemplateCompilerOptionsTest
{
  #region Tests

  [Fact]
  public void Ctor_Default_ShouldUseDeclaredConstants()
  {
    var options = new TemplateCompilerOptions();

    options.MacroDelimiter.Should().Be( TemplateCompilerOptions.DefaultMacroDelimiter );
    options.ArgumentSeparator.Should().Be( TemplateCompilerOptions.DefaultArgumentSeparator );
  }

  [Fact]
  public void Ctor_ShouldThrow_WhenDelimiterAndSeparatorAreTheSame()
  {
    Action act = () => _ = new TemplateCompilerOptions( '#', '#' );

    act.Should()
       .Throw<ArgumentException>()
       .WithMessage( "*cannot be the same*" );
  }

  [Theory]
  [InlineData( 'A' )]
  [InlineData( 'z' )]
  [InlineData( '0' )]
  [InlineData( '_' )]
  [InlineData( '-' )]
  [InlineData( ' ' )]
  [InlineData( '\n' )]
  public void Ctor_ShouldThrowArgumentException_WhenArgumentSeparatorInvalid(
    char invalid )
  {
    Action act = () => _ = new TemplateCompilerOptions( '#', invalid );

    act.Should()
       .Throw<ArgumentException>()
       .WithParameterName( "argumentSeparator" )
       .WithMessage( "*Cannot be alphanumeric, underscore, dash or whitespace.*" );
  }

  [Theory]
  [InlineData( 'A' )]
  [InlineData( 'z' )]
  [InlineData( '0' )]
  [InlineData( '_' )]
  [InlineData( '-' )]
  [InlineData( ' ' )]
  [InlineData( '\t' )]
  public void Ctor_ShouldThrowArgumentException_WhenMacroDelimiterInvalid(
    char invalid )
  {
    Action act = () => _ = new TemplateCompilerOptions( invalid, ':' );

    act.Should()
       .Throw<ArgumentException>()
       .WithParameterName( "macroDelimiter" )
       .WithMessage( "*Cannot be alphanumeric, underscore, dash or whitespace.*" );
  }

  [Theory]
  [InlineData( '#', '|' )]
  [InlineData( '#', ':' )]
  [InlineData( '%', ';' )]
  public void Ctor_WithCustomValues_ShouldSetProperties(
    char macroDelimiter,
    char argumentSeparator )
  {
    var options = new TemplateCompilerOptions( macroDelimiter, argumentSeparator );

    options.MacroDelimiter.Should().Be( macroDelimiter );
    options.ArgumentSeparator.Should().Be( argumentSeparator );
  }

  [Fact]
  public void Default_ShouldExposeSingletonWithExpectedDefaults()
  {
    var a = TemplateCompilerOptions.Default;
    var b = TemplateCompilerOptions.Default;

    a.Should().NotBeNull();
    ReferenceEquals( a, b ).Should().BeTrue();

    a.MacroDelimiter.Should().Be( TemplateCompilerOptions.DefaultMacroDelimiter );
    a.ArgumentSeparator.Should().Be( TemplateCompilerOptions.DefaultArgumentSeparator );
  }

  #endregion
}
