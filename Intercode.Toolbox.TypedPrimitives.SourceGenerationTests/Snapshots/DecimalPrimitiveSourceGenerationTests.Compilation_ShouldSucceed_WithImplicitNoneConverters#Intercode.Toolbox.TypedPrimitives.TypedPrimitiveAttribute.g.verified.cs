//HintName: Intercode.Toolbox.TypedPrimitives.TypedPrimitiveAttribute.g.cs
// Module Name: TypedPrimitiveAttribute.cs
// Author:      Eduardo Velasquez
// Copyright (c) 2024, Intercode Consulting, Inc.

#nullable enable

namespace Intercode.Toolbox.TypedPrimitives;

using System;

[Flags]
public enum TypedPrimitiveConverter
{
  None = 0,
  TypeConverter = 1,
  SystemTextJson = 2,
  EfCoreValueConverter = 4,
  NewtonsoftJson = 8
}

[AttributeUsage( AttributeTargets.Struct )]
public class TypedPrimitiveAttribute( Type primitiveType ): Attribute
{
  #region Properties

  public Type PrimitiveType { get; } = primitiveType;
  public TypedPrimitiveConverter Converters { get; set; }
  public object? StringComparison { get; set; }

  #endregion
}

#if NET7_0_OR_GREATER || TP_USE_GENERIC_ATTRIBUTES

[AttributeUsage( AttributeTargets.Struct )]
public class TypedPrimitiveAttribute<T>(): TypedPrimitiveAttribute( typeof( T ) )
{
}

#endif
