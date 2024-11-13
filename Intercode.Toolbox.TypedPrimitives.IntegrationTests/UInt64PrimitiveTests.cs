// Module Name: UInt64PrimitiveTests.cs
// Author:      Eduardo Velasquez
// Copyright (c) 2024, Intercode Consulting, Inc.

namespace Intercode.Toolbox.TypedPrimitives.IntegrationTests;

using FluentResults;

[TypedPrimitive<ulong>]
public readonly partial struct UnvalidatedUInt64Primitive;

[TypedPrimitive( typeof( ulong ), Converters = TypedPrimitiveConverter.All )]
public readonly partial struct UInt64Primitive
{
  #region Constants

  public const string ExpectedValidationErrorMessage = "Cannot be null or zero";

  #endregion

  #region Implementation

  static partial void ValidatePartial(
    ulong? value,
    ref Result result )
  {
    result = Result.FailIf( value is null or 0L, ExpectedValidationErrorMessage );
  }

  #endregion
}

public class UInt64PrimitiveTests
  : ValueTypePrimitiveTests<ulong, UInt64Primitive, UnvalidatedUInt64Primitive, UInt64PrimitiveValueConverter>
{
  #region Constructors

  public UInt64PrimitiveTests()
    : base(
      "Value must be a Number",
      [42, 43]
    )
  {
  }

  #endregion
}
