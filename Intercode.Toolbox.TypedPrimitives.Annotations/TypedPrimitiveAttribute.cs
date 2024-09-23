namespace Intercode.Toolbox.TypedPrimitives;

using System;

public class TypedPrimitiveAttribute( Type primitiveType ): Attribute
{
  #region Properties

  public Type PrimitiveType { get; } = primitiveType;
  public TypedPrimitiveConverter Converters { get; set; }
  public Type? ValidatorType { get; set; }
  public Type? ValidatorFlagsType { get; set; }
  public object? ValidatorFlagsDefaultValue { get; set; }
  public object? StringComparison { get; set; }

  #endregion
}
