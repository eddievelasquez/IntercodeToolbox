// Module Name: ValuePrimitiveValidationTests.cs
// Author:      Eduardo Velasquez
// Copyright (c) 2024, Intercode Consulting, Inc.

namespace Intercode.Toolbox.TypedPrimitives.IntegrationTests.Fixtures;

using FluentAssertions;

public abstract class ValuePrimitiveValidationTests<T, TPrimitive, TDataFactory>
  where TPrimitive: struct, IValueTypePrimitive<TPrimitive, T>
  where T: struct
  where TDataFactory: ITestDataFactory
{
  #region Constants

  private static readonly string s_validationError = typeof( T ).IsNumeric()
    ? "Cannot be null or zero"
    : "Cannot be null or empty";

  #endregion

  #region Tests

  [Fact]
  public void IsDefault_WithDefaultValue_ReturnsTrue()
  {
    // Arrange
    var primitive = ( TPrimitive ) default;

    // Act
    var result = primitive.IsDefault;

    result.Should()
          .BeTrue();
  }

  [Theory]
  [MemberData( nameof( GetData ) )]
  public void IsDefault_WithValue_ReturnsFalse(
    T value )
  {
    var primitive = ( TPrimitive ) value;

    var result = primitive.IsDefault;

    result.Should()
          .BeFalse();
  }

  [Theory]
  [MemberData( nameof( GetInvalidData ) )]
  public void IsValid_InvalidValue_ReturnsFalse(
    T? value )
  {
    var isValid = TPrimitive.IsValid( value );

    isValid.Should()
           .BeFalse();
  }

  [Theory]
  [MemberData( nameof( GetData ) )]
  public void IsValid_ValidValue_ReturnsTrue(
    T? value )
  {
    var isValid = TPrimitive.IsValid( value );

    isValid.Should()
           .BeTrue();
  }

  [Theory]
  [MemberData( nameof( GetInvalidData ) )]
  public void Validate_WithInvalidValue_ReturnsFailure(
    T? value )
  {
    var result = TPrimitive.Validate( value );

    result.IsFailed.Should()
          .BeTrue();

    result.Errors.Select( error => error.Message )
          .Should()
          .ContainSingle()
          .Which
          .Should()
          .Be( s_validationError );
  }

  [Theory]
  [MemberData( nameof( GetData ) )]
  public void Validate_WithValidValue_ReturnsSuccess(
    T? value )
  {
    var result = TPrimitive.Validate( value );

    result.IsSuccess.Should()
          .BeTrue();
  }

  [Theory]
  [MemberData( nameof( GetInvalidData ) )]
  public void ValidateOrThrow_WithInvalidValue_ShouldThrow(
    T? value )
  {
    var act = () => TPrimitive.ValidateOrThrow( value );

    act.Should()
       .Throw<ArgumentException>()
       .WithMessage( s_validationError );
  }

  [Theory]
  [MemberData( nameof( GetData ) )]
  public void ValidateOrThrow_WithValidValue_ShouldNotThrow(T? value)
  {
    var act = () => TPrimitive.ValidateOrThrow( value );

    act.Should()
       .NotThrow();
  }



  #endregion

  #region Implementation

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
