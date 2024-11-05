// Module Name: Macros.cs
// Author:      Eduardo Velasquez
// Copyright (c) 2024, Intercode Consulting, Inc.

namespace Intercode.Toolbox.TypedPrimitives;

internal static class Macros
{
  #region Constants

  /// <summary>
  ///   The namespace of the generated type.
  /// </summary>
  public const string Namespace = nameof( Namespace );

  /// <summary>
  ///   The C# keyword or full type name of the primitive type.
  /// </summary>
  public const string TypeKeyword = nameof( TypeKeyword );

  /// <summary>
  ///   The name of the generated type.
  /// </summary>
  public const string TypeName = nameof( TypeName );

  /// <summary>
  ///   The fully qualified name of the generated type.
  /// </summary>
  public const string TypeQualifiedName = nameof( TypeQualifiedName );

  /// <summary>
  ///   The string comparison to use for string primitive operations.
  /// </summary>
  public const string StringComparison = nameof( StringComparison );

  /// <summary>
  ///   The System.Text.Json type of the token for the primitive type.
  /// </summary>
  public const string JsonTokenType = nameof( JsonTokenType );

  /// <summary>
  ///   The System.Text.Json Utf8JsonReader method to read the primitive type.
  /// </summary>
  public const string JsonReader = nameof( JsonReader );

  /// <summary>
  ///   The System.Text.Json Utf8JsonWriter method to write the primitive type.
  /// </summary>
  public const string JsonWriter = nameof( JsonWriter );

  /// <summary>
  ///   The Newtonsoft.Json type of the token for the primitive type.
  /// </summary>
  public const string NewtonsoftJsonTokenType = nameof( NewtonsoftJsonTokenType );

  /// <summary>
  ///   The Newtonsoft.Json method to read the primitive type.
  /// </summary>
  public const string NewtonsoftJsonReader = nameof( NewtonsoftJsonReader );

  /// <summary>
  ///   The Newtonsoft.Json method to write the primitive type.
  /// </summary>
  public const string NewtonsoftJsonWriter = nameof( NewtonsoftJsonWriter );

  /// <summary>
  ///   The attribute name for the type converter.
  /// </summary>
  public const string TypeConverterAttribute = nameof( TypeConverterAttribute );

  /// <summary>
  ///   The attribute name for the System.Text.Json converter.
  /// </summary>
  public const string SystemTextJsonConverterAttribute = nameof( SystemTextJsonConverterAttribute );

  /// <summary>
  ///   The attribute name for the Newtonsoft.Json converter.
  /// </summary>
  public const string NewtonsoftJsonConverterAttribute = nameof( NewtonsoftJsonConverterAttribute );

  #endregion
}
