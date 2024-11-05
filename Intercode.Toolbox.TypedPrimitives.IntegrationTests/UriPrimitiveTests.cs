// Module Name: UriPrimitiveTests.cs
// Author:      Eduardo Velasquez
// Copyright (c) 2024, Intercode Consulting, Inc.

namespace Intercode.Toolbox.TypedPrimitives.IntegrationTests;

using System.ComponentModel;
using System.Text.Json;
using FluentAssertions;
using FluentResults;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using StjJsonException = System.Text.Json.JsonException;
using StjJsonSerializer = System.Text.Json.JsonSerializer;
using NsjJsonException = Newtonsoft.Json.JsonException;
using Newtonsoft.Json;

[TypedPrimitive<Uri>]
public readonly partial struct UnvalidatedUriPrimitive;

[TypedPrimitive(
  typeof( Uri ),
  Converters = TypedPrimitiveConverter.TypeConverter |
               TypedPrimitiveConverter.SystemTextJson |
               TypedPrimitiveConverter.EfCoreValueConverter |
               TypedPrimitiveConverter.NewtonsoftJson
)]
public readonly partial struct UriPrimitive
{
  #region Constants

  public const string ExpectedValidationErrorMessage = "Cannot be null or empty";
  public static readonly Uri InvalidUri = new ( "httpx://example.com" );

  #endregion

  #region Implementation

  static partial void ValidatePartial(
    Uri? value,
    ref Result result )
  {
    result = Result.FailIf(
      value is null || value == InvalidUri,
      ExpectedValidationErrorMessage
    );
  }

  #endregion
}

public class UriPrimitiveTests
{
  #region Constants

  private static readonly string s_jsonInvalidTokenTypeErrorMessage = "Value must be a String";
  private static readonly Uri s_validValueA = new ( "http://example.com/" );
  private static readonly Uri s_validValueB = new ( "https://example.com/api/" );

  #endregion

  #region Tests

  [Fact]
  public void CompareTo_DefaultAndValue_ReturnsLessThanZero()
  {
    // Arrange
    var a = ( UriPrimitive ) s_validValueA;
    var b = ( UriPrimitive ) default;

    // Act
    var result = b.CompareTo( a );

    // Assert
    result.Should()
          .BeLessThan( 0 );
  }

  [Fact]
  public void CompareTo_DifferentObjectValues_ReturnsComparisonResult()
  {
    var valueA = s_validValueA;
    var valueB = s_validValueB;

    // Arrange
    var a = ( UriPrimitive ) valueA;
    var b = ( UriPrimitive ) valueB;

    // Act
    var result = a.CompareTo( ( object ) b );

    // Assert
    result.Should()
          .Be( CompareUri( valueA, valueB ) );
  }

  [Fact]
  public void CompareTo_ObjectIsNull_ReturnsGreaterThanZero()
  {
    // Arrange
    var a = ( UriPrimitive ) s_validValueA;

    // Act
    var result = a.CompareTo( null );

    // Assert
    result.Should()
          .BeGreaterThan( 0 );
  }

  [Fact]
  public void CompareTo_ValueAndDefault_ReturnsGreaterThanZero()
  {
    // Arrange
    var primitive = ( UriPrimitive ) s_validValueA;

    // Act
    var result = primitive.CompareTo( default );

    // Assert
    result.Should()
          .BeGreaterThan( 0 );
  }

  [Fact]
  public void Equals_DefaultWithValue_ReturnsFalse()
  {
    // Arrange
    var a = ( UriPrimitive ) s_validValueA;
    var b = ( UriPrimitive ) default;

    // Act
    var areEqual = b.Equals( a );

    // Assert
    areEqual.Should()
            .BeFalse();
  }

  [Fact]
  public void Equals_DifferentValues_ReturnsFalse()
  {
    // Arrange
    var a = ( UriPrimitive ) s_validValueA;
    var b = ( UriPrimitive ) s_validValueB;

    // Act
    var areEqual = a.Equals( b );

    // Assert
    areEqual.Should()
            .BeFalse();
  }

  [Fact]
  public void Equals_SameValue_ReturnsTrue()
  {
    // Arrange
    var a = ( UriPrimitive ) s_validValueA;
    var b = ( UriPrimitive ) s_validValueA;

    // Act
    var areEqual = a.Equals( b );

    // Assert
    areEqual.Should()
            .BeTrue();
  }

  [Fact]
  public void Equals_ValueWithDefault_ReturnsFalse()
  {
    // Arrange
    var primitive = ( UriPrimitive ) s_validValueA;

    // Act
    var areEqual = primitive.Equals( default );

    // Assert
    areEqual.Should()
            .BeFalse();
  }

  [Fact]
  public void ExplicitOperator_DefaultToString_ShouldThrow()
  {
    // Arrange
    var primitive = ( UriPrimitive ) default;

    // Act
    var act = () => primitive.Value;

    // Assert
    act.Should()
       .Throw<InvalidOperationException>()
       .WithMessage( "Value is null" );
  }

  [Fact]
  public void ExplicitOperator_PrimitiveToString_ReturnsString()
  {
    // Arrange
    var value = s_validValueA;
    var primitive = ( UriPrimitive ) value;

    // Act
    var result = primitive.Value;

    // Assert
    result.Should()
          .Be( value );
  }

  [Theory]
  [MemberData( nameof( InvalidValues ) )]
  public void ExplicitOperator_StringToPrimitive_WithInvalidValue_Throws(
    Uri? value )
  {
    // Act
    var act = () => ( UriPrimitive ) value;

    // Assert
    act.Should()
       .Throw<InvalidOperationException>()
       .WithMessage( UriPrimitive.ExpectedValidationErrorMessage );
  }

  [Fact]
  public void ExplicitOperator_ValueToPrimitive_ReturnsPrimitiveWithValue()
  {
    // Act
    var value = s_validValueA;
    var primitive = ( UriPrimitive ) value;

    // Assert
    primitive.Value.Should()
             .Be( value );
  }

  [Fact]
  public void FromInt32_WithValidValue_ReturnsSuccess()
  {
    // Act
    var value = s_validValueA;
    var result = UriPrimitive.Create( value );

    // Assert
    result.IsSuccess.Should()
          .BeTrue();

    result.Value.Value.Should()
          .Be( value );
  }

  [Theory]
  [MemberData( nameof( InvalidValues ) )]
  public void FromString_InvalidValue_ReturnsFailure(
    Uri? value )
  {
    // Act
    var result = UriPrimitive.Create( value );

    // Assert
    result.IsFailed.Should()
          .BeTrue();

    result.Errors.Select( error => error.Message )
          .Should()
          .ContainSingle()
          .Which
          .Should()
          .Be( UriPrimitive.ExpectedValidationErrorMessage );
  }

  [Fact]
  public void GetHashCode_Default_ReturnsZero()
  {
    // Act
    var hashCodeA = ( ( UriPrimitive ) default ).GetHashCode();

    // Assert
    hashCodeA.Should()
             .Be( 0 );
  }

  [Fact]
  public void GetHashCode_DifferentValues_ReturnsDifferentHashCodes()
  {
    // Arrange
    var valueA = s_validValueA;
    var valueB = s_validValueB;
    var a = ( UriPrimitive ) valueA;
    var b = ( UriPrimitive ) valueB;

    // Act
    var hashCodeA = a.GetHashCode();
    var hashCodeB = b.GetHashCode();

    // Assert
    hashCodeA.Should()
             .NotBe( hashCodeB );
  }

  [Fact]
  public void GetHashCode_SameValue_ReturnsSameHashCode()
  {
    // Arrange
    var value = s_validValueA;
    var a = ( UriPrimitive ) value;
    var b = ( UriPrimitive ) value;

    // Act
    var hashCodeA = a.GetHashCode();
    var hashCodeB = b.GetHashCode();

    // Assert
    hashCodeA.Should()
             .Be( hashCodeB );
  }

  [Fact]
  public void IsDefault_WithDefaultValue_ReturnsTrue()
  {
    // Arrange
    var primitive = ( UriPrimitive ) default;

    // Act
    var result = primitive.IsDefault;

    result.Should()
          .BeTrue();
  }

  [Fact]
  public void IsDefault_WithValue_ReturnsFalse()
  {
    // Arrange
    var primitive = ( UriPrimitive ) s_validValueA;

    // Act
    var result = primitive.IsDefault;

    result.Should()
          .BeFalse();
  }

  [Theory]
  [MemberData( nameof( InvalidValues ) )]
  public void IsValid_InvalidValue_ReturnsFalse(
    Uri? value )
  {
    // Act
    var isValid = UriPrimitive.IsValid( value );

    // Assert
    isValid.Should()
           .BeFalse();
  }

  [Fact]
  public void IsValid_StringValidValue_ReturnsTrue()
  {
    // Act
    var isValid = UriPrimitive.IsValid( s_validValueA );

    // Assert
    isValid.Should()
           .BeTrue();
  }

  [Theory]
  [MemberData( nameof( InvalidValues ) )]
  public void NewtonsoftJson_Deserialization_InvalidValue_ShouldThrow(
    Uri? value )
  {
    var asString = value is null ? "null" : $"\"{value}\"";
    var json = $$"""{"Primitive":{{asString}}}""";

    var act = () => JsonConvert.DeserializeObject<JsonTestClass>( json );

    act.Should()
       .Throw<NsjJsonException>()
       .WithMessage( UriPrimitive.ExpectedValidationErrorMessage );
  }

  [Fact]
  public void NewtonsoftJson_Deserialization_WithNull_ShouldThrow()
  {
    var json = """{"Primitive":null}""";

    var act = () => JsonConvert.DeserializeObject<JsonTestClass>( json );

    act.Should()
       .Throw<NsjJsonException>()
       .WithMessage( UriPrimitive.ExpectedValidationErrorMessage );
  }

  [Fact]
  public void NewtonsoftJson_Deserialization_WithValidValue_ShouldSucceed()
  {
    var value = s_validValueA;
    var json = $$"""{"Primitive":"{{value}}"}""";

    var result = JsonConvert.DeserializeObject<JsonTestClass>( json );

    result.Should()
          .NotBeNull();

    result!.Primitive.Value.Should()
           .Be( value );
  }

  [Fact]
  public void NewtonsoftJson_Serialization_WithDefault_ShouldSucceed()
  {
    var test = new JsonTestClass { Primitive = default };

    var json = JsonConvert.SerializeObject( test );

    json.Should()
        .Be( """{"Primitive":null}""" );
  }

  [Fact]
  public void NewtonsoftJson_Serialization_WithValidValue_ShouldSucceed()
  {
    var value = s_validValueA;
    var test = new JsonTestClass { Primitive = ( UriPrimitive ) value };

    var json = JsonConvert.SerializeObject( test );

    json.Should()
        .Be( $$"""{"Primitive":"{{value}}"}""" );
  }

  [Fact]
  public void SystemTextJson_Deserialization_WithInvalidType_ShouldThrow()
  {
    var json = """{"Primitive":12345}""";

    var act = () => StjJsonSerializer.Deserialize<JsonTestClass>( json );

    act.Should()
       .Throw<StjJsonException>()
       .WithMessage( s_jsonInvalidTokenTypeErrorMessage );
  }

  [Fact]
  public void SystemTextJson_Deserialization_WithNull_ShouldThrow()
  {
    var json = """{"Primitive":null}""";

    var act = () => StjJsonSerializer.Deserialize<JsonTestClass>( json );

    act.Should()
       .Throw<StjJsonException>()
       .WithMessage( UriPrimitive.ExpectedValidationErrorMessage );
  }

  [Fact]
  public void SystemTextJson_Deserialization_WithValidValue_ShouldSucceed()
  {
    var value = s_validValueA;
    var json = $$"""{"Primitive":"{{value}}"}""";

    var result = StjJsonSerializer.Deserialize<JsonTestClass>( json );

    result.Should()
          .NotBeNull();

    result!.Primitive.Value.Should()
           .Be( value );
  }

  [Theory]
  [MemberData( nameof( InvalidValues ) )]
  public void SystemTextJson_DeserializationInvalidValue_ShouldThrow(
    Uri? value )
  {
    var asString = value is null ? "null" : $"\"{value}\"";
    var json = $$"""{"Primitive":{{asString}}}""";

    var act = () => StjJsonSerializer.Deserialize<JsonTestClass>( json );

    act.Should()
       .Throw<StjJsonException>()
       .WithMessage( UriPrimitive.ExpectedValidationErrorMessage );
  }

  [Fact]
  public void SystemTextJson_Serialization_WithDefault_ShouldSucceed()
  {
    var test = new JsonTestClass { Primitive = default };

    var json = StjJsonSerializer.Serialize( test );

    json.Should()
        .Be( """{"Primitive":null}""" );
  }

  [Fact]
  public void SystemTextJson_Serialization_WithValidValue_ShouldSucceed()
  {
    var value = s_validValueA;
    var expected = new JsonTestClass { Primitive = ( UriPrimitive ) value };

    var json = StjJsonSerializer.Serialize( expected );

    // Use roundtrip to avoid JSON encoding failures on Linux.
    // This is safer anyway, as it ensures the JSON is valid.
    var actual = StjJsonSerializer.Deserialize<JsonTestClass>( json );

    actual.Should()
          .NotBeNull();

    actual!.Primitive.Value.Should()
           .Be( value );
  }

  [Fact]
  public void ToString_Default_ReturnsEmpty()
  {
    // Arrange
    var primitive = ( UriPrimitive ) default;

    // Act
    var result = primitive.ToString();

    // Assert
    result.Should()
          .BeEmpty();
  }

  [Fact]
  public void ToString_ReturnsValue()
  {
    // Arrange
    var value = s_validValueA;
    var primitive = ( UriPrimitive ) value;

    // Act
    var result = primitive.ToString();

    // Assert
    result.Should()
          .Be( value.ToString() );
  }

  [Fact]
  public void TypeConverter_CanConvertFrom_SupportedType_ReturnsTrue()
  {
    var converter = TypeDescriptor.GetConverter( typeof( UriPrimitive ) );
    converter.CanConvertFrom( typeof( Uri ) )
             .Should()
             .BeTrue();
  }

  [Fact]
  public void TypeConverter_CanConvertTo_SupportedType_ReturnsTrue()
  {
    var converter = TypeDescriptor.GetConverter( typeof( UriPrimitive ) );
    converter.CanConvertTo( typeof( Uri ) )
             .Should()
             .BeTrue();
  }

  [Fact]
  public void TypeConverter_CanConvertTo_UnsupportedType_ReturnsFalse()
  {
    var converter = TypeDescriptor.GetConverter( typeof( UriPrimitive ) );
    converter.CanConvertTo( typeof( string ) )
             .Should()
             .BeFalse();
  }

  [Fact]
  public void TypeConverter_ConvertFrom_NullValue_ReturnsDefault()
  {
    var converter = TypeDescriptor.GetConverter( typeof( UriPrimitive ) );
    var result = ( UriPrimitive ) converter.ConvertFrom( null! )!;

    result.Should()
          .BeOfType<UriPrimitive>()
          .Which.Should()
          .Be( default );
  }

  [Fact]
  public void TypeConverter_ConvertFrom_SupportedType_Succeeds()
  {
    var value = s_validValueA;
    var converter = TypeDescriptor.GetConverter( typeof( UriPrimitive ) );
    var result = converter.ConvertFrom( value );

    result.Should()
          .BeOfType<UriPrimitive>()
          .Which.Value.Should()
          .Be( value );
  }

  [Fact]
  public void TypeConverter_ConvertFrom_UnsupportedType_Throws()
  {
    var converter = TypeDescriptor.GetConverter( typeof( UriPrimitive ) );

    var value = "12345";
    var act = () => converter.ConvertFrom( value );

    act.Should()
       .Throw<NotSupportedException>();
  }

  [Fact]
  public void TypeConverter_ConvertTo_SupportedType_Succeeds()
  {
    var converter = TypeDescriptor.GetConverter( typeof( UriPrimitive ) );

    var value = s_validValueA;
    var primitive = ( UriPrimitive ) value;
    var result = converter.ConvertTo( null, null, primitive, typeof( Uri ) );

    result.Should()
          .BeOfType<Uri>()
          .Which.Should()
          .Be( value );
  }

  [Fact]
  public void TypeConverter_ConvertTo_UnsupportedType_Throws()
  {
    var converter = TypeDescriptor.GetConverter( typeof( UriPrimitive ) );

    var primitive = ( UriPrimitive ) s_validValueA;
    var act = () => converter.ConvertTo( null, null, primitive, typeof( string ) );

    act.Should()
       .Throw<NotSupportedException>();
  }

  [Fact]
  public void TypeConverter_IsFound()
  {
    var converter = TypeDescriptor.GetConverter( typeof( UriPrimitive ) );
    converter.Should()
             .NotBeNull();
  }

  [Theory]
  [MemberData( nameof( InvalidValues ) )]
  public void Validate_InvalidValue_ReturnsFailure(
    Uri? value )
  {
    // Act
    var result = UriPrimitive.Validate( value );

    // Assert
    result.IsFailed.Should()
          .BeTrue();

    result.Errors.Select( error => error.Message )
          .Should()
          .ContainSingle()
          .Which
          .Should()
          .Be( UriPrimitive.ExpectedValidationErrorMessage );
  }

  [Fact]
  public void Validate_ValidValue_ReturnsSuccess()
  {
    // Act
    var result = UriPrimitive.Validate( s_validValueA );

    // Assert
    result.IsSuccess.Should()
          .BeTrue();
  }

  [Theory]
  [MemberData( nameof( InvalidValues ) )]
  public void Validate_WithUnvalidatedPrimitiveAndInvalidValue_ReturnsSuccess(
    Uri? value )
  {
    // Act
    var result = UnvalidatedUriPrimitive.Validate( value );

    // Assert
    result.IsSuccess.Should()
          .BeTrue();
  }

  [Fact]
  public void ValueConverter_WithValidValue_Succeeds()
  {
    // Arrange
    using var context = new TestDbContext();
    context.Database.EnsureCreated();

    var id = Guid.NewGuid();
    var value = s_validValueA;
    var entity = new TestEntity
    {
      Id = id,
      Primitive = ( UriPrimitive ) value
    };

    // Act
    context.Entities.Add( entity );
    context.SaveChanges();

    // Assert
    var result = context.Entities.Single( e => e.Id == id );

    result.Primitive.Value.Should()
          .Be( value );
  }

  #endregion

  #region Implementation

  private static int CompareUri(
    Uri? a,
    Uri? b )
  {
    if( a is null && b is null )
    {
      return 0;
    }

    if( a is null )
    {
      return -1;
    }

    if( b is null )
    {
      return 1;
    }

    return Uri.Compare( a, b, UriComponents.AbsoluteUri, UriFormat.SafeUnescaped, StringComparison.OrdinalIgnoreCase );
  }

  public static class Validator
  {
    #region Public Methods

    public static Result Validate(
      Uri? value )
    {
      return Result.FailIf(
        value is null || value == UriPrimitive.InvalidUri,
        UriPrimitive.ExpectedValidationErrorMessage
      );
    }

    #endregion
  }

  public static IEnumerable<object?[]> InvalidValues => new List<object?[]>
  {
    new object?[] { null },
    new object?[] { UriPrimitive.InvalidUri }
  };

  internal class JsonTestClass
  {
    #region Properties

    public UriPrimitive Primitive { get; set; }

    #endregion
  }

  internal class TestEntity
  {
    #region Properties

    public Guid Id { get; set; } = Guid.NewGuid();
    public UriPrimitive Primitive { get; set; }

    #endregion
  }

  internal class TestDbContext: DbContext
  {
    #region Constructors

    public TestDbContext()
    {
    }

    public TestDbContext(
      DbContextOptions options )
      : base( options )
    {
    }

    #endregion

    #region Properties

    public DbSet<TestEntity> Entities { get; set; } = null!;

    #endregion

    #region Implementation

    protected override void OnModelCreating(
      ModelBuilder modelBuilder )
    {
      modelBuilder.Entity<TestEntity>()
                  .Property( e => e.Primitive )
                  .HasConversion( new UriPrimitiveValueConverter() )
                  .ValueGeneratedNever();
    }

    protected override void OnConfiguring(
      DbContextOptionsBuilder optionsBuilder )
    {
      var connection = new SqliteConnection( "DataSource=:memory:" );
      connection.Open();

      optionsBuilder.UseSqlite( connection );
    }

    #endregion
  }

  #endregion
}
