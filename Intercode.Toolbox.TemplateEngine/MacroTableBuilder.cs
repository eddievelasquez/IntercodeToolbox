// Module Name: MacroTableBuilder.cs
// Author:      Eduardo Velasquez
// Copyright (c) 2025, Intercode Consulting, Inc.

namespace Intercode.Toolbox.TemplateEngine;

/// <summary>
///   Provides a builder for constructing a <see cref="MacroTable" /> with declared macro names.
/// </summary>
public sealed class MacroTableBuilder
{
  #region Fields

  private readonly Dictionary<string, int> _macroSlots = new ( StringComparer.OrdinalIgnoreCase );

#if NET9_0_OR_GREATER
  private readonly Dictionary<string, int>.AlternateLookup<ReadOnlySpan<char>> _altMacroNames;
#endif

  private int _currentSlot;

  #endregion

  #region Constructors

  /// <summary>
  ///   Initializes a new instance of the <see cref="MacroTableBuilder" /> class.
  /// </summary>
  public MacroTableBuilder()
  {
#if NET9_0_OR_GREATER
    _altMacroNames = _macroSlots.GetAlternateLookup<ReadOnlySpan<char>>();
#endif
  }

  #endregion

  #region Public Methods

  /// <summary>
  ///   Declares a macro name to be included in the resulting <see cref="MacroTable" />.
  /// </summary>
  /// <param name="macroName">The macro name to declare.</param>
  /// <returns>The current <see cref="MacroTableBuilder" /> instance.</returns>
  /// <exception cref="ArgumentException">Thrown if <paramref name="macroName" /> is invalid.</exception>
  public MacroTableBuilder Declare(
    string macroName )
  {
    DeclareInternal( macroName );
    return this;
  }

  /// <summary>
  ///   Declares a macro name (as a <see cref="ReadOnlySpan{Char}" />) to be included in the resulting
  ///   <see cref="MacroTable" />.
  /// </summary>
  /// <param name="macroName">The macro name to declare as a span.</param>
  /// <returns>The current <see cref="MacroTableBuilder" /> instance.</returns>
  /// <exception cref="ArgumentException">Thrown if <paramref name="macroName" /> is invalid.</exception>
  public MacroTableBuilder Declare(
    ReadOnlySpan<char> macroName )
  {
    DeclareInternal( macroName );
    return this;
  }

  /// <summary>
  ///   Constructs a <see cref="MacroTable" /> containing all declared macro names.
  /// </summary>
  /// <returns>A <see cref="MacroTable" /> instance populated with the declared macros.</returns>
  public MacroTable Build()
  {
    return new MacroTable( _macroSlots );
  }

  #endregion

  #region Implementation

  /// <summary>
  ///   Declares a macro name and assigns a slot index.
  /// </summary>
  /// <param name="macroName">The macro name to declare.</param>
  /// <returns>The slot index assigned to the macro name.</returns>
  /// <exception cref="ArgumentException">Thrown if <paramref name="macroName" /> is invalid.</exception>
  internal int DeclareInternal(
    string macroName )
  {
    MacroExtensions.ValidateMacroName( macroName );
    return _macroSlots.GetOrAdd( macroName, _ => GetAssignedSlot() );
  }

  /// <summary>
  ///   Declares a macro name from a <see cref="ReadOnlySpan{T}" /> and assigns a slot index.
  /// </summary>
  /// <param name="macroName">The macro name to declare as a span.</param>
  /// <returns>The slot index assigned to the macro name.</returns>
  /// <exception cref="ArgumentException">Thrown if <paramref name="macroName" /> is invalid.</exception>
  internal int DeclareInternal(
    ReadOnlySpan<char> macroName )
  {
    MacroExtensions.ValidateMacroName( macroName );

#if NET9_0_OR_GREATER
    return _altMacroNames.GetOrAdd( macroName, _ => GetAssignedSlot() );
#else
    return DeclareInternal( macroName.ToString() );
#endif
  }

  /// <summary>
  ///   Returns the next available slot index for a macro. Slots are 1-based.
  /// </summary>
  /// <returns>The next slot index.</returns>
  private int GetAssignedSlot()
  {
    // Slots are 1-based, they are not indices
    return ++_currentSlot;
  }

  #endregion
}
