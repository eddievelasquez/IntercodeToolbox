// Module Name: ValueTypePrimitiveTests.cs
// Author:      Eduardo Velasquez
// Copyright (c) 2024, Intercode Consulting, Inc.

namespace Intercode.Toolbox.TypedPrimitives.IntegrationTests;

using System.ComponentModel;
using FluentAssertions;
using Intercode.Toolbox.Core;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Newtonsoft.Json;
using StjJsonException = System.Text.Json.JsonException;
using StjJsonSerializer = System.Text.Json.JsonSerializer;
using NsjJsonException = Newtonsoft.Json.JsonException;

public abstract class ValueTypePrimitiveTests<T, TPrimitive, TUnvalidatedPrimitive, TValueConverter>
  where TPrimitive: struct, IValueTypePrimitive<TPrimitive, T>, IComparable<TPrimitive>, IComparable, ISpanFormattable,
  ISpanParsable<TPrimitive>
  where TUnvalidatedPrimitive: struct, IValueTypePrimitive<TUnvalidatedPrimitive, T>
  where T: struct, IComparable<T>, IComparable, ISpanParsable<T>
  where TValueConverter: ValueConverter, new()
{
  #region Constants

  private const string NUMERIC_VALIDATION_ERROR_MSG = "Cannot be null or zero";
  private const string TEXT_VALIDATION_ERROR_MSG = "Cannot be null or empty";

  #endregion

  #region Fields

  private readonly string _validationError;
  private readonly string _jsonValidationError;
  private readonly T[] _validValues;
  private readonly bool _isNumeric;

  #endregion

  #region Setup/Teardown

  protected ValueTypePrimitiveTests(
    string jsonValidationError,
    T[] validValues )
  {
    ArgumentException.ThrowIfNullOrEmpty( jsonValidationError );
    ArgumentNullException.ThrowIfNull( validValues );
    ArgumentOutOfRangeException.ThrowIfLessThan( validValues.Length, 2 );

    _isNumeric = typeof( T ).IsNumeric();
    _validationError = _isNumeric ? NUMERIC_VALIDATION_ERROR_MSG : TEXT_VALIDATION_ERROR_MSG;
    _jsonValidationError = jsonValidationError;
    _validValues = validValues;
  }

  #endregion

  #region Tests

  [Fact]
  public void CompareTo_DefaultAndValue_ReturnsLessThanZero()
  {
    // Arrange
    var a = ( TPrimitive ) _validValues[0];
    var b = ( TPrimitive ) default;

    // Act
    var result = b.CompareTo( a );

    // Assert
    result.Should()
          .BeLessThan( 0 );
  }

  [Fact]
  public void CompareTo_DifferentObjectValues_ReturnsComparisonResult()
  {
    var valueA = _validValues[0];
    var valueB = _validValues[1];

    // Arrange
    var a = ( TPrimitive ) valueA;
    var b = ( TPrimitive ) valueB;

    // Act
    var result = a.CompareTo( ( object? ) b );

    // Assert
    result.Should()
          .Be( valueA.CompareTo( valueB ) );
  }

  [Fact]
  public void CompareTo_ObjectIsNull_ReturnsGreaterThanZero()
  {
    // Arrange
    var a = ( TPrimitive ) _validValues[0];

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
    var primitive = ( TPrimitive ) _validValues[0];

    // Act
    var result = primitive.CompareTo( default );

    // Assert
    result.Should()
          .BeGreaterThan( 0 );
  }

  [Theory]
  [MemberData( nameof( InvalidValues ) )]
  public void Create_WithInvalidValue_ReturnsFailure(
    T? value )
  {
    // Act
    var result = TPrimitive.Create( value );

    // Assert
    result.IsFailed.Should()
          .BeTrue();

    result.Errors.Select( error => error.Message )
          .Should()
          .ContainSingle()
          .Which
          .Should()
          .Be( _validationError );
  }

  [Fact]
  public void Create_WithValidValue_ReturnsSuccess()
  {
    // Act
    var value = _validValues[0];
    var result = TPrimitive.Create( value );

    // Assert
    result.IsSuccess.Should()
          .BeTrue();

    result.Value.Value.Should()
          .Be( value );
  }

  [Theory]
  [MemberData( nameof( InvalidValues ) )]
  public void CreateOrThrow_WithInvalidValue_Throws(
    T? value )
  {
    // Act
    var act = () => TPrimitive.CreateOrThrow( value );

    // Assert
    act.Should()
       .Throw<ArgumentException>()
       .WithMessage( _validationError );
  }

  [Fact]
  public void CreateOrThrow_WithValidValue_ReturnsPrimitive()
  {
    // Act
    var value = _validValues[0];
    var primitive = TPrimitive.CreateOrThrow( value );

    // Assert
    primitive.Value.Should().Be( value );
  }

  [Fact]
  public void Equals_DefaultWithValue_ReturnsFalse()
  {
    // Arrange
    var a = ( TPrimitive ) _validValues[0];
    var b = ( TPrimitive ) default;

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
    var a = ( TPrimitive ) _validValues[0];
    var b = ( TPrimitive ) _validValues[1];

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
    var a = ( TPrimitive ) _validValues[0];
    var b = ( TPrimitive ) _validValues[0];

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
    var primitive = ( TPrimitive ) _validValues[0];

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
    var primitive = ( TPrimitive ) default;

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
    var value = _validValues[0];
    var primitive = ( TPrimitive ) value;

    // Act
    var result = primitive.Value;

    // Assert
    result.Should()
          .Be( value );
  }

  [Theory]
  [MemberData( nameof( InvalidValues ) )]
  public void ExplicitOperator_StringToPrimitive_WithInvalidValue_Throws(
    T? value )
  {
    // Act
    var act = () => ( TPrimitive ) value;

    // Assert
    act.Should()
       .Throw<InvalidOperationException>()
       .WithMessage( _validationError );
  }

  [Fact]
  public void ExplicitOperator_ValueToPrimitive_ReturnsPrimitiveWithValue()
  {
    // Act
    var value = _validValues[0];
    var primitive = ( TPrimitive ) value;

    // Assert
    primitive.Value.Should()
             .Be( value );
  }

  [Fact]
  public void GetHashCode_Default_ReturnsZero()
  {
    // Act
    var hashCodeA = ( ( TPrimitive ) default ).GetHashCode();

    // Assert
    hashCodeA.Should()
             .Be( 0 );
  }

  [Fact]
  public void GetHashCode_DifferentValues_ReturnsDifferentHashCodes()
  {
    // Arrange
    var valueA = _validValues[0];
    var valueB = _validValues[1];
    var a = ( TPrimitive ) valueA;
    var b = ( TPrimitive ) valueB;

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
    var value = _validValues[0];
    var a = ( TPrimitive ) value;
    var b = ( TPrimitive ) value;

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
    var primitive = ( TPrimitive ) default;

    // Act
    var result = primitive.IsDefault;

    result.Should()
          .BeTrue();
  }

  [Fact]
  public void IsDefault_WithValue_ReturnsFalse()
  {
    // Arrange
    var primitive = ( TPrimitive ) _validValues[0];

    // Act
    var result = primitive.IsDefault;

    result.Should()
          .BeFalse();
  }

  [Theory]
  [MemberData( nameof( InvalidValues ) )]
  public void IsValid_InvalidValue_ReturnsFalse(
    T? value )
  {
    // Act
    var isValid = TPrimitive.IsValid( value );

    // Assert
    isValid.Should()
           .BeFalse();
  }

  [Fact]
  public void IsValid_StringValidValue_ReturnsTrue()
  {
    // Act
    var isValid = TPrimitive.IsValid( _validValues[0] );

    // Assert
    isValid.Should()
           .BeTrue();
  }

  [Theory]
  [MemberData( nameof( InvalidValues ) )]
  public void NewtonsoftJson_Deserialization_InvalidValue_ShouldThrow(
    T? value )
  {
    var asString = value is null ? "null" : value.ToString();
    var json = $$"""{"Primitive":{{asString}}}""";

    var act = () => JsonConvert.DeserializeObject<JsonTestClass>( json );

    act.Should()
       .Throw<NsjJsonException>()
       .WithMessage( _validationError );
  }

  [Fact]
  public void NewtonsoftJson_Deserialization_WithNull_ShouldThrow()
  {
    var json = """{"Primitive":null}""";

    var act = () => JsonConvert.DeserializeObject<JsonTestClass>( json );

    act.Should()
       .Throw<NsjJsonException>()
       .WithMessage( _validationError );
  }

  [Fact]
  public void NewtonsoftJson_Serialization_RoundTripWithValidValue_ShouldSucceed()
  {
    var value = _validValues[0];
    var test = new JsonTestClass { Primitive = ( TPrimitive ) value };

    var json = JsonConvert.SerializeObject( test );

    var deserialized = JsonConvert.DeserializeObject<JsonTestClass>( json );
    deserialized.Should().NotBeNull();

    deserialized!.Primitive.Value.Should()
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
  public virtual void SystemTextJson_Deserialization_WithInvalidType_ShouldThrow()
  {
    var json = ToJson( "12345" );

    var act = () => StjJsonSerializer.Deserialize<JsonTestClass>( json );

    act.Should()
       .Throw<StjJsonException>()
       .WithMessage( _jsonValidationError );
  }

  [Fact]
  public void SystemTextJson_Deserialization_WithNull_ShouldThrow()
  {
    var json = """{"Primitive":null}""";

    var act = () => StjJsonSerializer.Deserialize<JsonTestClass>( json );

    act.Should()
       .Throw<StjJsonException>()
       .WithMessage( _validationError );
  }

  [Fact]
  public void SystemTextJson_Deserialization_WithValidValue_ShouldSucceed()
  {
    var value = _validValues[0];
    var json = ToJson( value );

    var result = StjJsonSerializer.Deserialize<JsonTestClass>( json );

    result.Should()
          .NotBeNull();

    result!.Primitive.Value.Should()
           .Be( value );
  }

  [Theory]
  [MemberData( nameof( InvalidValues ) )]
  public void SystemTextJson_DeserializationInvalidValue_ShouldThrow(
    T? value )
  {
    var asString = value is null ? "null" : value.ToString();
    var json = $$"""{"Primitive":{{asString}}}""";

    var act = () => StjJsonSerializer.Deserialize<JsonTestClass>( json );

    act.Should()
       .Throw<StjJsonException>()
       .WithMessage( _validationError );
  }

  [Fact]
  public void SystemTextJson_Serialization_RoundTripWithValidValue_ShouldSucceed()
  {
    var value = _validValues[0];
    var test = new JsonTestClass { Primitive = ( TPrimitive ) value };

    var json = StjJsonSerializer.Serialize( test );
    var deserialized = StjJsonSerializer.Deserialize<JsonTestClass>( json );
    deserialized.Should().NotBeNull();

    deserialized!.Primitive.Value.Should()
                 .Be( value );
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
  public void ToString_Default_ReturnsEmpty()
  {
    // Arrange
    var primitive = ( TPrimitive ) default;

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
    var value = _validValues[0];
    var primitive = ( TPrimitive ) value;

    // Act
    var result = primitive.ToString();

    // Assert
    result.Should()
          .Be( value.ToString() );
  }

  [Fact]
  public void TypeConverter_CanConvertFrom_SupportedType_ReturnsTrue()
  {
    var converter = TypeDescriptor.GetConverter( typeof( TPrimitive ) );
    converter.CanConvertFrom( typeof( T ) )
             .Should()
             .BeTrue();
  }

  [Fact]
  public void TypeConverter_CanConvertTo_SupportedType_ReturnsTrue()
  {
    var converter = TypeDescriptor.GetConverter( typeof( TPrimitive ) );
    converter.CanConvertTo( typeof( T ) )
             .Should()
             .BeTrue();
  }

  [Fact]
  public void TypeConverter_CanConvertTo_UnsupportedType_ReturnsFalse()
  {
    var converter = TypeDescriptor.GetConverter( typeof( TPrimitive ) );
    converter.CanConvertTo( typeof( string ) )
             .Should()
             .BeFalse();
  }

  [Fact]
  public void TypeConverter_ConvertFrom_NullValue_ReturnsDefault()
  {
    var converter = TypeDescriptor.GetConverter( typeof( TPrimitive ) );
    var result = ( TPrimitive ) converter.ConvertFrom( null! )!;

    result.Should()
          .BeOfType<TPrimitive>()
          .Which.Should()
          .Be( default );
  }

  [Fact]
  public void TypeConverter_ConvertFrom_SupportedType_Succeeds()
  {
    var value = _validValues[0];
    var converter = TypeDescriptor.GetConverter( typeof( TPrimitive ) );
    var result = converter.ConvertFrom( value );

    result.Should()
          .BeOfType<TPrimitive>()
          .Which.Value.Should()
          .Be( value );
  }

  [Fact]
  public void TypeConverter_ConvertFrom_UnsupportedType_Throws()
  {
    var converter = TypeDescriptor.GetConverter( typeof( TPrimitive ) );

    var value = "12345";
    var act = () => converter.ConvertFrom( value );

    act.Should()
       .Throw<NotSupportedException>();
  }

  [Fact]
  public void TypeConverter_ConvertTo_SupportedType_Succeeds()
  {
    var converter = TypeDescriptor.GetConverter( typeof( TPrimitive ) );

    var value = _validValues[0];
    var primitive = ( TPrimitive ) value;
    var result = converter.ConvertTo( null, null, primitive, typeof( T ) );

    result.Should()
          .BeOfType<T>()
          .Which.Should()
          .Be( value );
  }

  [Fact]
  public void TypeConverter_ConvertTo_UnsupportedType_Throws()
  {
    var converter = TypeDescriptor.GetConverter( typeof( TPrimitive ) );

    var primitive = ( TPrimitive ) _validValues[0];
    var act = () => converter.ConvertTo( null, null, primitive, typeof( string ) );

    act.Should()
       .Throw<NotSupportedException>();
  }

  [Fact]
  public void TypeConverter_IsFound()
  {
    var converter = TypeDescriptor.GetConverter( typeof( TPrimitive ) );
    converter.Should()
             .NotBeNull();
  }

  [Theory]
  [MemberData( nameof( InvalidValues ) )]
  public void Validate_WithInvalidValue_ReturnsFailure(
    T? value )
  {
    // Act
    var result = TPrimitive.Validate( value );

    // Assert
    result.IsFailed.Should()
          .BeTrue();

    result.Errors.Select( error => error.Message )
          .Should()
          .ContainSingle()
          .Which
          .Should()
          .Be( _validationError );
  }

  [Theory]
  [MemberData( nameof( InvalidValues ) )]
  public void Validate_WithUnvalidatedPrimitiveAndInvalidValue_ReturnsSuccess(
    T? value )
  {
    // Act
    var result = TUnvalidatedPrimitive.Validate( value );

    // Assert
    result.IsSuccess.Should()
          .BeTrue();
  }

  [Fact]
  public void Validate_WithValidValue_ReturnsSuccess()
  {
    // Act
    var result = TPrimitive.Validate( _validValues[0] );

    // Assert
    result.IsSuccess.Should()
          .BeTrue();
  }

  [Theory]
  [MemberData( nameof( InvalidValues ) )]
  public void ValidateOrThrow_WithInvalidValue_ShouldThrow(
    T? value )
  {
    // Act
    var act = () => TPrimitive.ValidateOrThrow( value );

    // Assert
    act.Should()
       .Throw<ArgumentException>()
       .WithMessage( _validationError );
  }

  [Theory]
  [MemberData( nameof( InvalidValues ) )]
  public void ValidateOrThrow_WithUnvalidatedPrimitiveAndInvalidValue_ShouldNotThrow(
    T? value )
  {
    // Act
    var act = () => TUnvalidatedPrimitive.ValidateOrThrow( value );

    // Assert
    act.Should()
       .NotThrow();
  }

  [Fact]
  public void ValidateOrThrow_WithValidValue_ShouldNotThrow()
  {
    // Act
    var act = () => TPrimitive.ValidateOrThrow( _validValues[0] );

    // Assert
    act.Should()
       .NotThrow();
  }

  [Fact]
  public void ValueConverter_WithValidValue_Succeeds()
  {
    // Arrange
    using var context = new TestDbContext( new TValueConverter() );
    context.Database.EnsureCreated();

    var id = Guid.NewGuid();
    var value = _validValues[0];
    var entity = new TestEntity
    {
      Id = id,
      Primitive = ( TPrimitive ) value
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

  protected static string ToJson(
    object? value )
  {
    if( value is null )
    {
      return """{""Primitive"":null}""";
    }

    if( value.IsNumber() )
    {
      return $$"""{"Primitive":{{value}}}""";
    }

    return $$"""{"Primitive":"{{value}}"}""";
  }

  public static TheoryData<T?> InvalidValues => new InvalidTheoryData();

  private class InvalidTheoryData: TheoryData<T?>
  {
    #region Constructors

    public InvalidTheoryData()
    {
      Add( null );
      Add( default );
    }

    #endregion
  }

  internal class JsonTestClass
  {
    #region Properties

    public TPrimitive Primitive { get; set; }

    #endregion
  }

  internal class TestEntity
  {
    #region Properties

    public Guid Id { get; set; } = Guid.NewGuid();
    public TPrimitive Primitive { get; set; }

    #endregion
  }

  internal class TestDbContext: DbContext
  {
    #region Fields

    private readonly ValueConverter _valueConverter;

    #endregion

    #region Constructors

    public TestDbContext(
      ValueConverter valueConverter )
    {
      ArgumentNullException.ThrowIfNull( valueConverter );
      _valueConverter = valueConverter;
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
                  .HasConversion( _valueConverter )
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
