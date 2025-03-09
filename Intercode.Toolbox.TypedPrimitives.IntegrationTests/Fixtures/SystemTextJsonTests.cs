// Module Name: SystemTextJsonTests.cs
// Author:      Eduardo Velasquez
// Copyright (c) 2024, Intercode Consulting, Inc.

namespace Intercode.Toolbox.TypedPrimitives.IntegrationTests.Fixtures;

using System.Text.Json;
using FluentAssertions;

public abstract class SystemTextJsonTests<T, TDataFactory>
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

    var act = () => JsonSerializer.Deserialize<Test>( json );

    act.Should()
       .Throw<JsonException>();
  }

  [Theory]
  [MemberData( nameof( GetData ) )]
  public void SerializeObject_ShouldRoundTrip(
    T value )
  {
    var expected = new Test { Value = value };

    var json = JsonSerializer.Serialize( expected );
    json.Should().NotBeNullOrEmpty();

    var actual = JsonSerializer.Deserialize<Test>( json );
    actual.Should().NotBeNull();

    actual!.Value.Should().Be( expected.Value );
  }

  #endregion
}
