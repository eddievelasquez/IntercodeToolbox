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
  public void Compile_ShouldReturnSingleConstantSegment_WhenEscapedDelimiterIsMiddleOfConstantSegments()
  {
    var escapedTemplate = "This is a $$ template.";
    var unescapedTemplate = "This is a $ template.";

    var compiler = new TemplateCompiler();
    var compiledTemplate = compiler.Compile( escapedTemplate );

    compiledTemplate.Should()
                    .NotBeNull();

    compiledTemplate.Template.Should()
                    .Be( unescapedTemplate );

    compiledTemplate.Segments.Should()
                    .HaveCount( 1 );

    compiledTemplate.Segments[0]
                    .Should()
                    .Match<Segment>( s => s.Kind == SegmentKind.Constant && s.Text == unescapedTemplate );
  }

  [Fact]
  public void Compile_ShouldReturnThreeSegments_WhenEscapedDelimiterIsBetweenMacroSegments()
  {
    var escapedTemplate = "$macro$$$$macro$";
    var unescapedTemplate = "$macro$$$macro$";

    var compiler = new TemplateCompiler();
    var compiledTemplate = compiler.Compile( escapedTemplate );

    compiledTemplate.Should()
                    .NotBeNull();

    compiledTemplate.Template.Should()
                    .Be( unescapedTemplate );

    compiledTemplate.Segments.Should()
                    .HaveCount( 3 );

    compiledTemplate.Segments[0]
                    .Should()
                    .Match<Segment>( s => s.Kind == SegmentKind.Macro && s.Text == "$macro$" );

    compiledTemplate.Segments[1]
                    .Should()
                    .Match<Segment>( s => s.Kind == SegmentKind.Constant && s.Text == "$" );

    compiledTemplate.Segments[2]
                    .Should()
                    .Match<Segment>( s => s.Kind == SegmentKind.Macro && s.Text == "$macro$" );
  }

  [Fact]
  public void Compile_ShouldReturnSingleConstantSegment_WhenDoubleEscapedDelimitersAreInMiddleOfConstantSegments()
  {
    var escapedTemplate = "This is a $$$$ template.";
    var unescapedTemplate = "This is a $$ template.";

    var compiler = new TemplateCompiler();
    var compiledTemplate = compiler.Compile( escapedTemplate );

    compiledTemplate.Should()
                    .NotBeNull();

    compiledTemplate.Template.Should()
                    .Be( unescapedTemplate );

    compiledTemplate.Segments.Should()
                    .HaveCount( 1 );

    compiledTemplate.Segments[0]
                    .Should()
                    .Match<Segment>( s => s.Kind == SegmentKind.Constant && s.Text == unescapedTemplate );
  }

  [Fact]
  public void Compile_ShouldReturnSingleConstantSegment_WhenStringStartsWithEscapedDelimiterFollowedByConstantSegment()
  {
    var escapedTemplate = "$$ template.";
    var unescapedTemplate = "$ template.";

    var compiler = new TemplateCompiler();
    var compiledTemplate = compiler.Compile( escapedTemplate );

    compiledTemplate.Should()
                    .NotBeNull();

    compiledTemplate.Template.Should()
                    .Be( unescapedTemplate );

    compiledTemplate.Segments.Should()
                    .HaveCount( 1 );

    compiledTemplate.Segments[0]
                    .Should()
                    .Match<Segment>( s => s.Kind == SegmentKind.Constant && s.Text == unescapedTemplate );
  }

  [Fact]
  public void Compile_ShouldReturnSingleConstantSegment_WhenStringEndsWithEscapedDelimiterFollowingConstantSegment()
  {
    var escapedTemplate = "template $$";
    var unescapedTemplate = "template $";

    var compiler = new TemplateCompiler();
    var compiledTemplate = compiler.Compile( escapedTemplate );

    compiledTemplate.Should()
                    .NotBeNull();

    compiledTemplate.Template.Should()
                    .Be( unescapedTemplate );

    compiledTemplate.Segments.Should()
                    .HaveCount( 1 );

    compiledTemplate.Segments[0]
                    .Should()
                    .Match<Segment>( s => s.Kind == SegmentKind.Constant && s.Text == unescapedTemplate );
  }

  [Fact]
  public void Compile_ShouldReturnSingleConstantSegment_WhenTemplateHasNoMacros()
  {
    var template = "I have no macros";

    var compiler = new TemplateCompiler();
    var compiledTemplate = compiler.Compile( template );

    compiledTemplate.Should()
                    .NotBeNull();

    compiledTemplate.Template.Should()
                    .Be( template );

    compiledTemplate.Segments.Should()
                    .HaveCount( 1 );

    compiledTemplate.Segments.Single()
                    .Should()
                    .Match<Segment>( s => s.Kind == SegmentKind.Constant && s.Text == template );
  }

  [Fact]
  public void Compile_ShouldReturnSingleMacroSegment_WhenTemplateIsOnlyTheMacro()
  {
    var template = "$macro$";

    var compiler = new TemplateCompiler();
    var compiledTemplate = compiler.Compile( template );

    compiledTemplate.Should()
                    .NotBeNull();

    compiledTemplate.Template.Should()
                    .Be( template );

    compiledTemplate.Segments.Should()
                    .HaveCount( 1 );

    compiledTemplate.Segments.Single()
                    .Should()
                    .Match<Segment>( s => s.Kind == SegmentKind.Macro && s.Text == template );
  }

  [Fact]
  public void Compile_ShouldReturnThreeSegments_WhenTemplateContainsMacroInMiddle()
  {
    var template = "This is a $macro$ template.";

    var compiler = new TemplateCompiler();
    var compiledTemplate = compiler.Compile( template );

    compiledTemplate.Should()
                    .NotBeNull();

    compiledTemplate.Template.Should()
                    .Be( template );

    compiledTemplate.Segments.Should()
                    .HaveCount( 3 );

    compiledTemplate.Segments[0]
                    .Should()
                    .Match<Segment>( static s => s.Kind == SegmentKind.Constant && s.Text == "This is a " );

    compiledTemplate.Segments[1]
                    .Should()
                    .Match<Segment>( static s => s.Kind == SegmentKind.Macro && s.Text == "$macro$" );

    compiledTemplate.Segments[2]
                    .Should()
                    .Match<Segment>( static s => s.Kind == SegmentKind.Constant && s.Text == " template." );
  }

  #endregion
}
