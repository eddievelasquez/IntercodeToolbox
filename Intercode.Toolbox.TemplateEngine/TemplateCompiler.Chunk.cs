// Module Name: TemplateCompiler.Chunk.cs
// Author:      Eduardo Velasquez
// Copyright (c) 2024, Intercode Consulting, Inc.

namespace Intercode.Toolbox.TemplateEngine;

using System.Diagnostics;

public partial class TemplateCompiler
{
  #region Nested Types

  [DebuggerDisplay( "Kind: {Kind}, Start: {Start}, Length: {Length}" )]
  private class Chunk(
    SegmentKind kind,
    int start,
    int length )
  {
    #region Properties

    public SegmentKind Kind { get; set; } = kind;
    public int Start { get; set; } = start;
    public int Length { get; set; } = length;

    #endregion

    #region Public Methods

    public static Chunk CreateConstant(
      int start,
      int length )
    {
      return new Chunk( SegmentKind.Constant, start, length );
    }

    public static Chunk CreateMacro(
      int start,
      int length )
    {
      return new Chunk( SegmentKind.Macro, start, length );
    }

    #endregion
  }

  #endregion
}
