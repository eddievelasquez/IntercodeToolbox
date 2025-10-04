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
    var template = TemplateCompiler.Compile( "template $$" );

    template.Should()
            .HaveSingleSegment()
            .Which
            .BeConstant( "template $" );
  }

  [Fact]
  public void
    Compile_ShouldHandleEscapedDelimiter_WhenStringStartsWithEscapedDelimiter()
  {
    const string Text = "$$ template.";

    var template = TemplateCompiler.Compile( Text );

    template.Should()
            .HaveSingleSegment()
            .Which
            .BeConstant( "$ template." );
  }

  [Fact]
  public void
    Compile_ShouldHandleEscapedDelimiters_WhenStringHasEscapedDelimitersInMiddle()
  {
    var template = TemplateCompiler.Compile( "012345$$$$012345" );

    template.Should().HaveSegmentCount( 2 );
    template.Should().HaveSegmentAt( 0 ).Which.BeConstant( "012345$" );
    template.Should().HaveSegmentAt( 1 ).Which.BeConstant( "$012345" );
  }

  [Fact]
  public void Compile_ShouldHandleIncludeWithNullGenerator()
  {
    var includes = new IncludesCollection();
    includes.AddInclude( "empty", ( MacroValueGenerator? ) null );

    var template = TemplateCompiler.Compile( "$empty$", includes );

    template.Text.Should().BeEmpty();
    template.Should().HaveSingleSegment().Which.BeConstant( string.Empty );
  }

  [Fact]
  public void Compile_ShouldHandleIncludeWithNullStringContent()
  {
    var includes = new IncludesCollection();
    includes.AddInclude( "empty", ( string? ) null );

    var template = TemplateCompiler.Compile( "$empty$", includes );

    template.Text.Should().BeEmpty();
    template.Should().HaveSingleSegment().Which.BeConstant( string.Empty );
  }

  [Fact]
  public void Compile_ShouldHandleMacroWithArgument()
  {
    var template = TemplateCompiler.Compile( "$macro:argument$" );

    template.Should().HaveSingleSegment().Which.BeMacro( "macro", "argument" );
  }

  [Fact]
  public void Compile_ShouldHandleNullIncludesParameter()
  {
    const string Text = "$macro$";
    var template = TemplateCompiler.Compile( Text, includes: null );

    template.Should().HaveText( Text );
    template.Should().HaveSingleSegment().Which.BeMacro( "macro" );
  }

  [Fact]
  public void Compile_ShouldLeaveUnclosedIncludePlaceholderUnchanged()
  {
    var includes = new IncludesCollection();
    includes.AddInclude( "x", "X" );

    const string Text = "prefix $x";
    var template = TemplateCompiler.Compile( Text, includes );

    template.Should().HaveText( Text );
    template.Should().HaveSingleSegment().Which.BeConstant( Text );
  }

  [Fact]
  public void
    Compile_ShouldNotRecursivelyExpandIncludes_WhenIncludeContentReferencesAnotherInclude()
  {
    var includes = new IncludesCollection();
    includes.AddInclude( "a", "$b$" );
    includes.AddInclude( "b", "B" );

    var template = TemplateCompiler.Compile( "$a$", includes );

    template.Should().HaveText( "$b$" );
    template.Should().HaveSingleSegment().Which.BeMacro( "b" );
  }

  [Fact]
  public void Compile_ShouldNotReplaceAnything_WhenIncludesProvidedButNotReferenced()
  {
    var includes = new IncludesCollection();
    includes.AddInclude( "unused", "UnusedContent" );

    const string Text = "No macros here";
    var template = TemplateCompiler.Compile( Text, includes );

    template.Should().HaveText( Text );
    template.Should().HaveSingleSegment().Which.BeConstant( Text );
  }

  [Fact]
  public void
    Compile_ShouldNotTreatPlaceholderWithArgumentAsInclude_WhenNameMatchesInclude()
  {
    var includes = new IncludesCollection();
    includes.AddInclude( "x", "INC" );

    var template = TemplateCompiler.Compile( "$x:arg$", includes );

    template.Should().HaveText( "$x:arg$" );
    template.Should().HaveSingleSegment().Which.BeMacro( "x", "arg" );
  }

  [Fact]
  public void Compile_ShouldProcessEscapedDelimitersInIncludeContent()
  {
    var includes = new IncludesCollection();
    includes.AddInclude( "section", "Start $$ End" );

    var template = TemplateCompiler.Compile( "$section$", includes );

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

    var template = TemplateCompiler.Compile( "$section$", includes );

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

    var template = TemplateCompiler.Compile( "$section$", includes );

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

    var template = TemplateCompiler.Compile( "$a$-$b$", includes );

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

    var template = TemplateCompiler.Compile( "$header$ body", includes );

    template.Should().HaveText( "HeaderContent body" );

    template.Should().HaveSingleSegment().Which.BeConstant( "HeaderContent body" );
  }

  [Fact]
  public void Compile_ShouldReplaceInclude_WithCustomDelimiter()
  {
    var includes = new IncludesCollection();
    includes.AddInclude( "sect", "X" );

    var options = new TemplateCompilerOptions( '#', ':' );
    var template = TemplateCompiler.Compile( "#sect#", includes, options );

    template.Should().HaveText( "X" );
    template.Should().HaveSingleSegment().Which.BeConstant( "X" );
  }

  [Fact]
  public void Compile_ShouldReplaceMultipleIncludes_WhenMultipleAreReferenced()
  {
    var includes = new IncludesCollection();
    includes.AddInclude( "a", "A" );
    includes.AddInclude( "b", "B" );

    var template = TemplateCompiler.Compile( "$a$-$b$", includes );

    template.Should().HaveText( "A-B" );
    template.Should().HaveSingleSegment().Which.BeConstant( "A-B" );
  }

  [Fact]
  public void Compile_ShouldReplaceWithEmpty_WhenIncludeIsReferencedButDoesNotExist()
  {
    const string Text = "$missing$";
    var template = TemplateCompiler.Compile( Text, new IncludesCollection() );

    template.Should().HaveText( Text );
    template.Should().HaveSingleSegment().Which.BeMacro( "missing" );
  }

  [Fact]
  public void Compile_ShouldReturnConstantSegment_WhenTextIsOnlyDelimiter()
  {
    const string Text = "$";
    var template = TemplateCompiler.Compile( Text );

    template.Should().HaveSingleSegment().Which.BeConstant( "$" );
  }

  [Fact]
  public void Compile_ShouldReturnConstantSegment_WhenTextIsUnclosedMacro()
  {
    const string Text = "$macro";
    var template = TemplateCompiler.Compile( Text );

    template.Should().HaveSingleSegment().Which.BeConstant( "$macro" );
  }

  [Fact]
  public void Compile_ShouldReturnDelimiterSegments_WhenTextIsMultipleEscapedDelimitersOnly()
  {
    const string Text = "$$$$";
    var template = TemplateCompiler.Compile( Text );

    template.Should().HaveSingleSegment().Which.BeConstant( "$$" );
  }

  [Fact]
  public void Compile_ShouldReturnMacroSegment_WhenIncludeCollectionIsEmptyAndIncludeReferenced()
  {
    var includes = new IncludesCollection();
    var template = TemplateCompiler.Compile( "$missing$", includes );

    template.Should().HaveText( "$missing$" );
    template.Should().HaveSingleSegment().Which.BeMacro( "missing" );
  }

  [Fact]
  public void Compile_ShouldReturnOneSegment_WhenConstantFollowedByEscapedDelimiter()
  {
    var template = TemplateCompiler.Compile( "text$$" );

    template.Should().HaveSingleSegment().Which.BeConstant( "text$" );
  }

  [Fact]
  public void Compile_ShouldReturnOneSegment_WhenConstantFollowedByOpenMacro()
  {
    var template = TemplateCompiler.Compile( "text$" );

    template.Should().HaveSingleSegment().Which.BeConstant( "text$" );
  }

  [Fact]
  public void Compile_ShouldReturnOneSegment_WhenEscapedDelimiterFollowedByConstant()
  {
    var template = TemplateCompiler.Compile( "$$text" );

    template.Should().HaveSingleSegment().Which.BeConstant( "$text" );
  }

  [Fact]
  public void Compile_ShouldReturnOneSegment_WhenOnlyConstant()
  {
    var template = TemplateCompiler.Compile( "text" );

    template.Should().HaveSingleSegment().Which.BeConstant( "text" );
  }

  [Fact]
  public void Compile_ShouldReturnOneSegment_WhenOpenMacroFollowedByConstant()
  {
    var template = TemplateCompiler.Compile( "$text" );

    template.Should().HaveSingleSegment().Which.BeConstant( "$text" );
  }

  [Fact]
  public void Compile_ShouldReturnSingleConstantSegment_WhenTemplateHasNoMacros()
  {
    const string Text = "I have no macros";

    var template = TemplateCompiler.Compile( Text );

    template.Should().HaveSingleSegment().Which.BeConstant( Text );
  }

  [Fact]
  public void Compile_ShouldReturnSingleMacroSegment_WhenTemplateIsOnlyTheMacro()
  {
    const string Text = "$macro$";

    var template = TemplateCompiler.Compile( Text );

    template.Should().HaveSingleSegment().Which.BeMacro( "macro" );
  }

  [Fact]
  public void Compile_ShouldReturnThreeMacroValues_WhenTemplateContainsThreeDifferentMacros()
  {
    const string Text = "$macroA$$macroB$$macroC$";

    var template = TemplateCompiler.Compile( Text );

    template.Should().HaveSegmentCount( 3 );

    template.Should().HaveSegmentAt( 0 ).Which.BeMacro( "macroA" ).And.HaveSlot( 1 );
    template.Should().HaveSegmentAt( 1 ).Which.BeMacro( "macroB" ).And.HaveSlot( 2 );
    template.Should().HaveSegmentAt( 2 ).Which.BeMacro( "macroC" ).And.HaveSlot( 3 );
  }

  [Fact]
  public void Compile_ShouldReturnThreeSegments_WhenConstantFollowedByMacroAndConstant()
  {
    var template = TemplateCompiler.Compile( "prefix$macro$suffix" );

    template.Should().HaveSegmentCount( 3 );

    template.Should().HaveSegmentAt( 0 ).Which.BeConstant( "prefix" );
    template.Should().HaveSegmentAt( 1 ).Which.BeMacro( "macro" ).And.HaveSlot( 1 );
    template.Should().HaveSegmentAt( 2 ).Which.BeConstant( "suffix" );
  }

  [Fact]
  public void Compile_ShouldReturnThreeSegments_WhenEscapedDelimiterIsBetweenMacroSegments()
  {
    const string Text = "$macro$$$$macro$";

    var template = TemplateCompiler.Compile( Text );

    template.Should().HaveSegmentCount( 3 );

    template.Should().HaveSegmentAt( 0 ).Which.BeMacro( "macro" );
    template.Should().HaveSegmentAt( 1 ).Which.BeConstant( "$" );
    template.Should().HaveSegmentAt( 2 ).Which.BeMacro( "macro" );
  }

  [Fact]
  public void Compile_ShouldReturnThreeSegments_WhenTemplateContainsMacroInMiddle()
  {
    const string Text = "This is a $macro$ template.";

    var template = TemplateCompiler.Compile( Text );

    template.Should().HaveSegmentCount( 3 );

    template.Should().HaveSegmentAt( 0 ).Which.BeConstant( "This is a " );
    template.Should().HaveSegmentAt( 1 ).Which.BeMacro( "macro" );
    template.Should().HaveSegmentAt( 2 ).Which.BeConstant( " template." );
  }

  [Fact]
  public void Compile_ShouldReturnTwoSegments_WhenConstantFollowedByMacro()
  {
    var template = TemplateCompiler.Compile( "text$macro$" );

    template.Should().HaveSegmentCount( 2 );

    template.Should().HaveSegmentAt( 0 ).Which.BeConstant( "text" );
    template.Should().HaveSegmentAt( 1 ).Which.BeMacro( "macro" ).And.HaveSlot( 1 );
  }

  [Fact]
  public void Compile_ShouldSupportCustomDelimiterAndArgumentSeparator()
  {
    var options = new TemplateCompilerOptions( '#', ':' );
    var template = TemplateCompiler.Compile( "#macro:arg#", options: options );

    template.Should().HaveSingleSegment().Which.BeMacro( "macro", "arg" );
  }

  [Fact]
  public void
    Compile_ShouldThrowArgumentException_WhenMacroNameIsEmptyButHasArgument()
  {
    Action act = () => TemplateCompiler.Compile( "$:arg$" );
    act.Should().Throw<InvalidOperationException>().WithMessage( "The macro name cannot be empty" );
  }

  [Theory]
  [InlineData( null )]
  [InlineData( "" )]
  [InlineData( "   " )]
  public void Compile_ShouldThrowArgumentException_WhenTextIsNullOrWhitespace(
    string? text )
  {
    Action act = () => TemplateCompiler.Compile( text! );

    act.Should()
       .Throw<ArgumentException>()
       .WithParameterName( "text" )
       .WithMessage( "*cannot be null, empty, or whitespace*" );
  }

  [Fact]
  public void
    Compile_WithMacroTable_ShouldHandleEscapedDelimiter_WhenStringEndsWithEscapedDelimiter()
  {
    var macroTable = DefineMacros();
    var template = TemplateCompiler.Compile( "template $$", macroTable );

    template.Should()
            .HaveSingleSegment()
            .Which
            .BeConstant( "template $" );
  }

  [Fact]
  public void
    Compile_WithMacroTable_ShouldHandleEscapedDelimiter_WhenStringStartsWithEscapedDelimiter()
  {
    const string Text = "$$ template.";

    var macroTable = DefineMacros();
    var template = TemplateCompiler.Compile( Text, macroTable );

    template.Should()
            .HaveSingleSegment()
            .Which
            .BeConstant( "$ template." );
  }

  [Fact]
  public void
    Compile_WithMacroTable_ShouldHandleEscapedDelimiters_WhenStringHasEscapedDelimitersInMiddle()
  {
    var macroTable = DefineMacros();
    var template = TemplateCompiler.Compile( "012345$$$$012345", macroTable );

    template.Should().HaveSegmentCount( 2 );
    template.Should().HaveSegmentAt( 0 ).Which.BeConstant( "012345$" );
    template.Should().HaveSegmentAt( 1 ).Which.BeConstant( "$012345" );
  }

  [Fact]
  public void Compile_WithMacroTable_ShouldHandleIncludeWithEmptyStringContent()
  {
    var includes = new IncludesCollection();
    includes.AddInclude( "empty", string.Empty );

    var macroTable = DefineMacros();
    var template = TemplateCompiler.Compile( "$empty$", macroTable, includes );

    template.Text.Should().BeEmpty();

    template.Should().HaveSingleSegment().Which.BeConstant( string.Empty );
  }

  [Fact]
  public void Compile_WithMacroTable_ShouldHandleIncludeWithNullGenerator()
  {
    var includes = new IncludesCollection();
    includes.AddInclude( "empty", ( MacroValueGenerator? ) null );

    var macroTable = DefineMacros();
    var template = TemplateCompiler.Compile( "$empty$", macroTable, includes );

    template.Text.Should().BeEmpty();
    template.Should().HaveSingleSegment().Which.BeConstant( string.Empty );
  }

  [Fact]
  public void Compile_WithMacroTable_ShouldHandleIncludeWithNullStringContent()
  {
    var includes = new IncludesCollection();
    includes.AddInclude( "empty", ( string? ) null );

    var macroTable = DefineMacros();
    var template = TemplateCompiler.Compile( "$empty$", macroTable, includes );

    template.Text.Should().BeEmpty();
    template.Should().HaveSingleSegment().Which.BeConstant( string.Empty );
  }

  [Fact]
  public void Compile_WithMacroTable_ShouldHandleMacroWithArgument()
  {
    var macroTable = DefineMacros( "macro" );
    var template = TemplateCompiler.Compile( "$macro:argument$", macroTable );

    template.Should().HaveSingleSegment().Which.BeMacro( "macro", "argument" );
  }

  [Fact]
  public void Compile_WithMacroTable_ShouldHandleNullIncludesParameter()
  {
    const string Text = "$macro$";
    var macroTable = DefineMacros( "macro" );
    var template = TemplateCompiler.Compile( Text, macroTable, null );

    template.Should().HaveText( Text );
    template.Should().HaveSingleSegment().Which.BeMacro( "macro" );
  }

  [Fact]
  public void Compile_WithMacroTable_ShouldLeaveUnclosedIncludePlaceholderUnchanged()
  {
    var includes = new IncludesCollection();
    includes.AddInclude( "x", "X" );

    const string Text = "prefix $x";
    var macroTable = DefineMacros();
    var template = TemplateCompiler.Compile( Text, macroTable, includes );

    template.Should().HaveText( Text );
    template.Should().HaveSingleSegment().Which.BeConstant( Text );
  }

  [Fact]
  public void
    Compile_WithMacroTable_ShouldLeaveUnknownIncludeAsMacro_WhenIncludesProvidedButNameMissing()
  {
    var includes = new IncludesCollection();
    includes.AddInclude( "present", "X" );

    var macroTable = DefineMacros( "missing" );
    var template = TemplateCompiler.Compile( "$missing$", macroTable, includes );

    template.Should().HaveText( "$missing$" );
    template.Should().HaveSingleSegment().Which.BeMacro( "missing" );
  }

  [Fact]
  public void
    Compile_WithMacroTable_ShouldNotRecursivelyExpandIncludes_WhenIncludeContentReferencesAnotherInclude()
  {
    var includes = new IncludesCollection();
    includes.AddInclude( "a", "$b$" );
    includes.AddInclude( "b", "B" );

    var macroTable = DefineMacros( "b" );
    var template = TemplateCompiler.Compile( "$a$", macroTable, includes );

    template.Should().HaveText( "$b$" );
    template.Should().HaveSingleSegment().Which.BeMacro( "b" );
  }

  [Fact]
  public void Compile_WithMacroTable_ShouldNotReplaceAnything_WhenIncludesProvidedButNotReferenced()
  {
    var includes = new IncludesCollection();
    includes.AddInclude( "unused", "UnusedContent" );

    const string Text = "No macros here";
    var macroTable = DefineMacros();
    var template = TemplateCompiler.Compile( Text, macroTable, includes );

    template.Should().HaveText( Text );
    template.Should().HaveSingleSegment().Which.BeConstant( Text );
  }

  [Fact]
  public void
    Compile_WithMacroTable_ShouldNotTreatPlaceholderWithArgumentAsInclude_WhenNameMatchesInclude()
  {
    var includes = new IncludesCollection();
    includes.AddInclude( "x", "INC" );

    var macroTable = DefineMacros( "x" );
    var template = TemplateCompiler.Compile( "$x:arg$", macroTable, includes );

    template.Should().HaveText( "$x:arg$" );
    template.Should().HaveSingleSegment().Which.BeMacro( "x", "arg" );
  }

  [Fact]
  public void Compile_WithMacroTable_ShouldProcessEscapedDelimitersInIncludeContent()
  {
    var includes = new IncludesCollection();
    includes.AddInclude( "section", "Start $$ End" );

    var macroTable = DefineMacros();
    var template = TemplateCompiler.Compile( "$section$", macroTable, includes );

    template.Should().HaveText( "Start $$ End" );
    template.Should().HaveSegmentCount( 2 );

    template.Should().HaveSegmentAt( 0 ).Which.BeConstant( "Start $" );
    template.Should().HaveSegmentAt( 1 ).Which.BeConstant( " End" );
  }

  [Fact]
  public void Compile_WithMacroTable_ShouldProcessIncludeWithMacroArgument()
  {
    var includes = new IncludesCollection();
    includes.AddInclude( "section", "Start $macro:arg$ End" );

    var macroTable = DefineMacros( "macro" );
    var template = TemplateCompiler.Compile( "$section$", macroTable, includes );

    template.Should().HaveText( "Start $macro:arg$ End" );
    template.Should().HaveSegmentCount( 3 );

    template.Should().HaveSegmentAt( 0 ).Which.BeConstant( "Start " );
    template.Should().HaveSegmentAt( 1 ).Which.BeMacro( "macro", "arg" );
    template.Should().HaveSegmentAt( 2 ).Which.BeConstant( " End" );
  }

  [Fact]
  public void Compile_WithMacroTable_ShouldProcessMacrosInIncludeContent()
  {
    var includes = new IncludesCollection();
    includes.AddInclude( "section", "Start $macro$ End" );

    var macroTable = DefineMacros( "macro" );
    var template = TemplateCompiler.Compile( "$section$", macroTable, includes );

    template.Should().HaveText( "Start $macro$ End" );
    template.Should().HaveSegmentCount( 3 );
    template.Should().HaveSegmentAt( 0 ).Which.BeConstant( "Start " );
    template.Should().HaveSegmentAt( 1 ).Which.BeMacro( "macro" );
    template.Should().HaveSegmentAt( 2 ).Which.BeConstant( " End" );
  }

  [Fact]
  public void Compile_WithMacroTable_ShouldProcessMacrosInMultipleIncludes()
  {
    var includes = new IncludesCollection();
    includes.AddInclude( "a", "A$macroA$" );
    includes.AddInclude( "b", "B$macroB$" );

    var macroTable = DefineMacros( "macroA", "macroB" );
    var template = TemplateCompiler.Compile( "$a$-$b$", macroTable, includes );

    template.Should().HaveText( "A$macroA$-B$macroB$" );
    template.Should().HaveSegmentCount( 4 );

    template.Should().HaveSegmentAt( 0 ).Which.BeConstant( "A" );
    template.Should().HaveSegmentAt( 1 ).Which.BeMacro( "macroA" );
    template.Should().HaveSegmentAt( 2 ).Which.BeConstant( "-B" );
    template.Should().HaveSegmentAt( 3 ).Which.BeMacro( "macroB" );
  }

  [Fact]
  public void Compile_WithMacroTable_ShouldReplaceInclude_WhenIncludeIsReferencedInTemplate()
  {
    var includes = new IncludesCollection();
    includes.AddInclude( "header", "HeaderContent" );

    var macroTable = DefineMacros();
    var template = TemplateCompiler.Compile( "$header$ body", macroTable, includes );

    template.Should().HaveText( "HeaderContent body" );

    template.Should().HaveSingleSegment().Which.BeConstant( "HeaderContent body" );
  }

  [Fact]
  public void Compile_WithMacroTable_ShouldReplaceInclude_WithCustomDelimiter()
  {
    var includes = new IncludesCollection();
    includes.AddInclude( "sect", "X" );

    var macroTable = DefineMacros();
    var options = new TemplateCompilerOptions( '#', ':' );
    var template = TemplateCompiler.Compile( "#sect#", macroTable, includes, options );

    template.Should().HaveText( "X" );
    template.Should().HaveSingleSegment().Which.BeConstant( "X" );
  }

  [Fact]
  public void Compile_WithMacroTable_ShouldReplaceMultipleIncludes_WhenMultipleAreReferenced()
  {
    var includes = new IncludesCollection();
    includes.AddInclude( "a", "A" );
    includes.AddInclude( "b", "B" );

    var macroTable = DefineMacros();
    var template = TemplateCompiler.Compile( "$a$-$b$", macroTable, includes );

    template.Should().HaveText( "A-B" );
    template.Should().HaveSingleSegment().Which.BeConstant( "A-B" );
  }

  [Fact]
  public void Compile_WithMacroTable_ShouldReplaceWithEmpty_WhenIncludeIsReferencedButDoesNotExist()
  {
    const string Text = "$missing$";
    var macroTable = DefineMacros( "missing" );
    var template = TemplateCompiler.Compile( Text, macroTable, new IncludesCollection() );

    template.Should().HaveText( Text );
    template.Should().HaveSingleSegment().Which.BeMacro( "missing" );
  }

  [Fact]
  public void Compile_WithMacroTable_ShouldReturnConstantSegment_WhenTextIsOnlyDelimiter()
  {
    const string Text = "$";
    var macroTable = DefineMacros();
    var template = TemplateCompiler.Compile( Text, macroTable );

    template.Should().HaveSingleSegment().Which.BeConstant( "$" );
  }

  [Fact]
  public void Compile_WithMacroTable_ShouldReturnConstantSegment_WhenTextIsUnclosedMacro()
  {
    const string Text = "$macro";
    var macroTable = DefineMacros();
    var template = TemplateCompiler.Compile( Text, macroTable );

    template.Should().HaveSingleSegment().Which.BeConstant( "$macro" );
  }

  [Fact]
  public void
    Compile_WithMacroTable_ShouldReturnDelimiterSegments_WhenTextIsMultipleEscapedDelimitersOnly()
  {
    const string Text = "$$$$";
    var macroTable = DefineMacros();
    var template = TemplateCompiler.Compile( Text, macroTable );

    template.Should().HaveSingleSegment().Which.BeConstant( "$$" );
  }

  [Fact]
  public void
    Compile_WithMacroTable_ShouldReturnMacroSegment_WhenIncludeCollectionIsEmptyAndIncludeReferenced()
  {
    var includes = new IncludesCollection();
    var macroTable = DefineMacros( "missing" );
    var template = TemplateCompiler.Compile( "$missing$", macroTable, includes );

    template.Should().HaveText( "$missing$" );
    template.Should().HaveSingleSegment().Which.BeMacro( "missing" );
  }

  [Fact]
  public void Compile_WithMacroTable_ShouldReturnOneSegment_WhenConstantFollowedByEscapedDelimiter()
  {
    var macroTable = DefineMacros();
    var template = TemplateCompiler.Compile( "text$$", macroTable );

    template.Should().HaveSingleSegment().Which.BeConstant( "text$" );
  }

  [Fact]
  public void Compile_WithMacroTable_ShouldReturnOneSegment_WhenConstantFollowedByOpenMacro()
  {
    var macroTable = DefineMacros();
    var template = TemplateCompiler.Compile( "text$", macroTable );

    template.Should().HaveSingleSegment().Which.BeConstant( "text$" );
  }

  [Fact]
  public void Compile_WithMacroTable_ShouldReturnOneSegment_WhenEscapedDelimiterFollowedByConstant()
  {
    var macroTable = DefineMacros();
    var template = TemplateCompiler.Compile( "$$text", macroTable );

    template.Should().HaveSingleSegment().Which.BeConstant( "$text" );
  }

  [Fact]
  public void Compile_WithMacroTable_ShouldReturnOneSegment_WhenOnlyConstant()
  {
    var macroTable = DefineMacros();
    var template = TemplateCompiler.Compile( "text", macroTable );

    template.Should().HaveSingleSegment().Which.BeConstant( "text" );
  }

  [Fact]
  public void Compile_WithMacroTable_ShouldReturnOneSegment_WhenOpenMacroFollowedByConstant()
  {
    var macroTable = DefineMacros();
    var template = TemplateCompiler.Compile( "$text", macroTable );

    template.Should().HaveSingleSegment().Which.BeConstant( "$text" );
  }

  [Fact]
  public void Compile_WithMacroTable_ShouldReturnSingleConstantSegment_WhenTemplateHasNoMacros()
  {
    const string Text = "I have no macros";

    var macroTable = DefineMacros();
    var template = TemplateCompiler.Compile( Text, macroTable );

    template.Should().HaveSingleSegment().Which.BeConstant( Text );
  }

  [Fact]
  public void Compile_WithMacroTable_ShouldReturnSingleMacroSegment_WhenTemplateIsOnlyTheMacro()
  {
    const string Text = "$macro$";

    var macroTable = DefineMacros( "macro" );
    var template = TemplateCompiler.Compile( Text, macroTable );

    template.Should().HaveSingleSegment().Which.BeMacro( "macro" );
  }

  [Fact]
  public void
    Compile_WithMacroTable_ShouldReturnThreeMacroValues_WhenTemplateContainsThreeDifferentMacros()
  {
    const string Text = "$macroA$$macroB$$macroC$";

    var macroTable = DefineMacros( "macroA", "macroB", "macroC" );
    var template = TemplateCompiler.Compile( Text, macroTable );

    template.Should().HaveSegmentCount( 3 );

    template.Should().HaveSegmentAt( 0 ).Which.BeMacro( "macroA" ).And.HaveSlot( 1 );
    template.Should().HaveSegmentAt( 1 ).Which.BeMacro( "macroB" ).And.HaveSlot( 2 );
    template.Should().HaveSegmentAt( 2 ).Which.BeMacro( "macroC" ).And.HaveSlot( 3 );
  }

  [Fact]
  public void
    Compile_WithMacroTable_ShouldReturnThreeSegments_WhenConstantFollowedByMacroAndConstant()
  {
    var macroTable = DefineMacros( "macro" );
    var template = TemplateCompiler.Compile( "prefix$macro$suffix", macroTable );

    template.Should().HaveSegmentCount( 3 );

    template.Should().HaveSegmentAt( 0 ).Which.BeConstant( "prefix" );
    template.Should().HaveSegmentAt( 1 ).Which.BeMacro( "macro" ).And.HaveSlot( 1 );
    template.Should().HaveSegmentAt( 2 ).Which.BeConstant( "suffix" );
  }

  [Fact]
  public void
    Compile_WithMacroTable_ShouldReturnThreeSegments_WhenEscapedDelimiterIsBetweenMacroSegments()
  {
    const string Text = "$macro$$$$macro$";

    var macroTable = DefineMacros( "macro" );
    var template = TemplateCompiler.Compile( Text, macroTable );

    template.Should().HaveSegmentCount( 3 );

    template.Should().HaveSegmentAt( 0 ).Which.BeMacro( "macro" );
    template.Should().HaveSegmentAt( 1 ).Which.BeConstant( "$" );
    template.Should().HaveSegmentAt( 2 ).Which.BeMacro( "macro" );
  }

  [Fact]
  public void Compile_WithMacroTable_ShouldReturnThreeSegments_WhenTemplateContainsMacroInMiddle()
  {
    const string Text = "This is a $macro$ template.";

    var macroTable = DefineMacros( "macro" );
    var template = TemplateCompiler.Compile( Text, macroTable );

    template.Should().HaveSegmentCount( 3 );

    template.Should().HaveSegmentAt( 0 ).Which.BeConstant( "This is a " );
    template.Should().HaveSegmentAt( 1 ).Which.BeMacro( "macro" );
    template.Should().HaveSegmentAt( 2 ).Which.BeConstant( " template." );
  }

  [Fact]
  public void
    Compile_WithMacroTable_ShouldReturnTwoMacroValues_WhenTemplateContainsTwoDifferentMacrosAndOneRepeated()
  {
    const string Text = "$macroA$$macroB$$macroA$";

    var template = TemplateCompiler.Compile( Text );

    template.Should().HaveSegmentCount( 3 );

    template.Should().HaveSegmentAt( 0 ).Which.BeMacro( "macroA" ).And.HaveSlot( 1 );
    template.Should().HaveSegmentAt( 1 ).Which.BeMacro( "macroB" ).And.HaveSlot( 2 );
    template.Should().HaveSegmentAt( 2 ).Which.BeMacro( "macroA" ).And.HaveSlot( 1 );
  }

  [Fact]
  public void Compile_WithMacroTable_ShouldReturnTwoSegments_WhenConstantFollowedByMacro()
  {
    var macroTable = DefineMacros( "macro" );
    var template = TemplateCompiler.Compile( "text$macro$", macroTable );

    template.Should().HaveSegmentCount( 2 );

    template.Should().HaveSegmentAt( 0 ).Which.BeConstant( "text" );
    template.Should().HaveSegmentAt( 1 ).Which.BeMacro( "macro" ).And.HaveSlot( 1 );
  }

  [Fact]
  public void Compile_WithMacroTable_ShouldSupportCustomDelimiterAndArgumentSeparator()
  {
    var options = new TemplateCompilerOptions( '#', ':' );
    var macroTable = new MacroTableBuilder().Declare( "macro" ).Build();
    var template = TemplateCompiler.Compile( "#macro:arg#", macroTable, options: options );

    template.Should().HaveSingleSegment().Which.BeMacro( "macro", "arg" );
  }

  [Fact]
  public void Compile_WithMacroTable_ShouldThrow_WhenMacroNotDeclared()
  {
    var macroTable = DefineMacros();
    var act = () => TemplateCompiler.Compile( "$unknown$", macroTable );

    act.Should().Throw<InvalidOperationException>().WithMessage( "Undefined macro: 'unknown'" );
  }

  [Fact]
  public void
    Compile_WithMacroTable_ShouldThrowArgumentException_WhenMacroNameIsEmptyButHasArgument()
  {
    var macroTable = DefineMacros( "x" );

    Action act = () => TemplateCompiler.Compile( "$:arg$", macroTable );
    act.Should().Throw<InvalidOperationException>().WithMessage( "The macro name cannot be empty" );
  }

  [Theory]
  [InlineData( null )]
  [InlineData( "" )]
  [InlineData( "   " )]
  public void Compile_WithMacroTable_ShouldThrowArgumentException_WhenTextIsNullOrWhitespace(
    string? text )
  {
    var macroTable = DefineMacros();
    Action act = () => TemplateCompiler.Compile( text!, macroTable );

    act.Should()
       .Throw<ArgumentException>()
       .WithParameterName( "text" )
       .WithMessage( "*cannot be null, empty, or whitespace*" );
  }

  [Fact]
  public void Compile_WithMacroTable_ShouldThrowArgumentNullException_WhenMacroTableIsNull()
  {
    Action act = () => TemplateCompiler.Compile( "text", macroTable: null! );

    act.Should()
       .Throw<ArgumentNullException>()
       .WithParameterName( "macroTable" );
  }

  [Fact]
  public void CompileShouldHandleIncludeWithEmptyStringContent()
  {
    var includes = new IncludesCollection();
    includes.AddInclude( "empty", string.Empty );

    var template = TemplateCompiler.Compile( "$empty$", includes );

    template.Text.Should().BeEmpty();

    template.Should().HaveSingleSegment().Which.BeConstant( string.Empty );
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
