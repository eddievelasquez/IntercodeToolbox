// Module Name: UnvalidatedReferencePrimitiveValidationTests.cs
// Author:      Eduardo Velasquez
// Copyright (c) 2024, Intercode Consulting, Inc.

namespace Intercode.Toolbox.TypedPrimitives.IntegrationTests.Fixtures;

using FluentAssertions;

public abstract class UnvalidatedReferencePrimitiveValidationTests<T, TTypedPrimitive, TDataFactory>
  where TTypedPrimitive: struct, IReferenceTypedPrimitive<T, TTypedPrimitive>
  where TDataFactory: ITestDataFactory
  where T : class
{
  #region Public Methods

  [Theory]
  [MemberData( nameof( GetInvalidData ) )]
  public void IsValid_InvalidValue_ReturnsTrue(
    T? value )
  {
    var isValid = TTypedPrimitive.IsValid( value );

    isValid.Should()
           .BeTrue();
  }

  [Theory]
  [MemberData( nameof( GetInvalidData ) )]
  public void Validate_WithInvalidValue_ReturnsSuccess(
    T? value )
  {
    var result = TTypedPrimitive.Validate( value );

    result.IsSuccess.Should()
          .BeTrue();
  }

  [Theory]
  [MemberData( nameof( GetInvalidData ) )]
  public void ValidateOrThrow_WithInvalidValue_ShouldNotThrow(
    T? value )
  {
    var act = () => TTypedPrimitive.ValidateOrThrow( value );
    act.Should().NotThrow();
  }

  public static IEnumerable<object?[]> GetData()
  {
    return TDataFactory.GetValidValues();
  }

  public static IEnumerable<object?[]> GetInvalidData()
  {
    return TDataFactory.GetInvalidValues();
  }

  #endregion
}
