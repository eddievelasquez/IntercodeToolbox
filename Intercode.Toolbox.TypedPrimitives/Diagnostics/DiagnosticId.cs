// Module Name: DiagnosticId.cs
// Author:      Eduardo Velasquez
// Copyright (c) 2024, Intercode Consulting, Inc.

namespace Intercode.Toolbox.TypedPrimitives.Diagnostics;

internal static class DiagnosticId
{
  #region Constants

  public const string Prefix = "TPR";
  public const string NotPartial = $"{Prefix}001";
  public const string NotSupported = $"{Prefix}002";
  public const string ValidatorFlagsMustBeEnum = $"{Prefix}003";
  public const string MissingPrimitiveType = $"{Prefix}004";
  public const string Unexpected = $"{Prefix}005";
  public const string UnsupportedPrimitiveTypeDiagnostic = $"{Prefix}006";

  #endregion
}
