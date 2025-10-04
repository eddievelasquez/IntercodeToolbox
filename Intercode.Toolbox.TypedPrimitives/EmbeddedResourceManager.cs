// Module Name: EmbeddedResourceManager.cs
// Author:      Eduardo Velasquez
// Copyright (c) 2024, Intercode Consulting, Inc.

namespace Intercode.Toolbox.TypedPrimitives;

using System.Collections.Frozen;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Text;

internal static class EmbeddedResourceManager
{
  #region Constants

  private const string RESOURCE_BASE_PATH = "Intercode.Toolbox.TypedPrimitives";
  private const string TEMPLATE_RESOURCE_BASE_PATH = $"{RESOURCE_BASE_PATH}.Templates";

  private static readonly Assembly s_thisAssembly = typeof( EmbeddedResourceManager ).Assembly;
  private static readonly FrozenSet<string> s_templateResourcePaths;

  #endregion

  #region Constructors

  static EmbeddedResourceManager()
  {
    s_templateResourcePaths = s_thisAssembly.GetManifestResourceNames()
                                            .Where( name => name.StartsWith( RESOURCE_BASE_PATH ) )
                                            .ToFrozenSet();
  }

  #endregion

  #region Public Methods

  public static string LoadTemplateResource(
    string resourcePath )
  {
    if( !TryLoadTextResource( resourcePath, out var template ) )
    {
      ThrowResourceNotFound( resourcePath );
    }

    return template;
  }

  public static string LoadTextResource(
    string resourceName,
    string? resourceDirectory = null )
  {
    var resourcePath = resourceDirectory is null
      ? $"{RESOURCE_BASE_PATH}.{resourceName}"
      : $"{RESOURCE_BASE_PATH}.{resourceDirectory}.{resourceName}";

    if( !TryLoadTextResource( resourcePath, out var text ) )
    {
      ThrowResourceNotFound( resourcePath );
    }

    return text;
  }

  public static bool TemplateExists(
    string resourceDirectory,
    string templateName )
  {
    var resourcePath = GetTemplateResourcePath( resourceDirectory, templateName );
    return s_templateResourcePaths.Contains( resourcePath );
  }

  public static string GetTemplateResourcePath(
    string resourceDirectory,
    string templateName )
  {
    return $"{TEMPLATE_RESOURCE_BASE_PATH}.{resourceDirectory}.{templateName}.template";
  }

  #endregion

  #region Implementation

  private static bool TryLoadTextResource(
    string resourcePath,
    [NotNullWhen( true )] out string? text )
  {
    // If the resource path is not in the known set, avoid attempting to load it
    if( !s_templateResourcePaths.Contains( resourcePath ) )
    {
      text = null;
      return false;
    }

    var resourceStream = s_thisAssembly.GetManifestResourceStream( resourcePath );

    // The resource doesn't exist or is not visible to the caller
    if( resourceStream is null )
    {
      text = null;
      return false;
    }

    using var reader = new StreamReader( resourceStream, Encoding.UTF8 );
    text = reader.ReadToEnd();
    return true;
  }

  [DoesNotReturn]
  private static void ThrowResourceNotFound(
    string resourcePath )
  {
#if DEBUG
    throw new InvalidOperationException(
      $"Could not find embedded resource '{resourcePath}'.\r\nAvailable names: {string.Join( ", ", s_templateResourcePaths )}"
    );
#else
    throw new InvalidOperationException( $"Could not find embedded resource '{resourcePath}'." );
#endif
  }

  #endregion
}
