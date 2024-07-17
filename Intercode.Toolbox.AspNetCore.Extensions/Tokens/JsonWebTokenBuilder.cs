// Module Name: JsonWebTokenBuilder.cs
// Author:      Eduardo Velasquez
// Copyright (c) 2024, Intercode Consulting, Inc.

namespace Intercode.Toolbox.AspNetCore.Extensions.Tokens;

using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using FluentResults;
using Intercode.Toolbox.Collections;
using Microsoft.IdentityModel.Tokens;

/// <summary>
///   Builds JSON Web Tokens (JWTs) with a fluent interface.
/// </summary>
public class JsonWebTokenBuilder
{
  #region Constants

  private static readonly Dictionary<RequiredClaim, string> REQUIRED_CLAIM_NAMES = new ()
  {
    { RequiredClaim.Subject, JwtRegisteredClaimNames.Sub },
    { RequiredClaim.Issuer, JwtRegisteredClaimNames.Iss },
    { RequiredClaim.Audience, JwtRegisteredClaimNames.Aud },
    { RequiredClaim.Role, ClaimTypes.Role },
    { RequiredClaim.Jti, JwtRegisteredClaimNames.Jti }
  };

  #endregion

  #region Fields

  private readonly SigningCredentials _signingCredentials;
  private readonly MutableHashSetLookup<string, object?> _claims = new ( StringComparer.OrdinalIgnoreCase );
  private readonly TimeProvider _timeProvider;
  private DateTime? _issuedAt;
  private DateTime? _validTo;
  private DateTime? _validFrom;
  private string? _compressionAlgorithm;
  private EncryptingCredentials? _encryptingCredentials;
  private TimeSpan _lifeTime = TimeSpan.FromMinutes( 30 );

  #endregion

  #region Constructors

  /// <summary>
  ///   Initializes a new instance of the <see cref="JsonWebTokenBuilder" /> class with a signing key and optional signature
  ///   algorithm.
  /// </summary>
  /// <param name="signingKey">The key used for signing the token.</param>
  /// <param name="signatureAlgorithm">
  ///   The algorithm used for signing. If <c>null</c>,
  ///   <see cref="DefaultSignatureAlgorithm" /> is used.
  /// </param>
  /// <param name="timeProvider">
  ///   The time provider used for token timestamps. If <c>null</c>,
  ///   <see cref="System.TimeProvider" /> is used.
  /// </param>
  public JsonWebTokenBuilder(
    string signingKey,
    string? signatureAlgorithm = null,
    TimeProvider? timeProvider = null )
    : this(
      CreateSigningCredentials( signingKey, signatureAlgorithm ?? DefaultSignatureAlgorithm ),
      timeProvider
    )
  {
  }

  /// <summary>
  ///   Initializes a new instance of the <see cref="JsonWebTokenBuilder" /> class with signing credentials.
  /// </summary>
  /// <param name="signingCredentials">The credentials used for signing the token.</param>
  /// <param name="timeProvider">
  ///   The time provider used for token timestamps. If <c>null</c>,
  ///   <see cref="System.TimeProvider" /> is used.
  /// </param>
  public JsonWebTokenBuilder(
    SigningCredentials signingCredentials,
    TimeProvider? timeProvider = null )
  {
    ArgumentNullException.ThrowIfNull( signingCredentials );

    _signingCredentials = signingCredentials;
    _timeProvider = timeProvider ?? TimeProvider.System;
  }

  #endregion

  #region Properties

  /// <summary>
  ///   Gets or sets the default signature algorithm used for token signing.
  /// </summary>
  public static string DefaultSignatureAlgorithm { get; set; } = SecurityAlgorithms.HmacSha512Signature;

  #endregion

  #region Public Methods

  /// <summary>
  ///   Sets the unique identifier (jti) claim for the token.
  /// </summary>
  /// <param name="tokenId">The unique identifier for the token.</param>
  /// <returns>The current <see cref="JsonWebTokenBuilder" /> instance.</returns>
  /// <exception cref="ArgumentNullException">If <paramref name="tokenId" /> is <c>null</c>.</exception>
  /// <exception cref="ArgumentException">If <paramref name="tokenId" /> is empty.</exception>
  public JsonWebTokenBuilder SetTokenId(
    string tokenId )
  {
    return AddClaim( JwtRegisteredClaimNames.Jti, tokenId );
  }

  /// <summary>
  ///   Sets the issuer (iss) claim of the token.
  /// </summary>
  /// <param name="issuer">The issuer of the token.</param>
  /// <returns>The current <see cref="JsonWebTokenBuilder" /> instance.</returns>
  /// <exception cref="ArgumentNullException">If <paramref name="issuer" /> is <c>null</c>.</exception>
  /// <exception cref="ArgumentException">If <paramref name="issuer" /> is empty.</exception>
  public JsonWebTokenBuilder SetIssuer(
    string issuer )
  {
    return AddClaim( JwtRegisteredClaimNames.Iss, issuer );
  }

  /// <summary>
  ///   Sets the issued at time (iat) claim of the token.
  /// </summary>
  /// <param name="issuedAt">The time at which the token was issued.</param>
  /// <returns>The current <see cref="JsonWebTokenBuilder" /> instance.</returns>
  /// <remarks><paramref name="issuedAt" /> is converted to UTC.</remarks>
  public JsonWebTokenBuilder SetIssuedAt(
    DateTime? issuedAt )
  {
    _issuedAt = issuedAt?.ToUniversalTime();
    return this;
  }

  /// <summary>
  ///   Sets the issued at time (iat) claim of the token.
  /// </summary>
  /// <param name="issuedAt">The time at which the token was issued.</param>
  /// <returns>The current <see cref="JsonWebTokenBuilder" /> instance.</returns>
  /// <remarks><paramref name="issuedAt" /> is converted to UTC.</remarks>
  public JsonWebTokenBuilder SetIssuedAt(
    DateTimeOffset? issuedAt )
  {
    _issuedAt = issuedAt?.UtcDateTime;
    return this;
  }

  /// <summary>
  ///   Sets the expiration time (exp) claim of the token.
  /// </summary>
  /// <param name="validTo">The expiration time of the token.</param>
  /// <returns>The current <see cref="JsonWebTokenBuilder" /> instance.</returns>
  /// <remarks><paramref name="validTo" /> is converted to UTC.</remarks>
  public JsonWebTokenBuilder SetValidTo(
    DateTime? validTo )
  {
    _validTo = validTo?.ToUniversalTime();
    return this;
  }

  /// <summary>
  ///   Sets the expiration time (exp) claim of the token.
  /// </summary>
  /// <param name="validTo">The expiration time of the token.</param>
  /// <returns>The current <see cref="JsonWebTokenBuilder" /> instance.</returns>
  /// <remarks><paramref name="validTo" /> is converted to UTC.</remarks>
  public JsonWebTokenBuilder SetValidTo(
    DateTimeOffset? validTo )
  {
    _validTo = validTo?.UtcDateTime;
    return this;
  }

  /// <summary>
  ///   Sets the lifetime of the token. The expiration time (exp) claim is calculated as the sum of the current time and the
  ///   lifetime.
  /// </summary>
  /// <param name="lifeTime">The lifetime of the token.</param>
  /// <returns>The current <see cref="JsonWebTokenBuilder" /> instance.</returns>
  /// <exception cref="ArgumentOutOfRangeException">Thrown when the lifetime is set to zero.</exception>
  public JsonWebTokenBuilder SetLifeTime(
    TimeSpan lifeTime )
  {
    _lifeTime = lifeTime;
    return this;
  }

  /// <summary>
  ///   Sets the time before which the token is not valid.
  /// </summary>
  /// <param name="validFrom">The time before which the token is not valid.</param>
  /// <returns>The current <see cref="JsonWebTokenBuilder" /> instance.</returns>
  /// <remarks>
  ///   <paramref name="validFrom" /> is converted to UTC. An exception is thrown when
  ///   building the token if <paramref name="validFrom" /> is earlier than the issuedAt value."/>.
  /// </remarks>
  public JsonWebTokenBuilder SetValidFrom(
    DateTime? validFrom )
  {
    _validFrom = validFrom?.ToUniversalTime();
    return this;
  }

  /// <summary>
  ///   Sets the time before which the token is not valid.
  /// </summary>
  /// <param name="validFrom">The time before which the token is not valid.</param>
  /// <returns>The current <see cref="JsonWebTokenBuilder" /> instance.</returns>
  /// <remarks>
  ///   <paramref name="validFrom" /> is converted to UTC. An exception is thrown when
  ///   building the token if <paramref name="validFrom" /> is earlier than the issuedAt value."/>.
  /// </remarks>
  public JsonWebTokenBuilder SetValidFrom(
    DateTimeOffset? validFrom )
  {
    _validFrom = validFrom?.UtcDateTime;
    return this;
  }

  /// <summary>
  ///   Adds an audience (aud) claim to the token.
  /// </summary>
  /// <param name="audience">The audience to add.</param>
  /// <returns>The current <see cref="JsonWebTokenBuilder" /> instance.</returns>
  /// <exception cref="ArgumentNullException">If <paramref name="audience" /> is <c>null</c>.</exception>
  /// <exception cref="ArgumentException">If <paramref name="audience" /> is empty.</exception>
  public JsonWebTokenBuilder AddAudience(
    string audience )
  {
    return AddClaim( JwtRegisteredClaimNames.Aud, audience );
  }

  /// <summary>
  ///   Sets the subject (sub) claim of the token.
  /// </summary>
  /// <param name="subject">The subject of the token.</param>
  /// <returns>The current <see cref="JsonWebTokenBuilder" /> instance.</returns>
  /// <exception cref="ArgumentNullException">If <paramref name="subject" /> is <c>null</c>.</exception>
  /// <exception cref="ArgumentException">If <paramref name="subject" /> is empty.</exception>
  public JsonWebTokenBuilder SetSubject(
    string subject )
  {
    return AddClaim( JwtRegisteredClaimNames.Sub, subject );
  }

  /// <summary>
  ///   Adds a <see cref="ClaimTypes.Role" /> claim to the token.
  /// </summary>
  /// <param name="role">The role to add.</param>
  /// <returns>The current <see cref="JsonWebTokenBuilder" /> instance.</returns>
  /// <exception cref="ArgumentNullException">If <paramref name="role" /> is <c>null</c>.</exception>
  /// <exception cref="ArgumentException">If <paramref name="role" /> is empty.</exception>
  public JsonWebTokenBuilder AddRole(
    string role )
  {
    return AddClaim( ClaimTypes.Role, role );
  }

  /// <summary>
  ///   Adds a custom claim to the token.
  /// </summary>
  /// <param name="claimName">The name of the claim.</param>
  /// <param name="value">The value of the claim.</param>
  /// <returns>The current <see cref="JsonWebTokenBuilder" /> instance.</returns>
  /// <exception cref="ArgumentNullException">If <paramref name="claimName" /> or <paramref name="value" /> are <c>null</c>.</exception>
  /// <exception cref="ArgumentException">If <paramref name="claimName" /> is empty.</exception>
  public JsonWebTokenBuilder AddClaim(
    string claimName,
    object value )
  {
    ArgumentException.ThrowIfNullOrEmpty( claimName );

    _claims.Add( claimName, value );
    return this;
  }

  /// <summary>
  ///   Sets the compression algorithm for the token.
  /// </summary>
  /// <param name="compressionAlgorithm">The compression algorithm to use.</param>
  /// <returns>The current <see cref="JsonWebTokenBuilder" /> instance.</returns>
  /// <exception cref="ArgumentNullException">If <paramref name="compressionAlgorithm" /> is <c>null</c>.</exception>
  /// <exception cref="ArgumentException">If <paramref name="compressionAlgorithm" /> is empty.</exception>
  public JsonWebTokenBuilder SetCompressionAlgorithm(
    string compressionAlgorithm )
  {
    _compressionAlgorithm = compressionAlgorithm;
    return this;
  }

  /// <summary>
  ///   Sets the encryption algorithm for the token.
  /// </summary>
  /// <param name="encryptingCredentials">The encrypting credentials to use.</param>
  /// <returns>The current <see cref="JsonWebTokenBuilder" /> instance.</returns>
  /// <exception cref="ArgumentNullException">If <paramref name="encryptingCredentials" /> is <c>null</c>.</exception>
  /// <exception cref="ArgumentException">If <paramref name="encryptingCredentials" /> is empty.</exception>
  public JsonWebTokenBuilder SetEncryptionAlgorithm(
    EncryptingCredentials encryptingCredentials )
  {
    _encryptingCredentials = encryptingCredentials;
    return this;
  }

  /// <summary>
  ///   Builds the JWT security token.
  /// </summary>
  /// <param name="requiredClaims">The claims that are required in the token.</param>
  /// <returns>A JwtSecurityToken object.</returns>
  /// <exception cref="InvalidOperationException">
  ///   If a required claim is missing or the validFrom value is less than
  ///   issuedAt.
  /// </exception>
  public Result<JwtSecurityToken> Build(
    RequiredClaim requiredClaims = RequiredClaim.Default )
  {
    var tokenHandler = new JwtSecurityTokenHandler();
    return BuildImpl( tokenHandler, requiredClaims );
  }

  /// <summary>
  ///   Builds and encodes in Base64 the JWT security token.
  /// </summary>
  /// <param name="requiredClaims">The claims that are required in the token.</param>
  /// <returns>A JwtSecurityToken object.</returns>
  /// <exception cref="InvalidOperationException">
  ///   If a required claim is missing or the validFrom value is less than
  ///   issuedAt.
  /// </exception>
  public Result<string> BuildEncoded(
    RequiredClaim requiredClaims = RequiredClaim.Default )
  {
    var tokenHandler = new JwtSecurityTokenHandler();

    var result = BuildImpl( tokenHandler, requiredClaims );
    if( result.IsFailed )
    {
      return Result.Fail<string>( result.Errors );
    }

    return tokenHandler.WriteToken( result.Value );
  }

  #endregion

  #region Implementation

  private Result<JwtSecurityToken> BuildImpl(
    JwtSecurityTokenHandler tokenHandler,
    RequiredClaim requiredClaims )
  {
    var errors = new List<string>();

    EnsureRequiredClaimWasProvided( RequiredClaim.Issuer );
    EnsureRequiredClaimWasProvided( RequiredClaim.Subject );
    EnsureRequiredClaimWasProvided( RequiredClaim.Audience );
    EnsureRequiredClaimWasProvided( RequiredClaim.Role );

    _issuedAt ??= _timeProvider.GetUtcNow()
                               .UtcDateTime;

    _validFrom ??= _issuedAt;
    if( _validFrom < _issuedAt )
    {
      errors.Add( "The 'validFrom' value cannot be earlier than 'issuedAt'" );
    }

    _validTo ??= _issuedAt + _lifeTime;
    if( _validTo <= _validFrom )
    {
      errors.Add( "The 'validTo' value cannot be earlier than 'validFrom'" );
    }

    if( IsClaimRequired( RequiredClaim.Jti ) )
    {
      _claims.TryAdd(
        JwtRegisteredClaimNames.Jti,
        Guid.NewGuid()
            .ToString()
      );
    }

    // Collect all the claims and report the ones that don't have a value
    var claims = new List<Claim>( _claims.Count );

    foreach( var grouping in _claims )
    {
      foreach( var value in grouping )
      {
        var s = value?.ToString();
        if( string.IsNullOrWhiteSpace( s ) )
        {
          errors.Add( $"Must provide a value for the '{grouping.Key}' claim" );
        }
        else
        {
          // Stop adding claims if there are any errors to avoid unnecessary allocations
          if( errors.Count == 0 )
          {
            claims.Add( new Claim( grouping.Key, s ) );
          }
        }
      }
    }

    // Cannot create the token if there were any errors
    if( errors.Count > 0 )
    {
      return Result.Fail( errors );
    }

    var descriptor = new SecurityTokenDescriptor
    {
      SigningCredentials = _signingCredentials,
      IssuedAt = _issuedAt,
      Expires = _validTo,
      NotBefore = _validFrom,
      Subject = new ClaimsIdentity( claims )
    };

    if( _encryptingCredentials is not null )
    {
      descriptor.EncryptingCredentials = _encryptingCredentials;
    }

    if( _compressionAlgorithm is not null )
    {
      descriptor.CompressionAlgorithm = _compressionAlgorithm;
    }

    var token = tokenHandler.CreateJwtSecurityToken( descriptor );
    return token;

    void EnsureRequiredClaimWasProvided(
      RequiredClaim claim )
    {
      if( !IsClaimRequired( claim ) )
      {
        return;
      }

      var claimName = REQUIRED_CLAIM_NAMES[claim];
      if( _claims.Contains( claimName ) )
      {
        return;
      }

      errors.Add( $"Must provide a value for the '{claimName}' claim" );
    }

    bool IsClaimRequired(
      RequiredClaim claim )
    {
      return ( requiredClaims & claim ) == claim;
    }
  }

  private static SigningCredentials CreateSigningCredentials(
    string signingKey,
    string signatureAlgorithm )
  {
    ArgumentException.ThrowIfNullOrEmpty( signingKey );

    var bytes = Encoding.UTF8.GetBytes( signingKey + signingKey );
    var securityKey = new SymmetricSecurityKey( bytes );
    var signingCredentials = new SigningCredentials( securityKey, signatureAlgorithm );
    return signingCredentials;
  }

  #endregion
}
