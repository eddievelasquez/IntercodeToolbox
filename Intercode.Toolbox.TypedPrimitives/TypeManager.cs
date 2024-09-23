// Module Name: TypeManager.cs
// Author:      Eduardo Velasquez
// Copyright (c) 2024, Intercode Consulting, Inc.

namespace Intercode.Toolbox.TypedPrimitives;

internal static class TypeManager
{
  #region Constants

  private static readonly Dictionary<Type, SupportedTypeInfo> SupportedTypes = new ()
  {
    {
      typeof( string ),
      new SupportedTypeInfo( "string", "String", "String", "reader.GetString()", "writer.WriteStringValue( value.Value )" )
    },
    {
      typeof( int ),
      new SupportedTypeInfo( "int", "Int32", "Number", "reader.GetInt32()", "writer.WriteNumberValue( value.Value )" )
    },
    {
      typeof( Guid ),
      new SupportedTypeInfo(
        "global::System.Guid",
        "Guid",
        "String",
        "reader.GetGuid()",
        "writer.WriteStringValue( value.ToString() )"
      )
    },
    {
      typeof( long ),
      new SupportedTypeInfo( "long", "Int64", "Number", "reader.GetInt64()", "writer.WriteNumberValue( value.Value )" )
    },
    {
      typeof( DateTime ),
      new SupportedTypeInfo(
        "global::System.DateTime",
        "DateTime",
        "String",
        "DateTime.Parse( reader.GetString()! )",
        "writer.WriteStringValue( value.ToString(\"O\") )"
      )
    },
    {
      typeof( DateTimeOffset ),
      new SupportedTypeInfo(
        "global::System.DateTimeOffset",
        "DateTimeOffset",
        "String",
        "DateTimeOffset.Parse( reader.GetString()! )",
        "writer.WriteStringValue( value.ToString(\"O\") )"
      )
    }
  };

  #endregion

  #region Public Methods

  public static bool IsTypeSupported(
    Type type )
  {
    return SupportedTypes.ContainsKey( type );
  }

  public static SupportedTypeInfo GetSupportedTypeInfo(
    Type type )
  {
    if( !SupportedTypes.TryGetValue( type, out var info ) )
    {
      throw new InvalidOperationException( $"Type {type.FullName} is not supported." );
    }

    return info;
  }

  #endregion
}
