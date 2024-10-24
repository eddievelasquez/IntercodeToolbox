// Module Name: TemplateCompiler.cs
// Author:      Eduardo Velasquez
// Copyright (c) 2024, Intercode Consulting, Inc.

namespace Intercode.Toolbox.TemplateEngine;

/// <summary>
///   Compiles a template text into a <see cref="Template" />.
/// </summary>
public partial class TemplateCompiler
{
  #region Fields

  private readonly TemplateEngineOptions _options;

  #endregion

  #region Constructors

  /// <summary>
  ///   Initializes a new instance of the <see cref="TemplateCompiler" /> class.
  /// </summary>
  /// <param name="options">
  ///   The Template Engine Options. Will use to <see cref="TemplateEngineOptions.Default" /> if
  ///   <c>null</c>.
  /// </param>
  public TemplateCompiler(
    TemplateEngineOptions? options = null )
  {
    _options = options ?? TemplateEngineOptions.Default;
  }

  #endregion

  #region Public Methods

  /// <summary>
  ///   Compiles a template text into a <see cref="Template" />.
  /// </summary>
  /// <param name="text">The template text to compile.</param>
  /// <returns>The compiled <see cref="Template" />.</returns>
  /// <exception cref="ArgumentException">Thrown with the template <paramref name="text" /> is <c>null</c> or empty.</exception>
  public Template Compile(
    string text )
  {
    if( string.IsNullOrEmpty( text ) )
    {
      throw new ArgumentException( "The template's text cannot be null or empty.", nameof( text ) );
    }

    var segments = SplitIntoSegments( text.AsMemory() );
    var template = new Template( segments );
    return template;
  }

  #endregion

  #region Implementation

  private Segment[] SplitIntoSegments(
    ReadOnlyMemory<char> text )
  {
    var segments = new List<Segment>();
    var delimiter = _options.MacroDelimiter;
    var separator = _options.ArgumentSeparator;
    var currentIndex = 0;

    while( currentIndex < text.Length )
    {
      var macroStart = text.Span.Slice( currentIndex )
                           .IndexOf( delimiter );

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
      var macroEnd = text.Span.Slice( ( macroStart + 1 ) )
                         .IndexOf( delimiter );

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
        var argStart = name.Span.IndexOf( separator );
        if( argStart != -1 )
        {
          var argument = name.Slice( argStart + 1 );
          name = name.Slice( 0, argStart );
          segments.Add( Segment.CreateMacro( name, argument ) );
        }
        else

        {
          segments.Add( Segment.CreateMacro( name ) );
        }
      }

      // Move the current index after the closing delimiter index
      currentIndex = macroEnd + 1;
    }

    return segments.ToArray();
  }

  #endregion
}
