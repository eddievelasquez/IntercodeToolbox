// Module Name: MacroValuesTest.cs
// Author:      Eduardo Velasquez
// Copyright (c) 2025, Intercode Consulting, Inc.

namespace Intercode.Toolbox.TemplateEngine.Tests;

using FluentAssertions;

public class MacroValuesTest
{
  #region Tests

  [Fact]
  public void GetValue_WithString_ShouldReturnNull_WhenMacroDoesNotExist()
  {
    var macroTable = DefineMacros( "FOO" );
    var values = macroTable.CreateValues();
    values.GetValue( "BAR" ).Should().BeNull();
  }

  [Fact]
  public void GetValue_WithString_ShouldReturnNull_WhenValueNotSet()
  {
    var macroTable = DefineMacros( "FOO" );
    var values = macroTable.CreateValues();
    values.GetValue( "FOO" ).Should().BeNull();
  }

  [Theory]
  [InlineData( "FOO", "arg", "arg-out" )]
  [InlineData( "FOO", "", "empty" )]
  public void GetValue_WithStringAndArgument_ShouldReturnExpectedValue_WhenGeneratorUsesArgument(
    string macro,
    string arg,
    string expected )
  {
    var macroTable = DefineMacros( macro );
    var values = macroTable.CreateValues();
    values.SetValue( macro, a => a.IsEmpty ? "empty" : a.ToString() + "-out" );
    values.GetValue( macro, arg.AsSpan() ).Should().Be( expected );
  }

  [Fact]
  public void GetValue_WithStringAndArgument_ShouldReturnNull_WhenMacroDoesNotExist()
  {
    var macroTable = DefineMacros( "FOO" );
    var values = macroTable.CreateValues();
    values.GetValue( "BAR", "test".AsSpan() ).Should().BeNull();
  }

  [Fact]
  public void GetValue_WithStringAndArgument_ShouldReturnNull_WhenValueNotSet()
  {
    var macroTable = DefineMacros( "FOO" );
    var values = macroTable.CreateValues();
    values.GetValue( "FOO", "test".AsSpan() ).Should().BeNull();
  }

  [Fact]
  public void MacroValues_ShouldBeIsolatedBetweenInstances()
  {
    var macroTable = DefineMacros( "FOO" );
    var values1 = macroTable.CreateValues();
    var values2 = macroTable.CreateValues();
    values1.SetValue( "FOO", "bar" );
    values2.SetValue( "FOO", "baz" );
    values1.GetValue( "FOO" ).Should().Be( "bar" );
    values2.GetValue( "FOO" ).Should().Be( "baz" );
  }

  [Fact]
  public void MacroValues_ShouldSupportMultipleMacros()
  {
    var macroTable = DefineMacros( "A", "B", "C" );
    var values = macroTable.CreateValues();
    values.SetValue( "A", "1" );
    values.SetValue( "B", "2" );
    values.SetValue( "C", "3" );
    values.GetValue( "A" ).Should().Be( "1" );
    values.GetValue( "B" ).Should().Be( "2" );
    values.GetValue( "C" ).Should().Be( "3" );
  }

  [Fact]
  public void SetValue_ShouldBeCaseInsensitive_WhenSettingAndGettingMacros()
  {
    var macroTable = DefineMacros( "FOO" );
    var values = macroTable.CreateValues();
    values.SetValue( "foo", "bar" );
    values.GetValue( "FOO" ).Should().Be( "bar" );
    values.GetValue( "foo" ).Should().Be( "bar" );
    values.GetValue( "FoO" ).Should().Be( "bar" );
  }

  [Fact]
  public void SetValue_ShouldOverwritePreviousValue_WhenCalledMultipleTimes()
  {
    var macroTable = DefineMacros( "FOO" );
    var values = macroTable.CreateValues();
    values.SetValue( "FOO", "bar" );
    values.SetValue( "FOO", "baz" );
    values.GetValue( "FOO" ).Should().Be( "baz" );
  }

  [Fact]
  public void SetValue_WithStringAndGenerator_ShouldClearValue_WhenSetToNull()
  {
    var macroTable = DefineMacros( "FOO" );
    var values = macroTable.CreateValues();
    values.SetValue( "FOO", _ => "bar" );
    values.SetValue( "FOO", ( MacroValueGenerator? ) null );
    values.GetValue( "FOO" ).Should().BeNull();
  }

  [Fact]
  public void SetValue_WithStringAndGenerator_ShouldSetAndRetrieveValue_WhenMacroExists()
  {
    var macroTable = DefineMacros( "FOO" );
    var values = macroTable.CreateValues();
    values.SetValue( "FOO", _ => "bar" );
    values.GetValue( "FOO" ).Should().Be( "bar" );
  }

  [Fact]
  public void SetValue_WithStringAndGenerator_ShouldThrow_WhenMacroDoesNotExist()
  {
    var macroTable = DefineMacros( "FOO" );
    var values = macroTable.CreateValues();
    var act = () => values.SetValue( "BAR", _ => "baz" );
    act.Should().Throw<ArgumentException>();
  }

  [Fact]
  public void SetValue_WithStringAndString_ShouldClearValue_WhenSetToNull()
  {
    var macroTable = DefineMacros( "FOO" );
    var values = macroTable.CreateValues();
    values.SetValue( "FOO", "bar" );
    values.SetValue( "FOO", ( string? ) null );
    values.GetValue( "FOO" ).Should().BeNull();
  }

  [Fact]
  public void SetValue_WithStringAndString_ShouldSetAndRetrieveStaticValue_WhenMacroExists()
  {
    var macroTable = DefineMacros( "FOO" );
    var values = macroTable.CreateValues();
    values.SetValue( "FOO", "bar" );
    values.GetValue( "FOO" ).Should().Be( "bar" );
  }

  [Fact]
  public void SetValue_WithStringAndString_ShouldThrow_WhenMacroDoesNotExist()
  {
    var macroTable = DefineMacros( "FOO" );
    var values = macroTable.CreateValues();
    var act = () => values.SetValue( "BAR", "baz" );
    act.Should().Throw<ArgumentException>();
  }

  #endregion

  #region Implementation

  private static MacroTable DefineMacros(
    params string[] macros )
  {
    var builder = new MacroTableBuilder();

    foreach( var macro in macros )
    {
      builder.Declare( macro );
    }

    return builder.Build();
  }

  #endregion

#if NET9_0_OR_GREATER

  [Fact]
  public void SetValue_WithSpanAndGenerator_ShouldSetAndRetrieveValue_WhenMacroExists()
  {
    var macroTable = DefineMacros( "FOO" );
    var values = macroTable.CreateValues();
    values.SetValue( "FOO".AsSpan(), _ => "bar" );
    values.GetValue( "FOO" ).Should().Be( "bar" );
  }

  [Fact]
  public void SetValue_WithSpanAndGenerator_ShouldThrow_WhenMacroDoesNotExist()
  {
    var macroTable = DefineMacros( "FOO" );
    var values = macroTable.CreateValues();
    var act = () => values.SetValue( "BAR".AsSpan(), _ => "baz" );
    act.Should().Throw<ArgumentException>();
  }

  [Fact]
  public void SetValue_WithSpanAndGenerator_ShouldClearValue_WhenSetToNull()
  {
    var macroTable = DefineMacros( "FOO" );
    var values = macroTable.CreateValues();
    values.SetValue( "FOO".AsSpan(), _ => "bar" );
    values.SetValue( "FOO".AsSpan(), ( MacroValueGenerator? ) null );
    values.GetValue( "FOO" ).Should().BeNull();
  }

  [Fact]
  public void SetValue_WithSpanAndString_ShouldSetAndRetrieveStaticValue_WhenMacroExists()
  {
    var macroTable = DefineMacros( "FOO" );
    var values = macroTable.CreateValues();
    values.SetValue( "FOO".AsSpan(), "bar" );
    values.GetValue( "FOO" ).Should().Be( "bar" );
  }

  [Fact]
  public void SetValue_WithSpanAndString_ShouldClearValue_WhenSetToNull()
  {
    var macroTable = DefineMacros( "FOO" );
    var values = macroTable.CreateValues();
    values.SetValue( "FOO".AsSpan(), "bar" );
    values.SetValue( "FOO".AsSpan(), ( string? ) null );
    values.GetValue( "FOO" ).Should().BeNull();
  }

  [Fact]
  public void SetValue_WithSpanAndString_ShouldThrow_WhenMacroDoesNotExist()
  {
    var macroTable = DefineMacros( "FOO" );
    var values = macroTable.CreateValues();
    var act = () => values.SetValue( "BAR".AsSpan(), "baz" );
    act.Should().Throw<ArgumentException>();
  }

  [Fact]
  public void GetValue_WithSpan_ShouldReturnNull_WhenValueNotSet()
  {
    var macroTable = DefineMacros( "FOO" );
    var values = macroTable.CreateValues();
    values.GetValue( "FOO".AsSpan() ).Should().BeNull();
  }

  [Fact]
  public void GetValue_WithSpan_ShouldReturnNull_WhenMacroDoesNotExist()
  {
    var macroTable = DefineMacros( "FOO" );
    var values = macroTable.CreateValues();
    values.GetValue( "BAR".AsSpan() ).Should().BeNull();
  }

  [Theory]
  [InlineData( "FOO", "arg", "arg-out" )]
  [InlineData( "FOO", "", "empty" )]
  public void GetValue_WithSpanAndArgument_ShouldReturnExpectedValue_WhenGeneratorUsesArgument(
    string macro,
    string arg,
    string expected )
  {
    var macroTable = DefineMacros( macro );
    var values = macroTable.CreateValues();
    values.SetValue( macro.AsSpan(), a => a.IsEmpty ? "empty" : a.ToString() + "-out" );
    values.GetValue( macro.AsSpan(), arg.AsSpan() ).Should().Be( expected );
  }

  [Fact]
  public void GetValue_WithSpanAndArgument_ShouldReturnNull_WhenMacroDoesNotExist()
  {
    var macroTable = DefineMacros( "FOO" );
    var values = macroTable.CreateValues();
    values.GetValue( "BAR".AsSpan(), "test".AsSpan() ).Should().BeNull();
  }

  [Fact]
  public void GetValue_WithSpanAndArgument_ShouldReturnNull_WhenValueNotSet()
  {
    var macroTable = DefineMacros( "FOO" );
    var values = macroTable.CreateValues();
    values.GetValue( "FOO".AsSpan(), "test".AsSpan() ).Should().BeNull();
  }

#endif
}
