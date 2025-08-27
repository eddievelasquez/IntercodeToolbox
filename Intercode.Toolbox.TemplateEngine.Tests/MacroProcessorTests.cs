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
  public void ProcessMacros_WithStringBuilder_ShouldReplaceEscapedDelimiters()
  {
    const string Text = "Give me the $$!";

    var context = new TemplateContext();
    context.AddMacro( "who", "World" );

    var template = TemplateCompiler.Compile( context, Text );

    var builder = new StringBuilder();
    MacroProcessor.ProcessMacros( template, builder );

    builder.ToString().Should().Be( "Give me the $!" );
  }

  [Fact]
  public void ProcessMacros_WithStringBuilder_ShouldReplaceMacrosWithDynamicValues()
  {
    const string Text = "Timestamp: $now$";

    var context = new TemplateContext();
    context.AddMacro( "now", _ => _timeProvider.GetLocalNow().ToString() );

    var template = TemplateCompiler.Compile( context, Text );

    var builder = new StringBuilder();
    MacroProcessor.ProcessMacros( template, builder );

    builder.ToString().Should().Be( "Timestamp: 10/20/2024 10:30:00 AM -07:00" );
  }

  [Fact]
  public void ProcessMacros_WithStringBuilder_ShouldReplaceMacrosWithDynamicValuesAndArgument()
  {
    const string Text = "Timestamp: $now:yyyyMMdd$";

    var context = new TemplateContext();
    context.AddMacro( "now", arg => _timeProvider.GetLocalNow().ToString( arg.ToString() ) );

    var template = TemplateCompiler.Compile( context, Text );

    var builder = new StringBuilder();
    MacroProcessor.ProcessMacros( template, builder );

    builder.ToString().Should().Be( "Timestamp: 20241020" );
  }

  [Fact]
  public void ProcessMacros_WithStringBuilder_ShouldReplaceMacrosWithStaticValues()
  {
    const string Text = "Hello, $who$!";

    var context = new TemplateContext();
    context.AddMacro( "who", "World" );

    var template = TemplateCompiler.Compile( context, Text );

    var builder = new StringBuilder();
    MacroProcessor.ProcessMacros( template, builder );

    builder.ToString().Should().Be( "Hello, World!" );
  }

  [Fact]
  public void ProcessMacros_WithStringBuilder_WithTemplateMacroValues_ShouldReplaceEscapedDelimiters()
  {
    const string Text = "Give me the $$!";

    var context = new TemplateContext();
    context.AddMacro( "who", "World" );

    var template = TemplateCompiler.Compile( context, Text );
    var builder = new StringBuilder();
    MacroProcessor.ProcessMacros( template, builder );

    builder.ToString().Should().Be( "Give me the $!" );
  }

  [Fact]
  public void ProcessMacros_WithStringBuilder_WithTemplateMacroValues_ShouldReplaceMacrosWithDynamicValues()
  {
    const string Text = "Timestamp: $now$";

    var context = new TemplateContext();
    context.AddMacro( "now", _ => _timeProvider.GetLocalNow().ToString() );

    var template = TemplateCompiler.Compile( context, Text );
    var builder = new StringBuilder();
    MacroProcessor.ProcessMacros( template, builder );

    builder.ToString().Should().Be( "Timestamp: 10/20/2024 10:30:00 AM -07:00" );
  }

  [Fact]
  public void ProcessMacros_WithStringBuilder_WithTemplateMacroValues_ShouldReplaceMacrosWithDynamicValuesAndArgument()
  {
    const string Text = "Timestamp: $now:yyyyMMdd$";

    var context = new TemplateContext();
    context.AddMacro( "now", arg => _timeProvider.GetLocalNow().ToString( arg.ToString() ) );

    var template = TemplateCompiler.Compile( context, Text );
    var builder = new StringBuilder();
    MacroProcessor.ProcessMacros( template, builder );

    builder.ToString().Should().Be( "Timestamp: 20241020" );
  }

  [Fact]
  public void ProcessMacros_WithStringBuilder_WithTemplateMacroValues_ShouldReplaceMacrosWithStaticValues()
  {
    const string Text = "Hello, $who$!";

    var context = new TemplateContext();
    context.AddMacro( "who", "World" );

    var template = TemplateCompiler.Compile( context, Text );
    var builder = new StringBuilder();
    MacroProcessor.ProcessMacros( template, builder );

    builder.ToString().Should().Be( "Hello, World!" );
  }

  [Fact]
  public void ProcessMacros_WithStringWriter_ShouldReplaceEscapedDelimiters()
  {
    const string Text = "Give me the $$!";

    var context = new TemplateContext();
    context.AddMacro( "who", "World" );

    var template = TemplateCompiler.Compile( context, Text );
    var writer = new StringWriter();
    MacroProcessor.ProcessMacros( template, writer );

    writer.ToString().Should().Be( "Give me the $!" );
  }

  [Fact]
  public void ProcessMacros_WithStringWriter_ShouldReplaceMacrosWithDynamicValues()
  {
    const string Text = "Timestamp: $now$";

    var context = new TemplateContext();
    context.AddMacro( "now", _ => _timeProvider.GetLocalNow().ToString() );

    var template = TemplateCompiler.Compile( context, Text );

    var writer = new StringWriter();
    MacroProcessor.ProcessMacros( template, writer );

    writer.ToString().Should().Be( "Timestamp: 10/20/2024 10:30:00 AM -07:00" );
  }

  [Fact]
  public void ProcessMacros_WithStringWriter_ShouldReplaceMacrosWithDynamicValuesAndArgument()
  {
    const string Text = "Timestamp: $now:yyyyMMdd$";

    var context = new TemplateContext();
    context.AddMacro( "now", arg => _timeProvider.GetLocalNow().ToString( arg.ToString() ) );

    var template = TemplateCompiler.Compile( context, Text );

    var writer = new StringWriter();
    MacroProcessor.ProcessMacros( template, writer );

    writer.ToString().Should().Be( "Timestamp: 20241020" );
  }

  [Fact]
  public void ProcessMacros_WithStringWriter_ShouldReplaceMacrosWithStaticValues()
  {
    const string Text = "Hello, $who$!";

    var context = new TemplateContext();
    context.AddMacro( "who", "World" );

    var template = TemplateCompiler.Compile( context, Text );

    var writer = new StringWriter();
    MacroProcessor.ProcessMacros( template, writer );

    writer.ToString().Should().Be( "Hello, World!" );
  }

  [Fact]
  public void ProcessMacros_WithStringWriter_WithTemplateMacroValues_ShouldReplaceEscapedDelimiters()
  {
    const string Text = "Give me the $$!";

    var context = new TemplateContext();
    context.AddMacro( "who", "World" );

    var template = TemplateCompiler.Compile( context, Text );
    var writer = new StringWriter();
    MacroProcessor.ProcessMacros( template, writer );

    writer.ToString().Should().Be( "Give me the $!" );
  }

  [Fact]
  public void ProcessMacros_WithStringWriter_WithTemplateMacroValues_ShouldReplaceMacrosWithDynamicValues()
  {
    const string Text = "Timestamp: $now$";

    var context = new TemplateContext();
    context.AddMacro( "now", _ => _timeProvider.GetLocalNow().ToString() );

    var template = TemplateCompiler.Compile( context, Text );
    var writer = new StringWriter();
    MacroProcessor.ProcessMacros( template, writer );

    writer.ToString().Should().Be( "Timestamp: 10/20/2024 10:30:00 AM -07:00" );
  }

  [Fact]
  public void ProcessMacros_WithStringWriter_WithTemplateMacroValues_ShouldReplaceMacrosWithDynamicValuesAndArgument()
  {
    const string Text = "Timestamp: $now:yyyyMMdd$";

    var context = new TemplateContext();
    context.AddMacro( "now", arg => _timeProvider.GetLocalNow().ToString( arg.ToString() ) );

    var template = TemplateCompiler.Compile( context, Text );

    var writer = new StringWriter();
    MacroProcessor.ProcessMacros( template, writer );

    writer.ToString().Should().Be( "Timestamp: 20241020" );
  }

  [Fact]
  public void ProcessMacros_WithStringWriter_WithTemplateMacroValues_ShouldReplaceMacrosWithStaticValues()
  {
    const string Text = "Hello, $who$!";

    var context = new TemplateContext();
    context.AddMacro( "who", "World" );

    var template = TemplateCompiler.Compile( context, Text );
    var writer = new StringWriter();
    MacroProcessor.ProcessMacros( template, writer );

    writer.ToString().Should().Be( "Hello, World!" );
  }

  #endregion
}
