// Module Name: DebugAssertException.cs
// Author:      Eduardo Velasquez
// Copyright (c) 2024, Intercode Consulting, Inc.

namespace Intercode.Toolbox.UnitTesting;

/// <summary>
///   Represents an exception that is thrown by the <see cref="DebugAssertListener" /> when a debug assertion fails.
/// </summary>
public class DebugAssertException: Exception
{
  #region Constructors

  /// <summary>
  ///   Initializes a new instance of the <see cref="DebugAssertException" /> class with the specified error message.
  /// </summary>
  /// <param name="message">The error message that explains the reason for the exception.</param>
  public DebugAssertException(
    string? message )
    : base( message )
  {
  }

  /// <summary>
  ///   Initializes a new instance of the <see cref="DebugAssertException" /> class with the specified error message and
  ///   detail message.
  /// </summary>
  /// <param name="message">The error message that explains the reason for the exception.</param>
  /// <param name="detailMessage">The detailed error message.</param>
  public DebugAssertException(
    string? message,
    string? detailMessage )
    : base( message )
  {
    DetailMessage = detailMessage;
  }

  #endregion

  #region Properties

  /// <summary>
  ///   Gets the detailed error message.
  /// </summary>
  public string? DetailMessage { get; }

  #endregion
}
