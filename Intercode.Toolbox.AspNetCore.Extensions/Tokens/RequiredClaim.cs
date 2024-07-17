// Module Name: RequiredClaim.cs
// Author:      Eduardo Velasquez
// Copyright (c) 2024, Intercode Consulting, Inc.

namespace Intercode.Toolbox.AspNetCore.Extensions.Tokens;

/// <summary>
///   Represents the set of required claims for a JSON Web Token.
/// </summary>
[Flags]
public enum RequiredClaim
{
  /// <summary>
  ///   No additional claims are required besides the subject and issuer claims.
  /// </summary>
  None = 0,

  /// <summary>
  ///   A subject claim must be provided.
  /// </summary>
  Subject = 1,

  /// <summary>
  ///   An issuer claim must be provided.
  /// </summary>
  Issuer = 2,

  /// <summary>
  ///   An audience claim must be provided.
  /// </summary>
  Audience = 4,

  /// <summary>
  ///   A role claim must be provided.
  /// </summary>
  Role = 8,

  /// <summary>
  ///   A claim for JTI (JSON Web Token ID) must be provided, if it's not explicitly set,
  ///   one will be automatically generated.
  /// </summary>
  Jti = 16,

  /// <summary>
  ///   Default required claims (Subject, Issuer, Audience, Role, JTI).
  /// </summary>
  Default = Subject | Issuer | Audience | Role | Jti
}
