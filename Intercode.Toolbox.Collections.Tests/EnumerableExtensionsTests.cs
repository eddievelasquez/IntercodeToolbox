// Module Name: EnumerableExtensionsTests.cs
// Author:      Eduardo Velasquez
// Copyright (c) 2024, Intercode Consulting, Inc.

namespace Intercode.Toolbox.Collections.Tests;

using FluentAssertions;

public class EnumerableExtensionsTests
{
  #region Tests

  [Fact]
  public void AsArray_ShouldReturnArrayWithSingleElement_WhenValueIsNotNull()
  {
    "test".AsArray()
          .Should()
          .NotBeNull()
          .And.HaveCount( 1 )
          .And.BeEquivalentTo( ["test"] );
  }

  [Fact]
  public void AsArray_ShouldReturnEmptyArray_WhenValueIsNull()
  {
    ( ( string? ) null ).AsArray()
                        .Should()
                        .NotBeNull()
                        .And.BeEmpty();
  }

  [Fact]
  public void Batch_WithIEnumerable_ShouldReturnBatches_WhenNotEmpty()
  {
    var source = Enumerable.Range( 1, 10 );

    var result = source.Batch( 3 )
                       .ToList();

    result.Should()
          .NotBeNull()
          .And.HaveCount( 4 );

    result[0]
      .Should()
      .NotBeNull()
      .And.HaveCount( 3 )
      .And.BeEquivalentTo( [1, 2, 3] );

    result[1]
      .Should()
      .NotBeNull()
      .And.HaveCount( 3 )
      .And.BeEquivalentTo( [4, 5, 6] );

    result[2]
      .Should()
      .NotBeNull()
      .And.HaveCount( 3 )
      .And.BeEquivalentTo( [7, 8, 9] );

    result[3]
      .Should()
      .NotBeNull()
      .And.HaveCount( 1 )
      .And.BeEquivalentTo( [10] );
  }

  [Fact]
  public void Batch_WithList_ShouldReturnBatches_WhenNotEmpty()
  {
    var source = Enumerable.Range( 1, 10 )
                           .ToList();

    var result = source.Batch( 3 )
                       .ToList();

    result.Should()
          .NotBeNull()
          .And.HaveCount( 4 );

    result[0]
      .Should()
      .NotBeNull()
      .And.HaveCount( 3 )
      .And.BeEquivalentTo( [1, 2, 3] );

    result[1]
      .Should()
      .NotBeNull()
      .And.HaveCount( 3 )
      .And.BeEquivalentTo( [4, 5, 6] );

    result[2]
      .Should()
      .NotBeNull()
      .And.HaveCount( 3 )
      .And.BeEquivalentTo( [7, 8, 9] );

    result[3]
      .Should()
      .NotBeNull()
      .And.HaveCount( 1 )
      .And.BeEquivalentTo( [10] );
  }

  [Fact]
  public void EmptyIfNull_ReturnsEmpty_WhenSourceArrayIsNull()
  {
    string[]? source = null;

    var result = source.EmptyIfNull();
    result.Should()
          .NotBeNull()
          .And.BeOfType<string[]>()
          .And.BeEmpty();
  }

  [Fact]
  public void EmptyIfNull_ReturnsEmpty_WhenSourceDictionaryIsNull()
  {
    Dictionary<int, string>? source = null;

    var result = source.EmptyIfNull();
    result.Should()
          .NotBeNull()
          .And.BeOfType<Dictionary<int, string>>()
          .And.BeEmpty();
  }

  [Fact]
  public void EmptyIfNull_ReturnsEmpty_WhenSourceHashSetIsNull()
  {
    HashSet<string>? source = null;

    var result = source.EmptyIfNull();
    result.Should()
          .NotBeNull()
          .And.BeOfType<HashSet<string>>()
          .And.BeEmpty();
  }

  [Fact]
  public void EmptyIfNull_ReturnsEmpty_WhenSourceIEnumerableIsNull()
  {
    IEnumerable<string>? source = null;

    var result = source.EmptyIfNull();
    result.Should()
          .NotBeNull()
          .And.BeOfType<string[]>()
          .And.BeEmpty();
  }

  [Fact]
  public void EmptyIfNull_ReturnsEmpty_WhenSourceListIsNull()
  {
    List<string>? source = null;

    var result = source.EmptyIfNull();
    result.Should()
          .NotBeNull()
          .And.BeOfType<List<string>>()
          .And.BeEmpty();
  }

  #endregion
}
