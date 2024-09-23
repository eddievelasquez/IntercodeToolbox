// Module Name: TemplateCompilerTests.cs
// Author:      Eduardo Velasquez
// Copyright (c) 2024, Intercode Consulting, Inc.

namespace Intercode.Toolbox.TypedPrimitives.Tests;

using FluentAssertions;

public class TemplateCompilerTests
{
  #region Tests

  [Fact]
  public void Compile_ShouldReturnEmptyMacro_WhenTemplateHasEscapedDelimiter()
  {
    var template = "This is a $$ template.";

    var compiler = new TemplateCompiler();
    var compiledTemplate = compiler.Compile( template );

    compiledTemplate.Should()
                    .NotBeNull();

    compiledTemplate.Template.Should()
                    .Be( template );

    compiledTemplate.ConstantTextLength.Should()
                    .Be( 10 + 10 );

    compiledTemplate.Segments.Should()
                    .HaveCount( 3 );

    compiledTemplate.Segments[0]
                    .Should()
                    .Match<Segment>( static s => s.Type == SegmentType.ConstantText && s.Text == "This is a " );

    compiledTemplate.Segments[1]
                    .Should()
                    .Match<Segment>( static s => s.Type == SegmentType.Macro && s.Text == "$$" );

    compiledTemplate.Segments[2]
                    .Should()
                    .Match<Segment>( static s => s.Type == SegmentType.ConstantText && s.Text == " template." );
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

    compiledTemplate.ConstantTextLength.Should()
                    .Be( 16 );

    compiledTemplate.Segments.Should()
                    .HaveCount( 1 );

    compiledTemplate.Segments.Single()
                    .Should()
                    .Match<Segment>( s => s.Type == SegmentType.ConstantText && s.Text == template );
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

    compiledTemplate.ConstantTextLength.Should()
                    .Be( 0 );

    compiledTemplate.Segments.Should()
                    .HaveCount( 1 );

    compiledTemplate.Segments.Single()
                    .Should()
                    .Match<Segment>( s => s.Type == SegmentType.Macro && s.Text == template );
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

    compiledTemplate.ConstantTextLength.Should()
                    .Be( 10 + 10 );

    compiledTemplate.Segments.Should()
                    .HaveCount( 3 );

    compiledTemplate.Segments[0]
                    .Should()
                    .Match<Segment>( static s => s.Type == SegmentType.ConstantText && s.Text == "This is a " );

    compiledTemplate.Segments[1]
                    .Should()
                    .Match<Segment>( static s => s.Type == SegmentType.Macro && s.Text == "$macro$" );

    compiledTemplate.Segments[2]
                    .Should()
                    .Match<Segment>( static s => s.Type == SegmentType.ConstantText && s.Text == " template." );
  }

  #endregion
}
