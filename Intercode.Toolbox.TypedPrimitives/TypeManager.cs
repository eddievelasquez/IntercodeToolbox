// Module Name: TypeManager.cs
// Author:      Eduardo Velasquez
// Copyright (c) 2024, Intercode Consulting, Inc.

namespace Intercode.Toolbox.TypedPrimitives;

using System.Diagnostics.CodeAnalysis;

internal static class TypeManager
{
  #region Nested Types

  private readonly struct JsonParameters(
    string tokenType,
    string reader,
    string writer )
  {
    #region Properties

    public string TokenType { get; } = tokenType;
    public string Reader { get; } = reader;
    public string Writer { get; } = writer;

    #endregion
  }

  #endregion

  #region Constants

  private const string TYPE_CONVERTER_ATTRIBUTE_TEMPLATE =
    $"[global::System.ComponentModel.TypeConverter( typeof( ${Macros.TypeQualifiedName}$TypeConverter ) )]\n";

  private const string SYSTEM_TEXT_JSON_CONVERTER_ATTRIBUTE_TEMPLATE =
    $"[global::System.Text.Json.Serialization.JsonConverter( typeof( ${Macros.TypeQualifiedName}$SystemTextJsonConverter ) )]\n";

  private const string NEWTONSOFT_JSON_CONVERTER_ATTRIBUTE_TEMPLATE =
    $"[global::Newtonsoft.Json.JsonConverter( typeof( ${Macros.TypeQualifiedName}$NewtonsoftJsonConverter ) )]\n";

  private static readonly Dictionary<string, SupportedTypeInfo> s_supportedTypes = new ( StringComparer.Ordinal );

  #endregion

  #region Constructors

  static TypeManager()
  {
    // @formatter:off
    Add<byte>(
      new JsonParameters( "Number", "reader.GetByte()", "writer.WriteNumberValue( value.Value )" ),
      new JsonParameters( "Integer", "global::System.Convert.ToByte( reader.Value! )", "writer.WriteValue( s.Value )" )
    );

    Add<sbyte>(
      new JsonParameters( "Number", "reader.GetSByte()", "writer.WriteNumberValue( value.Value )" ),
      new JsonParameters( "Integer", "global::System.Convert.ToSByte( reader.Value! )", "writer.WriteValue( s.Value )" )
    );

    Add<short>(
      new JsonParameters( "Number", "reader.GetInt16()", "writer.WriteNumberValue( value.Value )" ),
      new JsonParameters( "Integer", "global::System.Convert.ToInt16( reader.Value! )", "writer.WriteValue( s.Value )" )
    );

    Add<ushort>(
      new JsonParameters( "Number", "reader.GetUInt16()", "writer.WriteNumberValue( value.Value )" ),
      new JsonParameters( "Integer", "global::System.Convert.ToUInt16( reader.Value! )", "writer.WriteValue( s.Value )" )
    );

    Add<int>(
      new JsonParameters( "Number", "reader.GetInt32()", "writer.WriteNumberValue( value.Value )" ),
      new JsonParameters( "Integer", "global::System.Convert.ToInt32( reader.Value! )", "writer.WriteValue( s.Value )" )
    );

    Add<uint>(
      new JsonParameters( "Number", "reader.GetUInt32()", "writer.WriteNumberValue( value.Value )" ),
      new JsonParameters( "Integer", "global::System.Convert.ToUInt32( reader.Value! )", "writer.WriteValue( s.Value )" )
    );

    Add<long>(
      new JsonParameters( "Number", "reader.GetInt64()", "writer.WriteNumberValue( value.Value )" ),
      new JsonParameters( "Integer", "global::System.Convert.ToInt64( reader.Value! )", "writer.WriteValue( s.Value )" )
    );

    Add<ulong>(
      new JsonParameters( "Number", "reader.GetUInt64()", "writer.WriteNumberValue( value.Value )" ),
      new JsonParameters( "Integer", "global::System.Convert.ToUInt64( reader.Value! )", "writer.WriteValue( s.Value )" )
    );

    Add<float>(
      new JsonParameters( "Number", "reader.GetSingle()", "writer.WriteNumberValue( value.Value )" ),
      new JsonParameters( "Float", "global::System.Convert.ToSingle( reader.Value! )", "writer.WriteValue( s.Value )" )
    );

    Add<double>(
      new JsonParameters( "Number", "reader.GetDouble()", "writer.WriteNumberValue( value.Value )" ),
      new JsonParameters( "Float", "global::System.Convert.ToDouble( reader.Value! )", "writer.WriteValue( s.Value )" )
    );

    Add<decimal>(
      new JsonParameters( "Number", "reader.GetDecimal()", "writer.WriteNumberValue( value.Value )" ),
      new JsonParameters( "Float", "global::System.Convert.ToDecimal( reader.Value! )", "writer.WriteValue( s.Value )" )
    );

    Add<string>(
      new JsonParameters( "String", "reader.GetString()", "writer.WriteStringValue( value.Value )" ),
      new JsonParameters( "String", "( string ) reader.Value!", "writer.WriteValue( s.Value )" )
    );

    Add<Guid>(
      new JsonParameters( "String", "reader.GetGuid()", "writer.WriteStringValue( value.ToString() )" ),
      new JsonParameters( "String", "global::System.Guid.Parse( ( string ) reader.Value! )", "writer.WriteValue( s.Value )" )
    );

    Add<DateTime>(
      new JsonParameters( "String", "DateTime.Parse( reader.GetString()! )", "writer.WriteStringValue( value.ToString(\"O\") )" ),
      new JsonParameters( "String", "global::System.DateTime.Parse( ( string ) reader.Value! )", "writer.WriteValue( s.Value.ToString( \"O\" ) )" )
    );

    Add<DateTimeOffset>(
      new JsonParameters( "String", "DateTimeOffset.Parse( reader.GetString()! )", "writer.WriteStringValue( value.ToString(\"O\") )" ),
      new JsonParameters( "String", "global::System.DateTimeOffset.Parse( ( string ) reader.Value! )", "writer.WriteValue( s.Value.ToString( \"O\" ) )" )
    );

    Add<TimeSpan>(
      new JsonParameters( "String", "TimeSpan.Parse( reader.GetString()! )", "writer.WriteStringValue( value.ToString(\"c\") )" ),
      new JsonParameters( "String", "global::System.TimeSpan.ParseExact( (string) reader.Value!, \"c\", null )", "writer.WriteValue( s.Value.ToString( \"c\" ) )" )
    );

    Add<Uri>(
      new JsonParameters( "String", "new Uri( reader.GetString()! )", "writer.WriteStringValue( value.ToString() )" ),
      new JsonParameters( "String", "new Uri( ( string ) reader.Value! )", "writer.WriteValue( s.Value )" )
    );

    // @formatter:on

    return;

    static void Add<T>(
      JsonParameters systemTextJson,
      JsonParameters newtonsoftJson )
    {
      // @formatter:off
      var typeInfo = new SupportedTypeInfoBuilder( typeof( T ) ).AddConverterCustomMacros( TypedPrimitiveConverter.SystemTextJson,
                                                                  AddMacro( Macros.JsonTokenType, systemTextJson.TokenType ),
                                                                  AddMacro( Macros.JsonReader, systemTextJson.Reader ),
                                                                  AddMacro( Macros.JsonWriter, systemTextJson.Writer ),
                                                                  AddMacro(
                                                                    Macros.SystemTextJsonConverterAttribute,
                                                                    SYSTEM_TEXT_JSON_CONVERTER_ATTRIBUTE_TEMPLATE
                                                                  )
                                                                )
                                                                .AddConverterCustomMacros( TypedPrimitiveConverter.NewtonsoftJson,
                                                                  AddMacro( Macros.NewtonsoftJsonTokenType, newtonsoftJson.TokenType ),
                                                                  AddMacro( Macros.NewtonsoftJsonReader, newtonsoftJson.Reader ),
                                                                  AddMacro( Macros.NewtonsoftJsonWriter, newtonsoftJson.Writer ),
                                                                  AddMacro( Macros.NewtonsoftJsonConverterAttribute, NEWTONSOFT_JSON_CONVERTER_ATTRIBUTE_TEMPLATE )
                                                                )
                                                                .AddConverterCustomMacros( TypedPrimitiveConverter.TypeConverter,
                                                                  AddMacro( Macros.TypeConverterAttribute, TYPE_CONVERTER_ATTRIBUTE_TEMPLATE )
                                                                )
                                                                .Build();
      // @formatter:on
      s_supportedTypes.Add( typeof( T ).FullName, typeInfo );
      return;

      static KeyValuePair<string, string> AddMacro(
        string name,
        string value )
      {
        return new KeyValuePair<string, string>( name, value );
      }
    }
  }

  #endregion

  #region Public Methods

  public static bool TryGetSupportedTypeInfo(
    string typeName,
    [NotNullWhen( true )] out SupportedTypeInfo? info )
  {
    return s_supportedTypes.TryGetValue( typeName, out info );
  }

  #endregion
}
