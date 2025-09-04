// Module Name: SupportedTypeInfo.cs
// Author:      Eduardo Velasquez
// Copyright (c) 2024, Intercode Consulting, Inc.

namespace Intercode.Toolbox.TypedPrimitives;

using System.Collections.Frozen;

internal class SupportedTypeInfo
{
  #region Fields

  private readonly FrozenDictionary<TypedPrimitiveConverter, FrozenDictionary<string, string>> _customConverterMacros;
  private readonly FrozenDictionary<TypedPrimitiveConverter, FrozenDictionary<string, string>> _includes;

  #endregion

  #region Constructors

  public SupportedTypeInfo(
    string keyword,
    FrozenDictionary<TypedPrimitiveConverter, FrozenDictionary<string, string>> customConverterMacros,
    FrozenDictionary<TypedPrimitiveConverter, FrozenDictionary<string, string>> includes )
  {
    _customConverterMacros = customConverterMacros;
    _includes = includes;
    Keyword = keyword;
  }

  #endregion

  #region Properties

  public string Keyword { get; init; }

  #endregion

  #region Public Methods

  public IEnumerable<KeyValuePair<string, string>> GetConverterMacros(
    TypedPrimitiveConverter converter )
  {
    return _customConverterMacros.TryGetValue( converter, out var macros ) ? macros : [];
  }

  public IEnumerable<KeyValuePair<string, string>> GetIncludes(
    TypedPrimitiveConverter converter )
  {
    return _includes.TryGetValue( converter, out var includes ) ? includes : [];
  }

  #endregion
}
