// Module Name: TemplateTests.cs
// Author:      Eduardo Velasquez
// Copyright (c) 2025, Intercode Consulting, Inc.

namespace Intercode.Toolbox.TemplateEngine.Tests;

using FluentAssertions;

public class TemplateTests
{
  #region Tests

  [Fact]
  public void Constructor_ShouldThrow_WhenMacroTableIsNull()
  {
    Action act = () => new Template( [new Segment()], null! );

    act.Should().Throw<ArgumentException>().WithParameterName( "macroTable" );
  }

  [Fact]
  public void Constructor_ShouldThrow_WhenSegmentsIsEmpty()
  {
    Action act = () => new Template( [], new Dictionary<string, int>() );

    act.Should().Throw<ArgumentException>().WithParameterName( "segments" );
  }

  [Fact]
  public void Constructor_ShouldThrow_WhenSegmentsIsNull()
  {
    Action act = () => new Template( null!, new Dictionary<string, int>() );

    act.Should().Throw<ArgumentNullException>().WithParameterName( "segments" );
  }

  [Fact]
  public void Deconstruct_ShouldReturnSegments_FromConstructor()
  {
    Segment[] segments =
    [
      Segment.CreateConstant( ReadOnlyMemory<char>.Empty ), Segment.CreateDelimiter(),
      Segment.CreateMacro( ReadOnlyMemory<char>.Empty, 0 )
    ];

    var template = new Template( segments, new Dictionary<string, int>() );
    template.Deconstruct( out var actualSegments );
    actualSegments.Should().BeSameAs( segments );
  }

  #endregion
}
