// Module Name: EmbeddedResource.cs
// Author:      Eduardo Velasquez
// Copyright (c) 2024, Intercode Consulting, Inc.

namespace Intercode.Toolbox.Core.IO;

using System.Reflection;
using Microsoft.Extensions.FileProviders;

/// <summary>
///   Provides utility methods for loading embedded resources.
/// </summary>
public static class EmbeddedResource
{
  #region Public Methods

  /// <summary>
  ///   Loads the content of an embedded resource as a byte array.
  /// </summary>
  /// <param name="fileName">The name of the embedded resource file.</param>
  /// <param name="assembly">
  ///   The assembly containing the embedded resource. If not specified, the calling assembly will be used.
  /// </param>
  /// <param name="resourceRootName">
  ///   The root name of the embedded resource. If not specified, the default root name "Resources" will be used.
  /// </param>
  /// <returns>The content of the embedded resource as a byte array.</returns>
  /// <exception cref="FileNotFoundException">If the file was not found in the assembly's resources.</exception>
  /// <remarks>
  ///   The assembly that contains the resources must reference the
  ///   <see href="https://www.nuget.org/packages/Microsoft.Extensions.FileProviders.Embedded">Microsoft.Extensions.FileProviders.Embedded</see>
  ///   nuget package.
  /// </remarks>
  public static byte[] LoadBytesFromResource(
    string fileName,
    Assembly? assembly = null,
    string? resourceRootName = null )
  {
    ArgumentException.ThrowIfNullOrEmpty( fileName );

    // If not specified, ensure that the caller's assembly is used
    assembly ??= Assembly.GetCallingAssembly();
    resourceRootName ??= "Resources";

    var provider = new ManifestEmbeddedFileProvider(
      assembly,
      resourceRootName
    );

    using var source = provider.GetFileInfo( fileName )
                               .CreateReadStream();

    using var destination = new MemoryStream();
    source.CopyTo( destination );

    return destination.ToArray();
  }

  /// <summary>
  ///   Loads the content of an embedded resource as a stream along with its content type.
  /// </summary>
  /// <param name="fileName">The name of the embedded resource file.</param>
  /// <param name="assembly">
  ///   The assembly containing the embedded resource. If not specified, the calling assembly will be used.
  /// </param>
  /// <param name="resourceRootName">
  ///   The root name of the embedded resource. If not specified, the default root name "Resources" will be used.
  /// </param>
  /// <returns>A tuple containing the content of the embedded resource as a stream and its content type.</returns>
  /// <exception cref="FileNotFoundException">If the file was not found in the assembly's resources.</exception>
  /// <remarks>
  ///   The caller is responsible for disposing of the content stream.
  ///   The assembly that contains the resources must reference the
  ///   <see href="https://www.nuget.org/packages/Microsoft.Extensions.FileProviders.Embedded">Microsoft.Extensions.FileProviders.Embedded</see>
  ///   nuget package.
  /// </remarks>
  public static (Stream Content, string ContentType) LoadFromResource(
    string fileName,
    Assembly? assembly = null,
    string? resourceRootName = null )
  {
    // If not specified, ensure that the caller's assembly is used
    assembly ??= Assembly.GetCallingAssembly();

    var resourceData = LoadBytesFromResource( fileName, assembly, resourceRootName );

    // It's the responsibility of the caller to dispose the content stream.
    var content = new MemoryStream( resourceData );
    return ( content, Mime.GetContentType( fileName ) );
  }

  #endregion
}
