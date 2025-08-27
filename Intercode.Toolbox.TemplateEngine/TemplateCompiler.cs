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
  /// <param name="context">The <see cref="TemplateContext" /> providing macros and options for compilation.</param>
  /// <param name="text">The template text to compile.</param>
  /// <returns>The compiled <see cref="Template" />.</returns>
  /// <exception cref="ArgumentNullException">Thrown when <paramref name="context" /> is <c>null</c>.</exception>
  /// <exception cref="ArgumentException">
  ///   Thrown when the template <paramref name="text" /> is <c>null</c>, empty, or
  ///   consists only of whitespace.
  /// </exception>
  public static Template Compile(
    TemplateContext context,
    string text )
  {
    if( context == null )
    {
      throw new ArgumentNullException( nameof( context ) );
    }

    if( string.IsNullOrWhiteSpace( text ) )
    {
      throw new ArgumentException( "The template's text cannot be null, empty, or whitespace.", nameof( text ) );
    }

    var segments = SplitIntoSegments( context, text.AsMemory() );
    return new Template( context, segments );
  }

  #endregion

  #region Implementation

  private static Segment[] SplitIntoSegments(
    TemplateContext context,
    ReadOnlyMemory<char> text )
  {
    var segments = new List<Segment>();
    var delimiter = context.Options.MacroDelimiter;
    var separator = context.Options.ArgumentSeparator;
    var span = text.Span;
    var currentIndex = 0;

    while( currentIndex < text.Length )
    {
      var macroStart = span.Slice( currentIndex ).IndexOf( delimiter );

      if( macroStart == -1 )
      {
        // No more macros found, add the remaining text as a constant segment
        segments.Add( Segment.CreateConstant( text.Slice( currentIndex ) ) );
        break;
      }

      macroStart += currentIndex;

      // Add the text before the macro delimiter (if any) as a constant segment
      if( macroStart > currentIndex )
      {
        segments.Add( Segment.CreateConstant( text.Slice( currentIndex, macroStart - currentIndex ) ) );
      }

      // Look for the closing macro delimiter
      var macroEnd = span.Slice( macroStart + 1 ).IndexOf( delimiter );

      if( macroEnd == -1 )
      {
        // No closing delimiter found, treat the rest of the text as a constant segment
        segments.Add( Segment.CreateConstant( text.Slice( macroStart ) ) );
        break;
      }

      macroEnd += macroStart + 1;

      // Add the macro segment (The delimiters will be excluded)
      if( macroEnd == macroStart + 1 )
      {
        // An empty macro means we found an escaped delimiter
        segments.Add( Segment.CreateDelimiter() );
      }
      else
      {
        var name = text.Slice( macroStart + 1, macroEnd - macroStart - 1 );
        var argument = ReadOnlyMemory<char>.Empty;
        var argStart = name.Span.IndexOf( separator );

        if( argStart != -1 )
        {
          argument = name.Slice( argStart + 1 );
          name = name.Slice( 0, argStart );
        }

#if NET9_0_OR_GREATER
        var slotNumber = context.AddMacro( name.Span );
#else
        var slotNumber = context.AddMacro( name.ToString() );
#endif

        segments.Add( Segment.CreateMacro( name, argument, slotNumber ) );
      }

      // Move the current index after the closing delimiter index
      currentIndex = macroEnd + 1;
    }

    return segments.ToArray();
  }

  #endregion
}
