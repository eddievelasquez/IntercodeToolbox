// Module Name: BytePrimitiveTests.cs
// Author:      Eduardo Velasquez
// Copyright (c) 2024, Intercode Consulting, Inc.

namespace Intercode.Toolbox.TypedPrimitives.IntegrationTests;

using FluentResults;

[TypedPrimitive<byte>]
public readonly partial struct UnvalidatedBytePrimitive;

[TypedPrimitive( typeof( byte ), Converters = TypedPrimitiveConverter.All )]
public readonly partial struct BytePrimitive
{
  #region Constants

  public const string ExpectedValidationErrorMessage = "Cannot be null or zero";

  #endregion

  #region Implementation

  static partial void ValidatePartial(
    byte? value,
    ref Result result )
  {
    result = Result.FailIf( value is null or 0, ExpectedValidationErrorMessage );
  }

  #endregion
}

public class BytePrimitiveTests
  : ValueTypePrimitiveTests<byte, BytePrimitive, UnvalidatedBytePrimitive, BytePrimitiveValueConverter>
{
  #region Constructors

  public BytePrimitiveTests()
    : base(
      "Value must be a Number",
      [42, 43]
    )
  {
  }

  #endregion
}
