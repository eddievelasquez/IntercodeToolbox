// Module Name: SupportedTypeInfo.cs
// Author:      Eduardo Velasquez
// Copyright (c) 2024, Intercode Consulting, Inc.

namespace Intercode.Toolbox.TypedPrimitives;

internal class SupportedTypeInfo(
  string keyword,
  string name,
  string jsonTokenType,
  string jsonReader,
  string jsonWriter )
{
  #region Properties

  public string Keyword { get; } = keyword;
  public string Name { get; } = name;
  public string JsonTokenType { get; } = jsonTokenType;
  public string JsonReader { get; } = jsonReader;
  public string JsonWriter { get; } = jsonWriter;

  #endregion
}
