// Module Name: StandardMacrosExtensions.cs
// Author:      Eduardo Velasquez
// Copyright (c) 2025, Intercode Consulting, Inc.

namespace Intercode.Toolbox.TemplateEngine;

using System.Diagnostics.CodeAnalysis;
using System.Globalization;

/// <summary>
///   Extension methods for adding standard macros
/// </summary>
[ExcludeFromCodeCoverage] // Not testable
public static class StandardMacrosExtensions
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

  /// <summary>
  ///   Adds standard macros to the <see cref="MacroProcessorContext" />.
  /// </summary>
  /// <param name="context">The <see cref="MacroProcessorContext" /> to add macros to.</param>
  /// <returns>The <see cref="MacroProcessorContext" /> with the added macros.</returns>
  /// <remarks>
  ///   <list type="table">
  ///     <listheader>
  ///       <term>Name</term>
  ///       <description>Description</description>
  ///     </listheader>
  ///     <item>
  ///       <term>NOW</term>
  ///       <description>
  ///         Gets the current local date and time. The optional argument is the format string passed to the
  ///         <see cref="DateTime.ToString(String)" /> method.
  ///       </description>
  ///     </item>
  ///     <item>
  ///       <term>UTC_NOW</term>
  ///       <description>
  ///         Gets the current UTC date and time. The optional argument is the format string passed to the
  ///         <see cref="DateTime.ToString(String)" /> method.
  ///       </description>
  ///     </item>
  ///     <item>
  ///       <term>GUID</term>
  ///       <description>
  ///         Generates a new Guid. The optional argument is the format string passed to the
  ///         <see cref="Guid.ToString(String)" /> method.
  ///       </description>
  ///     </item>
  ///     <item>
  ///       <term>MACHINE</term>
  ///       <description>
  ///         Gets the name of the local computer as returned by the <see cref="Environment.MachineName" />
  ///         property.
  ///       </description>
  ///     </item>
  ///     <item>
  ///       <term>OS</term>
  ///       <description>Gets the operating system version as returned by the <see cref="Environment.OSVersion" /> property.</description>
  ///     </item>
  ///     <item>
  ///       <term>USER</term>
  ///       <description>Gets the name of the current user as returned by the <see cref="Environment.UserName" /> property.</description>
  ///     </item>
  ///     <item>
  ///       <term>CLR_VERSION</term>
  ///       <description>Gets the CLR version as returned by the <see cref="Environment.Version" /> property.</description>
  ///     </item>
  ///     <item>
  ///       <term>ENV</term>
  ///       <description>
  ///         Gets the value of the environment variable specified by the argument as return by the
  ///         <see cref="Environment.GetEnvironmentVariable(String)" /> method.
  ///       </description>
  ///     </item>
  ///   </list>
  ///   <para>NOTE: If a generator throws an exception, the macro's value will be the exception's error message.</para>
  /// </remarks>
  public static MacroProcessorContext AddStandardMacros(
    this MacroProcessorContext context )
  {
    context.AddMacro( NowMacroName, NowGenerator );
    context.AddMacro( UtcNowMacroName, UtcNowGenerator );
    context.AddMacro( GuidMacroName, GuidGenerator );
    context.AddMacro( MachineMacroName, _ => Environment.MachineName );
    context.AddMacro( OsMacroName, _ => Environment.OSVersion.VersionString );
    context.AddMacro( UserMacroName, _ => Environment.UserName );
    context.AddMacro( ClrVersionMacroName, _ => Environment.Version.ToString() );
    context.AddMacro( EnvMacroName, EnvironmentVarGenerator );

    return context;
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
