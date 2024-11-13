// Module Name: Int16PrimitiveTests.cs
// Author:      Eduardo Velasquez
// Copyright (c) 2024, Intercode Consulting, Inc.

namespace Intercode.Toolbox.TypedPrimitives.IntegrationTests;

using FluentResults;

[TypedPrimitive<short>]
public readonly partial struct UnvalidatedInt16Primitive;

[TypedPrimitive( typeof( short ), Converters = TypedPrimitiveConverter.All )]
public readonly partial struct Int16Primitive
{
  #region Constants

  public const string ExpectedValidationErrorMessage = "Cannot be null or zero";

  #endregion

  #region Implementation

  static partial void ValidatePartial(
    short? value,
    ref Result result )
  {
    result = Result.FailIf( value is null or 0, ExpectedValidationErrorMessage );
  }

  #endregion
}

public class Int16PrimitiveTests
  : ValueTypePrimitiveTests<short, Int16Primitive, UnvalidatedInt16Primitive, Int16PrimitiveValueConverter>
{
  #region Constructors

  public Int16PrimitiveTests()
    : base( "Value must be a Number", [42, 43] )
  {
  }

  #endregion
}
