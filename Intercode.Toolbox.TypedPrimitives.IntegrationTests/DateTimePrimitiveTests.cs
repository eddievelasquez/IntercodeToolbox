// Module Name: DateTimePrimitiveTests.cs
// Author:      Eduardo Velasquez
// Copyright (c) 2024, Intercode Consulting, Inc.

namespace Intercode.Toolbox.TypedPrimitives.IntegrationTests;

using FluentAssertions;
using FluentResults;
using Newtonsoft.Json;
using StjJsonSerializer = System.Text.Json.JsonSerializer;
using StjJsonException = System.Text.Json.JsonException;

[TypedPrimitive<DateTime>]
public readonly partial struct UnvalidatedDateTimePrimitive;

[TypedPrimitive( typeof( DateTime ), Converters = TypedPrimitiveConverter.All )]
public readonly partial struct DateTimePrimitive
{
  #region Constants

  public const string ExpectedValidationErrorMessage = "Cannot be null or empty";

  #endregion

  #region Implementation

  static partial void ValidatePartial(
    DateTime? value,
    ref Result result )
  {
    result = Result.FailIf(
      value is null || value.Value == DateTime.MinValue,
      ExpectedValidationErrorMessage
    );
  }

  #endregion
}

public class DateTimePrimitiveTests
  : ValueTypePrimitiveTests<DateTime, DateTimePrimitive, UnvalidatedDateTimePrimitive,
    DateTimePrimitiveValueConverter>
{
  #region Setup/Teardown

  public DateTimePrimitiveTests()
    : base(
      "Value must be a String",
      [DateTime.ParseExact( "1995-12-01T15:00:00", "s", null ), DateTime.ParseExact( "2018-02-06T12:45:00", "s", null )]
    )
  {
    JsonConvert.DefaultSettings = () => new JsonSerializerSettings { DateParseHandling = DateParseHandling.None };
  }

  #endregion

  #region Tests

  [Fact]
  public override void SystemTextJson_Deserialization_WithInvalidType_ShouldThrow()
  {
    var json = ToJson( 1 );

    var act = () => StjJsonSerializer.Deserialize<JsonTestClass>( json );

    act.Should()
       .Throw<StjJsonException>()
       .WithMessage( "Value must be a String" );
  }

  #endregion
}
