// Module Name: DateTimePrimitiveTests.cs
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

[TypedPrimitive<DateTime>]
public readonly partial struct UnvalidatedDateTimePrimitive;

[TypedPrimitive(
  typeof( DateTime ),
  Converters = TypedPrimitiveConverter.TypeConverter |
               TypedPrimitiveConverter.SystemTextJson |
               TypedPrimitiveConverter.EfCoreValueConverter |
               TypedPrimitiveConverter.NewtonsoftJson
)]
public readonly partial struct DateTimePrimitive
{
  #region Constants

  public const string ExpectedValidationErrorMessage = "Cannot be null or empty";

  #endregion

  #region Implementation

  static partial void ValidatePartial(
    DateTime? value,
    ref Result result )
  {
    result = Result.FailIf(
      value is null || value.Value == DateTime.MinValue,
      ExpectedValidationErrorMessage
    );
  }

  #endregion
}

public class DateTimePrimitiveTests
{
  #region Constants

  private static readonly string s_jsonInvalidTokenTypeErrorMessage = "Value must be a String";
  private static readonly DateTime s_validValueA = DateTime.ParseExact( "1995-12-01T15:00:00", "s", null );
  private static readonly DateTime s_validValueB = DateTime.ParseExact( "2018-02-06T12:45:00", "s", null );

  #endregion

  #region Tests

  [Fact]
  public void CompareTo_DefaultAndValue_ReturnsLessThanZero()
  {
    // Arrange
    var a = ( DateTimePrimitive ) s_validValueA;
    var b = ( DateTimePrimitive ) default;

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
    var a = ( DateTimePrimitive ) valueA;
    var b = ( DateTimePrimitive ) valueB;

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
    var a = ( DateTimePrimitive ) s_validValueA;

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
    var primitive = ( DateTimePrimitive ) s_validValueA;

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
    var a = ( DateTimePrimitive ) s_validValueA;
    var b = ( DateTimePrimitive ) default;

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
    var a = ( DateTimePrimitive ) s_validValueA;
    var b = ( DateTimePrimitive ) s_validValueB;

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
    var a = ( DateTimePrimitive ) s_validValueA;
    var b = ( DateTimePrimitive ) s_validValueA;

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
    var primitive = ( DateTimePrimitive ) s_validValueA;

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
    var primitive = ( DateTimePrimitive ) default;

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
    var primitive = ( DateTimePrimitive ) value;

    // Act
    var result = primitive.Value;

    // Assert
    result.Should()
          .Be( value );
  }

  [Theory]
  [MemberData( nameof( InvalidValues ) )]
  public void ExplicitOperator_StringToPrimitive_WithInvalidValue_Throws(
    DateTime? value )
  {
    // Act
    var act = () => ( DateTimePrimitive ) value;

    // Assert
    act.Should()
       .Throw<InvalidOperationException>()
       .WithMessage( DateTimePrimitive.ExpectedValidationErrorMessage );
  }

  [Fact]
  public void ExplicitOperator_ValueToPrimitive_ReturnsPrimitiveWithValue()
  {
    // Act
    var value = s_validValueA;
    var primitive = ( DateTimePrimitive ) value;

    // Assert
    primitive.Value.Should()
             .Be( value );
  }

  [Fact]
  public void FromInt32_WithValidValue_ReturnsSuccess()
  {
    // Act
    var value = s_validValueA;
    var result = DateTimePrimitive.Create( value );

    // Assert
    result.IsSuccess.Should()
          .BeTrue();

    result.Value.Value.Should()
          .Be( value );
  }

  [Theory]
  [MemberData( nameof( InvalidValues ) )]
  public void FromString_InvalidValue_ReturnsFailure(
    DateTime? value )
  {
    // Act
    var result = DateTimePrimitive.Create( value );

    // Assert
    result.IsFailed.Should()
          .BeTrue();

    result.Errors.Select( error => error.Message )
          .Should()
          .ContainSingle()
          .Which
          .Should()
          .Be( DateTimePrimitive.ExpectedValidationErrorMessage );
  }

  [Fact]
  public void GetHashCode_Default_ReturnsZero()
  {
    // Act
    var hashCodeA = ( ( DateTimePrimitive ) default ).GetHashCode();

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
    var a = ( DateTimePrimitive ) valueA;
    var b = ( DateTimePrimitive ) valueB;

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
    var a = ( DateTimePrimitive ) value;
    var b = ( DateTimePrimitive ) value;

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
    var primitive = ( DateTimePrimitive ) default;

    // Act
    var result = primitive.IsDefault;

    result.Should()
          .BeTrue();
  }

  [Fact]
  public void IsDefault_WithValue_ReturnsFalse()
  {
    // Arrange
    var primitive = ( DateTimePrimitive ) s_validValueA;

    // Act
    var result = primitive.IsDefault;

    result.Should()
          .BeFalse();
  }

  [Theory]
  [MemberData( nameof( InvalidValues ) )]
  public void IsValid_InvalidValue_ReturnsFalse(
    DateTime? value )
  {
    // Act
    var isValid = DateTimePrimitive.IsValid( value );

    // Assert
    isValid.Should()
           .BeFalse();
  }

  [Fact]
  public void IsValid_StringValidValue_ReturnsTrue()
  {
    // Act
    var isValid = DateTimePrimitive.IsValid( s_validValueA );

    // Assert
    isValid.Should()
           .BeTrue();
  }

  [Theory]
  [MemberData( nameof( InvalidValues ) )]
  public void NewtonsoftJson_Deserialization_InvalidValue_ShouldThrow(
    DateTime? value )
  {
    var asString = value is null ? "null" : $"\"{value:O}\"";
    var json = $$"""{"Primitive":{{asString}}}""";

    var act = () => JsonConvert.DeserializeObject<JsonTestClass>( json );

    act.Should()
       .Throw<NsjJsonException>()
       .WithMessage( DateTimePrimitive.ExpectedValidationErrorMessage );
  }

  [Fact]
  public void NewtonsoftJson_Deserialization_WithNull_ShouldThrow()
  {
    var json = """{"Primitive":null}""";

    var act = () => JsonConvert.DeserializeObject<JsonTestClass>( json );

    act.Should()
       .Throw<NsjJsonException>()
       .WithMessage( DateTimePrimitive.ExpectedValidationErrorMessage );
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
    var test = new JsonTestClass { Primitive = ( DateTimePrimitive ) value };

    var json = JsonConvert.SerializeObject( test );

    json.Should()
        .Be( $$"""{"Primitive":"{{value:O}}"}""" );
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
       .WithMessage( DateTimePrimitive.ExpectedValidationErrorMessage );
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
    DateTime? value )
  {
    var asString = value is null ? "null" : $"\"{value}\"";
    var json = $$"""{"Primitive":{{asString}}}""";

    var act = () => StjJsonSerializer.Deserialize<JsonTestClass>( json );

    act.Should()
       .Throw<StjJsonException>()
       .WithMessage( DateTimePrimitive.ExpectedValidationErrorMessage );
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
    var expected = new JsonTestClass { Primitive = ( DateTimePrimitive ) value };

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
    var primitive = ( DateTimePrimitive ) default;

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
    var primitive = ( DateTimePrimitive ) value;

    // Act
    var result = primitive.ToString();

    // Assert
    result.Should()
          .Be( value.ToString() );
  }

  [Fact]
  public void TypeConverter_CanConvertFrom_SupportedType_ReturnsTrue()
  {
    var converter = TypeDescriptor.GetConverter( typeof( DateTimePrimitive ) );
    converter.CanConvertFrom( typeof( DateTime ) )
             .Should()
             .BeTrue();
  }

  [Fact]
  public void TypeConverter_CanConvertTo_SupportedType_ReturnsTrue()
  {
    var converter = TypeDescriptor.GetConverter( typeof( DateTimePrimitive ) );
    converter.CanConvertTo( typeof( DateTime ) )
             .Should()
             .BeTrue();
  }

  [Fact]
  public void TypeConverter_CanConvertTo_UnsupportedType_ReturnsFalse()
  {
    var converter = TypeDescriptor.GetConverter( typeof( DateTimePrimitive ) );
    converter.CanConvertTo( typeof( string ) )
             .Should()
             .BeFalse();
  }

  [Fact]
  public void TypeConverter_ConvertFrom_NullValue_ReturnsDefault()
  {
    var converter = TypeDescriptor.GetConverter( typeof( DateTimePrimitive ) );
    var result = ( DateTimePrimitive ) converter.ConvertFrom( null! )!;

    result.Should()
          .BeOfType<DateTimePrimitive>()
          .Which.Should()
          .Be( default );
  }

  [Fact]
  public void TypeConverter_ConvertFrom_SupportedType_Succeeds()
  {
    var value = s_validValueA;
    var converter = TypeDescriptor.GetConverter( typeof( DateTimePrimitive ) );
    var result = converter.ConvertFrom( value );

    result.Should()
          .BeOfType<DateTimePrimitive>()
          .Which.Value.Should()
          .Be( value );
  }

  [Fact]
  public void TypeConverter_ConvertFrom_UnsupportedType_Throws()
  {
    var converter = TypeDescriptor.GetConverter( typeof( DateTimePrimitive ) );

    var value = "12345";
    var act = () => converter.ConvertFrom( value );

    act.Should()
       .Throw<NotSupportedException>();
  }

  [Fact]
  public void TypeConverter_ConvertTo_SupportedType_Succeeds()
  {
    var converter = TypeDescriptor.GetConverter( typeof( DateTimePrimitive ) );

    var value = s_validValueA;
    var primitive = ( DateTimePrimitive ) value;
    var result = converter.ConvertTo( null, null, primitive, typeof( DateTime ) );

    result.Should()
          .BeOfType<DateTime>()
          .Which.Should()
          .Be( value );
  }

  [Fact]
  public void TypeConverter_ConvertTo_UnsupportedType_Throws()
  {
    var converter = TypeDescriptor.GetConverter( typeof( DateTimePrimitive ) );

    var primitive = ( DateTimePrimitive ) s_validValueA;
    var act = () => converter.ConvertTo( null, null, primitive, typeof( string ) );

    act.Should()
       .Throw<NotSupportedException>();
  }

  [Fact]
  public void TypeConverter_IsFound()
  {
    var converter = TypeDescriptor.GetConverter( typeof( DateTimePrimitive ) );
    converter.Should()
             .NotBeNull();
  }

  [Theory]
  [MemberData( nameof( InvalidValues ) )]
  public void Validate_InvalidValue_ReturnsFailure(
    DateTime? value )
  {
    // Act
    var result = DateTimePrimitive.Validate( value );

    // Assert
    result.IsFailed.Should()
          .BeTrue();

    result.Errors.Select( error => error.Message )
          .Should()
          .ContainSingle()
          .Which
          .Should()
          .Be( DateTimePrimitive.ExpectedValidationErrorMessage );
  }

  [Fact]
  public void Validate_ValidValue_ReturnsSuccess()
  {
    // Act
    var result = DateTimePrimitive.Validate( s_validValueA );

    // Assert
    result.IsSuccess.Should()
          .BeTrue();
  }

  [Theory]
  [MemberData( nameof( InvalidValues ) )]
  public void Validate_WithUnvalidatedPrimitiveAndInvalidValue_ReturnsSuccess(
    DateTime? value )
  {
    // Act
    var result = UnvalidatedDateTimePrimitive.Validate( value );

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
      Primitive = ( DateTimePrimitive ) value
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

  public static class Validator
  {
    #region Public Methods

    public static Result Validate(
      DateTime? value )
    {
      return Result.FailIf(
        value is null || value.Value == DateTime.MinValue,
        DateTimePrimitive.ExpectedValidationErrorMessage
      );
    }

    #endregion
  }

  public static IEnumerable<object?[]> InvalidValues => new List<object?[]>
  {
    new object?[] { null },
    new object?[] { DateTime.MinValue }
  };

  internal class JsonTestClass
  {
    #region Properties

    public DateTimePrimitive Primitive { get; set; }

    #endregion
  }

  internal class TestEntity
  {
    #region Properties

    public Guid Id { get; set; } = Guid.NewGuid();
    public DateTimePrimitive Primitive { get; set; }

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
                  .HasConversion( new DateTimePrimitiveValueConverter() )
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
