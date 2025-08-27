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

    var template = TemplateCompiler.Compile( new TemplateContext(), Text );

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

    var template = TemplateCompiler.Compile( new TemplateContext(), Text );

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

    var template = TemplateCompiler.Compile( new TemplateContext(), Text );

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
  public void Compile_ShouldReturnSingleConstantSegment_WhenTemplateHasNoMacros()
  {
    const string Text = "I have no macros";

    var template = TemplateCompiler.Compile( new TemplateContext(), Text );

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

    var template = TemplateCompiler.Compile( new TemplateContext(), Text );

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

    var context = new TemplateContext();
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

    var template = TemplateCompiler.Compile( new TemplateContext(), Text );

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

    var template = TemplateCompiler.Compile( new TemplateContext(), Text );

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

    var context = new TemplateContext();
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

  #endregion
}
