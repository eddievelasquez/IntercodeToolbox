// Module Name: MacroProcessor.cs
// Author:      Eduardo Velasquez
// Copyright (c) 2024, Intercode Consulting, Inc.

namespace Intercode.Toolbox.TypedPrimitives.TemplateEngine;

using System.Collections.Frozen;

public class MacroProcessor
{
  #region Fields

  private readonly char _macroDelimiter;

  private readonly FrozenDictionary<string, string> _macros;

  #endregion

  #region Constructors

  internal MacroProcessor(
    IDictionary<string, string> macros,
    char macroDelimiter )
  {
    _macroDelimiter = macroDelimiter;
    _macros = macros.ToFrozenDictionary( StringComparer.OrdinalIgnoreCase );
  }

  #endregion

  #region Properties

  public int MacroCount => _macros.Count;

  #endregion

  #region Public Methods

  public string? GetMacroValue(
    string macroName )
  {
    var key = $"{_macroDelimiter}{macroName}{_macroDelimiter}";
    return _macros.TryGetValue( key, out var macroValue ) ? macroValue : null;
  }

  public void ProcessMacros(
    Template template,
    TextWriter writer )
  {
    foreach( var segment in template.Segments )
    {
      switch( segment.Kind )
      {
        case SegmentKind.Constant:

          writer.Write( segment.Memory );
          break;

        case SegmentKind.Macro:
        {
          // Unfortunately we cannot use a span for the macro as Dictionary does not
          // yet span lookup support; however it might come in .NET 9.
          // See: https://github.com/dotnet/runtime/issues/27229
          var macroName = segment.Text;
          if( _macros.TryGetValue( macroName, out var macroValue ) )
          {
            writer.Write( macroValue );
          }

          break;
        }

        default:
          throw new InvalidOperationException( $"Unknown segment type '{segment.Kind}'" );
      }
    }
  }

  #endregion
}
