// Module Name: TemplateCompiler.cs
// Author:      Eduardo Velasquez
// Copyright (c) 2024, Intercode Consulting, Inc.

namespace Intercode.Toolbox.TypedPrimitives.TemplateEngine;

using System.Diagnostics;

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

  public CompiledTemplate Compile(
    string template )
  {
    if( string.IsNullOrEmpty( template ) )
    {
      throw new ArgumentException( "Value cannot be null or empty.", nameof( template ) );
    }

    Phase1SplitIntoChunks( template, out var chunks, out var emptyMacros );

    // Optimize chunks:
    // Convert empty macro chunks into a single delimiter and merge into an adjacent constant segment
    Phase2RemoveEmptyMacroChunks( ref template, chunks, emptyMacros );

    // Optimize chunks:
    // Merge adjacent constant segments
    Phase3MergeAdjacentConstantSegments( chunks );

    // Generate compiled template
    var compiledTemplate = Phase4GenerateCompiledTemplate( template, chunks );
    return compiledTemplate;
  }

  #endregion

  #region Implementation

  private static void Phase1SplitIntoChunks(
    string template,
    out List<Chunk> chunks,
    out List<Chunk> emptyMacros )
  {
    chunks = [];
    emptyMacros = [];

    var templateMemory = template.AsMemory();
    var currentIndex = 0;

    while( currentIndex < templateMemory.Length )
    {
      var macroStart = templateMemory.Slice( currentIndex )
                                     .Span
                                     .IndexOf( MacroDelimiter );

      if( macroStart == -1 )
      {
        // No more macros found, add the remaining text as a constant segment
        chunks.Add( Chunk.CreateConstant( currentIndex, templateMemory.Length - currentIndex ) );
        break;
      }

      macroStart += currentIndex;

      // Add the text before the macro delimiter as a constant segment
      if( macroStart > currentIndex )
      {
        chunks.Add( Chunk.CreateConstant( currentIndex, macroStart - currentIndex ) );
      }

      // Look for the closing macro delimiter
      var macroEnd = templateMemory.Slice( ( macroStart + 1 ) )
                                   .Span
                                   .IndexOf( MacroDelimiter );

      if( macroEnd == -1 )
      {
        // No closing delimiter found, treat the rest as a constant segment
        chunks.Add( Chunk.CreateConstant( macroStart, templateMemory.Length - macroStart ) );
        break;
      }

      macroEnd += macroStart + 1;

      // Add the macro chunk
      var chunk = Chunk.CreateMacro( macroStart, macroEnd - macroStart + 1 );
      chunks.Add( chunk );

      // If empty macro, add it to the empty list
      if( macroEnd == macroStart + 1 )
      {
        emptyMacros.Add( chunk );
      }

      // Move the current index after the closing delimiter index
      currentIndex = macroEnd + 1;
    }
  }

  private static void Phase2RemoveEmptyMacroChunks(
    ref string template,
    List<Chunk> chunks,
    List<Chunk> emptyMacros )
  {
    if( emptyMacros.Count == 0 )
    {
      return;
    }

    var builder = StringBuilderPool.Default.Get();

    try
    {
      builder.Clear().Append( template );

      var index = 0;
      foreach( var chunk in emptyMacros )
      {
        // Start searching from the current index.
        // Can't start at index + 1 because the current chunk may have been removed.
        index = chunks.IndexOf( chunk, index );

        Debug.Assert( index >= 0 && index < chunks.Count, $"Index (index) out of range [0, {chunks.Count}]" );

        if( HasLeftConstantText( index ) )
        {
          // Remove the closing delimiter of the empty macro in the current chunk
          var macroChunk = chunks[index];
          builder.Remove( macroChunk.Start + 1, 1 );

          // Extend the prior constant text segment to include the opening delimiter of the empty macro
          var textChunk = chunks[index - 1];
          ++textChunk.Length;

          // All subsequent chunks must be shifted left by one
          ShiftChunksLeft( index + 1 );

          // Remove the macro chunk
          chunks.RemoveAt( index );
        }
        else if( HasRightConstantText( index ) )
        {
          // Remove the closing delimiter of the empty macro in the current chunk
          var macroChunk = chunks[index];
          builder.Remove( macroChunk.Start + 1, 1 );

          // Shift the right constant text segment to the left
          var textChunk = chunks[index + 1];
          --textChunk.Start;
          ++textChunk.Length;

          // All subsequent chunks must be shifted left by one
          ShiftChunksLeft( index + 1 );

          // Remove the macro chunk
          chunks.RemoveAt( index );
        }
        else
        {
          // Convert macro to constant text
          // Remove the closing delimiter of the empty macro in the current chunk
          var macroChunk = chunks[index];
          builder.Remove( macroChunk.Start + 1, 1 );

          macroChunk.Kind = SegmentKind.Constant;
          --macroChunk.Length;

          // All subsequent chunks must be shifted left by one
          ShiftChunksLeft( index + 1 );
        }
      }

      template = builder.ToString();
    }
    finally
    {
      StringBuilderPool.Default.Return( builder );
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

    bool HasLeftConstantText(
      int index )
    {
      return index > 0 && chunks[index - 1].Kind == SegmentKind.Constant;
    }

    bool HasRightConstantText(
      int index )
    {
      return index < chunks.Count - 1 && chunks[index + 1].Kind == SegmentKind.Constant;
    }
  }

  private static void Phase3MergeAdjacentConstantSegments(
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

      // Only merge adjacent constant text segments
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

  private static CompiledTemplate Phase4GenerateCompiledTemplate(
    string template,
    List<Chunk> chunks )
  {
    var templateMemory = template.AsMemory();
    var segments = chunks.Select( chunk => new Segment( chunk.Kind, templateMemory.Slice( chunk.Start, chunk.Length ) ) )
                         .ToArray();

    var compiledTemplate = new CompiledTemplate( template, segments );
    return compiledTemplate;
  }

  #endregion
}
