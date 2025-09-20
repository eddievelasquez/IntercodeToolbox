// Module Name: TemplateCompiler.cs
// Author:      Eduardo Velasquez
// Copyright (c) 2025, Intercode Consulting, Inc.

namespace Intercode.Toolbox.TemplateEngine;

/// <summary>
///   Compiles a template text into a <see cref="Template" />.
/// </summary>
public static class TemplateCompiler
{
  #region Public Methods

  /// <summary>
  ///   Compiles the specified template text into a <see cref="Template" />.
  /// </summary>
  /// <param name="macroTable">
  ///   The <see cref="MacroTable" /> containing macro definitions used during compilation.
  /// </param>
  /// <param name="templateText">
  ///   The text of the template to compile. Must not be <c>null</c>, empty, or consist only of whitespace.
  /// </param>
  /// <param name="includes">
  ///   An optional <see cref="IncludesCollection" /> containing additional content to include in the template.
  /// </param>
  /// <param name="options"></param>
  /// <returns>
  ///   A <see cref="Template" /> instance representing the compiled template.
  /// </returns>
  /// <exception cref="ArgumentNullException">
  ///   Thrown if <paramref name="macroTable" /> is <c>null</c>.
  /// </exception>
  /// <exception cref="ArgumentException">
  ///   Thrown if <paramref name="templateText" /> is <c>null</c>, empty, or consists only of whitespace.
  /// </exception>
  public static Template Compile(
    MacroTable macroTable,
    string templateText,
    IncludesCollection? includes = null,
    TemplateCompilerOptions? options = null )
  {
    if( macroTable == null )
    {
      throw new ArgumentNullException( nameof( macroTable ) );
    }

    if( string.IsNullOrWhiteSpace( templateText ) )
    {
      throw new ArgumentException( "The template's text cannot be null, empty, or whitespace.", nameof( templateText ) );
    }

    options ??= TemplateCompilerOptions.Default;

    if( includes?.Count > 0 )
    {
      templateText = ProcessIncludes( templateText.AsSpan(), includes, options );
    }

    var segments = SplitIntoSegments( macroTable, templateText.AsMemory(), options );

    // Create an empty constant segment if we ended up with no segments
    if( segments.Length == 0 )
    {
      segments = [Segment.CreateConstant( 0, ReadOnlyMemory<char>.Empty )];
    }

    return new Template( macroTable, segments, templateText.Length );
  }

  #endregion

  #region Implementation

  private static string ProcessIncludes(
    ReadOnlySpan<char> text,
    IncludesCollection includes,
    TemplateCompilerOptions options )
  {
    var builder = StringBuilderPool.Default.Get();

    try
    {
      // Ensure the builder has at least enough capacity to hold the pre-processed text
      builder.Capacity = Math.Max( builder.Capacity, text.Length );
      var currentIndex = 0;

      while( currentIndex < text.Length )
      {
        // Look for the position of the next macro opening delimiter
        var macroStart = text.Slice( currentIndex ).IndexOf( options.MacroDelimiter );

        // No more macros found, add the remaining text as a constant segment
        if( macroStart == -1 )
        {
          Append( text.Slice( currentIndex ) );
          break;
        }

        macroStart += currentIndex;

        // Add the text before the macro delimiter (if any) as a constant segment
        if( macroStart > currentIndex )
        {
          Append( text.Slice( currentIndex, macroStart - currentIndex ) );
        }

        // Look for the position of closing macro delimiter
        var macroEnd = text.Slice( macroStart + 1 ).IndexOf( options.MacroDelimiter );

        if( macroEnd == -1 )
        {
          // No closing delimiter found, treat the rest of the text as a constant segment
          Append( text.Slice( macroStart ) );
          break;
        }

        macroEnd += macroStart + 1;

        // Add the macro segment (The delimiters will be excluded)
        if( macroEnd == macroStart + 1 )
        {
          // An empty macro name means we found an escaped delimiter
          builder.Append( options.MacroDelimiter );
          builder.Append( options.MacroDelimiter );
        }
        else
        {
          // Attempt to get the include content
          var macroName = text.Slice( macroStart + 1, macroEnd - macroStart - 1 );

          if( TryGetIncludeContent( macroName, out var content ) )
          {
            // The macro represented an include, so append its content or an empty string if null
            builder.Append( content ?? string.Empty );
          }
          else
          {
            // The macro doesn't represent an include, so just append the original macro block
            var macroBlock = text.Slice( macroStart, macroEnd - macroStart + 1 );
            Append( macroBlock );
          }
        }

        // Move the current index after the closing delimiter index
        currentIndex = macroEnd + 1;
      }

      return builder.ToString();
    }
    finally
    {
      StringBuilderPool.Default.Return( builder );
    }

    void Append(
      ReadOnlySpan<char> span )
    {
#if NET6_0_OR_GREATER
      builder.Append( span );
#else
      builder.Append( span.ToString() );
#endif
    }

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
    MacroTable macroTable,
    ReadOnlyMemory<char> templateText,
    TemplateCompilerOptions options )
  {
    var segments = new List<Segment>();
    var span = templateText.Span;
    var currentPos = 0;

    // TODO: Consider a streaming approach to constants to avoid having to merge segments
    //       i.e. keeping a running constant span that gets flushed only when determining
    //       that the segment won't start or end with a delimiter
    while( currentPos < templateText.Length )
    {
      // Look for the position of the opening macro delimiter
      var macroStartPos = span.Slice( currentPos ).IndexOf( options.MacroDelimiter );

      // We're done, no more macros were found, add the
      // remaining text as a constant segment
      if( macroStartPos == -1 )
      {
        AddConstantSegment( currentPos, templateText.Length - currentPos );
        break;
      }

      macroStartPos += currentPos;

      // Add the text before the macro delimiter (if any) as a constant segment
      if( macroStartPos > currentPos )
      {
        AddConstantSegment( currentPos, macroStartPos - currentPos );
      }

      // Look for the closing macro delimiter
      var macroEndPos = span.Slice( macroStartPos + 1 ).IndexOf( options.MacroDelimiter );

      // We're done, no closing macro delimiter was found, treat the
      // rest of the text as a constant segment
      if( macroEndPos == -1 )
      {
        AddConstantSegment( macroStartPos, templateText.Length - macroStartPos );
        break;
      }

      macroEndPos += macroStartPos + 1;

      AddMacroSegment( macroStartPos, macroEndPos );

      // Move past the macro we just processed
      currentPos = macroEndPos + 1;
    }

    return segments.ToArray();

    void AddMacroSegment(
      int startPos,
      int endPos )
    {
      // An empty macro means we found an escaped delimiter
      if( endPos == startPos + 1 )
      {
        AddConstantSegment( startPos, 1 );
        return;
      }

      // Must be a macro
      var name = templateText.Slice( startPos + 1, endPos - startPos - 1 );
      var argument = ReadOnlyMemory<char>.Empty;
      var argStart = name.Span.IndexOf( options.ArgumentSeparator );

      // Does the macro have an argument?
      if( argStart != -1 )
      {
        argument = name.Slice( argStart + 1 );
        name = name.Slice( 0, argStart );

        if( name.IsEmpty )
        {
          throw new InvalidOperationException( "The macro name cannot be empty" );
        }
      }

#if NET9_0_OR_GREATER
      var slotNumber = macroTable.GetSlot( name.Span );
#else
      var slotNumber = macroTable.GetSlot( name.ToString() );
#endif
      segments.Add( Segment.CreateMacro( startPos + 1, name, argument, slotNumber ) );
    }

    void AddConstantSegment(
      int newSegmentStart,
      int newSegmentLength )
    {
      //   Merging rules:
      //   1. If the previous segment is an escaped delimiter (single delimiter constant) and the new
      //      segment is adjacent text, the merged segment starts at the second delimiter position so
      //      that the resulting text contains a single delimiter followed by the adjacent text.
      //   2. If the previous segment is a constant and is directly adjacent to the new segment,
      //      the segments are merged into a single constant segment covering both ranges.

      // If there's a previous segment, check if we can merge with it
      if( segments.Count > 0 )
      {
        var preSegmentIndex = segments.Count - 1;
        var prevSegment = segments[preSegmentIndex];

        // Only constant segments can be merged
        if( prevSegment.IsConstant )
        {
          var prevSegmentLength = prevSegment.Memory.Length;
          var prevSegmentStart = prevSegment.Start;

          // Case 1: Previous segment is an escaped delimiter and the new one
          // is adjacent to the second delimiter character.
          if( prevSegmentLength == 1 &&
              prevSegment.Memory.Span[0] == options.MacroDelimiter &&
              prevSegmentStart + 1 == newSegmentStart - 1 )
          {
            // Merge the segments, starting at the second delimiter position
            var newSegment = Segment.CreateConstant(
              prevSegmentStart + 1,
              templateText.Slice( prevSegmentStart + 1, newSegmentLength + 1 )
            );

            // Replace the existing segment with the merged one
            segments[preSegmentIndex] = newSegment;
            return;
          }

          // Case 2: New segment is adjacent to the previous one.
          if( prevSegmentStart + prevSegmentLength == newSegmentStart )
          {
            // Merge the segments
            var newSegment = Segment.CreateConstant(
              prevSegmentStart,
              templateText.Slice( prevSegmentStart, prevSegmentLength + newSegmentLength )
            );

            // Replace the existing segment with the merged one
            segments[preSegmentIndex] = newSegment;
            return;
          }
        }
      }

      // Add a new segment
      segments.Add( Segment.CreateConstant( newSegmentStart, templateText.Slice( newSegmentStart, newSegmentLength ) ) );
    }
  }

  #endregion
}
