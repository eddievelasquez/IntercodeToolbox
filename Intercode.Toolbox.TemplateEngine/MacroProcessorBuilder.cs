// Module Name: MacroProcessorBuilder.cs
// Author:      Eduardo Velasquez
// Copyright (c) 2024, Intercode Consulting, Inc.

namespace Intercode.Toolbox.TemplateEngine;

using System.Collections.Frozen;

/// <summary>
///   Callback for dynamic macros.
/// </summary>
/// <param name="argument">Optional data passed to the callback for generating a value.</param>
public delegate string MacroValueGenerator(
  ReadOnlySpan<char> argument );

/// <summary>
///   Creates a <see cref="MacroProcessor" /> instance.
/// </summary>
public class MacroProcessorBuilder
{
  #region Fields

  private readonly Dictionary<string, MacroValueGenerator> _generators = new ( StringComparer.OrdinalIgnoreCase );
  private readonly TemplateEngineOptions _options;

  #endregion

  #region Constructors

  /// <summary>
  ///   Initializes a new instance of the <see cref="MacroProcessorBuilder" /> class.
  /// </summary>
  /// <param name="options">
  ///   The template engine options. Will use <see cref="TemplateEngineOptions.Default" /> if <c>null</c>.
  /// </param>
  public MacroProcessorBuilder(
    TemplateEngineOptions? options = null )
  {
    _options = options ?? TemplateEngineOptions.Default;
  }

  #endregion

  #region Public Methods

  /// <summary>
  ///   Adds multiple macros with the specified names and values.
  /// </summary>
  /// <param name="macros">The collection of key-value pairs representing the macros to add.</param>
  /// <returns>The <see cref="MacroProcessorBuilder" /> instance.</returns>
  public MacroProcessorBuilder AddMacros(
    IEnumerable<KeyValuePair<string, string>> macros )
  {
    foreach( var pair in macros )
    {
      AddMacro( pair.Key, pair.Value );
    }

    return this;
  }

  /// <summary>
  ///   Adds multiple macros with the specified names and values.
  /// </summary>
  /// <param name="macros">The collection of key-value generator pairs representing the macros to add.</param>
  /// <returns>The <see cref="MacroProcessorBuilder" /> instance.</returns>
  public MacroProcessorBuilder AddMacros(
    IEnumerable<KeyValuePair<string, MacroValueGenerator>> macros )
  {
    foreach( var pair in macros )
    {
      AddMacro( pair.Key, pair.Value );
    }

    return this;
  }

  /// <summary>
  ///   Adds a macro with the specified name and value.
  /// </summary>
  /// <param name="name">The name of the macro.</param>
  /// <param name="value">The value of the macro.</param>
  /// <returns>The <see cref="MacroProcessorBuilder" /> instance.</returns>
  /// <exception cref="ArgumentException">
  ///   Thrown when the macro name is <c>null</c>, <see cref="String.Empty" />, or whitespace, or contains
  ///   any character that is not alphanumeric or an underscore.
  /// </exception>
  public MacroProcessorBuilder AddMacro(
    string name,
    string value )
  {
    ValidateMacroName( name );
    _generators.Add( name, _ => value );

    return this;
  }

  /// <summary>
  ///   Adds a macro with the specified name and a dynamically generated value using the provided generator.
  /// </summary>
  /// <param name="name">The name of the macro.</param>
  /// <param name="generator">A function that dynamically generates the macro value.</param>
  /// <returns></returns>
  public MacroProcessorBuilder AddMacro(
    string name,
    MacroValueGenerator generator )
  {
    ValidateMacroName( name );
    _generators.Add( name, generator );

    return this;
  }

  /// <summary>
  ///   Builds a <see cref="MacroProcessor" /> instance using the added macros and macro delimiter.
  /// </summary>
  /// <returns>The <see cref="MacroProcessor" /> instance.</returns>
  public MacroProcessor Build()
  {
    var generators = _generators.ToFrozenDictionary( StringComparer.OrdinalIgnoreCase );
    return new MacroProcessor( generators, _options );
  }

  #endregion

  #region Implementation

  private static void ValidateMacroName(
    string name )
  {
    if( string.IsNullOrEmpty( name ) )
    {
      throw new ArgumentException( "Value cannot be null or empty.", nameof( name ) );
    }

    if( !IsAlphanumericOrUnderscore( name ) )
    {
      throw new ArgumentException( "Macro name must be alphanumeric or underscore.", nameof( name ) );
    }

    return;

    static bool IsAlphanumericOrUnderscore(
      string input )
    {
      // NOTE: Use loop instead of LINQ for performance
      foreach( var c in input )
      {
        if( !char.IsLetterOrDigit( c ) && c != '_' )
        {
          return false;
        }
      }

      return true;
    }
  }

  #endregion
}
