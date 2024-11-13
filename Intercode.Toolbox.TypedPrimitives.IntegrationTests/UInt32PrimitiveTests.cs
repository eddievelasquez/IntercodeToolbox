// Module Name: UInt32PrimitiveTests.cs
// Author:      Eduardo Velasquez
// Copyright (c) 2024, Intercode Consulting, Inc.

namespace Intercode.Toolbox.TypedPrimitives.IntegrationTests;

using FluentResults;

[TypedPrimitive<uint>]
public readonly partial struct UnvalidatedUInt32Primitive;

[TypedPrimitive( typeof( uint ), Converters = TypedPrimitiveConverter.All )]
public readonly partial struct UInt32Primitive
{
  #region Constants

  public const string ExpectedValidationErrorMessage = "Cannot be null or zero";

  #endregion

  #region Implementation

  static partial void ValidatePartial(
    uint? value,
    ref Result result )
  {
    result = Result.FailIf( value is null or 0, ExpectedValidationErrorMessage );
  }

  #endregion
}

public class UInt32PrimitiveTests
  : ValueTypePrimitiveTests<uint, UInt32Primitive, UnvalidatedUInt32Primitive, UInt32PrimitiveValueConverter>
{
  #region Constructors

  public UInt32PrimitiveTests()
    : base( "Value must be a Number", [42, 43] )
  {
  }

  #endregion
}
