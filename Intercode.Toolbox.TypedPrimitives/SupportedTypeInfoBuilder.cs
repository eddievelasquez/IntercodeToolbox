// Module Name: SupportedTypeBuilder.cs
// Author:      Eduardo Velasquez
// Copyright (c) 2024, Intercode Consulting, Inc.

namespace Intercode.Toolbox.TypedPrimitives;

using System.Collections.Frozen;

internal class SupportedTypeInfoBuilder
{
  #region Fields

  private readonly string _typeName;
  private readonly Dictionary<TypedPrimitiveConverter, Dictionary<string, string>> _converterMacros = new ();
  private string? _typeKeyword;

  #endregion

  #region Constructors

  public SupportedTypeInfoBuilder(
    Type type )
  {
    _typeName = type.FullName ?? throw new ArgumentNullException( nameof( type ) );
  }

  public SupportedTypeInfoBuilder(
    string typeName )
  {
    if( string.IsNullOrWhiteSpace( typeName ) )
    {
      throw new ArgumentException( "Type name cannot be null or whitespace.", nameof( typeName ) );
    }

    if( !typeName.Contains( '.' ) )
    {
      throw new ArgumentException( "Type name must be fully qualified.", nameof( typeName ) );
    }

    _typeName = typeName;
  }

  #endregion

  #region Public Methods

  public SupportedTypeInfoBuilder AddTypeKeyword(
    string keyword )
  {
    _typeKeyword = keyword;
    return this;
  }

  public SupportedTypeInfoBuilder AddConverterCustomMacros(
    TypedPrimitiveConverter converter,
    params KeyValuePair<string, string>[] macros )
  {
    if( !_converterMacros.TryGetValue( converter, out var converterMacros ) )
    {
      converterMacros = new Dictionary<string, string>();
      _converterMacros.Add( converter, converterMacros );
    }

    foreach( var pair in macros )
    {
      converterMacros.Add( pair.Key, pair.Value );
    }

    return this;
  }

  public SupportedTypeInfo Build()
  {
    var keyword = _typeKeyword ?? GetKeyword( _typeName );
    var customMacros =
      _converterMacros.ToFrozenDictionary( pair => pair.Key, pair => pair.Value.ToFrozenDictionary() );

    return new SupportedTypeInfo( keyword, customMacros );
  }

  #endregion

  #region Implementation

  private static string GetKeyword(
    string typeName )
  {
    return typeName switch
    {
      "System.Boolean" => "bool",
      "System.Byte"    => "byte",
      "System.Char"    => "char",
      "System.Decimal" => "decimal",
      "System.Double"  => "double",
      "System.Int16"   => "short",
      "System.Int32"   => "int",
      "System.Int64"   => "long",
      "System.SByte"   => "sbyte",
      "System.Single"  => "float",
      "System.String"  => "string",
      "System.UInt16"  => "ushort",
      "System.UInt32"  => "uint",
      "System.UInt64"  => "ulong",
      _                => $"global::{typeName}"
    };
  }

  private static string GetKeyword(
    Type type )
  {
    return Type.GetTypeCode( type ) switch
    {
      TypeCode.Boolean => "bool",
      TypeCode.Byte    => "byte",
      TypeCode.Char    => "char",
      TypeCode.Decimal => "decimal",
      TypeCode.Double  => "double",
      TypeCode.Int16   => "short",
      TypeCode.Int32   => "int",
      TypeCode.Int64   => "long",
      TypeCode.SByte   => "sbyte",
      TypeCode.Single  => "float",
      TypeCode.String  => "string",
      TypeCode.UInt16  => "ushort",
      TypeCode.UInt32  => "uint",
      TypeCode.UInt64  => "ulong",
      _                => $"global::{type.FullName ?? type.Name}"
    };
  }

  #endregion
}
