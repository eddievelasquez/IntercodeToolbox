// Module Name: GuidPrimitiveTests.cs
// Author:      Eduardo Velasquez
// Copyright (c) 2024, Intercode Consulting, Inc.

namespace Intercode.Toolbox.TypedPrimitives.IntegrationTests;

using FluentResults;
using Intercode.Toolbox.TypedPrimitives.IntegrationTests.Fixtures;
using Newtonsoft.Json;

[TypedPrimitive<Guid>]
public readonly partial struct UnvalidatedGuidPrimitive;

[TypedPrimitive( typeof( Guid ), Converters = TypedPrimitiveConverter.All )]
public readonly partial struct GuidPrimitive
{
  #region Constants

  public const string ExpectedValidationErrorMessage = "Cannot be null or empty";

  #endregion

  #region Implementation

  static partial void ValidatePartial(
    Guid? value,
    ref Result result )
  {
    result = Result.FailIf( value is null || value.Value == Guid.Empty, ExpectedValidationErrorMessage );
  }

  #endregion
}

public class GuidPrimitiveTests
{
  #region Nested Types

  public class Comparable
    : ComparableTests<GuidPrimitive, Comparable>,
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
    : ComparableOfTTests<GuidPrimitive, ComparableOfT>,
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
    : ValuePrimitiveCreationTests<Guid, GuidPrimitive, ValueFactory>;

  public new class Equals
    : EqualsTests<GuidPrimitive, Equals>,
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
    : GetHashCodeTests<GuidPrimitive, GetHashCode>,
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
    : NewtonsoftJsonTests<GuidPrimitive, PrimitiveFactory>
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
    : SpanFormattableTests<GuidPrimitive, SpanFormattable>,
      ITestDataFactory
  {
    #region Public Methods

    public static IEnumerable<object?[]> GetValidValues()
    {
      yield return [s_primitiveA, "3fe7fbd6-ebd7-447c-95b3-0d5e5026d580", null, null!];
      yield return [s_primitiveA, "3fe7fbd6-ebd7-447c-95b3-0d5e5026d580", "D", null!];
      yield return [s_primitiveB, "{b8afdeb5-87d2-44e9-ba51-9369cd257170}", "B", null!];
    }

    public static IEnumerable<object?[]> GetInvalidValues()
    {
      yield break;
    }

    #endregion
  }

  public class SpanParsable
    : SpanParsableTests<GuidPrimitive, SpanParsable>,
      ITestDataFactory
  {
    #region Public Methods

    public static IEnumerable<object?[]> GetValidValues()
    {
      yield return ["3fe7fbd6-ebd7-447c-95b3-0d5e5026d580", s_primitiveA, null];
    }

    public static IEnumerable<object?[]> GetInvalidValues()
    {
      yield break;
    }

    #endregion
  }

  public class SystemTextJson
    : SystemTextJsonTests<GuidPrimitive, PrimitiveFactory>;

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
    : UnvalidatedValuePrimitiveValidationTests<Guid, UnvalidatedGuidPrimitive, ValueFactory>;

  public class Validation
    : ValuePrimitiveValidationTests<Guid, GuidPrimitive, ValueFactory>;

  public class ValueConverter
    : ValueConverterTests<GuidPrimitive, GuidPrimitiveValueConverter, ValueConverter>,
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
      yield return [Guid.Empty];
    }

    #endregion
  }

  #endregion

  #region Constants

  private static readonly Guid s_valueA = Guid.Parse( "3fe7fbd6-ebd7-447c-95b3-0d5e5026d580" );
  private static readonly Guid s_valueB = Guid.Parse( "b8afdeb5-87d2-44e9-ba51-9369cd257170" );
  private static readonly GuidPrimitive s_primitiveA = GuidPrimitive.CreateOrThrow( s_valueA );
  private static readonly GuidPrimitive s_primitiveB = GuidPrimitive.CreateOrThrow( s_valueB );
  private static readonly GuidPrimitive s_defaultPrimitive = default;

  #endregion
}
