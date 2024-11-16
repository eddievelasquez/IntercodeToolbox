// Module Name: UnvalidatedValuePrimitiveValidationTests.cs
// Author:      Eduardo Velasquez
// Copyright (c) 2024, Intercode Consulting, Inc.

namespace Intercode.Toolbox.TypedPrimitives.IntegrationTests.Fixtures;

using FluentAssertions;

public abstract class UnvalidatedValuePrimitiveValidationTests<T, TPrimitive, TDataFactory>
  where TPrimitive: struct, IValueTypePrimitive<TPrimitive, T>
  where T: struct
  where TDataFactory: ITestDataFactory
{
  #region Public Methods

  [Theory]
  [MemberData( nameof( GetInvalidData ) )]
  public void IsValid_InvalidValue_ReturnsTrue(
    T? value )
  {
    var isValid = TPrimitive.IsValid( value );

    isValid.Should()
           .BeTrue();
  }

  [Theory]
  [MemberData( nameof( GetInvalidData ) )]
  public void Validate_WithInvalidValue_ReturnsSuccess(
    T? value )
  {
    var result = TPrimitive.Validate( value );

    result.IsSuccess.Should()
          .BeTrue();
  }

  [Theory]
  [MemberData( nameof( GetInvalidData ) )]
  public void ValidateOrThrow_WithInvalidValue_ShouldNotThrow(
    T? value )
  {
    var act = () => TPrimitive.ValidateOrThrow( value );
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
