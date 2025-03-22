// Module Name: PostalCodeTests.cs
// Author:      Eduardo Velasquez
// Copyright (c) 2025, Intercode Consulting, Inc.

namespace Intercode.Toolbox.TypedPrimitives.Standard.Ca.Tests;

using FluentAssertions;

public class PostalCodeTests
{
  public static IEnumerable<string> ValidPostalCodes()
  {
    // Valid postal codes (with or without space)
    yield return "A1A 1A1";
    yield return "A1A1A1";
    yield return "K1A 0B1"; // Ottawa/Parliament Hill
    yield return "K1A0B1";
    yield return "M5V 2H1"; // Toronto
    yield return "M5V2H1";
    yield return "H3Z 2Y7"; // Montreal
    yield return "H3Z2Y7";
    yield return "V6B 3E6"; // Vancouver
    yield return "V6B3E6";
    yield return "T2P 1J9"; // Calgary
    yield return "T2P1J9";
    yield return "B3J 2S7"; // Halifax
    yield return "B3J2S7";

    // Only postal codes with a space are valid for this test
    yield return "A1A 1A1";
    yield return "K1A 0B1"; // Ottawa/Parliament Hill
    yield return "M5V 2H1"; // Toronto
    yield return "H3Z 2Y7"; // Montreal
    yield return "V6B 3E6"; // Vancouver
    yield return "T2P 1J9"; // Calgary
    yield return "B3J 2S7"; // Halifax
  }

  public static IEnumerable<string> InvalidPostalCodes()
  {
    // Null or empty
    yield return null!;
    yield return "";

    // Invalid formats (wrong length, characters, etc.)
    yield return "A1A"; // Too short
    yield return "A1A 1A1 1A1"; // Too long
    yield return "A1A-1A1"; // Incorrect separator
    yield return "A1A.1A1"; // Incorrect separator
    yield return "AAAAAA"; // All letters
    yield return "111111"; // All numbers
    yield return "1A1 A1A"; // Reversed format

    // Invalid first character
    yield return "D1A 1A1"; // First char can't be D
    yield return "F1A 1A1"; // First char can't be F
    yield return "I1A 1A1"; // First char can't be I
    yield return "O1A 1A1"; // First char can't be O
    yield return "Q1A 1A1"; // First char can't be Q
    yield return "U1A 1A1"; // First char can't be U
    yield return "W1A 1A1"; // First char can't be W

    // Invalid third character
    yield return "A1D 1A1"; // Third char can't be D
    yield return "A1F 1A1"; // Third char can't be F
    yield return "A1I 1A1"; // Third char can't be I
    yield return "A1O 1A1"; // Third char can't be O
    yield return "A1Q 1A1"; // Third char can't be Q
    yield return "A1U 1A1"; // Third char can't be U
  }

  public static IEnumerable<object[]> AllPostalCodes()
  {
    foreach( var value in ValidPostalCodes() )
    {
      yield return [value, true];
    }

    foreach( var value in InvalidPostalCodes() )
    {
      yield return [value, false];
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
