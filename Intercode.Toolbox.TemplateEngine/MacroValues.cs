// Module Name: MacroValues.cs
// Author:      Eduardo Velasquez
// Copyright (c) 2025, Intercode Consulting, Inc.

namespace Intercode.Toolbox.TemplateEngine;

/// <summary>
///   Represents a collection of macro values associated with a <see cref="MacroTable" />.
/// </summary>
/// <remarks>
///   This class provides functionality to set and retrieve values or value generators for macros
///   defined in the associated <see cref="MacroTable" />. It supports both static values and dynamic
///   value generators, allowing for flexible macro value management.
/// </remarks>
public sealed class MacroValues
{
  #region Constants

  private static readonly MacroValueGenerator s_nullGenerator = _ => null;

  #endregion

  #region Fields

  private readonly MacroValueGenerator[] _generators;

  #endregion

  #region Constructors

  internal MacroValues(
    MacroTable macroTable )
  {
    MacroTable = macroTable;
    _generators = new MacroValueGenerator[macroTable.Count];

#if NET8_0_OR_GREATER
    Array.Fill( _generators, s_nullGenerator );
#else
    for( var i = 0; i < _generators.Length; i++ )
    {
      _generators[i] = s_nullGenerator;
    }
#endif
  }

  #endregion

  #region Properties

  /// <summary>
  ///   Gets the <see cref="MacroTable" /> associated with this <see cref="MacroValues" /> instance.
  /// </summary>
  public MacroTable MacroTable { get; }

  #endregion

  #region Public Methods

  /// <summary>
  ///   Sets the value generator for the specified macro name.
  /// </summary>
  /// <param name="macroName">
  ///   The name of the macro for which the value generator is being set.
  /// </param>
  /// <param name="generator">
  ///   The value generator delegate to associate with the macro, or <c>null</c> to clear the value.
  /// </param>
  /// <returns>
  ///   The current <see cref="MacroValues" /> instance, allowing for method chaining.
  /// </returns>
  /// <exception cref="ArgumentException">
  ///   Thrown if the specified <paramref name="macroName" /> does not exist in the macro table.
  /// </exception>
  public MacroValues SetValue(
    string macroName,
    MacroValueGenerator? generator )
  {
    var slot = MacroTable.GetSlot( macroName );

    if( slot == MacroTable.MacroNotFoundSlot )
    {
      throw new ArgumentException(
        $"Macro '{macroName}' does not exist in the macro table.",
        nameof( macroName )
      );
    }

    return SetValue( slot, generator );
  }

  /// <summary>
  ///   Sets the static value for the specified macro name.
  /// </summary>
  /// <param name="macroName">The name of the macro to set.</param>
  /// <param name="value">
  ///   The value to associate with the macro, or <c>null</c> to clear the value.
  /// </param>
  /// <returns>
  ///   The current <see cref="MacroValues" /> instance, allowing for method chaining.
  /// </returns>
  /// <exception cref="ArgumentException">
  ///   Thrown if the specified <paramref name="macroName" /> does not exist in the macro table.
  /// </exception>
  public MacroValues SetValue(
    string macroName,
    string? value )
  {
    var slot = MacroTable.GetSlot( macroName );

    if( slot == MacroTable.MacroNotFoundSlot )
    {
      throw new ArgumentException(
        $"Macro '{macroName}' does not exist in the macro table.",
        nameof( macroName )
      );
    }

    MacroValueGenerator? generator = null;

    // If a non-null value is provided, create a generator that returns this value.
    if( value != null )
    {
      generator = _ => value;
    }

    return SetValue( slot, generator ?? s_nullGenerator );
  }

  /// <summary>
  ///   Sets the value generator for the specified macro slot.
  /// </summary>
  /// <param name="slot">
  ///   The slot index of the macro for which the value generator is being set.
  /// </param>
  /// <param name="generator">
  ///   The value generator delegate to associate with the macro, or <c>null</c> to clear the value.
  /// </param>
  /// <returns>
  ///   The current <see cref="MacroValues" /> instance, allowing for method chaining.
  /// </returns>
  /// <exception cref="ArgumentOutOfRangeException">
  ///   Thrown if the specified <paramref name="slot" /> is outside the valid range of slots.
  /// </exception>
  public MacroValues SetValue(
    int slot,
    MacroValueGenerator? generator )
  {
    // Only user-defined macros can be assigned
    if( slot <= MacroTable.MacroNotFoundSlot || slot > _generators.Length )
    {
      throw new ArgumentOutOfRangeException(
        nameof( slot ),
        slot,
        "Invalid slot number."
      );
    }

    _generators[slot - 1] = generator ?? s_nullGenerator;
    return this;
  }

  /// <summary>
  ///   Sets the value of a macro at the specified slot.
  /// </summary>
  /// <param name="slot">
  ///   The zero-based index of the slot where the macro value should be set.
  ///   Must be between 0 and the total number of slots minus 1.
  /// </param>
  /// <param name="value">
  ///   The value to assign to the macro. If <c>null</c>, the macro value will be cleared.
  /// </param>
  /// <returns>
  ///   The current <see cref="MacroValues" /> instance, allowing for method chaining.
  /// </returns>
  /// <exception cref="ArgumentOutOfRangeException">
  ///   Thrown when the <paramref name="slot" /> is less than 0 or greater than or equal to the number of available slots.
  /// </exception>
  public MacroValues SetValue(
    int slot,
    string? value )
  {
    return SetValue( slot, value != null ? _ => value : null );
  }

  /// <summary>
  ///   Retrieves the value associated with the specified macro name, using the provided argument if applicable.
  /// </summary>
  /// <param name="macroName">The name of the macro whose value is to be retrieved.</param>
  /// <param name="argument">
  ///   An optional argument to pass to the macro's value generator. Defaults to <see cref="ReadOnlySpan{T}.Empty" /> if not
  ///   provided.
  /// </param>
  /// <returns>
  ///   The value of the macro if it has been set; otherwise, <c>null</c>.
  /// </returns>
  public string? GetValue(
    string macroName,
    ReadOnlySpan<char> argument = default )
  {
    var slot = MacroTable.GetSlot( macroName );
    return GetValue( slot, argument );
  }

  /// <summary>
  ///   Retrieves the value associated with the specified macro slot, using the provided argument if applicable.
  /// </summary>
  /// <param name="slot">The index of the macro slot to retrieve the value for.</param>
  /// <param name="argument">
  ///   An optional argument to pass to the macro's value generator. Defaults to an empty span if not provided.
  /// </param>
  /// <returns>
  ///   The value of the macro if it is set and the slot is valid; otherwise, <c>null</c>.
  /// </returns>
  public string? GetValue(
    int slot,
    ReadOnlySpan<char> argument = default )
  {
    // Return null if the macro name does not exist or is out of range.
    if( slot == MacroTable.MacroNotFoundSlot || slot > _generators.Length )
    {
      return null;
    }

    // Return the value from StandardMacros if the slot is negative.
    if( slot < MacroTable.MacroNotFoundSlot )
    {
      return StandardMacros.GetValue( slot, argument );
    }

    var generator = _generators[slot - 1];
    return generator( argument );
  }

  /// <summary>
  ///   Sets the value generator for the specified macro name using a <see cref="System.ReadOnlySpan{T}" /> of
  ///   <see cref="char" />.
  /// </summary>
  /// <param name="macroName">The macro name as a <see cref="System.ReadOnlySpan{T}" /> of <see cref="char" />.</param>
  /// <param name="generator">The value generator delegate to associate with the macro, or <c>null</c> to clear the value.</param>
  /// <exception cref="ArgumentException">Thrown if the macro name does not exist in the macro table.</exception>
  public MacroValues SetValue(
    ReadOnlySpan<char> macroName,
    MacroValueGenerator? generator )
  {
#if NET9_0_OR_GREATER
    var slot = MacroTable.GetSlot( macroName );

    if( slot == MacroTable.MacroNotFoundSlot )
    {
      throw new ArgumentException(
        $"Macro '{macroName}' does not exist in the macro table.",
        nameof( macroName )
      );
    }

    return SetValue( slot, generator );
#else
    return SetValue( macroName.ToString(), generator );
#endif
  }

  /// <summary>
  ///   Sets the value for the specified macro name using a <see cref="System.ReadOnlySpan{T}" /> of <see cref="char" />.
  /// </summary>
  /// <param name="macroName">The macro name as a <see cref="System.ReadOnlySpan{T}" /> of <see cref="char" />.</param>
  /// <param name="value">The value to associate with the macro, or <c>null</c> to clear the value.</param>
  /// <exception cref="ArgumentException">Thrown if the macro name does not exist in the macro table.</exception>
  public MacroValues SetValue(
    ReadOnlySpan<char> macroName,
    string? value )
  {
    return SetValue( macroName, value != null ? _ => value : null );
  }

  /// <summary>
  ///   Gets the value for the specified macro name using a <see cref="System.ReadOnlySpan{T}" /> of <see cref="char" /> and
  ///   an optional argument.
  /// </summary>
  /// <param name="macroName">The macro name as a <see cref="System.ReadOnlySpan{T}" /> of <see cref="char" />.</param>
  /// <param name="argument">
  ///   An optional argument to pass to the value generator. Defaults to
  ///   <see cref="System.ReadOnlySpan{T}.Empty" />.
  /// </param>
  /// <returns>The value of the macro if set; otherwise, <c>null</c>.</returns>
  public string? GetValue(
    ReadOnlySpan<char> macroName,
    ReadOnlySpan<char> argument = default )
  {
    var slot = MacroTable.GetSlot( macroName );
    return GetValue( slot, argument );
  }

  #endregion
}
