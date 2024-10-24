// Module Name: TemplateCompilerTests.cs
// Author:      Eduardo Velasquez
// Copyright (c) 2024, Intercode Consulting, Inc.

namespace Intercode.Toolbox.TemplateEngine.Tests;

using FluentAssertions;

public class TemplateCompilerTests
{
  #region Tests

  [Fact]
  public void Compile_ShouldHandleEscapedDelimiter_WhenStringEndsWithEscapedDelimiter()
  {
    const string Text = "template $$";

    var compiler = new TemplateCompiler();
    var template = compiler.Compile( Text );

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

    var compiler = new TemplateCompiler();
    var template = compiler.Compile( Text );

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

    var compiler = new TemplateCompiler();
    var template = compiler.Compile( Text );

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
    var text = "I have no macros";

    var compiler = new TemplateCompiler();
    var template = compiler.Compile( text );

    template.Should()
            .NotBeNull();

    template.Segments.Should()
            .HaveCount( 1 );

    template.Segments.Single()
            .Should()
            .Match<Segment>( s => s.Kind == SegmentKind.Constant && s.Text == text );
  }

  [Fact]
  public void Compile_ShouldReturnSingleMacroSegment_WhenTemplateIsOnlyTheMacro()
  {
    var text = "$macro$";

    var compiler = new TemplateCompiler();
    var template = compiler.Compile( text );

    template.Should()
            .NotBeNull();

    template.Segments.Should()
            .HaveCount( 1 );

    template.Segments.Single()
            .Should()
            .Match<Segment>( s => s.Kind == SegmentKind.Macro && s.Text == "macro" );
  }

  [Fact]
  public void Compile_ShouldReturnThreeSegments_WhenEscapedDelimiterIsBetweenMacroSegments()
  {
    const string Text = "$macro$$$$macro$";

    var compiler = new TemplateCompiler();
    var template = compiler.Compile( Text );

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

    var compiler = new TemplateCompiler();
    var template = compiler.Compile( Text );

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

  #endregion
}
