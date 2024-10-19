// Module Name: MacroProcessorBuilder.cs
// Author:      Eduardo Velasquez
// Copyright (c) 2024, Intercode Consulting, Inc.

namespace Intercode.Toolbox.TemplateEngine;

using System.Collections.Frozen;
using System.Diagnostics;
using System.Text;

/// <summary>
///   Creates a <see cref="MacroProcessor" /> instance.
/// </summary>
public class MacroProcessorBuilder: IDisposable
{
  #region Nested Types

  // NOTE: netstandard2.0 doesn't have the Range type, so we use a custom record instead.
  [DebuggerDisplay( "Start: {Start}, Length: {Length}" )]
  private readonly record struct StringRange(
    int Start,
    int End )
  {
    #region Properties

    public int Length => End - Start;

    #endregion
  };

  #endregion

  #region Fields

  private readonly Dictionary<string, StringRange> _macros = new ( StringComparer.OrdinalIgnoreCase );
  private readonly TemplateEngineOptions _options;
  private StringBuilder? _values = StringBuilderPool.Default.Get();

  #endregion

  #region Constructors

  /// <summary>
  ///   Initializes a new instance of the <see cref="MacroProcessorBuilder" /> class.
  /// </summary>
  /// <param name="options">
  ///   The template engine options. Will use <see cref="TemplateEngineOptions.Default" /> if <c>null</c>
  ///   .
  /// </param>
  public MacroProcessorBuilder(
    TemplateEngineOptions? options = null )
  {
    _options = options ?? TemplateEngineOptions.Default;
  }

  #endregion

  #region Public Methods

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
    if( _values is null )
    {
      throw new ObjectDisposedException( nameof( MacroProcessorBuilder ) );
    }

    if( string.IsNullOrWhiteSpace( name ) )
    {
      throw new ArgumentException( "Value cannot be null or whitespace.", nameof( name ) );
    }

    if( !IsAlphanumericOrUnderscore( name ) )
    {
      throw new ArgumentException( "Macro name must be alphanumeric or underscore.", nameof( name ) );
    }

    var start = _values.Length;
    _values.Append( value );
    _macros.Add( _options.Delimit( name ), new StringRange( start, _values.Length ) );

    return this;

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

  /// <summary>
  ///   Builds a <see cref="MacroProcessor" /> instance using the added macros and macro delimiter.
  /// </summary>
  /// <returns>The <see cref="MacroProcessor" /> instance.</returns>
  public MacroProcessor Build()
  {
    if( _values is null )
    {
      throw new ObjectDisposedException( nameof( MacroProcessorBuilder ) );
    }

    // All the macro values are stored in a single string, so we can use ReadOnlyMemory<char> to reference them.
    // The ReadOnlyMemory dictionary values keep the underlying string alive, so we don't need to worry about
    // storing the values variable.
    var values = _values.ToString();

    var macros = _macros.ToFrozenDictionary(
      static pair => pair.Key,
      pair => values.AsMemory( pair.Value.Start, pair.Value.Length ),
      StringComparer.OrdinalIgnoreCase
    );

    Dispose();

    return new MacroProcessor( macros, _options );
  }

  /// <summary>
  ///   Releases the resources used by the <see cref="MacroProcessorBuilder" />.
  /// </summary>
  public void Dispose()
  {
    if( _values == null )
    {
      return;
    }

    StringBuilderPool.Default.Return( _values );
    _values = null;
  }

  #endregion
}
