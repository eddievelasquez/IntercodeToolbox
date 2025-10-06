// Module Name: SupportedTypeInfo.cs
// Author:      Eduardo Velasquez
// Copyright (c) 2024, Intercode Consulting, Inc.

namespace Intercode.Toolbox.TypedPrimitives;

using System.Collections.Frozen;

/// <summary>
///   Represents metadata and template macro/include configuration for a supported primitive type.
/// </summary>
/// <remarks>
///   Used by the typed primitives source generator to provide macro and include data for each converter type.
/// </remarks>
internal class SupportedTypeInfo
{
  #region Fields

  // Maps each converter type to its macro dictionary (macro name → macro value)
  private readonly FrozenDictionary<TypedPrimitiveConverter, FrozenDictionary<string, string>> _customConverterMacros;

  // Maps each converter type to its include dictionary (include name → include value)
  private readonly FrozenDictionary<TypedPrimitiveConverter, FrozenDictionary<string, string>> _includes;

  #endregion

  #region Constructors

  /// <summary>
  ///   Initializes a new instance of the <see cref="SupportedTypeInfo" /> class.
  /// </summary>
  /// <param name="keyword">The C# keyword representing the primitive type (e.g., <c>int</c>, <c>string</c>).</param>
  /// <param name="customConverterMacros">A frozen dictionary mapping converter types to their macro dictionaries.</param>
  /// <param name="includes">A frozen dictionary mapping converter types to their include dictionaries.</param>
  public SupportedTypeInfo(
    string keyword,
    FrozenDictionary<TypedPrimitiveConverter, FrozenDictionary<string, string>> customConverterMacros,
    FrozenDictionary<TypedPrimitiveConverter, FrozenDictionary<string, string>> includes )
  {
    if( string.IsNullOrWhiteSpace( keyword ) )
    {
      throw new ArgumentException( "Cannot be null, empty or blank", nameof( keyword ) );
    }

    _customConverterMacros = customConverterMacros ?? throw new ArgumentNullException( nameof( customConverterMacros ) );
    _includes = includes ?? throw new ArgumentNullException( nameof( includes ) );
    Keyword = keyword;
  }

  #endregion

  #region Properties

  /// <summary>
  ///   Gets the C# keyword representing the primitive type (e.g., <c>int</c>, <c>string</c>).
  /// </summary>
  public string Keyword { get; init; }

  #endregion

  #region Public Methods

  /// <summary>
  ///   Gets the macro key-value pairs for the specified converter type.
  /// </summary>
  /// <param name="converter">The converter type for which to retrieve macros.</param>
  /// <returns>An enumerable of macro key-value pairs, or an empty enumerable if none exist.</returns>
  public IEnumerable<KeyValuePair<string, string>> GetConverterMacros(
    TypedPrimitiveConverter converter )
  {
    // Return the macro dictionary for the converter, or an empty collection if not present
    return _customConverterMacros.TryGetValue( converter, out var macros ) ? macros : [];
  }

  /// <summary>
  ///   Gets the include key-value pairs for the specified converter type.
  /// </summary>
  /// <param name="converter">The converter type for which to retrieve includes.</param>
  /// <returns>An enumerable of include key-value pairs, or an empty enumerable if none exist.</returns>
  public IEnumerable<KeyValuePair<string, string>> GetIncludes(
    TypedPrimitiveConverter converter )
  {
    // Return the include dictionary for the converter, or an empty collection if not present
    return _includes.TryGetValue( converter, out var includes ) ? includes : [];
  }

  #endregion
}
