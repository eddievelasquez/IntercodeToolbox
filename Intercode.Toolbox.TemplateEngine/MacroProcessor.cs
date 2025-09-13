// Module Name: MacroProcessor.cs
// Author:      Eduardo Velasquez
// Copyright (c) 2025, Intercode Consulting, Inc.

namespace Intercode.Toolbox.TemplateEngine;

using System.Text;

/// <summary>
///   Processes macros in a template.
/// </summary>
public static class MacroProcessor
{
  #region Public Methods

  /// <summary>
  ///   Processes macros in the specified <see cref="Template" /> and writes the result to the provided
  ///   <see cref="TextWriter" />.
  /// </summary>
  /// <param name="template">The <see cref="Template" /> containing the macros to process.</param>
  /// <param name="macroValues">The <see cref="MacroValues" /> providing values for the macros in the template.</param>
  /// <param name="writer">The <see cref="TextWriter" /> to which the processed template will be written.</param>
  /// <exception cref="ArgumentException">
  ///   Thrown when the <paramref name="macroValues" /> is not associated with the same <see cref="MacroTable" /> as the
  ///   <paramref name="template" />.
  /// </exception>
  /// <exception cref="InvalidOperationException">Thrown when an unknown segment kind is encountered during processing.</exception>
  /// <remarks>
  ///   This method iterates through the segments of the provided <see cref="Template" />, processes macros, and writes the
  ///   result to the specified <see cref="TextWriter" />.
  /// </remarks>
  public static void ProcessMacros(
    Template template,
    MacroValues macroValues,
    TextWriter writer )
  {
    if( template.MacroTable != macroValues.MacroTable )
    {
      throw new ArgumentException(
        "The MacroValues instance must be associated with the same MacroTable as the Template.",
        nameof( macroValues )
      );
    }

    for( var index = 0; index < template.Segments.Length; index++ )
    {
      var segment = template.Segments[index];

      switch( segment.Kind )
      {
        case SegmentKind.Macro:
        {
          string value;

          try
          {
            value = macroValues.GetValue( segment.Slot, segment.ArgumentMemory.Span ) ?? segment.Text;
          }
          catch( Exception exception )
          {
            value = exception.Message;
          }

          writer.Write( value );
          break;
        }

        case SegmentKind.Delimiter:
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
  ///   Processes macros in a template and writes the result to a <see cref="StringBuilder" />.
  /// </summary>
  /// <param name="template">The template containing the macros to process.</param>
  /// <param name="macroValues">The macro values to use for processing the template.</param>
  /// <param name="builder">The <see cref="StringBuilder" /> to write the processed template to.</param>
  /// <exception cref="ArgumentException">
  ///   Thrown when the <paramref name="macroValues" /> instance is not associated with the same
  ///   <see cref="MacroTable" /> as the <paramref name="template" />.
  /// </exception>
  /// <exception cref="InvalidOperationException">
  ///   Thrown when an unknown segment kind is encountered during processing.
  /// </exception>
  public static void ProcessMacros(
    Template template,
    MacroValues macroValues,
    StringBuilder builder )
  {
    if( template.MacroTable != macroValues.MacroTable )
    {
      throw new ArgumentException(
        "The MacroValues instance must be associated with the same MacroTable as the Template.",
        nameof( macroValues )
      );
    }

    // Pre-allocate the StringBuilder capacity to avoid multiple allocations during appends
    builder.EnsureCapacity( template.TemplateTextLength );

    for( var index = 0; index < template.Segments.Length; index++ )
    {
      var segment = template.Segments[index];

      switch( segment.Kind )
      {
        case SegmentKind.Macro:
        {
          string value;

          try
          {
            value = macroValues.GetValue( segment.Slot, segment.ArgumentMemory.Span ) ?? string.Empty;
          }
          catch( Exception exception )
          {
            value = exception.Message;
          }

          builder.Append( value );

          break;
        }

        case SegmentKind.Delimiter:
        case SegmentKind.Constant:
#if NET6_0_OR_GREATER
          builder.Append( segment.Memory.Span );
#else
          // The .netstandard2.0 StringBuilder.Append method does not have a Span overload.
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
