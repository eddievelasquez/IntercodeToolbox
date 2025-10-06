// Module Name: TypeManager.cs
// Author:      Eduardo Velasquez
// Copyright (c) 2025, Intercode Consulting, Inc.

namespace Intercode.Toolbox.TypedPrimitives;

using System.Collections.Frozen;
using System.Diagnostics.CodeAnalysis;

/// <summary>
///   Provides metadata and macro/include configuration for supported primitive types and their converters.
/// </summary>
/// <remarks>
///   This static class initializes and exposes a registry of supported primitive types, including their JSON and type
///   converter macro templates.
/// </remarks>
internal static class TypeManager
{
  #region Nested Types

  /// <summary>
  ///   Encapsulates JSON serialization parameters for a primitive type and converter.
  /// </summary>
  /// <param name="TokenType">The JSON token type (e.g., "Number", "String").</param>
  /// <param name="Reader">The code snippet to read the value from a JSON reader.</param>
  /// <param name="Writer">The code snippet to write the value to a JSON writer.</param>
  private readonly record struct JsonParameters(
    string TokenType,
    string Reader,
    string Writer );

  #endregion

  #region Constants

  // Attribute macro templates for each converter type
  private const string TYPE_CONVERTER_ATTRIBUTE_TEMPLATE =
    $"[global::System.ComponentModel.TypeConverter( typeof( ${MacroNames.TypedPrimitiveQualifiedName}$TypeConverter ) )]\n";

  private const string SYSTEM_TEXT_JSON_CONVERTER_ATTRIBUTE_TEMPLATE =
    $"[global::System.Text.Json.Serialization.JsonConverter( typeof( ${MacroNames.TypedPrimitiveQualifiedName}$SystemTextJsonConverter ) )]\n";

  private const string NEWTONSOFT_JSON_CONVERTER_ATTRIBUTE_TEMPLATE =
    $"[global::Newtonsoft.Json.JsonConverter( typeof( ${MacroNames.TypedPrimitiveQualifiedName}$NewtonsoftJsonConverter ) )]\n";

  // Registry of supported types
  private static readonly FrozenDictionary<Type, SupportedTypeInfo> s_supportedTypes;

  #endregion

  #region Constructors

  /// <summary>
  ///   Static constructor. Initializes the supported types registry with macro and include data for each primitive type.
  /// </summary>
  static TypeManager()
  {
    var supportedTypes = new Dictionary<Type, SupportedTypeInfo>();

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

    s_supportedTypes = supportedTypes.ToFrozenDictionary();

    return;

    void Add<T>(
      JsonParameters stj,
      JsonParameters nsj )
    {
      var typeInfo = new SupportedTypeInfoBuilder( typeof( T ) )
                     .AddConverterCustomMacros(
                       TypedPrimitiveConverter.SystemTextJson,
                       ( MacroNames.SystemTextJsonTokenType, stj.TokenType ),
                       ( MacroNames.SystemTextJsonReader, stj.Reader ),
                       ( MacroNames.SystemTextJsonWriter, stj.Writer )
                     )
                     .AddIncludes(
                       TypedPrimitiveConverter.SystemTextJson,
                       ( MacroNames.SystemTextJsonConverterAttribute, SYSTEM_TEXT_JSON_CONVERTER_ATTRIBUTE_TEMPLATE )
                     )
                     .AddConverterCustomMacros(
                       TypedPrimitiveConverter.NewtonsoftJson,
                       ( MacroNames.NewtonsoftJsonTokenType, nsj.TokenType ),
                       ( MacroNames.NewtonsoftJsonReader, nsj.Reader ),
                       ( MacroNames.NewtonsoftJsonWriter, nsj.Writer )
                     )
                     .AddIncludes(
                       TypedPrimitiveConverter.NewtonsoftJson,
                       ( MacroNames.NewtonsoftJsonConverterAttribute, NEWTONSOFT_JSON_CONVERTER_ATTRIBUTE_TEMPLATE )
                     )
                     .AddIncludes(
                       TypedPrimitiveConverter.TypeConverter,
                       ( MacroNames.TypeConverterAttribute, TYPE_CONVERTER_ATTRIBUTE_TEMPLATE )
                     )
                     .Build();

      supportedTypes.Add( typeof( T ), typeInfo );
    }
  }

  #endregion

  #region Public Methods

  /// <summary>
  ///   Attempts to retrieve the <see cref="SupportedTypeInfo" /> associated with the specified type.
  /// </summary>
  /// <param name="type">The type for which to retrieve the supported type information.</param>
  /// <param name="info">
  ///   When this method returns, contains the <see cref="SupportedTypeInfo" /> associated with the specified type if it is
  ///   supported;
  ///   otherwise, <c>null</c>.
  /// </param>
  /// <returns>
  ///   <c>true</c> if the specified type is supported and <paramref name="info" /> contains the associated
  ///   <see cref="SupportedTypeInfo" />; otherwise, <c>false</c>.
  /// </returns>
  public static bool TryGetSupportedTypeInfo(
    Type type,
    [NotNullWhen( true )] out SupportedTypeInfo? info )
  {
    return s_supportedTypes.TryGetValue( type, out info );
  }

  /// <summary>
  ///   Determines whether the specified type is supported by the <see cref="TypeManager" />.
  /// </summary>
  /// <param name="type">The type to check for support.</param>
  /// <returns>
  ///   <c>true</c> if the specified type is supported; otherwise, <c>false</c>.
  /// </returns>
  public static bool IsSupported(
    Type type )
  {
    return s_supportedTypes.ContainsKey( type );
  }

  #endregion
}
