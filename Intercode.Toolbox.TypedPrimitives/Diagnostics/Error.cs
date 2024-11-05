// Module Name: Error.cs
// Author:      Eduardo Velasquez
// Copyright (c) 2024, Intercode Consulting, Inc.

namespace Intercode.Toolbox.TypedPrimitives.Diagnostics;

using Microsoft.CodeAnalysis;

internal static class Error
{
  #region Public Methods

  public static DiagnosticInfo NotPartial(
    INamedTypeSymbol symbol,
    SyntaxNode syntaxNode )
  {
    return CreateDiagnostic(
      DiagnosticId.NotPartial,
      "Must be partial",
      $"The '{symbol.QualifiedName()}' struct must be partial",
      syntaxNode
    );
  }

  public static Result<GeneratorModel> NotPartialFailure(
    INamedTypeSymbol symbol,
    SyntaxNode syntaxNode )
  {
    return Result.Fail<GeneratorModel>( NotPartial( symbol, syntaxNode ) );
  }

  public static DiagnosticInfo NotReadOnly(
    INamedTypeSymbol symbol,
    SyntaxNode syntaxNode )
  {
    return CreateDiagnostic(
      DiagnosticId.NotReadOnly,
      "Must be readonly",
      $"The '{symbol.QualifiedName()}' struct must be readonly",
      syntaxNode
    );
  }

  public static Result<GeneratorModel> NotReadOnlyFailure(
    INamedTypeSymbol symbol,
    SyntaxNode syntaxNode )
  {
    return Result.Fail<GeneratorModel>( NotReadOnly( symbol, syntaxNode ) );
  }

  public static DiagnosticInfo MissingPrimitiveType(
    ISymbol symbol,
    SyntaxNode? syntaxNode = null )
  {
    return CreateDiagnostic(
      DiagnosticId.MissingPrimitiveType,
      "Missing primitive type",
      $"The TypedPrimitive attribute for '{symbol.QualifiedName()}' must provide the primitive's type",
      syntaxNode
    );
  }

  public static Result<GeneratorModel> MissingPrimitiveTypeFailure(
    ISymbol symbol,
    SyntaxNode? syntaxNode = null )
  {
    return Result.Fail<GeneratorModel>( MissingPrimitiveType( symbol, syntaxNode ) );
  }

  public static DiagnosticInfo Unexpected(
    string message,
    SyntaxNode? syntaxNode = null )
  {
    return CreateDiagnostic(
      DiagnosticId.Unexpected,
      "Unexpected",
      message,
      syntaxNode
    );
  }

  public static Result<GeneratorModel> UnexpectedFailure(
    string message,
    SyntaxNode? syntaxNode = null )
  {
    return Result.Fail<GeneratorModel>( Unexpected( message, syntaxNode ) );
  }

  public static DiagnosticInfo UnsupportedType(
    string typeName,
    SyntaxNode? syntaxNode = null )
  {
    return CreateDiagnostic(
      DiagnosticId.UnsupportedPrimitiveTypeDiagnostic,
      "Unsupported type",
      $"The {typeName} type is not supported",
      syntaxNode
    );
  }

  public static Result<GeneratorModel> UnsupportedTypeFailure(
    string typeName,
    SyntaxNode? syntaxNode = null )
  {
    return Result.Fail<GeneratorModel>( UnsupportedType( typeName, syntaxNode ) );
  }

  #endregion

  #region Implementation

  private static DiagnosticInfo CreateDiagnostic(
    string diagnosticId,
    string title,
    string message,
    SyntaxNode? syntaxNode )
  {
    return new DiagnosticInfo(
      new DiagnosticDescriptor(
        diagnosticId,
        title,
        message,
        nameof( DiagnosticCategory.Usage ),
        DiagnosticSeverity.Error,
        true
      ),
      LocationInfo.Create( syntaxNode )
    );
  }

  #endregion
}
