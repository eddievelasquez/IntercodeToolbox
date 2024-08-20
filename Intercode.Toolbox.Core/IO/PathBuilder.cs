// Module Name: PathBuilder.cs
// Author:      Eduardo Velasquez
// Copyright (c) 2024, Intercode Consulting, Inc.

namespace Intercode.Toolbox.Core.IO;

/// <summary>
///   Provides a fluent API for building file paths.
/// </summary>
public class PathBuilder
{
  #region Fields

  private string? _directory;
  private string? _filename;
  private string? _extension;

  #endregion

  #region Constructors

  /// <summary>
  ///   Initializes a new instance of the <see cref="PathBuilder" /> class with the specified path.
  /// </summary>
  /// <param name="path">The path to initialize the <see cref="PathBuilder" /> with.</param>
  public PathBuilder(
    string? path )
  {
    _directory = Path.GetDirectoryName( path );
    _filename = Path.GetFileNameWithoutExtension( path );
    _extension = Path.GetExtension( path );
  }

  /// <summary>
  ///   Initializes a new instance of the <see cref="PathBuilder" /> class with the specified <see cref="FileInfo" />.
  /// </summary>
  /// <param name="fileInfo">The <see cref="FileInfo" /> to initialize the <see cref="PathBuilder" /> with.</param>
  public PathBuilder(
    FileInfo fileInfo )
  {
    _directory = fileInfo.DirectoryName;
    _filename = Path.GetFileNameWithoutExtension( fileInfo.Name );
    _extension = Path.GetExtension( fileInfo.Name );
  }

  /// <summary>
  ///   Initializes a new instance of the <see cref="PathBuilder" /> class with the specified <see cref="DirectoryInfo" />.
  /// </summary>
  /// <param name="directoryInfo">The <see cref="DirectoryInfo" /> to initialize the <see cref="PathBuilder" /> with.</param>
  public PathBuilder(
    DirectoryInfo directoryInfo )
  {
    _directory = directoryInfo.Parent?.FullName;
    _filename = Path.GetFileNameWithoutExtension( directoryInfo.Name );
    _extension = Path.GetExtension( directoryInfo.Name );
  }

  #endregion

  #region Public Methods

  /// <summary>
  ///   Sets the directory of the path.
  /// </summary>
  /// <param name="directory">The directory to set.</param>
  /// <returns>The <see cref="PathBuilder" /> instance.</returns>
  public PathBuilder SetDirectory(
    string? directory )
  {
    _directory = directory;
    return this;
  }

  /// <summary>
  ///   Sets the directory of the path.
  /// </summary>
  /// <param name="directoryInfo">The <see cref="DirectoryInfo" /> to set as the directory.</param>
  /// <returns>The <see cref="PathBuilder" /> instance.</returns>
  public PathBuilder SetDirectory(
    DirectoryInfo? directoryInfo )
  {
    _directory = directoryInfo?.FullName;
    return this;
  }

  /// <summary>
  ///   Changes the directory of the path using the specified function.
  /// </summary>
  /// <param name="directoryFunc">The function to change the path's current directory name.</param>
  /// <returns>The <see cref="PathBuilder" /> instance.</returns>
  public PathBuilder ChangeDirectory(
    Func<string?, string?> directoryFunc )
  {
    ArgumentNullException.ThrowIfNull( directoryFunc );

    _directory = directoryFunc( _directory );
    return this;
  }

  /// <summary>
  ///   Appends a directory to the path.
  /// </summary>
  /// <param name="directory">The directory to append.</param>
  /// <returns>The <see cref="PathBuilder" /> instance.</returns>
  public PathBuilder AppendDirectory(
    string? directory )
  {
    _directory = Path.Join( _directory, directory );
    return this;
  }

  /// <summary>
  ///   Appends multiple directories to the path.
  /// </summary>
  /// <param name="directories">The directories to append.</param>
  /// <returns>The <see cref="PathBuilder" /> instance.</returns>
  public PathBuilder AppendDirectories(
    params string?[] directories )
  {
    _directory = Path.Join( [_directory, ..directories] );
    return this;
  }

  /// <summary>
  ///   Sets the filename of the path.
  /// </summary>
  /// <param name="filename">The filename to set.</param>
  /// <returns>The <see cref="PathBuilder" /> instance.</returns>
  public PathBuilder SetFilename(
    string? filename )
  {
    _filename = filename;
    return this;
  }

  /// <summary>
  ///   Changes the filename of the path using the specified function.
  /// </summary>
  /// <param name="filenameFunc">The function to change the filename.</param>
  /// <returns>The <see cref="PathBuilder" /> instance.</returns>
  public PathBuilder ChangeFilename(
    Func<string?, string?> filenameFunc )
  {
    ArgumentNullException.ThrowIfNull( filenameFunc );

    _filename = filenameFunc( _filename );
    return this;
  }

  /// <summary>
  ///   Sets the extension of the path.
  /// </summary>
  /// <param name="extension">The extension to set.</param>
  /// <returns>The <see cref="PathBuilder" /> instance.</returns>
  public PathBuilder SetExtension(
    string? extension )
  {
    _extension = extension;
    return this;
  }

  /// <summary>
  ///   Changes the extension of the path using the specified function.
  /// </summary>
  /// <param name="extensionFunc">The function to change the extension.</param>
  /// <returns>The <see cref="PathBuilder" /> instance.</returns>
  public PathBuilder ChangeExtension(
    Func<string?, string?> extensionFunc )
  {
    ArgumentNullException.ThrowIfNull( extensionFunc );

    _extension = extensionFunc( _extension! );
    return this;
  }

  /// <summary>
  ///   Builds the path.
  /// </summary>
  /// <returns>The built path.</returns>
  public string Build()
  {
    var path = Path.Join(
      _directory,
      _filename
    );

    if( !string.IsNullOrEmpty( _extension ) )
    {
      path = Path.ChangeExtension( path, _extension );
    }

    return path;
  }

  #endregion
}
