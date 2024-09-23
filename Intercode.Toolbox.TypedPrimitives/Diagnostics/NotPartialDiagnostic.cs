// Module Name: NotPartialDiagnostic.cs
// Author:      Eduardo Velasquez
// Copyright (c) 2024, Intercode Consulting, Inc.

namespace Intercode.Toolbox.TypedPrimitives.Diagnostics;

using Microsoft.CodeAnalysis;

internal static class NotPartialDiagnostic
{
  #region Constants

  public const string Id = DiagnosticId.NotPartial;
  public const string Title = "Must be partial";
  public const string Message = "The target of the TypedPrimitive attribute must be partial";

  #endregion

  #region Public Methods

  public static DiagnosticInfo Create(
    SyntaxNode syntaxNode )
  {
    return new DiagnosticInfo(
      new DiagnosticDescriptor(
        Id,
        Title,
        Message,
        nameof( DiagnosticCategory.Usage ),
        DiagnosticSeverity.Error,
        true
      ),
      syntaxNode.GetLocation()
    );
  }

  #endregion
}
