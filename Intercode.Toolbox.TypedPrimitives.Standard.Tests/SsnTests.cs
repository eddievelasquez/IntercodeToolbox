// Module Name: SsnTests.cs
// Author:      Eduardo Velasquez
// Copyright (c) 2025, Intercode Consulting, Inc.

namespace Intercode.Toolbox.TypedPrimitives.Standard.Tests;

using FluentAssertions;
using Intercode.Toolbox.TypedPrimitives.Standard.Identifiers.US;

public class SsnTests
{
  public static IEnumerable<object[]> ValidSsns()
  {
    yield return ["123-45-6789", true]; // Lowest valid area
    yield return ["234-56-7890", true];
    yield return ["345-67-8901", true];
    yield return ["456-78-9012", true];
    yield return ["567-89-0123", true];
    yield return ["665-45-6789", true]; // Just before invalid 666
    yield return ["899-45-6789", true]; // Just before invalid range
  }

  public static IEnumerable<object[]> InvalidSsns()
  {
    yield return [null!, false]; // Invalid: null
    yield return ["000-12-3456", false]; // Invalid: first group all zeros
    yield return ["123-00-4567", false]; // Invalid: second group all zeros
    yield return ["123-45-0000", false]; // Invalid: third group all zeros
    yield return ["666-45-6789", false]; // Invalid: area 666
    yield return ["900-45-6789", false]; // Invalid: area in 900-999 range
    yield return ["999-45-6789", false]; // Invalid: area in 900-999 range
    yield return ["123-456-789", false]; // Invalid: wrong format
    yield return ["12-34-5678", false]; // Invalid: too few digits
    yield return ["1234-56-789", false]; // Invalid: too many digits
    yield return ["abc-de-fghi", false]; // Invalid: non-numeric
    yield return ["", false]; // Invalid: empty string
    yield return ["123 45 6789", false]; // Invalid: spaces instead of hyphens
  }

  public static IEnumerable<object[]> AllSsns()
  {
    foreach( var ssn in ValidSsns() )
    {
      yield return ssn;
    }

    foreach( var ssn in InvalidSsns() )
    {
      yield return ssn;
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
