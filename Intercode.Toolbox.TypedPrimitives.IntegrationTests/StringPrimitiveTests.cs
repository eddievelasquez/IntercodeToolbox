// Module Name: StringPrimitiveTests.cs
// Author:      Eduardo Velasquez
// Copyright (c) 2024, Intercode Consulting, Inc.

namespace Intercode.Toolbox.TypedPrimitives.IntegrationTests;

using System.ComponentModel;
using System.Globalization;
using System.Text.Json;
using FluentAssertions;
using FluentResults;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using SystemTextJsonException = System.Text.Json.JsonException;
using SystemTextJsonSerializer = System.Text.Json.JsonSerializer;

[TypedPrimitive<string>]
public readonly partial struct UnvalidatedStringPrimitive;

[TypedPrimitive(
  typeof( string ),
  Converters = TypedPrimitiveConverter.TypeConverter |
               TypedPrimitiveConverter.SystemTextJson |
               TypedPrimitiveConverter.EfCoreValueConverter |
               TypedPrimitiveConverter.NewtonsoftJson
)]
public readonly partial struct StringPrimitive
{
  #region Constants

  public const string ExpectedValidationErrorMessage = "Cannot be null or empty";

  #endregion

  #region Implementation

  static partial void ValidatePartial(
    string? value,
    ref Result result )
  {
    result = Result.FailIf( string.IsNullOrWhiteSpace( value ), ExpectedValidationErrorMessage );
  }

  #endregion
}

// A Newtonsoft JSON converter for StringPrimitive to support long values
public partial class StringPrimitiveNewtonsoftJsonConverter
{
  #region Implementation

  partial void ConvertToPartial(
    ref JsonReader reader,
    Type typeToConvert,
    ref string? convertedValue,
    ref bool converted )
  {
    if( reader.TokenType != JsonToken.Integer )
    {
      return;
    }

    var l = ( long ) reader.Value!;
    convertedValue = l.ToString( "D9" );
    converted = true;
  }

  #endregion
}

// A System.Text JSON converter for StringPrimitive to support long values
public partial class StringPrimitiveSystemTextJsonConverter
{
  #region Implementation

  partial void ConvertToPartial(
    ref Utf8JsonReader reader,
    Type typeToConvert,
    JsonSerializerOptions options,
    ref string? value,
    ref bool converted )
  {
    if( reader.TokenType != JsonTokenType.Number )
    {
      return;
    }

    var l = reader.GetInt64();
    value = l.ToString( "D9" );
    converted = true;
  }

  #endregion
}

// A Type converter for StringPrimitive to support long values
public partial class StringPrimitiveTypeConverter
{
  #region Implementation

  partial void CanConvertFromPartial(
    ITypeDescriptorContext? context,
    Type sourceType,
    ref bool canConvert )
  {
    canConvert = sourceType == typeof( long );
  }

  partial void ConvertFromPartial(
    ITypeDescriptorContext? context,
    CultureInfo? culture,
    object value,
    ref string? convertedValue,
    ref bool converted )
  {
    if( value is long l )
    {
      convertedValue = l.ToString( "D9" );
      converted = true;
    }
  }

  partial void CanConvertToPartial(
    ITypeDescriptorContext? context,
    Type? destinationType,
    ref bool canConvert )
  {
    canConvert = destinationType == typeof( long );
  }

  partial void ConvertToPartial(
    ITypeDescriptorContext? context,
    CultureInfo? culture,
    string? value,
    Type destinationType,
    ref object? convertedValue,
    ref bool converted )
  {
    if( value is not null && destinationType == typeof( long ) )
    {
      convertedValue = long.Parse( value );
      converted = true;
    }
  }

  #endregion
}

public class StringPrimitiveTests
{
  #region Constants

  private static readonly string s_jsonInvalidTokenTypeErrorMessage = "Value must be a string";
  private static readonly string s_validValueA = "valueA";
  private static readonly string s_validValueB = "valueB";

  #endregion

  #region Tests

  [Fact]
  public void CompareTo_DefaultAndValue_ReturnsLessThanZero()
  {
    // Arrange
    var a = ( StringPrimitive ) s_validValueA;
    var b = ( StringPrimitive ) default;

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
    var a = ( StringPrimitive ) valueA;
    var b = ( StringPrimitive ) valueB;

    // Act
    var result = a.CompareTo( ( object ) b );

    // Assert
    result.Should()
          .Be( string.CompareOrdinal( valueA, valueB ) );
  }

  [Fact]
  public void CompareTo_ObjectIsNull_ReturnsGreaterThanZero()
  {
    // Arrange
    var a = ( StringPrimitive ) s_validValueA;

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
    var primitive = ( StringPrimitive ) s_validValueA;

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
    var a = ( StringPrimitive ) s_validValueA;
    var b = ( StringPrimitive ) default;

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
    var a = ( StringPrimitive ) s_validValueA;
    var b = ( StringPrimitive ) s_validValueB;

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
    var a = ( StringPrimitive ) s_validValueA;
    var b = ( StringPrimitive ) s_validValueA;

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
    var primitive = ( StringPrimitive ) s_validValueA;

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
    var primitive = ( StringPrimitive ) default;

    // Act
    var act = () => primitive.Value;

    // Assert
    act.Should()
       .Throw<InvalidOperationException>()
       .WithMessage( "Value is null" );
  }

  [Fact]
  public void ExplicitOperator_PrimitiveToString_ReturnsValue()
  {
    // Arrange
    var value = s_validValueA;
    var primitive = ( StringPrimitive ) value;

    // Act
    var result = primitive.Value;

    // Assert
    result.Should()
          .Be( value );
  }

  [Fact]
  public void ExplicitOperator_StringToPrimitive_ReturnsPrimitiveWithValue()
  {
    // Act
    var value = s_validValueA;
    var primitive = ( StringPrimitive ) value;

    // Assert
    primitive.Value.Should()
             .Be( value );
  }

  [Theory]
  [MemberData( nameof( InvalidValues ) )]
  public void ExplicitOperator_StringToPrimitive_WithInvalidValue_Throws(
    string? value )
  {
    // Act
    var act = () => ( StringPrimitive ) value;

    // Assert
    act.Should()
       .Throw<InvalidOperationException>()
       .WithMessage( StringPrimitive.ExpectedValidationErrorMessage );
  }

  [Theory]
  [MemberData( nameof( InvalidValues ) )]
  public void FromSpan_InvalidValue_ReturnsFailure(
    string? value )
  {
    // Act
    var result = StringPrimitive.Create( value );

    // Assert
    result.IsFailed.Should()
          .BeTrue();

    result.Errors.Select( error => error.Message )
          .Should()
          .ContainSingle()
          .Which
          .Should()
          .Be( StringPrimitive.ExpectedValidationErrorMessage );
  }

  [Fact]
  public void FromSpan_ValidValue_ReturnsSuccess()
  {
    // Act
    var value = s_validValueA;
    var result = StringPrimitive.Create( value );

    // Assert
    result.IsSuccess.Should()
          .BeTrue();

    result.Value.Value.Should()
          .Be( value );
  }

  [Theory]
  [MemberData( nameof( InvalidValues ) )]
  public void FromString_InvalidValue_ReturnsFailure(
    string? value )
  {
    // Act
    var result = StringPrimitive.Create( value );

    // Assert
    result.IsFailed.Should()
          .BeTrue();

    result.Errors.Select( error => error.Message )
          .Should()
          .ContainSingle()
          .Which
          .Should()
          .Be( StringPrimitive.ExpectedValidationErrorMessage );
  }

  [Fact]
  public void FromString_WithValidValue_ReturnsSuccess()
  {
    // Act
    var value = s_validValueA;
    var result = StringPrimitive.Create( value );

    // Assert
    result.IsSuccess.Should()
          .BeTrue();

    result.Value.Value.Should()
          .Be( value );
  }

  [Fact]
  public void GetHashCode_Default_ReturnsZero()
  {
    // Act
    var hashCodeA = ( ( StringPrimitive ) default ).GetHashCode();

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
    var a = ( StringPrimitive ) valueA;
    var b = ( StringPrimitive ) valueB;

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
    var a = ( StringPrimitive ) value;
    var b = ( StringPrimitive ) value;

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
    var primitive = ( StringPrimitive ) default;

    // Act
    var result = primitive.IsDefault;

    result.Should()
          .BeTrue();
  }

  [Fact]
  public void IsDefault_WithValue_ReturnsFalse()
  {
    // Arrange
    var primitive = ( StringPrimitive ) s_validValueA;

    // Act
    var result = primitive.IsDefault;

    result.Should()
          .BeFalse();
  }

  [Theory]
  [MemberData( nameof( InvalidValues ) )]
  public void IsValid_StringInvalidValued_ReturnsFalse(
    string? value )
  {
    // Act
    var isValid = StringPrimitive.IsValid( value );

    // Assert
    isValid.Should()
           .BeFalse();
  }

  [Fact]
  public void IsValid_StringValidValue_ReturnsTrue()
  {
    // Act
    var isValid = StringPrimitive.IsValid( s_validValueA );

    // Assert
    isValid.Should()
           .BeTrue();
  }

  [Fact]
  public void NewtonsoftJson_Deserialization_WithInvalidType_ShouldThrow()
  {
    var json = """{"StringPrimitive":true}""";

    // The JSON deserializer should use StringPrimitive's SystemTextJsonConverter
    var act = () => JsonConvert.DeserializeObject<JsonTestClass>( json );

    act.Should()
       .Throw<JsonSerializationException>()
       .WithMessage( s_jsonInvalidTokenTypeErrorMessage );
  }

  [Fact]
  public void NewtonsoftJson_Deserialization_WithLongValue_ShouldSucceed()
  {
    var value = 049541698L;
    var json = $$"""{"StringPrimitive":{{value}}}""";

    // The JSON deserializer should use StringPrimitive's SystemTextJsonConverter
    var result = JsonConvert.DeserializeObject<JsonTestClass>( json );

    result.Should()
          .NotBeNull();

    result!.StringPrimitive.Value.Should()
           .Be( value.ToString( "D9" ) );
  }

  [Fact]
  public void NewtonsoftJson_Deserialization_WithNull_ShouldThrow()
  {
    var json = """{"StringPrimitive":null}""";

    // The JSON deserializer should use StringPrimitive's SystemTextJsonConverter
    var act = () => JsonConvert.DeserializeObject<JsonTestClass>( json );

    act.Should()
       .Throw<JsonSerializationException>()
       .WithMessage( StringPrimitive.ExpectedValidationErrorMessage );
  }

  [Fact]
  public void NewtonsoftJson_Deserialization_WithValidValue_ShouldSucceed()
  {
    var value = s_validValueA;
    var json = $$"""{"StringPrimitive":"{{value}}"}""";

    // The JSON deserializer should use StringPrimitive's SystemTextJsonConverter
    var result = JsonConvert.DeserializeObject<JsonTestClass>( json );

    result.Should()
          .NotBeNull();

    result!.StringPrimitive.Value.Should()
           .Be( value );
  }

  [Theory]
  [MemberData( nameof( InvalidValues ) )]
  public void NewtonsoftJson_DeserializationInvalidValue_ShouldThrow(
    string? value )
  {
    var json = $$"""{"StringPrimitive":"{{value}}"}""";

    // The JSON deserializer should use StringPrimitive's SystemTextJsonConverter
    var act = () => JsonConvert.DeserializeObject<JsonTestClass>( json );

    act.Should()
       .Throw<JsonSerializationException>()
       .WithMessage( StringPrimitive.ExpectedValidationErrorMessage );
  }

  [Fact]
  public void NewtonsoftJson_Serialization_WithDefault_ShouldSucceed()
  {
    var test = new JsonTestClass { StringPrimitive = default };

    // The JSON serializer should use StringPrimitive's SystemTextJsonConverter
    var json = JsonConvert.SerializeObject( test );

    json.Should()
        .Be( """{"StringPrimitive":null}""" );
  }

  [Fact]
  public void NewtonsoftJson_Serialization_WithValidValue_ShouldSucceed()
  {
    var value = s_validValueA;
    var test = new JsonTestClass { StringPrimitive = ( StringPrimitive ) value };

    // The JSON serializer should use StringPrimitive's SystemTextJsonConverter
    var json = JsonConvert.SerializeObject( test );

    json.Should()
        .Be( $$"""{"StringPrimitive":"{{value}}"}""" );
  }

  [Fact]
  public void SystemTextJson_Deserialization_WithInvalidType_ShouldThrow()
  {
    var json = """{"StringPrimitive":true}""";

    // The JSON deserializer should use StringPrimitive's SystemTextJsonConverter
    var act = () => SystemTextJsonSerializer.Deserialize<JsonTestClass>( json );

    act.Should()
       .Throw<SystemTextJsonException>()
       .WithMessage( s_jsonInvalidTokenTypeErrorMessage );
  }

  [Fact]
  public void SystemTextJson_Deserialization_WithLongValue_ShouldSucceed()
  {
    var value = 049541698L;
    var json = $$"""{"StringPrimitive":{{value}}}""";

    // The JSON deserializer should use StringPrimitive's SystemTextJsonConverter
    var result = SystemTextJsonSerializer.Deserialize<JsonTestClass>( json );

    result.Should()
          .NotBeNull();

    result!.StringPrimitive.Value.Should()
           .Be( value.ToString( "D9" ) );
  }

  [Fact]
  public void SystemTextJson_Deserialization_WithNull_ShouldThrow()
  {
    var json = """{"StringPrimitive":null}""";

    // The JSON deserializer should use StringPrimitive's SystemTextJsonConverter
    var act = () => SystemTextJsonSerializer.Deserialize<JsonTestClass>( json );

    act.Should()
       .Throw<SystemTextJsonException>()
       .WithMessage( StringPrimitive.ExpectedValidationErrorMessage );
  }

  [Fact]
  public void SystemTextJson_Deserialization_WithValidValue_ShouldSucceed()
  {
    var value = s_validValueA;
    var json = $$"""{"StringPrimitive":"{{value}}"}""";

    // The JSON deserializer should use StringPrimitive's SystemTextJsonConverter
    var result = SystemTextJsonSerializer.Deserialize<JsonTestClass>( json );

    result.Should()
          .NotBeNull();

    result!.StringPrimitive.Value.Should()
           .Be( value );
  }

  [Theory]
  [MemberData( nameof( InvalidValues ) )]
  public void SystemTextJson_DeserializationInvalidValue_ShouldThrow(
    string? value )
  {
    var json = $$"""{"StringPrimitive":"{{value}}"}""";

    // The JSON deserializer should use StringPrimitive's SystemTextJsonConverter
    var act = () => SystemTextJsonSerializer.Deserialize<JsonTestClass>( json );

    act.Should()
       .Throw<SystemTextJsonException>()
       .WithMessage( StringPrimitive.ExpectedValidationErrorMessage );
  }

  [Fact]
  public void SystemTextJson_Serialization_WithDefault_ShouldSucceed()
  {
    var test = new JsonTestClass { StringPrimitive = default };

    // The JSON serializer should use StringPrimitive's SystemTextJsonConverter
    var json = SystemTextJsonSerializer.Serialize( test );

    json.Should()
        .Be( """{"StringPrimitive":null}""" );
  }

  [Fact]
  public void SystemTextJson_Serialization_WithValidValue_ShouldSucceed()
  {
    var value = s_validValueA;
    var test = new JsonTestClass { StringPrimitive = ( StringPrimitive ) value };

    // The JSON serializer should use StringPrimitive's SystemTextJsonConverter
    var json = SystemTextJsonSerializer.Serialize( test );

    json.Should()
        .Be( $$"""{"StringPrimitive":"{{value}}"}""" );
  }

  [Fact]
  public void ToString_Default_ReturnsEmpty()
  {
    // Arrange
    var primitive = ( StringPrimitive ) default;

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
    var primitive = ( StringPrimitive ) value;

    // Act
    var result = primitive.ToString();

    // Assert
    result.Should()
          .Be( value );
  }

  [Fact]
  public void TypeConverter_CanConvertFrom_Long_ReturnsTrue()
  {
    var converter = TypeDescriptor.GetConverter( typeof( StringPrimitive ) );
    converter.CanConvertFrom( typeof( long ) )
             .Should()
             .BeTrue();
  }

  [Fact]
  public void TypeConverter_CanConvertFrom_SupportedType_ReturnsTrue()
  {
    var converter = TypeDescriptor.GetConverter( typeof( StringPrimitive ) );
    converter.CanConvertFrom( typeof( string ) )
             .Should()
             .BeTrue();
  }

  [Fact]
  public void TypeConverter_CanConvertTo_Long_ReturnsTrue()
  {
    var converter = TypeDescriptor.GetConverter( typeof( StringPrimitive ) );
    converter.CanConvertTo( typeof( long ) )
             .Should()
             .BeTrue();
  }

  [Fact]
  public void TypeConverter_CanConvertTo_SupportedType_ReturnsTrue()
  {
    var converter = TypeDescriptor.GetConverter( typeof( StringPrimitive ) );
    converter.CanConvertTo( typeof( string ) )
             .Should()
             .BeTrue();
  }

  [Fact]
  public void TypeConverter_CanConvertTo_UnsupportedType_ReturnsFalse()
  {
    var converter = TypeDescriptor.GetConverter( typeof( StringPrimitive ) );
    converter.CanConvertTo( typeof( DateTimeOffset ) )
             .Should()
             .BeFalse();
  }

  [Fact]
  public void TypeConverter_ConvertFrom_Long_Succeeds()
  {
    var value = 049541698L;
    var converter = TypeDescriptor.GetConverter( typeof( StringPrimitive ) );
    var result = converter.ConvertFrom( value );

    result.Should()
          .BeOfType<StringPrimitive>()
          .Which.Value.Should()
          .Be( value.ToString( "D9" ) );
  }

  [Fact]
  public void TypeConverter_ConvertFrom_NullValue_ReturnsDefault()
  {
    var converter = TypeDescriptor.GetConverter( typeof( StringPrimitive ) );
    var result = ( StringPrimitive ) converter.ConvertFrom( null! )!;

    result.Should()
          .BeOfType<StringPrimitive>()
          .Which.Should()
          .Be( default );
  }

  [Fact]
  public void TypeConverter_ConvertFrom_SupportedType_Succeeds()
  {
    var value = s_validValueA;
    var converter = TypeDescriptor.GetConverter( typeof( StringPrimitive ) );
    var result = converter.ConvertFrom( value );

    result.Should()
          .BeOfType<StringPrimitive>()
          .Which.Value.Should()
          .Be( value );
  }

  [Fact]
  public void TypeConverter_ConvertFrom_UnsupportedType_Throws()
  {
    var converter = TypeDescriptor.GetConverter( typeof( StringPrimitive ) );

    var value = DateTimeOffset.Now;
    var act = () => converter.ConvertFrom( value );

    act.Should()
       .Throw<NotSupportedException>();
  }

  [Fact]
  public void TypeConverter_ConvertTo_Long_Succeeds()
  {
    var converter = TypeDescriptor.GetConverter( typeof( StringPrimitive ) );

    var value = "049541698";
    var primitive = ( StringPrimitive ) value;
    var result = converter.ConvertTo( null, null, primitive, typeof( long ) );

    result.Should()
          .BeOfType<long>()
          .Which.Should()
          .Be( long.Parse( value ) );
  }

  [Fact]
  public void TypeConverter_ConvertTo_SupportedType_Succeeds()
  {
    var converter = TypeDescriptor.GetConverter( typeof( StringPrimitive ) );

    var value = s_validValueA;
    var primitive = ( StringPrimitive ) value;
    var result = converter.ConvertTo( null, null, primitive, typeof( string ) );

    result.Should()
          .BeOfType<string>()
          .Which.Should()
          .Be( value );
  }

  [Fact]
  public void TypeConverter_ConvertTo_UnsupportedType_Throws()
  {
    var converter = TypeDescriptor.GetConverter( typeof( StringPrimitive ) );

    var primitive = ( StringPrimitive ) s_validValueA;
    var act = () => converter.ConvertTo( null, null, primitive, typeof( DateTimeOffset ) );

    act.Should()
       .Throw<NotSupportedException>();
  }

  [Fact]
  public void TypeConverter_IsFound()
  {
    var converter = TypeDescriptor.GetConverter( typeof( StringPrimitive ) );
    converter.Should()
             .NotBeNull();
  }

  [Theory]
  [MemberData( nameof( InvalidValues ) )]
  public void Validate_InvalidValue_ReturnsFailure(
    string? value )
  {
    // Act
    var result = StringPrimitive.Validate( value );

    // Assert
    result.IsFailed.Should()
          .BeTrue();

    result.Errors.Select( error => error.Message )
          .Should()
          .ContainSingle()
          .Which
          .Should()
          .Be( StringPrimitive.ExpectedValidationErrorMessage );
  }

  [Fact]
  public void Validate_ValidValue_ReturnsSuccess()
  {
    // Act
    var result = StringPrimitive.Validate( s_validValueA );

    // Assert
    result.IsSuccess.Should()
          .BeTrue();
  }

  [Theory]
  [MemberData( nameof( InvalidValues ) )]
  public void Validate_WithUnvalidatedPrimitiveAndInvalidValue_ReturnsSuccess(
    string? value )
  {
    // Act
    var result = UnvalidatedStringPrimitive.Validate( value );

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
      StringPrimitive = ( StringPrimitive ) value
    };

    // Act
    context.Entities.Add( entity );
    context.SaveChanges();

    // Assert
    var result = context.Entities.Single( e => e.Id == id );

    result.StringPrimitive.Value.Should()
          .Be( value );
  }

  #endregion

  #region Implementation

  public static IEnumerable<object?[]> InvalidValues => new List<object?[]>
  {
    new object?[] { null },
    new object?[] { "" },
    new object?[] { "   " }
  };

  internal class JsonTestClass
  {
    #region Properties

    public StringPrimitive StringPrimitive { get; set; }

    #endregion
  }

  internal class TestEntity
  {
    #region Properties

    public Guid Id { get; set; } = Guid.NewGuid();
    public StringPrimitive StringPrimitive { get; set; }

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
                  .Property( e => e.StringPrimitive )
                  .IsUnicode( false )
                  .HasMaxLength( 9 )
                  .HasConversion( new StringPrimitiveValueConverter() )
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
