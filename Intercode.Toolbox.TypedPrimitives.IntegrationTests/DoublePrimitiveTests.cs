// Module Name: DoublePrimitiveTests.cs
// Author:      Eduardo Velasquez
// Copyright (c) 2024, Intercode Consulting, Inc.

namespace Intercode.Toolbox.TypedPrimitives.IntegrationTests;

using System.ComponentModel;
using FluentAssertions;
using FluentResults;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using StjJsonException = System.Text.Json.JsonException;
using StjJsonSerializer = System.Text.Json.JsonSerializer;
using NsjJsonException = Newtonsoft.Json.JsonException;

[TypedPrimitive<double>]
public readonly partial struct UnvalidatedDoublePrimitive;

[TypedPrimitive(
  typeof( double ),
  Converters = TypedPrimitiveConverter.TypeConverter |
               TypedPrimitiveConverter.SystemTextJson |
               TypedPrimitiveConverter.EfCoreValueConverter |
               TypedPrimitiveConverter.NewtonsoftJson
)]
public readonly partial struct DoublePrimitive
{
  #region Constants

  public const string ExpectedValidationErrorMessage = "Cannot be null or zero";

  #endregion

  #region Implementation

  static partial void ValidatePartial(
    double? value,
    ref Result result )
  {
    result = Result.FailIf( value is null or 0L, ExpectedValidationErrorMessage );
  }

  #endregion
}

public class DoublePrimitiveTests
{
  #region Constants

  private static readonly string s_jsonInvalidTokenTypeErrorMessage = "Value must be a Number";
  private static readonly double s_validValueA = 42.0;
  private static readonly double s_validValueB = 43.0;

  #endregion

  #region Tests

  [Fact]
  public void CompareTo_DefaultAndValue_ReturnsLessThanZero()
  {
    // Arrange
    var a = ( DoublePrimitive ) s_validValueA;
    var b = ( DoublePrimitive ) default;

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
    var a = ( DoublePrimitive ) valueA;
    var b = ( DoublePrimitive ) valueB;

    // Act
    var result = a.CompareTo( ( object ) b );

    // Assert
    result.Should()
          .Be( valueA.CompareTo( valueB ) );
  }

  [Fact]
  public void CompareTo_ObjectIsNull_ReturnsGreaterThanZero()
  {
    // Arrange
    var a = ( DoublePrimitive ) s_validValueA;

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
    var primitive = ( DoublePrimitive ) s_validValueA;

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
    var a = ( DoublePrimitive ) s_validValueA;
    var b = ( DoublePrimitive ) default;

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
    var a = ( DoublePrimitive ) s_validValueA;
    var b = ( DoublePrimitive ) s_validValueB;

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
    var a = ( DoublePrimitive ) s_validValueA;
    var b = ( DoublePrimitive ) s_validValueA;

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
    var primitive = ( DoublePrimitive ) s_validValueA;

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
    var primitive = ( DoublePrimitive ) default;

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
    var primitive = ( DoublePrimitive ) value;

    // Act
    var result = primitive.Value;

    // Assert
    result.Should()
          .Be( value );
  }

  [Theory]
  [MemberData( nameof( InvalidValues ) )]
  public void ExplicitOperator_StringToPrimitive_WithInvalidValue_Throws(
    double? value )
  {
    // Act
    var act = () => ( DoublePrimitive ) value;

    // Assert
    act.Should()
       .Throw<InvalidOperationException>()
       .WithMessage( DoublePrimitive.ExpectedValidationErrorMessage );
  }

  [Fact]
  public void ExplicitOperator_ValueToPrimitive_ReturnsPrimitiveWithValue()
  {
    // Act
    var value = s_validValueA;
    var primitive = ( DoublePrimitive ) value;

    // Assert
    primitive.Value.Should()
             .Be( value );
  }

  [Fact]
  public void FromInt32_WithValidValue_ReturnsSuccess()
  {
    // Act
    var value = s_validValueA;
    var result = DoublePrimitive.Create( value );

    // Assert
    result.IsSuccess.Should()
          .BeTrue();

    result.Value.Value.Should()
          .Be( value );
  }

  [Theory]
  [MemberData( nameof( InvalidValues ) )]
  public void FromString_InvalidValue_ReturnsFailure(
    double? value )
  {
    // Act
    var result = DoublePrimitive.Create( value );

    // Assert
    result.IsFailed.Should()
          .BeTrue();

    result.Errors.Select( error => error.Message )
          .Should()
          .ContainSingle()
          .Which
          .Should()
          .Be( DoublePrimitive.ExpectedValidationErrorMessage );
  }

  [Fact]
  public void GetHashCode_Default_ReturnsZero()
  {
    // Act
    var hashCodeA = ( ( DoublePrimitive ) default ).GetHashCode();

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
    var a = ( DoublePrimitive ) valueA;
    var b = ( DoublePrimitive ) valueB;

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
    var a = ( DoublePrimitive ) value;
    var b = ( DoublePrimitive ) value;

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
    var primitive = ( DoublePrimitive ) default;

    // Act
    var result = primitive.IsDefault;

    result.Should()
          .BeTrue();
  }

  [Fact]
  public void IsDefault_WithValue_ReturnsFalse()
  {
    // Arrange
    var primitive = ( DoublePrimitive ) s_validValueA;

    // Act
    var result = primitive.IsDefault;

    result.Should()
          .BeFalse();
  }

  [Theory]
  [MemberData( nameof( InvalidValues ) )]
  public void IsValid_InvalidValue_ReturnsFalse(
    double? value )
  {
    // Act
    var isValid = DoublePrimitive.IsValid( value );

    // Assert
    isValid.Should()
           .BeFalse();
  }

  [Fact]
  public void IsValid_StringValidValue_ReturnsTrue()
  {
    // Act
    var isValid = DoublePrimitive.IsValid( s_validValueA );

    // Assert
    isValid.Should()
           .BeTrue();
  }

  [Theory]
  [MemberData( nameof( InvalidValues ) )]
  public void NewtonsoftJson_Deserialization_InvalidValue_ShouldThrow(
    double? value )
  {
    var asString = value is null ? "null" : value.Value.ToString("f");
    var json = $$"""{"Primitive":{{asString}}}""";

    // The JSON deserializer should use Primitive's SystemTextJsonConverter
    var act = () => JsonConvert.DeserializeObject<JsonTestClass>( json );

    act.Should()
       .Throw<NsjJsonException>()
       .WithMessage( DoublePrimitive.ExpectedValidationErrorMessage );
  }

  [Fact]
  public void NewtonsoftJson_Deserialization_WithNull_ShouldThrow()
  {
    var json = """{"Primitive":null}""";

    // The JSON deserializer should use Primitive's SystemTextJsonConverter
    var act = () => JsonConvert.DeserializeObject<JsonTestClass>( json );

    act.Should()
       .Throw<NsjJsonException>()
       .WithMessage( DoublePrimitive.ExpectedValidationErrorMessage );
  }

  [Fact]
  public void NewtonsoftJson_Deserialization_WithValidValue_ShouldSucceed()
  {
    var value = s_validValueA;
    var json = $$"""{"Primitive":{{value:f}}}""";

    // The JSON deserializer should use Primitive's SystemTextJsonConverter
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

    // The JSON serializer should use Primitive's SystemTextJsonConverter
    var json = JsonConvert.SerializeObject( test );

    json.Should()
        .Be( """{"Primitive":null}""" );
  }

  [Fact]
  public void NewtonsoftJson_Serialization_WithValidValue_ShouldSucceed()
  {
    var value = s_validValueA;
    var test = new JsonTestClass { Primitive = ( DoublePrimitive ) value };

    // The JSON serializer should use Primitive's SystemTextJsonConverter
    var json = JsonConvert.SerializeObject( test );

    json.Should()
        .Be( $$"""{"Primitive":{{value:f1}}}""" );
  }

  [Fact]
  public void SystemTextJson_Deserialization_WithInvalidType_ShouldThrow()
  {
    var json = """{"Primitive":"12345"}""";

    // The JSON deserializer should use LongPrimitive's SystemTextJsonConverter
    var act = () => StjJsonSerializer.Deserialize<JsonTestClass>( json );

    act.Should()
       .Throw<StjJsonException>()
       .WithMessage( s_jsonInvalidTokenTypeErrorMessage );
  }

  [Fact]
  public void SystemTextJson_Deserialization_WithNull_ShouldThrow()
  {
    var json = """{"Primitive":null}""";

    // The JSON deserializer should use LongPrimitive's SystemTextJsonConverter
    var act = () => StjJsonSerializer.Deserialize<JsonTestClass>( json );

    act.Should()
       .Throw<StjJsonException>()
       .WithMessage( DoublePrimitive.ExpectedValidationErrorMessage );
  }

  [Fact]
  public void SystemTextJson_Deserialization_WithValidValue_ShouldSucceed()
  {
    var value = s_validValueA;
    var json = $$"""{"Primitive":{{value}}}""";

    // The JSON deserializer should use LongPrimitive's SystemTextJsonConverter
    var result = StjJsonSerializer.Deserialize<JsonTestClass>( json );

    result.Should()
          .NotBeNull();

    result!.Primitive.Value.Should()
           .Be( value );
  }

  [Theory]
  [MemberData( nameof( InvalidValues ) )]
  public void SystemTextJson_DeserializationInvalidValue_ShouldThrow(
    double? value )
  {
    var asString = value is null ? "null" : value.ToString();
    var json = $$"""{"Primitive":{{asString}}}""";

    // The JSON deserializer should use LongPrimitive's SystemTextJsonConverter
    var act = () => StjJsonSerializer.Deserialize<JsonTestClass>( json );

    act.Should()
       .Throw<StjJsonException>()
       .WithMessage( DoublePrimitive.ExpectedValidationErrorMessage );
  }

  [Fact]
  public void SystemTextJson_Serialization_WithDefault_ShouldSucceed()
  {
    var test = new JsonTestClass { Primitive = default };

    // The JSON serializer should use LongPrimitive's SystemTextJsonConverter
    var json = StjJsonSerializer.Serialize( test );

    json.Should()
        .Be( """{"Primitive":null}""" );
  }

  [Fact]
  public void SystemTextJson_Serialization_WithValidValue_ShouldSucceed()
  {
    var value = s_validValueA;
    var test = new JsonTestClass { Primitive = ( DoublePrimitive ) value };

    // The JSON serializer should use LongPrimitive's SystemTextJsonConverter
    var json = StjJsonSerializer.Serialize( test );

    json.Should()
        .Be( $$"""{"Primitive":{{value}}}""" );
  }

  [Fact]
  public void ToString_Default_ReturnsEmpty()
  {
    // Arrange
    var primitive = ( DoublePrimitive ) default;

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
    var primitive = ( DoublePrimitive ) value;

    // Act
    var result = primitive.ToString();

    // Assert
    result.Should()
          .Be( value.ToString() );
  }

  [Fact]
  public void TypeConverter_CanConvertFrom_SupportedType_ReturnsTrue()
  {
    var converter = TypeDescriptor.GetConverter( typeof( DoublePrimitive ) );
    converter.CanConvertFrom( typeof( double ) )
             .Should()
             .BeTrue();
  }

  [Fact]
  public void TypeConverter_CanConvertTo_SupportedType_ReturnsTrue()
  {
    var converter = TypeDescriptor.GetConverter( typeof( DoublePrimitive ) );
    converter.CanConvertTo( typeof( double ) )
             .Should()
             .BeTrue();
  }

  [Fact]
  public void TypeConverter_CanConvertTo_UnsupportedType_ReturnsFalse()
  {
    var converter = TypeDescriptor.GetConverter( typeof( DoublePrimitive ) );
    converter.CanConvertTo( typeof( string ) )
             .Should()
             .BeFalse();
  }

  [Fact]
  public void TypeConverter_ConvertFrom_NullValue_ReturnsDefault()
  {
    var converter = TypeDescriptor.GetConverter( typeof( DoublePrimitive ) );
    var result = ( DoublePrimitive ) converter.ConvertFrom( null! )!;

    result.Should()
          .BeOfType<DoublePrimitive>()
          .Which.Should()
          .Be( default );
  }

  [Fact]
  public void TypeConverter_ConvertFrom_SupportedType_Succeeds()
  {
    var value = s_validValueA;
    var converter = TypeDescriptor.GetConverter( typeof( DoublePrimitive ) );
    var result = converter.ConvertFrom( value );

    result.Should()
          .BeOfType<DoublePrimitive>()
          .Which.Value.Should()
          .Be( value );
  }

  [Fact]
  public void TypeConverter_ConvertFrom_UnsupportedType_Throws()
  {
    var converter = TypeDescriptor.GetConverter( typeof( DoublePrimitive ) );

    var value = "12345";
    var act = () => converter.ConvertFrom( value );

    act.Should()
       .Throw<NotSupportedException>();
  }

  [Fact]
  public void TypeConverter_ConvertTo_SupportedType_Succeeds()
  {
    var converter = TypeDescriptor.GetConverter( typeof( DoublePrimitive ) );

    var value = s_validValueA;
    var primitive = ( DoublePrimitive ) value;
    var result = converter.ConvertTo( null, null, primitive, typeof( double ) );

    result.Should()
          .BeOfType<double>()
          .Which.Should()
          .Be( value );
  }

  [Fact]
  public void TypeConverter_ConvertTo_UnsupportedType_Throws()
  {
    var converter = TypeDescriptor.GetConverter( typeof( DoublePrimitive ) );

    var primitive = ( DoublePrimitive ) s_validValueA;
    var act = () => converter.ConvertTo( null, null, primitive, typeof( string ) );

    act.Should()
       .Throw<NotSupportedException>();
  }

  [Fact]
  public void TypeConverter_IsFound()
  {
    var converter = TypeDescriptor.GetConverter( typeof( DoublePrimitive ) );
    converter.Should()
             .NotBeNull();
  }

  [Theory]
  [MemberData( nameof( InvalidValues ) )]
  public void Validate_InvalidValue_ReturnsFailure(
    double? value )
  {
    // Act
    var result = DoublePrimitive.Validate( value );

    // Assert
    result.IsFailed.Should()
          .BeTrue();

    result.Errors.Select( error => error.Message )
          .Should()
          .ContainSingle()
          .Which
          .Should()
          .Be( DoublePrimitive.ExpectedValidationErrorMessage );
  }

  [Fact]
  public void Validate_ValidValue_ReturnsSuccess()
  {
    // Act
    var result = DoublePrimitive.Validate( s_validValueA );

    // Assert
    result.IsSuccess.Should()
          .BeTrue();
  }

  [Theory]
  [MemberData( nameof( InvalidValues ) )]
  public void Validate_WithUnvalidatedPrimitiveAndInvalidValue_ReturnsSuccess(
    double? value )
  {
    // Act
    var result = UnvalidatedDoublePrimitive.Validate( value );

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
      Primitive = ( DoublePrimitive ) value
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

  public static IEnumerable<object?[]> InvalidValues => new List<object?[]>
  {
    new object?[] { null },
    new object?[] { 0.0 }
  };

  internal class JsonTestClass
  {
    #region Properties

    public DoublePrimitive Primitive { get; set; }

    #endregion
  }

  internal class TestEntity
  {
    #region Properties

    public Guid Id { get; set; } = Guid.NewGuid();
    public DoublePrimitive Primitive { get; set; }

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
                  .HasConversion( new DoublePrimitiveValueConverter() )
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
