// Module Name: TemplateCompilerTests.cs
// Author:      Eduardo Velasquez
// Copyright (c) 2025, Intercode Consulting, Inc.

namespace Intercode.Toolbox.TemplateEngine.Tests;

using Intercode.Toolbox.TemplateEngine.Tests.FluentAssertions;

public class TemplateCompilerTests
{
  #region Tests

  [Fact]
  public void Compile_ShouldHandleEscapedDelimiter_WhenStringEndsWithEscapedDelimiter()
  {
    var macroTable = DefineMacros();
    var template = TemplateCompiler.Compile( macroTable, "template $$" );

    template.Should()
            .HaveSingleSegment()
            .Which
            .BeConstant( "template $" );
  }

  [Fact]
  public void Compile_ShouldHandleEscapedDelimiter_WhenStringStartsWithEscapedDelimiter()
  {
    const string Text = "$$ template.";

    var macroTable = DefineMacros();
    var template = TemplateCompiler.Compile( macroTable, Text );

    template.Should()
            .HaveSingleSegment()
            .Which
            .BeConstant( "$ template." );
  }

  [Fact]
  public void Compile_ShouldHandleEscapedDelimiters_WhenStringHasEscapedDelimitersInMiddle()
  {
    var macroTable = DefineMacros();
    var template = TemplateCompiler.Compile( macroTable, "012345$$$$012345" );

    template.Should().HaveSegmentCount( 2 );
    template.Should().HaveSegmentAt( 0 ).Which.BeConstant( "012345$" );
    template.Should().HaveSegmentAt( 1 ).Which.BeConstant( "$012345" );
  }

  [Fact]
  public void Compile_ShouldHandleIncludeWithEmptyStringContent()
  {
    var includes = new IncludesCollection();
    includes.AddInclude( "empty", string.Empty );

    var macroTable = DefineMacros();
    var template = TemplateCompiler.Compile( macroTable, "$empty$", includes );

    template.Text.Should().BeEmpty();

    template.Should().HaveSingleSegment().Which.BeConstant( string.Empty );
  }

  [Fact]
  public void Compile_ShouldHandleIncludeWithNullGenerator()
  {
    var includes = new IncludesCollection();
    includes.AddInclude( "empty", ( MacroValueGenerator? ) null );

    var macroTable = DefineMacros();
    var template = TemplateCompiler.Compile( macroTable, "$empty$", includes );

    template.Text.Should().BeEmpty();
    template.Should().HaveSingleSegment().Which.BeConstant( string.Empty );
  }

  [Fact]
  public void Compile_ShouldHandleIncludeWithNullStringContent()
  {
    var includes = new IncludesCollection();
    includes.AddInclude( "empty", ( string? ) null );

    var macroTable = DefineMacros();
    var template = TemplateCompiler.Compile( macroTable, "$empty$", includes );

    template.Text.Should().BeEmpty();
    template.Should().HaveSingleSegment().Which.BeConstant( string.Empty );
  }

  [Fact]
  public void Compile_ShouldHandleMacroWithArgument()
  {
    var macroTable = DefineMacros( "macro" );
    var template = TemplateCompiler.Compile( macroTable, "$macro:argument$" );

    template.Should().HaveSingleSegment().Which.BeMacro( "macro", "argument" );
  }

  [Fact]
  public void Compile_ShouldHandleNullIncludesParameter()
  {
    const string Text = "$macro$";
    var macroTable = DefineMacros( "macro" );
    var template = TemplateCompiler.Compile( macroTable, Text, null );

    template.Should().HaveText( Text );
    template.Should().HaveSingleSegment().Which.BeMacro( "macro" );
  }

  [Fact]
  public void Compile_ShouldLeaveUnknownIncludeAsMacro_WhenIncludesProvidedButNameMissing()
  {
    var includes = new IncludesCollection();
    includes.AddInclude( "present", "X" );

    var macroTable = DefineMacros( "missing" );
    var template = TemplateCompiler.Compile( macroTable, "$missing$", includes );

    template.Should().HaveText( "$missing$" );
    template.Should().HaveSingleSegment().Which.BeMacro( "missing" );
  }

  [Fact]
  public void
    Compile_ShouldNotRecursivelyExpandIncludes_WhenIncludeContentReferencesAnotherInclude()
  {
    var includes = new IncludesCollection();
    includes.AddInclude( "a", "$b$" );
    includes.AddInclude( "b", "B" );

    var macroTable = DefineMacros( "b" );
    var template = TemplateCompiler.Compile( macroTable, "$a$", includes );

    template.Should().HaveText( "$b$" );
    template.Should().HaveSingleSegment().Which.BeMacro( "b" );
  }

  [Fact]
  public void Compile_ShouldNotReplaceAnything_WhenIncludesProvidedButNotReferenced()
  {
    var includes = new IncludesCollection();
    includes.AddInclude( "unused", "UnusedContent" );

    const string Text = "No macros here";
    var macroTable = DefineMacros();
    var template = TemplateCompiler.Compile( macroTable, Text, includes );

    template.Should().HaveText( Text );
    template.Should().HaveSingleSegment().Which.BeConstant( Text );
  }

  [Fact]
  public void Compile_ShouldProcessEscapedDelimitersInIncludeContent()
  {
    var includes = new IncludesCollection();
    includes.AddInclude( "section", "Start $$ End" );

    var macroTable = DefineMacros();
    var template = TemplateCompiler.Compile( macroTable, "$section$", includes );

    template.Should().HaveText( "Start $$ End" );
    template.Should().HaveSegmentCount( 2 );

    template.Should().HaveSegmentAt( 0 ).Which.BeConstant( "Start $" );
    template.Should().HaveSegmentAt( 1 ).Which.BeConstant( " End" );
  }

  [Fact]
  public void Compile_ShouldProcessIncludeWithMacroArgument()
  {
    var includes = new IncludesCollection();
    includes.AddInclude( "section", "Start $macro:arg$ End" );

    var macroTable = DefineMacros( "macro" );
    var template = TemplateCompiler.Compile( macroTable, "$section$", includes );

    template.Should().HaveText( "Start $macro:arg$ End" );
    template.Should().HaveSegmentCount( 3 );

    template.Should().HaveSegmentAt( 0 ).Which.BeConstant( "Start " );
    template.Should().HaveSegmentAt( 1 ).Which.BeMacro( "macro", "arg" );
    template.Should().HaveSegmentAt( 2 ).Which.BeConstant( " End" );
  }

  [Fact]
  public void Compile_ShouldProcessMacrosInIncludeContent()
  {
    var includes = new IncludesCollection();
    includes.AddInclude( "section", "Start $macro$ End" );

    var macroTable = DefineMacros( "macro" );
    var template = TemplateCompiler.Compile( macroTable, "$section$", includes );

    template.Should().HaveText( "Start $macro$ End" );
    template.Should().HaveSegmentCount( 3 );
    template.Should().HaveSegmentAt( 0 ).Which.BeConstant( "Start " );
    template.Should().HaveSegmentAt( 1 ).Which.BeMacro( "macro" );
    template.Should().HaveSegmentAt( 2 ).Which.BeConstant( " End" );
  }

  [Fact]
  public void Compile_ShouldProcessMacrosInMultipleIncludes()
  {
    var includes = new IncludesCollection();
    includes.AddInclude( "a", "A$macroA$" );
    includes.AddInclude( "b", "B$macroB$" );

    var macroTable = DefineMacros( "macroA", "macroB" );
    var template = TemplateCompiler.Compile( macroTable, "$a$-$b$", includes );

    template.Should().HaveText( "A$macroA$-B$macroB$" );
    template.Should().HaveSegmentCount( 4 );

    template.Should().HaveSegmentAt( 0 ).Which.BeConstant( "A" );
    template.Should().HaveSegmentAt( 1 ).Which.BeMacro( "macroA" );
    template.Should().HaveSegmentAt( 2 ).Which.BeConstant( "-B" );
    template.Should().HaveSegmentAt( 3 ).Which.BeMacro( "macroB" );
  }

  [Fact]
  public void Compile_ShouldReplaceInclude_WhenIncludeIsReferencedInTemplate()
  {
    var includes = new IncludesCollection();
    includes.AddInclude( "header", "HeaderContent" );

    var macroTable = DefineMacros();
    var template = TemplateCompiler.Compile( macroTable, "$header$ body", includes );

    template.Should().HaveText( "HeaderContent body" );

    template.Should().HaveSingleSegment().Which.BeConstant( "HeaderContent body" );
  }

  [Fact]
  public void Compile_ShouldReplaceMultipleIncludes_WhenMultipleAreReferenced()
  {
    var includes = new IncludesCollection();
    includes.AddInclude( "a", "A" );
    includes.AddInclude( "b", "B" );

    var macroTable = DefineMacros();
    var template = TemplateCompiler.Compile( macroTable, "$a$-$b$", includes );

    template.Should().HaveText( "A-B" );
    template.Should().HaveSingleSegment().Which.BeConstant( "A-B" );
  }

  [Fact]
  public void Compile_ShouldReplaceWithEmpty_WhenIncludeIsReferencedButDoesNotExist()
  {
    const string Text = "$missing$";
    var macroTable = DefineMacros( "missing" );
    var template = TemplateCompiler.Compile( macroTable, Text, new IncludesCollection() );

    template.Should().HaveText( Text );
    template.Should().HaveSingleSegment().Which.BeMacro( "missing" );
  }

  [Fact]
  public void Compile_ShouldReturnConstantSegment_WhenTextIsOnlyDelimiter()
  {
    const string Text = "$";
    var macroTable = DefineMacros();
    var template = TemplateCompiler.Compile( macroTable, Text );

    template.Should().HaveSingleSegment().Which.BeConstant( "$" );
  }

  [Fact]
  public void Compile_ShouldReturnConstantSegment_WhenTextIsUnclosedMacro()
  {
    const string Text = "$macro";
    var macroTable = DefineMacros();
    var template = TemplateCompiler.Compile( macroTable, Text );

    template.Should().HaveSingleSegment().Which.BeConstant( "$macro" );
  }

  [Fact]
  public void Compile_ShouldReturnDelimiterSegments_WhenTextIsMultipleEscapedDelimitersOnly()
  {
    const string Text = "$$$$";
    var macroTable = DefineMacros();
    var template = TemplateCompiler.Compile( macroTable, Text );

    template.Should().HaveSingleSegment().Which.BeConstant( "$$" );
  }

  [Fact]
  public void Compile_ShouldReturnMacroSegment_WhenIncludeCollectionIsEmptyAndIncludeReferenced()
  {
    var includes = new IncludesCollection();
    var macroTable = DefineMacros( "missing" );
    var template = TemplateCompiler.Compile( macroTable, "$missing$", includes );

    template.Should().HaveText( "$missing$" );
    template.Should().HaveSingleSegment().Which.BeMacro( "missing" );
  }

  [Fact]
  public void Compile_ShouldReturnOneSegment_WhenConstantFollowedByEscapedDelimiter()
  {
    var macroTable = DefineMacros();
    var template = TemplateCompiler.Compile( macroTable, "text$$" );

    template.Should().HaveSingleSegment().Which.BeConstant( "text$" );
  }

  [Fact]
  public void Compile_ShouldReturnOneSegment_WhenConstantFollowedByOpenMacro()
  {
    var macroTable = DefineMacros();
    var template = TemplateCompiler.Compile( macroTable, "text$" );

    template.Should().HaveSingleSegment().Which.BeConstant( "text$" );
  }

  [Fact]
  public void Compile_ShouldReturnOneSegment_WhenEscapedDelimiterFollowedByConstant()
  {
    var macroTable = DefineMacros();
    var template = TemplateCompiler.Compile( macroTable, "$$text" );

    template.Should().HaveSingleSegment().Which.BeConstant( "$text" );
  }

  [Fact]
  public void Compile_ShouldReturnOneSegment_WhenOnlyConstant()
  {
    var macroTable = DefineMacros();
    var template = TemplateCompiler.Compile( macroTable, "text" );

    template.Should().HaveSingleSegment().Which.BeConstant( "text" );
  }

  [Fact]
  public void Compile_ShouldReturnOneSegment_WhenOpenMacroFollowedByConstant()
  {
    var macroTable = DefineMacros();
    var template = TemplateCompiler.Compile( macroTable, "$text" );

    template.Should().HaveSingleSegment().Which.BeConstant( "$text" );
  }

  [Fact]
  public void Compile_ShouldReturnSingleConstantSegment_WhenTemplateHasNoMacros()
  {
    const string Text = "I have no macros";

    var macroTable = DefineMacros();
    var template = TemplateCompiler.Compile( macroTable, Text );

    template.Should().HaveSingleSegment().Which.BeConstant( Text );
  }

  [Fact]
  public void Compile_ShouldReturnSingleMacroSegment_WhenTemplateIsOnlyTheMacro()
  {
    const string Text = "$macro$";

    var macroTable = DefineMacros( "macro" );
    var template = TemplateCompiler.Compile( macroTable, Text );

    template.Should().HaveSingleSegment().Which.BeMacro( "macro" );
  }

  [Fact]
  public void Compile_ShouldReturnThreeMacroValues_WhenTemplateContainsThreeDifferentMacros()
  {
    const string Text = "$macroA$$macroB$$macroC$";

    var macroTable = DefineMacros( "macroA", "macroB", "macroC" );
    var template = TemplateCompiler.Compile( macroTable, Text );

    template.Should().HaveSegmentCount( 3 );

    template.Should().HaveSegmentAt( 0 ).Which.BeMacro( "macroA" ).And.HaveSlot( 0 );
    template.Should().HaveSegmentAt( 1 ).Which.BeMacro( "macroB" ).And.HaveSlot( 1 );
    template.Should().HaveSegmentAt( 2 ).Which.BeMacro( "macroC" ).And.HaveSlot( 2 );
  }

  [Fact]
  public void Compile_ShouldReturnThreeSegments_WhenConstantFollowedByMacroAndConstant()
  {
    var macroTable = DefineMacros( "macro" );
    var template = TemplateCompiler.Compile( macroTable, "prefix$macro$suffix" );

    template.Should().HaveSegmentCount( 3 );

    template.Should().HaveSegmentAt( 0 ).Which.BeConstant( "prefix" );
    template.Should().HaveSegmentAt( 1 ).Which.BeMacro( "macro" ).And.HaveSlot( 0 );
    template.Should().HaveSegmentAt( 2 ).Which.BeConstant( "suffix" );
  }

  [Fact]
  public void Compile_ShouldReturnThreeSegments_WhenEscapedDelimiterIsBetweenMacroSegments()
  {
    const string Text = "$macro$$$$macro$";

    var macroTable = DefineMacros( "macro" );
    var template = TemplateCompiler.Compile( macroTable, Text );

    template.Should().HaveSegmentCount( 3 );

    template.Should().HaveSegmentAt( 0 ).Which.BeMacro( "macro" );
    template.Should().HaveSegmentAt( 1 ).Which.BeConstant( "$" );
    template.Should().HaveSegmentAt( 2 ).Which.BeMacro( "macro" );
  }

  [Fact]
  public void Compile_ShouldReturnThreeSegments_WhenTemplateContainsMacroInMiddle()
  {
    const string Text = "This is a $macro$ template.";

    var macroTable = DefineMacros( "macro" );
    var template = TemplateCompiler.Compile( macroTable, Text );

    template.Should().HaveSegmentCount( 3 );

    template.Should().HaveSegmentAt( 0 ).Which.BeConstant( "This is a " );
    template.Should().HaveSegmentAt( 1 ).Which.BeMacro( "macro" );
    template.Should().HaveSegmentAt( 2 ).Which.BeConstant( " template." );
  }

  [Fact]
  public void
    Compile_ShouldReturnTwoMacroValues_WhenTemplateContainsTwoDifferentMacrosAndOneRepeated()
  {
    const string Text = "$macroA$$macroB$$macroA$";

    var macroTable = DefineMacros( "macroA", "macroB" );
    var template = TemplateCompiler.Compile( macroTable, Text );

    template.Should().HaveSegmentCount( 3 );

    template.Should().HaveSegmentAt( 0 ).Which.BeMacro( "macroA" ).And.HaveSlot( 0 );
    template.Should().HaveSegmentAt( 1 ).Which.BeMacro( "macroB" ).And.HaveSlot( 1 );
    template.Should().HaveSegmentAt( 2 ).Which.BeMacro( "macroA" ).And.HaveSlot( 0 );
  }

  [Fact]
  public void Compile_ShouldReturnTwoSegments_WhenConstantFollowedByMacro()
  {
    var macroTable = DefineMacros( "macro" );
    var template = TemplateCompiler.Compile( macroTable, "text$macro$" );

    template.Should().HaveSegmentCount( 2 );

    template.Should().HaveSegmentAt( 0 ).Which.BeConstant( "text" );
    template.Should().HaveSegmentAt( 1 ).Which.BeMacro( "macro" ).And.HaveSlot( 0 );
  }

  [Fact]
  public void Compile_ShouldSupportCustomDelimiterAndArgumentSeparator()
  {
    var options = new TemplateCompilerOptions( '#', ':' );
    var macroTable = new MacroTableBuilder().Declare( "macro" ).Build();
    var template = TemplateCompiler.Compile( macroTable, "#macro:arg#", options: options );

    template.Should().HaveSingleSegment().Which.BeMacro( "macro", "arg" );
  }

  [Fact]
  public void Compile_ShouldThrow_WhenMacroNotDeclared()
  {
    var macroTable = DefineMacros();
    var act = () => TemplateCompiler.Compile( macroTable, "$unknown$" );

    act.Should().Throw<InvalidOperationException>().WithMessage( "Undefined macro: 'unknown'" );
  }

  [Fact]
  public void Compile_ShouldThrowArgumentException_WhenMacroNameIsEmptyButHasArgument()
  {
    var macroTable = DefineMacros( "x" );

    Action act = () => TemplateCompiler.Compile( macroTable, "$:arg$" );
    act.Should().Throw<InvalidOperationException>().WithMessage( "The macro name cannot be empty" );
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
       .WithParameterName( "text" )
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
