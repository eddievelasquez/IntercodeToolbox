// Module Name: StandardMacros.cs
// Author:      Eduardo Velasquez
// Copyright (c) 2025, Intercode Consulting, Inc.

namespace Intercode.Toolbox.TemplateEngine;

using System.Collections.Frozen;
using System.Globalization;

/// <summary>
///   Extension methods for adding standard macros
/// </summary>
internal static class StandardMacros
{
  #region Constants

  /// <summary>
  ///   The macro name for the current local date and time.
  /// </summary>
  public const string NowMacroName = "NOW";

  /// <summary>
  ///   The macro name for the current UTC date and time.
  /// </summary>
  public const string UtcNowMacroName = "UTC_NOW";

  /// <summary>
  ///   The macro name for generating a new Guid.
  /// </summary>
  public const string GuidMacroName = "GUID";

  /// <summary>
  ///   The macro name for the local computer name.
  /// </summary>
  public const string MachineMacroName = "MACHINE";

  /// <summary>
  ///   The macro name for the operating system version.
  /// </summary>
  public const string OsMacroName = "OS";

  /// <summary>
  ///   The macro name for the current username.
  /// </summary>
  public const string UserMacroName = "USER";

  /// <summary>
  ///   The macro name for the CLR version.
  /// </summary>
  public const string ClrVersionMacroName = "CLR_VERSION";

  /// <summary>
  ///   The macro name for environment variable values.
  /// </summary>
  public const string EnvMacroName = "ENV";

  private static readonly MacroValueGenerator[] s_generators;
  private static readonly FrozenDictionary<string, int> s_macroSlots;

#if NET9_0_OR_GREATER
  private static readonly FrozenDictionary<string, int>.AlternateLookup<ReadOnlySpan<char>>
    s_altMacroSlots;
#endif

  #endregion

  #region Constructors

  static StandardMacros()
  {
    // @formatter:off
    s_generators =
    [
      NowGenerator,
      UtcNowGenerator,
      GuidGenerator,
      _ => Environment.MachineName,
      _ => Environment.OSVersion.VersionString,
      _ => Environment.UserName,
      _ => Environment.Version.ToString(),
      EnvironmentVarGenerator
    ];
    // @formatter:on

    s_macroSlots = new Dictionary<string, int>( StringComparer.OrdinalIgnoreCase )
    {
      { NowMacroName, -1 },
      { UtcNowMacroName, -2 },
      { GuidMacroName, -3 },
      { MachineMacroName, -4 },
      { OsMacroName, -5 },
      { UserMacroName, -6 },
      { ClrVersionMacroName, -7 },
      { EnvMacroName, -8 }
    }.ToFrozenDictionary( StringComparer.OrdinalIgnoreCase );

#if NET9_0_OR_GREATER
    s_altMacroSlots = s_macroSlots.GetAlternateLookup<ReadOnlySpan<char>>();
#endif
  }

  #endregion

  #region Public Methods

  /// <summary>
  ///   Retrieves the names of all standard macros supported by the system.
  /// </summary>
  /// <returns>
  ///   An <see cref="IEnumerable{T}" /> of <see cref="string" /> containing the names of the standard macros.
  /// </returns>
  public static IEnumerable<string> GetStandardMacroNames()
  {
    return s_macroSlots.Keys;
  }

  /// <summary>
  ///   Retrieves the slot index associated with the specified macro name.
  /// </summary>
  /// <param name="macroName">The name of the macro whose slot index is to be retrieved.</param>
  /// <returns>
  ///   The slot index associated with the specified macro name. If the macro name is not found,
  ///   returns <see cref="MacroTable.MacroNotFoundSlot" />.
  /// </returns>
  /// <exception cref="ArgumentException">
  ///   Thrown when <paramref name="macroName" /> is <c>null</c> or an empty string.
  /// </exception>
  public static int GetSlot(
    string macroName )
  {
    if( string.IsNullOrEmpty( macroName ) )
    {
      throw new ArgumentException( "The macro name cannot be null or empty", nameof( macroName ) );
    }

    // netstandard2.0 does not have GetValueOrDefault
    // ReSharper disable once CanSimplifyDictionaryTryGetValueWithGetValueOrDefault
    if( s_macroSlots.TryGetValue( macroName, out var slot ) )
    {
      return slot;
    }

    return MacroTable.MacroNotFoundSlot;
  }

  /// <summary>
  ///   Retrieves the slot index associated with the specified macro name.
  /// </summary>
  /// <param name="macroName">
  ///   A <see cref="ReadOnlySpan{T}" /> of <see cref="char" /> representing the name of the macro.
  /// </param>
  /// <returns>
  ///   The slot index associated with the macro name if found; otherwise,
  ///   <see cref="MacroTable.MacroNotFoundSlot" />.
  /// </returns>
  public static int GetSlot(
    ReadOnlySpan<char> macroName )
  {
#if NET9_0_OR_GREATER
    return s_altMacroSlots.TryGetValue( macroName, out var slot )
      ? slot
      : MacroTable.MacroNotFoundSlot;
#else
    return GetSlot( macroName.ToString() );
#endif
  }

  /// <summary>
  ///   Retrieves the value associated with the specified macro name and optional argument.
  /// </summary>
  /// <param name="macroName">The name of the macro whose value is to be retrieved.</param>
  /// <param name="argument">
  ///   An optional argument to be used when generating the macro's value. Defaults to an empty span if not provided.
  /// </param>
  /// <returns>
  ///   A <see cref="string" /> representing the value of the specified macro. Returns <c>null</c> if the macro name
  ///   is not recognized or if the slot index is invalid.
  /// </returns>
  /// <exception cref="ArgumentException">
  ///   Thrown when <paramref name="macroName" /> is <c>null</c> or an empty string.
  /// </exception>
  public static string? GetValue(
    string macroName,
    ReadOnlySpan<char> argument = default )
  {
    var slot = GetSlot( macroName );
    return GetValue( slot, argument );
  }

  /// <summary>
  ///   Retrieves the value associated with the specified macro name and optional argument.
  /// </summary>
  /// <param name="macroName">
  ///   A <see cref="ReadOnlySpan{T}" /> of <see cref="char" /> representing the name of the macro.
  /// </param>
  /// <param name="argument">
  ///   An optional <see cref="ReadOnlySpan{T}" /> of <see cref="char" /> representing the argument for the macro.
  ///   Defaults to an empty span if not provided.
  /// </param>
  /// <returns>
  ///   A <see cref="string" /> containing the value of the macro if the macro name is recognized;
  ///   otherwise, <c>null</c>.
  /// </returns>
  public static string? GetValue(
    ReadOnlySpan<char> macroName,
    ReadOnlySpan<char> argument = default )
  {
    var slot = GetSlot( macroName );
    return GetValue( slot, argument );
  }

  /// <summary>
  ///   Retrieves the value of a macro based on its slot index and an optional argument.
  /// </summary>
  /// <param name="slot">
  ///   The slot index of the macro. Negative slot indices represent standard macros.
  /// </param>
  /// <param name="argument">
  ///   An optional argument provided as a <see cref="ReadOnlySpan{T}" /> of <see cref="char" />.
  ///   Defaults to an empty span if not specified.
  /// </param>
  /// <returns>
  ///   A <see cref="string" /> containing the value of the macro if the slot index is valid and within range;
  ///   otherwise, <see langword="null" />.
  /// </returns>
  /// <remarks>
  ///   If the slot index is non-negative or out of range, the method returns <see langword="null" />.
  ///   Standard macros are identified by negative slot indices, and their values are generated
  ///   using predefined generators.
  /// </remarks>
  public static string? GetValue(
    int slot,
    ReadOnlySpan<char> argument = default )
  {
    // Standard macros have negative slots.
    if( slot >= MacroTable.MacroNotFoundSlot )
    {
      return null;
    }

    var index = Math.Abs( slot ) - 1;

    // If the slot is out of range, return null.
    if( index >= s_generators.Length )
    {
      return null;
    }

    var generator = s_generators[index];
    return generator( argument );
  }

  #endregion

  #region Implementation

  private static string NowGenerator(
    ReadOnlySpan<char> arg )
  {
    var now = DateTime.Now;

    return arg.IsEmpty
      ? now.ToString( CultureInfo.CurrentCulture )
      : now.ToString( arg.ToString() );
  }

  private static string UtcNowGenerator(
    ReadOnlySpan<char> arg )
  {
    var utcNow = DateTime.UtcNow;

    return arg.IsEmpty
      ? utcNow.ToString( CultureInfo.CurrentCulture )
      : utcNow.ToString( arg.ToString() );
  }

  private static string GuidGenerator(
    ReadOnlySpan<char> arg )
  {
    return arg.IsEmpty ? Guid.NewGuid().ToString() : Guid.NewGuid().ToString( arg.ToString() );
  }

  private static string EnvironmentVarGenerator(
    ReadOnlySpan<char> arg )
  {
    return Environment.GetEnvironmentVariable( arg.ToString() ) ?? string.Empty;
  }

  #endregion
}
