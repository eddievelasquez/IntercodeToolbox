// Module Name: ComparableTests.cs
// Author:      Eduardo Velasquez
// Copyright (c) 2024, Intercode Consulting, Inc.

namespace Intercode.Toolbox.TypedPrimitives.IntegrationTests.Fixtures;

using FluentAssertions;

public abstract class ComparableTests<TComparable, TDataFactory>
  where TComparable: IComparable
  where TDataFactory: ITestDataFactory
{
  #region Public Methods

  [Theory]
  [MemberData( nameof( GetData ) )]
  public void CompareTo_Succeeds(
    TComparable valueA,
    object? valueB,
    int expected )
  {
    var actual = valueA.CompareTo( valueB );

    switch( expected )
    {
      case 0:
        actual.Should().Be( 0 );
        break;

      case < 0:
        actual.Should().BeLessThan( 0 );
        break;

      default:
        actual.Should().BeGreaterThan( 0 );
        break;
    }
  }

  public static IEnumerable<object?[]> GetData()
  {
    return TDataFactory.GetValidValues();
  }

  #endregion
}
