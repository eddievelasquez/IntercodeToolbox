// Module Name: DependencyInversionTestBase.cs
// Author:      Eduardo Velasquez
// Copyright (c) 2024, Intercode Consulting, Inc.

namespace Intercode.Toolbox.UnitTesting.XUnit;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Xunit.Abstractions;

/// <summary>
///   Base class for test classes that require dependency inversion.
/// </summary>
public abstract class DependencyInversionTestBase
{
  #region Constructors

  /// <summary>
  ///   Initializes a new instance of the <see cref="DependencyInversionTestBase" /> class.
  /// </summary>
  /// <param name="outputHelper">The test output helper.</param>
  /// <param name="loggerType">The type of the logger.</param>
  /// <param name="logLevel">The log level.</param>
  /// <param name="servicesSetter">The action to set up additional services.</param>
  /// <param name="configurationValues">The configuration values.</param>
  /// <param name="configureSource">The action to set up the configuration source.</param>
  protected DependencyInversionTestBase(
    ITestOutputHelper outputHelper,
    Type loggerType,
    LogLevel logLevel = LogLevel.Information,
    Action<IServiceCollection>? servicesSetter = null,
    Dictionary<string, string?>? configurationValues = null,
    Action<IConfigurationBuilder>? configureSource = null )
  {
    if( !loggerType.IsAssignableTo( typeof( ILogger ) ) )
    {
      throw new ArgumentException( "The logger type must be assignable to ILogger.", nameof( loggerType ) );
    }

    var configuration = DependencyInjection.CreateConfiguration( configurationValues, configureSource );

    ServiceProvider = DependencyInjection.CreateServiceProvider(
      configuration,
      services =>
      {
        services.AddLogging( configure => { configure.AddProvider( new XUnitLoggerProvider( outputHelper, logLevel ) ); } );
        servicesSetter?.Invoke( services );
      }
    );

    Logger = ( ILogger ) ServiceProvider.GetRequiredService( loggerType );
  }

  #endregion

  #region Properties

  /// <summary>
  ///   Gets the service provider.
  /// </summary>
  protected IServiceProvider ServiceProvider { get; }

  /// <summary>
  ///   Gets the test's logger.
  /// </summary>
  protected ILogger Logger { get; }

  #endregion
}
