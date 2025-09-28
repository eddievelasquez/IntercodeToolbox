// Module Name: ResultTest.cs
// Author:      Eduardo Velasquez
// Copyright (c) 2025, Intercode Consulting, Inc.

namespace Intercode.Toolbox.TypedPrimitives.Tests;

using FluentAssertions;
using Intercode.Toolbox.TypedPrimitives.Diagnostics;
using Microsoft.CodeAnalysis;

public class ResultTest
{
  #region Tests

  [Fact]
  public void Fail_WithEmptyEnumerable_ShouldBeSuccess_AndHaveNoErrors()
  {
    // Act
    var result = Result.Fail( [] );

    // Assert
    result.IsSuccess.Should().BeTrue();
    result.Errors.Length.Should().Be( 0 );
  }

  [Fact]
  public void Fail_WithEnumerableDiagnostics_ShouldBeFailed_WithAllErrors()
  {
    // Arrange
    var diagnostics = new[] { CreateDiagnostic( "ID1" ), CreateDiagnostic( "ID2" ) };

    // Act
    var result = Result.Fail( diagnostics );

    // Assert
    result.IsFailed.Should().BeTrue();
    result.Errors.Length.Should().Be( diagnostics.Length );
  }

  [Fact]
  public void Fail_WithNull_ShouldBeSuccess_AndHaveNoErrors()
  {
    // Act
    var result = Result.Fail( errors: null );

    // Assert
    result.IsSuccess.Should().BeTrue();
    result.Errors.Length.Should().Be( 0 );
  }

  [Fact]
  public void Fail_WithSingleDiagnostic_ShouldBeFailed_WithOneError()
  {
    // Arrange
    var di = CreateDiagnostic();

    // Act
    var result = Result.Fail( di );

    // Assert
    result.IsFailed.Should().BeTrue();
    result.IsSuccess.Should().BeFalse();
    result.Errors.Length.Should().Be( 1 );
  }

  [Fact]
  public void Ok_ShouldBeSuccess_AndHaveNoErrors()
  {
    // Act
    var result = Result.Ok();

    // Assert
    result.IsSuccess.Should().BeTrue();
    result.IsFailed.Should().BeFalse();
    result.Errors.Length.Should().Be( 0 );
  }

  #endregion

  #region Implementation

  private static DiagnosticInfo CreateDiagnostic(
    string id = "ITTP0001",
    DiagnosticSeverity severity = DiagnosticSeverity.Error )
  {
    var descriptor = new DiagnosticDescriptor(
      id,
      "Test Title",
      "Test Message",
      "TypedPrimitives",
      severity,
      true
    );
    return new DiagnosticInfo( descriptor, null );
  }

  #endregion
}
