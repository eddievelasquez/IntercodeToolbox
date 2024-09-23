// Module Name: UnsupportedTypeDiagnostic.cs
// Author:      Eduardo Velasquez
// Copyright (c) 2024, Intercode Consulting, Inc.

namespace Intercode.Toolbox.TypedPrimitives.Diagnostics;

using Microsoft.CodeAnalysis;

internal static class UnsupportedTypeDiagnostic
{
  #region Constants

  public const string Id = DiagnosticId.UnsupportedPrimitiveTypeDiagnostic;

  #endregion

  #region Public Methods

  public static DiagnosticInfo Create(
    SyntaxNode? syntaxNode,
    Type unsupportedType )
  {
    return new DiagnosticInfo(
      new DiagnosticDescriptor(
        Id,
        "Unsupported type",
        $"The {unsupportedType.FullName} type is not supported",
        nameof( DiagnosticCategory.Usage ),
        DiagnosticSeverity.Error,
        true
      ),
      syntaxNode?.GetLocation()
    );
  }

  #endregion
}
