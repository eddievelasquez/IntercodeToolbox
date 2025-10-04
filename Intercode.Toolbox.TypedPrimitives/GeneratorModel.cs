// Module Name: GeneratorModel.cs
// Author:      Eduardo Velasquez
// Copyright (c) 2024, Intercode Consulting, Inc.

namespace Intercode.Toolbox.TypedPrimitives;

/// <summary>
///   Represents the model used for generating typed primitive converters and related artifacts.
///   Immutable and suitable for caching.
/// </summary>
/// <remarks>
///   This data is collected from the <see cref="TypedPrimitiveSourceGenerator.Initialize" /> method
///   and contains all the information that is necessary to generate the source code for a typed primitive
///   and its converters.
/// </remarks>
internal readonly record struct GeneratorModel
{
  #region Constructors

  /// <summary>
  ///   Initializes a new instance of the <see cref="GeneratorModel" /> struct.
  /// </summary>
  /// <param name="primitiveType">The underlying primitive <see cref="System.Type" />.</param>
  /// <param name="typeName">The name of the typed primitive.</param>
  /// <param name="namespace">The namespace of the typed primitive.</param>
  /// <param name="converters">The enabled converters as a <see cref="TypedPrimitiveConverter" /> flag enum.</param>
  /// <param name="stringComparison">The string comparison option, if applicable.</param>
  public GeneratorModel(
    Type primitiveType,
    string typeName,
    string @namespace,
    TypedPrimitiveConverter converters,
    string? stringComparison )
  {
    PrimitiveType = primitiveType;
    PrimitiveTypeName = primitiveType.FullName!; // FullName is never null for runtime types in this context
    IsValueType = primitiveType.IsValueType;
    TypeName = typeName;
    Namespace = @namespace;
    Converters = converters;
    StringComparison = stringComparison;

    // Initialize converter models for each supported converter type
    TypeConverter = new ConverterModel(
      TemplateType.TypeConverter,
      ResourceNames.TypeConverterTemplate,
      TypedPrimitiveConverter.TypeConverter,
      converters.HasFlag( TypedPrimitiveConverter.TypeConverter )
    );

    SystemTextJsonConverter = new ConverterModel(
      TemplateType.SystemTextJsonConverter,
      ResourceNames.SystemTextJsonConverterTemplate,
      TypedPrimitiveConverter.SystemTextJson,
      converters.HasFlag( TypedPrimitiveConverter.SystemTextJson )
    );

    NewtonsoftJsonConverter = new ConverterModel(
      TemplateType.NewtonsoftJsonConverter,
      ResourceNames.NewtonsoftJsonConverterTemplate,
      TypedPrimitiveConverter.NewtonsoftJson,
      converters.HasFlag( TypedPrimitiveConverter.NewtonsoftJson )
    );

    EfCoreValueConverter = new ConverterModel(
      TemplateType.EfCoreValueConverter,
      ResourceNames.EfCoreValueConverterTemplate,
      TypedPrimitiveConverter.EfCoreValueConverter,
      converters.HasFlag( TypedPrimitiveConverter.EfCoreValueConverter )
    );
  }

  #endregion

  #region Properties

  /// <summary>
  ///   Gets the runtime type of the primitive for which the typed primitive converters and related artifacts are generated.
  /// </summary>
  /// <remarks>
  ///   This property represents the actual <see cref="System.Type" /> of the primitive type being modeled.
  ///   It is used to determine characteristics such as whether the type is a value type and to generate
  ///   appropriate converters and other artifacts.
  /// </remarks>
  public Type PrimitiveType { get; }

  /// <summary>
  ///   Gets the full name of the underlying primitive type.
  /// </summary>
  public string PrimitiveTypeName { get; }

  /// <summary>
  ///   Gets a value indicating whether the underlying primitive type is a value type.
  /// </summary>
  public bool IsValueType { get; }

  /// <summary>
  ///   Gets the name of the typed primitive.
  /// </summary>
  public string TypeName { get; }

  /// <summary>
  ///   Gets the namespace of the typed primitive.
  /// </summary>
  public string Namespace { get; }

  /// <summary>
  ///   Gets the enabled converters as a <see cref="TypedPrimitiveConverter" /> flag enum.
  /// </summary>
  public TypedPrimitiveConverter Converters { get; }

  /// <summary>
  ///   Gets the string comparison option, if applicable.
  /// </summary>
  public string? StringComparison { get; }

  /// <summary>
  ///   Gets the model for the type converter.
  /// </summary>
  public ConverterModel TypeConverter { get; }

  /// <summary>
  ///   Gets the model for the System.Text.Json converter.
  /// </summary>
  public ConverterModel SystemTextJsonConverter { get; }

  /// <summary>
  ///   Gets the model for the Newtonsoft.Json converter.
  /// </summary>
  public ConverterModel NewtonsoftJsonConverter { get; }

  /// <summary>
  ///   Gets the model for the Entity Framework Core value converter.
  /// </summary>
  public ConverterModel EfCoreValueConverter { get; }

  #endregion

  #region Public Methods

  /// <summary>
  ///   Returns an enumeration of all enabled converter models for this typed primitive.
  /// </summary>
  /// <returns>An <see cref="IEnumerable{ConverterModel}" /> of enabled converters.</returns>
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

/// <summary>
///   Represents a single converter configuration for a typed primitive.
/// </summary>
/// <param name="Name">The template name for the converter.</param>
/// <param name="ConverterType">The converter type as a <see cref="TypedPrimitiveConverter" /> flag.</param>
/// <param name="IsEnabled">Indicates whether this converter is enabled for the primitive.</param>
internal record struct ConverterModel(
  TemplateType TemplateType,
  string Name,
  TypedPrimitiveConverter ConverterType,
  bool IsEnabled );
