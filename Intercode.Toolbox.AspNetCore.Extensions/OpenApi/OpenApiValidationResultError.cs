// Module Name: OpenApiValidationResultError.cs
// Author:      Eduardo Velasquez
// Copyright (c) 2024, Intercode Consulting, Inc.

namespace Intercode.Toolbox.AspNetCore.Extensions.OpenApi;

using FluentResults;
using Microsoft.OpenApi.Models;

/// <summary>
///   Represents an error that occurred during <see cref="OpenApiInfo" /> validation.
/// </summary>
public class OpenApiValidationResultError: IError
{
  #region Constructors

  /// <summary>
  ///   Initializes a new instance of the <see cref="OpenApiValidationResultError" /> class.
  /// </summary>
  /// <param name="error">The OpenAPI error.</param>
  public OpenApiValidationResultError(
    OpenApiError error )
  {
    Metadata["error"] = error;
    Message = error.Message;
  }

  #endregion

  #region Properties

  /// <summary>
  ///   Gets the error message.
  /// </summary>
  public string Message { get; }

  /// <summary>
  ///   Gets the metadata associated with the error.
  /// </summary>
  /// <remarks>The <see cref="OpenApiError" /> instance is added in the <b>error</b> key.</remarks>
  public Dictionary<string, object> Metadata { get; } = new ( StringComparer.OrdinalIgnoreCase );

  /// <summary>
  ///   Gets the list of reasons for the error.
  /// </summary>
  public List<IError> Reasons { get; } = new ();

  #endregion
}
