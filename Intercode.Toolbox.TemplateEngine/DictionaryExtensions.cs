// Module Name: DictionaryExtensions.cs
// Author:      Eduardo Velasquez
// Copyright (c) 2025, Intercode Consulting, Inc.

namespace Intercode.Toolbox.TemplateEngine;

using System.Runtime.InteropServices;

/// <summary>
///   Provides extension methods for <see cref="Dictionary{TKey, TValue}" /> to simplify value retrieval and creation.
/// </summary>
internal static class DictionaryExtensions
{
  #region Public Methods

  /// <summary>
  ///   Gets the value associated with the specified <paramref name="key" /> or adds a new value created by
  ///   <paramref name="valueFactory" /> if the key does not exist.
  /// </summary>
  /// <typeparam name="TKey">The type of the dictionary key. Must not be <c>null</c>.</typeparam>
  /// <typeparam name="TValue">The type of the dictionary value.</typeparam>
  /// <param name="dictionary">The dictionary to operate on.</param>
  /// <param name="key">The key whose value to get or add.</param>
  /// <param name="valueFactory">A function to generate a value for the key if it does not exist.</param>
  /// <returns>The existing value for the key, or the newly created value if the key was not present.</returns>
  public static TValue GetOrAdd<TKey, TValue>(
    this Dictionary<TKey, TValue> dictionary,
    TKey key,
    Func<TKey, TValue> valueFactory )
    where TKey: notnull
  {
#if NET8_0_OR_GREATER
    ref var valueRef =
      ref CollectionsMarshal.GetValueRefOrAddDefault( dictionary, key, out var exists );

    if( exists )
    {
      return valueRef!;
    }

    var newValue = valueFactory( key ); // invoked only when missing
    valueRef = newValue; // assign the added default slot
    return newValue;
#else
    if( dictionary.TryGetValue( key, out var existing ) )
    {
      return existing!;
    }

    var newValue = valueFactory( key );
    dictionary[key] = newValue;
    return newValue;
#endif
  }

#if NET9_0_OR_GREATER
  public static TValue GetOrAdd<TKey, TValue, TAltKey>(
    this Dictionary<TKey, TValue>.AlternateLookup<TAltKey> altLookup,
    TAltKey key,
    Func<TAltKey, TValue> valueFactory )
    where TKey: notnull
    where TAltKey: notnull, allows ref struct
  {
    ref var valueRef =
      ref CollectionsMarshal.GetValueRefOrAddDefault( altLookup, key, out var exists );

    if( exists )
    {
      return valueRef!;
    }

    var newValue = valueFactory( key );
    altLookup[key] = newValue;

    return newValue;
  }
#endif

  #endregion
}
