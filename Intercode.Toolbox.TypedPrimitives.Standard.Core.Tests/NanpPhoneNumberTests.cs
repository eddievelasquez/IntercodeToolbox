// Module Name: ZipCodeTests.cs
// Author:      Eduardo Velasquez
// Copyright (c) 2025, Intercode Consulting, Inc.

namespace Intercode.Toolbox.TypedPrimitives.Standard.Core.Tests;

using FluentAssertions;
using Intercode.Toolbox.TypedPrimitives.Standard.Core;

public class NanpPhoneNumberTests
{
  #region Public Methods

  public static IEnumerable<string> ValidPhoneNumbers()
  {
    // Basic formats
    yield return "1234567890";
    yield return "123-456-7890";
    yield return "123.456.7890";
    yield return "123 456 7890";
    yield return "(123) 456-7890";
    yield return "(123)456-7890";

    // With country code
    yield return "+11234567890";
    yield return "+1-123-456-7890";
    yield return "+1 123-456-7890";
    yield return "+1 (123) 456-7890";
    yield return "+1.123.456.7890";

    // Just country code prefix without +
    yield return "11234567890";
    yield return "1-123-456-7890";
    yield return "1 123-456-7890";
    yield return "1 (123) 456-7890";
    yield return "1(123)456-7890";

    // Mix of separators
    yield return "(123)-456-7890";
    yield return "123 456-7890";
    yield return "123.456 7890";
  }

  public static IEnumerable<string> InvalidPhoneNumbers()
  {
    // Too few digits
    yield return "123-456-789";
    yield return "12-456-7890";
    yield return "123-45-7890";

    // Too many digits
    yield return "123-4567-7890";
    yield return "123-456-78901";

    // Invalid country code
    yield return "+2-123-456-7890";
    yield return "2-123-456-7890";

    // Invalid characters
    yield return "abc-def-ghij";
    yield return "123-abc-7890";
    yield return "123-456-defg";
    yield return "(1a3) 456-7890";

    // Missing parts
    yield return "() 456-7890";
    yield return "(123) -7890";
    yield return "(123) 456-";

    // Invalid formats
    yield return "123)(456-7890";
    yield return "123-456-7890(";

    // International formats (non-NANP)
    yield return "+44 20 1234 5678";
    yield return "+61 2 1234 5678";

    // Empty or whitespace
    yield return "";
    yield return "   ";
    yield return null!;

    // Other invalid formats
    yield return "123.456,7890";
    yield return "123/456/7890";
    yield return "#123-456-7890";
  }

  public static IEnumerable<object[]> AllPhoneNumbers()
  {
    foreach( var value in ValidPhoneNumbers() )
    {
      yield return [value, true];
    }

    foreach( var value in InvalidPhoneNumbers() )
    {
      yield return [value, false];
    }
  }

  [Theory]
  [MemberData( nameof( AllPhoneNumbers ) )]
  public void Create_ShouldHandleValidAndInvalidPhoneNumbers(
    string value,
    bool expected )
  {
    var result = NanpPhoneNumber.Create( value );
    result.IsSuccess.Should().Be( expected );
  }

  [Theory]
  [MemberData( nameof( AllPhoneNumbers ) )]
  public void Validate_ShouldHandleValidAndInvalidPhoneNumbers(
    string value,
    bool expected )
  {
    var result = NanpPhoneNumber.Validate( value );
    result.IsSuccess.Should().Be( expected );
  }

  #endregion
}
