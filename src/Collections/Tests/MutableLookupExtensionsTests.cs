// Module Name: MutableLookupExtensionsTests.cs
// Author:      Eduardo Velasquez
// Copyright (c) 2024, Intercode Consulting, Inc.

namespace Intercode.Toolbox.Collections.Tests;

using FluentAssertions;

public class MutableLookupExtensionsTests
{
  #region Tests

  [Fact]
  public void ToHashSetLookup_WithKeyAndValueSelectors_ReturnsListLookup()
  {
    var source = new[] { new Thing( 1, "apple" ), new Thing( 1, "banana" ), new Thing( 2, "Onion" ) };

    var lookup = source.ToMutableHashSetLookup( thing => thing.Id, thing => thing.Name );

    lookup.Should()
          .NotBeNull()
          .And.BeOfType<MutableHashSetLookup<int, string>>();

    lookup.Should()
          .HaveCount( 2 );

    lookup[2]
      .Should()
      .HaveCount( 1 )
      .And.BeEquivalentTo( ["Onion"] );
  }

  [Fact]
  public void ToHashSetLookup_WithKeySelector_ReturnsListLookup()
  {
    var source = new[] { new Thing( 1, "apple" ), new Thing( 1, "banana" ), new Thing( 2, "Onion" ) };

    var lookup = source.ToMutableHashSetLookup( thing => thing.Id );

    lookup.Should()
          .NotBeNull()
          .And.BeOfType<MutableHashSetLookup<int, Thing>>();

    lookup.Should()
          .HaveCount( 2 );

    lookup[2]
      .Should()
      .HaveCount( 1 )
      .And.BeEquivalentTo( [new Thing( 2, "Onion" )] );
  }

  [Fact]
  public void ToListLookup_WithKeyAndValueSelectors_ReturnsListLookup()
  {
    var source = new[] { new Thing( 1, "apple" ), new Thing( 1, "banana" ), new Thing( 2, "Onion" ) };

    var lookup = source.ToMutableListLookup( thing => thing.Id, thing => thing.Name );

    lookup.Should()
          .NotBeNull()
          .And.BeOfType<MutableListLookup<int, string>>();

    lookup.Should()
          .HaveCount( 2 );

    lookup[2]
      .Should()
      .HaveCount( 1 )
      .And.BeEquivalentTo( ["Onion"] );
  }

  [Fact]
  public void ToListLookup_WithKeySelector_ReturnsListLookup()
  {
    var source = new[] { new Thing( 1, "apple" ), new Thing( 1, "banana" ), new Thing( 2, "Onion" ) };

    var lookup = source.ToMutableListLookup( thing => thing.Id );

    lookup.Should()
          .NotBeNull()
          .And.BeOfType<MutableListLookup<int, Thing>>();

    lookup.Should()
          .HaveCount( 2 );

    lookup[2]
      .Should()
      .HaveCount( 1 )
      .And.BeEquivalentTo( [new Thing( 2, "Onion" )] );
  }

  #endregion

  #region Implementation

  public record Thing(
    int Id,
    string Name );

  #endregion
}
