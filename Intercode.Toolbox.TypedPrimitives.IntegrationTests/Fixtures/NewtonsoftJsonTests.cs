// Module Name: NewtonsoftJsonTests.cs
// Author:      Eduardo Velasquez
// Copyright (c) 2024, Intercode Consulting, Inc.

namespace Intercode.Toolbox.TypedPrimitives.IntegrationTests.Fixtures;

using FluentAssertions;
using Newtonsoft.Json;

public abstract class NewtonsoftJsonTests<T, TDataFactory>
  : JsonTestsBase<T, TDataFactory>
  where TDataFactory: ITestDataFactory
{
  #region Public Methods

  [Theory]
  [MemberData( nameof( GetInvalidData ) )]
  public void Deserialization_InvalidValue_ShouldThrow(
    object? value )
  {
    var json = ToJson( value );

    var act = () => JsonConvert.DeserializeObject<Test>( json );

    act.Should()
       .Throw<JsonException>();
  }

  [Theory]
  [MemberData( nameof( GetData ) )]
  public void SerializeObject_ShouldRoundTrip(
    T value )
  {
    var expected = new Test { Value = value };

    var json = JsonConvert.SerializeObject( expected );
    json.Should().NotBeNullOrEmpty();

    var actual = JsonConvert.DeserializeObject<Test>( json );
    actual.Should().NotBeNull();

    actual!.Value.Should().Be( expected.Value );
  }

  #endregion
}
