// Module Name: GeneratorModel.cs
// Author:      Eduardo Velasquez
// Copyright (c) 2024, Intercode Consulting, Inc.

namespace Intercode.Toolbox.TypedPrimitives;

// NOTE: Use record to help caching by ensuring immutability and IEquatable support.
internal readonly record struct GeneratorModel(
  Type PrimitiveType,
  string Name,
  string Namespace,
  TypedPrimitiveConverter Converters,
  string? StringComparison )
{
  #region Public Methods

  public bool HasConverter(
    TypedPrimitiveConverter converter )
  {
    return Converters.HasFlag( converter );
  }

  #endregion
}
