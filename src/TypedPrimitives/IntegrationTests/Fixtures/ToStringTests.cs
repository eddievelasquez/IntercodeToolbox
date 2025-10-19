// Module Name: ToStringTests.cs
// Author:      Eduardo Velasquez
// Copyright (c) 2024, Intercode Consulting, Inc.

namespace Intercode.Toolbox.TypedPrimitives.IntegrationTests.Fixtures;

using FluentAssertions;

public abstract class ToStringTests<T, TDataFactory>
  where T: ITypedPrimitive
  where TDataFactory: ITestDataFactory
{
  #region Tests

  [Fact]
  public void ToString_Default_ReturnsEmpty()
  {
    var primitive = default( T );

    var result = primitive?.ToString();

    result.Should()
          .BeEmpty();
  }

  [Theory]
  [MemberData( nameof( GetToStringData ) )]
  public void ToString_Succeeds(
    T value )
  {
    // ToString of a typed primitive with value should return the ToString of the underlying value.
    value.HasValue.Should().BeTrue();
    var expected = value.GetValueObject()!.ToString()!;

    var actual = value.ToString();
    actual.Should().Be( expected );
  }

  #endregion

  #region Implementation

  public static IEnumerable<object?[]> GetToStringData()
  {
    return TDataFactory.GetValidValues()
                       .Take( 1 )
                       .Select( parameters => parameters[..1] );
  }

  #endregion
}
