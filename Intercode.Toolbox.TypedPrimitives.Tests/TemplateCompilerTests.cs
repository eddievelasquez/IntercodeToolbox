// Module Name: TemplateCompilerTests.cs
// Author:      Eduardo Velasquez
// Copyright (c) 2024, Intercode Consulting, Inc.

namespace Intercode.Toolbox.TypedPrimitives.Tests;

using FluentAssertions;
using Intercode.Toolbox.TypedPrimitives.TemplateEngine;

public class TemplateCompilerTests
{
  #region Tests

  [Fact]
  public void Compile_ShouldReturnSingleConstantSegment_WhenDoubleEscapedDelimitersAreInMiddleOfConstantSegments()
  {
    var escapedText = "This is a $$$$ template.";
    var unescapedText = "This is a $$ template.";

    var compiler = new TemplateCompiler();
    var template = compiler.Compile( escapedText );

    template.Should()
            .NotBeNull();

    template.Text.Should()
            .Be( unescapedText );

    template.Segments.Should()
            .HaveCount( 1 );

    template.Segments[0]
            .Should()
            .Match<Segment>( s => s.Kind == SegmentKind.Constant && s.Text == unescapedText );
  }

  [Fact]
  public void Compile_ShouldReturnSingleConstantSegment_WhenEscapedDelimiterIsMiddleOfConstantSegments()
  {
    var escapedText = "This is a $$ template.";
    var unescapedText = "This is a $ template.";

    var compiler = new TemplateCompiler();
    var compile = compiler.Compile( escapedText );

    compile.Should()
           .NotBeNull();

    compile.Text.Should()
           .Be( unescapedText );

    compile.Segments.Should()
           .HaveCount( 1 );

    compile.Segments[0]
           .Should()
           .Match<Segment>( s => s.Kind == SegmentKind.Constant && s.Text == unescapedText );
  }

  [Fact]
  public void Compile_ShouldReturnSingleConstantSegment_WhenStringEndsWithEscapedDelimiterFollowingConstantSegment()
  {
    var escapedText = "template $$";
    var unescapedText = "template $";

    var compiler = new TemplateCompiler();
    var template = compiler.Compile( escapedText );

    template.Should()
            .NotBeNull();

    template.Text.Should()
            .Be( unescapedText );

    template.Segments.Should()
            .HaveCount( 1 );

    template.Segments[0]
            .Should()
            .Match<Segment>( s => s.Kind == SegmentKind.Constant && s.Text == unescapedText );
  }

  [Fact]
  public void Compile_ShouldReturnSingleConstantSegment_WhenStringStartsWithEscapedDelimiterFollowedByConstantSegment()
  {
    var escapedText = "$$ template.";
    var unescapedText = "$ template.";

    var compiler = new TemplateCompiler();
    var template = compiler.Compile( escapedText );

    template.Should()
            .NotBeNull();

    template.Text.Should()
            .Be( unescapedText );

    template.Segments.Should()
            .HaveCount( 1 );

    template.Segments[0]
            .Should()
            .Match<Segment>( s => s.Kind == SegmentKind.Constant && s.Text == unescapedText );
  }

  [Fact]
  public void Compile_ShouldReturnSingleConstantSegment_WhenTemplateHasNoMacros()
  {
    var text = "I have no macros";

    var compiler = new TemplateCompiler();
    var template = compiler.Compile( text );

    template.Should()
            .NotBeNull();

    template.Text.Should()
            .Be( text );

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

    template.Text.Should()
            .Be( text );

    template.Segments.Should()
            .HaveCount( 1 );

    template.Segments.Single()
            .Should()
            .Match<Segment>( s => s.Kind == SegmentKind.Macro && s.Text == text );
  }

  [Fact]
  public void Compile_ShouldReturnThreeSegments_WhenEscapedDelimiterIsBetweenMacroSegments()
  {
    var escapedTemplate = "$macro$$$$macro$";
    var unescapedTemplate = "$macro$$$macro$";

    var compiler = new TemplateCompiler();
    var template = compiler.Compile( escapedTemplate );

    template.Should()
            .NotBeNull();

    template.Text.Should()
            .Be( unescapedTemplate );

    template.Segments.Should()
            .HaveCount( 3 );

    template.Segments[0]
            .Should()
            .Match<Segment>( s => s.Kind == SegmentKind.Macro && s.Text == "$macro$" );

    template.Segments[1]
            .Should()
            .Match<Segment>( s => s.Kind == SegmentKind.Constant && s.Text == "$" );

    template.Segments[2]
            .Should()
            .Match<Segment>( s => s.Kind == SegmentKind.Macro && s.Text == "$macro$" );
  }

  [Fact]
  public void Compile_ShouldReturnThreeSegments_WhenTemplateContainsMacroInMiddle()
  {
    var text = "This is a $macro$ template.";

    var compiler = new TemplateCompiler();
    var template = compiler.Compile( text );

    template.Should()
            .NotBeNull();

    template.Text.Should()
            .Be( text );

    template.Segments.Should()
            .HaveCount( 3 );

    template.Segments[0]
            .Should()
            .Match<Segment>( static s => s.Kind == SegmentKind.Constant && s.Text == "This is a " );

    template.Segments[1]
            .Should()
            .Match<Segment>( static s => s.Kind == SegmentKind.Macro && s.Text == "$macro$" );

    template.Segments[2]
            .Should()
            .Match<Segment>( static s => s.Kind == SegmentKind.Constant && s.Text == " template." );
  }

  #endregion
}
