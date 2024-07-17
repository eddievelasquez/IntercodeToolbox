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
    return value is sbyte or byte or short or ushort or int or uint or long or ulong or float or double or decimal;
  }

  #endregion
}
