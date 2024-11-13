// Module Name: DoublePrimitiveTests.cs
// Author:      Eduardo Velasquez
// Copyright (c) 2024, Intercode Consulting, Inc.

namespace Intercode.Toolbox.TypedPrimitives.IntegrationTests;

using FluentResults;

[TypedPrimitive<double>]
public readonly partial struct UnvalidatedDoublePrimitive;

[TypedPrimitive( typeof( double ), Converters = TypedPrimitiveConverter.All )]
public readonly partial struct DoublePrimitive
{
  #region Constants

  public const string ExpectedValidationErrorMessage = "Cannot be null or zero";

  #endregion

  #region Implementation

  static partial void ValidatePartial(
    double? value,
    ref Result result )
  {
    result = Result.FailIf( value is null or 0L, ExpectedValidationErrorMessage );
  }

  #endregion
}

public class DoublePrimitiveTests
  : ValueTypePrimitiveTests<double, DoublePrimitive, UnvalidatedDoublePrimitive, DoublePrimitiveValueConverter>
{
  #region Constructors

  public DoublePrimitiveTests()
    : base(
      "Value must be a Number",
      [42.0, 43.0]
    )
  {
  }

  #endregion
}
