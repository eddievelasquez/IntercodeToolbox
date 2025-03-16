// Module Name: UnitTest1.cs
// Author:      $USER_NAME$
// Copyright (c) $CREATED_YEAR$, Intercode Consulting, Inc.
//

namespace Intercode.Toolbox.TypedPrimitives.Standard.Tests;

using FluentAssertions;
using Intercode.Toolbox.TypedPrimitives.Standard.PostalCodes.US;

public class ZipCodeTests
{
  public static IEnumerable<object[]> ValidZipCodes()
  {
    yield return ["12345", true];
    yield return ["54321", true];
    yield return ["90210", true];
    yield return ["00000", true];
    yield return ["99999", true];
    yield return ["12345-6789", true];
    yield return ["00000-0000", true];
    yield return ["99999-9999", true];
  }

  public static IEnumerable<object[]> InvalidZipCodes()
  {
    yield return ["", false];
    yield return [null!, false];
    yield return ["1234", false]; // Too short
    yield return ["123456", false]; // Too long
    yield return ["abcde", false]; // Non-numeric
    yield return ["12-34", false]; // Contains hyphen
    yield return ["123 45", false]; // Contains space
    yield return ["12.45", false]; // Contains period
    yield return ["", false];
    yield return [null!, false];
    yield return ["1234", false]; // Too short
    yield return ["123456", false]; // Too long
    yield return ["abcde", false]; // Non-numeric
    yield return ["12345-", false]; // Hyphen without extension
    yield return ["12345-123", false]; // Extension too short
    yield return ["12345-12345", false]; // Extension too long
    yield return ["12345 6789", false]; // Space instead of hyphen
    yield return ["123456789", false]; // No hyphen separator
  }

  public static IEnumerable<object[]> AllZipCodes()
  {
    foreach( var zipcode in ValidZipCodes() )
    {
      yield return zipcode;
    }

    foreach( var zipcode in InvalidZipCodes() )
    {
      yield return zipcode;
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
}
