// Module Name: SBytePrimitiveTests.cs
// Author:      Eduardo Velasquez
// Copyright (c) 2024, Intercode Consulting, Inc.

namespace Intercode.Toolbox.TypedPrimitives.IntegrationTests;

using FluentResults;

[TypedPrimitive<sbyte>]
public readonly partial struct UnvalidatedSBytePrimitive;

[TypedPrimitive( typeof( sbyte ), Converters = TypedPrimitiveConverter.All )]
public readonly partial struct SBytePrimitive
{
  #region Constants

  public const string ExpectedValidationErrorMessage = "Cannot be null or zero";

  #endregion

  #region Implementation

  static partial void ValidatePartial(
    sbyte? value,
    ref Result result )
  {
    result = Result.FailIf( value is null or 0, ExpectedValidationErrorMessage );
  }

  #endregion
}

public class SBytePrimitiveTests
  : ValueTypePrimitiveTests<sbyte, SBytePrimitive, UnvalidatedSBytePrimitive, SBytePrimitiveValueConverter>
{
  #region Constructors

  public SBytePrimitiveTests()
    : base( "Value must be a Number", [42, -43] )
  {
  }

  #endregion
}
