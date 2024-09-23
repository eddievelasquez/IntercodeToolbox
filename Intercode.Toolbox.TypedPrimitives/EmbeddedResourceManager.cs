// Module Name: EmbeddedResourceManager.cs
// Author:      Eduardo Velasquez
// Copyright (c) 2024, Intercode Consulting, Inc.

namespace Intercode.Toolbox.TypedPrimitives;

using System.Reflection;
using System.Text;

internal static class EmbeddedResourceManager
{
  #region Constants

  private static readonly Assembly ThisAssembly = typeof( EmbeddedResourceManager ).Assembly;

  #endregion

  #region Public Methods

  public static bool DoesResourceExist(
    string resourceDirectory,
    string templateName )
  {
    var resourceName = GetResourceName( resourceDirectory, templateName );
    var info = ThisAssembly.GetManifestResourceInfo( resourceName );
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
      var existingResources = ThisAssembly.GetManifestResourceNames();
      throw new ArgumentException(
        $"Could not find embedded resource {resourceName}. Available names: {string.Join( ", ", existingResources )}"
      );
#else
      throw new ArgumentException( $"Could not find embedded resource {resourceName}." );
#endif
    }

    return template;
  }

  public static bool TryLoadTemplate(
    string resourceDirectory,
    string templateName,
    out string template )
  {
    var resourceName = GetResourceName( resourceDirectory, templateName );
    return TryLoadTemplate( resourceName, out template );
  }

  public static bool TryLoadTemplate(
    string resourceName,
    out string template )
  {
    var resourceStream = ThisAssembly.GetManifestResourceStream( resourceName );
    if( resourceStream is null )
    {
      template = string.Empty;
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
