// Module Name: GeneratorModel.cs
// Author:      Eduardo Velasquez
// Copyright (c) 2024, Intercode Consulting, Inc.

namespace Intercode.Toolbox.TypedPrimitives;

// NOTE: Use record to help caching by ensuring immutability and IEquatable support.
internal readonly record struct GeneratorModel
{
  #region Constants

  private const string TYPE_CONVERTER_TEMPLATE_NAME = "TypeConverter";
  private const string SYSTEM_TEXT_JSON_CONVERTER_TEMPLATE_NAME = "SystemTextJsonConverter";
  private const string NEWTONSOFT_JSON_CONVERTER_TEMPLATE_NAME = "NewtonsoftJsonConverter";
  private const string EF_CORE_VALUE_CONVERTER_TEMPLATE_NAME = "EFCoreValueConverter";

  #endregion

  #region Constructors

  public GeneratorModel(
    Type primitiveType,
    string typeName,
    string @namespace,
    TypedPrimitiveConverter converters,
    string? stringComparison )
  {
    PrimitiveTypeName = primitiveType.FullName!;
    IsValueType = primitiveType.IsValueType;
    TypeName = typeName;
    Namespace = @namespace;
    Converters = converters;
    StringComparison = stringComparison;

    TypeConverter = new ConverterModel(
      TYPE_CONVERTER_TEMPLATE_NAME,
      converters.HasFlag( TypedPrimitiveConverter.TypeConverter )
    );

    SystemTextJsonConverter = new ConverterModel(
      SYSTEM_TEXT_JSON_CONVERTER_TEMPLATE_NAME,
      converters.HasFlag( TypedPrimitiveConverter.SystemTextJson )
    );

    NewtonsoftJsonConverter = new ConverterModel(
      NEWTONSOFT_JSON_CONVERTER_TEMPLATE_NAME,
      converters.HasFlag( TypedPrimitiveConverter.NewtonsoftJson )
    );

    EfCoreValueConverter = new ConverterModel(
      EF_CORE_VALUE_CONVERTER_TEMPLATE_NAME,
      converters.HasFlag( TypedPrimitiveConverter.EfCoreValueConverter )
    );
  }

  #endregion

  #region Properties

  public string PrimitiveTypeName { get; }
  public bool IsValueType { get; }
  public string TypeName { get; }
  public string Namespace { get; }
  public TypedPrimitiveConverter Converters { get; }
  public string? StringComparison { get; }
  public ConverterModel TypeConverter { get; }
  public ConverterModel SystemTextJsonConverter { get; }
  public ConverterModel NewtonsoftJsonConverter { get; }
  public ConverterModel EfCoreValueConverter { get; }

  #endregion

  #region Public Methods

  public IEnumerable<ConverterModel> GetEnabledConverters()
  {
    if( TypeConverter.IsEnabled )
    {
      yield return TypeConverter;
    }

    if( SystemTextJsonConverter.IsEnabled )
    {
      yield return SystemTextJsonConverter;
    }

    if( NewtonsoftJsonConverter.IsEnabled )
    {
      yield return NewtonsoftJsonConverter;
    }

    if( EfCoreValueConverter.IsEnabled )
    {
      yield return EfCoreValueConverter;
    }
  }

  #endregion
}

internal record struct ConverterModel(
  string Name,
  bool IsEnabled );
