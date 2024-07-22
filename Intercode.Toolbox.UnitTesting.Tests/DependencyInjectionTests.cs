// Module Name: DependencyInjectionTests.cs
// Author:      Eduardo Velasquez
// Copyright (c) 2024, Intercode Consulting, Inc.

namespace Intercode.Toolbox.UnitTesting.Tests;

using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

public class DependencyInjectionTests
{
  #region Tests

  [Fact]
  public void CreateConfiguration_ShouldReturnEmpty_WhenNoValuesNorActionProvided()
  {
    var configuration = DependencyInjection.CreateConfiguration();
    configuration.AsEnumerable()
                 .Should()
                 .BeEmpty();
  }

  [Fact]
  public void CreateConfiguration_ShouldReturnValues_WhenActionAndValuesProvided()
  {
    var values = new Dictionary<string, string?>
    {
      { "Key1", "Value1" },
      { "Key2", "Value2" }
    };

    var actionValues = new Dictionary<string, string?> { { "Key3", "Value3" } };

    var configuration = DependencyInjection.CreateConfiguration(
      values,
      builder => builder.AddInMemoryCollection( actionValues )
    );

    var expectedValues = new Dictionary<string, string?>
    {
      { "Key1", "Value1" },
      { "Key2", "Value2" },
      { "Key3", "Value3" }
    };

    configuration.AsEnumerable()
                 .Should()
                 .BeEquivalentTo( expectedValues );
  }

  [Fact]
  public void CreateConfiguration_ShouldReturnValues_WhenActionProvided()
  {
    var values = new Dictionary<string, string?>
    {
      { "Key1", "Value1" },
      { "Key2", "Value2" }
    };

    var configuration =
      DependencyInjection.CreateConfiguration( configureSource: builder => builder.AddInMemoryCollection( values ) );

    configuration.AsEnumerable()
                 .Should()
                 .BeEquivalentTo( values );
  }

  [Fact]
  public void CreateConfiguration_ShouldReturnValues_WhenValuesProvided()
  {
    var values = new Dictionary<string, string?>
    {
      { "Key1", "Value1" },
      { "Key2", "Value2" }
    };

    var configuration = DependencyInjection.CreateConfiguration( values );
    configuration.AsEnumerable()
                 .Should()
                 .BeEquivalentTo( values );
  }

  [Fact]
  public void CreateServiceProvider_ShouldInvokeServiceSetter()
  {
    var configuration = DependencyInjection.CreateConfiguration();

    var serviceProvider = DependencyInjection.CreateServiceProvider(
      configuration,
      services => { services.AddSingleton( TimeProvider.System ); }
    );

    serviceProvider.GetService( typeof( TimeProvider ) )
                   .Should()
                   .BeSameAs( TimeProvider.System );
  }

  [Fact]
  public void CreateServiceProvider_ShouldReturnServiceProviderWithConfigurationSingleton()
  {
    var configuration = DependencyInjection.CreateConfiguration();

    var serviceProvider = DependencyInjection.CreateServiceProvider( configuration );

    serviceProvider.GetService( typeof( IConfiguration ) )
                   .Should()
                   .BeSameAs( configuration );
  }

  [Fact]
  public void CreateServiceProvider_ShouldThrow_WhenConfigurationIsNull()
  {
    var act = () => DependencyInjection.CreateServiceProvider( null! );

    act.Should()
       .Throw<ArgumentNullException>();
  }

  #endregion
}
