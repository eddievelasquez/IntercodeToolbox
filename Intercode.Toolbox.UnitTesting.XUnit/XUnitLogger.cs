// Module Name: XUnitLogger.cs
// Author:      Eduardo Velasquez
// Copyright (c) 2024, Intercode Consulting, Inc.

namespace Intercode.Toolbox.UnitTesting.XUnit;

using System.Text;
using Microsoft.Extensions.Logging;
using Xunit.Abstractions;

/// <summary>
///   A logger implementation that emits its output to an ITestOutputHelper instance.
/// </summary>
public sealed class XUnitLogger: ILogger
{
  #region Fields

  private readonly Stack<string> _scopes = new ();

  #endregion

  #region Constructors

  /// <summary>
  ///   Initializes a new instance of the <see cref="XUnitLogger" /> class.
  /// </summary>
  /// <param name="output">The XUnit test output helper.</param>
  /// <param name="categoryName">The category name for the logger.</param>
  /// <param name="logLevel">The minimum log level for the logger.</param>
  public XUnitLogger(
    ITestOutputHelper output,
    string? categoryName = null,
    LogLevel logLevel = LogLevel.Error )
  {
    ArgumentNullException.ThrowIfNull( output );

    Output = output;
    Level = logLevel;
    CategoryName = categoryName ?? string.Empty;
  }

  #endregion

  #region Properties

  /// <summary>
  ///   Gets the XUnit test output helper.
  /// </summary>
  public ITestOutputHelper Output { get; }

  /// <summary>
  ///   Gets the minimum log level for the logger.
  /// </summary>
  public LogLevel Level { get; }

  /// <summary>
  ///   Gets the category name for the logger.
  /// </summary>
  public string? CategoryName { get; }

  #endregion

  #region Public Methods

  /// <summary>
  ///   Writes a log message to the XUnit test output.
  /// </summary>
  /// <typeparam name="TState">The type of the log message.</typeparam>
  /// <param name="logLevel">The log level of the message.</param>
  /// <param name="eventId">The event ID of the message.</param>
  /// <param name="state">The log message.</param>
  /// <param name="exception">The exception associated with the message.</param>
  /// <param name="formatter">The formatter function to format the log message.</param>
  public void Log<TState>(
    LogLevel logLevel,
    EventId eventId,
    TState state,
    Exception? exception,
    Func<TState, Exception?, string> formatter )
  {
    if( !IsEnabled( logLevel ) )
    {
      return;
    }

    ArgumentNullException.ThrowIfNull( formatter );

    var builder = new StringBuilder();
    builder.Append( $"[{CategoryName}] [{logLevel}] [{eventId}] " );

    if( _scopes.Count > 0 )
    {
      builder.AppendJoin( " => ", _scopes.Reverse() );
      builder.Append( ' ' );
    }

    var message = formatter( state, exception );
    builder.Append( message );

    Output.WriteLine( builder.ToString() );
  }

  /// <summary>
  ///   Checks if the specified log level is enabled for logging.
  /// </summary>
  /// <param name="logLevel">The log level to check.</param>
  /// <returns><c>true</c> if the log level is enabled; otherwise, <c>false</c>.</returns>
  public bool IsEnabled(
    LogLevel logLevel )
  {
    return logLevel >= Level;
  }

  /// <summary>
  ///   Begins a logical operation scope.
  /// </summary>
  /// <typeparam name="TState">The type of the state.</typeparam>
  /// <param name="state">The state.</param>
  /// <returns>An <see cref="IDisposable" /> representing the logical operation scope.</returns>
  public IDisposable BeginScope<TState>(
    TState state )
    where TState: notnull
  {
    ArgumentNullException.ThrowIfNull( state );

    _scopes.Push( state.ToString()! );
    return new LoggerScope( this );
  }

  #endregion

  #region Implementation

  private void PopScope()
  {
    _scopes.TryPop( out _ );
  }

  #endregion

  #region Nested Types

  private sealed class LoggerScope( XUnitLogger logger ): IDisposable
  {
    #region Public Methods

    public void Dispose()
    {
      logger.PopScope();
    }

    #endregion
  }

  #endregion
}
