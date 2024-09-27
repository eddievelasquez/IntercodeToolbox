// Module Name: StringBuilderPool.cs
// Author:      Eduardo Velasquez
// Copyright (c) 2024, Intercode Consulting, Inc.

namespace Intercode.Toolbox.TypedPrimitives.TemplateEngine;

using System.Text;

public class StringBuilderPool
{
  #region Fields

  private readonly LinkedList<StringBuilder> _pool = new ();
  private readonly ReaderWriterLockSlim _lock = new ();

  #endregion

  #region Properties

  public static StringBuilderPool Default { get; } = new ();

  #endregion

  #region Public Methods

  public StringBuilder Get()
  {
    _lock.EnterUpgradeableReadLock();

    try
    {
      if( _pool.Count <= 0 )
      {
        return new StringBuilder();
      }

      var builder = _pool.First.Value;
      _lock.EnterWriteLock();

      try
      {
        _pool.RemoveFirst();
      }
      finally
      {
        _lock.ExitWriteLock();
      }

      return builder;
    }
    finally
    {
      _lock.ExitUpgradeableReadLock();
    }
  }

  public void Return(
    StringBuilder builder )
  {
    builder.Clear();

    _lock.EnterWriteLock();

    try
    {
      _pool.AddFirst( builder );
    }
    finally
    {
      _lock.ExitWriteLock();
    }
  }

  #endregion
}
