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
  NewtonsoftJson = 8,
  All = TypeConverter | SystemTextJson | EfCoreValueConverter | NewtonsoftJson
}

[AttributeUsage( AttributeTargets.Struct )]
public class TypedPrimitiveAttribute( Type primitiveType ): Attribute
{
  #region Properties

  public Type PrimitiveType { get; } = primitiveType;
  public TypedPrimitiveConverter Converters { get; set; }
  public StringComparison StringComparison { get; set; } = StringComparison.OrdinalIgnoreCase;

  #endregion
}

#if NET7_0_OR_GREATER || TP_USE_GENERIC_ATTRIBUTES

[AttributeUsage( AttributeTargets.Struct )]
public class TypedPrimitiveAttribute<T>(): TypedPrimitiveAttribute( typeof( T ) )
{
}

#endif

public interface IPrimitive
{
  bool IsDefault { get; }

#if NET7_0_OR_GREATER
  static abstract Type GetPrimitiveType();
#endif
}

public interface IPrimitive<out T>
  : IPrimitive
{
  T Value { get; }
}

public interface IValueTypePrimitive<TSelf, T>
  : IPrimitive<T>
  where TSelf: struct, IValueTypePrimitive<TSelf, T>
  where T: struct
{
  T? ValueOrDefault { get; }

#if NET7_0_OR_GREATER
  static abstract FluentResults.Result<TSelf> Create(
    T? value );

  static abstract TSelf CreateOrThrow(
    T? value );

  static abstract FluentResults.Result Validate(
    T? value );

  static abstract void ValidateOrThrow(
    T? value );

  static abstract bool IsValid(
    T? value );

  static abstract implicit operator T(
    TSelf primitive );

  static abstract explicit operator TSelf(
    T? value );

#endif
}

public interface IReferenceTypePrimitive<TSelf, T>
  : IPrimitive<T>
  where TSelf: struct, IReferenceTypePrimitive<TSelf, T>
  where T: class
{
  T? ValueOrDefault { get; }

#if NET7_0_OR_GREATER
  static abstract FluentResults.Result<TSelf> Create(
    T? value );

  static abstract TSelf CreateOrThrow(
    T? value );

  static abstract FluentResults.Result Validate(
    T? value );

  static abstract void ValidateOrThrow(
    T? value );

  static abstract bool IsValid(
    T? value );

  static abstract implicit operator T(
    TSelf primitive );

  static abstract explicit operator TSelf(
    T? value );

#endif
}
