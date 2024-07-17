// Module Name: UriBuilderExtensions.cs
// Author:      Eduardo Velasquez
// Copyright (c) 2024, Intercode Consulting, Inc.

namespace Intercode.Toolbox.Core;

using System.Net;

/// <summary>
///   Provides extension methods for <see cref="UriBuilder" /> instances.
/// </summary>
public static class UriBuilderExtensions
{
  #region Public Methods

  /// <summary>
  ///   Appends a path segment to the URI builder's path.
  /// </summary>
  /// <param name="builder">The URI builder.</param>
  /// <param name="path">The path segment to append.</param>
  /// <returns>The updated URI builder.</returns>
  /// <remarks>Ensures that <paramref name="path" /> is properly encoded if necessary.</remarks>
  public static UriBuilder AppendPath(
    this UriBuilder builder,
    string? path )
  {
    ArgumentNullException.ThrowIfNull( builder );

    if( string.IsNullOrEmpty( path ) )
    {
      return builder;
    }

    builder.Path = Path.Combine( builder.Path, WebUtility.UrlEncode( path ) );
    return builder;
  }

  /// <summary>
  ///   Appends a query parameter to the URI builder's query string.
  /// </summary>
  /// <param name="builder">The URI builder.</param>
  /// <param name="key">The key of the query parameter.</param>
  /// <returns>The updated URI builder.</returns>
  /// <remarks>Ensures that <paramref name="key" /> is properly encoded if necessary.</remarks>
  public static UriBuilder AppendQuery(
    this UriBuilder builder,
    string? key )
  {
    ArgumentNullException.ThrowIfNull( builder );

    if( string.IsNullOrEmpty( key ) )
    {
      return builder;
    }

    if( string.IsNullOrEmpty( builder.Query ) )
    {
      builder.Query = $"{WebUtility.UrlEncode( key )}";
      return builder;
    }

    builder.Query = $"{builder.Query}&{WebUtility.UrlEncode( key )}";
    return builder;
  }

  /// <summary>
  ///   Appends a query parameter with a value to the URI builder's query string.
  /// </summary>
  /// <typeparam name="T">The type of the value.</typeparam>
  /// <param name="builder">The URI builder.</param>
  /// <param name="key">The key of the query parameter.</param>
  /// <param name="value">The value of the query parameter.</param>
  /// <returns>The updated URI builder.</returns>
  /// <remarks>Ensures that <paramref name="key" /> and <paramref name="value" /> are properly encoded if necessary.</remarks>
  public static UriBuilder AppendQuery<T>(
    this UriBuilder builder,
    string? key,
    T value )
  {
    ArgumentNullException.ThrowIfNull( builder );

    if( string.IsNullOrEmpty( key ) )
    {
      return builder;
    }

    if( string.IsNullOrEmpty( builder.Query ) )
    {
      builder.Query = $"{WebUtility.UrlEncode( key )}={WebUtility.UrlEncode( value?.ToString() )}";
      return builder;
    }

    builder.Query = $"{builder.Query}&{WebUtility.UrlEncode( key )}={WebUtility.UrlEncode( value?.ToString() )}";
    return builder;
  }

  #endregion
}
