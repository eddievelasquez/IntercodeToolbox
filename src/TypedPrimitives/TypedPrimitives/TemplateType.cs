// Module Name: TemplateType.cs
// Author:      Eduardo Velasquez
// Copyright (c) 2025, Intercode Consulting, Inc.

namespace Intercode.Toolbox.TypedPrimitives;

/// <summary>
///   Specifies the available template types for typed primitive code generation.
/// </summary>
internal enum TemplateType
{
  /// <summary>
  ///   The main template for the typed primitive type.
  /// </summary>
  Main,

  /// <summary>
  ///   The template for generating a <c>TypeConverter</c> for the typed primitive.
  /// </summary>
  TypeConverter,

  /// <summary>
  ///   The template for generating a <c>System.Text.Json</c> converter for the typed primitive.
  /// </summary>
  SystemTextJsonConverter,

  /// <summary>
  ///   The template for generating a <c>Newtonsoft.Json</c> converter for the typed primitive.
  /// </summary>
  NewtonsoftJsonConverter,

  /// <summary>
  ///   The template for generating an Entity Framework Core value converter for the typed primitive.
  /// </summary>
  EfCoreValueConverter
}
