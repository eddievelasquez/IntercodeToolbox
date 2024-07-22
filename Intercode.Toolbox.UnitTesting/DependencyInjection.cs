// Module Name: DependencyInjection.cs
// Author:      Eduardo Velasquez
// Copyright (c) 2024, Intercode Consulting, Inc.

namespace Intercode.Toolbox.UnitTesting;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

/// <summary>
///   Provides infrastructure methods for creating instances of <see cref="IConfiguration" /> and
///   <see cref="IServiceProvider" /> for unit testing scenarios that require dependency injection.
/// </summary>
public static class DependencyInjection
{
  #region Public Methods

  /// <summary>
  ///   Creates an instance of <see cref="IConfigurationRoot" /> based on the provided values and/or configuration source.
  /// </summary>
  /// <param name="values">A dictionary of key-value pairs representing the configuration values.</param>
  /// <param name="configureSource">An action to configure the <see cref="IConfigurationBuilder" />.</param>
  /// <returns>An instance of <see cref="IConfiguration" />.</returns>
  public static IConfigurationRoot CreateConfiguration(
    Dictionary<string, string?>? values = null,
    Action<IConfigurationBuilder>? configureSource = null )
  {
    var builder = new ConfigurationBuilder();
    configureSource?.Invoke( builder );

    if( values is { Count: > 0 } )
    {
      builder.AddInMemoryCollection( values );
    }

    var configurationRoot = builder.Build();
    return configurationRoot;
  }

  /// <summary>
  ///   Creates an instance of <see cref="IServiceProvider" /> based on the provided <see cref="IConfiguration" /> and
  ///   service collection configuration.
  /// </summary>
  /// <param name="configurationRoot">The <see cref="IConfigurationRoot" /> instance.</param>
  /// <param name="servicesSetter">An action to configure the <see cref="IServiceCollection" />.</param>
  /// <returns>An instance of <see cref="IServiceProvider" />.</returns>
  public static IServiceProvider CreateServiceProvider(
    IConfigurationRoot configurationRoot,
    Action<IServiceCollection>? servicesSetter = null )
  {
    ArgumentNullException.ThrowIfNull( configurationRoot );

    var services = new ServiceCollection();
    services.AddSingleton<IConfiguration>( _ => configurationRoot );

    servicesSetter?.Invoke( services );

    var serviceProvider = services.BuildServiceProvider();
    return serviceProvider;
  }

  #endregion
}
