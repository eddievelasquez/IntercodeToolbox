# Intercode.Toolbox.AspNetCore.Extensions

A trimmable, AOT-compatible .NET library that contains types that provide functionality commonly used in ASP.NET Core applications.

---

## `JsonWebTokenBuilder` class

The `JsonWebTokenBuilder` class provides a fluent interface to create JSON Web Tokens (JWT) using the `System.IdentityModel.Tokens.Jwt` library.

### Usage

To create a JWT, create a `JsonWebTokenBuilder` instance class and call the `BuildEncoded` method after setting the token's properties.
The `BuildEncoded` method returns a [Result&lt;string&gt;](https://github.com/altmann/FluentResults) object containing the JWT if the 
provided token information is valid; otherwise, it returns a failure result with one or more `JwtValidationResultError` objects.

```csharp
var builder = new JsonWebTokenBuilder( "<<<My secret signing key>>>" )
builder.SetIssuer( "MyIssuer" )
       .AddAudience( "MyAudience" )
       .SetSubject( "sub-012345" )
       .SetTokenId( "my-JTI-value" )
       .SetLifetime( TimeSpan.FromMinutes( 30 ) )
       .AddRole( "Admin" )
       .AddClaim( "my-custom-claim", "my-claim-value" );

var result = builder.BuildEncoded();
if ( result.IsSuccess )
{
    string token = result.Value;
    // Use the token...
}
else
{
    // Handle the validation errors...
    var errors = result.Errors;
}
```

To obtain a [JwtSecurityToken](https://learn.microsoft.com/en-us/dotnet/api/system.identitymodel.tokens.jwt.jwtsecuritytoken) instance, call the `Build` method instead, 
which returns a [Result&lt;JwtSecurityToken&gt;](https://github.com/altmann/FluentResults) instance:

```csharp
var builder = new JsonWebTokenBuilder( "<<<My secret signing key>>>" );
// Set token values...
var result = builder.Build();
if ( result.IsSuccess )
{
    JwtSecurityToken token = result.Value;
    // Use the JwtSecurityToken instance...
}
else
{
    // Handle the validation errors...
    var errors = result.Errors;
}
```

### Reference

#### Constructors

- Pass the secret key used to sign the JWT. Optionally, you can specify the signature algorithm and a custom time provider.
By default, the builder uses the [SecurityAlgorithms.HmacSha512Signature](https://learn.microsoft.com/en-us/dotnet/api/microsoft.identitymodel.tokens.securityalgorithms.hmacsha512signature) algorithm 
and the [TimeProvider.System](https://learn.microsoft.com/en-us/dotnet/api/system.timeprovider.system) time provider.
The default signature algorithm can be changed by setting the `DefaultSignatureAlgorithm` static property.

```csharp
JsonWebTokenBuilder(
    string signingKey,
    string? signatureAlgorithm = null,
    TimeProvider? timeProvider = null )
```

- Pass the `SigningCredentials` instance used to sign the JWT. Optionally, you can specify a custom time provider.

```csharp
JsonWebTokenBuilder(
    SigningCredentials signingCredentials,
    TimeProvider? timeProvider = null )
```

### Methods

- Set the token's identifier (**jti**) claim.
 
```csharp
JsonWebTokenBuilder SetTokenId( string tokenId )
```

- Set the token's issuer (**iss**) claim 

```csharp
JsonWebTokenBuilder SetIssuer( string issuer )
```

- Set the token's issued at (**iat**) claim. If not set, the current date and time is used.

```csharp
JsonWebTokenBuilder SetIssuedAt( DateTime? issuedAt )
JsonWebTokenBuilder SetIssuedAt( DateTimeOffset? issuedAt )
```

- Sets the token's expiration time (**exp**) claim.

```csharp
JsonWebTokenBuilder SetValidTo( DateTime? validTo )
JsonWebTokenBuilder SetValidTo( DateTimeOffset? validTo )
```

- Set the token's lifetime by specifying the expiration time relative to `issuedAt` date and time.

```csharp
JsonWebTokenBuilder SetLifeTime( TimeSpan lifeTime )
```

- Set the token's not before (**nbf**) claim.

```csharp
JsonWebTokenBuilder SetValidFrom( DateTime? validFrom )
JsonWebTokenBuilder SetValidFrom( DateTimeOffset? validFrom )
```

- Add an audience (**aud**) claim to the token.

```csharp
JsonWebTokenBuilder AddAudience( string audience )
```

- Set the token's subject (**sub**) claim.

```csharp
JsonWebTokenBuilder SetSubject( string subject )
 ```

- Add a `ClaimTypes.Role` claim to the token.

```csharp
JsonWebTokenBuilder AddRole( string role )
```

- Add a custom claim to the token.

```csharp
JsonWebTokenBuilder AddClaim( string claimName, object value )
```

- Generate a `JwtSecurityToken` instance from the builder's current state. The `requiredClaims` parameter specifies which claims are required to be present in the token.

```csharp
Result<JwtSecurityToken> Build( RequiredClaim requiredClaims = RequiredClaim.Default )
```

- Generate a JWT string from the builder's current state. The `requiredClaims` parameter specifies which claims are required to be present in the token.

```csharp
Result<string> BuildEncoded( RequiredClaim requiredClaims = RequiredClaim.Default )
```

---

## `RequiredClaim` enumeration

The `RequireClaim` flag enumeration specifies which claims are required to be present in the JWT.

### Values

- `None` - No claims are required.
- `Subject` - The subject (**sub**) claim is required.
- `Issuer` - The issuer (**iss**) claim is required.
- `Audience` - At least one audience (**aud**) claim is required.
- `Role` - At least one role (**ClaimTypes.Role**) claim is required.
- `Jti` - The token identifier (**jti**) claim is required; a [Guid](https://learn.microsoft.com/en-us/dotnet/api/system.guid) will be automatically generated if not explicitly set.
- `Default` - The **subject**, **issuer**, **JTI**, and at least one **audience** and **role** claims are required.

---

## `OpenApiInfoBuilder` class

The `OpenApiInfoBuilder` class provides a fluent interface for the creation of `OpenApiInfo` instances used to configure the OpenAPI document in ASP.NET Core applications.

### Usage

To create an `OpenApiInfo` instance, create an `OpenApiInfoBuilder` instance and set the desired properties.
The `Build` method returns a [Result&lt;OpenApiInfo&gt;](https://github.com/altmann/FluentResults) object containing a [OpenApiInfo](https://learn.microsoft.com/en-us/dotnet/api/Microsoft.OpenApi.Models.OpenApiInfo) 
instance if it was valid according to the [ValidationRuleSet](https://learn.microsoft.com/en-us/dotnet/api/microsoft.openapi.validations.validationruleset) that was provided; 
otherwise, it returns a failure result with one or more `OpenApiValidationResultError` objects.

```csharp
var builder = new OpenApiInfoBuilder( "My API title", "v1" );
builder.AddContact( "Contact Name", "contact@example.com", "https://example.com/contact" )
       .AddDescription( "This is an example API" )
       .AddExtension( "x-example-extension", "example-extension-value" )")
       .AddLicense( "MyLicense", "https://example.com/license" )
       .AddTermsOfService( "https://example.com/terms-of-service" );

var result = builder.Build();
if ( result.IsSuccess )
{
    OpenApiInfo info = result.Value;
    // Use the OpenApiInfo instance...
}
else
{
    // Handle the validation errors...
    var errors = result.Errors;
}
```

### Reference

- Creates a new instance of the `OpenApiInfoBuilder` class with the specified title and version.

```csharp
OpenApiInfoBuilder( string title, string version )
```

- Set the API's contact information, you can specify the contact's name, email, URL, and optionally add contact custom extensions.

```csharp
OpenApiInfoBuilder AddContact( string? name, string? email, string? url = null, 
                               IDictionary<string, IOpenApiExtension>? extensions = null )
OpenApiInfoBuilder AddContact( string? name, string? email, Uri? uri = null, 
                               IDictionary<string, IOpenApiExtension>? extensions = null )
```

- Set the API's description.

```csharp
OpenApiInfoBuilder AddDescription( string description )
```

- Append custom extensions to the API.

```csharp
OpenApiInfoBuilder AddExtensions( IDictionary<string, IOpenApiExtension>? extensions )
```

- Append a custom extension to the API. 

```csharp
OpenApiInfoBuilder AddExtension( string key, IOpenApiExtension extension )
```

- Set the API's license information; you can specify the license's name, URL, and optionally add license custom extensions.

```csharp
OpenApiInfoBuilder AddLicense( string? name, string? url,
                               IDictionary<string, IOpenApiExtension>? extensions = null )
```

- Set the link to the API's terms of service.

```csharp
OpenApiInfoBuilder AddTermsOfService( string? url )
OpenApiInfoBuilder AddTermsOfService( Uri? uri )
```

- Build the `OpenApiInfo` instance from the builder's current state. The `ruleSet` parameter specifies the set of validation rules to apply to the builder's properties.
By default, the builder validates using the rule set returned by [ValidationRuleSet.GetDefaultRuleSet()](https://learn.microsoft.com/en-us/dotnet/api/microsoft.openapi.validations.validationruleset.getdefaultruleset).
If no validation is to be performed, pass [ValidationRuleSet.GetEmptyRuleSet()](https://learn.microsoft.com/en-us/dotnet/api/microsoft.openapi.validations.validationruleset.getemptyruleset) as the `ruleSet` parameter.<br>
A [Result&lt;OpenApiInfo&gt;](https://github.com/altmann/FluentResults) containing the built [OpenApiInfo](https://learn.microsoft.com/en-us/dotnet/api/Microsoft.OpenApi.Models.OpenApiInfo) object if it is valid;
otherwise a failure result with one or more `OpenApiValidationResultError` objects.

```csharp
Result<OpenApiInfo> Build( ValidationRuleSet? ruleSet = null )
```

---

## `OpenApiValidationResultError` class
The `OpenApiValidationResultError` class represents a validation error that occurred while building an `OpenApiInfo` instance using the `OpenApiInfoBuilder` class.
The actual [OpenApiError](https://learn.microsoft.com/en-us/dotnet/api/microsoft.openapi.models.openapierror) validation error is found in the `Metadata` property, 
using the `error` key

```csharp
class OpenApiValidationResultError
{
  // Initializes a new instance of the <see cref="OpenApiValidationResultError" /> class.
  public OpenApiValidationResultError( OpenApiError error )
  
  // Gets the error message.
  public string Message { get; }

  // Gets the metadata associated with the error.
  public Dictionary<string, object> Metadata { get; }

  // Gets the list of reasons for the error.
  public List<IError> Reasons { get; }
}
```

---

## License

This project is licensed under the [MIT License](LICENSE).
