// Module Name: TypeManager.cs
// Author:      Eduardo Velasquez
// Copyright (c) 2024, Intercode Consulting, Inc.

namespace Intercode.Toolbox.TypedPrimitives;

internal static class TypeManager
{
  #region Constants

  private static readonly Dictionary<Type, SupportedTypeInfo> s_supportedTypes = new ();

  #endregion

  #region Constructors

  static TypeManager()
  {
    Add( typeof( string ), "string", "String", "reader.GetString()", "writer.WriteStringValue( value.Value )" );
    Add( typeof( int ), "int", "Number", "reader.GetInt32()", "writer.WriteNumberValue( value.Value )" );
    Add(
      typeof( Guid ),
      "global::System.Guid",
      "String",
      "reader.GetGuid()",
      "writer.WriteStringValue( value.ToString() )"
    );
    Add( typeof( long ), "long", "Number", "reader.GetInt64()", "writer.WriteNumberValue( value.Value )" );
    Add(
      typeof( DateTime ),
      "global::System.DateTime",
      "String",
      "DateTime.Parse( reader.GetString()! )",
      "writer.WriteStringValue( value.ToString(\"O\") )"
    );
    Add(
      typeof( DateTimeOffset ),
      "global::System.DateTimeOffset",
      "String",
      "DateTimeOffset.Parse( reader.GetString()! )",
      "writer.WriteStringValue( value.ToString(\"O\") )"
    );
    return;

    static void Add(
      Type type,
      string keyword,
      string jsonTokenType,
      string jsonReader,
      string jsonWriter )
    {
      s_supportedTypes.Add( type, new SupportedTypeInfo( keyword, jsonTokenType, jsonReader, jsonWriter ) );
    }
  }

  #endregion

  #region Public Methods

  public static bool IsTypeSupported(
    Type type )
  {
    return s_supportedTypes.ContainsKey( type );
  }

  public static SupportedTypeInfo GetSupportedTypeInfo(
    Type type )
  {
    if( !s_supportedTypes.TryGetValue( type, out var info ) )
    {
      throw new InvalidOperationException( $"Type {type.FullName} is not supported." );
    }

    return info;
  }

  #endregion
}
