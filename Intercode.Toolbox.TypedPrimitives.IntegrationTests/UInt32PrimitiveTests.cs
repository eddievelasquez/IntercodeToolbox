// Module Name: UInt32PrimitiveTests.cs
// Author:      Eduardo Velasquez
// Copyright (c) 2024, Intercode Consulting, Inc.

namespace Intercode.Toolbox.TypedPrimitives.IntegrationTests;

using FluentResults;
using Intercode.Toolbox.TypedPrimitives.IntegrationTests.Fixtures;

[TypedPrimitive<uint>]
public readonly partial struct UnvalidatedUInt32Primitive;

[TypedPrimitive( typeof( uint ), Converters = TypedPrimitiveConverter.All )]
public readonly partial struct UInt32Primitive
{
  #region Constants

  public const string ExpectedValidationErrorMessage = "Cannot be null or zero";

  #endregion

  #region Implementation

  static partial void ValidatePartial(
    uint? value,
    ref Result result )
  {
    result = Result.FailIf( value is null or 0, ExpectedValidationErrorMessage );
  }

  #endregion
}

public class UInt32PrimitiveTests
{
  #region Nested Types

  public class Comparable
    : ComparableTests<UInt32Primitive, Comparable>,
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
    : ComparableOfTTests<UInt32Primitive, ComparableOfT>,
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
    : ValuePrimitiveCreationTests<uint, UInt32Primitive, ValueFactory>;

  public new class Equals
    : EqualsTests<UInt32Primitive, Equals>,
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
    : GetHashCodeTests<UInt32Primitive, GetHashCode>,
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
    : NewtonsoftJsonTests<UInt32Primitive, PrimitiveFactory>;

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
    : SpanFormattableTests<UInt32Primitive, SpanFormattable>,
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
    : SpanParsableTests<UInt32Primitive, SpanParsable>,
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
    : SystemTextJsonTests<UInt32Primitive, PrimitiveFactory>;

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
    : UnvalidatedValuePrimitiveValidationTests<uint, UnvalidatedUInt32Primitive, ValueFactory>;

  public class Validation
    : ValuePrimitiveValidationTests<uint, UInt32Primitive, ValueFactory>;

  public class ValueConverter
    : ValueConverterTests<UInt32Primitive, UInt32PrimitiveValueConverter, PrimitiveFactory>;

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
      yield return [(uint) 0];
    }

    #endregion
  }

  #endregion

  #region Constants

  private static readonly uint s_valueA = 42;
  private static readonly uint s_valueB = 43;
  private static readonly UInt32Primitive s_primitiveA = UInt32Primitive.CreateOrThrow( s_valueA );
  private static readonly UInt32Primitive s_primitiveB = UInt32Primitive.CreateOrThrow( s_valueB );
  private static readonly UInt32Primitive s_defaultPrimitive = default;

  #endregion
}
