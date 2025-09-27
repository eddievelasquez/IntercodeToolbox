// Module Name: MacroTable.cs
// Author:      Eduardo Velasquez
// Copyright (c) 2025, Intercode Consulting, Inc.

namespace Intercode.Toolbox.TemplateEngine;

using System.Collections.Frozen;

/// <summary>
///   Represents an immutable table of macro names and their associated slot indices.
/// </summary>
public sealed class MacroTable
{
  #region Constants

  /// <summary>
  ///   Represents the slot index returned when a macro name is not found in the <see cref="MacroTable" />.
  /// </summary>
  /// <remarks>
  ///   This constant is used as a default value to indicate that a macro name does not have an associated slot.
  /// </remarks>
  public const int MacroNotFoundSlot = 0;

  #endregion

  #region Fields

  private readonly FrozenDictionary<string, int> _macroSlots;
#if NET9_0_OR_GREATER
  private readonly FrozenDictionary<string, int>.AlternateLookup<ReadOnlySpan<char>> _altMacroSlots;
#endif

  #endregion

  #region Constructors

  /// <summary>
  ///   Initializes a new instance of the <see cref="MacroTable" /> class with the specified macro slots and a flag
  ///   indicating the presence of standard macros.
  /// </summary>
  /// <param name="macroSlots">A dictionary containing macro names and their corresponding slot indices.</param>
  internal MacroTable(
    IDictionary<string, int> macroSlots )
  {
    _macroSlots = macroSlots.ToFrozenDictionary( StringComparer.OrdinalIgnoreCase );
#if NET9_0_OR_GREATER
    _altMacroSlots = _macroSlots.GetAlternateLookup<ReadOnlySpan<char>>();
#endif
  }

  #endregion

  #region Properties

  /// <summary>
  ///   Gets the number of macros in the table.
  /// </summary>
  public int Count => _macroSlots.Count;

  #endregion

  #region Public Methods

  /// <summary>
  ///   Creates a new instance of <see cref="MacroValues" /> associated with this <see cref="MacroTable" />.
  /// </summary>
  /// <returns>
  ///   A <see cref="MacroValues" /> object that allows setting and retrieving macro values
  ///   based on the macros defined in this <see cref="MacroTable" />.
  /// </returns>
  public MacroValues CreateValues()
  {
    return new MacroValues( this );
  }

  /// <summary>
  ///   Gets the slot index for the specified macro name.
  /// </summary>
  /// <param name="macroName">The macro name to look up.</param>
  /// <returns>The slot index if found; otherwise, <c>-1</c>.</returns>
  /// <exception cref="ArgumentNullException">Thrown if <paramref name="macroName" /> is <c>null</c> or empty.</exception>
  public int GetSlot(
    string macroName )
  {
    if( string.IsNullOrEmpty( macroName ) )
    {
      throw new ArgumentException( "The macro name cannot be null or empty", nameof( macroName ) );
    }

    // First check in the declared macros; then check in standard macros.
    if( _macroSlots.TryGetValue( macroName, out var slot ) )
    {
      return slot;
    }

    return StandardMacros.GetSlot( macroName );
  }

  /// <summary>
  ///   Gets the slot index for the specified macro name as a <see cref="System.ReadOnlySpan{T}" /> of <see cref="char" />.
  /// </summary>
  /// <param name="macroName">The macro name to look up.</param>
  /// <returns>The slot index if found; otherwise, <c>-1</c>.</returns>
  public int GetSlot(
    ReadOnlySpan<char> macroName )
  {
#if NET9_0_OR_GREATER
    return _altMacroSlots.TryGetValue( macroName, out var slot )
      ? slot
      : StandardMacros.GetSlot( macroName );
#else
    return GetSlot( macroName.ToString() );
#endif
  }

  #endregion
}
