// Module Name: ZipCode.cs
// Author:      Eduardo Velasquez
// Copyright (c) 2025, Intercode Consulting, Inc.

namespace Intercode.Toolbox.TypedPrimitives.Standard.PostalCodes.US;

using FluentResults;
using System.Text.RegularExpressions;

[TypedPrimitive<string>]
public readonly partial struct ZipCode
{
  private const int DEFAULT_REGEX_TIMEOUT_IN_MS = 200;
  private const string DEFAULT_PATTERN = @"^\d{5}(-\d{4})?$";

  static partial void ValidatePartial(
    string? value,
    ref Result result )
  {
    if( value is null )
    {
      result = Result.Fail( $"A {nameof( ZipCode )} cannot be null" );
      return;
    }

    if( !CreateDefaultRegex().IsMatch( value ) )
    {
      result = Result.Fail( $"Invalid {nameof( ZipCode )} format" );
    }

    // result is set to Ok by the Validate implementation
  }

  [GeneratedRegex( DEFAULT_PATTERN, RegexOptions.IgnoreCase, DEFAULT_REGEX_TIMEOUT_IN_MS )]
  private static partial Regex CreateDefaultRegex();
}
