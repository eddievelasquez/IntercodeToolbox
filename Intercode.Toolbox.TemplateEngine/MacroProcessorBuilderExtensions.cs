// Module Name: MacroProcessorBuilderExtensions.cs
// Author:      Eduardo Velasquez
// Copyright (c) 2024, Intercode Consulting, Inc.

namespace Intercode.Toolbox.TemplateEngine;

using System.Globalization;

/// <summary>
///   Extension methods for the MacroProcessorBuilder class
/// </summary>
public static class MacroProcessorBuilderExtensions
{
  #region Public Methods

  /// <summary>
  ///   Adds standard macros to the <see cref="MacroProcessorBuilder" />.
  /// </summary>
  /// <param name="builder">The <see cref="MacroProcessorBuilder" /> to add macros to.</param>
  /// <returns>The <see cref="MacroProcessorBuilder" /> with the added macros.</returns>
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
  public static MacroProcessorBuilder AddStandardMacros(
    this MacroProcessorBuilder builder )
  {
    return builder
           .AddMacro( "NOW", NowGenerator )
           .AddMacro( "UTC_NOW", UtcNowGenerator )
           .AddMacro( "GUID", GuidGenerator )
           .AddMacro( "MACHINE", _ => Environment.MachineName )
           .AddMacro( "OS", _ => Environment.OSVersion.VersionString )
           .AddMacro( "USER", _ => Environment.UserName )
           .AddMacro( "CLR_VERSION", _ => Environment.Version.ToString() )
           .AddMacro( "ENV", EnvironmentVarGenerator );
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
