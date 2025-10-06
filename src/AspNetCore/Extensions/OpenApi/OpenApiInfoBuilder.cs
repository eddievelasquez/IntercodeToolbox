// Module Name: OpenApiInfoBuilder.cs
// Author:      Eduardo Velasquez
// Copyright (c) 2023, Intercode Consulting, Inc.

namespace Intercode.Toolbox.AspNetCore.Extensions.OpenApi;

using FluentResults;
using Microsoft.OpenApi.Extensions;
using Microsoft.OpenApi.Interfaces;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Validations;

/// <summary>
///   Implements the builder pattern for creating an <see cref="OpenApiInfo" /> object.
/// </summary>
public class OpenApiInfoBuilder
{
  #region Fields

  private readonly OpenApiInfo _info;

  #endregion

  #region Constructors

  /// <summary>
  ///   Initializes a new instance of the <see cref="OpenApiInfoBuilder" /> class with the specified title and version.
  /// </summary>
  /// <param name="title">The title of the OpenAPI info.</param>
  /// <param name="version">The version of the OpenAPI info.</param>
  /// <exception cref="ArgumentNullException">
  ///   Thrown if either <paramref name="title" /> or <paramref name="version" /> are
  ///   <c>null</c>.
  /// </exception>
  /// <exception cref="ArgumentException">
  ///   Thrown if either <paramref name="title" /> or <paramref name="version" /> are an
  ///   empty <c>string</c>.
  /// </exception>
  public OpenApiInfoBuilder(
    string title,
    string version )
  {
    ArgumentException.ThrowIfNullOrEmpty( title );
    ArgumentException.ThrowIfNullOrEmpty( version );

    _info = new OpenApiInfo
    {
      Title = title,
      Version = version
    };
  }

  #endregion

  #region Public Methods

  /// <summary>
  ///   Adds contact information to the OpenAPI info.
  /// </summary>
  /// <param name="name">The name of the contact.</param>
  /// <param name="email">The email of the contact.</param>
  /// <param name="url">[optional] The URL of the contact.</param>
  /// <param name="extensions">[optional] The extensions associated with the contact.</param>
  /// <returns>This <see cref="OpenApiInfoBuilder" /> instance.</returns>
  /// <remarks>
  ///   Will only create the <see cref="OpenApiInfo.Contact" /> object if at least one of the parameters is not <c>null</c>.
  /// </remarks>
  public OpenApiInfoBuilder AddContact(
    string? name,
    string? email,
    string? url = null,
    IDictionary<string, IOpenApiExtension>? extensions = null )
  {
    var uri = !string.IsNullOrWhiteSpace( url ) ? new Uri( url! ) : null;
    return AddContact( name, email, uri, extensions );
  }

  /// <summary>
  ///   Adds contact information to the OpenAPI info.
  /// </summary>
  /// <param name="name">The name of the contact.</param>
  /// <param name="email">The email of the contact.</param>
  /// <param name="uri">[optional] The URI of the contact.</param>
  /// <param name="extensions">[optional] The extensions associated with the contact.</param>
  /// <returns>This <see cref="OpenApiInfoBuilder" /> instance.</returns>
  /// <remarks>
  ///   Will only create the <see cref="OpenApiInfo.Contact" /> object if at least one of the parameters is not <c>null</c>.
  /// </remarks>
  public OpenApiInfoBuilder AddContact(
    string? name,
    string? email,
    Uri? uri = null,
    IDictionary<string, IOpenApiExtension>? extensions = null )
  {
    if( name != null || email != null || uri != null || extensions != null )
    {
      _info.Contact = new OpenApiContact
      {
        Name = name,
        Email = email,
        Url = uri,
        Extensions = extensions
      };
    }

    return this;
  }

  /// <summary>
  ///   Adds a description to the OpenAPI info.
  /// </summary>
  /// <param name="description">The description of the OpenAPI info.</param>
  /// <returns>This <see cref="OpenApiInfoBuilder" /> instance.</returns>
  public OpenApiInfoBuilder AddDescription(
    string description )
  {
    _info.Description = description;
    return this;
  }

  /// <summary>
  ///   Adds extensions to the OpenAPI info.
  /// </summary>
  /// <param name="extensions">The extensions to add.</param>
  /// <returns>This <see cref="OpenApiInfoBuilder" /> instance.</returns>
  public OpenApiInfoBuilder AddExtensions(
    IDictionary<string, IOpenApiExtension>? extensions )
  {
    if( extensions == null || extensions.Count == 0 )
    {
      return this;
    }

    _info.Extensions ??= new Dictionary<string, IOpenApiExtension>();

    foreach( var (key, value) in extensions )
    {
      _info.Extensions.Add( key, value );
    }

    return this;
  }

  /// <summary>
  ///   Adds an extension to the OpenAPI info.
  /// </summary>
  /// <param name="key">The key of the extension.</param>
  /// <param name="extension">The extension to add.</param>
  /// <returns>This <see cref="OpenApiInfoBuilder" /> instance.</returns>
  /// <exception cref="ArgumentNullException">
  ///   Thrown if either <paramref name="key" /> or <paramref name="extension" /> are
  ///   <c>null</c>.
  /// </exception>
  /// <exception cref="ArgumentException">
  ///   Thrown if <paramref name="key" /> is an empty <c>string</c>.
  /// </exception>
  public OpenApiInfoBuilder AddExtension(
    string key,
    IOpenApiExtension extension )
  {
    ArgumentException.ThrowIfNullOrEmpty( key );
    ArgumentNullException.ThrowIfNull( extension );

    _info.Extensions ??= new Dictionary<string, IOpenApiExtension>();
    _info.Extensions.Add( key, extension );

    return this;
  }

  /// <summary>
  ///   Adds a license to the OpenAPI info.
  /// </summary>
  /// <param name="name">The name of the license.</param>
  /// <param name="url">The URL of the license.</param>
  /// <param name="extensions">[optional] The extensions associated with the license.</param>
  /// <returns>This <see cref="OpenApiInfoBuilder" /> instance.</returns>
  /// <remarks>
  ///   Will only create the <see cref="OpenApiInfo.License" /> object if at least one of the parameters is not <c>null</c>.
  /// </remarks>
  public OpenApiInfoBuilder AddLicense(
    string? name,
    string? url,
    IDictionary<string, IOpenApiExtension>? extensions = null )
  {
    var uri = !string.IsNullOrWhiteSpace( url ) ? new Uri( url ) : null;
    return AddLicense( name, uri, extensions );
  }

  /// <summary>
  ///   Adds a license to the OpenAPI info.
  /// </summary>
  /// <param name="name">The name of the license.</param>
  /// <param name="url">The URL of the license.</param>
  /// <param name="extensions">The extensions associated with the license.</param>
  /// <returns>This <see cref="OpenApiInfoBuilder" /> instance.</returns>
  /// <remarks>
  ///   Will only create the <see cref="OpenApiInfo.License" /> object if at least one of the parameters is not <c>null</c>.
  /// </remarks>
  public OpenApiInfoBuilder AddLicense(
    string? name,
    Uri? url,
    IDictionary<string, IOpenApiExtension>? extensions = null )
  {
    if( name != null || url != null || extensions != null )
    {
      _info.License = new OpenApiLicense
      {
        Name = name,
        Url = url,
        Extensions = extensions
      };
    }

    return this;
  }

  /// <summary>
  ///   Adds the terms of service URL to the OpenAPI info.
  /// </summary>
  /// <param name="url">The URL of the terms of service.</param>
  /// <returns>This <see cref="OpenApiInfoBuilder" /> instance.</returns>
  public OpenApiInfoBuilder AddTermsOfService(
    string? url )
  {
    var uri = !string.IsNullOrWhiteSpace( url ) ? new Uri( url ) : null;
    return AddTermsOfService( uri );
  }

  /// <summary>
  ///   Adds the terms of service URL to the OpenAPI info.
  /// </summary>
  /// <param name="uri">The URI of the terms of service.</param>
  /// <returns>This <see cref="OpenApiInfoBuilder" /> instance.</returns>
  public OpenApiInfoBuilder AddTermsOfService(
    Uri? uri )
  {
    _info.TermsOfService = uri;
    return this;
  }

  /// <summary>
  ///   Builds the OpenAPI info object.
  /// </summary>
  /// <param name="ruleSet">The validation rule set to use.</param>
  /// <returns>
  ///   A <see cref="Result{T}" /> containing the built <see cref="OpenApiInfo" /> object if it is valid, otherwise a
  ///   failure result with one or more <see cref="OpenApiValidationResultError"/> objects.
  /// </returns>
  /// <remarks>
  ///   If the <paramref name="ruleSet" /> is <c>null</c>, the default rule set returned by
  ///   <see cref="ValidationRuleSet.GetDefaultRuleSet()" />  will be used.
  ///   Use <see cref="ValidationRuleSet.GetEmptyRuleSet()" /> if no validation should be performed.
  /// </remarks>
  public Result<OpenApiInfo> Build(
    ValidationRuleSet? ruleSet = null )
  {
    ruleSet ??= ValidationRuleSet.GetDefaultRuleSet();

    var errors = _info.Validate( ruleSet )
                      .ToList();

    if( errors.Count > 0 )
    {
      return Result.Fail( errors.Select( error => new OpenApiValidationResultError( error ) ) );
    }

    return _info;
  }

  #endregion
}
