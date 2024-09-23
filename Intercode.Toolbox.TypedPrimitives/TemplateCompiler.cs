// Module Name: TemplateCompiler.cs
// Author:      Eduardo Velasquez
// Copyright (c) 2024, Intercode Consulting, Inc.

namespace Intercode.Toolbox.TypedPrimitives;

public class TemplateCompiler
{
  // TODO: Should we leave escaped delimiters as an empty macros or create adjacent
  // constant text segments where the first segment ends with the delimiter and the second
  // one skips the ending delimiter? Right now, we can't merge these segments because we
  // don't alter the template in any way and each segment has a ReadOnlyMemory instance
  // for performance. Maybe a segment would have to change to just have the start index and
  // length and add an optimization phase that merges adjacent constant text segments and
  // adjusts the trailing segments start indexes.

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

    var segments = new List<Segment>();
    var templateMemory = template.AsMemory();
    var currentIndex = 0;
    var constantTextLength = 0;

    while( currentIndex < templateMemory.Length )
    {
      var macroStart = templateMemory.Span.Slice( currentIndex )
                                     .IndexOf( MacroDelimiter );

      if( macroStart == -1 )
      {
        // No more macros found, add the remaining text as a constant segment
        AddConstantSegment( templateMemory.Slice( currentIndex ) );
        break;
      }

      macroStart += currentIndex;

      // Add the text before the macro delimiter as a constant segment
      if( macroStart > currentIndex )
      {
        AddConstantSegment( templateMemory.Slice( currentIndex, macroStart - currentIndex ) );
      }

      // Look for the closing macro delimiter
      var macroEnd = templateMemory.Span.Slice( ( macroStart + 1 ) )
                                   .IndexOf( MacroDelimiter );

      if( macroEnd == -1 )
      {
        // No closing delimiter found, treat the rest as a constant segment
        AddConstantSegment( templateMemory.Slice( macroStart ) );
        break;
      }

      macroEnd += macroStart + 1;

      // Add the macro segment
      segments.Add( new Segment( SegmentType.Macro, templateMemory.Slice( macroStart, macroEnd - macroStart + 1 ) ) );

      // Move the current index to after the closing '$'
      currentIndex = macroEnd + 1;
    }

    var compiledTemplate = new CompiledTemplate( template, segments.ToArray(), constantTextLength );
    return compiledTemplate;

    void AddConstantSegment(
      ReadOnlyMemory<char> memory )
    {
      // Segments cannot be empty!
      if( !memory.IsEmpty )
      {
        constantTextLength += memory.Length;
        segments.Add( new Segment( SegmentType.ConstantText, memory ) );
      }
    }
  }

  #endregion
}
