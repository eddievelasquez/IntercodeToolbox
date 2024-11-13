// Module Name: MacroProcessor.cs
// Author:      Eduardo Velasquez
// Copyright (c) 2024, Intercode Consulting, Inc.

namespace Intercode.Toolbox.TemplateEngine;

using System.Collections.Frozen;
using System.Text;

/// <summary>
///   Processes macros in a template.
/// </summary>
public class MacroProcessor
{
  #region Fields

  private readonly FrozenDictionary<string, MacroValueGenerator> _valueGenerators;
  private readonly TemplateEngineOptions _options;

#if NET9_0_OR_GREATER
  private readonly FrozenDictionary<string, MacroValueGenerator>.AlternateLookup<ReadOnlySpan<char>> _alternate;
#endif

  #endregion

  #region Constructors

  /// <summary>
  ///   Initializes a new instance of the <see cref="MacroProcessor" /> class.
  /// </summary>
  /// <param name="valueGenerators">Macros with their corresponding value generators.</param>
  /// <param name="options">The Template Engine options.</param>
  /// <remarks>
  ///   The macro names in the <paramref name="valueGenerators" /> dictionary come surrounded by the delimiter character.
  /// </remarks>
  internal MacroProcessor(
    FrozenDictionary<string, MacroValueGenerator> valueGenerators,
    TemplateEngineOptions options )
  {
    _valueGenerators = valueGenerators;
    _options = options;

#if NET9_0_OR_GREATER
    _alternate = _valueGenerators.GetAlternateLookup<ReadOnlySpan<char>>();
#endif
  }

  #endregion

  #region Properties

  /// <summary>
  ///   Gets the number of registered macros.
  /// </summary>
  public int MacroCount => _valueGenerators.Count;

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
    return _valueGenerators.TryGetValue( macroName, out var generator ) ? generator( ReadOnlySpan<char>.Empty ) : null;
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
    return _valueGenerators.TryGetValue( macroName, out var generator ) ? generator( argument ) : null;
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
      switch( segment.Kind )
      {
        case SegmentKind.Macro:
        {
#if NET9_0_OR_GREATER
          if( _alternate.TryGetValue( segment.Memory.Span, out var generator ) )
#else
          if( _valueGenerators.TryGetValue( segment.Text, out var generator ) )
#endif
          {
            string value;

            try
            {
              value = generator( segment.ArgumentMemory.Span );
            }
            catch( Exception exception )
            {
              value = exception.Message;
            }

            writer.Write( value );
          }

          break;
        }

        case SegmentKind.Delimiter:
          writer.Write( _options.MacroDelimiter );
          break;

        case SegmentKind.Constant:
#if NET6_0_OR_GREATER
          writer.Write( segment.Memory.Span );
#else

          // The .netstandard2.0 TextWriter.Write method does not have a Span overload.
          writer.Write( segment.Memory.ToString() );
#endif
          break;

        default:
          throw new InvalidOperationException( "Unknown segment kind" );
      }
    }
  }

  /// <summary>
  ///   Processes macros in a template and writes the result to a <see cref="TextWriter" />.
  /// </summary>
  /// <param name="template">The template to process.</param>
  /// <param name="builder">The <see cref="Stream" /> to write the processed template to.</param>
  public void ProcessMacros(
    Template template,
    StringBuilder builder )
  {
    foreach( var segment in template.Segments )
    {
      switch( segment.Kind )
      {
        case SegmentKind.Macro:
        {
#if NET9_0_OR_GREATER
          if( _alternate.TryGetValue( segment.Memory.Span, out var generator ) )
#else
          if( _valueGenerators.TryGetValue( segment.Text, out var generator ) )
#endif
          {
            string value;

            try
            {
              value = generator( segment.ArgumentMemory.Span );
            }
            catch( Exception exception )
            {
              value = exception.Message;
            }

            builder.Append( value );
          }

          break;
        }

        case SegmentKind.Delimiter:
          builder.Append( _options.MacroDelimiter );
          break;

        case SegmentKind.Constant:
#if NET6_0_OR_GREATER
          builder.Append( segment.Memory.Span );
#else

          // The .netstandard2.0 TextWriter.Write method does not have a Span overload.
          builder.Append( segment.Memory.ToString() );
#endif
          break;

        default:
          throw new InvalidOperationException( "Unknown segment kind" );
      }
    }
  }

  #endregion
}
