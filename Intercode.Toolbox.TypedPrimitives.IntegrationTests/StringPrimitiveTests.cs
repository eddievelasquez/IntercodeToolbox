// Module Name: StringPrimitiveTests.cs
// Author:      Eduardo Velasquez
// Copyright (c) 2024, Intercode Consulting, Inc.

namespace Intercode.Toolbox.TypedPrimitives.IntegrationTests;

using FluentResults;
using Intercode.Toolbox.TypedPrimitives.IntegrationTests.Fixtures;
using Newtonsoft.Json;

[TypedPrimitive<string>]
public readonly partial struct UnvalidatedStringPrimitive;

[TypedPrimitive( typeof( string ), Converters = TypedPrimitiveConverter.All )]
public readonly partial struct StringPrimitive
{
  #region Constants

  public const string ExpectedValidationErrorMessage = "Cannot be null or empty";

  #endregion

  #region Implementation

  static partial void ValidatePartial(
    string? value,
    ref Result result )
  {
    result = Result.FailIf( string.IsNullOrWhiteSpace( value ), ExpectedValidationErrorMessage );
  }

  #endregion
}

public class StringPrimitiveTests
{
  #region Nested Types

  public class Comparable
    : ComparableTests<StringPrimitive, Comparable>,
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
    : ComparableOfTTests<StringPrimitive, ComparableOfT>,
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
    : ReferencePrimitiveCreationTests<string, StringPrimitive, ValueFactory>;

  public new class Equals
    : EqualsTests<StringPrimitive, Equals>,
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
    : GetHashCodeTests<StringPrimitive, GetHashCode>,
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
    : NewtonsoftJsonTests<StringPrimitive, PrimitiveFactory>
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

  public class SystemTextJson
    : SystemTextJsonTests<StringPrimitive, PrimitiveFactory>;

  public class ToStringTests
    : ToStringTests<StringPrimitive, ToStringTests>,
      ITestDataFactory
  {
    #region Public Methods

    public static IEnumerable<object?[]> GetValidValues()
    {
      yield return [s_primitiveA, s_valueA, null, null!];
    }

    public static IEnumerable<object?[]> GetInvalidValues()
    {
      yield break;
    }

    #endregion
  }

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
    : UnvalidatedReferencePrimitiveValidationTests<string, UnvalidatedStringPrimitive, ValueFactory>;

  public class Validation
    : ReferencePrimitiveValidationTests<string, StringPrimitive, ValueFactory>;

  public class ValueConverter
    : ValueConverterTests<StringPrimitive, StringPrimitiveValueConverter, ValueConverter>,
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
      yield return [string.Empty];
    }

    #endregion
  }

  #endregion

  #region Constants

  private static readonly string s_valueA = "valueA";
  private static readonly string s_valueB = "valueB";
  private static readonly StringPrimitive s_primitiveA = StringPrimitive.CreateOrThrow( s_valueA );
  private static readonly StringPrimitive s_primitiveB = StringPrimitive.CreateOrThrow( s_valueB );
  private static readonly StringPrimitive s_defaultPrimitive = default;

  #endregion
}
