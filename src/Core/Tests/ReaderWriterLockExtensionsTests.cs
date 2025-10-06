// Module Name: ReaderWriterLockExtensionsTests.cs
// Author:      Eduardo Velasquez
// Copyright (c) 2024, Intercode Consulting, Inc.

namespace Intercode.Toolbox.Core.Tests;

using FluentAssertions;
using Intercode.Toolbox.Core.Threading;

public class ReaderWriterLockExtensionsTests
{
  #region Tests

  [Fact]
  public void ReadLock_InvokesEnterReadLock()
  {
    var rwLock = new ReaderWriterLockSlim();

    using( rwLock.ReadLock() )
    {
      AssertLockHeld( rwLock, LockType.Read );
    }

    AssertLockHeld( rwLock, LockType.None );
  }

  [Fact]
  public void UpgradableReadLock_InvokesEnterUpgradeableReadLock()
  {
    var rwLock = new ReaderWriterLockSlim();

    using( rwLock.UpgradableReadLock() )
    {
      AssertLockHeld( rwLock, LockType.UpgradeableRead );

      using( rwLock.WriteLock() )
      {
        AssertLockHeld( rwLock, LockType.UpgradeableRead | LockType.Write );
      }

      AssertLockHeld( rwLock, LockType.UpgradeableRead );
    }

    AssertLockHeld( rwLock, LockType.None );
  }

  [Fact]
  public void WriteLock_InvokesEnterWriteLock()
  {
    var rwLock = new ReaderWriterLockSlim();

    using( rwLock.WriteLock() )
    {
      AssertLockHeld( rwLock, LockType.Write );
    }

    AssertLockHeld( rwLock, LockType.None );
  }

  #endregion

  #region Implementation

  [Flags]
  private enum LockType
  {
    None = 0,
    Read = 1,
    UpgradeableRead = 2,
    Write = 4
  }

  private static void AssertLockHeld(
    ReaderWriterLockSlim rwLock,
    LockType lockType )
  {
    rwLock.IsReadLockHeld.Should()
          .Be( LockHeld( LockType.Read ) );

    rwLock.IsUpgradeableReadLockHeld.Should()
          .Be( LockHeld( LockType.UpgradeableRead ) );

    rwLock.IsWriteLockHeld.Should()
          .Be( LockHeld( LockType.Write ) );

    return;

    bool LockHeld(
      LockType type )
    {
      return ( lockType & type ) == type;
    }
  }

  #endregion
}
