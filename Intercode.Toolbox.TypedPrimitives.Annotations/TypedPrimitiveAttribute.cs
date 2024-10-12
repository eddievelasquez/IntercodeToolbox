// Module Name: TypedPrimitiveAttribute.cs
// Author:      Eduardo Velasquez
// Copyright (c) 2024, Intercode Consulting, Inc.

namespace Intercode.Toolbox.TypedPrimitives;

using System;

[AttributeUsage( AttributeTargets.Struct )]
public class TypedPrimitiveAttribute( Type primitiveType ): Attribute
{
  #region Properties

  public Type PrimitiveType { get; } = primitiveType;
  public TypedPrimitiveConverter Converters { get; set; }
  public object? StringComparison { get; set; }

  #endregion
}

[AttributeUsage( AttributeTargets.Struct )]
public class TypedPrimitiveAttribute<T>(): TypedPrimitiveAttribute( typeof( T ) )
{
}
