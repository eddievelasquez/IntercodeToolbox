// Module Name: MacroProcessorContextTests.cs
// Author:      Eduardo Velasquez
// Copyright (c) 2025, Intercode Consulting, Inc.

namespace Intercode.Toolbox.TemplateEngine.Tests;

using FluentAssertions;

public class MacroProcessorContextTests
{
  #region Tests

  [Fact]
  public void AddMacro_WithGenerator_ShouldAddMacro_WhenValid()
  {
    var ctx = new MacroProcessorContext();
    var slot = ctx.AddMacro( "macro", _ => "dynamic" );
    slot.Should().Be( 0 );
    ctx.GetMacroValue( slot ).Should().Be( "dynamic" );
  }

  [Fact]
  public void AddMacro_WithGenerator_ShouldAddMacroWithNullValue_WhenGeneratorIsNull()
  {
    var ctx = new MacroProcessorContext();
    var slot = ctx.AddMacro( "macro" );
    slot.Should().Be( 0 );
    ctx.GetMacroValue( slot ).Should().BeNull();
  }

  [Theory]
  [InlineData( null )]
  [InlineData( "" )]
  [InlineData( "   " )]
  public void AddMacro_WithGenerator_ShouldThrow_WhenMacroNameIsInvalid(
    string? macroName )
  {
    var ctx = new MacroProcessorContext();
    var act = () => ctx.AddMacro( macroName!, _ => "value" );
    act.Should().Throw<ArgumentException>();
  }

  [Fact]
  public void AddMacro_WithStringValue_ShouldAddMacro_WhenValid()
  {
    var ctx = new MacroProcessorContext();
    var slot = ctx.AddMacro( "macro", "value" );
    slot.Should().Be( 0 );
    ctx.GetMacroValue( slot ).Should().Be( "value" );
  }

  [Theory]
  [InlineData( null )]
  [InlineData( "" )]
  [InlineData( "   " )]
  public void AddMacro_WithStringValue_ShouldThrow_WhenMacroNameIsInvalid(
    string? macroName )
  {
    var ctx = new MacroProcessorContext();
    var act = () => ctx.AddMacro( macroName!, "value" );
    act.Should().Throw<ArgumentException>();
  }

  [Fact]
  public void AddMacro_WithStringValue_ShouldThrow_WhenValueIsNull()
  {
    var ctx = new MacroProcessorContext();
    var act = () => ctx.AddMacro( "macro", ( string ) null! );
    act.Should().Throw<ArgumentNullException>();
  }

  [Fact]
  public void AddMacros_WithGenerator_ShouldAddMacros_WhenValid()
  {
    var ctx = new MacroProcessorContext();

    var macros = new[]
    {
      new KeyValuePair<string, MacroValueGenerator?>( "A", _ => "1" ),
      new KeyValuePair<string, MacroValueGenerator?>( "B", _ => "2" )
    };
    ctx.AddMacros( macros );
    ctx.GetMacroValue( "A" ).Should().Be( "1" );
    ctx.GetMacroValue( "B" ).Should().Be( "2" );
  }

  [Fact]
  public void AddMacros_WithString_ShouldAddMacros_WhenValid()
  {
    var ctx = new MacroProcessorContext();
    var macros = new[] { new KeyValuePair<string, string>( "A", "1" ), new KeyValuePair<string, string>( "B", "2" ) };

    ctx.AddMacros( macros );
    ctx.GetMacroValue( "A" ).Should().Be( "1" );
    ctx.GetMacroValue( "B" ).Should().Be( "2" );
  }

  [Fact]
  public void GetMacroSlot_ShouldReturnMinusOne_WhenNotFound()
  {
    var ctx = new MacroProcessorContext();
    ctx.GetMacroSlot( "not_found" ).Should().Be( -1 );
  }

  [Fact]
  public void GetMacroSlot_ShouldReturnSlot_WhenFound()
  {
    var ctx = new MacroProcessorContext();
    ctx.AddMacro( "macro", "value" );
    ctx.GetMacroSlot( "macro" ).Should().Be( 0 );
  }

  [Fact]
  public void GetMacroSlot_ShouldThrow_WhenMacroNameIsNull()
  {
    var ctx = new MacroProcessorContext();
    var act = () => ctx.GetMacroSlot( null! );
    act.Should().Throw<ArgumentNullException>();
  }

  [Theory]
  [InlineData( -1 )]
  [InlineData( 1 )]
  public void GetMacroValue_WithInt_ShouldReturnNull_WhenSlotIsInvalid(
    int slot )
  {
    var ctx = new MacroProcessorContext();
    ctx.AddMacro( "macro", "value" );
    ctx.GetMacroValue( slot ).Should().BeNull();
  }

  [Fact]
  public void GetMacroValue_WithInt_ShouldReturnValue_WhenArgumentIsProvided()
  {
    var ctx = new MacroProcessorContext();
    var slot = ctx.AddMacro( "macro", arg => arg.ToString() );
    ctx.GetMacroValue( slot, "test" ).Should().Be( "test" );
  }

  [Fact]
  public void GetMacroValue_WithInt_ShouldReturnValue_WhenSlotIsValid()
  {
    var ctx = new MacroProcessorContext();
    var slot = ctx.AddMacro( "macro", "value" );
    ctx.GetMacroValue( slot ).Should().Be( "value" );
  }

  [Fact]
  public void GetMacroValue_WithString_ShouldReturnNull_WhenNotFound()
  {
    var ctx = new MacroProcessorContext();
    ctx.GetMacroValue( "not_found" ).Should().BeNull();
  }

  [Fact]
  public void GetMacroValue_WithString_ShouldReturnValue_WhenArgumentIsProvided()
  {
    var ctx = new MacroProcessorContext();
    ctx.AddMacro( "macro", arg => arg.ToString() );
    ctx.GetMacroValue( "macro", "test" ).Should().Be( "test" );
  }

  [Fact]
  public void GetMacroValue_WithString_ShouldReturnValue_WhenFound()
  {
    var ctx = new MacroProcessorContext();
    ctx.AddMacro( "macro", "value" );
    ctx.GetMacroValue( "macro" ).Should().Be( "value" );
  }

  [Theory]
  [InlineData( null )]
  public void GetMacroValue_WithString_ShouldThrow_WhenMacroNameIsNull(
    string? macroName )
  {
    var ctx = new MacroProcessorContext();
    var act = () => ctx.GetMacroValue( macroName! );
    act.Should().Throw<ArgumentNullException>();
  }

  #endregion
}
