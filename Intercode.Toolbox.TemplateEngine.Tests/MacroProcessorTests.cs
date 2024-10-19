// Module Name: MacroProcessorTests.cs
// Author:      Eduardo Velasquez
// Copyright (c) 2024, Intercode Consulting, Inc.

namespace Intercode.Toolbox.TemplateEngine.Tests;

using FluentAssertions;

public class MacroProcessorTests
{
  #region Tests

  [Fact]
  public void Builder_ShouldReturnEmptyProcessor_WhenNoMacrosAdded()
  {
    var processor = new MacroProcessorBuilder().Build();
    processor.Should().NotBeNull();

    processor.MacroCount.Should().Be( 0 );
  }

  [Fact]
  public void Builder_ShouldReturnProcessor_WhenMacrosAdded()
  {
    var processor = new MacroProcessorBuilder().AddMacro( "macroA", "valueA" )
                                               .AddMacro( "macroB", "valueB" )
                                               .Build();

    processor.Should().NotBeNull();

    processor.MacroCount.Should().Be( 2 );
  }

  [Fact]
  public void GetMacroValue_ShouldBeCaseInsensitive()
  {
    var processor = new MacroProcessorBuilder().AddMacro( "macro", "value" )
                                               .Build();

    processor.GetMacroValue( "MaCrO" ).Should().Be( "value" );
  }

  [Fact]
  public void GetMacroValue_ShouldReturnValue_WhenFound()
  {
    var processor = new MacroProcessorBuilder().AddMacro( "macro", "value" )
                                               .Build();

    processor.GetMacroValue( "macro" ).Should().Be( "value" );
  }

  #endregion
}
