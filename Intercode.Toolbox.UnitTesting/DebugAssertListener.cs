// Module Name: DebugAssertListener.cs
// Author:      Eduardo Velasquez
// Copyright (c) 2024, Intercode Consulting, Inc.

using System.Runtime.CompilerServices;

[assembly:InternalsVisibleTo("Intercode.Toolbox.UnitTesting.Tests")]

namespace Intercode.Toolbox.UnitTesting;

using System.Diagnostics;

/// <summary>
///   Represents a listener for debug and trace assert events.
/// </summary>
public sealed class DebugAssertListener: IDisposable
{
  #region Constructors

  /// <summary>
  ///   Initializes a new instance of the <see cref="DebugAssertListener" /> class.
  /// </summary>
  public DebugAssertListener()
  {
    InnerListener = new TestTraceListener();

    // Our listener must be the first one to intercept the trace and throw the DebugAssertException.
    Trace.Listeners.Insert( 0, InnerListener );
  }

  #endregion

  #region Properties

  /// <summary>
  ///   Gets the inner trace listener.
  /// </summary>
  internal TestTraceListener InnerListener { get; }

  #endregion

  #region Public Methods

  /// <summary>
  ///   Gets the trace information captured by the listener.
  /// </summary>
  /// <returns>The trace information captured by the listener.</returns>
  public string GetTrace()
  {
    return InnerListener.GetTrace();
  }

  /// <summary>
  ///   Disposes the <see cref="DebugAssertListener" /> instance and removes it from the trace listener collection.
  /// </summary>
  public void Dispose()
  {
    // Remove our listener from the trace listener collection.
    Trace.Listeners.Remove( InnerListener );
  }

  #endregion
}
