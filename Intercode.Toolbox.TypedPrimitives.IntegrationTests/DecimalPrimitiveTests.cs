// Module Name: DecimalPrimitiveTests.cs
// Author:      Eduardo Velasquez
// Copyright (c) 2024, Intercode Consulting, Inc.

namespace Intercode.Toolbox.TypedPrimitives.IntegrationTests;

using FluentResults;

[TypedPrimitive<decimal>]
public readonly partial struct UnvalidatedDecimalPrimitive;

[TypedPrimitive( typeof( decimal ), Converters = TypedPrimitiveConverter.All )]
public readonly partial struct DecimalPrimitive
{
  #region Constants

  public const string ExpectedValidationErrorMessage = "Cannot be null or zero";

  #endregion

  #region Implementation

  static partial void ValidatePartial(
    decimal? value,
    ref Result result )
  {
    result = Result.FailIf( value is null or 0L, ExpectedValidationErrorMessage );
  }

  #endregion
}

public class DecimalPrimitiveTests
  : ValueTypePrimitiveTests<decimal, DecimalPrimitive, UnvalidatedDecimalPrimitive, DecimalPrimitiveValueConverter>
{
  #region Constructors

  public DecimalPrimitiveTests()
    : base( "Value must be a Number", [42.0m, 43.0m] )
  {
  }

  #endregion
}
