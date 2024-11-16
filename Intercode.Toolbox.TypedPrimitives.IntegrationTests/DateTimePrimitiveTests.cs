// Module Name: DateTimePrimitiveTests.cs
// Author:      Eduardo Velasquez
// Copyright (c) 2024, Intercode Consulting, Inc.

namespace Intercode.Toolbox.TypedPrimitives.IntegrationTests;

using FluentResults;
using Intercode.Toolbox.TypedPrimitives.IntegrationTests.Fixtures;
using Newtonsoft.Json;

[TypedPrimitive<DateTime>]
public readonly partial struct UnvalidatedDateTimePrimitive;

[TypedPrimitive( typeof( DateTime ), Converters = TypedPrimitiveConverter.All )]
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
    result = Result.FailIf( value is null || value.Value == DateTime.MinValue, ExpectedValidationErrorMessage );
  }

  #endregion
}

public class DateTimePrimitiveTests
{
  #region Nested Types

  public class Comparable
    : ComparableTests<DateTimePrimitive, Comparable>,
      ITestDataFactory
  {
    #region Public Methods

    public static IEnumerable<object[]> GetValidValues()
    {
      yield return [s_primitiveA, s_primitiveA, 0];
      yield return [s_primitiveA, s_primitiveB, -1];
      yield return [s_primitiveB, s_primitiveA, 1];
      yield return [s_primitiveB, null!, 1];
      yield return [s_primitiveB, "12345", 1];
    }

    public static IEnumerable<object?[]> GetInvalidValues()
    {
      yield break;
    }

    #endregion
  }

  public class ComparableOfT
    : ComparableOfTTests<DateTimePrimitive, ComparableOfT>,
      ITestDataFactory
  {
    #region Public Methods

    public static IEnumerable<object[]> GetValidValues()
    {
      yield return [s_primitiveA, s_primitiveA, 0];
      yield return [s_primitiveA, s_primitiveB, -1];
      yield return [s_primitiveB, s_primitiveA, 1];
    }

    public static IEnumerable<object?[]> GetInvalidValues()
    {
      yield break;
    }

    #endregion
  }

  public class Creation
    : ValuePrimitiveCreationTests<DateTime, DateTimePrimitive, ValueFactory>;

  public new class Equals
    : EqualsTests<DateTimePrimitive, Equals>,
      ITestDataFactory
  {
    #region Public Methods

    public static IEnumerable<object[]> GetValidValues()
    {
      yield return [s_primitiveA, s_primitiveA, true];
      yield return [s_primitiveA, s_primitiveB, false];
      yield return [s_primitiveB, s_primitiveA, false];
      yield return [s_primitiveB, null!, false];
      yield return [s_primitiveB, "12345", false];
    }

    public static IEnumerable<object?[]> GetInvalidValues()
    {
      yield break;
    }

    #endregion
  }

  public new class GetHashCode
    : GetHashCodeTests<DateTimePrimitive, GetHashCode>,
      ITestDataFactory
  {
    #region Public Methods

    public static IEnumerable<object[]> GetValidValues()
    {
      yield return [s_primitiveA, s_primitiveA.Value.GetHashCode()];
      yield return [s_primitiveB, s_primitiveB.GetHashCode()];
      yield return [s_defaultPrimitive, 0];
    }

    public static IEnumerable<object?[]> GetInvalidValues()
    {
      yield break;
    }

    #endregion
  }

  public class NewtonsoftJson
    : NewtonsoftJsonTests<DateTimePrimitive, PrimitiveFactory>
  {
    #region Constructors

    public NewtonsoftJson()
    {
      JsonConvert.DefaultSettings = () => new JsonSerializerSettings { DateParseHandling = DateParseHandling.None };
    }

    #endregion
  }

  public class PrimitiveFactory: ITestDataFactory
  {
    #region Public Methods

    public static IEnumerable<object?[]> GetValidValues()
    {
      yield return [s_primitiveA];
      yield return [s_primitiveB];
    }

    public static IEnumerable<object?[]> GetInvalidValues()
    {
      yield return [null];
      yield return [s_defaultPrimitive];
    }

    #endregion
  }

  public class SpanFormattable
    : SpanFormattableTests<DateTimePrimitive, SpanFormattable>,
      ITestDataFactory
  {
    #region Public Methods

    public static IEnumerable<object?[]> GetValidValues()
    {
      yield return [s_primitiveA, "12/1/1995 3:00:00 PM", null, null!];
      yield return [s_primitiveA, "19951201", "yyyyMMdd", null!];
      yield return [s_primitiveB, "2/6/2018", "d", null!];
    }

    public static IEnumerable<object?[]> GetInvalidValues()
    {
      yield break;
    }

    #endregion
  }

  public class SpanParsable
    : SpanParsableTests<DateTimePrimitive, SpanParsable>,
      ITestDataFactory
  {
    #region Public Methods

    public static IEnumerable<object?[]> GetValidValues()
    {
      yield return ["1995-12-01T15:00:00", s_primitiveA, null];
    }

    public static IEnumerable<object?[]> GetInvalidValues()
    {
      yield break;
    }

    #endregion
  }

  public class SystemTextJson
    : SystemTextJsonTests<DateTimePrimitive, PrimitiveFactory>;

  public class TypeConverter
    : TypeConverterTests<TypeConverter>,
      ITestDataFactory
  {
    #region Public Methods

    public static IEnumerable<object?[]> GetValidValues()
    {
      yield return [s_primitiveA, s_primitiveA.Value, true];
      yield return [s_primitiveB, s_primitiveB.Value, true];
      yield return [s_defaultPrimitive, null, true];
      yield return [s_primitiveB, "1234", false];
      yield return [s_primitiveB, "1234", false];
    }

    public static IEnumerable<object?[]> GetInvalidValues()
    {
      yield break;
    }

    #endregion
  }

  public class UnvalidatedValidation
    : UnvalidatedValuePrimitiveValidationTests<DateTime, UnvalidatedDateTimePrimitive, ValueFactory>;

  public class Validation
    : ValuePrimitiveValidationTests<DateTime, DateTimePrimitive, ValueFactory>;

  public class ValueConverter
    : ValueConverterTests<DateTimePrimitive, DateTimePrimitiveValueConverter, ValueConverter>,
      ITestDataFactory
  {
    #region Public Methods

    public static IEnumerable<object?[]> GetValidValues()
    {
      yield return [s_primitiveA];
      yield return [s_primitiveB];
    }

    public static IEnumerable<object?[]> GetInvalidValues()
    {
      yield break;
    }

    #endregion
  }

  public class ValueFactory: ITestDataFactory
  {
    #region Public Methods

    public static IEnumerable<object?[]> GetValidValues()
    {
      yield return [s_valueA];
      yield return [s_valueB];
    }

    public static IEnumerable<object?[]> GetInvalidValues()
    {
      yield return [null];
      yield return [DateTime.MinValue];
    }

    #endregion
  }

  #endregion

  #region Constants

  private static readonly DateTime s_valueA = DateTime.ParseExact( "1995-12-01T15:00:00", "s", null );
  private static readonly DateTime s_valueB = DateTime.ParseExact( "2018-02-06T12:45:00", "s", null );
  private static readonly DateTimePrimitive s_primitiveA = DateTimePrimitive.CreateOrThrow( s_valueA );
  private static readonly DateTimePrimitive s_primitiveB = DateTimePrimitive.CreateOrThrow( s_valueB );
  private static readonly DateTimePrimitive s_defaultPrimitive = default;

  #endregion
}
