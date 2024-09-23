// Module Name: Extensions.cs
// Author:      Eduardo Velasquez
// Copyright (c) 2024, Intercode Consulting, Inc.

namespace Intercode.Toolbox.TypedPrimitives;

using System.Diagnostics;
using System.Text;

internal static class Extensions
{
  #region Public Methods

  public static string ToCSharpKeyword(
    this Type type )
  {
    Debug.Assert( type != null );

    return type switch
    {
      not null when type == typeof( bool )    => "bool",
      not null when type == typeof( byte )    => "byte",
      not null when type == typeof( char )    => "char",
      not null when type == typeof( decimal ) => "decimal",
      not null when type == typeof( double )  => "double",
      not null when type == typeof( float )   => "float",
      not null when type == typeof( int )     => "int",
      not null when type == typeof( long )    => "long",
      not null when type == typeof( object )  => "object",
      not null when type == typeof( sbyte )   => "sbyte",
      not null when type == typeof( short )   => "short",
      not null when type == typeof( string )  => "string",
      not null when type == typeof( uint )    => "uint",
      not null when type == typeof( ulong )   => "ulong",
      not null when type == typeof( ushort )  => "ushort",
      _                                       => type!.FullName!
    };
  }

  public static string AsString(
    this IEnumerable<string> values,
    string? separator = null,
    char? quoteChar = null )
  {
    separator ??= ", ";

    var buffer = new StringBuilder();
    var needsSep = false;

    foreach( var value in values )
    {
      if( needsSep )
      {
        buffer.Append( separator );
      }
      else
      {
        needsSep = true;
      }

      if( quoteChar != null )
      {
        buffer.Append( quoteChar );
      }

      buffer.Append( value );

      if( quoteChar != null )
      {
        buffer.Append( quoteChar );
      }
    }

    return buffer.ToString();
  }

  #endregion
}
