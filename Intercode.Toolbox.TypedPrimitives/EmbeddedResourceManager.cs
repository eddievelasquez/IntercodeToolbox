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

  public static bool DoesTemplateExist(
    string resourceDirectory,
    string templateName )
  {
    var resourcePath = GetTemplateResourcePath( resourceDirectory, templateName );
    var info = s_thisAssembly.GetManifestResourceInfo( resourcePath );
    return info is not null;
  }

  public static string LoadTemplate(
    string resourceDirectory,
    string templateName )
  {
    var resourcePath = GetTemplateResourcePath( resourceDirectory, templateName );
    if( !TryLoadTextResource( resourcePath, out var template ) )
    {
#if DEBUG
      var existingResources = s_thisAssembly.GetManifestResourceNames();
      throw new ArgumentException(
        $"Could not find embedded resource '{resourcePath}'. Available names: {string.Join( ", ", existingResources )}"
      );
#else
      throw new ArgumentException( $"Could not find embedded resource '{resourcePath}'." );
#endif
    }

    return template;
  }

  public static string LoadTextResource(
    string resourceName,
    string? resourceDirectory = null )
  {
    var resourcePath = GetResourcePath( resourceName, resourceDirectory );
    if( !TryLoadTextResource( resourcePath, out var text ) )
    {
#if DEBUG
      var existingResources = s_thisAssembly.GetManifestResourceNames();
      throw new ArgumentException(
        $"Could not find embedded resource '{resourcePath}'. Available names: {string.Join( ", ", existingResources )}"
      );
#else
      throw new ArgumentException( $"Could not find embedded resource '{resourcePath}'." );
#endif
    }

    return text;
  }

  #endregion

  #region Implementation

  private static bool TryLoadTextResource(
    string resourcePath,
    [NotNullWhen( true )] out string? text )
  {
    var resourceStream = s_thisAssembly.GetManifestResourceStream( resourcePath );
    if( resourceStream is null )
    {
      text = null;
      return false;
    }

    using var reader = new StreamReader( resourceStream, Encoding.UTF8 );
    text = reader.ReadToEnd();
    return true;
  }

  private static string GetTemplateResourcePath(
    string resourceDirectory,
    string templateName )
  {
    return $"Intercode.Toolbox.TypedPrimitives.Templates.{resourceDirectory}.{templateName}.template";
  }

  private static string GetResourcePath(
    string resourceName,
    string? resourceDirectory = null )
  {
    if( resourceDirectory is not null )
    {
      return $"Intercode.Toolbox.TypedPrimitives.{resourceDirectory}.{resourceName}";
    }

    return $"Intercode.Toolbox.TypedPrimitives.{resourceName}";
  }

  #endregion
}
