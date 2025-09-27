// Module Name: MacroTableBuilderTest.cs
// Author:      Eduardo Velasquez
// Copyright (c) 2025, Intercode Consulting, Inc.

namespace Intercode.Toolbox.TemplateEngine.Tests;

public class MacroTableBuilderTest
{
  #region Tests

  [Fact]
  public void Build_ShouldAssignConsistentSlots_WhenMixingStandardAndUserMacros()
  {
    var builder1 = new MacroTableBuilder();
    builder1.Declare( "A" );
    builder1.Declare( "B" );
    var table1 = builder1.Build();

    var builder2 = new MacroTableBuilder();
    builder2.Declare( "A" );
    builder2.Declare( "B" );
    var table2 = builder2.Build();

    table1.Count.Should().Be( table2.Count );

    table1.GetSlot( "A" ).Should().Be( table2.GetSlot( "A" ) );
    table1.GetSlot( "B" ).Should().Be( table2.GetSlot( "B" ) );
  }

  [Fact]
  public void Build_ShouldBeCaseInsensitive_ForStandardMacros()
  {
    var builder = new MacroTableBuilder();
    var table = builder.Build();

    foreach( var std in StandardMacros.GetStandardMacroNames() )
    {
      table.GetSlot( std.ToLowerInvariant() ).Should().BeLessThan( MacroTable.MacroNotFoundSlot );

      table.GetSlot( std.ToUpperInvariant() )
           .Should()
           .BeLessThan( MacroTable.MacroNotFoundSlot );
    }
  }

  [Fact]
  public void Build_ShouldReturnMacroTable_WithCorrectSlots()
  {
    var builder = new MacroTableBuilder();
    builder.Declare( "A" );
    builder.Declare( "B" );
    builder.Declare( "C" );

    var table = builder.Build();
    table.Count.Should().Be( 3 );
    table.GetSlot( "A" ).Should().Be( 1 );
    table.GetSlot( "B" ).Should().Be( 2 );
    table.GetSlot( "C" ).Should().Be( 3 );
  }

  [Fact]
  public void Build_ShouldReturnEmptyTable_WhenNoMacrosDeclared()
  {
    var table = new MacroTableBuilder().Build();
    table.Count.Should().Be( 0 );
  }

  [Fact]
  public void Declare_WithString_ShouldAddMacro_WhenMacroNameIsValid()
  {
    var builder = new MacroTableBuilder();
    builder.Declare( "FOO" );

    var table = builder.Build();
    table.Count.Should().Be( 1 );
    table.GetSlot( "FOO" ).Should().Be( 1 );
  }

  [Fact]
  public void Declare_WithString_ShouldAllowStandardMacroNames()
  {
    var builder = new MacroTableBuilder();
    builder.Declare( StandardMacros.NowMacroName );

    var table = builder.Build();
    table.Count.Should().Be( 1 );
    table.GetSlot( StandardMacros.NowMacroName ).Should().Be( 1 );
  }

  [Fact]
  public void Declare_WithStringAndSpan_ShouldTreatDuplicatesAcrossApis()
  {
    var builder = new MacroTableBuilder();
#if NET9_0_OR_GREATER
    builder.Declare( "FOO".AsSpan() );
#else
    builder.Declare( "FOO" );
#endif
    builder.Declare( "foo" );

    var table = builder.Build();
    table.Count.Should().Be( 1 );
    table.GetSlot( "FOO" ).Should().Be( 1 );
  }

  [Fact]
  public void Declare_WithString_ShouldIgnoreDuplicateMacroNames_CaseInsensitive()
  {
    var builder = new MacroTableBuilder();
    builder.Declare( "FOO" );
    builder.Declare( "foo" );

    var table = builder.Build();
    table.Count.Should().Be( 1 );
    table.GetSlot( "FOO" ).Should().Be( 1 );
    table.GetSlot( "foo" ).Should().Be( 1 );
  }

  [Theory]
  [InlineData( null )]
  [InlineData( "" )]
  [InlineData( " " )]
  [InlineData( "A B" )]
  [InlineData( "A.B" )]
  [InlineData( "$X" )]
  [InlineData( "X!" )]
  public void Declare_WithString_ShouldThrowArgumentException_WhenMacroNameInvalid(
    string? macroName )
  {
    var builder = new MacroTableBuilder();
    var act = () => builder.Declare( macroName! );
    act.Should().Throw<ArgumentException>();
  }

  #endregion

#if NET9_0_OR_GREATER
  [Fact]
  public void Declare_WithReadOnlySpan_ShouldAddMacro_WhenMacroNameIsValid()
  {
    var builder = new MacroTableBuilder();
    builder.Declare( "FOO".AsSpan() );

    var table = builder.Build();
    table.Count.Should().Be( 1 );
    table.GetSlot( "FOO" ).Should().Be( 1 );
  }

  [Fact]
  public void Declare_WithReadOnlySpan_ShouldAllowStandardMacroNames()
  {
    var builder = new MacroTableBuilder();
    builder.Declare( StandardMacros.GuidMacroName.AsSpan() );

    var table = builder.Build();
    table.Count.Should().Be( 1 );
    table.GetSlot( StandardMacros.GuidMacroName ).Should().Be( 1 );
  }

  [Fact]
  public void Declare_WithReadOnlySpan_ShouldIgnoreDuplicateMacroNames_CaseInsensitive()
  {
    var builder = new MacroTableBuilder();
    builder.Declare( "FOO".AsSpan() );
    builder.Declare( "foo".AsSpan() );

    var table = builder.Build();
    table.Count.Should().Be( 1 );
    table.GetSlot( "FOO" ).Should().Be( 1 );
    table.GetSlot( "foo" ).Should().Be( 1 );
  }

  [Theory]
  [InlineData( null )]
  [InlineData( "" )]
  [InlineData( " " )]
  [InlineData( "A B" )]
  [InlineData( "A.B" )]
  [InlineData( "$X" )]
  [InlineData( "X!" )]
  public void Declare_WithReadOnlySpan_ShouldThrowArgumentException_WhenMacroNameInvalid(
    string? macroName )
  {
    var builder = new MacroTableBuilder();
    var act = () => builder.Declare( macroName.AsSpan() );
    act.Should().Throw<ArgumentException>();
  }
#endif
}
