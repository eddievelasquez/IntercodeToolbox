// Module Name: EnumerableExtensions.cs
// Author:      Eduardo Velasquez
// Copyright (c) 2024, Intercode Consulting, Inc.

namespace Intercode.Toolbox.Collections;

using System.Collections;

/// <summary>
///   Provides extension methods for working with enumerable collections.
/// </summary>
public static class EnumerableExtensions
{
  #region Public Methods

  /// <summary>
  ///   Returns an empty array if the source array is null.
  /// </summary>
  /// <typeparam name="T">The type of the array elements.</typeparam>
  /// <param name="source">The source array.</param>
  /// <returns>An empty array if the source array is null; otherwise, the source array.</returns>
  public static T[] EmptyIfNull<T>(
    this T[]? source )
  {
    return source ?? [];
  }

  /// <summary>
  ///   Returns an empty enumerable if the source enumerable is null.
  /// </summary>
  /// <typeparam name="T">The type of the enumerable elements.</typeparam>
  /// <param name="source">The source enumerable.</param>
  /// <returns>An empty enumerable if the source enumerable is null; otherwise, the source enumerable.</returns>
  public static IEnumerable<T> EmptyIfNull<T>(
    this IEnumerable<T>? source )
  {
    return source ?? [];
  }

  /// <summary>
  ///   Returns an empty collection if the source collection is null.
  /// </summary>
  /// <typeparam name="TCollection">The type of the collection.</typeparam>
  /// <param name="source">The source collection.</param>
  /// <returns>An empty collection if the source collection is null; otherwise, the source collection.</returns>
  public static TCollection EmptyIfNull<TCollection>(
    this TCollection? source )
    where TCollection: class, IEnumerable, new()
  {
    return source ?? new TCollection();
  }

  /// <summary>
  ///   Converts the specified item to an array.
  /// </summary>
  /// <typeparam name="T">The type of the item.</typeparam>
  /// <param name="item">The item to convert.</param>
  /// <returns>An array containing the specified item if it is not null; otherwise, an empty array.</returns>
  public static T[] AsArray<T>(
    this T? item )
  {
    return item == null ? [] : [item];
  }

  /// <summary>
  ///   Batches the elements of the source enumerable into smaller enumerables of the specified batch size.
  /// </summary>
  /// <typeparam name="T">The type of the enumerable elements.</typeparam>
  /// <param name="source">The source enumerable.</param>
  /// <param name="batchSize">The size of each batch.</param>
  /// <returns>
  ///   An enumerable of enumerables, each containing the elements of the source enumerable batched by the specified
  ///   size.
  /// </returns>
  /// <exception cref="ArgumentNullException">Thrown when the source enumerable is null.</exception>
  /// <exception cref="ArgumentOutOfRangeException">Thrown when the batch size is less than or equal to zero.</exception>
  public static IEnumerable<IEnumerable<T>> Batch<T>(
    this IEnumerable<T> source,
    int batchSize )
  {
    ArgumentNullException.ThrowIfNull( source );

    if( batchSize <= 0 )
    {
      throw new ArgumentOutOfRangeException( nameof( batchSize ), batchSize, "The batch size must be greater than zero." );
    }

    var batch = new List<T>( batchSize );
    foreach( var value in source )
    {
      batch.Add( value );

      if( batch.Count != batchSize )
      {
        continue;
      }

      yield return batch;

      batch = new List<T>( batchSize );
    }

    if( batch.Count > 0 )
    {
      yield return batch;
    }
  }

  /// <summary>
  ///   Batches the elements of the source list into smaller lists of the specified batch size.
  /// </summary>
  /// <typeparam name="T">The type of the list elements.</typeparam>
  /// <param name="source">The source list.</param>
  /// <param name="batchSize">The size of each batch.</param>
  /// <returns>An enumerable of lists, each containing the elements of the source list batched by the specified size.</returns>
  /// <exception cref="ArgumentNullException">Thrown when the source list is null.</exception>
  /// <exception cref="ArgumentOutOfRangeException">Thrown when the batch size is less than or equal to zero.</exception>
  public static IEnumerable<IEnumerable<T>> Batch<T>(
    this IList<T> source,
    int batchSize )
  {
    ArgumentNullException.ThrowIfNull( source );

    if( batchSize <= 0 )
    {
      throw new ArgumentOutOfRangeException( nameof( batchSize ), batchSize, "The batch size must be greater than zero." );
    }

    var total = 0;
    while( total < source.Count )
    {
      yield return source.Skip( total )
                         .Take( batchSize );

      total += batchSize;
    }
  }

  #endregion
}
