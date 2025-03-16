// Module Name: PostalCodeTests.cs
// Author:      Eduardo Velasquez
// Copyright (c) 2025, Intercode Consulting, Inc.

namespace Intercode.Toolbox.TypedPrimitives.Standard.Tests;

using FluentAssertions;
using Intercode.Toolbox.TypedPrimitives.Standard.PostalCodes.CA;

public class PostalCodeTests
{
  public static IEnumerable<object[]> ValidPostalCodes()
  {
    // Valid postal codes (with or without space)
    yield return ["A1A 1A1", true];
    yield return ["A1A1A1", true];
    yield return ["K1A 0B1", true]; // Ottawa/Parliament Hill
    yield return ["K1A0B1", true];
    yield return ["M5V 2H1", true]; // Toronto
    yield return ["M5V2H1", true];
    yield return ["H3Z 2Y7", true]; // Montreal
    yield return ["H3Z2Y7", true];
    yield return ["V6B 3E6", true]; // Vancouver
    yield return ["V6B3E6", true];
    yield return ["T2P 1J9", true]; // Calgary
    yield return ["T2P1J9", true];
    yield return ["B3J 2S7", true]; // Halifax
    yield return ["B3J2S7", true];

    // Only postal codes with a space are valid for this test
    yield return ["A1A 1A1", true];
    yield return ["K1A 0B1", true]; // Ottawa/Parliament Hill
    yield return ["M5V 2H1", true]; // Toronto
    yield return ["H3Z 2Y7", true]; // Montreal
    yield return ["V6B 3E6", true]; // Vancouver
    yield return ["T2P 1J9", true]; // Calgary
    yield return ["B3J 2S7", true]; // Halifax
  }

  public static IEnumerable<object[]> InvalidPostalCodes()
  {
    // Null or empty
    yield return [null!, false];
    yield return ["", false];

    // Invalid formats (wrong length, characters, etc.)
    yield return ["A1A", false]; // Too short
    yield return ["A1A 1A1 1A1", false]; // Too long
    yield return ["A1A-1A1", false]; // Incorrect separator
    yield return ["A1A.1A1", false]; // Incorrect separator
    yield return ["AAAAAA", false]; // All letters
    yield return ["111111", false]; // All numbers
    yield return ["1A1 A1A", false]; // Reversed format

    // Invalid first character
    yield return ["D1A 1A1", false]; // First char can't be D
    yield return ["F1A 1A1", false]; // First char can't be F
    yield return ["I1A 1A1", false]; // First char can't be I
    yield return ["O1A 1A1", false]; // First char can't be O
    yield return ["Q1A 1A1", false]; // First char can't be Q
    yield return ["U1A 1A1", false]; // First char can't be U
    yield return ["W1A 1A1", false]; // First char can't be W

    // Invalid third character
    yield return ["A1D 1A1", false]; // Third char can't be D
    yield return ["A1F 1A1", false]; // Third char can't be F
    yield return ["A1I 1A1", false]; // Third char can't be I
    yield return ["A1O 1A1", false]; // Third char can't be O
    yield return ["A1Q 1A1", false]; // Third char can't be Q
    yield return ["A1U 1A1", false]; // Third char can't be U
  }

  public static IEnumerable<object[]> AllPostalCodes()
  {
    foreach( var zipcode in ValidPostalCodes() )
    {
      yield return zipcode;
    }

    foreach( var zipcode in InvalidPostalCodes() )
    {
      yield return zipcode;
    }
  }

  [Theory]
  [MemberData( nameof( AllPostalCodes ) )]
  public void Create_ShouldHandleValidAndInvalidZipcodes(
    string value,
    bool expected )
  {
    var result = PostalCode.Create( value );
    result.IsSuccess.Should().Be( expected );
  }

  [Theory]
  [MemberData( nameof( AllPostalCodes ) )]
  public void Validate_ShouldHandleValidAndInvalidZipcodes(
    string value,
    bool expected )
  {
    var result = PostalCode.Validate( value );
    result.IsSuccess.Should().Be( expected );
  }
}
