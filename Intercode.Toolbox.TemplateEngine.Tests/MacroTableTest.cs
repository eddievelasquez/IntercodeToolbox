// Module Name: MacroTableTest.cs
// Author:      Eduardo Velasquez
// Copyright (c) 2025, Intercode Consulting, Inc.

namespace Intercode.Toolbox.TemplateEngine.Tests;

public class MacroTableTest
{
  #region Tests

  [Fact]
  public void GetSlot_WithString_ShouldPreferUserDefinedMacro_WhenNameMatchesStandardMacro()
  {
    var builder = new MacroTableBuilder();
    builder.Declare( StandardMacros.GuidMacroName );
    var table = builder.Build();

    var slot = table.GetSlot( StandardMacros.GuidMacroName );
    slot.Should().BePositive();
  }

  [Fact]
  public void GetSlot_WithString_ShouldReturnCorrectSlot_WhenMultipleMacrosDeclared()
  {
    var builder = new MacroTableBuilder();
    builder.Declare( "A" );
    builder.Declare( "B" );
    builder.Declare( "C" );

    var table = builder.Build();
    table.GetSlot( "A" ).Should().Be( 1 );
    table.GetSlot( "B" ).Should().Be( 2 );
    table.GetSlot( "C" ).Should().Be( 3 );
  }

  [Fact]
  public void GetSlot_WithString_ShouldReturnMinusOne_WhenMacroDoesNotExist()
  {
    var builder = new MacroTableBuilder();
    builder.Declare( "FOO" );

    var table = builder.Build();
    table.GetSlot( "BAR" ).Should().Be( MacroTable.MacroNotFoundSlot );
  }

  [Fact]
  public void GetSlot_WithString_ShouldReturnNotFound_WhenMacroNameIsWhitespace()
  {
    var table = new MacroTableBuilder().Build();
    table.GetSlot( "   " ).Should().Be( MacroTable.MacroNotFoundSlot );
  }

  [Fact]
  public void GetSlot_WithString_ShouldReturnSlotIndex_WhenMacroExists_CaseInsensitive()
  {
    var builder = new MacroTableBuilder();
    builder.Declare( "FOO" );

    var table = builder.Build();
    table.GetSlot( "FOO" ).Should().Be( 1 );
    table.GetSlot( "foo" ).Should().Be( 1 );
    table.GetSlot( "FoO" ).Should().Be( 1 );
  }

  [Theory]
  [InlineData( null )]
  [InlineData( "" )]
  public void GetSlot_WithString_ShouldThrow_WhenMacroNameIsNullOrEmpty(
    string? macroName )
  {
    var builder = new MacroTableBuilder();
    builder.Declare( "Foo" );

    var table = builder.Build();
    Action act = () => table.GetSlot( macroName! );
    act.Should().Throw<ArgumentException>();
  }

  #endregion

#if NET9_0_OR_GREATER
  [Fact]
  public void GetSlot_WithReadOnlySpan_ShouldReturnMinusOne_WhenMacroDoesNotExist()
  {
    var builder = new MacroTableBuilder();
    builder.Declare( "FOO" );

    var table = builder.Build();
    table.GetSlot( "BAR".AsSpan() ).Should().Be( 0 );
  }

  [Fact]
  public void GetSlot_WithReadOnlySpan_ShouldReturnMinusOne_WhenMacroIsEmpty()
  {
    var builder = new MacroTableBuilder();
    builder.Declare( "FOO" );

    var table = builder.Build();
    table.GetSlot( ReadOnlySpan<char>.Empty ).Should().Be( 0 );
  }

  [Fact]
  public void GetSlot_WithReadOnlySpan_ShouldReturnNotFound_WhenMacroWhitespace()
  {
    var table = new MacroTableBuilder().Build();
    table.GetSlot( "   ".AsSpan() ).Should().Be( MacroTable.MacroNotFoundSlot );
  }

  [Fact]
  public void GetSlot_WithReadOnlySpan_ShouldPreferUserDefinedMacro_WhenNameMatchesStandardMacro()
  {
    var builder = new MacroTableBuilder();
    builder.Declare( StandardMacros.EnvMacroName );
    var table = builder.Build();

    table.GetSlot( StandardMacros.EnvMacroName.AsSpan() ).Should().BePositive();
  }

  [Fact]
  public void GetSlot_WithReadOnlySpan_ShouldReturnSlotIndex_WhenMacroExists_CaseInsensitive()
  {
    var builder = new MacroTableBuilder();
    builder.Declare( "FOO" );

    var table = builder.Build();
    table.GetSlot( "FOO".AsSpan() ).Should().Be( 1 );
    table.GetSlot( "foo".AsSpan() ).Should().Be( 1 );
    table.GetSlot( "FoO".AsSpan() ).Should().Be( 1 );
  }

  [Fact]
  public void GetSlot_WithReadOnlySpan_ShouldReturnCorrectSlot_WhenMultipleMacrosDeclared()
  {
    var builder = new MacroTableBuilder();
    builder.Declare( "A" );
    builder.Declare( "B" );
    builder.Declare( "C" );

    var table = builder.Build();
    table.GetSlot( "A".AsSpan() ).Should().Be( 1 );
    table.GetSlot( "B".AsSpan() ).Should().Be( 2 );
    table.GetSlot( "C".AsSpan() ).Should().Be( 3 );
  }

#endif
}
