namespace Intercode.Toolbox.TypedPrimitives.IntegrationTests;

using System.ComponentModel;
using System.Globalization;
using System.Text.Json;
using FluentAssertions;
using FluentResults;
using Intercode.Toolbox.TypedPrimitives;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

[TypedPrimitive(
  typeof( string ),
  ValidatorType = typeof( StringPrimitiveTests.Validator ),
  ValidatorFlagsType = typeof( StringPrimitiveTests.ValidatorFlags ),
  ValidatorFlagsDefaultValue = StringPrimitiveTests.ValidatorFlags.Simple
)]
public readonly partial record struct StringPrimitive;

public readonly partial record struct StringPrimitive
{
  #region Nested Types

  public partial class SystemTextJsonConverter
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

  public partial class TypeConverter
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
      if( value is string s && destinationType == typeof( long ) )
      {
        convertedValue = long.Parse( s );
        converted = true;
      }
    }

    #endregion
  }

  #endregion
}

public class StringPrimitiveTests
{
  #region Constants

  private static readonly string s_expectedValidationErrorMessage = "Cannot be null or empty";
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
  public void ExplicitOperator_SpanToPrimitive_ReturnsValue()
  {
    // Act
    var value = s_validValueA;
    var primitive = ( StringPrimitive ) value.AsSpan();

    // Assert
    primitive.Value.Should()
             .Be( value );
  }

  [Theory]
  [MemberData( nameof( InvalidValues ) )]
  public void ExplicitOperator_SpanToPrimitive_WithInvalidValue_Throws(
    string? value )
  {
    // Act
    var act = () => ( StringPrimitive ) value.AsSpan();

    // Assert
    act.Should()
       .Throw<InvalidOperationException>()
       .WithMessage( s_expectedValidationErrorMessage );
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
       .WithMessage( s_expectedValidationErrorMessage );
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
          .Be( s_expectedValidationErrorMessage );
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
          .Be( s_expectedValidationErrorMessage );
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
  public void IsValid_SpanInvalidValue_ReturnsFalse(
    string? value )
  {
    // Act
    var isValid = StringPrimitive.IsValid( value.AsSpan() );

    // Assert
    isValid.Should()
           .BeFalse();
  }

  [Fact]
  public void IsValid_SpanValidValue_ReturnsTrue()
  {
    // Act
    var isValid = StringPrimitive.IsValid( s_validValueA.AsSpan() );

    // Assert
    isValid.Should()
           .BeTrue();
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
  public void SystemTextJson_Deserialization_WithInvalidType_ShouldThrow()
  {
    var json = """{"StringPrimitive":true}""";

    // The JSON deserializer should use StringPrimitive's SystemTextJsonConverter
    var act = () => JsonSerializer.Deserialize<JsonTestClass>( json );

    act.Should()
       .Throw<JsonException>()
       .WithMessage( s_jsonInvalidTokenTypeErrorMessage );
  }

  [Fact]
  public void SystemTextJson_Deserialization_WithLongValue_ShouldSucceed()
  {
    var value = 049541698L;
    var json = $$"""{"StringPrimitive":{{value}}}""";

    // The JSON deserializer should use StringPrimitive's SystemTextJsonConverter
    var result = JsonSerializer.Deserialize<JsonTestClass>( json );

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
    var act = () => JsonSerializer.Deserialize<JsonTestClass>( json );

    act.Should()
       .Throw<JsonException>()
       .WithMessage( s_expectedValidationErrorMessage );
  }

  [Fact]
  public void SystemTextJson_Deserialization_WithValidValue_ShouldSucceed()
  {
    var value = s_validValueA;
    var json = $$"""{"StringPrimitive":"{{value}}"}""";

    // The JSON deserializer should use StringPrimitive's SystemTextJsonConverter
    var result = JsonSerializer.Deserialize<JsonTestClass>( json );

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
    var act = () => JsonSerializer.Deserialize<JsonTestClass>( json );

    act.Should()
       .Throw<JsonException>()
       .WithMessage( s_expectedValidationErrorMessage );
  }

  [Fact]
  public void SystemTextJson_Serialization_WithDefault_ShouldSucceed()
  {
    var test = new JsonTestClass { StringPrimitive = default };

    // The JSON serializer should use StringPrimitive's SystemTextJsonConverter
    var json = JsonSerializer.Serialize( test );

    json.Should()
        .Be( """{"StringPrimitive":null}""" );
  }

  [Fact]
  public void SystemTextJson_Serialization_WithValidValue_ShouldSucceed()
  {
    var value = s_validValueA;
    var test = new JsonTestClass { StringPrimitive = ( StringPrimitive ) value };

    // The JSON serializer should use StringPrimitive's SystemTextJsonConverter
    var json = JsonSerializer.Serialize( test );

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
          .Be( s_expectedValidationErrorMessage );
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

  public enum ValidatorFlags
  {
    None = 0,
    Simple = 1
  }

  public static class Validator
  {
    #region Public Methods

    public static Result Validate(
      string? value,
      ValidatorFlags flags = default )
    {
      return flags switch
      {
        ValidatorFlags.None   => Result.Ok(),
        ValidatorFlags.Simple => Result.FailIf( string.IsNullOrWhiteSpace( value ), "Cannot be null or empty" ),
        _                     => Result.Fail( "Invalid validation flag" )
      };
    }

    public static Result Validate(
      ReadOnlySpan<char> span,
      ValidatorFlags flags = default )
    {
      return flags switch
      {
        ValidatorFlags.None => Result.Ok(),
        ValidatorFlags.Simple => Result.FailIf(
          span.Trim()
              .IsEmpty,
          "Cannot be null or empty"
        ),
        _ => Result.Fail( "Invalid validation flag" )
      };
    }

    #endregion
  }

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
                  .HasConversion( new StringPrimitive.ValueConverter() )
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
