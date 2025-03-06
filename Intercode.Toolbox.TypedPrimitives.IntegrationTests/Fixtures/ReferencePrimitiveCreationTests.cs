// Module Name: ReferencePrimitiveCreationTests.cs
// Author:      Eduardo Velasquez
// Copyright (c) 2024, Intercode Consulting, Inc.

namespace Intercode.Toolbox.TypedPrimitives.IntegrationTests.Fixtures;

using FluentAssertions;

public abstract class ReferencePrimitiveCreationTests<T, TTypedPrimitive, TDataFactory>
  where TTypedPrimitive: IReferenceTypedPrimitive<T, TTypedPrimitive>
  where TDataFactory: ITestDataFactory
  where T: class
{
  #region Constants

  private static readonly string s_validationError = typeof( T ).IsNumeric()
    ? "Cannot be null or zero"
    : "Cannot be null or empty";

  #endregion

  #region Public Methods

  [Theory]
  [MemberData( nameof( GetInvalidData ) )]
  public void Create_WithInvalidValue_ReturnsFailure(
    T? value )
  {
    var result = TTypedPrimitive.Create( value );

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
  public void Create_WithValidValue_ReturnsSuccess(
    T value )
  {
    var result = TTypedPrimitive.Create( value );

    // Assert
    result.IsSuccess.Should()
          .BeTrue();

    result.Value.Value.Should()
          .Be( value );
  }

  [Theory]
  [MemberData( nameof( GetInvalidData ) )]
  public void CreateOrThrow_WithInvalidValue_Throws(
    object? value )
  {
    var act = () => TTypedPrimitive.CreateOrThrow( ( T? ) value );

    act.Should()
       .Throw<ArgumentException>()
       .WithMessage( s_validationError );
  }

  [Theory]
  [MemberData( nameof( GetData ) )]
  public void CreateOrThrow_WithValidValue_ReturnsPrimitive(
    T? value )
  {
    var primitive = TTypedPrimitive.CreateOrThrow( value );

    primitive.Value.Should().Be( value );
  }

  [Theory]
  [MemberData( nameof( GetInvalidData ) )]
  public void ExplicitOperator_ValueToPrimitive_WithInvalidValue_ShouldThrow(
    T value )
  {
    var act = () => (TTypedPrimitive) value;

    act.Should()
       .Throw<InvalidOperationException>()
       .WithMessage( s_validationError );
  }

  [Theory]
  [MemberData( nameof( GetData ) )]
  public void ExplicitOperator_ValueToPrimitive_WithValidValue_Succeeds(
    T value )
  {
    var result = TTypedPrimitive.CreateOrThrow( value ).Value;

    result.Should()
          .Be( value );
  }

  [Theory]
  [MemberData( nameof( GetData ) )]
  public void ExplicitOperator_PrimitiveToValue_ReturnsValue(
    T value )
  {
    var typedPrimitive = TTypedPrimitive.CreateOrThrow( value );

    ( ( T ) typedPrimitive ).Should()
                            .Be( value );
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
