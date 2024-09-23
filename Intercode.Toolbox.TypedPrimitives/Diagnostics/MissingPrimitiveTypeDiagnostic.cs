// Module Name: MissingPrimitiveTypeDiagnostic.cs
// Author:      Eduardo Velasquez
// Copyright (c) 2024, Intercode Consulting, Inc.

namespace Intercode.Toolbox.TypedPrimitives.Diagnostics;

using Microsoft.CodeAnalysis;

internal static class MissingPrimitiveTypeDiagnostic
{
  #region Constants

  public const string Id = DiagnosticId.MissingPrimitiveType;
  public const string Title = "Missing primitive type";
  public const string Message = "The constructor of the TypedPrimitive attribute must provide the primitive's type";

  #endregion

  #region Public Methods

  public static DiagnosticInfo Create(
    SyntaxNode? syntaxNode,
    string? message = null )
  {
    return new DiagnosticInfo(
      new DiagnosticDescriptor(
        Id,
        Title,
        message ?? Message,
        nameof( DiagnosticCategory.Usage ),
        DiagnosticSeverity.Error,
        true
      ),
      syntaxNode?.GetLocation()
    );
  }

  #endregion
}
