// Module Name: MacroProcessorTests.cs
// Author:      Eduardo Velasquez
// Copyright (c) 2025, Intercode Consulting, Inc.

#pragma warning disable CS0618 // Type or member is obsolete

namespace Intercode.Toolbox.TemplateEngine.Tests;

using System.Text;
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
  public void ProcessMacros_WithStringBuilder_ShouldReplaceEscapedDelimiters()
  {
    const string Text = "Give me the $$!";
    var template = new TemplateCompiler().Compile( Text );

    var processor = new MacroProcessorBuilder().AddMacro( "who", "World" )
                                               .Build();
    var builder = new StringBuilder();
    processor.ProcessMacros( template, builder );

    builder.ToString().Should().Be( "Give me the $!" );
  }

  [Fact]
  public void ProcessMacros_WithStringBuilder_ShouldReplaceMacrosWithDynamicValues()
  {
    const string Text = "Timestamp: $now$";
    var template = new TemplateCompiler().Compile( Text );

    var processor = new MacroProcessorBuilder().AddMacro( "now", _ => _timeProvider.GetLocalNow().ToString() )
                                               .Build();
    var builder = new StringBuilder();
    processor.ProcessMacros( template, builder );

    builder.ToString().Should().Be( "Timestamp: 10/20/2024 10:30:00 AM -07:00" );
  }

  [Fact]
  public void ProcessMacros_WithStringBuilder_ShouldReplaceMacrosWithDynamicValuesAndArgument()
  {
    const string Text = "Timestamp: $now:yyyyMMdd$";
    var template = new TemplateCompiler().Compile( Text );

    var processor = new MacroProcessorBuilder()
                    .AddMacro( "now", arg => _timeProvider.GetLocalNow().ToString( arg.ToString() ) )
                    .Build();
    var builder = new StringBuilder();
    processor.ProcessMacros( template, builder );

    builder.ToString().Should().Be( "Timestamp: 20241020" );
  }

  [Fact]
  public void ProcessMacros_WithStringBuilder_ShouldReplaceMacrosWithStaticValues()
  {
    const string Text = "Hello, $who$!";
    var template = new TemplateCompiler().Compile( Text );

    var processor = new MacroProcessorBuilder().AddMacro( "who", "World" )
                                               .Build();
    var builder = new StringBuilder();
    processor.ProcessMacros( template, builder );

    builder.ToString().Should().Be( "Hello, World!" );
  }

  [Fact]
  public void ProcessMacros_WithStringBuilder_WithTemplateMacroValues_ShouldReplaceEscapedDelimiters()
  {
    const string Text = "Give me the $$!";
    var template = new TemplateCompiler().Compile( Text );
    var templateValues = template.CreateMacroValues().SetMacro( "who", "World" );
    var processor = new MacroProcessor();
    var builder = new StringBuilder();
    processor.ProcessMacros( templateValues, builder );

    builder.ToString().Should().Be( "Give me the $!" );
  }

  [Fact]
  public void ProcessMacros_WithStringBuilder_WithTemplateMacroValues_ShouldReplaceMacrosWithDynamicValues()
  {
    const string Text = "Timestamp: $now$";
    var template = new TemplateCompiler().Compile( Text );
    var templateValues = template.CreateMacroValues().SetMacro( "now", _ => _timeProvider.GetLocalNow().ToString() );
    var processor = new MacroProcessor();
    var builder = new StringBuilder();
    processor.ProcessMacros( templateValues, builder );

    builder.ToString().Should().Be( "Timestamp: 10/20/2024 10:30:00 AM -07:00" );
  }

  [Fact]
  public void ProcessMacros_WithStringBuilder_WithTemplateMacroValues_ShouldReplaceMacrosWithDynamicValuesAndArgument()
  {
    const string Text = "Timestamp: $now:yyyyMMdd$";
    var template = new TemplateCompiler().Compile( Text );

    var templateValues = template.CreateMacroValues()
                                 .SetMacro( "now", arg => _timeProvider.GetLocalNow().ToString( arg.ToString() ) );
    var processor = new MacroProcessor();
    var builder = new StringBuilder();
    processor.ProcessMacros( templateValues, builder );

    builder.ToString().Should().Be( "Timestamp: 20241020" );
  }

  [Fact]
  public void ProcessMacros_WithStringBuilder_WithTemplateMacroValues_ShouldReplaceMacrosWithStaticValues()
  {
    const string Text = "Hello, $who$!";
    var template = new TemplateCompiler().Compile( Text );
    var templateValues = template.CreateMacroValues().SetMacro( "who", "World" );
    var processor = new MacroProcessor();
    var builder = new StringBuilder();
    processor.ProcessMacros( templateValues, builder );

    builder.ToString().Should().Be( "Hello, World!" );
  }

  [Fact]
  public void ProcessMacros_WithStringWriter_ShouldReplaceEscapedDelimiters()
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
  public void ProcessMacros_WithStringWriter_ShouldReplaceMacrosWithDynamicValues()
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
  public void ProcessMacros_WithStringWriter_ShouldReplaceMacrosWithDynamicValuesAndArgument()
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
  public void ProcessMacros_WithStringWriter_ShouldReplaceMacrosWithStaticValues()
  {
    const string Text = "Hello, $who$!";
    var template = new TemplateCompiler().Compile( Text );

    var processor = new MacroProcessorBuilder().AddMacro( "who", "World" )
                                               .Build();
    var writer = new StringWriter();
    processor.ProcessMacros( template, writer );

    writer.ToString().Should().Be( "Hello, World!" );
  }

  [Fact]
  public void ProcessMacros_WithStringWriter_WithTemplateMacroValues_ShouldReplaceEscapedDelimiters()
  {
    const string Text = "Give me the $$!";
    var template = new TemplateCompiler().Compile( Text );
    var templateValues = template.CreateMacroValues().SetMacro( "who", "World" );
    var processor = new MacroProcessor();
    var writer = new StringWriter();
    processor.ProcessMacros( templateValues, writer );

    writer.ToString().Should().Be( "Give me the $!" );
  }

  [Fact]
  public void ProcessMacros_WithStringWriter_WithTemplateMacroValues_ShouldReplaceMacrosWithDynamicValues()
  {
    const string Text = "Timestamp: $now$";
    var template = new TemplateCompiler().Compile( Text );
    var templateValues = template.CreateMacroValues().SetMacro( "now", _ => _timeProvider.GetLocalNow().ToString() );
    var processor = new MacroProcessor();
    var writer = new StringWriter();
    processor.ProcessMacros( templateValues, writer );

    writer.ToString().Should().Be( "Timestamp: 10/20/2024 10:30:00 AM -07:00" );
  }

  [Fact]
  public void ProcessMacros_WithStringWriter_WithTemplateMacroValues_ShouldReplaceMacrosWithDynamicValuesAndArgument()
  {
    const string Text = "Timestamp: $now:yyyyMMdd$";
    var template = new TemplateCompiler().Compile( Text );

    var templateValues = template.CreateMacroValues()
                                 .SetMacro( "now", arg => _timeProvider.GetLocalNow().ToString( arg.ToString() ) );
    var processor = new MacroProcessor();
    var writer = new StringWriter();
    processor.ProcessMacros( templateValues, writer );

    writer.ToString().Should().Be( "Timestamp: 20241020" );
  }

  [Fact]
  public void ProcessMacros_WithStringWriter_WithTemplateMacroValues_ShouldReplaceMacrosWithStaticValues()
  {
    const string Text = "Hello, $who$!";
    var template = new TemplateCompiler().Compile( Text );
    var templateValues = template.CreateMacroValues().SetMacro( "who", "World" );
    var processor = new MacroProcessor();
    var writer = new StringWriter();
    processor.ProcessMacros( templateValues, writer );

    writer.ToString().Should().Be( "Hello, World!" );
  }

  #endregion
}
