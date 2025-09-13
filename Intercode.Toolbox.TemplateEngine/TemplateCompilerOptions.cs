// Module Name: TemplateCompilerOptions.cs
// Author:      Eduardo Velasquez
// Copyright (c) 2024, Intercode Consulting, Inc.

namespace Intercode.Toolbox.TemplateEngine;

/// <summary>
///   Represents the options for the <see cref="TemplateCompiler" />.
/// </summary>
public sealed class TemplateCompilerOptions
{
  #region Constants

  /// <summary>
  ///   The default template engine options.
  /// </summary>
  public static readonly TemplateCompilerOptions Default = new TemplateCompilerOptionsBuilder().Build();

  #endregion

  #region Constructors

  internal TemplateCompilerOptions()
  {
  }

  #endregion

  #region Properties

  /// <summary>
  ///   Gets or initializes the macro delimiter.
  /// </summary>
  public char MacroDelimiter { get; internal init; }

  /// <summary>
  ///   Gets the macro's argument separator.
  /// </summary>
  public char ArgumentSeparator { get; internal init; }

  #endregion
}
