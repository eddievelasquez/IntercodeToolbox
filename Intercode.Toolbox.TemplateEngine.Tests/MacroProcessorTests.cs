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

    var context = new MacroProcessorContext();
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

    var context = new MacroProcessorContext();
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

    var context = new MacroProcessorContext();
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

    var context = new MacroProcessorContext();
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

    var context = new MacroProcessorContext();
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

    var context = new MacroProcessorContext();
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

    var context = new MacroProcessorContext();
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

    var context = new MacroProcessorContext();
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

    var context = new MacroProcessorContext();
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

    var context = new MacroProcessorContext();
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

    var context = new MacroProcessorContext();
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

    var context = new MacroProcessorContext();
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

    var context = new MacroProcessorContext();
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

    var context = new MacroProcessorContext();
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

    var context = new MacroProcessorContext();
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

    var context = new MacroProcessorContext();
    context.AddMacro( "who", "World" );

    var template = TemplateCompiler.Compile( context, Text );
    var writer = new StringWriter();
    MacroProcessor.ProcessMacros( template, writer );

    writer.ToString().Should().Be( "Hello, World!" );
  }

  [Fact]
  public void ProcessMacros_WithStringBuilder_ShouldProcessConstantOnlyTemplate()
  {
    const string Text = "Just text.";
    var template = TemplateCompiler.Compile( new MacroProcessorContext(), Text );
    var builder = new StringBuilder();
    MacroProcessor.ProcessMacros( template, builder );
    builder.ToString().Should().Be( Text );
  }

  [Fact]
  public void ProcessMacros_WithStringBuilder_ShouldProcessDelimiterOnlyTemplate()
  {
    const string Text = "$$";
    var template = TemplateCompiler.Compile( new MacroProcessorContext(), Text );
    var builder = new StringBuilder();
    MacroProcessor.ProcessMacros( template, builder );
    builder.ToString().Should().Be( "$" );
  }

  [Fact]
  public void ProcessMacros_WithStringBuilder_ShouldProcessUnclosedMacroAsConstant()
  {
    const string Text = "$macro";
    var template = TemplateCompiler.Compile( new MacroProcessorContext(), Text );
    var builder = new StringBuilder();
    MacroProcessor.ProcessMacros( template, builder );
    builder.ToString().Should().Be( Text );
  }

  [Fact]
  public void ProcessMacros_WithStringBuilder_ShouldWriteMacroTextIfValueIsNull()
  {
    const string Text = "Hello, $who$!";
    var context = new MacroProcessorContext(); // No macro value set
    var template = TemplateCompiler.Compile( context, Text );
    var builder = new StringBuilder();
    MacroProcessor.ProcessMacros( template, builder );
    builder.ToString().Should().Be( "Hello, !" );
  }

  [Fact]
  public void ProcessMacros_WithStringBuilder_ShouldWriteExceptionMessageIfMacroThrows()
  {
    const string Text = "Hello, $fail$!";
    var context = new MacroProcessorContext();
    context.AddMacro( "fail", _ => throw new InvalidOperationException( "fail macro error" ) );
    var template = TemplateCompiler.Compile( context, Text );
    var builder = new StringBuilder();
    MacroProcessor.ProcessMacros( template, builder );
    builder.ToString().Should().Contain( "fail macro error" );
  }

  [Fact]
  public void ProcessMacros_WithStringWriter_ShouldProcessConstantOnlyTemplate()
  {
    const string Text = "Just text.";
    var template = TemplateCompiler.Compile( new MacroProcessorContext(), Text );
    var writer = new StringWriter();
    MacroProcessor.ProcessMacros( template, writer );
    writer.ToString().Should().Be( Text );
  }

  [Fact]
  public void ProcessMacros_WithStringWriter_ShouldProcessDelimiterOnlyTemplate()
  {
    const string Text = "$$";
    var template = TemplateCompiler.Compile( new MacroProcessorContext(), Text );
    var writer = new StringWriter();
    MacroProcessor.ProcessMacros( template, writer );
    writer.ToString().Should().Be( "$" );
  }

  [Fact]
  public void ProcessMacros_WithStringWriter_ShouldProcessUnclosedMacroAsConstant()
  {
    const string Text = "$macro";
    var template = TemplateCompiler.Compile( new MacroProcessorContext(), Text );
    var writer = new StringWriter();
    MacroProcessor.ProcessMacros( template, writer );
    writer.ToString().Should().Be( Text );
  }

  [Fact]
  public void ProcessMacros_WithStringWriter_ShouldWriteMacroTextIfValueIsNull()
  {
    const string Text = "Hello, $who$!";
    var context = new MacroProcessorContext(); // No macro value set
    var template = TemplateCompiler.Compile( context, Text );
    var writer = new StringWriter();
    MacroProcessor.ProcessMacros( template, writer );
    writer.ToString().Should().Be( "Hello, who!" );
  }

  [Fact]
  public void ProcessMacros_WithStringWriter_ShouldWriteExceptionMessageIfMacroThrows()
  {
    const string Text = "Hello, $fail$!";
    var context = new MacroProcessorContext();
    context.AddMacro( "fail", _ => throw new InvalidOperationException( "fail macro error" ) );
    var template = TemplateCompiler.Compile( context, Text );
    var writer = new StringWriter();
    MacroProcessor.ProcessMacros( template, writer );
    writer.ToString().Should().Contain( "fail macro error" );
  }

  #endregion
}
