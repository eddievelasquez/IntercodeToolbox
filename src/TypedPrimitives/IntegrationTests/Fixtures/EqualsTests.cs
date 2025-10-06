// Module Name: EqualsTests.cs
// Author:      Eduardo Velasquez
// Copyright (c) 2024, Intercode Consulting, Inc.

namespace Intercode.Toolbox.TypedPrimitives.IntegrationTests.Fixtures;

using FluentAssertions;

public abstract class EqualsTests<T, TDataFactory>()
  where T: notnull
  where TDataFactory: ITestDataFactory
{
  #region Public Methods

  [Theory]
  [MemberData( nameof( GetData ) )]
  public void CompareTo_Succeeds(
    T valueA,
    object? valueB,
    bool expected )
  {
    var actual = valueA.Equals( valueB );

    actual.Should().Be( expected );
  }

  public static IEnumerable<object?[]> GetData()
  {
    return TDataFactory.GetValidValues();
  }

  #endregion
}
