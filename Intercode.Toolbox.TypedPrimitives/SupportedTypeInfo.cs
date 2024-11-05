// Module Name: SupportedTypeInfo.cs
// Author:      Eduardo Velasquez
// Copyright (c) 2024, Intercode Consulting, Inc.

namespace Intercode.Toolbox.TypedPrimitives;

using System.Collections.Frozen;

internal class SupportedTypeInfo
{
  #region Fields

  private readonly FrozenDictionary<TypedPrimitiveConverter, FrozenDictionary<string, string>> _customConverterMacros;

  #endregion

  #region Constructors

  public SupportedTypeInfo(
    string keyword,
    FrozenDictionary<TypedPrimitiveConverter, FrozenDictionary<string, string>> customConverterMacros )
  {
    _customConverterMacros = customConverterMacros;
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

  #endregion
}
