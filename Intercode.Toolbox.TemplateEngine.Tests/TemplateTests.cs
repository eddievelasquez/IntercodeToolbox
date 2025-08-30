// Module Name: TemplateTests.cs
// Author:      Eduardo Velasquez
// Copyright (c) 2025, Intercode Consulting, Inc.

namespace Intercode.Toolbox.TemplateEngine.Tests;

using FluentAssertions;

public class TemplateTests
{
  #region Tests

  [Fact]
  public void Constructor_ShouldThrow_WhenSegmentsIsEmpty()
  {
    Action act = () => new Template( new MacroProcessorContext(), [] );

    act.Should().Throw<ArgumentException>().WithParameterName( "segments" );
  }

  [Fact]
  public void Constructor_ShouldThrow_WhenSegmentsIsNull()
  {
    Action act = () => new Template( new MacroProcessorContext(), null! );

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

    var context = new MacroProcessorContext();
    var template = new Template( context, segments );
    template.Deconstruct( out var actualContext, out var actualSegments );
    actualContext.Should().Be( context );
    actualSegments.Should().BeSameAs( segments );
  }

  #endregion
}
