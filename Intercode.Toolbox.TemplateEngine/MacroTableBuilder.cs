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
  private bool _addStandardMacros;

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
    MacroExtensions.ValidateMacroName( macroName );

    if( !_macroSlots.ContainsKey( macroName ) )
    {
      _macroSlots.Add( macroName, GetAssignedSlot() );
    }

    return this;
  }

  /// <summary>
  ///   Declares a macro name (as a <see cref="ReadOnlySpan{Char}" />) to be included in the resulting
  ///   <see cref="MacroTable" />.
  /// </summary>
  /// <param name="macroName">The macro name to declare.</param>
  /// <returns>The current <see cref="MacroTableBuilder" /> instance.</returns>
  /// <exception cref="ArgumentException">Thrown if <paramref name="macroName" /> is invalid.</exception>
  public MacroTableBuilder Declare(
    ReadOnlySpan<char> macroName )
  {
#if NET9_0_OR_GREATER
    MacroExtensions.ValidateMacroName( macroName );

    if( !_altMacroNames.ContainsKey( macroName ) )
    {
      _altMacroNames.TryAdd( macroName, GetAssignedSlot() );
    }
#else
    Declare( macroName.ToString() );
#endif

    return this;
  }

  /// <summary>
  ///   Declares the standard macros.
  /// </summary>
  /// <returns>The current <see cref="MacroTableBuilder" /> instance.</returns>
  /// <remarks>
  ///   <list type="table">
  ///     <listheader>
  ///       <term>Name</term>
  ///       <description>Description</description>
  ///     </listheader>
  ///     <item>
  ///       <term>NOW</term>
  ///       <description>
  ///         Gets the current local date and time. The optional argument is the format string passed to the
  ///         <see cref="DateTime.ToString(String)" /> method.
  ///       </description>
  ///     </item>
  ///     <item>
  ///       <term>UTC_NOW</term>
  ///       <description>
  ///         Gets the current UTC date and time. The optional argument is the format string passed to the
  ///         <see cref="DateTime.ToString(String)" /> method.
  ///       </description>
  ///     </item>
  ///     <item>
  ///       <term>GUID</term>
  ///       <description>
  ///         Generates a new Guid. The optional argument is the format string passed to the
  ///         <see cref="Guid.ToString(String)" /> method.
  ///       </description>
  ///     </item>
  ///     <item>
  ///       <term>MACHINE</term>
  ///       <description>
  ///         Gets the name of the local computer as returned by the <see cref="Environment.MachineName" />
  ///         property.
  ///       </description>
  ///     </item>
  ///     <item>
  ///       <term>OS</term>
  ///       <description>Gets the operating system version as returned by the <see cref="Environment.OSVersion" /> property.</description>
  ///     </item>
  ///     <item>
  ///       <term>USER</term>
  ///       <description>Gets the name of the current user as returned by the <see cref="Environment.UserName" /> property.</description>
  ///     </item>
  ///     <item>
  ///       <term>CLR_VERSION</term>
  ///       <description>Gets the CLR version as returned by the <see cref="Environment.Version" /> property.</description>
  ///     </item>
  ///     <item>
  ///       <term>ENV</term>
  ///       <description>
  ///         Gets the value of the environment variable specified by the argument as return by the
  ///         <see cref="Environment.GetEnvironmentVariable(String)" /> method.
  ///       </description>
  ///     </item>
  ///   </list>
  ///   <para>NOTE: If a generator throws an exception, the macro's value will be the exception's error message.</para>
  /// </remarks>
  public MacroTableBuilder DeclareStandardMacros()
  {
    _addStandardMacros = true;
    return this;
  }

  /// <summary>
  ///   Constructs a <see cref="MacroTable" /> containing all declared macro names.
  /// </summary>
  /// <returns>A <see cref="MacroTable" /> instance populated with the declared macros.</returns>
  public MacroTable Build()
  {
    // Standard macros are always added at the end to ensure consistent slot assignments;
    // as custom macros slots are assigned in order of declaration
    if( _addStandardMacros )
    {
      var slot = _macroSlots.Count;

      foreach( var macroName in StandardMacros.GetStandardMacroNames() )
      {
        _macroSlots.Add( macroName, slot++ );
      }
    }

    return new MacroTable( _macroSlots, _addStandardMacros );
  }

  #endregion

  #region Implementation

  private int GetAssignedSlot()
  {
    return _currentSlot++;
  }

  #endregion
}
