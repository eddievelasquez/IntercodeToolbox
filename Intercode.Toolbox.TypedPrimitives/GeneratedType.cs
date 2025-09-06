// Module Name: GeneratedType.cs
// Author:      Eduardo Velasquez
// Copyright (c) 2024, Intercode Consulting, Inc.

namespace Intercode.Toolbox.TypedPrimitives;

using Microsoft.CodeAnalysis.Text;

internal record struct GeneratedType(
  string TypeName,
  SourceText SourceText )
{
  #region Constructors

  public GeneratedType(
    GeneratorModel model,
    ConverterModel converter,
    SourceText sourceText )
    : this( $"{model.Namespace}.{model.TypeName}{converter.Name}", sourceText )
  {
  }

  #endregion
}
