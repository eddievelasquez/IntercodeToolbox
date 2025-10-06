// Module Name: TemplateTests.cs
// Author:      Eduardo Velasquez
// Copyright (c) 2025, Intercode Consulting, Inc.

namespace Intercode.Toolbox.TemplateEngine.Tests;

public class TemplateTests
{
  #region Tests

  [Fact]
  public void Constructor_ShouldThrow_WhenSegmentsIsEmpty()
  {
    Action act = () => new Template( "", new MacroTableBuilder().Build(), [] );

    act.Should().Throw<ArgumentException>().WithParameterName( "segments" );
  }

  [Fact]
  public void Constructor_ShouldThrow_WhenSegmentsIsNull()
  {
    Action act = () => new Template( "", new MacroTableBuilder().Build(), null! );

    act.Should().Throw<ArgumentNullException>().WithParameterName( "segments" );
  }

  #endregion
}
