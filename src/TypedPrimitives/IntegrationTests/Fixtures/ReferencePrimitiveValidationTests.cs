// Module Name: ReferencePrimitiveValidationTests.cs
// Author:      Eduardo Velasquez
// Copyright (c) 2024, Intercode Consulting, Inc.

namespace Intercode.Toolbox.TypedPrimitives.IntegrationTests.Fixtures;

using FluentAssertions;

public abstract class ReferencePrimitiveValidationTests<T, TTypedPrimitive, TDataFactory>
  where TTypedPrimitive: IReferenceTypedPrimitive<T, TTypedPrimitive>
  where TDataFactory: ITestDataFactory
  where T : class
{
  #region Constants

  private static readonly string s_validationError = typeof( T ).IsNumeric()
    ? "Cannot be null or zero"
    : "Cannot be null or empty";

  #endregion

  #region Tests

  [Fact]
  public void HasValue_WithDefaultValue_ReturnsFalse()
  {
    // Arrange
    var typedPrimitive = (TTypedPrimitive) default!;

    // Act
    var result = typedPrimitive.HasValue;

    result.Should()
          .BeFalse();
  }

  [Theory]
  [MemberData( nameof( GetData ) )]
  public void HasValue_WithValue_ReturnsTrue(
    T value )
  {
    var typedPrimitive = TTypedPrimitive.CreateOrThrow( value );

    var result = typedPrimitive.HasValue;

    result.Should()
          .BeTrue();
  }

  [Theory]
  [MemberData( nameof( GetInvalidData ) )]
  public void IsValid_InvalidValue_ReturnsFalse(
    T? value )
  {
    var isValid = TTypedPrimitive.IsValid( value );

    isValid.Should()
           .BeFalse();
  }

  [Theory]
  [MemberData( nameof( GetData ) )]
  public void IsValid_ValidValue_ReturnsTrue(
    T? value )
  {
    var isValid = TTypedPrimitive.IsValid( value );

    isValid.Should()
           .BeTrue();
  }

  [Theory]
  [MemberData( nameof( GetInvalidData ) )]
  public void Validate_WithInvalidValue_ReturnsFailure(
    T? value )
  {
    var result = TTypedPrimitive.Validate( value );

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
    var result = TTypedPrimitive.Validate( value );

    result.IsSuccess.Should()
          .BeTrue();
  }

  [Theory]
  [MemberData( nameof( GetInvalidData ) )]
  public void ValidateOrThrow_WithInvalidValue_ShouldThrow(
    T? value )
  {
    var act = () => TTypedPrimitive.ValidateOrThrow( value );

    act.Should()
       .Throw<ArgumentException>()
       .WithMessage( s_validationError );
  }

  [Theory]
  [MemberData( nameof( GetData ) )]
  public void ValidateOrThrow_WithValidValue_ShouldNotThrow(
    T? value )
  {
    var act = () => TTypedPrimitive.ValidateOrThrow( value );

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
