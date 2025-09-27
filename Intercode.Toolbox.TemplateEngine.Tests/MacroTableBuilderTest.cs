// Module Name: MacroTableBuilderTest.cs
// Author:      Eduardo Velasquez
// Copyright (c) 2025, Intercode Consulting, Inc.

namespace Intercode.Toolbox.TemplateEngine.Tests;

using FluentAssertions;

public class MacroTableBuilderTest
{
  #region Tests

  [Fact]
  public void Build_ShouldAssignConsistentSlots_WhenMixingStandardAndUserMacros()
  {
    var builder1 = new MacroTableBuilder();
    builder1.DeclareStandardMacros();
    builder1.Declare( "A" );
    builder1.Declare( "B" );
    var table1 = builder1.Build();

    var builder2 = new MacroTableBuilder();
    builder2.DeclareStandardMacros();
    builder2.Declare( "A" );
    builder2.Declare( "B" );
    var table2 = builder2.Build();

    table1.Count.Should().Be( table2.Count );

    foreach( var std in StandardMacros.GetStandardMacroNames() )
    {
      table1.GetSlot( std ).Should().Be( table2.GetSlot( std ) );
    }

    table1.GetSlot( "A" ).Should().Be( table2.GetSlot( "A" ) );
    table1.GetSlot( "B" ).Should().Be( table2.GetSlot( "B" ) );
  }

  [Fact]
  public void Build_ShouldBeCaseInsensitive_ForStandardMacros()
  {
    var builder = new MacroTableBuilder();
    builder.DeclareStandardMacros();
    var table = builder.Build();

    foreach( var std in StandardMacros.GetStandardMacroNames() )
    {
      table.GetSlot( std.ToLowerInvariant() ).Should().BeGreaterOrEqualTo( 0 );
      table.GetSlot( std.ToUpperInvariant() ).Should().BeGreaterOrEqualTo( 0 );
    }
  }

  [Fact]
  public void Build_ShouldIncludeStandardAndUserMacros_WithCorrectSlotOrder()
  {
    var builder = new MacroTableBuilder();
    builder.DeclareStandardMacros();
    builder.Declare( "USER1" );
    builder.Declare( "USER2" );
    var table = builder.Build();

    var std = StandardMacros.GetStandardMacroNames().ToList();
    table.Count.Should().Be( std.Count + 2 );

    table.GetSlot( "USER1" ).Should().Be( 0 );
    table.GetSlot( "USER2" ).Should().Be( 1 );

    for( var i = 0; i < std.Count; i++ )
    {
      table.GetSlot( std[i] ).Should().Be( i + 2 );
    }
  }

  [Fact]
  public void Build_ShouldIncludeStandardMacros_WhenDeclareStandardMacrosIsCalled()
  {
    var builder = new MacroTableBuilder();
    builder.DeclareStandardMacros();
    var table = builder.Build();
    var expected = StandardMacros.GetStandardMacroNames().ToList();
    table.Count.Should().Be( expected.Count );

    for( var i = 0; i < expected.Count; i++ )
    {
      table.GetSlot( expected[i] ).Should().Be( i );
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
    table.GetSlot( "A" ).Should().Be( 0 );
    table.GetSlot( "B" ).Should().Be( 1 );
    table.GetSlot( "C" ).Should().Be( 2 );
  }

  //[Fact]
  //public void Build_ShouldThrowInvalidOperationException_WhenNoMacrosDeclared()
  //{
  //  var builder = new MacroTableBuilder();
  //  Action act = () => builder.Build();

  //  act.Should()
  //     .Throw<InvalidOperationException>()
  //     .WithMessage( "Must declare at least one macro" );
  //}

  [Fact]
  public void Declare_WithString_ShouldAddMacro_WhenMacroNameIsValid()
  {
    var builder = new MacroTableBuilder();
    builder.Declare( "FOO" );

    var table = builder.Build();
    table.Count.Should().Be( 1 );
    table.GetSlot( "FOO" ).Should().Be( 0 );
  }

  [Fact]
  public void Declare_WithString_ShouldIgnoreDuplicateMacroNames_CaseInsensitive()
  {
    var builder = new MacroTableBuilder();
    builder.Declare( "FOO" );
    builder.Declare( "foo" );

    var table = builder.Build();
    table.Count.Should().Be( 1 );
    table.GetSlot( "FOO" ).Should().Be( 0 );
    table.GetSlot( "foo" ).Should().Be( 0 );
  }

  [Theory]
  [InlineData( null )]
  [InlineData( "" )]
  public void Declare_WithString_ShouldThrowArgumentException_WhenMacroNameIsNullOrEmpty(
    string? macroName )
  {
    var builder = new MacroTableBuilder();
    Action act = () => builder.Declare( macroName! );
    act.Should().Throw<ArgumentException>();
  }

  [Fact]
  public void DeclareStandardMacros_CalledMultipleTimes_ShouldNotDuplicateMacros()
  {
    var builder = new MacroTableBuilder();
    builder.DeclareStandardMacros();
    builder.DeclareStandardMacros();
    var table = builder.Build();
    var std = StandardMacros.GetStandardMacroNames().ToList();
    table.Count.Should().Be( std.Count );

    for( var i = 0; i < std.Count; i++ )
    {
      table.GetSlot( std[i] ).Should().Be( i );
    }
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
    table.GetSlot( "FOO" ).Should().Be( 0 );
  }

  [Fact]
  public void Declare_WithReadOnlySpan_ShouldIgnoreDuplicateMacroNames_CaseInsensitive()
  {
    var builder = new MacroTableBuilder();
    builder.Declare( "FOO".AsSpan() );
    builder.Declare( "foo".AsSpan() );

    var table = builder.Build();
    table.Count.Should().Be( 1 );
    table.GetSlot( "FOO" ).Should().Be( 0 );
    table.GetSlot( "foo" ).Should().Be( 0 );
  }

  [Theory]
  [InlineData( null )]
  [InlineData( "" )]
  public void Declare_WithReadOnlySpan_ShouldThrowArgumentException_WhenMacroNameIsNullOrEmpty(
    string? macroName )
  {
    var builder = new MacroTableBuilder();
    var act = () => builder.Declare( macroName.AsSpan() );
    act.Should().Throw<ArgumentException>();
  }
#endif
}
