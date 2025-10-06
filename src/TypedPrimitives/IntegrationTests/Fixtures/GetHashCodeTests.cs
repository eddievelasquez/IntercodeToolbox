// Module Name: GetHashCodeTests.cs
// Author:      Eduardo Velasquez
// Copyright (c) 2024, Intercode Consulting, Inc.

namespace Intercode.Toolbox.TypedPrimitives.IntegrationTests.Fixtures;

using FluentAssertions;

public abstract class GetHashCodeTests<T, TDataFactory>
  where T: notnull
  where TDataFactory: ITestDataFactory
{
  #region Public Methods

  [Theory]
  [MemberData( nameof( GetData ) )]
  public void GetHashCode_ShouldSucceed(
    T value,
    int expected )
  {
    var actual = value.GetHashCode();
    actual.Should().Be( expected );
  }

  public static IEnumerable<object?[]> GetData()
  {
    return TDataFactory.GetValidValues();
  }

  #endregion
}
