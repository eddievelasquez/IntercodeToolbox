// Module Name: GeneratorModel.cs
// Author:      Eduardo Velasquez
// Copyright (c) 2024, Intercode Consulting, Inc.

namespace Intercode.Toolbox.TypedPrimitives;

// NOTE: Use record to help caching by ensuring immutability and IEquatable support.
internal readonly record struct GeneratorModel
{
  #region Constructors

  public GeneratorModel(
    Type primitiveType,
    string typeName,
    string @namespace,
    TypedPrimitiveConverter converters,
    string? stringComparison )
  {
    PrimitiveType = primitiveType;
    TypeName = typeName;
    Namespace = @namespace;
    Converters = converters;
    StringComparison = stringComparison;

    HasTypeConverter = HasConverter( TypedPrimitiveConverter.TypeConverter );
    HasSystemTextJsonConverter = HasConverter( TypedPrimitiveConverter.SystemTextJson );
    HasEfCoreConverter = HasConverter( TypedPrimitiveConverter.EfCoreValueConverter );
    HasNewtonsoftJsonConverter = HasConverter( TypedPrimitiveConverter.NewtonsoftJson );
  }

  #endregion

  #region Properties

  public Type PrimitiveType { get; init; }
  public string TypeName { get; init; }
  public string Namespace { get; init; }
  public TypedPrimitiveConverter Converters { get; init; }
  public string? StringComparison { get; init; }
  public bool HasTypeConverter { get; }
  public bool HasSystemTextJsonConverter { get; }
  public bool HasNewtonsoftJsonConverter { get; }
  public bool HasEfCoreConverter { get; }

  #endregion

  #region Public Methods

  public bool HasConverter(
    TypedPrimitiveConverter converter )
  {
    return Converters.HasFlag( converter );
  }

  #endregion
}
