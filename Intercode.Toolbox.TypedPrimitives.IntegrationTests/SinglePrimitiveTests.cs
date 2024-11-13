// Module Name: SinglePrimitiveTests.cs
// Author:      Eduardo Velasquez
// Copyright (c) 2024, Intercode Consulting, Inc.

namespace Intercode.Toolbox.TypedPrimitives.IntegrationTests;

using FluentResults;

[TypedPrimitive<float>]
public readonly partial struct UnvalidatedSinglePrimitive;

[TypedPrimitive( typeof( float ), Converters = TypedPrimitiveConverter.All )]
public readonly partial struct SinglePrimitive
{
  #region Constants

  public const string ExpectedValidationErrorMessage = "Cannot be null or zero";

  #endregion

  #region Implementation

  static partial void ValidatePartial(
    float? value,
    ref Result result )
  {
    result = Result.FailIf( value is null or 0L, ExpectedValidationErrorMessage );
  }

  #endregion
}

public class SinglePrimitiveTests
  : ValueTypePrimitiveTests<float, SinglePrimitive, UnvalidatedSinglePrimitive, SinglePrimitiveValueConverter>
{
  #region Constructors

  public SinglePrimitiveTests()
    : base( "Value must be a Number", [42.0f, 43.0f] )
  {
  }

  #endregion
}
