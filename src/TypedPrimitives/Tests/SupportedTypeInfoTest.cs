// Module Name: SupportedTypeInfoTest.cs
// Author:      Eduardo Velasquez
// Copyright (c) 2025, Intercode Consulting, Inc.

namespace Intercode.Toolbox.TypedPrimitives.Tests;

using System.Collections.Frozen;
using FluentAssertions;

public class SupportedTypeInfoTest
{
  #region Constants

  private static readonly FrozenDictionary<string, string> s_typeConverterMacros =
    new Dictionary<string, string> { { "TCMacro1", "TCValue1" } }.ToFrozenDictionary();

  private static readonly FrozenDictionary<string, string> s_systemTextJsonMacros =
    new Dictionary<string, string> { { "STJMacro1", "STJValue1" } }.ToFrozenDictionary();

  private static readonly FrozenDictionary<string, string> s_typeConverterIncludes =
    new Dictionary<string, string> { { "TCInclude1", "TCIncludeValue1" } }.ToFrozenDictionary();

  private static readonly FrozenDictionary<string, string> s_efCoreIncludes =
    new Dictionary<string, string> { { "EFInclude1", "EFIncludeValue1" } }.ToFrozenDictionary();

  private static readonly FrozenDictionary<TypedPrimitiveConverter, FrozenDictionary<string, string>> s_customMacros =
    new Dictionary<TypedPrimitiveConverter, FrozenDictionary<string, string>>
    {
      [TypedPrimitiveConverter.TypeConverter] = s_typeConverterMacros,
      [TypedPrimitiveConverter.SystemTextJson] = s_systemTextJsonMacros
    }.ToFrozenDictionary();

  private static readonly FrozenDictionary<TypedPrimitiveConverter, FrozenDictionary<string, string>> s_customIncludes =
    new Dictionary<TypedPrimitiveConverter, FrozenDictionary<string, string>>
    {
      [TypedPrimitiveConverter.TypeConverter] = s_typeConverterIncludes,
      [TypedPrimitiveConverter.EfCoreValueConverter] = s_efCoreIncludes
    }.ToFrozenDictionary();

  #endregion

  #region Tests

  [Fact]
  public void Ctor_ShouldSetKeywordProperty()
  {
    var keyword = "int";
    var info = new SupportedTypeInfo( keyword, s_customMacros, s_customIncludes );

    info.Keyword.Should().Be( keyword );
  }

  [Fact]
  public void Ctor_ShouldThrow_WhenCustomConverterMacrosIsNull()
  {
    var act = () => new SupportedTypeInfo( "int", null!, s_customIncludes );
    act.Should().Throw<ArgumentNullException>().WithParameterName( "customConverterMacros" );
  }

  [Fact]
  public void Ctor_ShouldThrow_WhenIncludesIsNull()
  {
    var act = () => new SupportedTypeInfo( "int", s_customMacros, null! );
    act.Should().Throw<ArgumentNullException>().WithParameterName( "includes" );
  }

  [Fact]
  public void GetConverterMacros_ShouldReturnEmpty_WhenConverterDoesNotExist()
  {
    var info = new SupportedTypeInfo( "int", s_customMacros, s_customIncludes );

    var macros = info.GetConverterMacros( TypedPrimitiveConverter.NewtonsoftJson );

    macros.Should().BeEmpty();
  }

  [Fact]
  public void GetConverterMacros_ShouldReturnEmpty_WhenConverterExistsInIncludesButNotMacros()
  {
    var info = new SupportedTypeInfo( "int", s_customMacros, s_customIncludes );
    var macros = info.GetConverterMacros( TypedPrimitiveConverter.EfCoreValueConverter );
    macros.Should().BeEmpty();
  }

  [Fact]
  public void GetConverterMacros_ShouldReturnEmpty_WhenMacrosAreEmpty()
  {
    var emptyMacros = FrozenDictionary<TypedPrimitiveConverter, FrozenDictionary<string, string>>.Empty;
    var info = new SupportedTypeInfo( "int", emptyMacros, s_customIncludes );

    var macros = info.GetConverterMacros( TypedPrimitiveConverter.TypeConverter );

    macros.Should().BeEmpty();
  }

  [Fact]
  public void GetConverterMacros_ShouldReturnMacros_WhenConverterExists()
  {
    var info = new SupportedTypeInfo( "int", s_customMacros, s_customIncludes );

    var macros = info.GetConverterMacros( TypedPrimitiveConverter.TypeConverter );

    macros.Should().BeEquivalentTo( s_typeConverterMacros );
  }

  [Fact]
  public void GetIncludes_ShouldReturnEmpty_WhenConverterDoesNotExist()
  {
    var info = new SupportedTypeInfo( "int", s_customMacros, s_customIncludes );

    var includes = info.GetIncludes( TypedPrimitiveConverter.NewtonsoftJson );

    includes.Should().BeEmpty();
  }

  [Fact]
  public void GetIncludes_ShouldReturnEmpty_WhenConverterExistsInMacrosButNotIncludes()
  {
    var info = new SupportedTypeInfo( "int", s_customMacros, s_customIncludes );
    var includes = info.GetIncludes( TypedPrimitiveConverter.SystemTextJson );
    includes.Should().BeEmpty();
  }

  [Fact]
  public void GetIncludes_ShouldReturnEmpty_WhenIncludesAreEmpty()
  {
    var emptyIncludes = FrozenDictionary<TypedPrimitiveConverter, FrozenDictionary<string, string>>.Empty;
    var info = new SupportedTypeInfo( "int", s_customMacros, emptyIncludes );

    var includes = info.GetIncludes( TypedPrimitiveConverter.TypeConverter );

    includes.Should().BeEmpty();
  }

  [Fact]
  public void GetIncludes_ShouldReturnIncludes_WhenConverterExists()
  {
    var info = new SupportedTypeInfo( "int", s_customMacros, s_customIncludes );

    var includes = info.GetIncludes( TypedPrimitiveConverter.EfCoreValueConverter );

    includes.Should().BeEquivalentTo( s_efCoreIncludes );
  }

  #endregion
}
