// Module Name: ZipCodeTests.cs
// Author:      Eduardo Velasquez
// Copyright (c) 2025, Intercode Consulting, Inc.

namespace Intercode.Toolbox.TypedPrimitives.Standard.Us.Tests;

using FluentAssertions;

public class ZipCodeTests
{
  #region Public Methods

  public static IEnumerable<string> ValidZipCodes()
  {
    yield return "12345";
    yield return "54321";
    yield return "90210";
    yield return "00000";
    yield return "99999";
    yield return "12345-6789";
    yield return "00000-0000";
    yield return "99999-9999";
  }

  public static IEnumerable<string> InvalidZipCodes()
  {
    yield return "";
    yield return null!;
    yield return "1234"; // Too short
    yield return "123456"; // Too long
    yield return "abcde"; // Non-numeric
    yield return "12-34"; // Contains hyphen
    yield return "123 45"; // Contains space
    yield return "12.45"; // Contains period
    yield return "";
    yield return null!;
    yield return "1234"; // Too short
    yield return "123456"; // Too long
    yield return "abcde"; // Non-numeric
    yield return "12345-"; // Hyphen without extension
    yield return "12345-123"; // Extension too short
    yield return "12345-12345"; // Extension too long
    yield return "12345 6789"; // Space instead of hyphen
    yield return "123456789"; // No hyphen separator
  }

  public static IEnumerable<object[]> AllZipCodes()
  {
    foreach( var value in ValidZipCodes() )
    {
      yield return [value, true];
    }

    foreach( var value in InvalidZipCodes() )
    {
      yield return [value, false];
    }
  }

  [Theory]
  [MemberData( nameof( AllZipCodes ) )]
  public void Create_ShouldHandleValidAndInvalidZipCodes(
    string value,
    bool expected )
  {
    var result = ZipCode.Create( value );
    result.IsSuccess.Should().Be( expected );
  }

  [Theory]
  [MemberData( nameof( AllZipCodes ) )]
  public void Validate_ShouldHandleValidAndInvalidZipCodes(
    string value,
    bool expected )
  {
    var result = ZipCode.Validate( value );
    result.IsSuccess.Should().Be( expected );
  }

  #endregion
}
