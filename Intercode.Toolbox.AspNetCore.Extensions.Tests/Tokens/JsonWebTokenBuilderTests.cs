// Module Name: JsonWebTokenBuilderTests.cs
// Author:      Eduardo Velasquez
// Copyright (c) 2024, Intercode Consulting, Inc.

namespace Intercode.Toolbox.AspNetCore.Extensions.Tests.Tokens;

using System.IdentityModel.Tokens.Jwt;
using FluentAssertions;
using FluentResults.Extensions.FluentAssertions;
using Intercode.Toolbox.AspNetCore.Extensions.Tokens;
using Microsoft.Extensions.Time.Testing;
using Microsoft.IdentityModel.Tokens;
using JwtRegisteredClaimNames = Microsoft.IdentityModel.JsonWebTokens.JwtRegisteredClaimNames;

public class JsonWebTokenBuilderTests
{
  #region Constants

  private const string SIGNING_KEY = "The signing key must be at least 128 bits long";
  private const string SIGNING_ALGO = SecurityAlgorithms.HmacSha256Signature;
  private const string ISSUER = "TestIssuer";
  private const string SUBJECT = "TestSubject";
  private const string AUDIENCE = "TestAudience";
  private const string ROLE = "TestRole";

  #endregion

  #region Tests

  [Fact]
  public void AddAudience_ShouldSucceed()
  {
    var builder = new JsonWebTokenBuilder( SIGNING_KEY, SIGNING_ALGO );
    var act = () => builder.AddAudience( "Audience" );

    act.Should()
       .NotThrow();
  }

  [Fact]
  public void AddClaim_ShouldSucceed()
  {
    var builder = new JsonWebTokenBuilder( SIGNING_KEY, SIGNING_ALGO );
    var act = () => builder.AddClaim( "Claim", "Value" );

    act.Should()
       .NotThrow();
  }

  [Fact]
  public void AddClaim_ShouldThrow_WhenClaimNameIsEmpty()
  {
    var builder = new JsonWebTokenBuilder( SIGNING_KEY, SIGNING_ALGO );
    var act = () => builder.AddClaim( "", "Value" );

    act.Should()
       .Throw<ArgumentException>();
  }

  [Fact]
  public void AddClaim_ShouldThrow_WhenClaimNameIsNull()
  {
    var builder = new JsonWebTokenBuilder( SIGNING_KEY, SIGNING_ALGO );
    var act = () => builder.AddClaim( null!, "Value" );

    act.Should()
       .Throw<ArgumentNullException>();
  }

  [Fact]
  public void AddRole_ShouldSucceed()
  {
    var builder = new JsonWebTokenBuilder( SIGNING_KEY, SIGNING_ALGO );
    var act = () => builder.AddRole( "Role" );

    act.Should()
       .NotThrow();
  }

  [Fact]
  public void Build_ShouldFail_WhenCustomClaimValueIsEmpty()
  {
    var result = new JsonWebTokenBuilder( SIGNING_KEY, SIGNING_ALGO ).AddClaim( "Claim", "" )
                                                                     .Build( RequiredClaim.None );

    result.Should()
          .BeFailure();

    result.Should()
          .HaveReason( "Must provide a value for the 'Claim' claim" );
  }

  [Fact]
  public void Build_ShouldFail_WhenCustomClaimValueIsNull()
  {
    var result = new JsonWebTokenBuilder( SIGNING_KEY, SIGNING_ALGO ).AddClaim( "Claim", null! )
                                                                     .Build( RequiredClaim.None );

    result.Should()
          .BeFailure();

    result.Should()
          .HaveReason( "Must provide a value for the 'Claim' claim" );
  }

  [Fact]
  public void Build_ShouldFail_WhenLifetimeIsZero()
  {
    var result = new JsonWebTokenBuilder( SIGNING_KEY, SIGNING_ALGO ).SetLifeTime( TimeSpan.Zero )
                                                                     .Build( RequiredClaim.None );

    result.Should()
          .BeFailure();

    result.Should()
          .HaveReason( "The 'validTo' value cannot be earlier than 'validFrom'" );
  }

  [Fact]
  public void Build_ShouldFail_WhenNotBeforeIsEarlierThanIssuedAt()
  {
    var result = new JsonWebTokenBuilder( SIGNING_KEY, SIGNING_ALGO ).SetIssuer( ISSUER )
                                                                     .SetSubject( SUBJECT )
                                                                     .AddAudience( AUDIENCE )
                                                                     .AddRole( ROLE )
                                                                     .SetIssuedAt( TimeProvider.GetUtcNow() )
                                                                     .SetValidFrom(
                                                                       TimeProvider.GetUtcNow()
                                                                         .AddSeconds( -1.0 )
                                                                     )
                                                                     .Build();

    result.Should()
          .BeFailure();

    result.Should()
          .HaveReason( "The 'validFrom' value cannot be earlier than 'issuedAt'" );
  }

  [Fact]
  public void Build_ShouldFail_WhenRequiredAudienceIsEmpty()
  {
    var result = new JsonWebTokenBuilder( SIGNING_KEY, SIGNING_ALGO ).AddAudience( "" )
                                                                     .Build( RequiredClaim.Audience );

    result.Should()
          .BeFailure();

    result.Should()
          .HaveReason( "Must provide a value for the 'aud' claim" );
  }

  [Fact]
  public void Build_ShouldFail_WhenRequiredAudienceIsMissing()
  {
    var result = new JsonWebTokenBuilder( SIGNING_KEY, SIGNING_ALGO ).Build( RequiredClaim.Audience );

    result.Should()
          .BeFailure();

    result.Should()
          .HaveReason( "Must provide a value for the 'aud' claim" );
  }

  [Fact]
  public void Build_ShouldFail_WhenRequiredAudienceIsNull()
  {
    var result = new JsonWebTokenBuilder( SIGNING_KEY, SIGNING_ALGO ).AddAudience( null! )
                                                                     .Build( RequiredClaim.Audience );

    result.Should()
          .BeFailure();

    result.Should()
          .HaveReason( "Must provide a value for the 'aud' claim" );
  }

  [Fact]
  public void Build_ShouldFail_WhenRequiredIssuerIsEmpty()
  {
    var result = new JsonWebTokenBuilder( SIGNING_KEY, SIGNING_ALGO ).SetIssuer( "" )
                                                                     .Build( RequiredClaim.Issuer );

    result.Should()
          .BeFailure();

    result.Should()
          .HaveReason( "Must provide a value for the 'iss' claim" );
  }

  [Fact]
  public void Build_ShouldFail_WhenRequiredIssuerIsMissing()
  {
    var result = new JsonWebTokenBuilder( SIGNING_KEY, SIGNING_ALGO ).Build( RequiredClaim.Issuer );

    result.Should()
          .BeFailure();

    result.Should()
          .HaveReason( "Must provide a value for the 'iss' claim" );
  }

  [Fact]
  public void Build_ShouldFail_WhenRequiredIssuerIsNull()
  {
    var result = new JsonWebTokenBuilder( SIGNING_KEY, SIGNING_ALGO ).SetIssuer( null! )
                                                                     .Build( RequiredClaim.Issuer );

    result.Should()
          .BeFailure();

    result.Should()
          .HaveReason( "Must provide a value for the 'iss' claim" );
  }

  [Fact]
  public void Build_ShouldFail_WhenRequiredRoleIsEmpty()
  {
    var result = new JsonWebTokenBuilder( SIGNING_KEY, SIGNING_ALGO ).AddRole( "" )
                                                                     .Build( RequiredClaim.Role );

    result.Should()
          .BeFailure();

    result.Should()
          .HaveReason( "Must provide a value for the 'http://schemas.microsoft.com/ws/2008/06/identity/claims/role' claim" );
  }

  [Fact]
  public void Build_ShouldFail_WhenRequiredRoleIsMissing()
  {
    var result = new JsonWebTokenBuilder( SIGNING_KEY, SIGNING_ALGO ).Build( RequiredClaim.Role );

    result.Should()
          .BeFailure();

    result.Should()
          .HaveReason( "Must provide a value for the 'http://schemas.microsoft.com/ws/2008/06/identity/claims/role' claim" );
  }

  [Fact]
  public void Build_ShouldFail_WhenRequiredRoleIsNull()
  {
    var result = new JsonWebTokenBuilder( SIGNING_KEY, SIGNING_ALGO ).AddRole( null! )
                                                                     .Build( RequiredClaim.Role );

    result.Should()
          .BeFailure();

    result.Should()
          .HaveReason( "Must provide a value for the 'http://schemas.microsoft.com/ws/2008/06/identity/claims/role' claim" );
  }

  [Fact]
  public void Build_ShouldFail_WhenRequiredSubjectIsEmpty()
  {
    var result = new JsonWebTokenBuilder( SIGNING_KEY, SIGNING_ALGO ).SetSubject( "" )
                                                                     .Build( RequiredClaim.Subject );

    result.Should()
          .BeFailure();

    result.Should()
          .HaveReason( "Must provide a value for the 'sub' claim" );
  }

  [Fact]
  public void Build_ShouldFail_WhenRequiredSubjectIsMissing()
  {
    var result = new JsonWebTokenBuilder( SIGNING_KEY, SIGNING_ALGO ).Build( RequiredClaim.Subject );

    result.Should()
          .BeFailure();

    result.Should()
          .HaveReason( "Must provide a value for the 'sub' claim" );
  }

  [Fact]
  public void Build_ShouldFail_WhenRequiredSubjectIsNull()
  {
    var result = new JsonWebTokenBuilder( SIGNING_KEY, SIGNING_ALGO ).SetSubject( null! )
                                                                     .Build( RequiredClaim.Subject );

    result.Should()
          .BeFailure();

    result.Should()
          .HaveReason( "Must provide a value for the 'sub' claim" );
  }

  [Fact]
  public void Build_ShouldGenerateTokenId_WhenNotProvided()
  {
    var result = new JsonWebTokenBuilder( SIGNING_KEY, SIGNING_ALGO, TimeProvider ).SetIssuer( ISSUER )
      .SetSubject( SUBJECT )
      .Build( RequiredClaim.Jti );

    result.Should()
          .BeSuccess();

    var token = result.Value;

    var tokenId = Guid.Empty;
    token.Claims.Should()
         .ContainSingle( claim => claim.Type == JwtRegisteredClaimNames.Jti && Guid.TryParse( claim.Value, out tokenId ) );

    tokenId.Should()
           .NotBe( Guid.Empty );
  }

  [Fact]
  public void Build_ShouldSucceed()
  {
    var tokenLifeTime = TimeSpan.FromMinutes( 5 );

    var result = new JsonWebTokenBuilder( SIGNING_KEY, SIGNING_ALGO, TimeProvider ).SetIssuer( ISSUER )
      .SetSubject( SUBJECT )
      .AddAudience( AUDIENCE )
      .AddRole( ROLE )
      .SetLifeTime( tokenLifeTime )
      .Build();

    result.Should()
          .BeSuccess();

    var token = result.Value;
    token.Should()
         .NotBeNull();

    token.Issuer.Should()
         .Be( ISSUER );

    token.Subject.Should()
         .Be( SUBJECT );

    token.Audiences.Should()
         .ContainSingle( AUDIENCE );

    token.Claims.Should()
         .ContainSingle( claim => claim.Type == "role" && claim.Value == ROLE );

    var issuedAt = TimeProvider.GetUtcNow()
                               .UtcDateTime;
    token.IssuedAt.Should()
         .Be( issuedAt );

    token.ValidFrom.Should()
         .Be( issuedAt );

    token.ValidTo.Should()
         .Be( issuedAt + tokenLifeTime );
  }

  [Fact]
  public void BuildEncoded_ShouldSucceed()
  {
    var result = new JsonWebTokenBuilder( SIGNING_KEY, SIGNING_ALGO, TimeProvider ).SetIssuer( ISSUER )
      .SetSubject( SUBJECT )
      .AddAudience( AUDIENCE )
      .AddRole( ROLE )
      .BuildEncoded();

    result.Should()
          .BeSuccess();

    var handler = new JwtSecurityTokenHandler();
    var token = handler.ReadToken( result.Value ) as JwtSecurityToken;
    token.Should()
         .NotBeNull();

    token!.Issuer.Should()
          .Be( ISSUER );

    token.Subject.Should()
         .Be( SUBJECT );

    token.Audiences.Should()
         .ContainSingle( AUDIENCE );

    token.Claims.Should()
         .ContainSingle( claim => claim.Type == "role" && claim.Value == ROLE );
  }

  [Fact]
  public void Ctor_ShouldSucceed_WhenSigningCredentialsIsProvided()
  {
    var credentials = new SigningCredentials(
      new SymmetricSecurityKey(
        Guid.NewGuid()
            .ToByteArray()
      ),
      SecurityAlgorithms.HmacSha256Signature
    );

    var act = () => new JsonWebTokenBuilder( credentials );

    act.Should()
       .NotThrow();
  }

  [Fact]
  public void Ctor_ShouldSucceed_WhenSigningKeyIsProvided()
  {
    var act = () => new JsonWebTokenBuilder( SIGNING_KEY, SIGNING_ALGO );

    act.Should()
       .NotThrow();
  }

  [Fact]
  public void Ctor_ShouldThrow_WhenSigningCredentialsIsNull()
  {
    var act = () => new JsonWebTokenBuilder( ( SigningCredentials ) null! );

    act.Should()
       .Throw<ArgumentNullException>();
  }

  [Fact]
  public void Ctor_ShouldThrow_WhenStringSigningKeyIsEmpty()
  {
    var act = () => new JsonWebTokenBuilder( "" );

    act.Should()
       .Throw<ArgumentException>();
  }

  [Fact]
  public void Ctor_ShouldThrow_WhenStringSigningKeyIsNull()
  {
    var act = () => new JsonWebTokenBuilder( ( string ) null! );

    act.Should()
       .Throw<ArgumentNullException>();
  }

  [Fact]
  public void SetCompressionAlgorithm_ShouldSucceed_WhenValueIsProvided()
  {
    var builder = new JsonWebTokenBuilder( SIGNING_KEY, SIGNING_ALGO );
    var act = () => builder.SetCompressionAlgorithm( "zip" );

    act.Should()
       .NotThrow();
  }

  [Fact]
  public void SetEncryptionAlgorithm_ShouldSucceed_WhenValueIsProvided()
  {
    var encryptingCredentials = new EncryptingCredentials(
      new SymmetricSecurityKey(
        Guid.NewGuid()
            .ToByteArray()
      ),
      "enc"
    );
    var builder = new JsonWebTokenBuilder( SIGNING_KEY, SIGNING_ALGO );
    var act = () => builder.SetEncryptionAlgorithm( encryptingCredentials );

    act.Should()
       .NotThrow();
  }

  [Fact]
  public void SetIssuedAt_WithDateTime_ShouldSucceed_WhenValueIsNull()
  {
    var builder = new JsonWebTokenBuilder( SIGNING_KEY, SIGNING_ALGO );
    var act = () => builder.SetIssuedAt( null );

    act.Should()
       .NotThrow();
  }

  [Fact]
  public void SetIssuedAt_WithDateTime_ShouldSucceed_WhenValueIsProvided()
  {
    var builder = new JsonWebTokenBuilder( SIGNING_KEY, SIGNING_ALGO );
    var act = () => builder.SetIssuedAt(
      TimeProvider.GetUtcNow()
                  .Date
    );

    act.Should()
       .NotThrow();
  }

  [Fact]
  public void SetIssuedAt_WithDateTimeOffset_ShouldSucceed_WhenValueIsNull()
  {
    var builder = new JsonWebTokenBuilder( SIGNING_KEY, SIGNING_ALGO );
    var act = () => builder.SetIssuedAt( ( DateTimeOffset? ) null );

    act.Should()
       .NotThrow();
  }

  [Fact]
  public void SetIssuedAt_WithDateTimeOffset_ShouldSucceed_WhenValueIsProvided()
  {
    var builder = new JsonWebTokenBuilder( SIGNING_KEY, SIGNING_ALGO );
    var act = () => builder.SetIssuedAt( TimeProvider.GetUtcNow() );

    act.Should()
       .NotThrow();
  }

  [Fact]
  public void SetIssuer_ShouldSucceed_WhenValueIsProvided()
  {
    var builder = new JsonWebTokenBuilder( SIGNING_KEY, SIGNING_ALGO );
    var act = () => builder.SetIssuer( ISSUER );

    act.Should()
       .NotThrow();
  }

  [Fact]
  public void SetLifeTime_ShouldSucceed_WhenValueIsProvided()
  {
    var builder = new JsonWebTokenBuilder( SIGNING_KEY, SIGNING_ALGO );
    var act = () => builder.SetLifeTime( TimeSpan.FromMinutes( 10 ) );

    act.Should()
       .NotThrow();
  }

  [Fact]
  public void SetSubject_ShouldSucceed_WhenValueIsProvided()
  {
    var builder = new JsonWebTokenBuilder( SIGNING_KEY, SIGNING_ALGO );
    var act = () => builder.SetSubject( "Subject" );

    act.Should()
       .NotThrow();
  }

  [Fact]
  public void SetTokenId_ShouldNotGenerateId_WhenValueIsProvided()
  {
    const string ExpectedTokenId = "TokenId";

    var result = new JsonWebTokenBuilder( SIGNING_KEY, SIGNING_ALGO ).SetTokenId( ExpectedTokenId )
                                                                     .Build( RequiredClaim.None );

    result.IsSuccess.Should()
          .BeTrue();

    var token = result.Value;
    token.Claims.Should()
         .ContainSingle( claim => claim.Type == JwtRegisteredClaimNames.Jti && claim.Value == ExpectedTokenId );
  }

  [Fact]
  public void SetValidFrom_WithDateTime_ShouldSucceed()
  {
    var builder = new JsonWebTokenBuilder( SIGNING_KEY, SIGNING_ALGO );
    var act = () => builder.SetValidFrom(
      TimeProvider.GetUtcNow()
                  .Date
    );

    act.Should()
       .NotThrow();
  }

  [Fact]
  public void SetValidFrom_WithDateTime_ShouldSucceed_WhenValueIsNull()
  {
    var builder = new JsonWebTokenBuilder( SIGNING_KEY, SIGNING_ALGO );
    var act = () => builder.SetValidFrom( null );

    act.Should()
       .NotThrow();
  }

  [Fact]
  public void SetValidFrom_WithDateTimeOffset_ShouldSucceed()
  {
    var builder = new JsonWebTokenBuilder( SIGNING_KEY, SIGNING_ALGO );
    var act = () => builder.SetValidFrom( TimeProvider.GetUtcNow() );

    act.Should()
       .NotThrow();
  }

  [Fact]
  public void SetValidFrom_WithDateTimeOffset_ShouldSucceed_WhenValueIsNull()
  {
    var builder = new JsonWebTokenBuilder( SIGNING_KEY, SIGNING_ALGO );
    var act = () => builder.SetValidFrom( ( DateTimeOffset? ) null );

    act.Should()
       .NotThrow();
  }

  [Fact]
  public void SetValidTo_WithDateTime_ShouldSucceed()
  {
    var builder = new JsonWebTokenBuilder( SIGNING_KEY, SIGNING_ALGO );
    var act = () => builder.SetValidTo(
      TimeProvider.GetUtcNow()
                  .Date
    );

    act.Should()
       .NotThrow();
  }

  [Fact]
  public void SetValidTo_WithDateTime_ShouldSucceed_WhenValueIsNull()
  {
    var builder = new JsonWebTokenBuilder( SIGNING_KEY, SIGNING_ALGO );
    var act = () => builder.SetValidTo( null );

    act.Should()
       .NotThrow();
  }

  [Fact]
  public void SetValidTo_WithDateTimeOffset_ShouldSucceed()
  {
    var builder = new JsonWebTokenBuilder( SIGNING_KEY, SIGNING_ALGO );
    var act = () => builder.SetValidTo( TimeProvider.GetUtcNow() );

    act.Should()
       .NotThrow();
  }

  [Fact]
  public void SetValidTo_WithDateTimeOffset_ShouldSucceed_WhenValueIsNull()
  {
    var builder = new JsonWebTokenBuilder( SIGNING_KEY, SIGNING_ALGO );
    var act = () => builder.SetValidTo( ( DateTimeOffset? ) null );

    act.Should()
       .NotThrow();
  }

  #endregion

  #region Implementation

  public TimeProvider TimeProvider { get; } = new FakeTimeProvider(
    new DateTimeOffset(
      2023,
      9,
      13,
      14,
      10,
      0,
      TimeSpan.FromHours( -7 )
    )
  );

  #endregion
}
