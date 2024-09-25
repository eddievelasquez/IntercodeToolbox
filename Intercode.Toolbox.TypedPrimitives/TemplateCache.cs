// Module Name: TemplateCache.cs
// Author:      Eduardo Velasquez
// Copyright (c) 2024, Intercode Consulting, Inc.

namespace Intercode.Toolbox.TypedPrimitives;

using System.Collections.Concurrent;

internal class TemplateCache
{
  #region Fields

  private readonly ConcurrentDictionary<string, CompiledTemplate> _cache = new ();

  #endregion

  #region Public Methods

  public CompiledTemplate GetOrAddTemplate(
    TemplateContext context,
    Func<TemplateContext, CompiledTemplate> factory )
  {
    return _cache.GetOrAdd( context.TemplateKey, _ => factory( context ) );
  }

  public CompiledTemplate GetOrAddTemplate(
    string templateKey,
    TemplateContext context,
    Func<TemplateContext, CompiledTemplate> factory )
  {
    return _cache.GetOrAdd( templateKey, _ => factory( context ) );
  }

  #endregion
}
