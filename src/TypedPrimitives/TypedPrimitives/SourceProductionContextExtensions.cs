// Module Name: SourceProductionContextExtensions.cs
// Author:      Eduardo Velasquez
// Copyright (c) 2024, Intercode Consulting, Inc.

namespace Intercode.Toolbox.TypedPrimitives;

using Intercode.Toolbox.TypedPrimitives.Diagnostics;
using Microsoft.CodeAnalysis;

internal static class SourceProductionContextExtensions
{
  #region Public Methods

  public static void ReportDiagnostic(
    this SourceProductionContext context,
    DiagnosticInfo diagnosticInfo )
  {
    context.ReportDiagnostic( diagnosticInfo.ToDiagnostic() );
  }

  #endregion
}
