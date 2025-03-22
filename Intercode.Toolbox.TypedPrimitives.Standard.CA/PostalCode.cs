// Module Name: PostalCode.cs
// Author:      Eduardo Velasquez
// Copyright (c) 2025, Intercode Consulting, Inc.

namespace Intercode.Toolbox.TypedPrimitives.Standard.Ca;

using System.Text.RegularExpressions;
using FluentResults;

[TypedPrimitive<string>]
public readonly partial struct PostalCode
{
  private const int DEFAULT_REGEX_TIMEOUT_IN_MS = 200;
  private const string DEFAULT_PATTERN = @"^[ABCEGHJKLMNPRSTVXY]\d[ABCEGHJKLMNPRSTVWXYZ][ ]?\d[ABCEGHJKLMNPRSTVWXYZ]\d$";

  static partial void ValidatePartial(
    string? value,
    ref Result result )
  {
    if( value is null )
    {
      result = Result.Fail( $"A {nameof(PostalCode)} cannot be null" );
      return;
    }

    if( !CreateDefaultRegex().IsMatch( value ) )
    {
      result = Result.Fail( $"Invalid {nameof(PostalCode)} format" );
    }

    // result is set to Ok by the Validate implementation
  }

  [GeneratedRegex( DEFAULT_PATTERN, RegexOptions.IgnoreCase, DEFAULT_REGEX_TIMEOUT_IN_MS )]
  private static partial Regex CreateDefaultRegex();
}
