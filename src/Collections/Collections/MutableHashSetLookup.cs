// Module Name: MutableHashSetLookup.cs
// Author:      Eduardo Velasquez
// Copyright (c) 2024, Intercode Consulting, Inc.

namespace Intercode.Toolbox.Collections;

/// <summary>
///   Represents a lookup collection that uses a <see cref="HashSet{TValue}" /> as the underlying value container.
/// </summary>
/// <typeparam name="TKey">The type of the keys in the lookup.</typeparam>
/// <typeparam name="TValue">The type of the values in the lookup.</typeparam>
public sealed class MutableHashSetLookup<TKey, TValue>: MutableLookup<TKey, TValue, HashSet<TValue>>
  where TKey: notnull
{
  #region Fields

  private readonly IEqualityComparer<TValue>? _valueComparer;

  #endregion

  #region Constructors

  /// <summary>
  ///   Initializes a new instance of the <see cref="MutableHashSetLookup{TKey,TValue}" /> class.
  /// </summary>
  public MutableHashSetLookup()
  {
  }

  /// <summary>
  ///   Initializes a new instance of the <see cref="MutableHashSetLookup{TKey,TValue}" /> class with the specified comparer.
  /// </summary>
  /// <param name="comparer">
  ///   The <see cref="IEqualityComparer{T}" /> implementation to use when comparing keys, or <c>null</c>
  ///   to use the default <see cref="EqualityComparer{T}" />  for the type of the key.
  /// </param>
  public MutableHashSetLookup(
    IEqualityComparer<TKey>? comparer )
    : base( comparer )
  {
  }

  /// <summary>
  ///   Initializes a new instance of the <see cref="MutableHashSetLookup{TKey,TValue}" /> class with the specified comparer.
  /// </summary>
  /// <param name="keyComparer">
  ///   The <see cref="IEqualityComparer{T}" /> implementation to use when comparing keys, or <c>null</c>
  ///   to use the default <see cref="EqualityComparer{T}" />  for the type of the key.
  /// </param>
  /// <param name="valueComparer">
  ///   The <see cref="IEqualityComparer{T}" /> implementation to use when comparing values, or <c>null</c>
  ///   to use the default <see cref="EqualityComparer{T}" />  for the type of the value.
  /// </param>
  public MutableHashSetLookup(
    IEqualityComparer<TKey>? keyComparer,
    IEqualityComparer<TValue>? valueComparer )
    : base( keyComparer )
  {
    _valueComparer = valueComparer;
  }

  /// <summary>
  ///   Initializes a new instance of the <see cref="MutableHashSetLookup{TKey,TValue}" /> class with the specified capacity.
  /// </summary>
  /// <param name="capacity">The initial number of elements that the lookup can contain.</param>
  public MutableHashSetLookup(
    int capacity )
    : base( capacity )
  {
  }

  /// <summary>
  ///   Initializes a new instance of the <see cref="MutableHashSetLookup{TKey,TValue}" /> class with the specified capacity
  ///   and
  ///   comparer.
  /// </summary>
  /// <param name="capacity">The initial number of elements that the lookup can contain.</param>
  /// <param name="comparer">
  ///   The <see cref="IEqualityComparer{T}" /> implementation to use when comparing keys, or <c>null</c>
  ///   to use the default <see cref="EqualityComparer{T}" />  for the type of the key.
  /// </param>
  public MutableHashSetLookup(
    int capacity,
    IEqualityComparer<TKey>? comparer )
    : base( capacity, comparer )
  {
  }

  /// <summary>
  ///   Initializes a new instance of the <see cref="MutableHashSetLookup{TKey,TValue}" /> class with the specified capacity,
  ///   key and value comparers.
  /// </summary>
  /// <param name="capacity">The initial number of elements that the lookup can contain.</param>
  /// <param name="keyComparer">
  ///   The <see cref="IEqualityComparer{T}" /> implementation to use when comparing keys, or <c>null</c>
  ///   to use the default <see cref="EqualityComparer{T}" />  for the type of the key.
  /// </param>
  /// <param name="valueComparer">
  ///   The <see cref="IEqualityComparer{T}" /> implementation to use when comparing values, or <c>null</c>
  ///   to use the default <see cref="EqualityComparer{T}" />  for the type of the value.
  /// </param>
  public MutableHashSetLookup(
    int capacity,
    IEqualityComparer<TKey>? keyComparer,
    IEqualityComparer<TValue>? valueComparer )
    : base( capacity, keyComparer )
  {
    _valueComparer = valueComparer;
  }

  #endregion

  #region Implementation

  /// <inheritdoc />
  protected override HashSet<TValue> CreateContainer()
  {
    return new HashSet<TValue>( _valueComparer );
  }

  /// <inheritdoc />
  protected override bool AddValue(
    HashSet<TValue> container,
    TValue value )
  {
    return container.Add( value );
  }

  /// <inheritdoc />
  protected override void AddValues(
    HashSet<TValue> container,
    IEnumerable<TValue> values )
  {
    foreach( var value in values )
    {
      container.Add( value );
    }
  }

  /// <inheritdoc />
  protected override void TrimExcess(
    HashSet<TValue> container )
  {
    container.TrimExcess();
  }

  #endregion
}
