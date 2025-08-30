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
  ///   Processes macros in a template and writes the result to a <see cref="TextWriter" />.
  /// </summary>
  /// <param name="template">The <see cref="Template" /> containing the macros to process.</param>
  /// <param name="writer">The <see cref="TextWriter" /> to write the processed template to.</param>
  /// <exception cref="InvalidOperationException">Thrown when an unknown segment kind is encountered.</exception>
  /// <remarks>
  ///   This method iterates through the segments of the provided template, processes macros, and writes the result to the
  ///   specified <see cref="TextWriter" />.
  /// </remarks>
  public static void ProcessMacros(
    Template template,
    TextWriter writer )
  {
    var context = template.Context;
    var macroDelimiter = context.CompilerOptions.MacroDelimiter;

    foreach( var segment in template.Segments )
    {
      switch( segment.Kind )
      {
        case SegmentKind.Macro:
        {
          string value;

          try
          {
            value = context.GetMacroValue( segment.ValueSlot, segment.ArgumentMemory.Span ) ?? segment.Text;
          }
          catch( Exception exception )
          {
            value = exception.Message;
          }

          writer.Write( value );
          break;
        }

        case SegmentKind.Delimiter:
          writer.Write( macroDelimiter );
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
  ///   Processes macros in a template and writes the result to a <see cref="StringBuilder" />.
  /// </summary>
  /// <param name="template">The template containing the macros to process.</param>
  /// <param name="builder">The <see cref="StringBuilder" /> to write the processed template to.</param>
  /// <exception cref="InvalidOperationException">Thrown when an unknown segment kind is encountered.</exception>
  public static void ProcessMacros(
    Template template,
    StringBuilder builder )
  {
    var context = template.Context;
    var macroDelimiter = context.CompilerOptions.MacroDelimiter;

    foreach( var segment in template.Segments )
    {
      switch( segment.Kind )
      {
        case SegmentKind.Macro:
        {
          string value;

          try
          {
            value = context.GetMacroValue( segment.ValueSlot, segment.ArgumentMemory.Span ) ?? string.Empty;
          }
          catch( Exception exception )
          {
            value = exception.Message;
          }

          builder.Append( value );

          break;
        }

        case SegmentKind.Delimiter:
          builder.Append( macroDelimiter );
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
