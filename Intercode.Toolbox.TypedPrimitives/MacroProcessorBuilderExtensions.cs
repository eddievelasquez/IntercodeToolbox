// Module Name: MacroProcessorBuilderExtensions.cs
// Author:      Eduardo Velasquez
// Copyright (c) 2025, Intercode Consulting, Inc.

namespace Intercode.Toolbox.TypedPrimitives;

using Intercode.Toolbox.TemplateEngine;

internal static class MacroProcessorBuilderExtensions
{
  #region Public Methods

  /// <summary>
  ///   Adds macros to the specified <see cref="MacroValues" /> instance based on the provided
  ///   <paramref name="converter" /> and <paramref name="typeInfo" />.
  /// </summary>
  /// <param name="macroValues">
  ///   The <see cref="MacroValues" /> instance to which macros will be added.
  /// </param>
  /// <param name="converter">
  ///   The <see cref="ConverterModel" /> representing the converter whose macros are to be added.
  /// </param>
  /// <param name="typeInfo">
  ///   The <see cref="SupportedTypeInfo" /> containing the macros associated with the converter type.
  /// </param>
  /// <returns>
  ///   The updated <see cref="MacroValues" /> instance with the added macros.
  /// </returns>
  /// <remarks>
  ///   If a converter is not enabled, its macro values are set to <c>null</c>.
  /// </remarks>
  public static MacroValues AddConverterMacros(
    this MacroValues macroValues,
    ConverterModel converter,
    SupportedTypeInfo typeInfo )
  {
    foreach( var pair in typeInfo.GetConverterMacros( converter.Type ) )
    {
      macroValues.SetValue( pair.Key, converter.IsEnabled ? pair.Value : null );
    }

    return macroValues;
  }

  /// <summary>
  ///   Adds include definitions to the specified <see cref="IncludesCollection" />
  ///   based on the provided <see cref="ConverterModel" /> and <see cref="SupportedTypeInfo" />.
  /// </summary>
  /// <param name="includes">
  ///   The <see cref="IncludesCollection" /> to which the include definitions will be added.
  /// </param>
  /// <param name="converter">
  ///   The <see cref="ConverterModel" /> representing the converter type and its enabled state.
  /// </param>
  /// <param name="typeInfo">
  ///   The <see cref="SupportedTypeInfo" /> containing the include definitions for the specified converter type.
  /// </param>
  /// <returns>
  ///   The updated <see cref="IncludesCollection" /> with the added include definitions.
  /// </returns>
  /// <remarks>
  ///   If a converter is not enabled, its includes are skipped.
  /// </remarks>
  public static IncludesCollection AddIncludes(
    this IncludesCollection includes,
    ConverterModel converter,
    SupportedTypeInfo typeInfo )
  {
    foreach( var pair in typeInfo.GetIncludes( converter.Type ) )
    {
      includes.AddInclude( pair.Key, converter.IsEnabled ? pair.Value : null );
    }

    return includes;
  }

  #endregion
}
