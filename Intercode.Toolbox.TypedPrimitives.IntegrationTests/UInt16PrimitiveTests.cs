// Module Name: UInt16PrimitiveTests.cs
// Author:      Eduardo Velasquez
// Copyright (c) 2024, Intercode Consulting, Inc.

namespace Intercode.Toolbox.TypedPrimitives.IntegrationTests;

using FluentResults;

[TypedPrimitive<ushort>]
public readonly partial struct UnvalidatedUInt16Primitive;

[TypedPrimitive( typeof( ushort ), Converters = TypedPrimitiveConverter.All )]
public readonly partial struct UInt16Primitive
{
  #region Constants

  public const string ExpectedValidationErrorMessage = "Cannot be null or zero";

  #endregion

  #region Implementation

  static partial void ValidatePartial(
    ushort? value,
    ref Result result )
  {
    result = Result.FailIf( value is null or 0, ExpectedValidationErrorMessage );
  }

  #endregion
}

public class UInt16PrimitiveTests
  : ValueTypePrimitiveTests<ushort, UInt16Primitive, UnvalidatedUInt16Primitive, UInt16PrimitiveValueConverter>
{
  #region Constructors

  public UInt16PrimitiveTests()
    : base( "Value must be a Number", [42, 43] )
  {
  }

  #endregion
}
