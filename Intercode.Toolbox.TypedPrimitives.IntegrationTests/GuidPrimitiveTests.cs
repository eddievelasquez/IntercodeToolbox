// Module Name: GuidPrimitiveTests.cs
// Author:      Eduardo Velasquez
// Copyright (c) 2024, Intercode Consulting, Inc.

namespace Intercode.Toolbox.TypedPrimitives.IntegrationTests;

using FluentAssertions;
using FluentResults;
using StjJsonSerializer = System.Text.Json.JsonSerializer;
using StjJsonException = System.Text.Json.JsonException;

[TypedPrimitive<Guid>]
public readonly partial struct UnvalidatedGuidPrimitive;

[TypedPrimitive( typeof( Guid ), Converters = TypedPrimitiveConverter.All )]
public readonly partial struct GuidPrimitive
{
  #region Constants

  public const string ExpectedValidationErrorMessage = "Cannot be null or empty";

  #endregion

  #region Implementation

  static partial void ValidatePartial(
    Guid? value,
    ref Result result )
  {
    result = Result.FailIf(
      value is null || value.Value.Equals( Guid.Empty ),
      ExpectedValidationErrorMessage
    );
  }

  #endregion
}

public class GuidPrimitiveTests
  : ValueTypePrimitiveTests<Guid, GuidPrimitive, UnvalidatedGuidPrimitive,
    GuidPrimitiveValueConverter>
{
  #region Constructors

  public GuidPrimitiveTests()
    : base(
      "Value must be a String",
      [Guid.Parse( "3fe7fbd6-ebd7-447c-95b3-0d5e5026d580" ), Guid.Parse( "b8afdeb5-87d2-44e9-ba51-9369cd257170" )]
    )
  {
  }

  #endregion

  [Fact]
  public override void SystemTextJson_Deserialization_WithInvalidType_ShouldThrow()
  {
    var json = ToJson( 1 );

    var act = () => StjJsonSerializer.Deserialize<JsonTestClass>( json );

    act.Should()
       .Throw<StjJsonException>()
       .WithMessage( "Value must be a String" );
  }
}
