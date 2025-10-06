// Module Name: Disposer.cs
// Author:      Eduardo Velasquez
// Copyright (c) 2024, Intercode Consulting, Inc.

namespace Intercode.Toolbox.Core;

/// <summary>
///   Disposer will invoke a user-provided action when disposed.
/// </summary>
public sealed class Disposer: IDisposable
{
  #region Fields

  private Action? _disposer;

  #endregion

  #region Constructors

  /// <summary>
  ///   Initializes a new instance of the <see cref="Disposer" /> class.
  /// </summary>
  /// <param name="disposer">The action to invoke when this instance is disposed.</param>
  public Disposer(
    Action? disposer = null )
  {
    _disposer = disposer;
  }

  #endregion

  #region Public Methods

  /// <summary>
  ///   Disposes of the object.
  /// </summary>
  public void Dispose()
  {
    _disposer?.Invoke();
    _disposer = null; // Prevent double disposal
  }

  #endregion
}
