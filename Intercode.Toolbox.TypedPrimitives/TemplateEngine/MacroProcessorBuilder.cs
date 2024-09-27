// Module Name: MacroProcessorBuilder.cs
// Author:      Eduardo Velasquez
// Copyright (c) 2024, Intercode Consulting, Inc.

namespace Intercode.Toolbox.TypedPrimitives.TemplateEngine;

public class MacroProcessorBuilder
{
  #region Fields

  private readonly Dictionary<string, string> _macroValues = new ( StringComparer.OrdinalIgnoreCase );

  #endregion

  #region Public Methods

  public void AddMacro(
    string name,
    string value )
  {
    _macroValues.Add( $"${name}$", value );
  }

  public MacroProcessor Build()
  {
    return new MacroProcessor( _macroValues );
  }

  #endregion
}
