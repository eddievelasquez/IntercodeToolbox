// Module Name: ValidatorFlagsMustBeEnum.cs
// Author:      Eduardo Velasquez
// Copyright (c) 2024, Intercode Consulting, Inc.

namespace Intercode.Toolbox.TypedPrimitives.Diagnostics;

using Microsoft.CodeAnalysis;

internal static class ValidatorFlagsMustBeEnum
{
  #region Constants

  public const string Id = DiagnosticId.ValidatorFlagsMustBeEnum;
  public const string Title = "Must be enum";
  public const string Message = "The validator flags type must be an enum";

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
