// Module Name: IncludesCollection.cs
// Author:      Eduardo Velasquez
// Copyright (c) 2025, Intercode Consulting, Inc.

namespace Intercode.Toolbox.TemplateEngine;

public class IncludesCollection
{
  #region Fields

  private readonly Dictionary<string, string?> _includes = new ( StringComparer.OrdinalIgnoreCase );

#if NET9_0_OR_GREATER
  private readonly Dictionary<string, string?>.AlternateLookup<ReadOnlySpan<char>> _alternate;
#endif

  #endregion

  #region Constructors

  public IncludesCollection()
  {
#if NET9_0_OR_GREATER
    _alternate = _includes.GetAlternateLookup<ReadOnlySpan<char>>();
#endif
  }

  #endregion

  #region Properties

  /// <summary>
  ///   Gets the number of include entries in the <see cref="IncludesCollection" />.
  /// </summary>
  /// <value>
  ///   The total count of include entries stored in the collection.
  /// </value>
  public int Count => _includes.Count;

  #endregion

  #region Public Methods

  /// <summary>
  ///   Adds an include to the collection with the specified name and content.
  /// </summary>
  /// <param name="name">The name of an include. Must be a valid macro name.</param>
  /// <param name="content">The content associated with an include. Can be <c>null</c>.</param>
  /// <exception cref="ArgumentException">
  ///   Thrown if the <paramref name="name" /> is null, empty, or contains invalid characters.
  ///   Valid macro names must consist of letters, digits, underscores ('_'), or dashes ('-').
  /// </exception>
  /// <remarks>
  ///   If an include with the specified name already exists, its content will be replaced.
  /// </remarks>
  public void AddInclude(
    string name,
    string? content )
  {
    MacroExtensions.ValidateMacroName( name );

    _includes[name] = content;
  }

  /// <summary>
  ///   Attempts to retrieve the content of an include by its name.
  /// </summary>
  /// <param name="name">The name of the include to retrieve. Must be a valid macro name.</param>
  /// <param name="content">
  ///   When this method returns <c>true</c>, contains the content associated with the specified include name.
  ///   When this method returns <c>false</c>, contains <c>null</c>.
  /// </param>
  /// <returns>
  ///   <c>true</c> if an include with the specified name exists; otherwise, <c>false</c>.
  /// </returns>
  /// <exception cref="ArgumentNullException">
  ///   Thrown if the <paramref name="name" /> is <c>null</c>.
  /// </exception>
  /// <remarks>
  ///   This method performs a case-insensitive lookup for the specified include name.
  /// </remarks>
  public bool TryGetIncludeContent(
    string name,
    out string? content )
  {
    if( name == null )
    {
      throw new ArgumentNullException( nameof( name ) );
    }

    return _includes.TryGetValue( name, out content );
  }

#if NET9_0_OR_GREATER

  /// <summary>
  ///   Attempts to retrieve the content of an include by its name.
  /// </summary>
  /// <param name="name">
  ///   The name of the include to retrieve, represented as a <see cref="ReadOnlySpan{T}" /> of characters.
  ///   Must be a valid macro name.
  /// </param>
  /// <param name="content">
  ///   When this method returns <c>true</c>, contains the content associated with the specified include name.
  ///   When this method returns <c>false</c>, contains <c>null</c>.
  /// </param>
  /// <returns>
  ///   <c>true</c> if an include with the specified name exists; otherwise, <c>false</c>.
  /// </returns>
  /// <remarks>
  ///   This method performs a case-insensitive lookup for the specified include name.
  /// </remarks>
  public bool TryGetIncludeContent(
    ReadOnlySpan<char> name,
    out string? content )
  {
    return _alternate.TryGetValue( name, out content );
  }

#endif

  #endregion
}
