// Module Name: UnexpectedDiagnostic.cs
// Author:      Eduardo Velasquez
// Copyright (c) 2024, Intercode Consulting, Inc.

namespace Intercode.Toolbox.TypedPrimitives.Diagnostics;

using Microsoft.CodeAnalysis;

internal static class UnexpectedDiagnostic
{
  #region Constants

  public const string Id = DiagnosticId.Unexpected;
  public const string Title = "Unexpected";

  #endregion

  #region Public Methods

  public static DiagnosticInfo Create(
    string message,
    SyntaxNode? syntaxNode )
  {
    return new DiagnosticInfo(
      new DiagnosticDescriptor(
        Id,
        Title,
        message,
        nameof( DiagnosticCategory.Usage ),
        DiagnosticSeverity.Error,
        true
      ),
      syntaxNode?.GetLocation()
    );
  }

  #endregion
}
