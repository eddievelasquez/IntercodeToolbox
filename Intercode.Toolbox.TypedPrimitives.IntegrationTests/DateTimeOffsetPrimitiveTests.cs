namespace Intercode.Toolbox.TypedPrimitives.IntegrationTests;

using System.ComponentModel;
using System.Text.Json;
using FluentAssertions;
using FluentResults;
using Intercode.Toolbox.TypedPrimitives;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

[TypedPrimitive(
  typeof( DateTimeOffset ),
  ValidatorType = typeof( DateTimeOffsetPrimitiveTests.Validator ),
  ValidatorFlagsType = typeof( DateTimeOffsetPrimitiveTests.ValidatorFlags ),
  ValidatorFlagsDefaultValue = DateTimeOffsetPrimitiveTests.ValidatorFlags.Simple
)]
public readonly partial record struct DateTimeOffsetPrimitive;

public class DateTimeOffsetPrimitiveTests
{
  #region Constants

  private static readonly string ExpectedValidationErrorMessage = "Cannot be null or empty";
  private static readonly string JsonInvalidTokenTypeErrorMessage = "Value must be a String";
  private static readonly DateTimeOffset ValidValueA = DateTimeOffset.ParseExact( "1995-12-01T15:00:00", "s", null );
  private static readonly DateTimeOffset ValidValueB = DateTimeOffset.ParseExact( "2018-02-06T12:45:00", "s", null );

  #endregion

  #region Tests

  [Fact]
  public void CompareTo_DefaultAndValue_ReturnsLessThanZero()
  {
    // Arrange
    var a = ( DateTimeOffsetPrimitive ) ValidValueA;
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
    var valueA = ValidValueA;
    var valueB = ValidValueB;

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
    var a = ( DateTimeOffsetPrimitive ) ValidValueA;

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
    var primitive = ( DateTimeOffsetPrimitive ) ValidValueA;

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
    var a = ( DateTimeOffsetPrimitive ) ValidValueA;
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
    var a = ( DateTimeOffsetPrimitive ) ValidValueA;
    var b = ( DateTimeOffsetPrimitive ) ValidValueB;

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
    var a = ( DateTimeOffsetPrimitive ) ValidValueA;
    var b = ( DateTimeOffsetPrimitive ) ValidValueA;

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
    var primitive = ( DateTimeOffsetPrimitive ) ValidValueA;

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
    var value = ValidValueA;
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
       .WithMessage( ExpectedValidationErrorMessage );
  }

  [Fact]
  public void ExplicitOperator_ValueToPrimitive_ReturnsPrimitiveWithValue()
  {
    // Act
    var value = ValidValueA;
    var primitive = ( DateTimeOffsetPrimitive ) value;

    // Assert
    primitive.Value.Should()
             .Be( value );
  }

  [Fact]
  public void FromInt32_WithValidValue_ReturnsSuccess()
  {
    // Act
    var value = ValidValueA;
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
          .Be( ExpectedValidationErrorMessage );
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
    var valueA = ValidValueA;
    var valueB = ValidValueB;
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
    var value = ValidValueA;
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
    var primitive = ( DateTimeOffsetPrimitive ) ValidValueA;

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
    var isValid = DateTimeOffsetPrimitive.IsValid( ValidValueA );

    // Assert
    isValid.Should()
           .BeTrue();
  }

  [Fact]
  public void SystemTextJson_Deserialization_WithInvalidType_ShouldThrow()
  {
    var json = """{"DateTimeOffsetPrimitive":12345}""";

    // The JSON deserializer should use DateTimeOffsetPrimitive's SystemTextJsonConverter
    var act = () => JsonSerializer.Deserialize<JsonTestClass>( json );

    act.Should()
       .Throw<JsonException>()
       .WithMessage( JsonInvalidTokenTypeErrorMessage );
  }

  [Fact]
  public void SystemTextJson_Deserialization_WithNull_ShouldThrow()
  {
    var json = """{"DateTimeOffsetPrimitive":null}""";

    // The JSON deserializer should use DateTimeOffsetPrimitive's SystemTextJsonConverter
    var act = () => JsonSerializer.Deserialize<JsonTestClass>( json );

    act.Should()
       .Throw<JsonException>()
       .WithMessage( ExpectedValidationErrorMessage );
  }

  [Fact]
  public void SystemTextJson_Deserialization_WithValidValue_ShouldSucceed()
  {
    var value = ValidValueA;
    var json = $$"""{"DateTimeOffsetPrimitive":"{{value:O}}"}""";

    // The JSON deserializer should use DateTimeOffsetPrimitive's SystemTextJsonConverter
    var result = JsonSerializer.Deserialize<JsonTestClass>( json );

    result.Should()
          .NotBeNull();

    result!.DateTimeOffsetPrimitive.Value.Should()
           .Be( value );
  }

  [Theory]
  [MemberData( nameof( InvalidValues ) )]
  public void SystemTextJson_DeserializationInvalidValue_ShouldThrow(
    DateTimeOffset? value )
  {
    var asString = value is null ? "null" : $"\"{value}\"";
    var json = $$"""{"DateTimeOffsetPrimitive":{{asString}}}""";

    // The JSON deserializer should use DateTimeOffsetPrimitive's SystemTextJsonConverter
    var act = () => JsonSerializer.Deserialize<JsonTestClass>( json );

    act.Should()
       .Throw<JsonException>()
       .WithMessage( ExpectedValidationErrorMessage );
  }

  [Fact]
  public void SystemTextJson_Serialization_WithDefault_ShouldSucceed()
  {
    var test = new JsonTestClass { DateTimeOffsetPrimitive = default };

    // The JSON serializer should use DateTimeOffsetPrimitive's SystemTextJsonConverter
    var json = JsonSerializer.Serialize( test );

    json.Should()
        .Be( """{"DateTimeOffsetPrimitive":null}""" );
  }

  [Fact]
  public void SystemTextJson_Serialization_WithValidValue_ShouldSucceed()
  {
    var value = ValidValueA;
    var expected = new JsonTestClass { DateTimeOffsetPrimitive = ( DateTimeOffsetPrimitive ) value };

    // The JSON serializer should use DateTimeOffsetPrimitive's SystemTextJsonConverter
    var json = JsonSerializer.Serialize( expected );

    // Use roundtrip to avoid JSON encoding failures on Linux.
    // This is safer anyway, as it ensures the JSON is valid.
    var actual = JsonSerializer.Deserialize<JsonTestClass>( json );

    actual.Should()
          .NotBeNull();

    actual!.DateTimeOffsetPrimitive.Value.Should()
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
    var value = ValidValueA;
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
    var value = ValidValueA;
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

    var value = ValidValueA;
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

    var primitive = ( DateTimeOffsetPrimitive ) ValidValueA;
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
          .Be( ExpectedValidationErrorMessage );
  }

  [Fact]
  public void Validate_ValidValue_ReturnsSuccess()
  {
    // Act
    var result = DateTimeOffsetPrimitive.Validate( ValidValueA );

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
    var value = ValidValueA;
    var entity = new TestEntity
    {
      Id = id,
      DateTimeOffsetPrimitive = ( DateTimeOffsetPrimitive ) value
    };

    // Act
    context.Entities.Add( entity );
    context.SaveChanges();

    // Assert
    var result = context.Entities.Single( e => e.Id == id );

    result.DateTimeOffsetPrimitive.Value.Should()
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
      DateTimeOffset? value,
      ValidatorFlags flags = default )
    {
      return flags switch
      {
        ValidatorFlags.None => Result.Ok(),
        ValidatorFlags.Simple => Result.FailIf(
          value is null || value.Value == DateTimeOffset.MinValue,
          ExpectedValidationErrorMessage
        ),
        _ => Result.Fail( "Invalid validation flag" )
      };
    }

    #endregion
  }

  public static IEnumerable<object?[]> InvalidValues => new List<object?[]>
  {
    new object?[] { null },
    new object?[] { DateTimeOffset.MinValue }
  };

  internal class JsonTestClass
  {
    #region Properties

    public DateTimeOffsetPrimitive DateTimeOffsetPrimitive { get; set; }

    #endregion
  }

  internal class TestEntity
  {
    #region Properties

    public Guid Id { get; set; } = Guid.NewGuid();
    public DateTimeOffsetPrimitive DateTimeOffsetPrimitive { get; set; }

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
                  .Property( e => e.DateTimeOffsetPrimitive )
                  .HasConversion( new DateTimeOffsetPrimitive.ValueConverter() )
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
