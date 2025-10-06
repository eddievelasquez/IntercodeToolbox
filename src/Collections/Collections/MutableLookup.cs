// Module Name: MutableLookup.cs
// Author:      Eduardo Velasquez
// Copyright (c) 2024, Intercode Consulting, Inc.

namespace Intercode.Toolbox.Collections;

using System.Collections;

/// <summary>
///   Represents a mutable lookup collection that maps keys to multiple values.
///   The collection is not thread-safe.
/// </summary>
/// <typeparam name="TKey">The type of the keys in the lookup.</typeparam>
/// <typeparam name="TValue">The type of the values in the lookup.</typeparam>
/// <typeparam name="TContainer">The type of the container that holds the values for each key.</typeparam>
public abstract class MutableLookup<TKey, TValue, TContainer>: IMutableLookup<TKey, TValue>
  where TKey: notnull
  where TContainer: ICollection<TValue>
{
  #region Fields

  private readonly Dictionary<TKey, TContainer> _dictionary;

  #endregion

  #region Constructors

  /// <summary>
  ///   Initializes a new instance of the <see cref="MutableLookup{TKey, TValue, TContainer}" /> class.
  /// </summary>
  protected MutableLookup()
  {
    _dictionary = new Dictionary<TKey, TContainer>();
  }

  /// <summary>
  ///   Initializes a new instance of the <see cref="MutableLookup{TKey, TValue, TContainer}" /> class with the specified
  ///   comparer.
  /// </summary>
  /// <param name="comparer">
  ///   The <see cref="IEqualityComparer{T}" /> implementation to use when comparing keys, or <c>null</c>
  ///   to use the default <see cref="EqualityComparer{T}" />  for the type of the key.
  /// </param>
  protected MutableLookup(
    IEqualityComparer<TKey>? comparer )
  {
    _dictionary = new Dictionary<TKey, TContainer>( comparer );
  }

  /// <summary>
  ///   Initializes a new instance of the <see cref="MutableLookup{TKey, TValue, TContainer}" /> class with the specified
  ///   capacity.
  /// </summary>
  /// <param name="capacity">The initial number of elements that the lookup can contain.</param>
  protected MutableLookup(
    int capacity )
  {
    _dictionary = new Dictionary<TKey, TContainer>( capacity );
  }

  /// <summary>
  ///   Initializes a new instance of the <see cref="MutableLookup{TKey, TValue, TContainer}" /> class with the specified
  ///   capacity and comparer.
  /// </summary>
  /// <param name="capacity">The initial number of elements that the lookup can contain.</param>
  /// <param name="comparer">
  ///   The <see cref="IEqualityComparer{T}" /> implementation to use when comparing keys, or <c>null</c>
  ///   to use the default <see cref="EqualityComparer{T}" />  for the type of the key.
  /// </param>
  protected MutableLookup(
    int capacity,
    IEqualityComparer<TKey>? comparer )
  {
    _dictionary = new Dictionary<TKey, TContainer>( capacity, comparer );
  }

  #endregion

  #region Properties

  /// <summary>
  ///   Gets the number of groupings in the lookup.
  /// </summary>
  public int Count => _dictionary.Count;

  /// <summary>
  ///   Gets the collection of values associated with the specified key.
  /// </summary>
  /// <param name="key">The key of the values to get.</param>
  /// <returns>An enumerable collection of values associated with the specified key.</returns>
  public IEnumerable<TValue> this[
    TKey key ] =>
    _dictionary.TryGetValue( key, out var values ) ? values : Enumerable.Empty<TValue>();

  #endregion

  #region Public Methods

  /// <summary>
  ///   Adds a key-value pair to the lookup.
  /// </summary>
  /// <param name="key">The key of the value to add.</param>
  /// <param name="value">The value to add.</param>
  public bool Add(
    TKey key,
    TValue value )
  {
    if( !_dictionary.TryGetValue( key, out var container ) )
    {
      container = CreateContainer();
      _dictionary.Add( key, container );
    }

    if( !AddValue( container, value ) )
    {
      return false;
    }

    return true;
  }

  /// <summary>
  ///   Attempts to add the specified key and value to the lookup.
  /// </summary>
  /// <param name="key">The key of the value to add.</param>
  /// <param name="value">The value to add.</param>
  /// <returns><c>true</c> if the key was not found in the lookup and was added successfully; otherwise, <c>false</c>.</returns>
  public bool TryAdd(
    TKey key,
    TValue value )
  {
    if( _dictionary.ContainsKey( key ) )
    {
      return false;
    }

    var container = CreateContainer();
    AddValue( container, value );
    _dictionary.Add( key, container );
    return true;
  }

  /// <summary>
  ///   Adds a range of values to the lookup associated with the specified key.
  /// </summary>
  /// <param name="key">The key of the values to add.</param>
  /// <param name="values">The values to add.</param>
  public void Add(
    TKey key,
    IEnumerable<TValue> values )
  {
    if( !_dictionary.TryGetValue( key, out var container ) )
    {
      container = CreateContainer();
      _dictionary.Add( key, container );
    }

    AddValues( container, values );
  }

  /// <summary>
  ///   Removes the specified key and its associated values from the lookup.
  /// </summary>
  /// <param name="key">The key of the values to remove.</param>
  /// <returns><c>true</c> if the key and its associated values are successfully removed; otherwise, <c>false</c>.</returns>
  public bool Remove(
    TKey key )
  {
    return _dictionary.Remove( key );
  }

  /// <summary>
  ///   Removes a specific value from the lookup associated with the specified key.
  /// </summary>
  /// <param name="key">The key of the value to remove.</param>
  /// <param name="value">The value to remove.</param>
  /// <returns><c>true</c> if the value is successfully removed; otherwise, <c>false</c>.</returns>
  public bool Remove(
    TKey key,
    TValue value )
  {
    if( !_dictionary.TryGetValue( key, out var container ) )
    {
      return false;
    }

    var removed = container.Remove( value );
    if( container.Count == 0 )
    {
      _dictionary.Remove( key );
    }

    return removed;
  }

  /// <summary>
  ///   Removes all keys and values from the lookup.
  /// </summary>
  public void Clear()
  {
    _dictionary.Clear();
  }

  /// <summary>
  ///   Tries to get the collection of values associated with the specified key.
  /// </summary>
  /// <param name="key">The key of the values to get.</param>
  /// <param name="values">
  ///   When this method returns, contains the collection of values associated with the specified key, if
  ///   the key is found; otherwise, an empty collection.
  /// </param>
  /// <returns><c>true</c> if the lookup contains the specified key; otherwise, <c>false</c>.</returns>
  public bool TryGetValues(
    TKey key,
    out IEnumerable<TValue> values )
  {
    if( _dictionary.TryGetValue( key, out var container ) )
    {
      values = container;
      return true;
    }

    values = [];
    return false;
  }

  /// <summary>
  ///   Determines whether the lookup contains a specific key-value pair.
  /// </summary>
  /// <param name="key">The key to check.</param>
  /// <param name="value">The value to check.</param>
  /// <returns><c>true</c> if the key-value pair is found; otherwise, <c>false</c>.</returns>
  public bool Contains(
    TKey key,
    TValue value )
  {
    return _dictionary.TryGetValue( key, out var container ) && container.Contains( value );
  }

  /// <summary>
  ///   Ensures that the lookup has the specified capacity.
  /// </summary>
  /// <param name="capacity">The minimum number of elements that the lookup can contain.</param>
  /// <returns>The actual capacity of the lookup after ensuring the specified capacity.</returns>
  public int EnsureCapacity(
    int capacity )
  {
    return _dictionary.EnsureCapacity( capacity );
  }

  /// <summary>
  ///   Sets the capacity of this lookup to what it would be if it had been originally initialized with all its entries.
  /// </summary>
  /// <remarks>
  ///   This method can be used to minimize the memory overhead once it is known that no new elements will be added.
  /// </remarks>
  public void TrimExcess()
  {
    _dictionary.TrimExcess();

    foreach( var container in _dictionary.Values )
    {
      TrimExcess( container );
    }
  }

  /// <summary>
  ///   Returns an enumerator that iterates through the lookup.
  /// </summary>
  /// <returns>An enumerator that can be used to iterate through the lookup.</returns>
  public IEnumerator<IGrouping<TKey, TValue>> GetEnumerator()
  {
    return _dictionary.SelectMany( pair => pair.Value.Select( value => ( Key: pair.Key, Value: value ) ) )
                      .GroupBy( p => p.Key, p => p.Value )
                      .GetEnumerator();
  }

  /// <summary>
  ///   Determines whether the lookup contains the specified key.
  /// </summary>
  /// <param name="key">The key to locate in the lookup.</param>
  /// <returns><c>true</c> if the lookup contains the specified key; otherwise, <c>false</c>.</returns>
  public bool Contains(
    TKey key )
  {
    return _dictionary.ContainsKey( key );
  }

  #endregion

  #region Implementation

  /// <inheritdoc />
  IEnumerator IEnumerable.GetEnumerator()
  {
    return GetEnumerator();
  }

  /// <summary>
  ///   Creates a new container for the values associated with a key.
  /// </summary>
  /// <returns></returns>
  protected abstract TContainer CreateContainer();

  /// <summary>
  ///   Adds a value to the specified container.
  /// </summary>
  /// <param name="container">The container to add the value to.</param>
  /// <param name="value">The value to add.</param>
  /// <returns><c>true</c> if the item was added to the collection; otherwise, <c>false</c>.</returns>
  protected abstract bool AddValue(
    TContainer container,
    TValue value );

  /// <summary>
  ///   Adds a range of values to the specified container.
  /// </summary>
  /// <param name="container">The container to add the values to.</param>
  /// <param name="values">The values to add.</param>
  protected abstract void AddValues(
    TContainer container,
    IEnumerable<TValue> values );

  /// <summary>
  ///   Sets the capacity of the container to what it would be if it had been originally initialized with all its entries.
  /// </summary>
  protected abstract void TrimExcess(
    TContainer container );

  #endregion
}
