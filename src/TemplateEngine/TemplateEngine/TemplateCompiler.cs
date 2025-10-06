// Module Name: TemplateCompiler.cs
// Author:      Eduardo Velasquez
// Copyright (c) 2025, Intercode Consulting, Inc.

namespace Intercode.Toolbox.TemplateEngine;

/// <summary>
///   Provides static methods to compile template text into a <see cref="Template" /> instance.
///   Thread-safety: This class is stateless and thread-safe.
///   Complexity: Compilation is O(n) where n is the length of the template text.
/// </summary>
public static class TemplateCompiler
{
  #region Public Methods

  /// <summary>
  ///   Compiles the specified template text into a <see cref="Template" /> using the provided macro table and optional
  ///   includes.
  /// </summary>
  /// <param name="text">
  ///   The text of the template to compile. Must not be <c>null</c>, empty, or consist only of whitespace.
  /// </param>
  /// <param name="macroTable">
  ///   The <see cref="MacroTable" /> containing macro definitions used during compilation.
  /// </param>
  /// <param name="includes">
  ///   An optional <see cref="IncludesCollection" /> containing additional content to include in the template.
  /// </param>
  /// <param name="options">
  ///   Optional <see cref="TemplateCompilerOptions" /> to customize macro delimiters and argument separators. If <c>null</c>
  ///   , defaults are used.
  /// </param>
  /// <returns>
  ///   A <see cref="Template" /> instance representing the compiled template.
  /// </returns>
  /// <exception cref="ArgumentNullException">
  ///   Thrown if <paramref name="macroTable" /> is <c>null</c>.
  /// </exception>
  /// <exception cref="ArgumentException">
  ///   Thrown if <paramref name="text" /> is <c>null</c>, empty, or consists only of whitespace.
  /// </exception>
  /// <remarks>
  ///   This method processes includes and splits the template into segments for efficient macro expansion.
  /// </remarks>
  public static Template Compile(
    string text,
    MacroTable macroTable,
    IncludesCollection? includes = null,
    TemplateCompilerOptions? options = null )
  {
    if( macroTable is null )
    {
      throw new ArgumentNullException( nameof( macroTable ) );
    }

    return CompileImpl( text, macroTable, null, includes, options );
  }

  /// <summary>
  ///   Compiles the specified template text into a <see cref="Template" /> using a new <see cref="MacroTableBuilder" /> and
  ///   optional includes.
  /// </summary>
  /// <param name="text">The text of the template to compile.</param>
  /// <param name="includes">An optional <see cref="IncludesCollection" /> for additional content.</param>
  /// <param name="options">Optional <see cref="TemplateCompilerOptions" /> for customization.</param>
  /// <returns>A <see cref="Template" /> instance representing the compiled template.</returns>
  /// <exception cref="ArgumentException">Thrown if <paramref name="text" /> is <c>null</c>, empty, or whitespace.</exception>
  /// <remarks>
  ///   This overload creates a new macro table builder for macro declarations during compilation.
  /// </remarks>
  public static Template Compile(
    string text,
    IncludesCollection? includes = null,
    TemplateCompilerOptions? options = null )
  {
    return CompileImpl( text, null, new MacroTableBuilder(), includes, options );
  }

  /// <summary>
  ///   Compiles the specified template text into a <see cref="Template" /> using the provided
  ///   <see cref="MacroTableBuilder" /> and optional includes.
  /// </summary>
  /// <param name="text">The text of the template to compile.</param>
  /// <param name="builder">The <see cref="MacroTableBuilder" /> for macro declarations.</param>
  /// <param name="includes">An optional <see cref="IncludesCollection" /> for additional content.</param>
  /// <param name="options">Optional <see cref="TemplateCompilerOptions" /> for customization.</param>
  /// <returns>A <see cref="Template" /> instance representing the compiled template.</returns>
  /// <exception cref="ArgumentException">Thrown if <paramref name="text" /> is <c>null</c>, empty, or whitespace.</exception>
  /// <remarks>
  ///   This overload allows macro declarations to be tracked and reused via the provided builder.
  /// </remarks>
  public static Template Compile(
    string text,
    MacroTableBuilder builder,
    IncludesCollection? includes = null,
    TemplateCompilerOptions? options = null )
  {
    if( builder is null )
    {
      throw new ArgumentNullException( nameof( builder ) );
    }

    return CompileImpl( text, null, builder, includes, options );
  }

  #endregion

  #region Implementation

  private static Template CompileImpl(
    string text,
    MacroTable? macroTable,
    MacroTableBuilder? macroTableBuilder,
    IncludesCollection? includes,
    TemplateCompilerOptions? options )
  {
    if( string.IsNullOrWhiteSpace( text ) )
    {
      throw new ArgumentException(
        "The template's text cannot be null, empty, or whitespace.",
        nameof( text )
      );
    }

    // Use default options if none were provided
    options ??= TemplateCompilerOptions.Default;

    // Process includes if any were provided
    if( includes?.Count > 0 )
    {
      text = ProcessIncludes( text, includes, options );
    }

    var segments = SplitIntoSegments( macroTable, macroTableBuilder, text, options );

    // Create an empty constant segment if we ended up with no segments
    if( segments.Length == 0 )
    {
      segments = [Segment.Empty];
    }

    macroTable ??= macroTableBuilder!.Build();
    return new Template( text, macroTable, segments );
  }

  private static string ProcessIncludes(
    string text,
    IncludesCollection includes,
    TemplateCompilerOptions options )
  {
    var builder = StringBuilderPool.Default.Get();

    try
    {
      // Ensure the builder has at least enough capacity to hold the pre-processed text
      builder.Capacity = Math.Max( builder.Capacity, text.Length );

      var delimiter = options.MacroDelimiter;
      var segmentStart = 0;

      while( segmentStart < text.Length )
      {
        var macroStart = text.IndexOf( delimiter, segmentStart );

        if( macroStart == -1 )
        {
          // No more macros found, add the remaining text and exit
          builder.Append( text, segmentStart, text.Length - segmentStart );
          break;
        }

        // We might have found a macro placeholder
        var macroEnd = text.IndexOf( delimiter, macroStart + 1 );

        if( macroEnd == -1 )
        {
          // No closing delimiter found, append the rest of the text and exit
          builder.Append( text, segmentStart, text.Length - segmentStart );
          break;
        }

        // Flush any constant text before the macro placeholder
        if( segmentStart < macroStart )
        {
          builder.Append( text, segmentStart, macroStart - segmentStart );
        }

        // An empty macro name means we found an escaped delimiter.
        if( macroEnd == macroStart + 1 )
        {
          builder.Append( delimiter );
          builder.Append( delimiter );
        }
        else
        {
          // Attempt to get the include content
          var macroName = text.AsSpan().Slice( macroStart + 1, macroEnd - macroStart - 1 );

          if( TryGetIncludeContent( macroName, out var content ) )
          {
            // The macro represented an include, so append its content or an empty string if null
            builder.Append( content ?? string.Empty );
          }
          else
          {
            // The macro doesn't represent an include, so just append the original macro placeholder
            builder.Append( text, macroStart, macroEnd - macroStart + 1 );
          }
        }

        // Skip past the macro placeholder
        segmentStart = macroEnd + 1;
      }

      return builder.ToString();
    }
    finally
    {
      StringBuilderPool.Default.Return( builder );
    }

    // Attempts to retrieve include content by macro name (span or string depending on TFM)
    bool TryGetIncludeContent(
      ReadOnlySpan<char> name,
      out string? content )
    {
#if NET9_0_OR_GREATER
      return includes.TryGetIncludeContent( name, out content );
#else
      return includes.TryGetIncludeContent( name.ToString(), out content );
#endif
    }
  }

  private static Segment[] SplitIntoSegments(
    MacroTable? macroTable,
    MacroTableBuilder? macroTableBuilder,
    string text,
    TemplateCompilerOptions options )
  {
#if NET9_0_OR_GREATER
    Func<ReadOnlySpan<char>, int> getSlot =
      macroTable != null ? macroTable.GetSlot : macroTableBuilder!.DeclareInternal;
#else
    Func<string, int> getSlot =
      macroTable != null ? macroTable.GetSlot : macroTableBuilder!.DeclareInternal;
#endif

    var segments = new List<Segment>();
    var delimiter = options.MacroDelimiter;
    var segmentStart = 0;

    while( segmentStart < text.Length )
    {
      var macroStart = text.IndexOf( delimiter, segmentStart );

    Process:

      if( macroStart == -1 )
      {
        // No more macro placeholders were found, add the remaining text
        // as a constant segment and exit
        AddConstantSegment( segmentStart, text.Length - segmentStart );
        break;
      }

      // We might have found a macro placeholder
      var macroEnd = text.IndexOf( delimiter, macroStart + 1 );

      if( macroEnd == -1 )
      {
        // No closing delimiter found, treat the rest of the text as a constant segment
        // and exit
        AddConstantSegment( segmentStart, text.Length - segmentStart );
        break;
      }

      // An empty macro name means we found an escaped delimiter.
      if( macroEnd == macroStart + 1 )
      {
        // Is the escaped delimiter is at the start of the segment?
        if( segmentStart == macroStart )
        {
          // The segment starts with the closing delimiter, so we skip it
          // and look for the next macro placeholder
          segmentStart = macroEnd;
          macroStart = text.IndexOf( delimiter, macroEnd + 1 );

          // NOTE: First time I've used a 'goto' in C# code. But this is a valid use case,
          // as it simplifies the logic and avoids deep nesting.
          goto Process;
        }

        // The escaped delimiter is at end of the constant segment
        AddConstantSegment( segmentStart, macroStart - segmentStart + 1 );
        segmentStart = macroEnd + 1;
        continue;
      }

      // Flush any constant text before the macro placeholder
      if( segmentStart < macroStart )
      {
        AddConstantSegment( segmentStart, macroStart - segmentStart );
      }

      // We found a macro placeholder, so add it as a macro segment
      // NOTE: The opening and closing delimiter chars are excluded
      AddMacroSegment( macroStart + 1, macroEnd - macroStart - 1 );

      // Skip past the macro
      segmentStart = macroEnd + 1;
    }

    return segments.ToArray();

    // Adds a constant segment to the segments list
    void AddConstantSegment(
      int start,
      int length )
    {
      segments.Add( Segment.CreateConstant( start, length ) );
    }

    // Adds a macro segment to the segments list, handling arguments and slot lookup
    void AddMacroSegment(
      int start,
      int length )
    {
      var name = text.AsSpan( start, length );
      var argStart = name.IndexOf( options.ArgumentSeparator );
      var argLength = 0;

      // Does the macro placeholder have an argument?
      if( argStart != -1 )
      {
        var argument = name.Slice( argStart + 1 );
        name = name.Slice( 0, argStart );

        if( name.IsEmpty )
        {
          throw new InvalidOperationException( "The macro name cannot be empty" );
        }

        length = name.Length;

        // Account for the delimiter, and it is relative to the macros placeholder start
        argStart += start + 1;
        argLength = argument.Length;
      }

#if NET9_0_OR_GREATER
      var slot = getSlot( name );
#else
      var slot = getSlot( name.ToString() );
#endif

      if( slot == MacroTable.MacroNotFoundSlot )
      {
        throw new InvalidOperationException( $"Undefined macro: '{name.ToString()}'" );
      }

      segments.Add( Segment.CreateMacro( start, length, argStart, argLength, slot ) );
    }
  }

  #endregion
}
