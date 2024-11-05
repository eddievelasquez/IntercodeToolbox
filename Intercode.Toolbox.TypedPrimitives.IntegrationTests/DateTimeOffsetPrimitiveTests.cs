// Module Name: DateTimeOffsetPrimitiveTests.cs
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

[TypedPrimitive<DateTimeOffset>]
public readonly partial struct UnvalidatedDateTimeOffsetPrimitive;

[TypedPrimitive(
  typeof( DateTimeOffset ),
  Converters = TypedPrimitiveConverter.TypeConverter |
               TypedPrimitiveConverter.SystemTextJson |
               TypedPrimitiveConverter.EfCoreValueConverter |
               TypedPrimitiveConverter.NewtonsoftJson
)]
public readonly partial struct DateTimeOffsetPrimitive
{
  #region Constants

  public const string ExpectedValidationErrorMessage = "Cannot be null or empty";

  #endregion

  #region Implementation

  static partial void ValidatePartial(
    DateTimeOffset? value,
    ref Result result )
  {
    result = Result.FailIf(
      value is null || value.Value == DateTimeOffset.MinValue,
      ExpectedValidationErrorMessage
    );
  }

  #endregion
}

public class DateTimeOffsetPrimitiveTests
{
  #region Constants

  private static readonly string s_jsonInvalidTokenTypeErrorMessage = "Value must be a String";
  private static readonly DateTimeOffset s_validValueA = DateTimeOffset.ParseExact( "1995-12-01T15:00:00", "s", null );
  private static readonly DateTimeOffset s_validValueB = DateTimeOffset.ParseExact( "2018-02-06T12:45:00", "s", null );

  #endregion

  #region Tests

  [Fact]
  public void CompareTo_DefaultAndValue_ReturnsLessThanZero()
  {
    // Arrange
    var a = ( DateTimeOffsetPrimitive ) s_validValueA;
    var b = ( DateTimeOffsetPrimitive ) default;

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
    var a = ( DateTimeOffsetPrimitive ) valueA;
    var b = ( DateTimeOffsetPrimitive ) valueB;

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
    var a = ( DateTimeOffsetPrimitive ) s_validValueA;

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
    var primitive = ( DateTimeOffsetPrimitive ) s_validValueA;

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
    var a = ( DateTimeOffsetPrimitive ) s_validValueA;
    var b = ( DateTimeOffsetPrimitive ) default;

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
    var a = ( DateTimeOffsetPrimitive ) s_validValueA;
    var b = ( DateTimeOffsetPrimitive ) s_validValueB;

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
    var a = ( DateTimeOffsetPrimitive ) s_validValueA;
    var b = ( DateTimeOffsetPrimitive ) s_validValueA;

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
    var primitive = ( DateTimeOffsetPrimitive ) s_validValueA;

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
    var primitive = ( DateTimeOffsetPrimitive ) default;

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
    var primitive = ( DateTimeOffsetPrimitive ) value;

    // Act
    var result = primitive.Value;

    // Assert
    result.Should()
          .Be( value );
  }

  [Theory]
  [MemberData( nameof( InvalidValues ) )]
  public void ExplicitOperator_StringToPrimitive_WithInvalidValue_Throws(
    DateTimeOffset? value )
  {
    // Act
    var act = () => ( DateTimeOffsetPrimitive ) value;

    // Assert
    act.Should()
       .Throw<InvalidOperationException>()
       .WithMessage( DateTimeOffsetPrimitive.ExpectedValidationErrorMessage );
  }

  [Fact]
  public void ExplicitOperator_ValueToPrimitive_ReturnsPrimitiveWithValue()
  {
    // Act
    var value = s_validValueA;
    var primitive = ( DateTimeOffsetPrimitive ) value;

    // Assert
    primitive.Value.Should()
             .Be( value );
  }

  [Fact]
  public void FromInt32_WithValidValue_ReturnsSuccess()
  {
    // Act
    var value = s_validValueA;
    var result = DateTimeOffsetPrimitive.Create( value );

    // Assert
    result.IsSuccess.Should()
          .BeTrue();

    result.Value.Value.Should()
          .Be( value );
  }

  [Theory]
  [MemberData( nameof( InvalidValues ) )]
  public void FromString_InvalidValue_ReturnsFailure(
    DateTimeOffset? value )
  {
    // Act
    var result = DateTimeOffsetPrimitive.Create( value );

    // Assert
    result.IsFailed.Should()
          .BeTrue();

    result.Errors.Select( error => error.Message )
          .Should()
          .ContainSingle()
          .Which
          .Should()
          .Be( DateTimeOffsetPrimitive.ExpectedValidationErrorMessage );
  }

  [Fact]
  public void GetHashCode_Default_ReturnsZero()
  {
    // Act
    var hashCodeA = ( ( DateTimeOffsetPrimitive ) default ).GetHashCode();

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
    var a = ( DateTimeOffsetPrimitive ) valueA;
    var b = ( DateTimeOffsetPrimitive ) valueB;

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
    var a = ( DateTimeOffsetPrimitive ) value;
    var b = ( DateTimeOffsetPrimitive ) value;

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
    var primitive = ( DateTimeOffsetPrimitive ) default;

    // Act
    var result = primitive.IsDefault;

    result.Should()
          .BeTrue();
  }

  [Fact]
  public void IsDefault_WithValue_ReturnsFalse()
  {
    // Arrange
    var primitive = ( DateTimeOffsetPrimitive ) s_validValueA;

    // Act
    var result = primitive.IsDefault;

    result.Should()
          .BeFalse();
  }

  [Theory]
  [MemberData( nameof( InvalidValues ) )]
  public void IsValid_InvalidValue_ReturnsFalse(
    DateTimeOffset? value )
  {
    // Act
    var isValid = DateTimeOffsetPrimitive.IsValid( value );

    // Assert
    isValid.Should()
           .BeFalse();
  }

  [Fact]
  public void IsValid_StringValidValue_ReturnsTrue()
  {
    // Act
    var isValid = DateTimeOffsetPrimitive.IsValid( s_validValueA );

    // Assert
    isValid.Should()
           .BeTrue();
  }

  [Theory]
  [MemberData( nameof( InvalidValues ) )]
  public void NewtonsoftJson_Deserialization_InvalidValue_ShouldThrow(
    DateTimeOffset? value )
  {
    var asString = value is null ? "null" : $"\"{value}\"";
    var json = $$"""{"Primitive":{{asString}}}""";

    var act = () => JsonConvert.DeserializeObject<JsonTestClass>( json );

    act.Should()
       .Throw<NsjJsonException>()
       .WithMessage( DateTimeOffsetPrimitive.ExpectedValidationErrorMessage );
  }

  [Fact]
  public void NewtonsoftJson_Deserialization_WithNull_ShouldThrow()
  {
    var json = """{"Primitive":null}""";

    var act = () => JsonConvert.DeserializeObject<JsonTestClass>( json );

    act.Should()
       .Throw<NsjJsonException>()
       .WithMessage( DateTimeOffsetPrimitive.ExpectedValidationErrorMessage );
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
    var test = new JsonTestClass { Primitive = ( DateTimeOffsetPrimitive ) value };

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
       .WithMessage( DateTimeOffsetPrimitive.ExpectedValidationErrorMessage );
  }

  [Fact]
  public void SystemTextJson_Deserialization_WithValidValue_ShouldSucceed()
  {
    var value = s_validValueA;
    var json = $$"""{"Primitive":"{{value:O}}"}""";

    var result = StjJsonSerializer.Deserialize<JsonTestClass>( json );

    result.Should()
          .NotBeNull();

    result!.Primitive.Value.Should()
           .Be( value );
  }

  [Theory]
  [MemberData( nameof( InvalidValues ) )]
  public void SystemTextJson_DeserializationInvalidValue_ShouldThrow(
    DateTimeOffset? value )
  {
    var asString = value is null ? "null" : $"\"{value}\"";
    var json = $$"""{"Primitive":{{asString}}}""";

    var act = () => StjJsonSerializer.Deserialize<JsonTestClass>( json );

    act.Should()
       .Throw<StjJsonException>()
       .WithMessage( DateTimeOffsetPrimitive.ExpectedValidationErrorMessage );
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
    var expected = new JsonTestClass { Primitive = ( DateTimeOffsetPrimitive ) value };

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
    var primitive = ( DateTimeOffsetPrimitive ) default;

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
    var primitive = ( DateTimeOffsetPrimitive ) value;

    // Act
    var result = primitive.ToString();

    // Assert
    result.Should()
          .Be( value.ToString() );
  }

  [Fact]
  public void TypeConverter_CanConvertFrom_SupportedType_ReturnsTrue()
  {
    var converter = TypeDescriptor.GetConverter( typeof( DateTimeOffsetPrimitive ) );
    converter.CanConvertFrom( typeof( DateTimeOffset ) )
             .Should()
             .BeTrue();
  }

  [Fact]
  public void TypeConverter_CanConvertTo_SupportedType_ReturnsTrue()
  {
    var converter = TypeDescriptor.GetConverter( typeof( DateTimeOffsetPrimitive ) );
    converter.CanConvertTo( typeof( DateTimeOffset ) )
             .Should()
             .BeTrue();
  }

  [Fact]
  public void TypeConverter_CanConvertTo_UnsupportedType_ReturnsFalse()
  {
    var converter = TypeDescriptor.GetConverter( typeof( DateTimeOffsetPrimitive ) );
    converter.CanConvertTo( typeof( string ) )
             .Should()
             .BeFalse();
  }

  [Fact]
  public void TypeConverter_ConvertFrom_NullValue_ReturnsDefault()
  {
    var converter = TypeDescriptor.GetConverter( typeof( DateTimeOffsetPrimitive ) );
    var result = ( DateTimeOffsetPrimitive ) converter.ConvertFrom( null! )!;

    result.Should()
          .BeOfType<DateTimeOffsetPrimitive>()
          .Which.Should()
          .Be( default );
  }

  [Fact]
  public void TypeConverter_ConvertFrom_SupportedType_Succeeds()
  {
    var value = s_validValueA;
    var converter = TypeDescriptor.GetConverter( typeof( DateTimeOffsetPrimitive ) );
    var result = converter.ConvertFrom( value );

    result.Should()
          .BeOfType<DateTimeOffsetPrimitive>()
          .Which.Value.Should()
          .Be( value );
  }

  [Fact]
  public void TypeConverter_ConvertFrom_UnsupportedType_Throws()
  {
    var converter = TypeDescriptor.GetConverter( typeof( DateTimeOffsetPrimitive ) );

    var value = "12345";
    var act = () => converter.ConvertFrom( value );

    act.Should()
       .Throw<NotSupportedException>();
  }

  [Fact]
  public void TypeConverter_ConvertTo_SupportedType_Succeeds()
  {
    var converter = TypeDescriptor.GetConverter( typeof( DateTimeOffsetPrimitive ) );

    var value = s_validValueA;
    var primitive = ( DateTimeOffsetPrimitive ) value;
    var result = converter.ConvertTo( null, null, primitive, typeof( DateTimeOffset ) );

    result.Should()
          .BeOfType<DateTimeOffset>()
          .Which.Should()
          .Be( value );
  }

  [Fact]
  public void TypeConverter_ConvertTo_UnsupportedType_Throws()
  {
    var converter = TypeDescriptor.GetConverter( typeof( DateTimeOffsetPrimitive ) );

    var primitive = ( DateTimeOffsetPrimitive ) s_validValueA;
    var act = () => converter.ConvertTo( null, null, primitive, typeof( string ) );

    act.Should()
       .Throw<NotSupportedException>();
  }

  [Fact]
  public void TypeConverter_IsFound()
  {
    var converter = TypeDescriptor.GetConverter( typeof( DateTimeOffsetPrimitive ) );
    converter.Should()
             .NotBeNull();
  }

  [Theory]
  [MemberData( nameof( InvalidValues ) )]
  public void Validate_InvalidValue_ReturnsFailure(
    DateTimeOffset? value )
  {
    // Act
    var result = DateTimeOffsetPrimitive.Validate( value );

    // Assert
    result.IsFailed.Should()
          .BeTrue();

    result.Errors.Select( error => error.Message )
          .Should()
          .ContainSingle()
          .Which
          .Should()
          .Be( DateTimeOffsetPrimitive.ExpectedValidationErrorMessage );
  }

  [Fact]
  public void Validate_ValidValue_ReturnsSuccess()
  {
    // Act
    var result = DateTimeOffsetPrimitive.Validate( s_validValueA );

    // Assert
    result.IsSuccess.Should()
          .BeTrue();
  }

  [Theory]
  [MemberData( nameof( InvalidValues ) )]
  public void Validate_WithUnvalidatedPrimitiveAndInvalidValue_ReturnsSuccess(
    DateTimeOffset? value )
  {
    // Act
    var result = UnvalidatedDateTimeOffsetPrimitive.Validate( value );

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
      Primitive = ( DateTimeOffsetPrimitive ) value
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
    new object?[] { DateTimeOffset.MinValue }
  };

  internal class JsonTestClass
  {
    #region Properties

    public DateTimeOffsetPrimitive Primitive { get; set; }

    #endregion
  }

  internal class TestEntity
  {
    #region Properties

    public Guid Id { get; set; } = Guid.NewGuid();
    public DateTimeOffsetPrimitive Primitive { get; set; }

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
                  .HasConversion( new DateTimeOffsetPrimitiveValueConverter() )
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
