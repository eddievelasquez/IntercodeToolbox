// Module Name: SegmentAssertionsExtensions.cs
// Author:      Eduardo Velasquez
// Copyright (c) 2025, Intercode Consulting, Inc.

#pragma warning disable IDE0130
namespace FluentAssertions;
#pragma warning restore IDE0130
using Intercode.Toolbox.TemplateEngine;

/// <summary>
///   Extension methods to obtain custom assertions for <see cref="Segment" />.
/// </summary>
internal static class SegmentAssertionsExtensions
{
  #region Public Methods

  /// <summary>
  ///   Returns an object that can be used to assert the current <see cref="Segment" />.
  /// </summary>
  public static SegmentAssertions Should(
    this Segment instance )
  {
    return new SegmentAssertions( instance );
  }

  #endregion
}
