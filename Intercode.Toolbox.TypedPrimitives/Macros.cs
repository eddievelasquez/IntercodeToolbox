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
  ///   The name of the generated type.
  /// </summary>
  public const string Name = nameof( Name );

  /// <summary>
  ///   The fully qualified name of the generated type.
  /// </summary>
  public const string FullName = nameof( FullName );

  /// <summary>
  ///   List of attributes to apply to the generated type depending on the Converters values.
  /// </summary>
  public const string Attributes = nameof( Attributes );

  /// <summary>
  ///   List of factory methods to create the type depending on the presence of the ValidatorType.
  ///   Will include the contents of one of the following templates:
  ///   - Factory_Unvalidated: ValidatorType is null.
  ///   - Factory_Validated: ValidatorType is null and ValidatorFlagsType is null.
  ///   - Factory_ValidatedWithFlags: ValidatorType is not null and ValidatorFlagsType is not null.
  /// </summary>
  public const string Factory = nameof( Factory );

  /// <summary>
  ///   Conversion operators to use depending on the presence of a ValidatorType.
  /// </summary>
  public const string Operators = nameof( Operators );

  /// <summary>
  ///   The combination of zero or more of the TypeConverter, SystemTextJsonConverter, and EFCoreValueConverter
  ///   according to the values of the Converters property.
  /// </summary>
  public const string Converters = nameof( Converters );

  /// <summary>
  ///   The type of the class that contains a static validation method named "Validate" that returns a FluentResults.Result
  ///   and takes a PrimitiveType as the first parameter.
  ///   If the ValidatorFlagsType is not null, the method should also take a parameter of that type.
  /// </summary>
  public const string ValidatorType = nameof( ValidatorType );

  /// <summary>
  ///   The string comparison to use for string operations.
  /// </summary>
  public const string StringComparison = nameof( StringComparison );

  /// <summary>
  ///   The C# keyword or full type name of the primitive type.
  /// </summary>
  public const string TypeKeyword = nameof( TypeKeyword );

  /// <summary>
  ///   The name of the primitive type.
  /// </summary>
  public const string TypeName = nameof( TypeName );

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

  public const string TypeConverterAttribute = nameof( TypeConverterAttribute );
  public const string SystemTextJsonConverterAttribute = nameof( SystemTextJsonConverterAttribute );
  public const string NewtonsoftJsonConverterAttribute = nameof( NewtonsoftJsonConverterAttribute );

  #endregion
}
