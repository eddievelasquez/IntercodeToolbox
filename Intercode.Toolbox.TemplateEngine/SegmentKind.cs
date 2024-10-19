// Module Name: SegmentKind.cs
// Author:      Eduardo Velasquez
// Copyright (c) 2024, Intercode Consulting, Inc.

namespace Intercode.Toolbox.TemplateEngine;

/// <summary>
///   Represents the kind of text segment in a template.
/// </summary>
public enum SegmentKind
{
  /// <summary>
  ///   Represents a constant text segment.
  /// </summary>
  Constant,

  /// <summary>
  ///   Represents a macro segment.
  /// </summary>
  Macro
}
