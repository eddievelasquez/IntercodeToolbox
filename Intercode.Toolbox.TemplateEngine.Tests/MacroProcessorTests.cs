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

  // Edge case: Macro not defined in MacroTable
  [Fact]
  public void ProcessMacros_ShouldIgnoreUndefinedMacros()
  {
    var values = CreateStaticMacroValues( ( "unused", "unused value" ) );
    var template = TemplateCompiler.Compile( values.MacroTable, "Hello, $undefined$!" );
    var builder = new StringBuilder();
    MacroProcessor.ProcessMacros( template, values, builder );

    // Should treat as empty since slot is -1
    builder.ToString().Should().Be( "Hello, !" );
  }

  // Edge case: Macro with argument but no generator
  [Fact]
  public void ProcessMacros_ShouldTreatMacroWithArgumentButNoGeneratorAsEmpty()
  {
    var values = CreateStaticMacroValues( ( "who", null ) );
    var template = TemplateCompiler.Compile( values.MacroTable, "Hello, $who:arg$!" );
    var builder = new StringBuilder();
    MacroProcessor.ProcessMacros( template, values, builder );
    builder.ToString().Should().Be( "Hello, !" );
  }

  // Edge case: Macro defined but value explicitly set to null
  [Fact]
  public void ProcessMacros_ShouldTreatNullMacroValueAsEmpty()
  {
    var values = CreateStaticMacroValues( ( "who", null ) );
    var template = TemplateCompiler.Compile( values.MacroTable, "Hello, $who$!" );
    var builder = new StringBuilder();
    MacroProcessor.ProcessMacros( template, values, builder );
    builder.ToString().Should().Be( "Hello, !" );
  }

  [Fact]
  public void ProcessMacros_WithStringBuilder_ShouldProcessConstantOnlyTemplate()
  {
    var values = CreateStaticMacroValues();
    var template = TemplateCompiler.Compile( values.MacroTable, "Just text." );
    var builder = new StringBuilder();
    MacroProcessor.ProcessMacros( template, values, builder );
    builder.ToString().Should().Be( "Just text." );
  }

  [Fact]
  public void ProcessMacros_WithStringBuilder_ShouldProcessDelimiterOnlyTemplate()
  {
    var values = CreateStaticMacroValues();
    var template = TemplateCompiler.Compile( values.MacroTable, "$$" );
    var builder = new StringBuilder();
    MacroProcessor.ProcessMacros( template, values, builder );
    builder.ToString().Should().Be( "$" );
  }

  [Fact]
  public void ProcessMacros_WithStringBuilder_ShouldProcessUnclosedMacroAsConstant()
  {
    var values = CreateStaticMacroValues();
    var template = TemplateCompiler.Compile( values.MacroTable, "$macro" );
    var builder = new StringBuilder();
    MacroProcessor.ProcessMacros( template, values, builder );
    builder.ToString().Should().Be( "$macro" );
  }

  [Fact]
  public void ProcessMacros_WithStringBuilder_ShouldReplaceEscapedDelimiters()
  {
    var values = CreateStaticMacroValues();
    var template = TemplateCompiler.Compile( values.MacroTable, "Give me the $$!" );
    var builder = new StringBuilder();
    MacroProcessor.ProcessMacros( template, values, builder );
    builder.ToString().Should().Be( "Give me the $!" );
  }

  [Fact]
  public void ProcessMacros_WithStringBuilder_ShouldReplaceMacrosWithDynamicValues()
  {
    var values = CreateDynamicMacroValues( ( "now", _ => _timeProvider.GetLocalNow().ToString() ) );
    var template = TemplateCompiler.Compile( values.MacroTable, "Timestamp: $now$" );
    var builder = new StringBuilder();
    MacroProcessor.ProcessMacros( template, values, builder );
    builder.ToString().Should().Be( "Timestamp: 10/20/2024 10:30:00 AM -07:00" );
  }

  [Fact]
  public void ProcessMacros_WithStringBuilder_ShouldReplaceMacrosWithDynamicValuesAndArgument()
  {
    var values = CreateDynamicMacroValues( ( "now", arg => _timeProvider.GetLocalNow().ToString( arg.ToString() ) ) );
    var template = TemplateCompiler.Compile( values.MacroTable, "Timestamp: $now:yyyyMMdd$" );
    var builder = new StringBuilder();
    MacroProcessor.ProcessMacros( template, values, builder );
    builder.ToString().Should().Be( "Timestamp: 20241020" );
  }

  [Fact]
  public void ProcessMacros_WithStringBuilder_ShouldReplaceMacrosWithStaticValues()
  {
    var values = CreateStaticMacroValues( ( "who", "World" ) );
    var template = TemplateCompiler.Compile( values.MacroTable, "Hello, $who$!" );
    var builder = new StringBuilder();
    MacroProcessor.ProcessMacros( template, values, builder );
    builder.ToString().Should().Be( "Hello, World!" );
  }

  [Fact]
  public void ProcessMacros_WithStringBuilder_ShouldWriteExceptionMessageIfMacroThrows()
  {
    var values = CreateDynamicMacroValues( ( "fail", _ => throw new InvalidOperationException( "fail macro error" ) ) );
    var template = TemplateCompiler.Compile( values.MacroTable, "Hello, $fail$!" );
    var builder = new StringBuilder();
    MacroProcessor.ProcessMacros( template, values, builder );
    builder.ToString().Should().Contain( "fail macro error" );
  }

  [Fact]
  public void ProcessMacros_WithStringBuilder_ShouldWriteMacroTextIfValueIsNull()
  {
    var values = CreateStaticMacroValues( ( "who", null ) );
    var template = TemplateCompiler.Compile( values.MacroTable, "Hello, $who$!" );
    var builder = new StringBuilder();
    MacroProcessor.ProcessMacros( template, values, builder );
    builder.ToString().Should().Be( "Hello, !" );
  }

  [Fact]
  public void ProcessMacros_WithStringWriter_ShouldProcessConstantOnlyTemplate()
  {
    var values = CreateStaticMacroValues();
    var template = TemplateCompiler.Compile( values.MacroTable, "Just text." );
    var writer = new StringWriter();
    MacroProcessor.ProcessMacros( template, values, writer );
    writer.ToString().Should().Be( "Just text." );
  }

  [Fact]
  public void ProcessMacros_WithStringWriter_ShouldProcessDelimiterOnlyTemplate()
  {
    var values = CreateStaticMacroValues();
    var template = TemplateCompiler.Compile( values.MacroTable, "$$" );
    var writer = new StringWriter();
    MacroProcessor.ProcessMacros( template, values, writer );
    writer.ToString().Should().Be( "$" );
  }

  [Fact]
  public void ProcessMacros_WithStringWriter_ShouldProcessUnclosedMacroAsConstant()
  {
    var values = CreateStaticMacroValues();
    var template = TemplateCompiler.Compile( values.MacroTable, "$macro" );
    var writer = new StringWriter();
    MacroProcessor.ProcessMacros( template, values, writer );
    writer.ToString().Should().Be( "$macro" );
  }

  [Fact]
  public void ProcessMacros_WithStringWriter_ShouldReplaceEscapedDelimiters()
  {
    var values = CreateStaticMacroValues();
    var template = TemplateCompiler.Compile( values.MacroTable, "Give me the $$!" );
    var writer = new StringWriter();
    MacroProcessor.ProcessMacros( template, values, writer );
    writer.ToString().Should().Be( "Give me the $!" );
  }

  [Fact]
  public void ProcessMacros_WithStringWriter_ShouldReplaceMacrosWithDynamicValues()
  {
    var values = CreateDynamicMacroValues( ( "now", _ => _timeProvider.GetLocalNow().ToString() ) );
    var template = TemplateCompiler.Compile( values.MacroTable, "Timestamp: $now$" );
    var writer = new StringWriter();
    MacroProcessor.ProcessMacros( template, values, writer );
    writer.ToString().Should().Be( "Timestamp: 10/20/2024 10:30:00 AM -07:00" );
  }

  [Fact]
  public void ProcessMacros_WithStringWriter_ShouldReplaceMacrosWithDynamicValuesAndArgument()
  {
    var values = CreateDynamicMacroValues( ( "now", arg => _timeProvider.GetLocalNow().ToString( arg.ToString() ) ) );
    var template = TemplateCompiler.Compile( values.MacroTable, "Timestamp: $now:yyyyMMdd$" );
    var writer = new StringWriter();
    MacroProcessor.ProcessMacros( template, values, writer );
    writer.ToString().Should().Be( "Timestamp: 20241020" );
  }

  [Fact]
  public void ProcessMacros_WithStringWriter_ShouldReplaceMacrosWithStaticValues()
  {
    var values = CreateStaticMacroValues( ( "who", "World" ) );
    var template = TemplateCompiler.Compile( values.MacroTable, "Hello, $who$!" );
    var writer = new StringWriter();
    MacroProcessor.ProcessMacros( template, values, writer );
    writer.ToString().Should().Be( "Hello, World!" );
  }

  [Fact]
  public void ProcessMacros_WithStringWriter_ShouldWriteExceptionMessageIfMacroThrows()
  {
    var values = CreateDynamicMacroValues( ( "fail", _ => throw new InvalidOperationException( "fail macro error" ) ) );
    var template = TemplateCompiler.Compile( values.MacroTable, "Hello, $fail$!" );
    var writer = new StringWriter();
    MacroProcessor.ProcessMacros( template, values, writer );
    writer.ToString().Should().Contain( "fail macro error" );
  }

  [Fact]
  public void ProcessMacros_WithStringWriter_ShouldWriteMacroTextIfValueIsNull()
  {
    var values = CreateStaticMacroValues( ( "who", null ) );
    var template = TemplateCompiler.Compile( values.MacroTable, "Hello, $who$!" );
    var writer = new StringWriter();
    MacroProcessor.ProcessMacros( template, values, writer );
    writer.ToString().Should().Be( "Hello, !" );
  }

  #endregion

  #region Implementation

  private static MacroValues CreateDynamicMacroValues(
    params (string name, MacroValueGenerator? generator)[] macros )
  {
    var builder = new MacroTableBuilder();

    foreach( var (name, _) in macros )
    {
      builder.Declare( name );
    }

    if( macros.Length == 0 )
    {
      // Ensure at least one macro is declared to avoid empty MacroTable
      builder.Declare( "dummy" );
    }

    var macroTable = builder.Build();
    var values = macroTable.CreateValues();

    foreach( var (name, generator) in macros )
    {
      values.SetValue( name, generator );
    }

    return values;
  }

  private static MacroValues CreateStaticMacroValues(
    params (string name, string? value)[] macros )
  {
    var builder = new MacroTableBuilder();

    foreach( var (name, _) in macros )
    {
      builder.Declare( name );
    }

    if( macros.Length == 0 )
    {
      // Ensure at least one macro is declared to avoid empty MacroTable
      builder.Declare( "dummy" );
    }

    var macroTable = builder.Build();
    var values = macroTable.CreateValues();

    foreach( var (name, value) in macros )
    {
      values.SetValue( name, value );
    }

    return values;
  }

  #endregion
}
