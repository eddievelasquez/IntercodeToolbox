// Module Name: DateTimeOffsetPrimitiveTests.cs
// Author:      Eduardo Velasquez
// Copyright (c) 2024, Intercode Consulting, Inc.

namespace Intercode.Toolbox.TypedPrimitives.IntegrationTests;

using FluentResults;
using Intercode.Toolbox.TypedPrimitives.IntegrationTests.Fixtures;
using Newtonsoft.Json;

[TypedPrimitive<DateTimeOffset>]
public readonly partial struct UnvalidatedDateTimeOffsetPrimitive;

[TypedPrimitive( typeof( DateTimeOffset ), Converters = TypedPrimitiveConverter.All )]
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
    result = Result.FailIf( value is null || value.Value == DateTimeOffset.MinValue, ExpectedValidationErrorMessage );
  }

  #endregion
}

public class DateTimeOffsetPrimitiveTests
{
  #region Nested Types

  public class Comparable
    : ComparableTests<DateTimeOffsetPrimitive, Comparable>,
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
    : ComparableOfTTests<DateTimeOffsetPrimitive, ComparableOfT>,
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
    : ValuePrimitiveCreationTests<DateTimeOffset, DateTimeOffsetPrimitive, ValueFactory>;

  public new class Equals
    : EqualsTests<DateTimeOffsetPrimitive, Equals>,
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
    : GetHashCodeTests<DateTimeOffsetPrimitive, GetHashCode>,
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
    : NewtonsoftJsonTests<DateTimeOffsetPrimitive, PrimitiveFactory>;

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
    : SpanFormattableTests<DateTimeOffsetPrimitive, SpanFormattable>,
      ITestDataFactory
  {
    #region Public Methods

    public static IEnumerable<object?[]> GetValidValues()
    {
      yield return [s_primitiveA, "12/1/1995 3:00:00 PM +00:00", null, null!];
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
    : SpanParsableTests<DateTimeOffsetPrimitive, SpanParsable>,
      ITestDataFactory
  {
    #region Public Methods

    public static IEnumerable<object?[]> GetValidValues()
    {
      yield return ["12/1/1995 3:00:00 PM +00:00", s_primitiveA, null];
    }

    public static IEnumerable<object?[]> GetInvalidValues()
    {
      yield break;
    }

    #endregion
  }

  public class SystemTextJson
    : SystemTextJsonTests<DateTimeOffsetPrimitive, PrimitiveFactory>;

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
    : UnvalidatedValuePrimitiveValidationTests<DateTimeOffset, UnvalidatedDateTimeOffsetPrimitive, ValueFactory>;

  public class Validation
    : ValuePrimitiveValidationTests<DateTimeOffset, DateTimeOffsetPrimitive, ValueFactory>;

  public class ValueConverter
    : ValueConverterTests<DateTimeOffsetPrimitive, DateTimeOffsetPrimitiveValueConverter, ValueConverter>,
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
      yield return [DateTimeOffset.MinValue];
    }

    #endregion
  }

  #endregion

  #region Constants

  private static readonly DateTimeOffset s_valueA = DateTimeOffset.ParseExact( "1995-12-01 15:00:00Z", "u", null );
  private static readonly DateTimeOffset s_valueB = DateTimeOffset.ParseExact( "2018-02-06 12:45:00Z", "u", null );
  private static readonly DateTimeOffsetPrimitive s_primitiveA = DateTimeOffsetPrimitive.CreateOrThrow( s_valueA );
  private static readonly DateTimeOffsetPrimitive s_primitiveB = DateTimeOffsetPrimitive.CreateOrThrow( s_valueB );
  private static readonly DateTimeOffsetPrimitive s_defaultPrimitive = default;

  #endregion
}
