// Module Name: IMutableLookup.cs
// Author:      Eduardo Velasquez
// Copyright (c) 2024, Intercode Consulting, Inc.

namespace Intercode.Toolbox.Collections;

/// <summary>
///   Represents a mutable lookup collection that maps keys to multiple values.
/// </summary>
/// <typeparam name="TKey">The type of the keys in the lookup.</typeparam>
/// <typeparam name="TValue">The type of the values in the lookup.</typeparam>
public interface IMutableLookup<TKey, TValue>: ILookup<TKey, TValue>
{
  /// <summary>
  ///   Adds a key-value pair to the lookup.
  /// </summary>
  /// <param name="key">The key to add.</param>
  /// <param name="value">The value to add.</param>
  /// <returns><c>true</c> if the key and its associated value was successfully added; otherwise, <c>false</c>.</returns>
  bool Add(
    TKey key,
    TValue value );

  /// <summary>
  ///   Adds a key and a collection of values to the lookup.
  /// </summary>
  /// <param name="key">The key to add.</param>
  /// <param name="values">The collection of values to add.</param>
  void Add(
    TKey key,
    IEnumerable<TValue> values );

  /// <summary>
  ///   Removes a key and all its associated values from the lookup.
  /// </summary>
  /// <param name="key">The key to remove.</param>
  /// <returns><c>true</c> if the key and its associated values are successfully removed; otherwise, <c>false</c>.</returns>
  bool Remove(
    TKey key );

  /// <summary>
  ///   Removes a specific value associated with a key from the lookup.
  /// </summary>
  /// <param name="key">The key to remove the value from.</param>
  /// <param name="value">The value to remove.</param>
  /// <returns><c>true</c> if the value is successfully removed; otherwise, <c>false</c>.</returns>
  bool Remove(
    TKey key,
    TValue value );

  /// <summary>
  ///   Removes all keys and values from the lookup.
  /// </summary>
  void Clear();

  /// <summary>
  ///   Tries to get the values associated with a key in the lookup.
  /// </summary>
  /// <param name="key">The key to get the values for.</param>
  /// <param name="values">
  ///   When this method returns, contains the values associated with the specified key, if the key is
  ///   found; otherwise, an empty collection.
  /// </param>
  /// <returns><c>true</c> if the key is found; otherwise, <c>false</c>.</returns>
  bool TryGetValues(
    TKey key,
    out IEnumerable<TValue> values );

  /// <summary>
  ///   Determines whether the lookup contains a specific key-value pair.
  /// </summary>
  /// <param name="key">The key to check.</param>
  /// <param name="value">The value to check.</param>
  /// <returns><c>true</c> if the key-value pair is found; otherwise, <c>false</c>.</returns>
  bool Contains(
    TKey key,
    TValue value );
}
