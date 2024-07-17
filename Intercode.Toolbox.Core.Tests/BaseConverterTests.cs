// Module Name: BaseConverterTests.cs
// Author:      Eduardo Velasquez
// Copyright (c) 2024, Intercode Consulting, Inc.

namespace Intercode.Toolbox.Core.Tests;

using System.Numerics;
using FluentAssertions;

public class BaseConverterTests
{
  #region Tests

  [Fact]
  public void ConvertFromString_WithLong_ShouldConvert()
  {
    BaseConverter.ConvertFromString<long>( "10", 10 )
                 .Should()
                 .Be( 10L );
  }

  [Fact]
  public void ConvertFromString_WithLong_ShouldReturnZero_WhenValueIsEmptyString()
  {
    BaseConverter.ConvertFromString<long>( "", 10 )
                 .Should()
                 .Be( 0L );
  }

  [Fact]
  public void ConvertFromString_WithLong_ShouldReturnZero_WhenValueIsNullString()
  {
    BaseConverter.ConvertFromString<long>( null, 10 )
                 .Should()
                 .Be( 0L );
  }

  [Fact]
  public void ConvertFromString_WithLong_ShouldThrow_WhenRadixIsGreaterThanMaximum()
  {
    var act = () => BaseConverter.ConvertFromString<long>( "10", BaseConverter.MaxRadix + 1 );

    act.Should()
       .Throw<ArgumentOutOfRangeException>()
       .Where(
         ex => ex.Message.StartsWith( $"The radix must be between {BaseConverter.MinRadix} and {BaseConverter.MaxRadix}" )
       );
  }

  [Fact]
  public void ConvertFromString_WithLong_ShouldThrow_WhenRadixIsLessThanMinimum()
  {
    var act = () => BaseConverter.ConvertFromString<long>( "10", 1 );

    act.Should()
       .Throw<ArgumentOutOfRangeException>()
       .Where(
         ex => ex.Message.StartsWith( $"The radix must be between {BaseConverter.MinRadix} and {BaseConverter.MaxRadix}" )
       );
  }

  [Fact]
  public void ConvertFromString_WithLong_ShouldThrow_WhenValueContainsInvalidCharacters()
  {
    var act = () => BaseConverter.ConvertFromString<long>( "10$", 10 );

    act.Should()
       .Throw<FormatException>()
       .WithMessage( "Invalid character '$'" );
  }

  [Fact]
  public void ConvertToString_WithLong_ShouldThrow_WhenRadixIsGreaterThanMaximum()
  {
    var act = () => BaseConverter.ConvertToString<long>( 0L, BaseConverter.MaxRadix + 1 );

    act.Should()
       .Throw<ArgumentOutOfRangeException>()
       .Where(
         ex => ex.Message.StartsWith( $"The radix must be between {BaseConverter.MinRadix} and {BaseConverter.MaxRadix}" )
       );
  }

  [Fact]
  public void ConvertToString_WithLong_ShouldThrow_WhenRadixIsLessThanMinimum()
  {
    var act = () => BaseConverter.ConvertToString<long>( 0L, 1 );

    act.Should()
       .Throw<ArgumentOutOfRangeException>()
       .Where(
         ex => ex.Message.StartsWith( $"The radix must be between {BaseConverter.MinRadix} and {BaseConverter.MaxRadix}" )
       );
  }

  [Fact]
  public void TryConvertFromString_WithLong_ShouldConvertValue()
  {
    BaseConverter.TryConvertFromString( "1", 10, out long result )
                 .Should()
                 .BeTrue();

    result.Should()
          .Be( 1L );
  }

  [Fact]
  public void TryConvertFromString_WithLong_ShouldPropagateNegativeSign()
  {
    BaseConverter.TryConvertFromString( "-1", 10, out long result )
                 .Should()
                 .BeTrue();

    result.Should()
          .Be( -1L );
  }

  [Fact]
  public void TryConvertFromString_WithLong_ShouldReturnFalse_WhenRadixIsGreaterThanMaximum()
  {
    BaseConverter.TryConvertFromString( "1", BaseConverter.MaxRadix + 1, out long _ )
                 .Should()
                 .BeFalse();
  }

  [Fact]
  public void TryConvertFromString_WithLong_ShouldReturnFalse_WhenRadixIsLessThanMinimum()
  {
    BaseConverter.TryConvertFromString( "1", 1, out long _ )
                 .Should()
                 .BeFalse();
  }

  [Fact]
  public void TryConvertFromString_WithLong_ShouldReturnFalse_WhenValueContainsInvalidCharacters()
  {
    BaseConverter.TryConvertFromString( "1$", 1, out long _ )
                 .Should()
                 .BeFalse();
  }

  [Fact]
  public void TryConvertFromString_WithLong_ShouldReturnZero_WhenValueIsEmpty()
  {
    BaseConverter.TryConvertFromString( "", 10, out long result )
                 .Should()
                 .BeTrue();

    result.Should()
          .Be( 0L );
  }

  [Fact]
  public void TryConvertFromString_WithLong_ShouldReturnZero_WhenValueIsNull()
  {
    BaseConverter.TryConvertFromString( null, 10, out long result )
                 .Should()
                 .BeTrue();

    result.Should()
          .Be( 0L );
  }

  #endregion

  #region Implementation

  private class BaseConverterData<T>: TheoryData<long, int>
  {
    #region Constructors

    public BaseConverterData()
    {
      for( var radix = BaseConverter.MinRadix; radix <= BaseConverter.MaxRadix; radix++ )
      {
        Add( long.MinValue + 1, radix );
        Add( -1, radix );
        Add( 0, radix );
        Add( 1, radix );
        Add( long.MaxValue - 1, radix );
      }
    }

    #endregion
  }

  #endregion
}

public abstract class BaseConverterRoundTrip<T>
  where T: IBinaryInteger<T>, IMinMaxValue<T>
{
  #region Tests

  [Theory]
  [MemberData( nameof( Data ) )]
  public void ConvertToFromString_ShouldRoundTrip(
    T value,
    int radix )
  {
    var s = BaseConverter.ConvertToString( value, radix );
    var d = BaseConverter.ConvertFromString<T>( s, radix );

    d.Should()
     .Be( value );
  }

  #endregion

  #region Implementation

  public static IEnumerable<object[]> Data
  {
    get
    {
      var isSigned = T.MinValue < T.Zero;

      for( var radix = BaseConverter.MinRadix; radix <= BaseConverter.MaxRadix; radix++ )
      {
        if( isSigned )
        {
          // Negating (Abs) the minimum value of a twos complement number is invalid,
          // so we just test net next value after it.
          yield return [T.MinValue + T.One, radix];
        }
        else
        {
          yield return [T.MinValue, radix];
        }

        yield return [T.Zero, radix];
        yield return [T.MaxValue, radix];
      }
    }
  }

  #endregion
}

public class BaseConverterIntRoundTrip: BaseConverterRoundTrip<int>
{
}

public class BaseConverterUIntRoundTrip: BaseConverterRoundTrip<uint>
{
}

public class BaseConverterLongRoundTrip: BaseConverterRoundTrip<long>
{
}

public class BaseConverterULongRoundTrip: BaseConverterRoundTrip<ulong>
{
}
