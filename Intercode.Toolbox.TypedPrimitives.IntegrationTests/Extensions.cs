// Module Name: Extensions.cs
// Author:      Eduardo Velasquez
// Copyright (c) 2024, Intercode Consulting, Inc.

namespace Intercode.Toolbox.TypedPrimitives.IntegrationTests;

public static class Extensions
{
  #region Public Methods

  public static bool IsNumeric(
    this Type type )
  {
    return type == typeof( byte ) ||
           type == typeof( sbyte ) ||
           type == typeof( short ) ||
           type == typeof( ushort ) ||
           type == typeof( int ) ||
           type == typeof( uint ) ||
           type == typeof( long ) ||
           type == typeof( ulong ) ||
           type == typeof( float ) ||
           type == typeof( double ) ||
           type == typeof( decimal );
  }

  public static string ToStringForJson(
    this object? value )
  {
    if( value is null || value is IPrimitive { IsDefault: true } primitive )
    {
      return "null";
    }

    if( value.GetType().IsNumeric() )
    {
      return value.ToString()!;
    }

    return $"""
      "{value}"
      """;
  }

  #endregion
}
