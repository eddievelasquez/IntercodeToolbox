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
    PrimitiveTypeName = primitiveType.FullName!;
    IsValueType = primitiveType.IsValueType;
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

  public string PrimitiveTypeName { get; }
  public bool IsValueType { get; }
  public string TypeName { get; }
  public string Namespace { get; }
  public TypedPrimitiveConverter Converters { get; }
  public string? StringComparison { get; }
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
