// Module Name: TemplateCompilerTests.cs
// Author:      Eduardo Velasquez
// Copyright (c) 2025, Intercode Consulting, Inc.

namespace Intercode.Toolbox.TemplateEngine.Tests;

using FluentAssertions;

public class TemplateCompilerTests
{
  #region Tests

  [Fact]
  public void Compile_ShouldHandleEscapedDelimiter_WhenStringEndsWithEscapedDelimiter()
  {
    const string Text = "template $$";
    var macroTable = DefineMacros();
    var template = TemplateCompiler.Compile( macroTable, Text );

    template.Should()
            .NotBeNull();

    template.Segments.Should()
            .HaveCount( 2 );

    template.Segments[0]
            .Should()
            .Match<Segment>( s => s.IsConstant && s.Text == "template " );

    template.Segments[1]
            .Should()
            .Match<Segment>( s => s.IsConstant && s.Text == "$" );
  }

  [Fact]
  public void Compile_ShouldHandleEscapedDelimiter_WhenStringStartsWithEscapedDelimiter()
  {
    const string Text = "$$ template.";

    var macroTable = DefineMacros();
    var template = TemplateCompiler.Compile( macroTable, Text );

    template.Should()
            .NotBeNull();

    template.Segments.Should()
            .HaveCount( 2 );

    template.Segments[0]
            .Should()
            .Match<Segment>( s => s.IsConstant && s.Text == "$" );

    template.Segments[1]
            .Should()
            .Match<Segment>( s => s.IsConstant && s.Text == " template." );
  }

  [Fact]
  public void Compile_ShouldHandleEscapedDelimiters_WhenStringHasEscapedDelimitersInMiddle()
  {
    const string Text = "012345$$$$012345";

    var macroTable = DefineMacros();
    var template = TemplateCompiler.Compile( macroTable, Text );

    template.Should()
            .NotBeNull();

    template.Segments.Should()
            .HaveCount( 4 );

    template.Text.Should().Be( Text );

    template.Segments[0]
            .Should()
            .Match<Segment>( s => s.IsConstant && s.Text == "012345" );

    template.Segments[1]
            .Should()
            .Match<Segment>( s => s.IsConstant && s.Text == "$" );

    template.Segments[2]
            .Should()
            .Match<Segment>( s => s.IsConstant && s.Text == "$" );

    template.Segments[3]
            .Should()
            .Match<Segment>( s => s.IsConstant && s.Text == "012345" );
  }

  [Fact]
  public void Compile_ShouldHandleIncludeWithNullGenerator()
  {
    var includes = new IncludesCollection();
    includes.AddInclude( "empty", ( MacroValueGenerator? ) null );

    var macroTable = DefineMacros();
    var template = TemplateCompiler.Compile( macroTable, "$empty$", includes );

    template.Should().NotBeNull();
    template.Text.Should().Be( "" );
    template.Segments.Should().HaveCount( 1 );
    template.Segments[0].Should().Match<Segment>( s => s.IsConstant && s.Text == "" );
  }

  [Fact]
  public void Compile_ShouldHandleIncludeWithNullStringContent()
  {
    var includes = new IncludesCollection();
    includes.AddInclude( "empty", ( string? ) null );

    var macroTable = DefineMacros();
    var template = TemplateCompiler.Compile( macroTable, "$empty$", includes );

    template.Should().NotBeNull();
    template.Text.Should().Be( "" );
    template.Segments.Should().HaveCount( 1 );
    template.Segments[0].Should().Match<Segment>( s => s.IsConstant && s.Text == "" );
  }

  [Fact]
  public void Compile_ShouldHandleMacroWithArgument()
  {
    var macroTable = DefineMacros( "macro" );
    var template = TemplateCompiler.Compile( macroTable, "$macro:argument$" );

    template.Should().NotBeNull();
    template.Segments.Should().HaveCount( 1 );

    template.Segments[0]
            .Should()
            .Match<Segment>( s => s.IsMacro &&
                                  s.Text == "macro" &&
                                  s.ArgumentMemory.ToString() == "argument"
            );
  }

  [Fact]
  public void Compile_ShouldHandleNullIncludesParameter()
  {
    const string Text = "$macro$";
    var macroTable = DefineMacros( "macro" );
    var template = TemplateCompiler.Compile( macroTable, Text, null );

    template.Should().NotBeNull();
    template.Text.Should().Be( Text );
    template.Segments.Should().HaveCount( 1 );
    template.Segments[0].Should().Match<Segment>( s => s.IsMacro && s.Text == "macro" );
  }

  [Fact]
  public void Compile_ShouldNotReplaceAnything_WhenIncludesProvidedButNotReferenced()
  {
    var includes = new IncludesCollection();
    includes.AddInclude( "unused", "UnusedContent" );

    const string Text = "No macros here";
    var macroTable = DefineMacros();
    var template = TemplateCompiler.Compile( macroTable, Text, includes );

    template.Should().NotBeNull();
    template.Text.Should().Be( Text );
    template.Segments.Should().HaveCount( 1 );
    template.Segments[0].Should().Match<Segment>( s => s.IsConstant && s.Text == Text );
  }

  [Fact]
  public void Compile_ShouldProcessEscapedDelimitersInIncludeContent()
  {
    var includes = new IncludesCollection();
    includes.AddInclude( "section", "Start $$ End" );

    var macroTable = DefineMacros();
    var template = TemplateCompiler.Compile( macroTable, "$section$", includes );

    template.Should().NotBeNull();
    template.Text.Should().Be( "Start $$ End" );
    template.Segments.Should().HaveCount( 3 );
    template.Segments[1].Should().Match<Segment>( s => s.IsConstant );
  }

  [Fact]
  public void Compile_ShouldProcessIncludeWithMacroArgument()
  {
    var includes = new IncludesCollection();
    includes.AddInclude( "section", "Start $macro:arg$ End" );

    var macroTable = DefineMacros( "macro" );
    var template = TemplateCompiler.Compile( macroTable, "$section$", includes );

    template.Should().NotBeNull();
    template.Text.Should().Be( "Start $macro:arg$ End" );
    template.Segments.Should().HaveCount( 3 );

    template.Segments[1]
            .Should()
            .Match<Segment>( s => s.IsMacro && s.Text == "macro" && s.ArgumentMemory.ToString() == "arg" );
  }

  [Fact]
  public void Compile_ShouldProcessMacrosInIncludeContent()
  {
    var includes = new IncludesCollection();
    includes.AddInclude( "section", "Start $macro$ End" );

    var macroTable = DefineMacros( "macro" );
    var template = TemplateCompiler.Compile( macroTable, "$section$", includes );

    template.Should().NotBeNull();
    template.Text.Should().Be( "Start $macro$ End" );
    template.Segments.Should().HaveCount( 3 );
    template.Segments[0].Should().Match<Segment>( s => s.IsConstant && s.Text == "Start " );
    template.Segments[1].Should().Match<Segment>( s => s.IsMacro && s.Text == "macro" );
    template.Segments[2].Should().Match<Segment>( s => s.IsConstant && s.Text == " End" );
  }

  [Fact]
  public void Compile_ShouldProcessMacrosInMultipleIncludes()
  {
    var includes = new IncludesCollection();
    includes.AddInclude( "a", "A$macroA$" );
    includes.AddInclude( "b", "B$macroB$" );

    var macroTable = DefineMacros( "macroA", "macroB" );
    var template = TemplateCompiler.Compile( macroTable, "$a$-$b$", includes );

    template.Should().NotBeNull();
    template.Text.Should().Be( "A$macroA$-B$macroB$" );
    template.Segments.Should().HaveCount( 4 );
    template.Segments[0].Should().Match<Segment>( s => s.IsConstant && s.Text == "A" );
    template.Segments[1].Should().Match<Segment>( s => s.IsMacro && s.Text == "macroA" );
    template.Segments[2].Should().Match<Segment>( s => s.IsConstant && s.Text == "-B" );
    template.Segments[3].Should().Match<Segment>( s => s.IsMacro && s.Text == "macroB" );
  }

  [Fact]
  public void Compile_ShouldReplaceInclude_WhenIncludeIsReferencedInTemplate()
  {
    var includes = new IncludesCollection();
    includes.AddInclude( "header", "HeaderContent" );

    var macroTable = DefineMacros();
    var template = TemplateCompiler.Compile( macroTable, "$header$ body", includes );

    template.Should().NotBeNull();
    template.Text.Should().Be( "HeaderContent body" );
    template.Segments.Should().HaveCount( 1 );
    template.Segments[0].Should().Match<Segment>( s => s.IsConstant && s.Text == "HeaderContent body" );
  }

  [Fact]
  public void Compile_ShouldReplaceMultipleIncludes_WhenMultipleAreReferenced()
  {
    var includes = new IncludesCollection();
    includes.AddInclude( "a", "A" );
    includes.AddInclude( "b", "B" );

    var macroTable = DefineMacros();
    var template = TemplateCompiler.Compile( macroTable, "$a$-$b$", includes );
    template.Should().NotBeNull();
    template.Text.Should().Be( "A-B" );
    template.Segments.Should().HaveCount( 1 );
    template.Segments[0].Should().Match<Segment>( s => s.IsConstant && s.Text == "A-B" );
  }

  [Fact]
  public void Compile_ShouldReplaceWithEmpty_WhenIncludeIsReferencedButDoesNotExist()
  {
    const string Text = "$missing$";
    var macroTable = DefineMacros( "missing" );
    var template = TemplateCompiler.Compile( macroTable, Text, new IncludesCollection() );

    template.Should().NotBeNull();
    template.Text.Should().Be( Text );
    template.Segments.Should().HaveCount( 1 );
    template.Segments[0].Should().Match<Segment>( s => s.IsMacro && s.Text == "missing" );
  }

  [Fact]
  public void Compile_ShouldReturnConstantSegment_WhenTextIsOnlyDelimiter()
  {
    const string Text = "$";
    var macroTable = DefineMacros();
    var template = TemplateCompiler.Compile( macroTable, Text );
    template.Should().NotBeNull();
    template.Segments.Should().HaveCount( 1 );
    template.Segments[0].Should().Match<Segment>( s => s.IsConstant && s.Text == "$" );
  }

  [Fact]
  public void Compile_ShouldReturnConstantSegment_WhenTextIsUnclosedMacro()
  {
    const string Text = "$macro";
    var macroTable = DefineMacros();
    var template = TemplateCompiler.Compile( macroTable, Text );
    template.Should().NotBeNull();
    template.Segments.Should().HaveCount( 1 );
    template.Segments[0].Should().Match<Segment>( s => s.IsConstant && s.Text == "$macro" );
  }

  [Fact]
  public void Compile_ShouldReturnDelimiterSegments_WhenTextIsMultipleEscapedDelimitersOnly()
  {
    const string Text = "$$$$";
    var macroTable = DefineMacros();
    var template = TemplateCompiler.Compile( macroTable, Text );
    template.Should().NotBeNull();
    template.Segments.Should().HaveCount( 2 );
    template.Segments[0].Should().Match<Segment>( s => s.IsConstant && s.Text == "$" );
    template.Segments[1].Should().Match<Segment>( s => s.IsConstant && s.Text == "$" );
  }

  [Fact]
  public void Compile_ShouldReturnMacroSegment_WhenIncludeCollectionIsEmptyAndIncludeReferenced()
  {
    var includes = new IncludesCollection();
    var macroTable = DefineMacros( "missing" );
    var template = TemplateCompiler.Compile( macroTable, "$missing$", includes );

    template.Should().NotBeNull();
    template.Text.Should().Be( "$missing$" );
    template.Segments.Should().HaveCount( 1 );
    template.Segments[0].Should().Match<Segment>( s => s.IsMacro && s.Text == "missing" );
  }

  [Fact]
  public void Compile_ShouldReturnSingleConstantSegment_WhenTemplateHasNoMacros()
  {
    const string Text = "I have no macros";

    var macroTable = DefineMacros();
    var template = TemplateCompiler.Compile( macroTable, Text );

    template.Should()
            .NotBeNull();

    template.Segments.Should()
            .HaveCount( 1 );

    template.Segments.Single()
            .Should()
            .Match<Segment>( s => s.IsConstant && s.Text == Text );
  }

  [Fact]
  public void Compile_ShouldReturnSingleMacroSegment_WhenTemplateIsOnlyTheMacro()
  {
    const string Text = "$macro$";

    var macroTable = DefineMacros( "macro" );
    var template = TemplateCompiler.Compile( macroTable, Text );

    template.Should()
            .NotBeNull();

    template.Segments.Should()
            .HaveCount( 1 );

    template.Segments.Single()
            .Should()
            .Match<Segment>( s => s.IsMacro && s.Text == "macro" );
  }

  [Fact]
  public void Compile_ShouldReturnThreeMacroValues_WhenTemplateContainsThreeDifferentMacros()
  {
    const string Text = "$macroA$$macroB$$macroC$";

    var macroTable = DefineMacros( "macroA", "macroB", "macroC" );
    var template = TemplateCompiler.Compile( macroTable, Text );

    template.Should()
            .NotBeNull();

    template.Segments.Should()
            .HaveCount( 3 );

    template.Segments[0]
            .Should()
            .Match<Segment>( s => s.IsMacro && s.Text == "macroA" && s.Slot == 0 );

    template.Segments[1]
            .Should()
            .Match<Segment>( s => s.IsMacro && s.Text == "macroB" && s.Slot == 1 );

    template.Segments[2]
            .Should()
            .Match<Segment>( s => s.IsMacro && s.Text == "macroC" && s.Slot == 2 );
  }

  [Fact]
  public void Compile_ShouldReturnThreeSegments_WhenEscapedDelimiterIsBetweenMacroSegments()
  {
    const string Text = "$macro$$$$macro$";

    var macroTable = DefineMacros( "macro" );
    var template = TemplateCompiler.Compile( macroTable, Text );

    template.Should()
            .NotBeNull();

    template.Segments.Should()
            .HaveCount( 3 );

    template.Segments[0]
            .Should()
            .Match<Segment>( s => s.IsMacro && s.Text == "macro" );

    template.Segments[1]
            .Should()
            .Match<Segment>( s => s.IsConstant && s.Text == "$" );

    template.Segments[2]
            .Should()
            .Match<Segment>( s => s.IsMacro && s.Text == "macro" );
  }

  [Fact]
  public void Compile_ShouldReturnThreeSegments_WhenTemplateContainsMacroInMiddle()
  {
    const string Text = "This is a $macro$ template.";

    var macroTable = DefineMacros( "macro" );
    var template = TemplateCompiler.Compile( macroTable, Text );

    template.Should()
            .NotBeNull();

    template.Segments.Should()
            .HaveCount( 3 );

    template.Segments[0]
            .Should()
            .Match<Segment>( static s => s.IsConstant && s.Text == "This is a " );

    template.Segments[1]
            .Should()
            .Match<Segment>( static s => s.IsMacro && s.Text == "macro" );

    template.Segments[2]
            .Should()
            .Match<Segment>( static s => s.IsConstant && s.Text == " template." );
  }

  [Fact]
  public void Compile_ShouldReturnTwoMacroValues_WhenTemplateContainsTwoDifferentMacrosAndOneRepeated()
  {
    const string Text = "$macroA$$macroB$$macroA$";

    var macroTable = DefineMacros( "macroA", "macroB" );
    var template = TemplateCompiler.Compile( macroTable, Text );

    template.Should()
            .NotBeNull();

    template.Segments.Should()
            .HaveCount( 3 );

    template.Segments[0]
            .Should()
            .Match<Segment>( s => s.IsMacro && s.Text == "macroA" && s.Slot == 0 );

    template.Segments[1]
            .Should()
            .Match<Segment>( s => s.IsMacro && s.Text == "macroB" && s.Slot == 1 );

    template.Segments[2]
            .Should()
            .Match<Segment>( s => s.IsMacro && s.Text == "macroA" && s.Slot == 0 );
  }

  [Fact]
  public void Compile_ShouldSupportCustomDelimiterAndArgumentSeparator()
  {
    var options = new TemplateCompilerOptions( '#', ':' );
    var macroTable = new MacroTableBuilder().Declare( "macro" ).Build();
    var template = TemplateCompiler.Compile( macroTable, "#macro:arg#", options: options );

    template.Should().NotBeNull();
    template.Segments.Should().HaveCount( 1 );

    template.Segments[0]
            .Should()
            .Match<Segment>( s => s.IsMacro && s.Text == "macro" && s.ArgumentMemory.ToString() == "arg" );
  }

  [Theory]
  [InlineData( null )]
  [InlineData( "" )]
  [InlineData( "   " )]
  public void Compile_ShouldThrowArgumentException_WhenTextIsNullOrWhitespace(
    string? text )
  {
    var macroTable = DefineMacros();
    Action act = () => TemplateCompiler.Compile( macroTable, text! );

    act.Should()
       .Throw<ArgumentException>()
       .WithParameterName( "templateText" )
       .WithMessage( "*cannot be null, empty, or whitespace*" );
  }

  [Fact]
  public void Compile_ShouldThrowArgumentNullException_WhenMacroTableIsNull()
  {
    Action act = () => TemplateCompiler.Compile( null!, "text" );

    act.Should()
       .Throw<ArgumentNullException>()
       .WithParameterName( "macroTable" );
  }

  #endregion

  #region Implementation

  private static MacroTable DefineMacros(
    params string[] macroNames )
  {
    var builder = new MacroTableBuilder();

    foreach( var name in macroNames )
    {
      builder.Declare( name );
    }

    if( macroNames.Length == 0 )
    {
      // Ensure at least one macro is declared to avoid InvalidOperationException
      builder.Declare( "defaultMacro" );
    }

    return builder.Build();
  }

  #endregion
}
