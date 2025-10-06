// Module Name: MutableLookupExtensions.cs
// Author:      Eduardo Velasquez
// Copyright (c) 2024, Intercode Consulting, Inc.

namespace Intercode.Toolbox.Collections;

/// <summary>
///   Provides extension methods for creating mutable lookups from <see cref="IEnumerable{T}" />>.
/// </summary>
public static class MutableLookupExtensions
{
  #region Public Methods

  /// <summary>
  ///   Converts the source collection into a <see cref="MutableListLookup{TKey,TValue}" /> using the specified key selector.
  /// </summary>
  /// <typeparam name="TKey">The type of the key.</typeparam>
  /// <typeparam name="TValue">The type of the value.</typeparam>
  /// <param name="source">The source collection.</param>
  /// <param name="keySelector">The key selector function.</param>
  /// <param name="keyComparer">
  ///   The <see cref="IEqualityComparer{T}" /> implementation to use when comparing keys, or <c>null</c>
  ///   to use the default <see cref="EqualityComparer{T}" /> for the type of the key.
  /// </param>
  /// <returns>
  ///   A <see cref="MutableListLookup{TKey,TValue}" /> containing the elements of the source collection grouped by keys.
  /// </returns>
  /// <exception cref="ArgumentNullException">
  ///   Thrown when <paramref name="source" /> or <paramref name="keySelector" /> are null.
  /// </exception>
  public static MutableListLookup<TKey, TValue> ToMutableListLookup<TKey, TValue>(
    this IEnumerable<TValue> source,
    Func<TValue, TKey> keySelector,
    IEqualityComparer<TKey>? keyComparer = null )
    where TKey: notnull
  {
    ArgumentNullException.ThrowIfNull( source );
    ArgumentNullException.ThrowIfNull( keySelector );

    return new MutableListLookup<TKey, TValue>( keyComparer ).FillLookup( source, keySelector );
  }

  /// <summary>
  ///   Converts the source collection into a <see cref="MutableListLookup{TKey,TValue}" /> using the specified key and value
  ///   selectors.
  /// </summary>
  /// <typeparam name="TSource">The type of the source elements.</typeparam>
  /// <typeparam name="TKey">The type of the key.</typeparam>
  /// <typeparam name="TValue">The type of the value.</typeparam>
  /// <param name="source">The source collection.</param>
  /// <param name="keySelector">The key selector function.</param>
  /// <param name="valueSelector">The value selector function.</param>
  /// <param name="keyComparer">
  ///   The <see cref="IEqualityComparer{T}" /> implementation to use when comparing keys, or <c>null</c>
  ///   to use the default <see cref="EqualityComparer{T}" /> for the type of the key.
  /// </param>
  /// <returns>
  ///   A <see cref="MutableListLookup{TKey,TValue}" /> containing the elements of the source collection grouped by keys.
  /// </returns>
  /// <exception cref="ArgumentNullException">
  ///   Thrown when <paramref name="source" />, <paramref name="keySelector" />, or <paramref name="valueSelector" /> are
  ///   null.
  /// </exception>
  public static MutableListLookup<TKey, TValue> ToMutableListLookup<TSource, TKey, TValue>(
    this IEnumerable<TSource> source,
    Func<TSource, TKey> keySelector,
    Func<TSource, TValue> valueSelector,
    IEqualityComparer<TKey>? keyComparer = null )
    where TKey: notnull
  {
    ArgumentNullException.ThrowIfNull( source );
    ArgumentNullException.ThrowIfNull( keySelector );
    ArgumentNullException.ThrowIfNull( valueSelector );

    return new MutableListLookup<TKey, TValue>( keyComparer ).FillLookup( source, keySelector, valueSelector );
  }

  /// <summary>
  ///   Converts the source collection into a <see cref="MutableHashSetLookup{TKey,TValue}" /> using the specified key
  ///   selector.
  /// </summary>
  /// <typeparam name="TKey">The type of the key.</typeparam>
  /// <typeparam name="TValue">The type of the value.</typeparam>
  /// <param name="source">The source collection.</param>
  /// <param name="keySelector">The key selector function.</param>
  /// <param name="keyComparer">
  ///   The <see cref="IEqualityComparer{T}" /> implementation to use when comparing keys, or <c>null</c>
  ///   to use the default <see cref="EqualityComparer{T}" /> for the type of the key.
  /// </param>
  /// <param name="valueComparer">
  ///   The <see cref="IEqualityComparer{T}" /> implementation to use when comparing values, or <c>null</c>
  ///   to use the default <see cref="EqualityComparer{T}" /> for the type of the value.
  /// </param>
  /// <returns>
  ///   A <see cref="MutableHashSetLookup{TKey,TValue}" /> containing the elements of the source collection grouped by
  ///   keys.
  /// </returns>
  /// <exception cref="ArgumentNullException">
  ///   Thrown when <paramref name="source" /> or <paramref name="keySelector" /> are null.
  /// </exception>
  public static MutableHashSetLookup<TKey, TValue> ToMutableHashSetLookup<TKey, TValue>(
    this IEnumerable<TValue> source,
    Func<TValue, TKey> keySelector,
    IEqualityComparer<TKey>? keyComparer = null,
    IEqualityComparer<TValue>? valueComparer = null )
    where TKey: notnull
  {
    ArgumentNullException.ThrowIfNull( source );
    ArgumentNullException.ThrowIfNull( keySelector );

    return new MutableHashSetLookup<TKey, TValue>( keyComparer, valueComparer ).FillLookup( source, keySelector );
  }

  /// <summary>
  ///   Converts the source collection into a <see cref="MutableHashSetLookup{TKey,TValue}" /> using the specified key and
  ///   value
  ///   selectors.
  /// </summary>
  /// <typeparam name="TSource">The type of the source elements.</typeparam>
  /// <typeparam name="TKey">The type of the key.</typeparam>
  /// <typeparam name="TValue">The type of the value.</typeparam>
  /// <param name="source">The source collection.</param>
  /// <param name="keySelector">The key selector function.</param>
  /// <param name="valueSelector">The value selector function.</param>
  /// <param name="keyComparer">
  ///   The <see cref="IEqualityComparer{T}" /> implementation to use when comparing keys, or <c>null</c>
  ///   to use the default <see cref="EqualityComparer{T}" /> for the type of the key.
  /// </param>
  /// <param name="valueComparer">
  ///   The <see cref="IEqualityComparer{T}" /> implementation to use when comparing values, or <c>null</c>
  ///   to use the default <see cref="EqualityComparer{T}" /> for the type of the value.
  /// </param>
  /// <returns>
  ///   A <see cref="MutableHashSetLookup{TKey,TValue}" /> containing the elements of the source collection grouped by keys.
  /// </returns>
  /// <exception cref="ArgumentNullException">
  ///   Thrown when <paramref name="source" />, <paramref name="keySelector" />, or <paramref name="valueSelector" /> are
  ///   null.
  /// </exception>
  public static MutableHashSetLookup<TKey, TValue> ToMutableHashSetLookup<TSource, TKey, TValue>(
    this IEnumerable<TSource> source,
    Func<TSource, TKey> keySelector,
    Func<TSource, TValue> valueSelector,
    IEqualityComparer<TKey>? keyComparer = null,
    IEqualityComparer<TValue>? valueComparer = null )
    where TKey: notnull
  {
    ArgumentNullException.ThrowIfNull( source );
    ArgumentNullException.ThrowIfNull( keySelector );
    ArgumentNullException.ThrowIfNull( valueSelector );

    return new MutableHashSetLookup<TKey, TValue>( keyComparer, valueComparer ).FillLookup(
      source,
      keySelector,
      valueSelector
    );
  }

  #endregion

  #region Implementation

  private static TLookup FillLookup<TLookup, TKey, TValue>(
    this TLookup lookup,
    IEnumerable<TValue> source,
    Func<TValue, TKey> keySelector )
    where TLookup: IMutableLookup<TKey, TValue>
  {
    foreach( var value in source )
    {
      lookup.Add( keySelector( value ), value );
    }

    return lookup;
  }

  private static TLookup FillLookup<TLookup, TSource, TKey, TValue>(
    this TLookup lookup,
    IEnumerable<TSource> source,
    Func<TSource, TKey> keySelector,
    Func<TSource, TValue> valueSelector )
    where TLookup: IMutableLookup<TKey, TValue>
  {
    foreach( var value in source )
    {
      lookup.Add( keySelector( value ), valueSelector( value ) );
    }

    return lookup;
  }

  #endregion
}
