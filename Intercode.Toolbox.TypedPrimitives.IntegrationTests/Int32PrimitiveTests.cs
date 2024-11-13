// Module Name: Int32PrimitiveTests.cs
// Author:      Eduardo Velasquez
// Copyright (c) 2024, Intercode Consulting, Inc.

namespace Intercode.Toolbox.TypedPrimitives.IntegrationTests;

using FluentResults;

[TypedPrimitive<int>]
public readonly partial struct UnvalidatedInt32Primitive;

[TypedPrimitive( typeof( int ), Converters = TypedPrimitiveConverter.All )]
public readonly partial struct Int32Primitive
{
  #region Constants

  public const string ExpectedValidationErrorMessage = "Cannot be null or zero";

  #endregion

  #region Implementation

  static partial void ValidatePartial(
    int? value,
    ref Result result )
  {
    result = Result.FailIf( value is null or 0, ExpectedValidationErrorMessage );
  }

  #endregion
}

public class Int32PrimitiveTests
  : ValueTypePrimitiveTests<int, Int32Primitive, UnvalidatedInt32Primitive, Int32PrimitiveValueConverter>
{
  #region Constructors

  public Int32PrimitiveTests()
    : base( "Value must be a Number", [42, 43] )
  {
  }

  #endregion
}
