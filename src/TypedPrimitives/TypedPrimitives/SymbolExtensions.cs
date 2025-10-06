// Module Name: SymbolExtensions.cs
// Author:      Eduardo Velasquez
// Copyright (c) 2024, Intercode Consulting, Inc.

namespace Intercode.Toolbox.TypedPrimitives;

using Microsoft.CodeAnalysis;

internal static class SymbolExtensions
{
  #region Constants

  private static readonly SymbolDisplayFormat s_format = new (
    SymbolDisplayGlobalNamespaceStyle.OmittedAsContaining,
    SymbolDisplayTypeQualificationStyle.NameAndContainingTypesAndNamespaces,
    SymbolDisplayGenericsOptions.IncludeTypeParameters,
    miscellaneousOptions: SymbolDisplayMiscellaneousOptions.UseSpecialTypes
  );

  #endregion

  #region Public Methods

  public static string QualifiedName(
    this ISymbol symbol )
  {
    return symbol.ToDisplayString( s_format );
  }

  #endregion
}
