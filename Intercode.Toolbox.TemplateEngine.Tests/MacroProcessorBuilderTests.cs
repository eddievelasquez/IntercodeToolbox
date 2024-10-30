// Module Name: MacroProcessorBuilderTests.cs
// Author:      Eduardo Velasquez
// Copyright (c) 2024, Intercode Consulting, Inc.

namespace Intercode.Toolbox.TemplateEngine.Tests;

using FluentAssertions;
using Microsoft.Extensions.Time.Testing;

public class MacroProcessorBuilderTests
{
  #region Fields

  private readonly FakeTimeProvider _timeProvider = new (
    new DateTimeOffset(
      2024,
      10,
      20,
      10,
      30,
      0,
      TimeSpan.FromHours( -7 )
    )
  );

  #endregion

  #region Tests

  [Fact]
  public void AddMacro_ShouldSucceed_WhenMacroNameContainsDash()
  {
    var builder = new MacroProcessorBuilder();
    var act = () => builder.AddMacro( "macro-a", "value" );

    act.Should().NotThrow();
  }

  [Fact]
  public void AddMacro_ShouldSucceed_WhenMacroNameContainsUnderscore()
  {
    var builder = new MacroProcessorBuilder();
    var act = () => builder.AddMacro( "macro_A", "value" );

    act.Should().NotThrow();
  }

  [Fact]
  public void AddMacro_ShouldSucceed_WhenMacroNameISAlphaNumeric()
  {
    var builder = new MacroProcessorBuilder();
    var act = () => builder.AddMacro( "macro1", "value" );

    act.Should().NotThrow();
  }

  [Fact]
  public void AddMacro_ShouldThrow_WhenMacroNameContainsInvalidChars()
  {
    var builder = new MacroProcessorBuilder();
    var act = () => builder.AddMacro( "macro@", "value" );

    act.Should().Throw<ArgumentException>().WithMessage( "Macro name must be alphanumeric, underscore, or dash*" );
  }

  [Fact]
  public void AddMacro_ShouldThrow_WhenMacroNameIsNull()
  {
    var builder = new MacroProcessorBuilder();
    var act = () => builder.AddMacro( null!, "value" );

    act.Should().Throw<ArgumentException>().WithMessage( "Value cannot be null or empty*" );
  }

  [Fact]
  public void AddMacro_ShouldThrow_WhenMacroNameIsEmpty()
  {
    var builder = new MacroProcessorBuilder();
    var act = () => builder.AddMacro( "", "value" );

    act.Should().Throw<ArgumentException>().WithMessage( "Value cannot be null or empty*" );
  }

  [Fact]
  public void AddStandardMacros_ShouldAddClrVersionMacro()
  {
    var processor = new MacroProcessorBuilder().AddStandardMacros()
                                               .Build();
    processor.Should().NotBeNull();
    processor.GetMacroValue( "CLR_VERSION" ).Should().Be( Environment.Version.ToString() );
  }

  [Fact]
  public void AddStandardMacros_ShouldAddEnvMacro()
  {
    var processor = new MacroProcessorBuilder().AddStandardMacros()
                                               .Build();
    processor.Should().NotBeNull();
    processor.GetMacroValue( "ENV", "TEMP" ).Should().Be( Environment.GetEnvironmentVariable( "TEMP" ) );
  }

  [Fact]
  public void AddStandardMacros_ShouldAddGuidMacro()
  {
    var processor = new MacroProcessorBuilder().AddStandardMacros()
                                               .Build();
    processor.Should().NotBeNull();
    processor.GetMacroValue( "GUID" ).Should().NotBeNullOrEmpty();
  }

  [Fact]
  public void AddStandardMacros_ShouldAddMachineMacro()
  {
    var processor = new MacroProcessorBuilder().AddStandardMacros()
                                               .Build();
    processor.Should().NotBeNull();
    processor.GetMacroValue( "MACHINE" ).Should().Be( Environment.MachineName );
  }

  [Fact]
  public void AddStandardMacros_ShouldAddNowMacro()
  {
    var processor = new MacroProcessorBuilder().AddStandardMacros()
                                               .Build();
    processor.Should().NotBeNull();
    processor.GetMacroValue( "NOW" ).Should().NotBeNullOrEmpty();
  }

  [Fact]
  public void AddStandardMacros_ShouldAddOsMacro()
  {
    var processor = new MacroProcessorBuilder().AddStandardMacros()
                                               .Build();
    processor.Should().NotBeNull();
    processor.GetMacroValue( "OS" ).Should().Be( Environment.OSVersion.VersionString );
  }

  [Fact]
  public void AddStandardMacros_ShouldAddUserMacro()
  {
    var processor = new MacroProcessorBuilder().AddStandardMacros()
                                               .Build();
    processor.Should().NotBeNull();
    processor.GetMacroValue( "USER" ).Should().Be( Environment.UserName );
  }

  [Fact]
  public void AddStandardMacros_ShouldAddUtcNowMacro()
  {
    var processor = new MacroProcessorBuilder().AddStandardMacros()
                                               .Build();
    processor.Should().NotBeNull();
    processor.GetMacroValue( "UTC_NOW" ).Should().NotBeNullOrEmpty();
  }

  [Fact]
  public void Builder_ShouldReturnEmptyProcessor_WhenNoMacrosAdded()
  {
    var processor = new MacroProcessorBuilder().Build();
    processor.Should().NotBeNull();

    processor.MacroCount.Should().Be( 0 );
  }

  [Fact]
  public void Builder_ShouldReturnProcessor_WhenMacrosDynamicMacrosAdded()
  {
    var processor = new MacroProcessorBuilder().AddMacro( "macroA", _ => _timeProvider.GetLocalNow().ToString() )
                                               .AddMacro(
                                                 "macroB",
                                                 _ => _timeProvider.GetLocalNow().AddMinutes( 1 ).ToString()
                                               )
                                               .Build();

    processor.Should().NotBeNull();
    processor.MacroCount.Should().Be( 2 );

    processor.GetMacroValue( "macroA" ).Should().Be( "10/20/2024 10:30:00 AM -07:00" );
    processor.GetMacroValue( "macroB" ).Should().Be( "10/20/2024 10:31:00 AM -07:00" );
  }

  [Fact]
  public void Builder_ShouldReturnProcessor_WhenMacrosStaticMacrosAdded()
  {
    var processor = new MacroProcessorBuilder().AddMacro( "macroA", "valueA" )
                                               .AddMacro( "macroB", "valueB" )
                                               .Build();

    processor.Should().NotBeNull();
    processor.MacroCount.Should().Be( 2 );

    processor.GetMacroValue( "macroA" ).Should().Be( "valueA" );
    processor.GetMacroValue( "macroB" ).Should().Be( "valueB" );
  }

  #endregion
}
