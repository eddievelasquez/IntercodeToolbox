// Module Name: StandardMacros.cs
// Author:      Eduardo Velasquez
// Copyright (c) 2025, Intercode Consulting, Inc.

namespace Intercode.Toolbox.TemplateEngine;

using System.Diagnostics.CodeAnalysis;
using System.Globalization;

/// <summary>
///   Extension methods for adding standard macros
/// </summary>
[ExcludeFromCodeCoverage] // Not testable
internal static class StandardMacros
{
  #region Constants

  /// <summary>
  ///   The macro name for the current local date and time.
  /// </summary>
  public const string NowMacroName = "NOW";

  /// <summary>
  ///   The macro name for the current UTC date and time.
  /// </summary>
  public const string UtcNowMacroName = "UTC_NOW";

  /// <summary>
  ///   The macro name for generating a new Guid.
  /// </summary>
  public const string GuidMacroName = "GUID";

  /// <summary>
  ///   The macro name for the local computer name.
  /// </summary>
  public const string MachineMacroName = "MACHINE";

  /// <summary>
  ///   The macro name for the operating system version.
  /// </summary>
  public const string OsMacroName = "OS";

  /// <summary>
  ///   The macro name for the current user name.
  /// </summary>
  public const string UserMacroName = "USER";

  /// <summary>
  ///   The macro name for the CLR version.
  /// </summary>
  public const string ClrVersionMacroName = "CLR_VERSION";

  /// <summary>
  ///   The macro name for environment variable values.
  /// </summary>
  public const string EnvMacroName = "ENV";

  #endregion

  #region Public Methods

  [ExcludeFromCodeCoverage]
  public static IEnumerable<string> GetStandardMacroNames()
  {
    // NOTE:Must always return names in the same order as <see cref="GetStandardMacroGenerators" />
    yield return NowMacroName;
    yield return UtcNowMacroName;
    yield return GuidMacroName;
    yield return MachineMacroName;
    yield return OsMacroName;
    yield return UserMacroName;
    yield return ClrVersionMacroName;
    yield return EnvMacroName;
  }

  [ExcludeFromCodeCoverage]
  public static IEnumerable<MacroValueGenerator> GetStandardMacroGenerators()
  {
    // Note: Must always return generators in the same order as <see cref="GetStandardMacroNames" />
    yield return NowGenerator;
    yield return UtcNowGenerator;
    yield return GuidGenerator;
    yield return _ => Environment.MachineName;
    yield return _ => Environment.OSVersion.VersionString;
    yield return _ => Environment.UserName;
    yield return _ => Environment.Version.ToString();
    yield return EnvironmentVarGenerator;
  }

  #endregion

  #region Implementation

  private static string NowGenerator(
    ReadOnlySpan<char> arg )
  {
    var now = DateTime.Now;
    return arg.IsEmpty ? now.ToString( CultureInfo.CurrentCulture ) : now.ToString( arg.ToString() );
  }

  private static string UtcNowGenerator(
    ReadOnlySpan<char> arg )
  {
    var utcNow = DateTime.UtcNow;
    return arg.IsEmpty ? utcNow.ToString( CultureInfo.CurrentCulture ) : utcNow.ToString( arg.ToString() );
  }

  private static string GuidGenerator(
    ReadOnlySpan<char> arg )
  {
    return arg.IsEmpty ? Guid.NewGuid().ToString() : Guid.NewGuid().ToString( arg.ToString() );
  }

  private static string EnvironmentVarGenerator(
    ReadOnlySpan<char> arg )
  {
    return Environment.GetEnvironmentVariable( arg.ToString() ) ?? string.Empty;
  }

  #endregion
}
