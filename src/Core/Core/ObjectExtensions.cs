// Module Name: ObjectExtensions.cs
// Author:      Eduardo Velasquez
// Copyright (c) 2024, Intercode Consulting, Inc.

namespace Intercode.Toolbox.Core;

/// <summary>
///   Provides extension methods for the <see cref="object" /> class.
/// </summary>
public static class ObjectExtensions
{
  #region Public Methods

  /// <summary>
  ///   Determines whether the specified value is a number.
  /// </summary>
  /// <param name="value">The value to check.</param>
  /// <returns><c>true</c> if the value is a number; otherwise, <c>false</c>.</returns>
  public static bool IsNumber(
    this object value )
  {
    return value.GetType().IsNumeric();
  }

  /// <summary>
  ///   Determines whether the specified type is numeric.
  /// </summary>
  /// <param name="type">The type to check.</param>
  /// <returns><c>true</c> if the type is numeric; otherwise, <c>false</c>.</returns>
  public static bool IsNumeric(
    this Type type )
  {
    return Type.GetTypeCode( type ) switch
    {
      TypeCode.Byte
        or TypeCode.SByte
        or TypeCode.Int16
        or TypeCode.UInt16
        or TypeCode.Int32
        or TypeCode.UInt32
        or TypeCode.Int64
        or TypeCode.UInt64
        or TypeCode.Single
        or TypeCode.Double
        or TypeCode.Decimal => true,
      _ => false
    };
  }

  #endregion
}
