// Module Name: MutableHashSetLookupTests.cs
// Author:      Eduardo Velasquez
// Copyright (c) 2024, Intercode Consulting, Inc.

namespace Intercode.Toolbox.Collections.Tests;

using System.Collections;
using FluentAssertions;

public class MutableHashSetLookupTests
{
  #region Tests

  [Fact]
  public void Add_WithValue_ShouldReturnFalse_WhenValueAlreadyExists()
  {
    var lookup = new MutableHashSetLookup<int, string>() { { 1, "Value1" } };

    lookup.Add( 1, "Value1" )
          .Should()
          .BeFalse();

    lookup.Should()
          .HaveCount( 1 );

    lookup.Contains( 1, "Value1" )
          .Should()
          .BeTrue();
  }

  [Fact]
  public void Add_WithValue_ShouldReturnTrue_WhenAddingNewValue()
  {
    var lookup = new MutableHashSetLookup<int, string>();

    lookup.Add( 1, "Value1" )
          .Should()
          .BeTrue();

    lookup.Should()
          .HaveCount( 1 );

    lookup.Contains( 1, "Value1" )
          .Should()
          .BeTrue();
  }

  [Fact]
  public void Add_WithValues_ShouldAddValues()
  {
    var lookup = new MutableHashSetLookup<int, string>();

    lookup.Add( 1, ["Value1", "Value2"] );

    lookup.Should()
          .HaveCount( lookup.Count );

    lookup.Contains( 1, "Value1" )
          .Should()
          .BeTrue();

    lookup.Contains( 1, "Value2" )
          .Should()
          .BeTrue();
  }

  [Fact]
  public void Clear_ShouldRemoveAllValues()
  {
    var lookup = new MutableHashSetLookup<int, string>
    {
      { 1, "Value1" },
      { 2, "Value2" }
    };

    lookup.Clear();

    lookup.Should()
          .BeEmpty();
  }

  [Fact]
  public void Contains_WithKey_ShouldReturnFalse_WhenKeyIsNotFound()
  {
    var lookup = new MutableHashSetLookup<int, string> { { 1, "Value1" } };

    lookup.Contains( 2 )
          .Should()
          .BeFalse();
  }

  [Fact]
  public void Contains_WithKey_ShouldReturnTrue_WhenKeyIsFound()
  {
    var lookup = new MutableHashSetLookup<int, string> { { 1, "Value1" } };

    lookup.Contains( 1 )
          .Should()
          .BeTrue();
  }

  [Fact]
  public void Contains_WithKeyAndValue_ShouldReturnFalse_WhenKeyIsNotFound()
  {
    var lookup = new MutableHashSetLookup<int, string> { { 1, "Value1" } };

    lookup.Contains( 1, "Value2" )
          .Should()
          .BeFalse();
  }

  [Fact]
  public void Contains_WithKeyAndValue_ShouldReturnTrue_WhenKeyIsFound()
  {
    var lookup = new MutableHashSetLookup<int, string> { { 1, "Value1" } };

    lookup.Contains( 1, "Value1" )
          .Should()
          .BeTrue();
  }

  [Fact]
  public void Count_ShouldReturnTotalNumberOfGroupings()
  {
    var lookup = new MutableHashSetLookup<int, string>
    {
      { 1, "Value1" },
      { 1, "Value2" },
      { 2, "Value3" }
    };

    lookup.Count.Should()
          .Be( 2 );
  }

  [Fact]
  public void Ctor_ShouldUseDefaultComparer_WhenAddingValues()
  {
    var lookup = new MutableHashSetLookup<string, string>();

    lookup.Add( "Key1", "Value1" )
          .Should()
          .BeTrue();

    lookup.Count.Should()
          .Be( 1 );

    lookup.Add( "KEY1", "Value2" )
          .Should()
          .BeTrue();

    lookup.Count.Should()
          .Be( 2 );

    lookup.TryGetValues( "Key1", out var values )
          .Should()
          .BeTrue();

    values.Should()
          .BeEquivalentTo( ["Value1"] );

    lookup.TryGetValues( "KEY1", out values )
          .Should()
          .BeTrue();

    values.Should()
          .BeEquivalentTo( ["Value2"] );
  }

  [Theory]
  [InlineData( 0 )]
  [InlineData( 2 )]
  public void Ctor_WithCapacity_ShouldSucceed_WhenCapacityIsNotNegative(
    int capacity )
  {
    var act = () => new MutableHashSetLookup<string, string>( capacity );

    act.Should()
       .NotThrow();
  }

  [Fact]
  public void Ctor_WithCapacity_ShouldThrow_WhenCapacityIsNegative()
  {
    var act = () => new MutableHashSetLookup<string, string>( -2 );

    act.Should()
       .Throw<ArgumentOutOfRangeException>();
  }

  [Theory]
  [InlineData( 0 )]
  [InlineData( 2 )]
  public void Ctor_WithCapacityAndComparer_ShouldSucceed_WhenCapacityIsNotNegative(
    int capacity )
  {
    var act = () => new MutableHashSetLookup<string, string>( capacity, StringComparer.OrdinalIgnoreCase );

    act.Should()
       .NotThrow();
  }

  [Fact]
  public void Ctor_WithCapacityAndComparer_ShouldThrow_WhenCapacityIsNegative()
  {
    var act = () => new MutableHashSetLookup<string, string>( -2, StringComparer.OrdinalIgnoreCase );

    act.Should()
       .Throw<ArgumentOutOfRangeException>();
  }

  [Fact]
  public void Ctor_WithCapacityAndComparer_ShouldUseComparer_WhenAddingValues()
  {
    var lookup = new MutableHashSetLookup<string, string>( 2, StringComparer.OrdinalIgnoreCase );

    lookup.Add( "Key1", "Value1" )
          .Should()
          .BeTrue();

    lookup.Count.Should()
          .Be( 1 );

    lookup.Add( "KEY1", "Value2" )
          .Should()
          .BeTrue();

    lookup.Count.Should()
          .Be( 1 );

    lookup.TryGetValues( "Key1", out var values )
          .Should()
          .BeTrue();

    values.Should()
          .BeEquivalentTo( ["Value1", "Value2"] );

    lookup.TryGetValues( "KEY1", out values )
          .Should()
          .BeTrue();

    values.Should()
          .BeEquivalentTo( ["Value1", "Value2"] );
  }

  [Fact]
  public void Ctor_WithCapacityKeyAndValueComparers_ShouldUseComparers_WhenAddingValues()
  {
    var lookup = new MutableHashSetLookup<string, string>(
      2,
      StringComparer.OrdinalIgnoreCase,
      StringComparer.OrdinalIgnoreCase
    );

    lookup.Add( "Key1", "Value1" )
          .Should()
          .BeTrue();

    lookup.Count.Should()
          .Be( 1 );

    lookup.Add( "KEY1", "Value1" )
          .Should()
          .BeFalse();

    lookup.Count.Should()
          .Be( 1 );

    lookup.TryGetValues( "Key1", out var values )
          .Should()
          .BeTrue();

    values.Should()
          .BeEquivalentTo( ["Value1"] );

    lookup.TryGetValues( "KEY1", out values )
          .Should()
          .BeTrue();

    values.Should()
          .BeEquivalentTo( ["Value1"] );
  }

  [Fact]
  public void Ctor_WithComparer_ShouldUseComparer_WhenAddingValues()
  {
    var lookup = new MutableHashSetLookup<string, string>( StringComparer.OrdinalIgnoreCase );

    lookup.Add( "Key1", "Value1" )
          .Should()
          .BeTrue();

    lookup.Count.Should()
          .Be( 1 );

    lookup.Add( "KEY1", "VALUE1" )
          .Should()
          .BeTrue();

    lookup.Count.Should()
          .Be( 1 );

    lookup.TryGetValues( "Key1", out var values )
          .Should()
          .BeTrue();

    values.Should()
          .BeEquivalentTo( ["Value1", "VALUE1"] );

    lookup.TryGetValues( "KEY1", out values )
          .Should()
          .BeTrue();

    values.Should()
          .BeEquivalentTo( ["Value1", "VALUE1"] );
  }

  [Fact]
  public void Ctor_WithKeyAndValueComparers_ShouldUseComparers_WhenAddingValues()
  {
    var lookup = new MutableHashSetLookup<string, string>(
      StringComparer.OrdinalIgnoreCase,
      StringComparer.OrdinalIgnoreCase
    );

    lookup.Add( "Key1", "Value1" )
          .Should()
          .BeTrue();

    lookup.Count.Should()
          .Be( 1 );

    lookup.Add( "KEY1", "Value1" )
          .Should()
          .BeFalse();

    lookup.Count.Should()
          .Be( 1 );

    lookup.TryGetValues( "Key1", out var values )
          .Should()
          .BeTrue();

    values.Should()
          .BeEquivalentTo( ["Value1"] );

    lookup.TryGetValues( "KEY1", out values )
          .Should()
          .BeTrue();

    values.Should()
          .BeEquivalentTo( ["Value1"] );
  }

  [Fact]
  public void GetEnumerator_ShouldReturnAllGroupings()
  {
    var lookup = new MutableHashSetLookup<int, string>
    {
      { 1, "Value1" },
      { 1, "Value2" },
      { 2, "Value3" }
    };

    using var enumerator = lookup.GetEnumerator();
    enumerator.Should()
              .NotBeNull();

    var keys = new List<int>();
    var values = new List<string>();

    while( enumerator.MoveNext() )
    {
      keys.Add( enumerator.Current.Key );
      values.AddRange( enumerator.Current );
    }

    keys.Should()
        .BeEquivalentTo( [1, 2] );

    values.Should()
          .BeEquivalentTo( ["Value1", "Value2", "Value3"] );
  }

  [Fact]
  public void IEnumerableGetEnumerator_ShouldReturnAllGroupings()
  {
    var lookup = new MutableHashSetLookup<int, string>
    {
      { 1, "Value1" },
      { 1, "Value2" },
      { 2, "Value3" }
    };

    var enumerator = ( ( IEnumerable ) lookup ).GetEnumerator();
    enumerator.Should()
              .NotBeNull();

    var keys = new List<int>();
    var values = new List<string>();

    while( enumerator.MoveNext() )
    {
      var grouping = ( IGrouping<int, string>? ) enumerator.Current;

      grouping.Should()
              .NotBeNull();

      keys.Add( grouping!.Key );
      values.AddRange( grouping );
    }

    keys.Should()
        .BeEquivalentTo( [1, 2] );

    values.Should()
          .BeEquivalentTo( ["Value1", "Value2", "Value3"] );
  }

  [Fact]
  public void Indexer_ShouldReturnValues_WhenKeyIsFound()
  {
    var lookup = new MutableHashSetLookup<int, string> { { 1, "Value1" } };

    lookup[1]
      .Should()
      .BeEquivalentTo( ["Value1"] );
  }

  [Fact]
  public void Indexer_ShouldReturnValues_WhenKeyIsNotFound()
  {
    var lookup = new MutableHashSetLookup<int, string> { { 1, "Value1" } };

    lookup[2]
      .Should()
      .BeEmpty();
  }

  [Fact]
  public void Remove_WithKey_ShouldReturnFalse_WhenKeyIsNotFound()
  {
    var lookup = new MutableHashSetLookup<int, string> { { 1, "Value1" } };

    lookup.Remove( 2 )
          .Should()
          .BeFalse();

    lookup.Contains( 1, "Value1" )
          .Should()
          .BeTrue();

    lookup.Contains( 1 )
          .Should()
          .BeTrue();
  }

  [Fact]
  public void Remove_WithKey_ShouldReturnTrue_WhenKeyIsFound()
  {
    var lookup = new MutableHashSetLookup<int, string> { { 1, "Value1" } };

    lookup.Remove( 1 )
          .Should()
          .BeTrue();

    lookup.Contains( 1, "Value1" )
          .Should()
          .BeFalse();

    lookup.Contains( 1 )
          .Should()
          .BeFalse();
  }

  [Fact]
  public void Remove_WithKeyAndValue_ShouldReturnFalse_WhenKeyIsNotFound()
  {
    var lookup = new MutableHashSetLookup<int, string> { { 1, "Value1" } };

    lookup.Remove( 2, "Value1" )
          .Should()
          .BeFalse();

    lookup.Contains( 1, "Value1" )
          .Should()
          .BeTrue();
  }

  [Fact]
  public void Remove_WithKeyAndValue_ShouldReturnFalse_WhenValueIsNotFound()
  {
    var lookup = new MutableHashSetLookup<int, string> { { 1, "Value1" } };

    lookup.Remove( 1, "Value3" )
          .Should()
          .BeFalse();

    lookup.Contains( 1, "Value1" )
          .Should()
          .BeTrue();
  }

  [Fact]
  public void Remove_WithKeyAndValue_ShouldReturnTrue_WhenKeyIsFound()
  {
    var lookup = new MutableHashSetLookup<int, string> { { 1, "Value1" } };

    lookup.Remove( 1, "Value1" )
          .Should()
          .BeTrue();

    lookup.Contains( 1, "Value1" )
          .Should()
          .BeFalse();

    lookup.Contains( 1 )
          .Should()
          .BeFalse();
  }

  [Fact]
  public void Try_Add_ShouldReturnFalse_WhenAddingExistentKey()
  {
    var lookup = new MutableHashSetLookup<int, string> { { 1, "Value1" } };

    lookup.TryAdd( 1, "Value1" )
          .Should()
          .BeFalse();

    lookup.Should()
          .HaveCount( 1 );

    lookup.Contains( 1, "Value1" )
          .Should()
          .BeTrue();

    lookup.TryGetValues( 1, out var values )
          .Should()
          .BeTrue();

    values.Should()
          .BeEquivalentTo( ["Value1"] );
  }

  [Fact]
  public void TryAdd_ShouldReturnTrue_WhenAddingNonExistentKey()
  {
    var lookup = new MutableHashSetLookup<int, string>();

    lookup.TryAdd( 1, "Value1" )
          .Should()
          .BeTrue();

    lookup.Should()
          .HaveCount( 1 );

    lookup.Contains( 1, "Value1" )
          .Should()
          .BeTrue();

    lookup.TryGetValues( 1, out var values )
          .Should()
          .BeTrue();

    values.Should()
          .BeEquivalentTo( ["Value1"] );
  }

  [Fact]
  public void TryGetValues_WithKey_ShouldReturnFalse_WhenKeyIsNotFound()
  {
    var lookup = new MutableHashSetLookup<int, string> { { 1, "Value1" } };

    lookup.TryGetValues( 2, out var values )
          .Should()
          .BeFalse();

    values.Should()
          .BeEmpty();
  }

  [Fact]
  public void TryGetValues_WithKey_ShouldReturnTrue_WhenKeyIsFound()
  {
    var lookup = new MutableHashSetLookup<int, string> { { 1, "Value1" } };

    lookup.TryGetValues( 1, out var values )
          .Should()
          .BeTrue();

    values.Should()
          .BeEquivalentTo( ["Value1"] );
  }

  #endregion
}
