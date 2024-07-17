// Module Name: ReaderWriterLockExtensions.cs
// Author:      Eduardo Velasquez
// Copyright (c) 2023, Intercode Consulting, Inc.

namespace Intercode.Toolbox.Core.Threading;

/// <summary>
///   Extension methods for <see cref="ReaderWriterLockSlim" />.
/// </summary>
public static class ReaderWriterLockExtensions
{
  #region Public Methods

  /// <summary>
  ///   Acquires a read lock on the specified <see cref="ReaderWriterLockSlim" />.
  /// </summary>
  /// <param name="rwLock">The lock to acquire a read lock on.</param>
  /// <returns>An instance of <see cref="Disposer" /> which can be used to release the acquired lock.</returns>
  public static Disposer ReadLock(
    this ReaderWriterLockSlim rwLock )
  {
    rwLock.EnterReadLock();
    return new Disposer( rwLock.ExitReadLock );
  }

  /// <summary>
  ///   Acquires an upgradable read lock on the specified <see cref="ReaderWriterLockSlim" />.
  /// </summary>
  /// <param name="rwLock">The lock to acquire an upgradable read lock on.</param>
  /// <returns>An instance of <see cref="Disposer" /> which can be used to release the acquired lock.</returns>
  public static Disposer UpgradableReadLock(
    this ReaderWriterLockSlim rwLock )
  {
    rwLock.EnterUpgradeableReadLock();
    return new Disposer( rwLock.ExitUpgradeableReadLock );
  }

  /// <summary>
  ///   Acquires a write lock on the specified <see cref="ReaderWriterLockSlim" />.
  /// </summary>
  /// <param name="rwLock">The lock to acquire a write lock on.</param>
  /// <returns>An instance of <see cref="Disposer" /> which can be used to release the acquired lock.</returns>
  public static Disposer WriteLock(
    this ReaderWriterLockSlim rwLock )
  {
    rwLock.EnterWriteLock();
    return new Disposer( rwLock.ExitWriteLock );
  }

  #endregion
}
