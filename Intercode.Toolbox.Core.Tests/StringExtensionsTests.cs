// Module Name: StringExtensionsTests.cs
// Author:      Eduardo Velasquez
// Copyright (c) 2024, Intercode Consulting, Inc.

// ReSharper disable StringLiteralTypo

namespace Intercode.Toolbox.Core.Tests;

using FluentAssertions;
using Intercode.Toolbox.Core.Text;

public class StringExtensionsTests
{
  #region Tests

  [Theory]
  [InlineData( "AÀÁÂÃÄÅǺĀĂĄǍẢẠẦẪẨẬẰẮẴẲẶ", 'A' )]
  [InlineData( "aàáâãåǻāăąǎảạầấẫẩậằắẵẳặ", 'a' )]
  [InlineData( "CÇĆĈĊČ", 'C' )]
  [InlineData( "cçćĉċč", 'c' )]
  [InlineData( "EÈÉÊËĒĔĖĘĚẼẺẸỀẾỄỂỆ", 'E' )]
  [InlineData( "eèéêëēĕėęěẽẻẹềếễểệ", 'e' )]
  [InlineData( "IÌÍÎÏĨĪĬǏĮỈỊ", 'I' )]
  [InlineData( "iìíîïĩīĭǐįỉị", 'i' )]
  [InlineData( "NÑŃŅŇ", 'N' )]
  [InlineData( "ñńņň", 'n' )]
  [InlineData( "OÒÓÔÕŌŎǑŐƠỎỌỒỐỖỔỘỜỚỠỞỢ", 'O' )]
  [InlineData( "oòóôõōŏǒőơỏọồốỗổộờớỡởợ", 'o' )]
  [InlineData( "UÙÚÛŨŪŬŮŰŲƯǓǕǗǙǛŨỦỤỪỨỮỬỰ", 'U' )]
  public void RemoveDiacritics_ShouldRemoveDiacritics(
    string actual,
    char expectedChar )
  {
    var expected = new string( expectedChar, actual.Length );
    var result = actual.RemoveDiacritics();

    result.Should()
          .Be( expected );
  }

  [Fact]
  public void RemoveDiacritics_ShouldReturnEmpty_WhenValueIsEmpty()
  {
    var result = "".RemoveDiacritics();

    result.Should()
          .BeEmpty();
  }

  [Fact]
  public void RemoveDiacritics_ShouldReturnNull_WhenValueIsNull()
  {
    var result = StringExtensions.RemoveDiacritics( null );

    result.Should()
          .BeNull();
  }

  #endregion
}
