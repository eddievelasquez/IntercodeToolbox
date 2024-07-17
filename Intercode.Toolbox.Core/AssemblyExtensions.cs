// Module Name: AssemblyExtensions.cs
// Author:      Eduardo Velasquez
// Copyright (c) 2024, Intercode Consulting, Inc.

namespace Intercode.Toolbox.Core;

using System.Reflection;

/// <summary>
///   Provides extension methods for working with assemblies.
/// </summary>
public static class AssemblyExtensions
{
  #region Public Methods

  /// <summary>
  ///   Gets the version string of the assembly.
  /// </summary>
  /// <param name="assembly">The assembly.</param>
  /// <returns>The version string of the assembly.</returns>
  /// <remarks>
  ///   Will return the assembly's informational version if the <see cref="AssemblyInformationalVersionAttribute" />
  ///   if found.
  /// </remarks>
  public static string? GetVersionString(
    this Assembly? assembly )
  {
    if( assembly == null )
    {
      return null;
    }

    if( assembly.GetCustomAttributes( typeof( AssemblyInformationalVersionAttribute ), false )
                .FirstOrDefault() is AssemblyInformationalVersionAttribute infoVersion )
    {
      return infoVersion.InformationalVersion;
    }

    var assemblyName = assembly.GetName();
    return assemblyName.Version?.ToString();
  }

  #endregion
}
