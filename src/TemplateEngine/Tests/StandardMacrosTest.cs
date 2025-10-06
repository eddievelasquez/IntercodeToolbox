// Module Name: StandardMacrosTest.cs
// Author:      Eduardo Velasquez
// Copyright (c) 2025, Intercode Consulting, Inc.

namespace Intercode.Toolbox.TemplateEngine.Tests;

using System.Globalization;

public class StandardMacrosTest
{
  #region Tests

  [Theory]
  [InlineData( "NOW", -1 )]
  [InlineData( "UTC_NOW", -2 )]
  [InlineData( "GUID", -3 )]
  [InlineData( "MACHINE", -4 )]
  [InlineData( "OS", -5 )]
  [InlineData( "USER", -6 )]
  [InlineData( "CLR_VERSION", -7 )]
  [InlineData( "ENV", -8 )]
  public void GetSlot_WithSpan_ShouldReturnExpectedNegativeSlot_WhenMacroKnown(
    string name,
    int expectedSlot )
  {
    var slot = StandardMacros.GetSlot( name.AsSpan() );
    slot.Should().Be( expectedSlot );
  }

  [Fact]
  public void GetSlot_WithSpan_ShouldReturnZero_WhenEmptySpan()
  {
    var slot = StandardMacros.GetSlot( ReadOnlySpan<char>.Empty );
    slot.Should().Be( MacroTable.MacroNotFoundSlot );
  }

  [Fact]
  public void GetSlot_WithSpan_ShouldReturnZero_WhenUnknownMacro()
  {
    var slot = StandardMacros.GetSlot( "__unknown__".AsSpan() );
    slot.Should().Be( MacroTable.MacroNotFoundSlot );
  }

  [Theory]
  [InlineData( "now", -1 )]
  [InlineData( "Utc_Now", -2 )]
  [InlineData( "gUiD", -3 )]
  [InlineData( "machine", -4 )]
  [InlineData( "os", -5 )]
  [InlineData( "user", -6 )]
  [InlineData( "clr_version", -7 )]
  [InlineData( "env", -8 )]
  public void GetSlot_WithString_ShouldBeCaseInsensitive(
    string name,
    int expectedSlot )
  {
    var slot = StandardMacros.GetSlot( name );
    slot.Should().Be( expectedSlot );
  }

  [Theory]
  [InlineData( StandardMacros.NowMacroName, -1 )]
  [InlineData( StandardMacros.UtcNowMacroName, -2 )]
  [InlineData( StandardMacros.GuidMacroName, -3 )]
  [InlineData( StandardMacros.MachineMacroName, -4 )]
  [InlineData( StandardMacros.OsMacroName, -5 )]
  [InlineData( StandardMacros.UserMacroName, -6 )]
  [InlineData( StandardMacros.ClrVersionMacroName, -7 )]
  [InlineData( StandardMacros.EnvMacroName, -8 )]
  public void GetSlot_WithString_ShouldReturnExpectedNegativeSlot_WhenMacroKnown(
    string name,
    int expectedSlot )
  {
    var slot = StandardMacros.GetSlot( name );
    slot.Should().Be( expectedSlot );
  }

  [Fact]
  public void GetSlot_WithString_ShouldReturnZero_WhenUnknownMacro()
  {
    var slot = StandardMacros.GetSlot( "UNKNOWN_MACRO" );
    slot.Should().Be( MacroTable.MacroNotFoundSlot );
  }

  [Theory]
  [InlineData( null )]
  [InlineData( "" )]
  public void GetSlot_WithString_ShouldThrow_WhenNameNullOrEmpty(
    string? name )
  {
    var act = () => StandardMacros.GetSlot( name! );

    act.Should()
       .Throw<ArgumentException>()
       .WithParameterName( "macroName" );
  }

  [Fact]
  public void GetStandardMacroNames_ShouldReturnAllExpectedNames()
  {
    var names = StandardMacros.GetStandardMacroNames().ToArray();

    names.Should().NotBeNull();
    names.Should().HaveCount( 8 );

    names.Should()
         .BeEquivalentTo(
           new[]
           {
             StandardMacros.NowMacroName, StandardMacros.UtcNowMacroName,
             StandardMacros.GuidMacroName,
             StandardMacros.MachineMacroName, StandardMacros.OsMacroName,
             StandardMacros.UserMacroName,
             StandardMacros.ClrVersionMacroName, StandardMacros.EnvMacroName
           },
           options => options.WithoutStrictOrdering()
         );
  }

  [Fact]
  public void GetValue_WithClrVersion_ShouldMatchEnvironmentVersion()
  {
    var text = StandardMacros.GetValue( StandardMacros.ClrVersionMacroName );
    text.Should().Be( Environment.Version.ToString() );
  }

  [Fact]
  public void GetValue_WithEnv_WhenArgumentEmpty_ShouldReturnEmptyString()
  {
    var value = StandardMacros.GetValue( StandardMacros.EnvMacroName, ReadOnlySpan<char>.Empty );
    value.Should().BeEmpty();
  }

  [Fact]
  public void GetValue_WithEnv_WhenVariableMissing_ShouldReturnEmptyString()
  {
    var uniqueKey = "_ITB_TEST_ENV_MISSING_" + Guid.NewGuid().ToString( "N" );

    try
    {
      Environment.SetEnvironmentVariable( uniqueKey, null );
      var value = StandardMacros.GetValue( StandardMacros.EnvMacroName, uniqueKey.AsSpan() );
      value.Should().BeEmpty();
    }
    finally
    {
      Environment.SetEnvironmentVariable( uniqueKey, null );
    }
  }

  [Fact]
  public void GetValue_WithEnv_WhenVariablePresent_ShouldReturnValue()
  {
    var key = "_ITB_TEST_ENV_PRESENT_" + Guid.NewGuid().ToString( "N" );
    const string expected = "expected-value";

    try
    {
      Environment.SetEnvironmentVariable( key, expected );
      var value = StandardMacros.GetValue( StandardMacros.EnvMacroName, key.AsSpan() );
      value.Should().Be( expected );
    }
    finally
    {
      Environment.SetEnvironmentVariable( key, null );
    }
  }

  [Fact]
  public void GetValue_WithGuidMacro_ShouldReturnParsableGuid()
  {
    var text = StandardMacros.GetValue( StandardMacros.GuidMacroName );
    text.Should().NotBeNullOrEmpty();
    Guid.TryParse( text, out _ ).Should().BeTrue();
  }

  [Theory]
  [InlineData( "N", 32 )]
  [InlineData( "D", 36 )]
  [InlineData( "B", 38 )]
  [InlineData( "P", 38 )]
  public void GetValue_WithGuidMacroAndFormat_ShouldEmitExpectedFormat(
    string format,
    int expectedLength )
  {
    var text = StandardMacros.GetValue( StandardMacros.GuidMacroName, format.AsSpan() );
    text.Should().NotBeNull();
    text!.Length.Should().Be( expectedLength );
    Guid.TryParseExact( text, format, out _ ).Should().BeTrue();
  }

  [Fact]
  public void GetValue_WithGuidMacroAndInvalidFormat_ShouldThrow()
  {
    Action act = () => StandardMacros.GetValue( StandardMacros.GuidMacroName, "Z".AsSpan() );
    act.Should().Throw<FormatException>();
  }

  [Fact]
  public void GetValue_WithGuidSlot_ShouldReturnParsableGuid()
  {
    var guidText = StandardMacros.GetValue( -3 );
    guidText.Should().NotBeNullOrEmpty();
    Guid.TryParse( guidText, out _ ).Should().BeTrue();
  }

  [Fact]
  public void GetValue_WithMachine_ShouldMatchEnvironmentMachineName()
  {
    var text = StandardMacros.GetValue( StandardMacros.MachineMacroName );
    text.Should().Be( Environment.MachineName );
  }

  [Fact]
  public void GetValue_WithMacroNotFoundSlot_ShouldReturnNull()
  {
    var value = StandardMacros.GetValue( MacroTable.MacroNotFoundSlot );
    value.Should().BeNull();
  }

  [Fact]
  public void GetValue_WithNow_ShouldHonorCustomFormat()
  {
    var text = StandardMacros.GetValue( StandardMacros.NowMacroName, "O".AsSpan() );
    text.Should().NotBeNullOrEmpty();

    DateTime.TryParseExact(
              text,
              "O",
              CultureInfo.InvariantCulture,
              DateTimeStyles.RoundtripKind,
              out _
            )
            .Should()
            .BeTrue();
  }

  [Fact]
  public void GetValue_WithOs_ShouldMatchEnvironmentOsVersionString()
  {
    var text = StandardMacros.GetValue( StandardMacros.OsMacroName );
    text.Should().Be( Environment.OSVersion.VersionString );
  }

  [Fact]
  public void GetValue_WithOutOfRangeNegativeSlot_ShouldReturnNull()
  {
    var value = StandardMacros.GetValue( -99 );
    value.Should().BeNull();
  }

  [Fact]
  public void GetValue_WithPositiveSlot_ShouldReturnNull()
  {
    var value = StandardMacros.GetValue( 1 );
    value.Should().BeNull();
  }

  [Fact]
  public void GetValue_WithSpanEmptyMacroName_ShouldReturnNull()
  {
    var value = StandardMacros.GetValue( ReadOnlySpan<char>.Empty );
    value.Should().BeNull();
  }

  [Fact]
  public void GetValue_WithSpanGuidAndFormat_ShouldReturnParsableGuid()
  {
    var text = StandardMacros.GetValue( StandardMacros.GuidMacroName.AsSpan(), "N".AsSpan() );
    text.Should().NotBeNull();
    Guid.TryParseExact( text, "N", out _ ).Should().BeTrue();
  }

  [Fact]
  public void GetValue_WithSpanUnknownMacro_ShouldReturnNull()
  {
    var value = StandardMacros.GetValue( "__unknown__".AsSpan() );
    value.Should().BeNull();
  }

  [Theory]
  [InlineData( null )]
  [InlineData( "" )]
  public void GetValue_WithString_ShouldThrow_WhenNameNullOrEmpty(
    string? name )
  {
    var act = () => StandardMacros.GetValue( name! );

    act.Should()
       .Throw<ArgumentException>()
       .WithParameterName( "macroName" );
  }

  [Fact]
  public void GetValue_WithUnknownMacroName_ShouldReturnNull()
  {
    var value = StandardMacros.GetValue( "UNKNOWN" );
    value.Should().BeNull();
  }

  [Fact]
  public void GetValue_WithUser_ShouldMatchEnvironmentUserName()
  {
    var text = StandardMacros.GetValue( StandardMacros.UserMacroName );
    text.Should().Be( Environment.UserName );
  }

  [Fact]
  public void GetValue_WithUtcNow_ShouldHonorCustomFormat_AndRoundtripKindUtc()
  {
    var text = StandardMacros.GetValue( StandardMacros.UtcNowMacroName, "O".AsSpan() );
    text.Should().NotBeNullOrEmpty();

    DateTime.TryParseExact(
              text,
              "O",
              CultureInfo.InvariantCulture,
              DateTimeStyles.RoundtripKind,
              out var parsed
            )
            .Should()
            .BeTrue();

    parsed.Kind.Should().Be( DateTimeKind.Utc );
  }

  #endregion
}
