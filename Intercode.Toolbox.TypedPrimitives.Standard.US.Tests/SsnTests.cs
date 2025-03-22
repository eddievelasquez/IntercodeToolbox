// Module Name: SsnTests.cs
// Author:      Eduardo Velasquez
// Copyright (c) 2025, Intercode Consulting, Inc.

namespace Intercode.Toolbox.TypedPrimitives.Standard.Us.Tests;

using FluentAssertions;

public class SsnTests
{
  public static IEnumerable<string> ValidSsns()
  {
    yield return "123-45-6789"; // Lowest valid area
    yield return "234-56-7890";
    yield return "345-67-8901";
    yield return "456-78-9012";
    yield return "567-89-0123";
    yield return "665-45-6789"; // Just before invalid 666
    yield return "899-45-6789"; // Just before invalid range
  }

  public static IEnumerable<string> InvalidSsns()
  {
    yield return null!; // Invalid: null
    yield return "000-12-3456"; // Invalid: first group all zeros
    yield return "123-00-4567"; // Invalid: second group all zeros
    yield return "123-45-0000"; // Invalid: third group all zeros
    yield return "666-45-6789"; // Invalid: area 666
    yield return "900-45-6789"; // Invalid: area in 900-999 range
    yield return "999-45-6789"; // Invalid: area in 900-999 range
    yield return "123-456-789"; // Invalid: wrong format
    yield return "12-34-5678"; // Invalid: too few digits
    yield return "1234-56-789"; // Invalid: too many digits
    yield return "abc-de-fghi"; // Invalid: non-numeric
    yield return ""; // Invalid: empty string
    yield return "123 45 6789"; // Invalid: spaces instead of hyphens
  }

  public static IEnumerable<object[]> AllSsns()
  {
    foreach( var value in ValidSsns() )
    {
      yield return [value, true];
    }

    foreach( var value in InvalidSsns() )
    {
      yield return [value, false];
    }
  }

  [Theory]
  [MemberData( nameof( AllSsns ) )]
  public void Create_ShouldHandleValidAndInvalidSsns(
    string value,
    bool expected )
  {
    var result = Ssn.Create( value );
    result.IsSuccess.Should().Be( expected );
  }

  [Theory]
  [MemberData( nameof( AllSsns ) )]
  public void Validate_ShouldHandleValidAndInvalidSsns(
    string value,
    bool expected )
  {
    var result = Ssn.Validate( value );
    result.IsSuccess.Should().Be( expected );
  }
}
