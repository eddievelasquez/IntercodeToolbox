// Module Name: MacroProcessorContext.cs
// Author:      Eduardo Velasquez
// Copyright (c) 2025, Intercode Consulting, Inc.

namespace Intercode.Toolbox.TemplateEngine;

/// <summary>
///   Represents the context for the <see cref="MacroProcessor" />, providing functionality to manage macros and their
///   values.
/// </summary>
/// <remarks>
///   This class is used to define and manage macros for the macro processor. It supports adding macros with
///   static or dynamic values, retrieving macro values, and managing macro slots. The context is initialized
///   with options that configure its behavior, such as registering standard macros.
/// </remarks>
public class MacroProcessorContext
{
  #region Constants

  private const int MACRO_NOT_FOUND_SLOT = -1;

  #endregion

  #region Fields

  private readonly Dictionary<string, int> _generatorsSlots = new ( StringComparer.OrdinalIgnoreCase );
  private readonly List<MacroValueGenerator?> _generators = [];

#if NET9_0_OR_GREATER
  private readonly Dictionary<string, int>.AlternateLookup<ReadOnlySpan<char>> _alternate;
#endif

  #endregion

  #region Constructors

  /// <summary>
  ///   Initializes a new instance of the <see cref="MacroProcessorContext" /> class.
  /// </summary>
  /// <param name="compilerOptions">
  ///   The Template Engine CompilerOptions. Will use to <see cref="TemplateCompilerOptions.Default" /> if
  ///   <c>null</c>.
  /// </param>
  public MacroProcessorContext(
    TemplateCompilerOptions? compilerOptions = null )
  {
    CompilerOptions = compilerOptions ?? TemplateCompilerOptions.Default;

#if NET9_0_OR_GREATER
    _alternate = _generatorsSlots.GetAlternateLookup<ReadOnlySpan<char>>();
#endif
  }

  #endregion

  #region Properties

  /// <summary>
  ///   Gets the options for the template engine.
  /// </summary>
  /// <value>
  ///   An instance of <see cref="TemplateCompilerOptions" /> representing the configuration
  ///   for the template engine. Defaults to <see cref="TemplateCompilerOptions.Default" /> if not explicitly set.
  /// </value>
  public TemplateCompilerOptions CompilerOptions { get; }

  /// <summary>
  ///   Gets the total number of macros currently added to the template context.
  /// </summary>
  /// <value>
  ///   The count of macros added to the template context.
  /// </value>
  public int MacroCount => _generators.Count;

  #endregion

  #region Public Methods

  /// <summary>
  ///   Adds a macro to the template context with a static value.
  /// </summary>
  /// <param name="macroName">
  ///   The name of the macro to add. Cannot be <c>null</c>, empty, or consist only of whitespace.
  /// </param>
  /// <param name="value">
  ///   The static value of the macro. Cannot be <c>null</c>.
  /// </param>
  /// <returns>
  ///   The slot index of the added macro.
  /// </returns>
  /// <exception cref="ArgumentNullException">
  ///   Thrown if <paramref name="value" /> is <c>null</c>.
  /// </exception>
  /// <exception cref="ArgumentException">
  ///   Thrown if <paramref name="macroName" /> is <c>null</c>, empty, or consists only of whitespace.
  /// </exception>
  public int AddMacro(
    string macroName,
    string? value )
  {
    MacroValueGenerator? generator = null;

    if( value != null )
    {
      generator = _ => value;
    }

    return AddMacro( macroName, generator );
  }

  /// <summary>
  ///   Adds a macro to the template context.
  /// </summary>
  /// <param name="macroName">
  ///   The name of the macro to add. Cannot be <c>null</c>, empty, or consist only of whitespace.
  /// </param>
  /// <param name="generator">
  ///   An optional <see cref="MacroValueGenerator" /> to generate the macro's value dynamically.
  ///   If <c>null</c>, the macro will not have a value generator.
  /// </param>
  /// <returns>
  ///   The slot index of the added macro.
  /// </returns>
  /// <exception cref="ArgumentException">
  ///   Thrown if <paramref name="macroName" /> is <c>null</c>, empty, or consists only of whitespace.
  /// </exception>
  public int AddMacro(
    string macroName,
    MacroValueGenerator? generator = null )
  {
    ValidateMacroName( macroName );

    // Use the existing slot if the macro was already added
    if( _generatorsSlots.TryGetValue( macroName, out var slot ) )
    {
      _generators[slot] = generator;
      return slot;
    }

    // Add a new slot if the macro was not previously added
    slot = _generators.Count;

    _generatorsSlots.Add( macroName, slot );
    _generators.Add( generator );

    return slot;
  }

  /// <summary>
  ///   Adds multiple macros to the template context.
  /// </summary>
  /// <param name="macros">
  ///   A collection of key-value pairs where the key represents the macro name and the value represents the macro's static
  ///   value.
  ///   The macro names cannot be <c>null</c>, empty, or consist only of whitespace, and the values cannot be <c>null</c>.
  /// </param>
  /// <exception cref="ArgumentNullException">
  ///   Thrown if any macro name or value in the collection is <c>null</c>.
  /// </exception>
  /// <exception cref="ArgumentException">
  ///   Thrown if any macro name in the collection is empty or consists only of whitespace.
  /// </exception>
  public void AddMacros(
    IEnumerable<KeyValuePair<string, string>> macros )
  {
    foreach( var pair in macros )
    {
      AddMacro( pair.Key, pair.Value );
    }
  }

  /// <summary>
  ///   Adds multiple macros to the template context.
  /// </summary>
  /// <param name="macros">
  ///   A collection of key-value pairs where the key is the macro name and the value is an optional
  ///   <see cref="MacroValueGenerator" /> to dynamically generate the macro's value.
  ///   The macro name cannot be <c>null</c>, empty, or consist only of whitespace.
  /// </param>
  /// <exception cref="ArgumentException">
  ///   Thrown if any macro name in the collection is <c>null</c>, empty, or consists only of whitespace.
  /// </exception>
  public void AddMacros(
    IEnumerable<KeyValuePair<string, MacroValueGenerator?>> macros )
  {
    foreach( var pair in macros )
    {
      AddMacro( pair.Key, pair.Value );
    }
  }

  /// <summary>
  ///   Retrieves the slot index of a macro in the template context.
  /// </summary>
  /// <param name="macroName">
  ///   The name of the macro whose slot index is to be retrieved. Cannot be <c>null</c>.
  /// </param>
  /// <returns>
  ///   The slot index of the macro if it exists; otherwise, <c>-1</c>.
  /// </returns>
  /// <exception cref="ArgumentNullException">
  ///   Thrown if <paramref name="macroName" /> is <c>null</c>.
  /// </exception>
  public int GetMacroSlot(
    string macroName )
  {
    if( macroName == null )
    {
      throw new ArgumentNullException( nameof( macroName ) );
    }

    if( !_generatorsSlots.TryGetValue( macroName, out var slot ) )
    {
      return MACRO_NOT_FOUND_SLOT;
    }

    return slot;
  }

  /// <summary>
  ///   Retrieves the value of the macro associated with the specified slot index.
  /// </summary>
  /// <param name="slot">
  ///   The slot index of the macro whose value is to be retrieved. Must be a valid index.
  /// </param>
  /// <returns>
  ///   The value of the macro if the slot index is valid and a value generator is associated with the slot;
  ///   otherwise, <c>null</c>.
  /// </returns>
  /// <exception cref="ArgumentOutOfRangeException">
  ///   Thrown if <paramref name="slot" /> is less than 0 or greater than or equal to the total number of slots.
  /// </exception>
  public string? GetMacroValue(
    int slot )
  {
    return GetMacroValue( slot, ReadOnlySpan<char>.Empty );
  }

  /// <summary>
  ///   Retrieves the value of a macro at the specified slot, optionally using the provided argument.
  /// </summary>
  /// <param name="slot">
  ///   The slot index of the macro. Must be a valid index within the range of added macros.
  /// </param>
  /// <param name="argument">
  ///   An optional argument to pass to the macro's value generator. Can be empty.
  /// </param>
  /// <returns>
  ///   The value of the macro if the slot is valid and a value generator exists; otherwise, <c>null</c>.
  /// </returns>
  public string? GetMacroValue(
    int slot,
    ReadOnlySpan<char> argument )
  {
    if( slot < 0 || slot >= _generators.Count )
    {
      return null;
    }

    var generator = _generators[slot];
    return generator?.Invoke( argument );
  }

  /// <summary>
  ///   Retrieves the value of a macro by its name.
  /// </summary>
  /// <param name="macroName">
  ///   The name of the macro whose value is to be retrieved. Cannot be <c>null</c>.
  /// </param>
  /// <returns>
  ///   The value of the macro if it exists and a value generator is defined; otherwise, <c>null</c>.
  /// </returns>
  /// <exception cref="ArgumentNullException">
  ///   Thrown if <paramref name="macroName" /> is <c>null</c>.
  /// </exception>
  public string? GetMacroValue(
    string macroName )
  {
    return GetMacroValue( GetMacroSlot( macroName ), ReadOnlySpan<char>.Empty );
  }

  /// <summary>
  ///   Retrieves the value of a macro identified by its name, optionally using the provided argument.
  /// </summary>
  /// <param name="macroName">
  ///   The name of the macro whose value is to be retrieved. Cannot be <c>null</c>.
  /// </param>
  /// <param name="argument">
  ///   An optional argument to pass to the macro's value generator. Can be empty.
  /// </param>
  /// <returns>
  ///   The value of the macro if the macro exists and a value generator is defined; otherwise, <c>null</c>.
  /// </returns>
  /// <exception cref="ArgumentNullException">
  ///   Thrown if <paramref name="macroName" /> is <c>null</c>.
  /// </exception>
  public string? GetMacroValue(
    string macroName,
    ReadOnlySpan<char> argument )
  {
    return GetMacroValue( GetMacroSlot( macroName ), argument );
  }

  #endregion

  #region Implementation

  private static void ValidateMacroName(
    string macroName )
  {
    if( macroName == null )
    {
      throw new ArgumentNullException( nameof( macroName ) );
    }

    ValidateMacroName( macroName.AsSpan() );
  }

  private static void ValidateMacroName(
    ReadOnlySpan<char> macroName )
  {
    if( macroName.IsEmpty )
    {
      throw new ArgumentException( "Macro name cannot be empty", nameof( macroName ) );
    }

    if( !IsAlphanumericOrUnderscoreOrDash( macroName ) )
    {
      throw new ArgumentException( "Macro name must be alphanumeric, underscore, or dash", nameof( macroName ) );
    }

    return;

    static bool IsAlphanumericOrUnderscoreOrDash(
      ReadOnlySpan<char> macroName )
    {
      // NOTE: Use loop instead of LINQ for performance
      for( var index = 0; index < macroName.Length; index++ )
      {
        var c = macroName[index];

        if( !c.IsMacroNameChar() )
        {
          return false;
        }
      }

      return true;
    }
  }

  #endregion

#if NET9_0_OR_GREATER
  /// <summary>
  ///   Retrieves the slot index of a macro in the template context.
  /// </summary>
  /// <param name="macroName">
  ///   The name of the macro to retrieve the slot for.
  /// </param>
  /// <returns>
  ///   The slot index of the macro if found; otherwise, <c>-1</c>.
  /// </returns>
  public int GetMacroSlot(
    ReadOnlySpan<char> macroName )
  {
    if( macroName.IsEmpty )
    {
      return MACRO_NOT_FOUND_SLOT;
    }

    return _alternate.TryGetValue( macroName, out var slot ) ? slot : MACRO_NOT_FOUND_SLOT;
  }

  /// <summary>
  ///   Adds a macro to the template context with a dynamic value generator.
  /// </summary>
  /// <param name="macroName">
  ///   The name of the macro to add.
  /// </param>
  /// <param name="generator">
  ///   A callback function to dynamically generate the macro value. Can be <c>null</c>.
  /// </param>
  /// <returns>
  ///   The slot index of the added macro.
  /// </returns>
  /// <remarks>
  ///   If the macro with the specified name already exists, the existing slot will be reused,
  ///   and its value generator will be updated.
  /// </remarks>
  public int AddMacro(
    ReadOnlySpan<char> macroName,
    MacroValueGenerator? generator = null )
  {
    ValidateMacroName( macroName );

    // Use the existing slot if the macro was already added
    if( _alternate.TryGetValue( macroName, out var slot ) )
    {
      var existingGenerator = _generators[slot];

      // Only replace the generator if the existing one is null and a new one is provided
      if( existingGenerator is null && generator is not null )
      {
        _generators[slot] = generator;
      }

      return slot;
    }

    // Add a new slot if the macro was not previously added
    slot = _generators.Count;

    _alternate.TryAdd( macroName, slot );
    _generators.Add( generator );

    return slot;
  }

#endif
}
