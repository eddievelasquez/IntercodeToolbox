// Module Name: MacroProcessorTests.cs
// Author:      Eduardo Velasquez
// Copyright (c) 2025, Intercode Consulting, Inc.

#pragma warning disable CS0618 // Type or member is obsolete

namespace Intercode.Toolbox.TemplateEngine.Tests;

using System.Text;
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

  // Edge case: Macro with argument but no generator
  [Fact]
  public void ProcessMacros_ShouldTreatMacroWithArgumentButNoGeneratorAsEmpty()
  {
    var values = CreateStaticMacroValues( ( "who", null ) );
    var template = TemplateCompiler.Compile( "Hello, $who:arg$!", values.MacroTable );
    var builder = new StringBuilder();
    template.ProcessMacros( builder, values );
    builder.ToString().Should().Be( "Hello, !" );
  }

  // Edge case: Macro defined but value explicitly set to null
  [Fact]
  public void ProcessMacros_ShouldTreatNullMacroValueAsEmpty()
  {
    var values = CreateStaticMacroValues( ( "who", null ) );
    var template = TemplateCompiler.Compile( "Hello, $who$!", values.MacroTable );
    var builder = new StringBuilder();
    template.ProcessMacros( builder, values );
    builder.ToString().Should().Be( "Hello, !" );
  }

  [Fact]
  public void ProcessMacros_WithStringBuilder_ShouldProcessConstantOnlyTemplate()
  {
    var values = CreateStaticMacroValues();
    var template = TemplateCompiler.Compile( "Just text.", values.MacroTable );
    var builder = new StringBuilder();
    template.ProcessMacros( builder, values );
    builder.ToString().Should().Be( "Just text." );
  }

  [Fact]
  public void ProcessMacros_WithStringBuilder_ShouldProcessDelimiterOnlyTemplate()
  {
    var values = CreateStaticMacroValues();
    var template = TemplateCompiler.Compile( "$$", values.MacroTable );
    var builder = new StringBuilder();
    template.ProcessMacros( builder, values );
    builder.ToString().Should().Be( "$" );
  }

  [Fact]
  public void ProcessMacros_WithStringBuilder_ShouldProcessUnclosedMacroAsConstant()
  {
    var values = CreateStaticMacroValues();
    var template = TemplateCompiler.Compile( "$macro", values.MacroTable );
    var builder = new StringBuilder();
    template.ProcessMacros( builder, values );
    builder.ToString().Should().Be( "$macro" );
  }

  [Fact]
  public void ProcessMacros_WithStringBuilder_ShouldReplaceEscapedDelimiters()
  {
    var values = CreateStaticMacroValues();
    var template = TemplateCompiler.Compile( "Give me the $$!", values.MacroTable );
    var builder = new StringBuilder();
    template.ProcessMacros( builder, values );
    builder.ToString().Should().Be( "Give me the $!" );
  }

  [Fact]
  public void ProcessMacros_WithStringBuilder_ShouldReplaceMacrosWithDynamicValues()
  {
    var timestamp = _timeProvider.GetLocalNow();

    var values = CreateDynamicMacroValues( ( "now", _ => timestamp.ToString() ) );
    var template = TemplateCompiler.Compile( "Timestamp: $now$", values.MacroTable );
    var builder = new StringBuilder();
    template.ProcessMacros( builder, values );
    builder.ToString().Should().Be( $"Timestamp: {timestamp}" );
  }

  [Fact]
  public void ProcessMacros_WithStringBuilder_ShouldReplaceMacrosWithDynamicValuesAndArgument()
  {
    var values = CreateDynamicMacroValues( ( "now", arg => _timeProvider.GetLocalNow().ToString( arg.ToString() ) ) );
    var template = TemplateCompiler.Compile( "Timestamp: $now:yyyyMMdd$", values.MacroTable );
    var builder = new StringBuilder();
    template.ProcessMacros( builder, values );
    builder.ToString().Should().Be( "Timestamp: 20241020" );
  }

  [Fact]
  public void ProcessMacros_WithStringBuilder_ShouldReplaceMacrosWithStaticValues()
  {
    var values = CreateStaticMacroValues( ( "who", "World" ) );
    var template = TemplateCompiler.Compile( "Hello, $who$!", values.MacroTable );
    var builder = new StringBuilder();
    template.ProcessMacros( builder, values );
    builder.ToString().Should().Be( "Hello, World!" );
  }

  [Fact]
  public void ProcessMacros_WithStringBuilder_ShouldWriteExceptionMessageIfMacroThrows()
  {
    var values = CreateDynamicMacroValues( ( "fail", _ => throw new InvalidOperationException( "fail macro error" ) ) );
    var template = TemplateCompiler.Compile( "Hello, $fail$!", values.MacroTable );
    var builder = new StringBuilder();
    template.ProcessMacros( builder, values );
    builder.ToString().Should().Contain( "fail macro error" );
  }

  [Fact]
  public void ProcessMacros_WithStringBuilder_ShouldWriteMacroTextIfValueIsNull()
  {
    var values = CreateStaticMacroValues( ( "who", null ) );
    var template = TemplateCompiler.Compile( "Hello, $who$!", values.MacroTable );
    var builder = new StringBuilder();
    template.ProcessMacros( builder, values );
    builder.ToString().Should().Be( "Hello, !" );
  }

  [Fact]
  public void ProcessMacros_WithStringWriter_ShouldProcessConstantOnlyTemplate()
  {
    var values = CreateStaticMacroValues();
    var template = TemplateCompiler.Compile( "Just text.", values.MacroTable );
    var writer = new StringWriter();
    template.ProcessMacros( writer, values );
    writer.ToString().Should().Be( "Just text." );
  }

  [Fact]
  public void ProcessMacros_WithStringWriter_ShouldProcessDelimiterOnlyTemplate()
  {
    var values = CreateStaticMacroValues();
    var template = TemplateCompiler.Compile( "$$", values.MacroTable );
    var writer = new StringWriter();
    template.ProcessMacros( writer, values );
    writer.ToString().Should().Be( "$" );
  }

  [Fact]
  public void ProcessMacros_WithStringWriter_ShouldProcessUnclosedMacroAsConstant()
  {
    var values = CreateStaticMacroValues();
    var template = TemplateCompiler.Compile( "$macro", values.MacroTable );
    var writer = new StringWriter();
    template.ProcessMacros( writer, values );
    writer.ToString().Should().Be( "$macro" );
  }

  [Fact]
  public void ProcessMacros_WithStringWriter_ShouldReplaceEscapedDelimiters()
  {
    var values = CreateStaticMacroValues();
    var template = TemplateCompiler.Compile( "Give me the $$!", values.MacroTable );
    var writer = new StringWriter();
    template.ProcessMacros( writer, values );
    writer.ToString().Should().Be( "Give me the $!" );
  }

  [Fact]
  public void ProcessMacros_WithStringWriter_ShouldReplaceMacrosWithDynamicValues()
  {
    var timestamp = _timeProvider.GetLocalNow();

    var values = CreateDynamicMacroValues( ( "now", _ => timestamp.ToString() ) );
    var template = TemplateCompiler.Compile( "Timestamp: $now$", values.MacroTable );
    var writer = new StringWriter();
    template.ProcessMacros( writer, values );
    writer.ToString().Should().Be( $"Timestamp: {timestamp}" );
  }

  [Fact]
  public void ProcessMacros_WithStringWriter_ShouldReplaceMacrosWithDynamicValuesAndArgument()
  {
    var values = CreateDynamicMacroValues( ( "now", arg => _timeProvider.GetLocalNow().ToString( arg.ToString() ) ) );
    var template = TemplateCompiler.Compile( "Timestamp: $now:yyyyMMdd$", values.MacroTable );
    var writer = new StringWriter();
    template.ProcessMacros( writer, values );
    writer.ToString().Should().Be( "Timestamp: 20241020" );
  }

  [Fact]
  public void ProcessMacros_WithStringWriter_ShouldReplaceMacrosWithStaticValues()
  {
    var values = CreateStaticMacroValues( ( "who", "World" ) );
    var template = TemplateCompiler.Compile( "Hello, $who$!", values.MacroTable );
    var writer = new StringWriter();
    template.ProcessMacros( writer, values );
    writer.ToString().Should().Be( "Hello, World!" );
  }

  [Fact]
  public void ProcessMacros_WithStringWriter_ShouldWriteExceptionMessageIfMacroThrows()
  {
    var values = CreateDynamicMacroValues( ( "fail", _ => throw new InvalidOperationException( "fail macro error" ) ) );
    var template = TemplateCompiler.Compile( "Hello, $fail$!", values.MacroTable );
    var writer = new StringWriter();
    template.ProcessMacros( writer, values );
    writer.ToString().Should().Contain( "fail macro error" );
  }

  [Fact]
  public void ProcessMacros_WithStringWriter_ShouldWriteMacroTextIfValueIsNull()
  {
    var values = CreateStaticMacroValues( ( "who", null ) );
    var template = TemplateCompiler.Compile( "Hello, $who$!", values.MacroTable );
    var writer = new StringWriter();
    template.ProcessMacros( writer, values );
    writer.ToString().Should().Be( "Hello, !" );
  }

  [Fact]
  public void ProcessMacros_WithStringBuilder_ShouldThrow_WhenMacroValuesFromDifferentTable()
  {
    var table1 = new MacroTableBuilder().Declare( "A" ).Build();
    var table2 = new MacroTableBuilder().Declare( "A" ).Build();
    var template = TemplateCompiler.Compile( "x$A$x", table1 );
    var values = table2.CreateValues();
    values.SetValue( "A", "v" );

    var builder = new StringBuilder();
    var act = () => template.ProcessMacros( builder, values );

    act.Should()
       .Throw<ArgumentException>()
       .WithParameterName( "macroValues" )
       .WithMessage( "*must be associated with the same MacroTable*" );
  }

  [Fact]
  public void ProcessMacros_WithTextWriter_ShouldThrow_WhenMacroValuesFromDifferentTable()
  {
    var table1 = new MacroTableBuilder().Declare( "A" ).Build();
    var table2 = new MacroTableBuilder().Declare( "A" ).Build();
    var template = TemplateCompiler.Compile( "x$A$x", table1 );
    var values = table2.CreateValues();

    using var writer = new StringWriter();
    var act = () => template.ProcessMacros( writer, values );

    act.Should()
       .Throw<ArgumentException>()
       .WithParameterName( "macroValues" )
       .WithMessage( "*must be associated with the same MacroTable*" );
  }

  [Fact]
  public void ProcessMacros_ReturningString_ShouldReturnProcessedText()
  {
    var values = CreateStaticMacroValues( ( "A", "1" ), ( "B", "2" ) );
    var template = TemplateCompiler.Compile( "$A$-$B$", values.MacroTable );

    var result = template.ProcessMacros( values );
    result.Should().Be( "1-2" );
  }

  [Fact]
  public void ProcessMacros_WithValuesSpan_ShouldThrow_WhenValuesArrayTooSmall()
  {
    var table = new MacroTableBuilder().Declare( "A" ).Declare( "B" ).Build();
    var template = TemplateCompiler.Compile( "$A$-$B$", table );

    var builder = new StringBuilder();
    var tooSmall = new string?[] { "1" };

    var act = () => template.ProcessMacros( builder, tooSmall );

    act.Should()
       .Throw<ArgumentException>()
       .WithParameterName( "values" )
       .WithMessage( "*must have at least as many elements as the MacroTable has slots*" );
  }

  [Fact]
  public void ProcessMacros_WithValuesSpan_ShouldUseProvidedValues()
  {
    var table = new MacroTableBuilder().Declare( "A" ).Declare( "B" ).Build();
    var template = TemplateCompiler.Compile( "$A$-$B$", table );

    var builder = new StringBuilder();
    var values = new string?[] { "X", "Y" };

    template.ProcessMacros( builder, values.AsSpan() );
    builder.ToString().Should().Be( "X-Y" );
  }

  [Fact]
  public void ProcessMacros_WithValuesSpan_ShouldSkipNullValues()
  {
    var table = new MacroTableBuilder().Declare( "A" ).Build();
    var template = TemplateCompiler.Compile( "X$A$Y", table );

    var builder = new StringBuilder();
    var values = new string?[] { null };

    template.ProcessMacros( builder, values.AsSpan() );
    builder.ToString().Should().Be( "XY" );
  }

  [Fact]
  public void ProcessMacros_WithValuesSpan_ShouldUseStandardMacros_ForNegativeSlots()
  {
    var table = new MacroTableBuilder().Build();
    var template = TemplateCompiler.Compile( "$GUID$", table );

    var builder = new StringBuilder();

    template.ProcessMacros( builder, ReadOnlySpan<string?>.Empty );

    var output = builder.ToString();
    output.Should().NotBeNullOrEmpty();
    Guid.TryParse( output, out _ ).Should().BeTrue();
  }

  [Fact]
  public void ProcessMacros_WithValuesArray_ShouldUseProvidedValues()
  {
    var table = new MacroTableBuilder().Declare( "A" ).Declare( "B" ).Build();
    var template = TemplateCompiler.Compile( "$A$-$B$", table );

    var builder = new StringBuilder();
    template.ProcessMacros( builder, "X", "Y" );

    builder.ToString().Should().Be( "X-Y" );
  }

  [Fact]
  public void ProcessMacros_ReturningString_WithValuesSpan_ShouldReturnProcessedText()
  {
    var table = new MacroTableBuilder().Declare( "A" ).Declare( "B" ).Build();
    var template = TemplateCompiler.Compile( "$A$-$B$", table );

    var values = new string?[] { "X", "Y" };
    var result = template.ProcessMacros( values.AsSpan() );

    result.Should().Be( "X-Y" );
  }

  [Fact]
  public void ProcessMacros_ReturningString_WithValuesArray_ShouldReturnProcessedText()
  {
    var table = new MacroTableBuilder().Declare( "A" ).Declare( "B" ).Build();
    var template = TemplateCompiler.Compile( "$A$-$B$", table );

    var result = template.ProcessMacros( "X", "Y" );

    result.Should().Be( "X-Y" );
  }

  [Fact]
  public void ProcessMacros_WithMacroValues_ShouldSupportStandardMacros()
  {
    var table = new MacroTableBuilder().Build();
    var values = table.CreateValues();
    var template = TemplateCompiler.Compile( "$GUID$", table );

    var result = template.ProcessMacros( values );

    result.Should().NotBeNullOrEmpty();
    Guid.TryParse( result, out _ ).Should().BeTrue();
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
