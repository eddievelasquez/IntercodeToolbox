// Module Name: MacroProcessor.cs
// Author:      Eduardo Velasquez
// Copyright (c) 2024, Intercode Consulting, Inc.

namespace Intercode.Toolbox.TypedPrimitives;

using System.Collections.Frozen;

internal class MacroProcessor
{
  #region Fields

  private readonly FrozenDictionary<string, string> _macros;

  #endregion

  #region Constructors

  public MacroProcessor(
    IDictionary<string, string> macros )
  {
    _macros = macros.ToFrozenDictionary( StringComparer.OrdinalIgnoreCase );
  }

  #endregion

  #region Public Methods

  public string ProcessMacros(
    CompiledTemplate compiledTemplate )
  {
    var outputBuilder = ContentBuilder.StringBuilderPool.Get();

    // This assumes that every macro value is used in the template. It doesn't account for
    // the fact that a macro can be used several times in a template, but it's a good
    // heuristic for now.
    var bufferLength = _macros.Values.Sum( s => s.Length ) + compiledTemplate.ConstantTextLength;
    outputBuilder.EnsureCapacity( bufferLength );

    try
    {
      foreach( var segment in compiledTemplate.Segments )
      {
        switch( segment.Type )
        {
          case SegmentType.ConstantText:

            // NOTE: StringBuilder in NetStandard2.0 doesn't have an Append method
            // that takes a ReadOnlySpan<char>, unfortunately we must first convert the span to text.
            outputBuilder.Append( segment.Text );
            break;

          case SegmentType.Macro:
          {
            // Unfortunately we cannot use a span for the macro as Dictionary does not
            // yet span lookup support; however it might come in .NET 9.
            // See: https://github.com/dotnet/runtime/issues/27229
            var macroName = segment.Text;
            outputBuilder.Append( _macros.TryGetValue( macroName, out var macroValue ) ? macroValue : macroName );
            break;
          }

          default:
            throw new InvalidOperationException( $"Unknown segment type '{segment.Type}'" );
        }
      }

      var output = outputBuilder.ToString();
      return output;
    }
    finally
    {
      ContentBuilder.StringBuilderPool.Return( outputBuilder );
    }
  }

  #endregion
}
