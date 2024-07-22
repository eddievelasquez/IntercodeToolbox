// Module Name: XUnitLoggerProviderTests.cs
// Author:      Eduardo Velasquez
// Copyright (c) 2024, Intercode Consulting, Inc.

namespace Intercode.Toolbox.UnitTesting.XUnit.Tests;

using FluentAssertions;
using Microsoft.Extensions.Logging;
using Xunit.Abstractions;

public class XUnitLoggerProviderTests
{
  #region Fields

  private readonly ITestOutputHelper _output;

  #endregion

  #region Setup/Teardown

  public XUnitLoggerProviderTests(
    ITestOutputHelper output )
  {
    _output = output;
  }

  #endregion

  #region Tests

  [Fact]
  public void CreateLogger_ShouldReturnSameLogger_WhenUsingSameCategoryName()
  {
    using var provider = new XUnitLoggerProvider( _output, LogLevel.Information );
    var logger1 = provider.CreateLogger( "Test" );
    var logger2 = provider.CreateLogger( "Test" );

    logger1.Should()
           .BeSameAs( logger2 );
  }

  [Fact]
  public void CreateLogger_ShouldSucceed()
  {
    using var provider = new XUnitLoggerProvider( _output, LogLevel.Information );
    var logger = provider.CreateLogger( "Test" );

    logger.Should()
          .NotBeNull();

    logger.IsEnabled( LogLevel.Information )
          .Should()
          .BeTrue();

    logger.Should()
          .BeOfType<XUnitLogger>()
          .Which.Output.Should()
          .Be( _output );
  }

  #endregion
}
