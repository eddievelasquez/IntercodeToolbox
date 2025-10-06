// Module Name: TypedConstantExtensions.cs
// Author:      Eduardo Velasquez
// Copyright (c) 2024, Intercode Consulting, Inc.

namespace Intercode.Toolbox.TypedPrimitives;

using System.Collections.Frozen;
using System.Collections.Immutable;
using Intercode.Toolbox.TypedPrimitives.Diagnostics;
using Microsoft.CodeAnalysis;

internal static class TypedConstantExtensions
{
  #region Constants

  private static readonly SymbolDisplayFormat s_symbolDisplayFormat = new (
    typeQualificationStyle: SymbolDisplayTypeQualificationStyle.NameAndContainingTypesAndNamespaces
  );

  private static readonly FrozenDictionary<string, Type> s_cachedTypes;

  #endregion

  #region Constructors

  static TypedConstantExtensions()
  {
    // Preload supported types that aren't detected by using the SpecialType enum
    s_cachedTypes =
      new[] { typeof( Guid ), typeof( DateTimeOffset ), typeof( TimeSpan ), typeof( Uri ) }.ToFrozenDictionary(
        type => type.FullName!
      );
  }

  #endregion

  #region Public Methods

  public static T? GetEnumValue<T>(
    this ImmutableArray<KeyValuePair<string, TypedConstant>> arguments,
    string name )
    where T: struct, Enum
  {
    var typedConstant = arguments.FirstOrDefault( pair => pair.Key == name )
                                 .Value;

    if( typedConstant.IsNull )
    {
      return null;
    }

    if( typedConstant.Kind != TypedConstantKind.Enum )
    {
      return null;
    }

    return ( T ) typedConstant.Value!;
  }

  public static T? GetValue<T>(
    this ImmutableArray<KeyValuePair<string, TypedConstant>> arguments,
    string name )
    where T: struct
  {
    var typedConstant = arguments.FirstOrDefault( pair => pair.Key == name )
                                 .Value;

    if( typedConstant.IsNull )
    {
      return null;
    }

    if( typedConstant.Kind != TypedConstantKind.Primitive )
    {
      return null;
    }

    return ( T ) typedConstant.Value!;
  }

  public static object? GetValue(
    this TypedConstant typedConstant )
  {
    if( typedConstant.IsNull )
    {
      return null;
    }

    return typedConstant.Kind switch
    {
      TypedConstantKind.Primitive => typedConstant.Value,
      TypedConstantKind.Enum      => typedConstant.Value,
      _                           => null
    };
  }

  public static Result<Type> GetTypeOf(
    this TypedConstant typedConstant )
  {
    if( typedConstant.IsNull )
    {
      return Result.Fail<Type>( Error.Unexpected( "The primitive type should not be null" ) );
    }

    if( typedConstant.Kind != TypedConstantKind.Type )
    {
      return Result.Fail<Type>( Error.Unexpected( "The primitive type is not a type" ) );
    }

    if( typedConstant.Value is INamedTypeSymbol typeSymbol )
    {
      return typeSymbol.GetTypeOf();
    }

    return Result.Fail<Type>( Error.UnsupportedType( typedConstant.Value!.ToString() ) );
  }

  public static Result<Type> GetTypeOf(
    this ITypeSymbol typeSymbol )
  {
    var type = typeSymbol.SpecialType switch
    {
      SpecialType.System_Boolean  => typeof( bool ),
      SpecialType.System_Byte     => typeof( byte ),
      SpecialType.System_Char     => typeof( char ),
      SpecialType.System_DateTime => typeof( DateTime ),
      SpecialType.System_Decimal  => typeof( decimal ),
      SpecialType.System_Double   => typeof( double ),
      SpecialType.System_Int16    => typeof( short ),
      SpecialType.System_Int32    => typeof( int ),
      SpecialType.System_Int64    => typeof( long ),
      SpecialType.System_SByte    => typeof( sbyte ),
      SpecialType.System_Single   => typeof( float ),
      SpecialType.System_String   => typeof( string ),
      SpecialType.System_UInt16   => typeof( ushort ),
      SpecialType.System_UInt32   => typeof( uint ),
      SpecialType.System_UInt64   => typeof( ulong ),
      _                           => null
    };

    if( type is null )
    {
      return GetTypeFromSymbol( typeSymbol );
    }

    return Result.Ok( type );
  }

  #endregion

  #region Implementation

  private static Result<Type> GetTypeFromSymbol(
    ITypeSymbol namedTypeSymbol )
  {
    var typeName = namedTypeSymbol.ToDisplayString();

    if( s_cachedTypes.TryGetValue( typeName, out var type ) )
    {
      return Result.Ok( type );
    }

    return Result.Fail<Type>( Error.UnsupportedType( namedTypeSymbol.ToDisplayString( s_symbolDisplayFormat ) ) );
  }

  #endregion
}
