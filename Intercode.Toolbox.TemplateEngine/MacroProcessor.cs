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
  /// <param name="writer">The <see cref="TextWriter" /> to which the processed template will be written.</param>
  /// <param name="macroValues">The <see cref="MacroValues" /> providing values for the macros in the template.</param>
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
    this Template template,
    TextWriter writer,
    MacroValues macroValues )
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

      if( segment.IsMacro )
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

        writer.Write( value );
        continue;
      }

#if NET6_0_OR_GREATER
      writer.Write( segment.Memory.Span );
#else

      // The .netstandard2.0 TextWriter.Write method does not have a Span overload.
      writer.Write( segment.Memory.ToString() );
#endif
    }
  }

  /// <summary>
  ///   Processes macros in a template and writes the result to a <see cref="StringBuilder" />.
  /// </summary>
  /// <param name="template">The template containing the macros to process.</param>
  /// <param name="builder">The <see cref="StringBuilder" /> to write the processed template to.</param>
  /// <param name="macroValues">The macro values to use for processing the template.</param>
  /// <exception cref="ArgumentException">
  ///   Thrown when the <paramref name="macroValues" /> instance is not associated with the same
  ///   <see cref="MacroTable" /> as the <paramref name="template" />.
  /// </exception>
  /// <exception cref="InvalidOperationException">
  ///   Thrown when an unknown segment kind is encountered during processing.
  /// </exception>
  public static void ProcessMacros(
    this Template template,
    StringBuilder builder,
    MacroValues macroValues )
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

      if( segment.IsMacro )
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
        continue;
      }

#if NET6_0_OR_GREATER
      builder.Append( segment.Memory.Span );
#else

      // The .netstandard2.0 StringBuilder.Append method does not have a Span overload.
      builder.Append( segment.Memory.ToString() );
#endif
    }
  }

  /// <summary>
  ///   Processes macros in the specified <see cref="Template" /> and returns the resulting string.
  /// </summary>
  /// <param name="template">The <see cref="Template" /> containing the macros to process.</param>
  /// <param name="macroValues">The <see cref="MacroValues" /> providing values for the macros in the template.</param>
  /// <returns>A string with the macros in the template replaced by their corresponding values.</returns>
  /// <remarks>
  ///   This method processes the macros in the provided template using the specified macro values.
  ///   It utilizes a pooled <see cref="StringBuilder" /> for efficient string manipulation.
  /// </remarks>
  /// <exception cref="ArgumentNullException">
  ///   Thrown if <paramref name="template" /> or <paramref name="macroValues" /> is <c>null</c>.
  /// </exception>
  public static string ProcessMacros(
    this Template template,
    MacroValues macroValues )
  {
    var builder = StringBuilderPool.Default.Get();

    try
    {
      ProcessMacros( template, builder, macroValues );
      return builder.ToString();
    }
    finally
    {
      StringBuilderPool.Default.Return( builder );
    }
  }

  /// <summary>
  ///   Processes macros in a template and writes the result to a <see cref="StringBuilder" />.
  /// </summary>
  /// <param name="template">The template containing the macros to process.</param>
  /// <param name="builder">The <see cref="StringBuilder" /> to write the processed template to.</param>
  /// <param name="values">
  ///   A span of string values to replace the macros in the template. The array must have at least as many elements
  ///   as the <see cref="MacroTable" /> associated with the template.
  /// </param>
  /// <exception cref="ArgumentException">
  ///   Thrown when the <paramref name="values" /> array has fewer elements than the <see cref="MacroTable" /> requires.
  /// </exception>
  public static void ProcessMacros(
    this Template template,
    StringBuilder builder,
    params ReadOnlySpan<string?> values )
  {
    if( values.Length < template.MacroTable.Count )
    {
      throw new ArgumentException(
        "The values array must have at least as many elements as the MacroTable has slots.",
        nameof( values )
      );
    }

    // Pre-allocate the StringBuilder capacity to avoid multiple allocations during appends
    builder.EnsureCapacity( template.TemplateTextLength );

    for( var index = 0; index < template.Segments.Length; index++ )
    {
      var segment = template.Segments[index];

      if( segment.IsMacro )
      {
        var value = values[segment.Slot] ?? string.Empty;
        builder.Append( value );
        continue;
      }

#if NET6_0_OR_GREATER
      builder.Append( segment.Memory.Span );
#else

      // The .netstandard2.0 StringBuilder.Append method does not have a Span overload.
      builder.Append( segment.Memory.ToString() );
#endif
    }
  }

  /// <summary>
  ///   Processes macros in a template and writes the result to a <see cref="StringBuilder" />.
  /// </summary>
  /// <param name="template">The template containing the macros to process.</param>
  /// <param name="builder">The <see cref="StringBuilder" /> to write the processed template to.</param>
  /// <param name="values">
  ///   An array of string values to replace the macros in the template. The array must have at least as many elements
  ///   as the <see cref="MacroTable" /> associated with the template.
  /// </param>
  /// <exception cref="ArgumentException">
  ///   Thrown when the <paramref name="values" /> array has fewer elements than the <see cref="MacroTable" /> requires.
  /// </exception>
  public static void ProcessMacros(
    this Template template,
    StringBuilder builder,
    params string?[] values )
  {
    template.ProcessMacros( builder, values.AsSpan() );
  }

  /// <summary>
  ///   Processes macros in the specified <see cref="Template" /> using the provided values and returns the result as a
  ///   string.
  /// </summary>
  /// <param name="template">The <see cref="Template" /> containing the macros to process.</param>
  /// <param name="values">
  ///   An array of string values to replace the macros in the template. The array must have at least as many elements
  ///   as the <see cref="MacroTable" /> associated with the template.
  /// </param>
  /// <exception cref="ArgumentException">
  ///   Thrown when the <paramref name="values" /> array has fewer elements than the <see cref="MacroTable" /> requires.
  /// </exception>
  public static string ProcessMacros(
    this Template template,
    params ReadOnlySpan<string?> values )
  {
    var builder = StringBuilderPool.Default.Get();

    try
    {
      ProcessMacros( template, builder, values );
      return builder.ToString();
    }
    finally
    {
      StringBuilderPool.Default.Return( builder );
    }
  }

  /// <summary>
  ///   Processes macros in the specified <see cref="Template" /> using the provided values and writes the result to the
  ///   specified <see cref="StringBuilder" />.
  /// </summary>
  /// <param name="template">The <see cref="Template" /> containing the macros to process.</param>
  /// <param name="values">
  ///   An array of string values to replace the macros in the template. The array must have at least as many elements
  ///   as the <see cref="MacroTable" /> associated with the template.
  /// </param>
  /// <exception cref="ArgumentException">
  ///   Thrown when the <paramref name="values" /> array has fewer elements than the <see cref="MacroTable" /> requires.
  /// </exception>
  public static string ProcessMacros(
    this Template template,
    params string?[] values )
  {
    return template.ProcessMacros( values.AsSpan() );
  }

  #endregion
}
