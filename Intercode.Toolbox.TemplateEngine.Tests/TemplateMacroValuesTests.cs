#if false
// Module Name: TemplateMacroValuesTests.cs
// Author:      Eduardo Velasquez
// Copyright (c) 2025, Intercode Consulting, Inc.

namespace Intercode.Toolbox.TemplateEngine.Tests;

using FluentAssertions;
using Microsoft.Extensions.Time.Testing;

public class TemplateMacroValuesTests
{
  #region Fields

  private readonly FakeTimeProvider _timeProvider = new(
    new DateTimeOffset(
      2024,
      10,
      20,
      10,
      30,
      0,
      TimeSpan.FromHours( -7 )
    )
  );

  #endregion

  #region Tests

  [Theory]
  [InlineData( null )]
  [InlineData( "" )]
  [InlineData( " " )]
  [InlineData( "\t" )]
  [InlineData( "\n" )]
  [InlineData( "\r" )]
  public void GetMacroSlot_ShouldThrow_WhenMacroNameIsInvalid(
    string? macroName )
  {
    var template = CompileTemplate( "Hello $macroA$" );
    var values = new TemplateMacroValues( template );

    var act = () => values.GetMacroSlot( macroName! );
    act.Should().Throw<ArgumentException>();
  }

  [Fact]
  public void GetMacroSlot_ShouldReturnSlot_WhenMacroIsFound()
  {
    var template = CompileTemplate( "$macroA$$macroB$$macroA$" );
    var values = new TemplateMacroValues( template );
    values.GetMacroSlot( "macroA" ).Should().Be( 0 );
    values.GetMacroSlot( "macroB" ).Should().Be( 1 );
  }

  [Theory]
  [InlineData( -1 )]
  [InlineData( 1 )]
  [InlineData( 100 )]
  public void GetMacroValue_BySlot_ReturnsNull_WhenSlotIsInvalid(
    int slot )
  {
    var template = CompileTemplate( "Hello $macroA$" );

    var values = new TemplateMacroValues( template )
      .SetMacro( "macroA", "value1" );

    values.GetMacroValue( slot ).Should().BeNull();
  }

  [Fact]
  public void GetMacroValue_BySlot_ReturnsValue_WhenSlotIsValid()
  {
    var template = CompileTemplate( "Hello $macroA$ $macroB$" );

    var values = new TemplateMacroValues( template )
                 .SetMacro( "macroA", "value1" )
                 .SetMacro( "macroB", "value2" );

    var macroASlot = values.GetMacroSlot( "macroA" );
    var macroBSlot = values.GetMacroSlot( "macroB" );

    values.GetMacroValue( macroASlot ).Should().Be( "value1" );
    values.GetMacroValue( macroBSlot ).Should().Be( "value2" );
  }

  [Fact]
  public void GetMacroValue_BySlotWithArgument_PassesArgumentToGenerator()
  {
    var template = CompileTemplate( "Hello $macroA$" );

    var values = new TemplateMacroValues( template )
      .SetMacro( "macroA", arg => $"arg:{arg.ToString()}" );

    var macroASlot = values.GetMacroSlot( "macroA" );
    values.GetMacroValue( macroASlot, "testArg".AsSpan() ).Should().Be( "arg:testArg" );
  }

  [Theory]
  [InlineData( -1 )]
  [InlineData( 1 )]
  [InlineData( 100 )]
  public void GetMacroValue_BySlotWithArgument_ReturnsNull_WhenSlotIsInvalid(
    int slot )
  {
    var template = CompileTemplate( "Hello $macroA$" );

    var values = new TemplateMacroValues( template )
      .SetMacro( "macroA", arg => $"arg:{arg.ToString()}" );
    values.GetMacroValue( slot, "testArg" ).Should().BeNull();
  }

  [Fact]
  public void GetMacroValue_ReturnsValue_WhenMacroIsSet()
  {
    var template = CompileTemplate( "Hello $macroA$" );

    var values = new TemplateMacroValues( template )
      .SetMacro( "macroA", "value1" );
    values.GetMacroValue( "macroA" ).Should().Be( "value1" );
  }

  [Fact]
  public void GetMacroValue_ShouldReturnNull_WhenMacroNotFound()
  {
    var template = CompileTemplate( "Hello $macroA$" );
    var values = new TemplateMacroValues( template );
    values.GetMacroValue( "macroB" ).Should().BeNull();
  }

  [Fact]
  public void GetMacroValue_WithArgument_PassesArgumentToGenerator()
  {
    var template = CompileTemplate( "Hello $macroA$" );

    var values = new TemplateMacroValues( template )
      .SetMacro( "macroA", arg => $"arg:{arg.ToString()}" );
    values.GetMacroValue( "macroA", "testArg".AsSpan() ).Should().Be( "arg:testArg" );
  }

  [Fact]
  public void GetMacroValue_WithDynamicValue_ShouldBeCaseInsensitive()
  {
    var template = CompileTemplate( "$macro$" );
    var values = template.CreateMacroValues().SetMacro( "macro", _ => _timeProvider.GetLocalNow().ToString() );

    values.GetMacroValue( "MaCrO" ).Should().Be( "10/20/2024 10:30:00 AM -07:00" );
  }

  [Fact]
  public void GetMacroValue_WithDynamicValue_ShouldReturnNull_WhenNotFound()
  {
    var template = CompileTemplate( "$macro$" );
    var values = template.CreateMacroValues().SetMacro( "macro", _ => _timeProvider.GetLocalNow().ToString() );

    values.GetMacroValue( "Unknown" ).Should().BeNull();
  }

  [Fact]
  public void GetMacroValue_WithDynamicValue_ShouldReturnValue_WhenFound()
  {
    var template = CompileTemplate( "$macro$" );
    var values = template.CreateMacroValues().SetMacro( "macro", _ => _timeProvider.GetLocalNow().ToString() );

    values.GetMacroValue( "macro" ).Should().Be( "10/20/2024 10:30:00 AM -07:00" );
  }

  [Fact]
  public void GetMacroValue_WithDynamicValueAndArgument_ShouldReturnValue()
  {
    var template = CompileTemplate( "$macro$" );

    var values = template.CreateMacroValues()
                         .SetMacro( "macro", arg => _timeProvider.GetLocalNow().ToString( arg.ToString() ) );

    values.GetMacroValue( "macro", "yyyyMMdd" ).Should().Be( "20241020" );
  }

  [Fact]
  public void GetMacroValue_WithStaticValue_ShouldBeCaseInsensitive()
  {
    var template = CompileTemplate( "$macro$" );

    var values = template.CreateMacroValues().SetMacro( "macro", "value" );
    values.GetMacroValue( "MaCrO" ).Should().Be( "value" );
  }

  [Fact]
  public void GetMacroValue_WithStaticValue_ShouldReturnNull_WhenNotFound()
  {
    var template = CompileTemplate( "$macro$" );

    var values = template.CreateMacroValues().SetMacro( "macro", "value" );
    values.GetMacroValue( "Unknown" ).Should().BeNull();
  }

  [Fact]
  public void GetMacroValue_WithStaticValue_ShouldReturnValue_WhenFound()
  {
    var template = CompileTemplate( "$macro$" );

    var values = template.CreateMacroValues().SetMacro( "macro", "value" );
    values.GetMacroValue( "macro" ).Should().Be( "value" );
  }

  [Fact]
  public void SetMacro_Generator_IgnoresUnknownMacro()
  {
    var template = CompileTemplate( "Hello $macroA$" );

    var values = new TemplateMacroValues( template )
      .SetMacro( "macroB", arg => "baz" );
    values.GetMacroValue( "macroB" ).Should().BeNull();
  }

  [Fact]
  public void SetMacro_String_IgnoresUnknownMacro()
  {
    var template = CompileTemplate( "Hello $macroA$" );

    var values = new TemplateMacroValues( template )
      .SetMacro( "macroB", "baz" );
    values.GetMacroValue( "macroB" ).Should().BeNull();
  }

  [Fact]
  public void SetMacros_GeneratorDictionary_SetsAllMacros()
  {
    var template = CompileTemplate( "$a$ $b$" );

    var values = new TemplateMacroValues( template )
      .SetMacros(
        [
          new KeyValuePair<string, MacroValueGenerator>( "a", arg => "A" ),
          new KeyValuePair<string, MacroValueGenerator>( "b", arg => "B" )
        ]
      );

    values.GetMacroValue( "a" ).Should().Be( "A" );
    values.GetMacroValue( "b" ).Should().Be( "B" );
  }

  [Fact]
  public void SetMacros_StringDictionary_SetsAllMacros()
  {
    var template = CompileTemplate( "$a$ $b$" );

    var values = new TemplateMacroValues( template )
      .SetMacros( [new KeyValuePair<string, string>( "a", "1" ), new KeyValuePair<string, string>( "b", "2" )] );

    values.GetMacroValue( "a" ).Should().Be( "1" );
    values.GetMacroValue( "b" ).Should().Be( "2" );
  }

  #endregion

  #region Implementation

  private static Template CompileTemplate(
    string templateText )
  {
    var compiler = new TemplateCompiler();
    return compiler.Compile( templateText );
  }

  #endregion
}

#endif
