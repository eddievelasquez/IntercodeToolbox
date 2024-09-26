// Module Name: SupportedTypeInfo.cs
// Author:      Eduardo Velasquez
// Copyright (c) 2024, Intercode Consulting, Inc.

namespace Intercode.Toolbox.TypedPrimitives;

internal record SupportedTypeInfo(
  string Keyword,
  string JsonTokenType,
  string JsonReader,
  string JsonWriter,
  string NewtonsoftJsonTokenType );
