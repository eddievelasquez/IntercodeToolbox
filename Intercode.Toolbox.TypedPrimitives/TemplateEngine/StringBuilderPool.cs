// Module Name: StringBuilderPool.cs
// Author:      Eduardo Velasquez
// Copyright (c) 2024, Intercode Consulting, Inc.

namespace Intercode.Toolbox.TypedPrimitives.TemplateEngine;

using System.Collections.Concurrent;
using System.Text;

public class StringBuilderPool
{
  #region Fields

  private readonly ConcurrentBag<StringBuilder> _pool;
  private readonly int _initialCapacity;
  private readonly int _maxPoolSize;

  #endregion

  #region Constructors

  public StringBuilderPool(
    int initialCapacity = 1024,
    int maxPoolSize = 100 )
  {
    _initialCapacity = initialCapacity;
    _maxPoolSize = maxPoolSize;
    _pool = [];
  }

  #endregion

  #region Properties

  public static StringBuilderPool Default { get; } = new ();

  #endregion

  #region Public Methods

  public StringBuilder Get()
  {
    // Try to get a builder from the pool
    if( _pool.TryTake( out var builder ) )
    {
      return builder;
    }

    // The pool was empty, so create a new builder
    return new StringBuilder( _initialCapacity );
  }

  public void Return(
    StringBuilder builder )
  {
    if( builder == null )
    {
      throw new ArgumentNullException( nameof( builder ) );
    }

    // Clear the builder to avoid leaking data
    builder.Clear();

    // Add the builder back to the pool if it's not full
    if( _pool.Count < _maxPoolSize )
    {
      _pool.Add( builder );
    }
  }

  #endregion
}
