// Module Name: TemplateMacroValues.cs
// Author:      Eduardo Velasquez
// Copyright (c) 2025, Intercode Consulting, Inc.

namespace Intercode.Toolbox.TemplateEngine;

/// <summary>
///   Represents a collection of macro values and their associated generators for use within a template.
/// </summary>
/// <remarks>
///   This class provides methods to retrieve, set, and manage macro values, which can be used to dynamically
///   generate content within templates. Macros can be accessed by their names or slot indices, and their values
///   can be generated dynamically using custom generators.
/// </remarks>
public class TemplateMacroValues
{
  #region Constructors

  internal TemplateMacroValues(
    Template template )
  {
    Template = template;
    Generators = new MacroValueGenerator[template.MacroTable.Count];
  }

  #endregion

  #region Properties

  /// <summary>
  ///   Gets the <see cref="Template" /> associated with the current instance of <see cref="TemplateMacroValues" />.
  /// </summary>
  /// <remarks>
  ///   The <see cref="Template" /> provides the structure and macro definitions used for generating dynamic content.
  ///   It includes text, segments, and a macro table that maps macro names to their respective slots.
  /// </remarks>
  public Template Template { get; }
  private MacroValueGenerator?[] Generators { get; }

  #endregion

  #region Public Methods

  /// <summary>
  ///   Gets the value of a macro or <c>null</c> if not found.
  /// </summary>
  /// <param name="macroName">
  ///   The name of the macro. The macro name used here does not include the delimiter character.
  /// </param>
  /// <returns>The value of the macro, or null if the macro does not exist.</returns>
  public string? GetMacroValue(
    string macroName )
  {
    return GetMacroValue( macroName, ReadOnlySpan<char>.Empty );
  }

  /// <summary>
  ///   Gets the value of a macro or <c>null</c> if not found.
  /// </summary>
  /// <param name="macroName">
  ///   The name of the macro. The macro name used here does not include the delimiter character.
  /// </param>
  /// <param name="argument">Data passed into the value generator.</param>
  /// <returns>The value of the macro, or null if the macro does not exist.</returns>
  public string? GetMacroValue(
    string macroName,
    ReadOnlySpan<char> argument )
  {
    return GetMacroValue( GetMacroSlot( macroName ), argument );
  }

  /// <summary>
  ///   Gets the value of a macro or <c>null</c> if not found.
  /// </summary>
  /// <param name="slot">
  ///   The slot index of the macro. Must be greater than or equal to zero and less
  ///   than the total number of macros.
  /// </param>
  /// <returns>
  ///   The value of the macro, or null a generator has not been set for the given slot,
  ///   or the slot is out of range.
  /// </returns>
  public string? GetMacroValue(
    int slot )
  {
    return GetMacroValue( slot, ReadOnlySpan<char>.Empty );
  }

  /// <summary>
  ///   Gets the value of a macro or <c>null</c> if not found.
  /// </summary>
  /// <param name="slot">
  ///   The slot index of the macro. Must be greater than or equal to zero and less
  ///   than the total number of macros.
  /// </param>
  /// <param name="argument">Data passed into the value generator.</param>
  /// <returns>
  ///   The value of the macro, or null a generator has not been set for the given slot,
  ///   or the slot is out of range.
  /// </returns>
  public string? GetMacroValue(
    int slot,
    ReadOnlySpan<char> argument )
  {
    if( slot < 0 || slot >= Generators.Length )
    {
      return null;
    }

    var generator = Generators[slot];
    var value = generator?.Invoke( argument );
    return value;
  }

  /// <summary>
  ///   Adds multiple macros with the specified names and values.
  /// </summary>
  /// <param name="macros">The collection of key-value pairs representing the macros to add.</param>
  /// <returns>The <see cref="TemplateMacroValues" /> instance.</returns>
  public TemplateMacroValues SetMacros(
    IEnumerable<KeyValuePair<string, string>> macros )
  {
    foreach( var pair in macros )
    {
      SetMacro( pair.Key, pair.Value );
    }

    return this;
  }

  /// <summary>
  ///   Adds multiple macros with the specified names and values.
  /// </summary>
  /// <param name="macros">The collection of key-value generator pairs representing the macros to add.</param>
  /// <returns>The <see cref="TemplateMacroValues" /> instance.</returns>
  public TemplateMacroValues SetMacros(
    IEnumerable<KeyValuePair<string, MacroValueGenerator>> macros )
  {
    foreach( var pair in macros )
    {
      SetMacro( pair.Key, pair.Value );
    }

    return this;
  }

  /// <summary>
  ///   Adds a macro with the specified name and value.
  /// </summary>
  /// <param name="name">The name of the macro.</param>
  /// <param name="value">The value of the macro.</param>
  /// <returns>The <see cref="TemplateMacroValues" /> instance.</returns>
  /// <exception cref="ArgumentException">
  ///   Thrown when the macro name is <c>null</c> or whitespace.
  /// </exception>
  /// <remarks>Macros names that are not found in the template are ignored.</remarks>
  public TemplateMacroValues SetMacro(
    string name,
    string value )
  {
    var slot = GetMacroSlot( name );

    // Ignore macros that are not in the template
    if( slot >= 0 )
    {
      Generators[slot] = _ => value;
    }

    return this;
  }

  /// <summary>
  ///   Adds a macro with the specified name and a dynamically generated value using the provided generator.
  /// </summary>
  /// <param name="name">The name of the macro.</param>
  /// <param name="generator">A function that dynamically generates the macro value.</param>
  /// <returns>The <see cref="TemplateMacroValues" /> instance.</returns>
  /// <remarks>Macros names that are not found in the template are ignored.</remarks>
  public TemplateMacroValues SetMacro(
    string name,
    MacroValueGenerator generator )
  {
    var slot = GetMacroSlot( name );

    // Ignore macros that are not in the template
    if( slot >= 0 )
    {
      Generators[slot] = generator;
    }

    return this;
  }

  #endregion

  #region Implementation

  internal int GetMacroSlot(
    string name )
  {
    if( string.IsNullOrWhiteSpace( name ) )
    {
      throw new ArgumentException( "The macro name cannot be null or empty", nameof( name ) );
    }

    if( Template.MacroTable.TryGetValue( name, out var slot ) )
    {
      return slot;
    }

    return -1;
  }

  #endregion
}
