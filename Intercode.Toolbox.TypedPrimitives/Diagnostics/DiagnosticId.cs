// Module Name: DiagnosticId.cs
// Author:      Eduardo Velasquez
// Copyright (c) 2024, Intercode Consulting, Inc.

namespace Intercode.Toolbox.TypedPrimitives.Diagnostics;

internal static class DiagnosticId
{
  #region Constants

  public const string Prefix = "TP";
  public const string NotPartial = $"{Prefix}0001";
  public const string NotReadOnly = $"{Prefix}0002";
  public const string MissingPrimitiveType = $"{Prefix}0003";
  public const string Unexpected = $"{Prefix}0004";
  public const string UnsupportedPrimitiveTypeDiagnostic = $"{Prefix}0005";

  #endregion
}
