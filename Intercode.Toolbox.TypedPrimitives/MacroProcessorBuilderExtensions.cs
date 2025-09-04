// Module Name: MacroProcessorBuilderExtensions.cs
// Author:      Eduardo Velasquez
// Copyright (c) 2025, Intercode Consulting, Inc.

namespace Intercode.Toolbox.TypedPrimitives;

using Intercode.Toolbox.TemplateEngine;

internal static class MacroProcessorBuilderExtensions
{
  #region Public Methods

  public static MacroProcessorContext AddConverterMacros(
    this MacroProcessorContext templateContext,
    bool addMacros,
    TypedPrimitiveConverter converter,
    SupportedTypeInfo typeInfo )
  {
    foreach( var pair in typeInfo.GetConverterMacros( converter ) )
    {
      templateContext.AddMacro( pair.Key, addMacros ? pair.Value : null );
    }

    return templateContext;
  }

  public static IncludesCollection AddIncludes(
    this IncludesCollection includes,
    bool addIncludes,
    TypedPrimitiveConverter converter,
    SupportedTypeInfo typeInfo )
  {
    foreach( var pair in typeInfo.GetIncludes( converter ) )
    {
      includes.AddInclude( pair.Key, addIncludes ? pair.Value : null );
    }

    return includes;
  }

  #endregion
}
