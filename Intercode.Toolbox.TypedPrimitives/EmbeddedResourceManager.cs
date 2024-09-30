// Module Name: EmbeddedResourceManager.cs
// Author:      Eduardo Velasquez
// Copyright (c) 2024, Intercode Consulting, Inc.

namespace Intercode.Toolbox.TypedPrimitives;

using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Text;

internal static class EmbeddedResourceManager
{
  #region Constants

  private static readonly Assembly s_thisAssembly = typeof( EmbeddedResourceManager ).Assembly;

  #endregion

  #region Public Methods

  public static bool DoesResourceExist(
    string resourceDirectory,
    string templateName )
  {
    var resourceName = GetResourceName( resourceDirectory, templateName );
    var info = s_thisAssembly.GetManifestResourceInfo( resourceName );
    return info is not null;
  }

  public static string LoadTemplate(
    string resourceDirectory,
    string templateName )
  {
    var resourceName = GetResourceName( resourceDirectory, templateName );
    if( !TryLoadTemplate( resourceName, out var template ) )
    {
#if DEBUG
      var existingResources = s_thisAssembly.GetManifestResourceNames();
      throw new ArgumentException(
        $"Could not find embedded resource '{resourceName}'. Available names: {string.Join( ", ", existingResources )}"
      );
#else
      throw new ArgumentException( $"Could not find embedded resource '{resourceName}'." );
#endif
    }

    return template;
  }

  public static bool TryLoadTemplate(
    string resourceName,
    [NotNullWhen( true )] out string? template )
  {
    var resourceStream = s_thisAssembly.GetManifestResourceStream( resourceName );
    if( resourceStream is null )
    {
      template = null;
      return false;
    }

    using var reader = new StreamReader( resourceStream, Encoding.UTF8 );
    template = reader.ReadToEnd();
    return true;
  }

  #endregion

  #region Implementation

  private static string GetResourceName(
    string resourceDirectory,
    string templateName )
  {
    return $"Intercode.Toolbox.TypedPrimitives.Templates.{resourceDirectory}.{templateName}.template";
  }

  #endregion
}
