// Module Name: MacroProcessorBuilderExtensions.cs
// Author:      Eduardo Velasquez
// Copyright (c) 2024, Intercode Consulting, Inc.

namespace Intercode.Toolbox.TypedPrimitives;

using Intercode.Toolbox.TemplateEngine;

internal static class MacroProcessorBuilderExtensions
{
  #region Public Methods

  public static MacroProcessorBuilder AddConverterMacros(
    this MacroProcessorBuilder builder,
    TypedPrimitiveConverter converter,
    SupportedTypeInfo typeInfo )
  {
    foreach( var pair in typeInfo.GetConverterMacros( converter ) )
    {
      builder.AddMacro( pair.Key, pair.Value );
    }

    return builder;
  }

  #endregion
}
