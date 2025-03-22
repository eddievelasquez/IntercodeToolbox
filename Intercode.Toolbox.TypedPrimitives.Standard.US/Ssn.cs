// Module Name: Ssn.cs
// Author:      Eduardo Velasquez
// Copyright (c) 2025, Intercode Consulting, Inc.

namespace Intercode.Toolbox.TypedPrimitives.Standard.Us;

using System.Text.RegularExpressions;
using FluentResults;

[TypedPrimitive<string>]
public readonly partial struct Ssn
{
  #region Constants

  private const int DEFAULT_REGEX_TIMEOUT_IN_MS = 200;
  private const string DEFAULT_PATTERN = @"^(?!000|666|9\d\d)\d{3}-?(?!00)\d{2}-?(?!0000)\d{4}$";

  #endregion

  #region Implementation

  static partial void ValidatePartial(
    string? value,
    ref Result result )
  {
    if( value is null )
    {
      result = Result.Fail( $"A {nameof( Ssn )} cannot be null" );
      return;
    }

    if( !CreateDefaultRegex().IsMatch( value ) )
    {
      result = Result.Fail( $"Invalid {nameof( Ssn )} format" );
    }

    // result is set to Ok by the Validate implementation
  }

  [GeneratedRegex( DEFAULT_PATTERN, RegexOptions.IgnoreCase, DEFAULT_REGEX_TIMEOUT_IN_MS )]
  private static partial Regex CreateDefaultRegex();

  #endregion
}
