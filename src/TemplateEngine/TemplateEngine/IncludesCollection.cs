// Module Name: IncludesCollection.cs
// Author:      Eduardo Velasquez
// Copyright (c) 2025, Intercode Consulting, Inc.

namespace Intercode.Toolbox.TemplateEngine;

/// <summary>
///   Represents a collection of includes that can be used in a template engine.
/// </summary>
/// <remarks>
///   This class provides functionality to manage includes by their names, allowing for the addition,
///   retrieval, and validation of includes. Names of includes must adhere to specific
///   validation rules to ensure they are valid macro names.
/// </remarks>
public sealed class IncludesCollection
{
  #region Fields

  private readonly Dictionary<string, MacroValueGenerator?> _includes = new ( StringComparer.OrdinalIgnoreCase );

#if NET9_0_OR_GREATER
  private readonly Dictionary<string, MacroValueGenerator?>.AlternateLookup<ReadOnlySpan<char>> _alternate;
#endif

  #endregion

  #region Constructors

  /// <summary>
  ///   Initializes a new instance of the <see cref="IncludesCollection" /> class.
  /// </summary>
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
  /// <param name="name">The name of the included content. Must be a valid macro name.</param>
  /// <param name="content">The content to be included. Can be <c>null</c>.</param>
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

    MacroValueGenerator? generator = null;

    if( content != null )
    {
      generator = _ => content;
    }

    AddIncludeImpl( name, generator );
  }

  /// <summary>
  ///   Adds a new include to the collection with the specified name and an optional generator.
  /// </summary>
  /// <param name="name">The name of the included content. Must be a valid macro name.</param>
  /// <param name="generator">
  ///   An optional <see cref="MacroValueGenerator" /> used to dynamically generate the included content.
  ///   If <c>null</c>, the content will be empty.
  /// </param>
  /// <exception cref="ArgumentException">
  ///   Thrown if the <paramref name="name" /> is not a valid macro name.
  /// </exception>
  /// <remarks>
  ///   This method allows adding dynamic macros to the collection, where the content can be generated
  ///   at runtime using the provided generator.
  /// </remarks>
  public void AddInclude(
    string name,
    MacroValueGenerator? generator = null )
  {
    MacroExtensions.ValidateMacroName( name );

    AddIncludeImpl( name, generator );
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
  internal bool TryGetIncludeContent(
    string name,
    out string? content )
  {
    if( name == null )
    {
      throw new ArgumentNullException( nameof( name ) );
    }

    if( _includes.TryGetValue( name, out var generator ) )
    {
      content = generator?.Invoke( ReadOnlySpan<char>.Empty );
      return true;
    }

    content = null;
    return false;
  }

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
#if NET9_0_OR_GREATER
    if( _alternate.TryGetValue( name, out var generator ) )
    {
      content = generator?.Invoke( name );
      return true;
    }

    content = null;
    return false;
#else
    return TryGetIncludeContent( name.ToString(), out content );
#endif
  }

  #endregion

  #region Implementation

  private void AddIncludeImpl(
    string name,
    MacroValueGenerator? generator )
  {
    _includes[name] = generator;
  }

  #endregion
}
