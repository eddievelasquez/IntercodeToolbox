// Module Name: TemplateAssertionsExtensions.cs
// Author:      Eduardo Velasquez
// Copyright (c) 2025, Intercode Consulting, Inc.

namespace Intercode.Toolbox.TemplateEngine.Tests.FluentAssertions;

internal static class TemplateAssertionsExtensions
{
  #region Public Methods

  public static TemplateAssertions Should(
    this Template instance )
  {
    return new TemplateAssertions( instance );
  }

  #endregion
}
