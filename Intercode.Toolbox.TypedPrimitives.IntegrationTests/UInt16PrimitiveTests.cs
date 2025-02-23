// Module Name: UInt16PrimitiveTests.cs
// Author:      Eduardo Velasquez
// Copyright (c) 2024, Intercode Consulting, Inc.

namespace Intercode.Toolbox.TypedPrimitives.IntegrationTests;

using FluentResults;
using Intercode.Toolbox.TypedPrimitives.IntegrationTests.Fixtures;

[TypedPrimitive<ushort>]
public readonly partial struct UnvalidatedUInt16Primitive;

[TypedPrimitive( typeof( ushort ), Converters = TypedPrimitiveConverter.All )]
public readonly partial struct UInt16Primitive
{
  #region Constants

  public const string ExpectedValidationErrorMessage = "Cannot be null or zero";

  #endregion

  #region Implementation

  static partial void ValidatePartial(
    ushort? value,
    ref Result result )
  {
    result = Result.FailIf( value is null or 0, ExpectedValidationErrorMessage );
  }

  #endregion
}

public class UInt16PrimitiveTests
{
  #region Nested Types

  public class Comparable
    : ComparableTests<UInt16Primitive, Comparable>,
      ITestDataFactory
  {
    #region Public Methods

    public static IEnumerable<object[]> GetValidValues()
    {
      yield return [s_primitiveA, s_primitiveA, 0];
      yield return [s_primitiveA, s_primitiveB, -1];
      yield return [s_primitiveB, s_primitiveA, 1];
      yield return [s_primitiveB, null!, 1];
    }

    public static IEnumerable<object?[]> GetInvalidValues()
    {
      yield break;
    }

    #endregion
  }

  public class ComparableOfT
    : ComparableOfTTests<UInt16Primitive, ComparableOfT>,
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
    : ValuePrimitiveCreationTests<ushort, UInt16Primitive, ValueFactory>;

  public new class Equals
    : EqualsTests<UInt16Primitive, Equals>,
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
    : GetHashCodeTests<UInt16Primitive, GetHashCode>,
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
    : NewtonsoftJsonTests<UInt16Primitive, PrimitiveFactory>;

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
    : SpanFormattableTests<UInt16Primitive, SpanFormattable>,
      ITestDataFactory
  {
    #region Public Methods

    public static IEnumerable<object?[]> GetValidValues()
    {
      yield return [s_primitiveA, "42", "G", null!];
      yield return [s_primitiveA, "42", null, null!];
      yield return [s_primitiveB, "2B", "X2", null!];
    }

    public static IEnumerable<object?[]> GetInvalidValues()
    {
      yield break;
    }

    #endregion
  }

  public class SpanParsable
    : SpanParsableTests<UInt16Primitive, SpanParsable>,
      ITestDataFactory
  {
    #region Public Methods

    public static IEnumerable<object?[]> GetValidValues()
    {
      yield return ["42", s_primitiveA, null];
    }

    public static IEnumerable<object?[]> GetInvalidValues()
    {
      yield break;
    }

    #endregion
  }

  public class SystemTextJson
    : SystemTextJsonTests<UInt16Primitive, PrimitiveFactory>;

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
    : UnvalidatedValuePrimitiveValidationTests<ushort, UnvalidatedUInt16Primitive, ValueFactory>;

  public class Validation
    : ValuePrimitiveValidationTests<ushort, UInt16Primitive, ValueFactory>;

  public class ValueConverter
    : ValueConverterTests<UInt16Primitive, UInt16PrimitiveValueConverter, PrimitiveFactory>;

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
      yield return [(ushort) 0];
    }

    #endregion
  }

  #endregion

  #region Constants

  private static readonly ushort s_valueA = 42;
  private static readonly ushort s_valueB = 43;
  private static readonly UInt16Primitive s_primitiveA = UInt16Primitive.CreateOrThrow( s_valueA );
  private static readonly UInt16Primitive s_primitiveB = UInt16Primitive.CreateOrThrow( s_valueB );
  private static readonly UInt16Primitive s_defaultPrimitive = default;

  #endregion
}
