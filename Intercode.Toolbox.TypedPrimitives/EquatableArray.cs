// Module Name: EquatableArray.cs
// Author:      Eduardo Velasquez
// Copyright (c) 2024, Intercode Consulting, Inc.

namespace Intercode.Toolbox.TypedPrimitives;

using System.Collections;

// <summary>
// A cache-friendly, immutable and equatable array. Just like <see cref="Array{T}"/>
// but with value equality support.
// Loosely based on the ImmutableArray<T> type of the CommunityToolkit.
// </summary
internal readonly struct EquatableArray<T>
  : IEquatable<EquatableArray<T>>,
    IEnumerable<T>
  where T: IEquatable<T>
{
  #region Constants

  public static readonly EquatableArray<T> Empty = new ( [] );

  #endregion

  #region Fields

  private readonly T[]? _array;

  #endregion

  #region Constructors

  public EquatableArray(
    T[]? array )
  {
    _array = array;
  }

  #endregion

  #region Properties

  public int Length => _array?.Length ?? 0;

  #endregion

  #region Public Methods

  /// <inheritdoc />
  public bool Equals(
    EquatableArray<T> other )
  {
    return AsSpan()
      .SequenceEqual( other.AsSpan() );
  }

  /// <inheritdoc />
  public override bool Equals(
    object? obj )
  {
    return obj is EquatableArray<T> other && Equals( other );
  }

  /// <inheritdoc />
  public override int GetHashCode()
  {
    if( _array == null || _array.Length == 0 )
    {
      return 0;
    }

    unchecked
    {
      var hash = 17;

      foreach( var item in _array )
      {
        hash = hash * 31 + ( item?.GetHashCode() ?? 0 );
      }

      return hash;
    }
  }

  public IEnumerator<T> GetEnumerator()
  {
    return ( _array ?? [] )
           .AsEnumerable()
           .GetEnumerator();
  }

  public ReadOnlySpan<T> AsSpan()
  {
    return _array.AsSpan();
  }

  #endregion

  #region Implementation

  IEnumerator IEnumerable.GetEnumerator()
  {
    return GetEnumerator();
  }

  #endregion

  #region Operators

  public static bool operator ==(
    EquatableArray<T> left,
    EquatableArray<T> right )
  {
    return left.Equals( right );
  }

  public static bool operator !=(
    EquatableArray<T> left,
    EquatableArray<T> right )
  {
    return !( left == right );
  }

  #endregion
}
