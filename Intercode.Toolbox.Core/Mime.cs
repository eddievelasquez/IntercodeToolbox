// Module Name: Mime.cs
// Author:      Eduardo Velasquez
// Copyright (c) 2024, Intercode Consulting, Inc.

namespace Intercode.Toolbox.Core;

using Microsoft.AspNetCore.StaticFiles;

/// <summary>
///   Provides methods for working with MIME types.
/// </summary>
public static class Mime
{
  #region Constants

  /// <summary>
  ///   The default content type used when the content type cannot be determined.
  /// </summary>
  public const string DefaultContentType = "application/octet-stream";

  private static readonly FileExtensionContentTypeProvider s_provider = new ();

  #endregion

  #region Properties

  /// <inheritdoc cref="FileExtensionContentTypeProvider.Mappings" />
  public static IDictionary<string, string> Mappings => s_provider.Mappings;

  #endregion

  #region Public Methods

  /// <summary>
  ///   Gets the content type based on the file name.
  /// </summary>
  /// <param name="fileName">The name of the file.</param>
  /// <returns>The content type associated with the file name, or the default content type if it cannot be determined.</returns>
  public static string GetContentType(
    string fileName )
  {
    return s_provider.TryGetContentType( fileName, out var contentType ) ? contentType : DefaultContentType;
  }

  #endregion
}
