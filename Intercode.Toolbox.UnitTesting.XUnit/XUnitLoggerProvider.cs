// Module Name: XUnitLoggerProvider.cs
// Author:      Eduardo Velasquez
// Copyright (c) 2024, Intercode Consulting, Inc.

namespace Intercode.Toolbox.UnitTesting.XUnit;

using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;
using Xunit.Abstractions;

/// <summary>
///   Logger provider for XUnit tests.
/// </summary>
public sealed class XUnitLoggerProvider: ILoggerProvider
{
  #region Fields

  private readonly ConcurrentDictionary<string, XUnitLogger> _loggers = new ();
  private readonly ITestOutputHelper _outputHelper;
  private readonly LogLevel _logLevel;

  #endregion

  #region Constructors

  /// <summary>
  ///   Initializes a new instance of the <see cref="XUnitLoggerProvider" /> class.
  /// </summary>
  /// <param name="outputHelper">The test output helper.</param>
  /// <param name="logLevel">The log level.</param>
  public XUnitLoggerProvider(
    ITestOutputHelper outputHelper,
    LogLevel logLevel = LogLevel.Error )
  {
    ArgumentNullException.ThrowIfNull( outputHelper );

    _outputHelper = outputHelper;
    _logLevel = logLevel;
  }

  #endregion

  #region Public Methods

  /// <summary>
  ///   Disposes the logger provider.
  /// </summary>
  public void Dispose()
  {
    // Nothing to dispose
  }

  /// <summary>
  ///   Creates a logger for the specified category name. If a logger for the category name already exists, it is returned.
  /// </summary>
  /// <param name="categoryName">The category name.</param>
  /// <returns>The logger instance.</returns>
  public ILogger CreateLogger(
    string categoryName )
  {
    return _loggers.GetOrAdd( categoryName, name => new XUnitLogger( _outputHelper, name, _logLevel ) );
  }

  #endregion
}
