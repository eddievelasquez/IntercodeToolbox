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
  /// <param name="context">The <see cref="MacroProcessorContext" /> providing macros and options for compilation.</param>
  /// <param name="text">The template text to compile.</param>
  /// <param name="includes">
  ///   An optional <see cref="IncludesCollection" /> that provides additional resources
  ///   required during the compilation of the template. If not provided, it defaults to <c>null</c>.
  /// </param>
  /// <returns>The compiled <see cref="Template" />.</returns>
  /// <exception cref="ArgumentNullException">Thrown when <paramref name="context" /> is <c>null</c>.</exception>
  /// <exception cref="ArgumentException">
  ///   Thrown when the template <paramref name="text" /> is <c>null</c>, empty, or consists only of whitespace.
  /// </exception>
  public static Template Compile(
    MacroProcessorContext context,
    string text,
    IncludesCollection? includes = null )
  {
    if( context == null )
    {
      throw new ArgumentNullException( nameof( context ) );
    }

    if( string.IsNullOrWhiteSpace( text ) )
    {
      throw new ArgumentException( "The template's text cannot be null, empty, or whitespace.", nameof( text ) );
    }

    if( includes?.Count > 0 )
    {
      text = ProcessIncludes( context, text.AsSpan(), includes );
    }

    var segments = SplitIntoSegments( context, text.AsMemory() );

    // Create an empty constant segment if we ended up with no segments
    if( segments.Length == 0 )
    {
      segments = [Segment.CreateConstant( ReadOnlyMemory<char>.Empty )];
    }

    return new Template( context, segments );
  }

  #endregion

  #region Implementation

  private static string ProcessIncludes(
    MacroProcessorContext context,
    ReadOnlySpan<char> text,
    IncludesCollection includes )
  {
    var delimiter = context.CompilerOptions.MacroDelimiter;
    var builder = StringBuilderPool.Default.Get();

    try
    {
      // Ensure the builder has at least enough capacity to hold the pre-processed text
      builder.Capacity = Math.Max( builder.Capacity, text.Length );

      var currentIndex = 0;

      while( currentIndex < text.Length )
      {
        var macroStart = text.Slice( currentIndex ).IndexOf( delimiter );

        if( macroStart == -1 )
        {
          // No more macros found, add the remaining text as a constant segment
          Append( text.Slice( currentIndex ) );
          break;
        }

        macroStart += currentIndex;

        // Add the text before the macro delimiter (if any) as a constant segment
        if( macroStart > currentIndex )
        {
          Append( text.Slice( currentIndex, macroStart - currentIndex ) );
        }

        // Look for the closing macro delimiter
        var macroEnd = text.Slice( macroStart + 1 ).IndexOf( delimiter );

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
          builder.Append( delimiter );
          builder.Append( delimiter );
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
    MacroProcessorContext context,
    ReadOnlyMemory<char> text )
  {
    var segments = new List<Segment>();
    var delimiter = context.CompilerOptions.MacroDelimiter;
    var separator = context.CompilerOptions.ArgumentSeparator;
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
