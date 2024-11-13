// Module Name: Int64PrimitiveTests.cs
// Author:      Eduardo Velasquez
// Copyright (c) 2024, Intercode Consulting, Inc.

namespace Intercode.Toolbox.TypedPrimitives.IntegrationTests;

using FluentResults;

[TypedPrimitive<long>]
public readonly partial struct UnvalidatedInt64Primitive;

[TypedPrimitive( typeof( long ), Converters = TypedPrimitiveConverter.All )]
public readonly partial struct Int64Primitive
{
  #region Constants

  public const string ExpectedValidationErrorMessage = "Cannot be null or zero";

  #endregion

  #region Implementation

  static partial void ValidatePartial(
    long? value,
    ref Result result )
  {
    result = Result.FailIf( value is null or 0L, ExpectedValidationErrorMessage );
  }

  #endregion
}

public class Int64PrimitiveTests
  : ValueTypePrimitiveTests<long, Int64Primitive, UnvalidatedInt64Primitive, Int64PrimitiveValueConverter>
{
  #region Constructors

  public Int64PrimitiveTests()
    : base( "Value must be a Number", [42, 43] )
  {
  }

  #endregion
}
