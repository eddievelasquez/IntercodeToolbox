// Module Name: MutableListLookup.cs
// Author:      Eduardo Velasquez
// Copyright (c) 2024, Intercode Consulting, Inc.

namespace Intercode.Toolbox.Collections;

/// <summary>
///   Represents a lookup collection that uses a <see cref="List{TValue}" /> as the container for values.
/// </summary>
/// <typeparam name="TKey">The type of the keys in the lookup.</typeparam>
/// <typeparam name="TValue">The type of the values in the lookup.</typeparam>
public sealed class MutableListLookup<TKey, TValue>: MutableLookup<TKey, TValue, List<TValue>>
  where TKey: notnull
{
  #region Constructors

  /// <summary>
  ///   Initializes a new instance of the <see cref="MutableListLookup{TKey,TValue}" /> class.
  /// </summary>
  public MutableListLookup()
  {
  }

  /// <summary>
  ///   Initializes a new instance of the <see cref="MutableListLookup{TKey,TValue}" /> class with the specified comparer.
  /// </summary>
  /// <param name="comparer">
  ///   The <see cref="IEqualityComparer{T}" /> implementation to use when comparing keys, or <c>null</c>
  ///   to use the default <see cref="EqualityComparer{T}" />  for the type of the key.
  /// </param>
  public MutableListLookup(
    IEqualityComparer<TKey>? comparer )
    : base( comparer )
  {
  }

  /// <summary>
  ///   Initializes a new instance of the <see cref="MutableListLookup{TKey,TValue}" /> class with the specified capacity.
  /// </summary>
  /// <param name="capacity">The initial capacity of the lookup.</param>
  public MutableListLookup(
    int capacity )
    : base( capacity )
  {
  }

  /// <summary>
  ///   Initializes a new instance of the <see cref="MutableListLookup{TKey,TValue}" /> class with the specified capacity and
  ///   comparer.
  /// </summary>
  /// <param name="capacity">The initial capacity of the lookup.</param>
  /// <param name="comparer">
  ///   The <see cref="IEqualityComparer{T}" /> implementation to use when comparing keys, or <c>null</c>
  ///   to use the default <see cref="EqualityComparer{T}" />  for the type of the key.
  /// </param>
  public MutableListLookup(
    int capacity,
    IEqualityComparer<TKey>? comparer )
    : base( capacity, comparer )
  {
  }

  #endregion

  #region Implementation

  /// <inheritdoc />
  protected override List<TValue> CreateContainer()
  {
    return [];
  }

  /// <inheritdoc />
  protected override bool AddValue(
    List<TValue> container,
    TValue value )
  {
    container.Add( value );
    return true;
  }

  /// <inheritdoc />
  protected override void AddValues(
    List<TValue> container,
    IEnumerable<TValue> values )
  {
    container.AddRange( values );
  }

  /// <inheritdoc />
  protected override void TrimExcess(
    List<TValue> container )
  {
    container.TrimExcess();
  }

  #endregion
}
