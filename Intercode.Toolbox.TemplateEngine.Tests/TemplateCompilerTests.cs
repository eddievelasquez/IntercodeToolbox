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

    var template = TemplateCompiler.Compile( new MacroProcessorContext(), Text );

    template.Should()
            .NotBeNull();

    template.Segments.Should()
            .HaveCount( 2 );

    template.Segments[0]
            .Should()
            .Match<Segment>( s => s.Kind == SegmentKind.Constant && s.Text == "template " );

    template.Segments[1]
            .Should()
            .Match<Segment>( s => s.Kind == SegmentKind.Delimiter && s.Memory.IsEmpty );
  }

  [Fact]
  public void Compile_ShouldHandleEscapedDelimiter_WhenStringStartsWithEscapedDelimiter()
  {
    const string Text = "$$ template.";

    var template = TemplateCompiler.Compile( new MacroProcessorContext(), Text );

    template.Should()
            .NotBeNull();

    template.Segments.Should()
            .HaveCount( 2 );

    template.Segments[0]
            .Should()
            .Match<Segment>( s => s.Kind == SegmentKind.Delimiter && s.Memory.IsEmpty );

    template.Segments[1]
            .Should()
            .Match<Segment>( s => s.Kind == SegmentKind.Constant && s.Text == " template." );
  }

  [Fact]
  public void Compile_ShouldHandleEscapedDelimiters_WhenStringHasEscapedDelimitersInMiddle()
  {
    const string Text = "012345$$$$012345";

    var template = TemplateCompiler.Compile( new MacroProcessorContext(), Text );

    template.Should()
            .NotBeNull();

    template.Segments.Should()
            .HaveCount( 4 );

    template.Text.Should().Be( Text );

    template.Segments[0]
            .Should()
            .Match<Segment>( s => s.Kind == SegmentKind.Constant && s.Text == "012345" );

    template.Segments[1]
            .Should()
            .Match<Segment>( s => s.Kind == SegmentKind.Delimiter && s.Memory.IsEmpty );

    template.Segments[2]
            .Should()
            .Match<Segment>( s => s.Kind == SegmentKind.Delimiter && s.Memory.IsEmpty );

    template.Segments[3]
            .Should()
            .Match<Segment>( s => s.Kind == SegmentKind.Constant && s.Text == "012345" );
  }

  [Fact]
  public void Compile_ShouldHandleIncludeWithNullStringContent()
  {
    var includes = new IncludesCollection();
    includes.AddInclude( "empty", ( string? ) null );

    var template = TemplateCompiler.Compile( new MacroProcessorContext(), "$empty$", includes );

    template.Should().NotBeNull();
    template.Text.Should().Be( "" );
    template.Segments.Should().HaveCount( 1 );
    template.Segments[0].Should().Match<Segment>( s => s.Kind == SegmentKind.Constant && s.Text == "" );
  }

  [Fact]
  public void Compile_ShouldHandleIncludeWithNullGenerator()
  {
    var includes = new IncludesCollection();
    includes.AddInclude( "empty", ( MacroValueGenerator? ) null );

    var template = TemplateCompiler.Compile( new MacroProcessorContext(), "$empty$", includes );

    template.Should().NotBeNull();
    template.Text.Should().Be( "" );
    template.Segments.Should().HaveCount( 1 );
    template.Segments[0].Should().Match<Segment>( s => s.Kind == SegmentKind.Constant && s.Text == "" );
  }

  [Fact]
  public void Compile_ShouldHandleMacroWithArgument()
  {
    var context = new MacroProcessorContext();
    var template = TemplateCompiler.Compile( context, "$macro:argument$" );

    template.Should().NotBeNull();
    template.Segments.Should().HaveCount( 1 );

    template.Segments[0]
            .Should()
            .Match<Segment>( s => s.Kind == SegmentKind.Macro &&
                                  s.Text == "macro" &&
                                  s.ArgumentMemory.ToString() == "argument"
            );
  }

  [Fact]
  public void Compile_ShouldHandleNullIncludesParameter()
  {
    const string Text = "$macro$";
    var template = TemplateCompiler.Compile( new MacroProcessorContext(), Text, null );

    template.Should().NotBeNull();
    template.Text.Should().Be( Text );
    template.Segments.Should().HaveCount( 1 );
    template.Segments[0].Should().Match<Segment>( s => s.Kind == SegmentKind.Macro && s.Text == "macro" );
  }

  [Fact]
  public void Compile_ShouldNotReplaceAnything_WhenIncludesProvidedButNotReferenced()
  {
    var includes = new IncludesCollection();
    includes.AddInclude( "unused", "UnusedContent" );

    const string Text = "No macros here";
    var template = TemplateCompiler.Compile( new MacroProcessorContext(), Text, includes );

    template.Should().NotBeNull();
    template.Text.Should().Be( Text );
    template.Segments.Should().HaveCount( 1 );
    template.Segments[0].Should().Match<Segment>( s => s.Kind == SegmentKind.Constant && s.Text == Text );
  }

  [Fact]
  public void Compile_ShouldProcessEscapedDelimitersInIncludeContent()
  {
    var includes = new IncludesCollection();
    includes.AddInclude( "section", "Start $$ End" );

    var template = TemplateCompiler.Compile( new MacroProcessorContext(), "$section$", includes );

    template.Should().NotBeNull();
    template.Text.Should().Be( "Start $$ End" );
    template.Segments.Should().HaveCount( 3 );
    template.Segments[1].Should().Match<Segment>( s => s.Kind == SegmentKind.Delimiter );
  }

  [Fact]
  public void Compile_ShouldProcessIncludeWithMacroArgument()
  {
    var includes = new IncludesCollection();
    includes.AddInclude( "section", "Start $macro:arg$ End" );

    var template = TemplateCompiler.Compile( new MacroProcessorContext(), "$section$", includes );

    template.Should().NotBeNull();
    template.Text.Should().Be( "Start $macro:arg$ End" );
    template.Segments.Should().HaveCount( 3 );

    template.Segments[1]
            .Should()
            .Match<Segment>( s => s.Kind == SegmentKind.Macro && s.Text == "macro" && s.ArgumentMemory.ToString() == "arg" );
  }

  [Fact]
  public void Compile_ShouldProcessMacrosInIncludeContent()
  {
    var includes = new IncludesCollection();
    includes.AddInclude( "section", "Start $macro$ End" );

    var template = TemplateCompiler.Compile( new MacroProcessorContext(), "$section$", includes );

    template.Should().NotBeNull();
    template.Text.Should().Be( "Start $macro$ End" );
    template.Segments.Should().HaveCount( 3 );
    template.Segments[0].Should().Match<Segment>( s => s.Kind == SegmentKind.Constant && s.Text == "Start " );
    template.Segments[1].Should().Match<Segment>( s => s.Kind == SegmentKind.Macro && s.Text == "macro" );
    template.Segments[2].Should().Match<Segment>( s => s.Kind == SegmentKind.Constant && s.Text == " End" );
  }

  [Fact]
  public void Compile_ShouldProcessMacrosInMultipleIncludes()
  {
    var includes = new IncludesCollection();
    includes.AddInclude( "a", "A$macroA$" );
    includes.AddInclude( "b", "B$macroB$" );

    var template = TemplateCompiler.Compile( new MacroProcessorContext(), "$a$-$b$", includes );

    template.Should().NotBeNull();
    template.Text.Should().Be( "A$macroA$-B$macroB$" );
    template.Segments.Should().HaveCount( 4 );
    template.Segments[0].Should().Match<Segment>( s => s.Kind == SegmentKind.Constant && s.Text == "A" );
    template.Segments[1].Should().Match<Segment>( s => s.Kind == SegmentKind.Macro && s.Text == "macroA" );
    template.Segments[2].Should().Match<Segment>( s => s.Kind == SegmentKind.Constant && s.Text == "-B" );
    template.Segments[3].Should().Match<Segment>( s => s.Kind == SegmentKind.Macro && s.Text == "macroB" );
  }

  [Fact]
  public void Compile_ShouldReplaceInclude_WhenIncludeIsReferencedInTemplate()
  {
    var includes = new IncludesCollection();
    includes.AddInclude( "header", "HeaderContent" );

    var template = TemplateCompiler.Compile( new MacroProcessorContext(), "$header$ body", includes );

    template.Should().NotBeNull();
    template.Text.Should().Be( "HeaderContent body" );
    template.Segments.Should().HaveCount( 1 );
    template.Segments[0].Should().Match<Segment>( s => s.Kind == SegmentKind.Constant && s.Text == "HeaderContent body" );
  }

  [Fact]
  public void Compile_ShouldReplaceMultipleIncludes_WhenMultipleAreReferenced()
  {
    var includes = new IncludesCollection();
    includes.AddInclude( "a", "A" );
    includes.AddInclude( "b", "B" );

    var template = TemplateCompiler.Compile( new MacroProcessorContext(), "$a$-$b$", includes );
    template.Should().NotBeNull();
    template.Text.Should().Be( "A-B" );
    template.Segments.Should().HaveCount( 1 );
    template.Segments[0].Should().Match<Segment>( s => s.Kind == SegmentKind.Constant && s.Text == "A-B" );
  }

  [Fact]
  public void Compile_ShouldReplaceWithEmpty_WhenIncludeIsReferencedButDoesNotExist()
  {
    const string Text = "$missing$";
    var template = TemplateCompiler.Compile( new MacroProcessorContext(), Text, new IncludesCollection() );

    template.Should().NotBeNull();
    template.Text.Should().Be( Text );
    template.Segments.Should().HaveCount( 1 );
    template.Segments[0].Should().Match<Segment>( s => s.Kind == SegmentKind.Macro && s.Text == "missing" );
  }

  [Fact]
  public void Compile_ShouldReturnConstantSegment_WhenTextIsOnlyDelimiter()
  {
    const string Text = "$";
    var template = TemplateCompiler.Compile( new MacroProcessorContext(), Text );
    template.Should().NotBeNull();
    template.Segments.Should().HaveCount( 1 );
    template.Segments[0].Should().Match<Segment>( s => s.Kind == SegmentKind.Constant && s.Text == "$" );
  }

  [Fact]
  public void Compile_ShouldReturnConstantSegment_WhenTextIsUnclosedMacro()
  {
    const string Text = "$macro";
    var template = TemplateCompiler.Compile( new MacroProcessorContext(), Text );
    template.Should().NotBeNull();
    template.Segments.Should().HaveCount( 1 );
    template.Segments[0].Should().Match<Segment>( s => s.Kind == SegmentKind.Constant && s.Text == "$macro" );
  }

  [Fact]
  public void Compile_ShouldReturnDelimiterSegments_WhenTextIsMultipleEscapedDelimitersOnly()
  {
    const string Text = "$$$$";
    var template = TemplateCompiler.Compile( new MacroProcessorContext(), Text );
    template.Should().NotBeNull();
    template.Segments.Should().HaveCount( 2 );
    template.Segments[0].Should().Match<Segment>( s => s.Kind == SegmentKind.Delimiter && s.Memory.IsEmpty );
    template.Segments[1].Should().Match<Segment>( s => s.Kind == SegmentKind.Delimiter && s.Memory.IsEmpty );
  }

  [Fact]
  public void Compile_ShouldReturnMacroSegment_WhenIncludeCollectionIsEmptyAndIncludeReferenced()
  {
    var includes = new IncludesCollection();
    var template = TemplateCompiler.Compile( new MacroProcessorContext(), "$missing$", includes );

    template.Should().NotBeNull();
    template.Text.Should().Be( "$missing$" );
    template.Segments.Should().HaveCount( 1 );
    template.Segments[0].Should().Match<Segment>( s => s.Kind == SegmentKind.Macro && s.Text == "missing" );
  }

  [Fact]
  public void Compile_ShouldReturnSingleConstantSegment_WhenTemplateHasNoMacros()
  {
    const string Text = "I have no macros";

    var template = TemplateCompiler.Compile( new MacroProcessorContext(), Text );

    template.Should()
            .NotBeNull();

    template.Segments.Should()
            .HaveCount( 1 );

    template.Segments.Single()
            .Should()
            .Match<Segment>( s => s.Kind == SegmentKind.Constant && s.Text == Text );
  }

  [Fact]
  public void Compile_ShouldReturnSingleMacroSegment_WhenTemplateIsOnlyTheMacro()
  {
    const string Text = "$macro$";

    var template = TemplateCompiler.Compile( new MacroProcessorContext(), Text );

    template.Should()
            .NotBeNull();

    template.Segments.Should()
            .HaveCount( 1 );

    template.Segments.Single()
            .Should()
            .Match<Segment>( s => s.Kind == SegmentKind.Macro && s.Text == "macro" );
  }

  [Fact]
  public void Compile_ShouldReturnThreeMacroValues_WhenTemplateContainsThreeDifferentMacros()
  {
    const string Text = "$macroA$$macroB$$macroC$";

    var context = new MacroProcessorContext();
    var template = TemplateCompiler.Compile( context, Text );

    template.Should()
            .NotBeNull();

    template.Segments.Should()
            .HaveCount( 3 );

    template.Segments[0]
            .Should()
            .Match<Segment>( s => s.Kind == SegmentKind.Macro && s.Text == "macroA" && s.ValueSlot == 0 );

    template.Segments[1]
            .Should()
            .Match<Segment>( s => s.Kind == SegmentKind.Macro && s.Text == "macroB" && s.ValueSlot == 1 );

    template.Segments[2]
            .Should()
            .Match<Segment>( s => s.Kind == SegmentKind.Macro && s.Text == "macroC" && s.ValueSlot == 2 );

    context.MacroCount.Should().Be( 3 );

    context.GetMacroSlot( "macroA" ).Should().Be( 0 );
    context.GetMacroSlot( "macroB" ).Should().Be( 1 );
    context.GetMacroSlot( "macroC" ).Should().Be( 2 );
  }

  [Fact]
  public void Compile_ShouldReturnThreeSegments_WhenEscapedDelimiterIsBetweenMacroSegments()
  {
    const string Text = "$macro$$$$macro$";

    var template = TemplateCompiler.Compile( new MacroProcessorContext(), Text );

    template.Should()
            .NotBeNull();

    template.Segments.Should()
            .HaveCount( 3 );

    template.Segments[0]
            .Should()
            .Match<Segment>( s => s.Kind == SegmentKind.Macro && s.Text == "macro" );

    template.Segments[1]
            .Should()
            .Match<Segment>( s => s.Kind == SegmentKind.Delimiter && s.Memory.IsEmpty );

    template.Segments[2]
            .Should()
            .Match<Segment>( s => s.Kind == SegmentKind.Macro && s.Text == "macro" );
  }

  [Fact]
  public void Compile_ShouldReturnThreeSegments_WhenTemplateContainsMacroInMiddle()
  {
    const string Text = "This is a $macro$ template.";

    var template = TemplateCompiler.Compile( new MacroProcessorContext(), Text );

    template.Should()
            .NotBeNull();

    template.Segments.Should()
            .HaveCount( 3 );

    template.Segments[0]
            .Should()
            .Match<Segment>( static s => s.Kind == SegmentKind.Constant && s.Text == "This is a " );

    template.Segments[1]
            .Should()
            .Match<Segment>( static s => s.Kind == SegmentKind.Macro && s.Text == "macro" );

    template.Segments[2]
            .Should()
            .Match<Segment>( static s => s.Kind == SegmentKind.Constant && s.Text == " template." );
  }

  [Fact]
  public void Compile_ShouldReturnTwoMacroValues_WhenTemplateContainsTwoDifferentMacrosAndOneRepeated()
  {
    const string Text = "$macroA$$macroB$$macroA$";

    var context = new MacroProcessorContext();
    var template = TemplateCompiler.Compile( context, Text );

    template.Should()
            .NotBeNull();

    template.Segments.Should()
            .HaveCount( 3 );

    template.Segments[0]
            .Should()
            .Match<Segment>( s => s.Kind == SegmentKind.Macro && s.Text == "macroA" && s.ValueSlot == 0 );

    template.Segments[1]
            .Should()
            .Match<Segment>( s => s.Kind == SegmentKind.Macro && s.Text == "macroB" && s.ValueSlot == 1 );

    template.Segments[2]
            .Should()
            .Match<Segment>( s => s.Kind == SegmentKind.Macro && s.Text == "macroA" && s.ValueSlot == 0 );

    context.MacroCount.Should().Be( 2 );
    context.GetMacroSlot( "macroA" ).Should().Be( 0 );
    context.GetMacroSlot( "macroB" ).Should().Be( 1 );
  }

  [Fact]
  public void Compile_ShouldSupportCustomDelimiterAndArgumentSeparator()
  {
    var options = new TemplateCompilerOptionsBuilder().SetMacroDelimiter( '#' ).SetArgumentSeparator( ':' ).Build();
    var context = new MacroProcessorContext( options );
    var template = TemplateCompiler.Compile( context, "#macro:arg#" );

    template.Should().NotBeNull();
    template.Segments.Should().HaveCount( 1 );

    template.Segments[0]
            .Should()
            .Match<Segment>( s => s.Kind == SegmentKind.Macro && s.Text == "macro" && s.ArgumentMemory.ToString() == "arg" );
  }

  [Theory]
  [InlineData( null )]
  [InlineData( "" )]
  [InlineData( "   " )]
  public void Compile_ShouldThrowArgumentException_WhenTextIsNullOrWhitespace(
    string? text )
  {
    Action act = () => TemplateCompiler.Compile( new MacroProcessorContext(), text! );

    act.Should()
       .Throw<ArgumentException>()
       .WithParameterName( "text" )
       .WithMessage( "*cannot be null, empty, or whitespace*" );
  }

  [Fact]
  public void Compile_ShouldThrowArgumentNullException_WhenContextIsNull()
  {
    Action act = () => TemplateCompiler.Compile( null!, "text" );

    act.Should()
       .Throw<ArgumentNullException>()
       .WithParameterName( "context" );
  }

  #endregion
}
