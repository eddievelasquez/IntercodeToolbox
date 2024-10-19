// Module Name: MacroProcessor.cs
// Author:      Eduardo Velasquez
// Copyright (c) 2024, Intercode Consulting, Inc.

namespace Intercode.Toolbox.TemplateEngine;

using System.Collections.Frozen;
using System.Collections.ObjectModel;

/// <summary>
///   Processes macros in a template.
/// </summary>
public class MacroProcessor
{
  #region Fields

  private readonly FrozenDictionary<string, ReadOnlyMemory<char>> _macros;
  private readonly TemplateEngineOptions _options;

  #endregion

  #region Constructors

  /// <summary>
  ///   Initializes a new instance of the <see cref="MacroProcessor" /> class.
  /// </summary>
  /// <param name="macros">The dictionary of macro names and values.</param>
  /// <param name="options">The Template Engine options.</param>
  /// <remarks>
  ///   * The macro names in the <paramref name="macros" /> dictionary come surrounded by the delimiter character.<br />
  ///   * The <see cref="ReadOnlyMemory{T}" /> values in the dictionary keep the underlying string alive.
  /// </remarks>
  internal MacroProcessor(
    FrozenDictionary<string, ReadOnlyMemory<char>> macros,
    TemplateEngineOptions options )
  {
    _macros = macros;
    _options = options;
  }

  #endregion

  #region Properties

  /// <summary>
  ///   Gets the number of registered macros.
  /// </summary>
  public int MacroCount => _macros.Count;

  #endregion

  #region Public Methods

  /// <summary>
  ///   Gets all the registered macros as a read-only dictionary.
  /// </summary>
  /// <returns>A read-only dictionary containing the registered macros.</returns>
  public IReadOnlyDictionary<string, string> GetMacros()
  {
    return new ReadOnlyDictionary<string, string>(
      _macros.ToDictionary(
        static pair => pair.Key,
        static pair => pair.Value.ToString(),
        StringComparer.OrdinalIgnoreCase
      )
    );
  }

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
    var key = _options.Delimit( macroName );
    return _macros.TryGetValue( key, out var macroValue ) ? macroValue.ToString() : null;
  }

  /// <summary>
  ///   Gets the value of a macro as a <see cref="ReadOnlyMemory{T}" /> or <c>null</c> if not found.
  /// </summary>
  /// <param name="macroName">The name of the macro. The macro name used here does not include the delimiter character.</param>
  /// <returns>The value of the macro as a <see cref="ReadOnlyMemory{T}" />, or null if the macro does not exist.</returns>
  public ReadOnlyMemory<char>? GetMacroValueMemory(
    string macroName )
  {
    var key = _options.Delimit( macroName );
    return _macros.TryGetValue( key, out var macroValue ) ? macroValue : null;
  }

  /// <summary>
  ///   Processes macros in a template and writes the result to a <see cref="TextWriter" />.
  /// </summary>
  /// <param name="template">The template to process.</param>
  /// <param name="writer">The <see cref="TextWriter" /> to write the processed template to.</param>
  public void ProcessMacros(
    Template template,
    TextWriter writer )
  {
    foreach( var segment in template.Segments )
    {
      if( segment.IsMacro )
      {
        // Unfortunately we cannot use a Span for the macro lookup as Dictionary does not
        // yet Span lookup support; but .NET 9.0 does.
        // see https://blog.ndepend.com/alternate-lookup-for-dictionary-and-hashset-in-net-9/
        var macroName = segment.Text;
        if( _macros.TryGetValue( macroName, out var macroValue ) )
        {
#if NET6_0_OR_GREATER
          writer.Write( macroValue.Span );
#else

          // The .netstandard2.0 TextWriter.Write method does not have a Span overload.
          writer.Write( macroValue.ToString() );
#endif
        }
      }
      else
      {
#if NET6_0_OR_GREATER
        writer.Write( segment.Memory.Span );
#else

        // The .netstandard2.0 TextWriter.Write method does not have a Span overload.
        writer.Write( segment.Memory.ToString() );
#endif
      }
    }
  }

  #endregion
}
