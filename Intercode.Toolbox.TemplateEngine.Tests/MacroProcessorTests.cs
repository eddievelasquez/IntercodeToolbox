// Module Name: MacroProcessorTests.cs
// Author:      Eduardo Velasquez
// Copyright (c) 2024, Intercode Consulting, Inc.

namespace Intercode.Toolbox.TemplateEngine.Tests;

using FluentAssertions;
using Microsoft.Extensions.Time.Testing;

public class MacroProcessorTests
{
  #region Fields

  private readonly FakeTimeProvider _timeProvider = new (
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

  [Fact]
  public void GetMacroValue_WithDynamicValue_ShouldBeCaseInsensitive()
  {
    var processor = new MacroProcessorBuilder().AddMacro( "macro", _ => _timeProvider.GetLocalNow().ToString() )
                                               .Build();

    processor.GetMacroValue( "MaCrO" ).Should().Be( "10/20/2024 10:30:00 AM -07:00" );
  }

  [Fact]
  public void GetMacroValue_WithDynamicValue_ShouldReturnNull_WhenNotFound()
  {
    var processor = new MacroProcessorBuilder().AddMacro( "macro", _ => _timeProvider.GetLocalNow().ToString() )
                                               .Build();

    processor.GetMacroValue( "Unknown" ).Should().BeNull();
  }

  [Fact]
  public void GetMacroValue_WithDynamicValue_ShouldReturnValue_WhenFound()
  {
    var processor = new MacroProcessorBuilder().AddMacro( "macro", _ => _timeProvider.GetLocalNow().ToString() )
                                               .Build();

    processor.GetMacroValue( "macro" ).Should().Be( "10/20/2024 10:30:00 AM -07:00" );
  }

  [Fact]
  public void GetMacroValue_WithDynamicValueAndArgument_ShouldReturnValue()
  {
    var processor = new MacroProcessorBuilder()
                    .AddMacro( "macro", arg => _timeProvider.GetLocalNow().ToString( arg.ToString() ) )
                    .Build();

    processor.GetMacroValue( "macro", "yyyyMMdd" ).Should().Be( "20241020" );
  }

  [Fact]
  public void GetMacroValue_WithStaticValue_ShouldBeCaseInsensitive()
  {
    var processor = new MacroProcessorBuilder().AddMacro( "macro", "value" )
                                               .Build();

    processor.GetMacroValue( "MaCrO" ).Should().Be( "value" );
  }

  [Fact]
  public void GetMacroValue_WithStaticValue_ShouldReturnNull_WhenNotFound()
  {
    var processor = new MacroProcessorBuilder().AddMacro( "macro", "value" )
                                               .Build();

    processor.GetMacroValue( "Unknown" ).Should().BeNull();
  }

  [Fact]
  public void GetMacroValue_WithStaticValue_ShouldReturnValue_WhenFound()
  {
    var processor = new MacroProcessorBuilder().AddMacro( "macro", "value" )
                                               .Build();

    processor.GetMacroValue( "macro" ).Should().Be( "value" );
  }

  [Fact]
  public void ProcessMacros_ShouldReplaceEscapedDelimiters()
  {
    const string Text = "Give me the $$!";
    var template = new TemplateCompiler().Compile( Text );
    var processor = new MacroProcessorBuilder().AddMacro( "who", "World" )
                                               .Build();
    var writer = new StringWriter();
    processor.ProcessMacros( template, writer );

    writer.ToString().Should().Be( "Give me the $!" );
  }

  [Fact]
  public void ProcessMacros_ShouldReplaceMacrosWithDynamicValues()
  {
    const string Text = "Timestamp: $now$";
    var template = new TemplateCompiler().Compile( Text );
    var processor = new MacroProcessorBuilder().AddMacro( "now", _ => _timeProvider.GetLocalNow().ToString() )
                                               .Build();
    var writer = new StringWriter();
    processor.ProcessMacros( template, writer );

    writer.ToString().Should().Be( "Timestamp: 10/20/2024 10:30:00 AM -07:00" );
  }

  [Fact]
  public void ProcessMacros_ShouldReplaceMacrosWithDynamicValuesAndArgument()
  {
    const string Text = "Timestamp: $now:yyyyMMdd$";
    var template = new TemplateCompiler().Compile( Text );
    var processor = new MacroProcessorBuilder()
                    .AddMacro( "now", arg => _timeProvider.GetLocalNow().ToString( arg.ToString() ) )
                    .Build();
    var writer = new StringWriter();
    processor.ProcessMacros( template, writer );

    writer.ToString().Should().Be( "Timestamp: 20241020" );
  }

  [Fact]
  public void ProcessMacros_ShouldReplaceMacrosWithStaticValues()
  {
    const string Text = "Hello, $who$!";
    var template = new TemplateCompiler().Compile( Text );
    var processor = new MacroProcessorBuilder().AddMacro( "who", "World" )
                                               .Build();
    var writer = new StringWriter();
    processor.ProcessMacros( template, writer );

    writer.ToString().Should().Be( "Hello, World!" );
  }

  #endregion
}
