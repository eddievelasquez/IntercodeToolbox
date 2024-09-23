// Module Name: GeneratorModel.cs
// Author:      Eduardo Velasquez
// Copyright (c) 2024, Intercode Consulting, Inc.

namespace Intercode.Toolbox.TypedPrimitives;

internal enum ValidationType
{
  Unvalidated,
  Validated,
  ValidatedWithFlags
}

// NOTE: Use record to help caching by ensuring immutability and IEquatable support.
internal readonly record struct GeneratorModel(
  Type PrimitiveType,
  string Name,
  string Namespace,
  TypedPrimitiveConverter Converters,
  string? ValidatorTypeName,
  string? ValidatorFlagsTypeName,
  string? ValidatorFlagsDefaultValue,
  string? StringComparison )
{
  #region Properties

  public ValidationType ValidationType { get; } = ValidatorTypeName == null
    ? ValidationType.Unvalidated
    : ValidatorFlagsTypeName == null
      ? ValidationType.Validated
      : ValidationType.ValidatedWithFlags;

  #endregion

  #region Public Methods

  public bool HasConverter(
    TypedPrimitiveConverter converter )
  {
    return Converters.HasFlag( converter );
  }

  #endregion
}
