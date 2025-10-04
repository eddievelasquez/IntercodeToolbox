// Module Name: SupportedTypeInfoBuilder.cs
// Author:      Eduardo Velasquez
// Copyright (c) 2024, Intercode Consulting, Inc.

namespace Intercode.Toolbox.TypedPrimitives;

using System.Collections.Frozen;

/// <summary>
///   Provides a builder for constructing <see cref="SupportedTypeInfo" /> instances with custom macros and includes for
///   typed primitive converters.
/// </summary>
/// <remarks>
///   This builder is used to fluently configure macros and includes for each supported converter type, and to specify the
///   C# keyword for the primitive type.
/// </remarks>
internal class SupportedTypeInfoBuilder
{
  #region Fields

  // The full name of the primitive type being described.
  private readonly string _typeName;

  // Stores macros for each converter type.
  private readonly Dictionary<TypedPrimitiveConverter, Dictionary<string, string>> _converterMacros = new ();

  // Stores includes for each converter type.
  private readonly Dictionary<TypedPrimitiveConverter, Dictionary<string, string>> _includes = new ();

  // The C# keyword for the primitive type, if explicitly set.
  private string? _typeKeyword;

  #endregion

  #region Constructors

  /// <summary>
  ///   Initializes a new instance of the <see cref="SupportedTypeInfoBuilder" /> class for the specified primitive type.
  /// </summary>
  /// <param name="type">The primitive <see cref="System.Type" /> to describe.</param>
  /// <exception cref="ArgumentNullException">Thrown if <paramref name="type" />.FullName is null.</exception>
  public SupportedTypeInfoBuilder(
    Type type )
  {
    _typeName = type.FullName ?? throw new ArgumentNullException( nameof( type ) );
  }

  #endregion

  #region Public Methods

  /// <summary>
  ///   Sets the C# keyword for the primitive type (e.g., <c>int</c>, <c>string</c>).
  /// </summary>
  /// <param name="keyword">The C# keyword to use.</param>
  /// <returns>The current <see cref="SupportedTypeInfoBuilder" /> instance for chaining.</returns>
  public SupportedTypeInfoBuilder AddTypeKeyword(
    string keyword )
  {
    _typeKeyword = keyword;
    return this;
  }

  /// <summary>
  ///   Adds custom macro macroName-value tuples for a specific converter type.
  /// </summary>
  /// <param name="converter">The converter type to associate macros with.</param>
  /// <param name="macros">
  ///   A collection of macroName-value tuples, where each pair consists of a macro name and its corresponding value.
  /// </param>
  /// <returns>The current <see cref="SupportedTypeInfoBuilder" /> instance for method chaining.</returns>
  /// <exception cref="ArgumentException">
  ///   Thrown if a macro with the same name already exists for the specified converter.
  /// </exception>
  public SupportedTypeInfoBuilder AddConverterCustomMacros(
    TypedPrimitiveConverter converter,
    params (string macroName, string value)[] macros )
  {
    if( !_converterMacros.TryGetValue( converter, out var converterMacros ) )
    {
      converterMacros = new Dictionary<string, string>();
      _converterMacros.Add( converter, converterMacros );
    }

    foreach( var (macroName, value) in macros )
    {
      converterMacros.Add( macroName, value );
    }

    return this;
  }

  /// <summary>
  ///   Adds include name-value tuples for a specific converter type.
  /// </summary>
  /// <param name="converter">
  ///   The <see cref="TypedPrimitiveConverter" /> representing the converter type to associate the includes with.
  /// </param>
  /// <param name="includes">
  ///   A collection of name-value tuples, where each name and content of an include.
  /// </param>
  /// <returns>
  ///   The current <see cref="SupportedTypeInfoBuilder" /> instance, allowing for method chaining.
  /// </returns>
  /// <exception cref="ArgumentException">
  ///   Thrown if duplicate include names are provided for the same converter.
  /// </exception>
  public SupportedTypeInfoBuilder AddIncludes(
    TypedPrimitiveConverter converter,
    params (string name, string)[] includes )
  {
    if( !_includes.TryGetValue( converter, out var converterIncludes ) )
    {
      converterIncludes = new Dictionary<string, string>();
      _includes.Add( converter, converterIncludes );
    }

    foreach( var (name, value) in includes )
    {
      converterIncludes.Add( name, value );
    }

    return this;
  }

  /// <summary>
  ///   Builds a <see cref="SupportedTypeInfo" /> instance using the configured macros, includes, and type keyword.
  /// </summary>
  /// <returns>A new <see cref="SupportedTypeInfo" /> instance.</returns>
  public SupportedTypeInfo Build()
  {
    var keyword = _typeKeyword ?? GetKeyword( _typeName );

    var customMacros = _converterMacros.ToFrozenDictionary( kvp => kvp.Key, pair => pair.Value.ToFrozenDictionary() );
    var includes = _includes.ToFrozenDictionary( kvp => kvp.Key, pair => pair.Value.ToFrozenDictionary() );

    return new SupportedTypeInfo( keyword, customMacros, includes );
  }

  #endregion

  #region Implementation

  /// <summary>
  ///   Gets the C# keyword for a given type name, or returns a global-qualified type name if not a known primitive.
  /// </summary>
  /// <param name="typeName">The full type name.</param>
  /// <returns>The C# keyword or global-qualified type name.</returns>
  private static string GetKeyword(
    string typeName )
  {
    return typeName switch
    {
      "System.Boolean" => "bool",
      "System.Byte"    => "byte",
      "System.Char"    => "char",
      "System.Decimal" => "decimal",
      "System.Double"  => "double",
      "System.Int16"   => "short",
      "System.Int32"   => "int",
      "System.Int64"   => "long",
      "System.SByte"   => "sbyte",
      "System.Single"  => "float",
      "System.String"  => "string",
      "System.UInt16"  => "ushort",
      "System.UInt32"  => "uint",
      "System.UInt64"  => "ulong",
      _                => $"global::{typeName}"
    };
  }

  #endregion
}
