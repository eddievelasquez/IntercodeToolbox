// Module Name: ResultOfTTest.cs
// Author:      Eduardo Velasquez
// Copyright (c) 2025, Intercode Consulting, Inc.

namespace Intercode.Toolbox.TypedPrimitives.Tests;

using FluentAssertions;
using Intercode.Toolbox.TypedPrimitives.Diagnostics;
using Microsoft.CodeAnalysis;

public class ResultOfTTest
{
  #region Tests

  [Fact]
  public void FailT_WithEmptyEnumerable_ShouldBeSuccess_WithDefaultValue_AndNoErrors()
  {
    // Act
    var result = Result.Fail<int>( [] );

    // Assert
    result.IsSuccess.Should().BeTrue();
    result.Errors.Length.Should().Be( 0 );
    result.Value.Should().Be( 0 );
  }

  [Fact]
  public void FailT_WithEnumerableDiagnostics_ShouldBeFailed_WithDefaultValue()
  {
    // Arrange
    var diagnostics = new[] { CreateDiagnostic( "ID1" ), CreateDiagnostic( "ID2" ) };

    // Act
    var result = Result.Fail<int>( diagnostics );

    // Assert
    result.IsFailed.Should().BeTrue();
    result.Errors.Length.Should().Be( diagnostics.Length );
    result.Value.Should().Be( 0 );
  }

  [Fact]
  public void FailT_WithNull_ShouldBeSuccess_WithDefaultValue_AndNoErrors()
  {
    // Act
    var result = Result.Fail<int>( errors: null );

    // Assert
    result.IsSuccess.Should().BeTrue();
    result.Errors.Length.Should().Be( 0 );
    result.Value.Should().Be( 0 );
  }

  [Fact]
  public void FailT_WithSingleDiagnostic_ShouldBeFailed_WithDefaultValue()
  {
    // Arrange
    var di = CreateDiagnostic();

    // Act
    var result = Result.Fail<int>( di );

    // Assert
    result.IsFailed.Should().BeTrue();
    result.Errors.Length.Should().Be( 1 );
    result.Value.Should().Be( 0 );
  }

  [Fact]
  public void OkT_ShouldBeSuccess_WithValue_AndNoErrors()
  {
    // Arrange
    var value = 123;

    // Act
    var result = Result.Ok( value );

    // Assert
    result.IsSuccess.Should().BeTrue();
    result.IsFailed.Should().BeFalse();
    result.Errors.Length.Should().Be( 0 );
    result.Value.Should().Be( value );
  }

  [Fact]
  public void Value_ShouldBeNullable_ForReferenceTypes()
  {
    // Arrange
    string? value = null;

    // Act
    var ok = Result.Ok( value );
    var fail = Result.Fail<string?>( CreateDiagnostic() );

    // Assert
    ok.IsSuccess.Should().BeTrue();
    ok.Value.Should().BeNull();
    fail.IsFailed.Should().BeTrue();
    fail.Value.Should().BeNull();
  }

  #endregion

  #region Implementation

  private static DiagnosticInfo CreateDiagnostic(
    string id = "ITTP0002",
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
