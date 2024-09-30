// Module Name: MacroProcessorBuilder.cs
// Author:      Eduardo Velasquez
// Copyright (c) 2024, Intercode Consulting, Inc.

namespace Intercode.Toolbox.TypedPrimitives.TemplateEngine;

public class MacroProcessorBuilder( char? macroDelimiter = null )
{
  #region Constants

  public const char DefaultMacroDelimiter = '$';

  #endregion

  #region Fields

  private readonly Dictionary<string, string> _macroValues = new ( StringComparer.OrdinalIgnoreCase );

  #endregion

  #region Properties

  public char MacroDelimiter { get; } = macroDelimiter ?? DefaultMacroDelimiter;

  #endregion

  #region Public Methods

  public MacroProcessorBuilder AddMacro(
    string name,
    string value )
  {
    if( string.IsNullOrWhiteSpace( name ) )
    {
      throw new ArgumentException( "Value cannot be null or whitespace.", nameof( name ) );
    }

    if( !IsAlphanumericOrUnderscore( name ) )
    {
      throw new ArgumentException( "Macro name must be alphanumeric or underscore.", nameof( name ) );
    }

    _macroValues.Add( $"{MacroDelimiter}{name}{MacroDelimiter}", value );
    return this;

    bool IsAlphanumericOrUnderscore(
      string input )
    {
      // NOTE: Use loop instead of LINQ for performance
      foreach( var c in input )
      {
        if( !char.IsLetterOrDigit( c ) && c != '_' )
        {
          return false;
        }
      }

      return true;
    }
  }

  public MacroProcessor Build()
  {
    return new MacroProcessor( _macroValues, MacroDelimiter );
  }

  #endregion
}
