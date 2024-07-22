// Module Name: XUnitLoggerTests.cs
// Author:      Eduardo Velasquez
// Copyright (c) 2024, Intercode Consulting, Inc.

namespace Intercode.Toolbox.UnitTesting.XUnit.Tests;

using FluentAssertions;
using Microsoft.Extensions.Logging;
using Xunit.Abstractions;

public class XUnitLoggerTests
{
  #region Fields

  private readonly OutputCapture _output;

  #endregion

  #region Setup/Teardown

  public XUnitLoggerTests(
    ITestOutputHelper output )
  {
    _output = new OutputCapture( output );
  }

  #endregion

  #region Tests

  [Fact]
  public void BeginScope_ShouldAddScope()
  {
    var logger = new XUnitLogger( _output, "Test", LogLevel.Information );
    var lines = _output.Lines;

    using( logger.BeginScope( "Scope1" ) )
    {
      WriteLog();

      lines.Count.Should()
           .Be( 1 );

      lines[0]
        .Should()
        .Be( "[Test] [Information] [1] Scope1 Test" );

      using( logger.BeginScope( "Scope2" ) )
      {
        WriteLog();

        lines.Count.Should()
             .Be( 2 );

        lines[1]
          .Should()
          .Be( "[Test] [Information] [1] Scope1 => Scope2 Test" );
      }

      WriteLog();

      lines.Count.Should()
           .Be( 3 );

      lines[2]
        .Should()
        .Be( "[Test] [Information] [1] Scope1 Test" );
    }

    WriteLog();

    lines.Count.Should()
         .Be( 4 );

    lines[3]
      .Should()
      .Be( "[Test] [Information] [1] Test" );

    void WriteLog()
    {
      logger.Log(
        LogLevel.Information,
        new EventId( 1 ),
        "Test",
        null,
        static (
          state,
          _ ) => state
      );
    }
  }

  [Fact]
  public void Constructor_ShouldHaveEmptyCategoryName_WhenNameIsNull()
  {
    var logger = new XUnitLogger( _output, null );

    logger.CategoryName.Should()
          .BeEmpty();
  }

  [Fact]
  public void Constructor_ShouldSucceed()
  {
    var logger = new XUnitLogger( _output, "Test", LogLevel.Information );

    logger.Output.Should()
          .BeSameAs( _output );

    logger.Level.Should()
          .Be( LogLevel.Information );

    logger.CategoryName.Should()
          .Be( "Test" );
  }

  [Fact]
  public void Constructor_ShouldThrow_WhenOutputIsNull()
  {
    var act = () => new XUnitLogger( null! );

    act.Should()
       .Throw<ArgumentNullException>();
  }

  [Fact]
  public void Constructor_ShouldUseErrorLogLevel_WhenLogLevelIsNotProvided()
  {
    var logger = new XUnitLogger( _output );

    logger.Level.Should()
          .Be( LogLevel.Error );
  }

  [Fact]
  public void IsEnabled_ShouldReturnFalse_WhenTheLevelIsLessThanLoggerLevel()
  {
    var logger = new XUnitLogger( _output, "Test", LogLevel.Information );

    var result = logger.IsEnabled( LogLevel.Debug );

    result.Should()
          .BeFalse();
  }

  [Fact]
  public void IsEnabled_ShouldReturnTrue_WhenTheLevelIsGreaterOrEqualThanLoggerLevel()
  {
    var logger = new XUnitLogger( _output, "Test", LogLevel.Information );

    var result = logger.IsEnabled( LogLevel.Warning );

    result.Should()
          .BeTrue();
  }

  [Fact]
  public void Log_ShouldNotWrite_WhenLevelIsLessThanLoggerLevel()
  {
    var logger = new XUnitLogger( _output, "Test", LogLevel.Information );

    logger.Log(
      LogLevel.Debug,
      new EventId( 1 ),
      "Test",
      null,
      static (
        state,
        _ ) => state
    );

    _output.Lines.Should()
           .BeEmpty();
  }

  [Fact]
  public void Log_ShouldWrite_WhenLevelIsGreaterOrEqualThanLoggerLevel()
  {
    var logger = new XUnitLogger( _output, "Test", LogLevel.Information );

    logger.Log(
      LogLevel.Information,
      new EventId( 1 ),
      "Test",
      null,
      static (
        state,
        _ ) => state
    );

    _output.Lines.Should()
           .ContainSingle()
           .Which.Should()
           .Contain( "[Test] [Information] [1] Test" );
  }

  [Fact]
  public void Log_ShouldWriteException_WhenExceptionIsProvided()
  {
    var logger = new XUnitLogger( _output, "Test", LogLevel.Information );
    var exception = new Exception( "Test exception" );

    logger.Log(
      LogLevel.Information,
      new EventId( 1 ),
      "Test",
      exception,
      static (
        _,
        ex ) => ex?.Message ?? string.Empty
    );

    _output.Lines.Should()
           .ContainSingle()
           .Which.Should()
           .Be( "[Test] [Information] [1] Test exception" );
  }

  #endregion

  #region Implementation

  private sealed class OutputCapture: ITestOutputHelper
  {
    #region Fields

    private readonly List<string> _lines = new ();
    private readonly ITestOutputHelper _output;

    #endregion

    #region Constructors

    public OutputCapture(
      ITestOutputHelper output )
    {
      _output = output;
    }

    #endregion

    #region Properties

    public IReadOnlyList<string> Lines => _lines;

    #endregion

    #region Public Methods

    public void WriteLine(
      string message )
    {
      _lines.Add( message );
      _output.WriteLine( message );
    }

    public void WriteLine(
      string format,
      params object[] args )
    {
      _lines.Add( string.Format( format, args ) );
      _output.WriteLine( format, args );
    }

    #endregion
  }

  #endregion
}
