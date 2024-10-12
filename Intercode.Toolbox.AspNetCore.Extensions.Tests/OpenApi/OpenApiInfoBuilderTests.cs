// Module Name: OpenApiInfoBuilderTests.cs
// Author:      Eduardo Velasquez
// Copyright (c) 2024, Intercode Consulting, Inc.

// ReSharper disable ObjectCreationAsStatement

namespace Intercode.Toolbox.AspNetCore.Extensions.Tests.OpenApi;

using FluentAssertions;
using Intercode.Toolbox.AspNetCore.Extensions.OpenApi;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Interfaces;
using Microsoft.OpenApi.Validations;

public class OpenApiInfoBuilderTests
{
  #region Constants

  private const string EXPECTED_TITLE = "Test API";
  private const string EXPECTED_VERSION = "v1";
  private const string EXPECTED_CONTACT_NAME = "Contact Name";
  private const string EXPECTED_CONTACT_EMAIL = "contact@example.com";
  private const string EXPECTED_CONTACT_URL = "https://example.com/";
  private const string EXPECTED_DESCRIPTION = "Description";
  private const string EXPECTED_EXTENSION_KEY = "x-test-extension";
  private const string EXPECTED_LICENSE = "License";
  private const string EXPECTED_LICENSE_URL = "https://example.com/license";
  private const string EXPECTED_TERMS_OF_SERVICE_URL = "https://example.com/terms/";

  private static readonly IOpenApiExtension s_expectedExtensionValue = new OpenApiNull();

  private static readonly Dictionary<string, IOpenApiExtension> s_expectedExtensions = new ()
  {
    { EXPECTED_EXTENSION_KEY, s_expectedExtensionValue }
  };

  #endregion

  #region Tests

  [Fact]
  public void AddContact_WithUri_ShouldOnlyAdd_WhenAtLeaseOneValueIsProvided()
  {
    var builder = new OpenApiInfoBuilder( EXPECTED_TITLE, EXPECTED_VERSION ).AddContact( null, null, ( Uri? ) null );

    var result = builder.Build();
    result.IsSuccess.Should()
          .BeTrue();

    var info = result.Value;
    info.Contact.Should()
        .BeNull();
  }

  [Fact]
  public void AddContact_WithUrl_ShouldOnlyAdd_WhenAtLeaseOneValueIsProvided()
  {
    var builder = new OpenApiInfoBuilder( EXPECTED_TITLE, EXPECTED_VERSION ).AddContact( null, null, ( string? ) null );

    var result = builder.Build();
    result.IsSuccess.Should()
          .BeTrue();

    var info = result.Value;
    info.Contact.Should()
        .BeNull();
  }

  [Fact]
  public void AddExtension_ShouldThrow_WhenKeyIsEmpty()
  {
    var builder = new OpenApiInfoBuilder( EXPECTED_TITLE, EXPECTED_VERSION );

    var act = () => builder.AddExtension( string.Empty, s_expectedExtensionValue );

    act.Should()
       .Throw<ArgumentException>()
       .WithMessage( "The value cannot be an empty string. (Parameter 'key')" );
  }

  [Fact]
  public void AddExtension_ShouldThrow_WhenKeyIsNull()
  {
    var builder = new OpenApiInfoBuilder( EXPECTED_TITLE, EXPECTED_VERSION );

    var act = () => builder.AddExtension( null!, s_expectedExtensionValue );

    act.Should()
       .Throw<ArgumentNullException>()
       .WithMessage( "Value cannot be null. (Parameter 'key')" );
  }

  [Fact]
  public void AddExtension_ShouldThrow_WhenValueIsNull()
  {
    var builder = new OpenApiInfoBuilder( EXPECTED_TITLE, EXPECTED_VERSION );

    var act = () => builder.AddExtension( EXPECTED_EXTENSION_KEY, null! );

    act.Should()
       .Throw<ArgumentNullException>()
       .WithMessage( "Value cannot be null. (Parameter 'extension')" );
  }

  [Fact]
  public void AddExtensions_ShouldNotAdd_WhenDictionaryIsEmpty()
  {
    var builder = new OpenApiInfoBuilder( EXPECTED_TITLE, EXPECTED_VERSION )
      .AddExtensions( new Dictionary<string, IOpenApiExtension>() );

    var result = builder.Build();
    result.IsSuccess.Should()
          .BeTrue();

    var info = result.Value;
    info.Extensions.Should()
        .BeEmpty();
  }

  [Fact]
  public void AddExtensions_ShouldNotAdd_WhenDictionaryIsNull()
  {
    var builder = new OpenApiInfoBuilder( EXPECTED_TITLE, EXPECTED_VERSION )
      .AddExtensions( null );

    var result = builder.Build();
    result.IsSuccess.Should()
          .BeTrue();

    var info = result.Value;
    info.Extensions.Should()
        .BeEmpty();
  }

  [Fact]
  public void AddExtensions_WithDictionary_ShouldNotThrow_WhenExtensionsIsNull()
  {
    var builder = new OpenApiInfoBuilder( EXPECTED_TITLE, EXPECTED_VERSION );

    var act = () => builder.AddExtensions( null );

    act.Should()
       .NotThrow();
  }

  [Fact]
  public void AddLicense_WithUri_ShouldOnlyAdd_WhenAtLeastOneValueIsProvided()
  {
    var builder = new OpenApiInfoBuilder( EXPECTED_TITLE, EXPECTED_VERSION ).AddLicense( null, ( Uri? ) null );

    var result = builder.Build();
    result.IsSuccess.Should()
          .BeTrue();

    var info = result.Value;
    info.License.Should()
        .BeNull();
  }

  [Fact]
  public void AddLicense_WithUrl_ShouldOnlyAdd_WhenAtLeastOneValueIsProvided()
  {
    var builder = new OpenApiInfoBuilder( EXPECTED_TITLE, EXPECTED_VERSION ).AddLicense( null, ( string? ) null );

    var result = builder.Build();
    result.IsSuccess.Should()
          .BeTrue();

    var info = result.Value;
    info.License.Should()
        .BeNull();
  }

  [Fact]
  public void Build_ShouldFail_WhenProvidedAnInvalidEmail()
  {
    var builder = new OpenApiInfoBuilder( EXPECTED_TITLE, EXPECTED_VERSION ).AddContact(
      EXPECTED_CONTACT_NAME,
      "bad-email.com",
      ( string? ) null
    );

    var result = builder.Build( ValidationRuleSet.GetDefaultRuleSet() );
    result.Should()
          .NotBeNull();

    result.IsSuccess.Should()
          .BeFalse();

    result.Errors.Should()
          .HaveCount( 1 );

    var error = result.Errors.First();

    error.Message.Should()
         .Be( "The string 'bad-email.com' MUST be in the format of an email address." );

    error.Metadata.Should()
         .ContainKey( "error" )
         .WhoseValue.Should()
         .BeOfType<OpenApiValidatorError>()
         .Which.Pointer.Should()
         .Be( "#/contact/email" );
  }

  [Fact]
  public void Build_ShouldSucceed_WhenAllValuesAreProvided()
  {
    var builder = new OpenApiInfoBuilder( EXPECTED_TITLE, EXPECTED_VERSION ).AddContact(
                                                                              EXPECTED_CONTACT_NAME,
                                                                              EXPECTED_CONTACT_EMAIL,
                                                                              EXPECTED_CONTACT_URL,
                                                                              s_expectedExtensions
                                                                            )
                                                                            .AddDescription( EXPECTED_DESCRIPTION )
                                                                            .AddExtension(
                                                                              EXPECTED_EXTENSION_KEY,
                                                                              s_expectedExtensionValue
                                                                            )
                                                                            .AddLicense(
                                                                              EXPECTED_LICENSE,
                                                                              EXPECTED_LICENSE_URL,
                                                                              s_expectedExtensions
                                                                            )
                                                                            .AddTermsOfService(
                                                                              EXPECTED_TERMS_OF_SERVICE_URL
                                                                            );

    var result = builder.Build();
    result.Should()
          .NotBeNull();

    result.IsSuccess.Should()
          .BeTrue();

    var info = result.Value;
    info.Title.Should()
        .Be( EXPECTED_TITLE );

    info.Version.Should()
        .Be( EXPECTED_VERSION );

    var contact = info.Contact;
    contact.Should()
           .NotBeNull();

    contact.Name.Should()
           .Be( EXPECTED_CONTACT_NAME );

    contact.Email.Should()
           .Be( EXPECTED_CONTACT_EMAIL );

    contact.Url.ToString()
           .Should()
           .Be( EXPECTED_CONTACT_URL );

    contact.Extensions.Should()
           .ContainKey( EXPECTED_EXTENSION_KEY )
           .WhoseValue
           .Should()
           .Be( s_expectedExtensionValue );

    info.Description.Should()
        .Be( EXPECTED_DESCRIPTION );

    info.Extensions.Should()
        .ContainKey( EXPECTED_EXTENSION_KEY )
        .WhoseValue.Should()
        .Be( s_expectedExtensionValue );

    var license = info.License;
    license.Should()
           .NotBeNull();

    license.Name.Should()
           .Be( EXPECTED_LICENSE );

    license.Url.ToString()
           .Should()
           .Be( EXPECTED_LICENSE_URL );

    license.Extensions.Should()
           .ContainKey( EXPECTED_EXTENSION_KEY )
           .WhoseValue.Should()
           .Be( s_expectedExtensionValue );

    info.TermsOfService.Should()
        .Be( EXPECTED_TERMS_OF_SERVICE_URL );
  }

  [Fact]
  public void Build_ShouldSucceed_WhenProvidedAnInvalidEmailAndEmptyRuleSet()
  {
    var builder = new OpenApiInfoBuilder( EXPECTED_TITLE, EXPECTED_VERSION ).AddContact(
      EXPECTED_CONTACT_NAME,
      "bad-email.com",
      ( string? ) null
    );

    var result = builder.Build( ValidationRuleSet.GetEmptyRuleSet() );
    result.Should()
          .NotBeNull();

    result.IsSuccess.Should()
          .BeTrue();
  }

  [Fact]
  public void Build_ShouldSucceed_WhenProvidedTitleAndVersionOnly()
  {
    var builder = new OpenApiInfoBuilder( EXPECTED_TITLE, EXPECTED_VERSION );

    var result = builder.Build();
    result.Should()
          .NotBeNull();

    result.IsSuccess.Should()
          .BeTrue();

    var info = result.Value;
    info.Title.Should()
        .Be( EXPECTED_TITLE );

    info.Version.Should()
        .Be( EXPECTED_VERSION );

    info.Contact.Should()
        .BeNull();

    info.Description.Should()
        .BeNull();

    info.Extensions.Should()
        .BeEmpty();

    info.License.Should()
        .BeNull();

    info.TermsOfService.Should()
        .BeNull();
  }

  [Fact]
  public void Build_ShouldSucceed_WhenUsingDefaultRuleSet()
  {
    var builder = new OpenApiInfoBuilder( EXPECTED_TITLE, EXPECTED_VERSION );

    var result = builder.Build( ValidationRuleSet.GetDefaultRuleSet() );
    result.Should()
          .NotBeNull();

    result.IsSuccess.Should()
          .BeTrue();

    var info = result.Value;
    info.Title.Should()
        .Be( EXPECTED_TITLE );

    info.Version.Should()
        .Be( EXPECTED_VERSION );

    info.Contact.Should()
        .BeNull();

    info.Description.Should()
        .BeNull();

    info.Extensions.Should()
        .BeEmpty();

    info.License.Should()
        .BeNull();

    info.TermsOfService.Should()
        .BeNull();
  }

  [Fact]
  public void Constructor_ShouldThrow_WhenTitleIsEmpty()
  {
    var act = () => new OpenApiInfoBuilder( string.Empty, EXPECTED_VERSION );
    act.Should()
       .Throw<ArgumentException>()
       .WithMessage( "The value cannot be an empty string. (Parameter 'title')" );
  }

  [Fact]
  public void Constructor_ShouldThrow_WhenTitleIsNull()
  {
    var act = () => new OpenApiInfoBuilder( null!, EXPECTED_VERSION );
    act.Should()
       .Throw<ArgumentException>()
       .WithMessage( "Value cannot be null. (Parameter 'title')" );
  }

  [Fact]
  public void Constructor_ShouldThrow_WhenVersionIsEmpty()
  {
    var act = () => new OpenApiInfoBuilder( EXPECTED_TITLE, string.Empty );
    act.Should()
       .Throw<ArgumentException>()
       .WithMessage( "The value cannot be an empty string. (Parameter 'version')" );
  }

  [Fact]
  public void Constructor_ShouldThrow_WhenVersionIsNull()
  {
    var act = () => new OpenApiInfoBuilder( EXPECTED_TITLE, null! );
    act.Should()
       .Throw<ArgumentException>()
       .WithMessage( "Value cannot be null. (Parameter 'version')" );
  }

  #endregion
}
