// Module Name: TemplateCompiler.cs
// Author:      Eduardo Velasquez
// Copyright (c) 2024, Intercode Consulting, Inc.

namespace Intercode.Toolbox.TypedPrimitives.TemplateEngine;

using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

public class TemplateCompiler
{
  #region Nested Types

  [DebuggerDisplay( "Kind: {Kind}, Start: {Start}, Length: {Length}" )]
  private class Chunk(
    SegmentKind kind,
    int start,
    int length )
  {
    #region Properties

    public SegmentKind Kind { get; set; } = kind;
    public int Start { get; set; } = start;
    public int Length { get; set; } = length;

    #endregion

    #region Public Methods

    public static Chunk CreateConstant(
      int start,
      int length )
    {
      return new Chunk( SegmentKind.Constant, start, length );
    }

    public static Chunk CreateMacro(
      int start,
      int length )
    {
      return new Chunk( SegmentKind.Macro, start, length );
    }

    #endregion
  }

  #endregion

  #region Constants

  public const char MacroDelimiter = '$';

  #endregion

  #region Public Methods

  public Template Compile(
    string text )
  {
    if( string.IsNullOrEmpty( text ) )
    {
      throw new ArgumentException( "The template's text cannot be null or empty.", nameof( text ) );
    }

    // Split the text into chunks of constant text and macros
    Phase1SplitIntoChunks( text.AsSpan(), out var chunks, out var emptyMacros );

    // Optimize chunks: Convert empty macro chunks into a single delimiter and merge it into
    // an adjacent constant text chunk (basically escaping $$ into $)
    Phase2RemoveEmptyMacroChunks( ref text, chunks, emptyMacros );

    // Optimize chunks: Merge adjacent constant text chunks into the leftmost chunk
    Phase3MergeAdjacentConstantTextChunks( chunks );

    // Generate template from the chunks
    var template = Phase4GenerateTemplate( text, chunks );
    return template;
  }

  #endregion

  #region Implementation

  private static void Phase1SplitIntoChunks(
    ReadOnlySpan<char> text,
    out List<Chunk> chunks,
    out List<Chunk> emptyMacros )
  {
    chunks = [];
    emptyMacros = []; // Optimization: keep a list of empty macros to remove them in the next phase

    var currentIndex = 0;

    while( currentIndex < text.Length )
    {
      var macroStart = text.Slice( currentIndex )
                           .IndexOf( MacroDelimiter );

      if( macroStart == -1 )
      {
        // No more macros found, add the remaining text as a constant chunk
        chunks.Add( Chunk.CreateConstant( currentIndex, text.Length - currentIndex ) );
        break;
      }

      macroStart += currentIndex;

      // Add the text before the macro delimiter (if any) as a constant chunk
      if( macroStart > currentIndex )
      {
        chunks.Add( Chunk.CreateConstant( currentIndex, macroStart - currentIndex ) );
      }

      // Look for the closing macro delimiter
      var macroEnd = text.Slice( ( macroStart + 1 ) )
                         .IndexOf( MacroDelimiter );

      if( macroEnd == -1 )
      {
        // No closing delimiter found, treat the rest of the text as a constant chunk
        chunks.Add( Chunk.CreateConstant( macroStart, text.Length - macroStart ) );
        break;
      }

      macroEnd += macroStart + 1;

      // Add the macro chunk
      var chunk = Chunk.CreateMacro( macroStart, macroEnd - macroStart + 1 );
      chunks.Add( chunk );

      // If the macro is empty, add it to the empty list
      if( macroEnd == macroStart + 1 )
      {
        emptyMacros.Add( chunk );
      }

      // Move the current index after the closing delimiter index
      currentIndex = macroEnd + 1;
    }
  }

  private static void Phase2RemoveEmptyMacroChunks(
    ref string text,
    List<Chunk> chunks,
    List<Chunk> emptyMacros )
  {
    // Nothing to do if there are no empty macros
    if( emptyMacros.Count == 0 )
    {
      return;
    }

    var sb = StringBuilderPool.Default.Get();

    try
    {
      sb.Clear().Append( text );

      var index = 0;
      foreach( var chunk in emptyMacros )
      {
        // Start searching from the current index.
        // Can't start at index + 1 because the current chunk may have been removed.
        index = chunks.IndexOf( chunk, index );

        Debug.Assert( index >= 0 && index < chunks.Count, $"Index (index) out of range [0, {chunks.Count}]" );

        // Remove the closing delimiter of the empty macro in the current chunk
        var macroChunk = chunks[index];
        sb.Remove( macroChunk.Start + 1, 1 );

        if( TryGetLeftConstantChunk( index, out var textChunk ) )
        {
          // Extend the prior constant text chunk to include the opening delimiter of the empty macro
          ++textChunk.Length;

          // All subsequent chunk start position must be shifted left by one character (the removed macro closing delimiter)
          ShiftChunksLeft( index + 1 );

          // Remove the macro chunk
          chunks.RemoveAt( index );
        }
        else if( TryGetRightConstantChunk( index, out textChunk ) )
        {
          // Shift the start of the constant text chunk to the left and extend to cover the delimiter
          --textChunk.Start;
          ++textChunk.Length;

          // All subsequent chunk start position must be shifted left by one character (the removed macro closing delimiter)
          ShiftChunksLeft( index + 1 );

          // Remove the macro chunk
          chunks.RemoveAt( index );
        }
        else
        {
          // Convert macro chunk kind to Constant to avoid processing empty macros in subsequent phases
          macroChunk.Kind = SegmentKind.Constant;

          // Remove the closing delimiter of the empty macro in the current chunk
          --macroChunk.Length;

          // All subsequent chunk start position must be shifted left by one character (the removed macro closing delimiter)
          ShiftChunksLeft( index + 1 );
        }
      }

      text = sb.ToString();
    }
    finally
    {
      StringBuilderPool.Default.Return( sb );
    }

    return;

    void ShiftChunksLeft(
      int startIndex )
    {
      for( var i = startIndex; i < chunks.Count; i++ )
      {
        --chunks[i].Start;
      }
    }

    bool TryGetLeftConstantChunk(
      int index,
      [NotNullWhen( true )] out Chunk? chunk )
    {
      // Check if there is a constant chunk to the left of the current chunk
      if( index > 0 )
      {
        var c = chunks[index - 1];
        if( c.Kind == SegmentKind.Constant )
        {
          chunk = c;
          return true;
        }
      }

      chunk = null;
      return false;
    }

    bool TryGetRightConstantChunk(
      int index,
      [NotNullWhen( true )] out Chunk? chunk )
    {
      // Check if there is a constant chunk to the right of the current chunk
      if( index < chunks.Count - 1 )
      {
        var c = chunks[index + 1];
        if( c.Kind == SegmentKind.Constant )
        {
          chunk = c;
          return true;
        }
      }

      chunk = null;
      return false;
    }
  }

  private static void Phase3MergeAdjacentConstantTextChunks(
    List<Chunk> chunks )
  {
    if( chunks.Count < 2 )
    {
      // Nothing to merge
      return;
    }

    for( var i = 1; i < chunks.Count; i++ )
    {
      var firstChunk = chunks[i - 1];
      var secondChunk = chunks[i];

      // Only merge adjacent constant text chunks
      if( firstChunk.Kind != SegmentKind.Constant || secondChunk.Kind != SegmentKind.Constant )
      {
        continue;
      }

      // Extend the first chunk to include the second chunk
      firstChunk.Length += secondChunk.Length;

      // Remove the second chunk
      chunks.RemoveAt( i );
    }
  }

  private static Template Phase4GenerateTemplate(
    string text,
    List<Chunk> chunks )
  {
    // Must use Memory because Span is not allowed on the heap
    var memory = text.AsMemory();
    var segments = chunks.Select( chunk => new Segment( chunk.Kind, memory.Slice( chunk.Start, chunk.Length ) ) )
                         .ToArray();

    var template = new Template( text, segments );
    return template;
  }

  #endregion
}
