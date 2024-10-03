// Module Name: TypedConstantExtensions.cs
// Author:      Eduardo Velasquez
// Copyright (c) 2024, Intercode Consulting, Inc.

namespace Intercode.Toolbox.TypedPrimitives;

using System.Collections.Concurrent;
using System.Collections.Immutable;
using Intercode.Toolbox.TypedPrimitives.Diagnostics;
using Microsoft.CodeAnalysis;

internal static class TypedConstantExtensions
{
  #region Constants

  // We use a nullable Type value so we don't have to lookup unknown types multiple times
  private static readonly ConcurrentDictionary<string, Result<Type>> s_cachedTypes;

  #endregion

  #region Constructors

  static TypedConstantExtensions()
  {
    // Preload well-known types
    s_cachedTypes = new ConcurrentDictionary<string, Result<Type>>();
    s_cachedTypes.TryAdd( typeof( Guid ).FullName!, Result.Ok( typeof( Guid ) ) );
    s_cachedTypes.TryAdd( typeof( DateTimeOffset ).FullName!, Result.Ok( typeof( DateTimeOffset ) ) );
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

  public static string? GetTypeName(
    this TypedConstant typedConstant )
  {
    if( typedConstant.IsNull )
    {
      return null;
    }

    if( typedConstant.Kind != TypedConstantKind.Type )
    {
      return null;
    }

    if( typedConstant.Value is INamedTypeSymbol namedTypeSymbol )
    {
      return namedTypeSymbol.SpecialType switch
      {
        SpecialType.System_Boolean  => typeof( bool ).FullName!,
        SpecialType.System_Byte     => typeof( byte ).FullName!,
        SpecialType.System_Char     => typeof( char ).FullName!,
        SpecialType.System_DateTime => typeof( DateTime ).FullName!,
        SpecialType.System_Decimal  => typeof( decimal ).FullName!,
        SpecialType.System_Double   => typeof( double ).FullName!,
        SpecialType.System_Int16    => typeof( short ).FullName!,
        SpecialType.System_Int32    => typeof( int ).FullName!,
        SpecialType.System_Int64    => typeof( long ).FullName!,
        SpecialType.System_SByte    => typeof( sbyte ).FullName!,
        SpecialType.System_Single   => typeof( float ).FullName!,
        SpecialType.System_String   => typeof( string ).FullName!,
        SpecialType.System_UInt16   => typeof( ushort ).FullName!,
        SpecialType.System_UInt32   => typeof( uint ).FullName!,
        SpecialType.System_UInt64   => typeof( ulong ).FullName!,
        _                           => namedTypeSymbol.ToDisplayString()
      };
    }

    return null;
  }

  public static Result<Type> GetTypeValue(
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
      return typeSymbol.GetTypeValue();
    }

    return Result.Fail<Type>( Error.UnsupportedType( typedConstant.Value!.ToString() ) );
  }

  public static Result<Type> GetTypeValue(
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

  #endregion

  #region Implementation

  private static Result<Type> GetTypeFromSymbol(
    ITypeSymbol namedTypeSymbol )
  {
    var typeName = namedTypeSymbol.ToDisplayString();
    var type = s_cachedTypes.GetOrAdd( typeName, s => GetTypeFromSymbolCore( namedTypeSymbol, s ) );
    return type;
  }

  private static Result<Type> GetTypeFromSymbolCore(
    ITypeSymbol namedTypeSymbol,
    string typeName )
  {
    // Get metadata reference for the named type symbol
    var containingAssembly = namedTypeSymbol.ContainingAssembly;
    if( containingAssembly is null )
    {
      return Result.Fail<Type>( Error.Unexpected( $"The containing assembly for the '{typeName}' was not found" ) );
    }

    var assemblyName = containingAssembly.Identity.GetDisplayName();

    var assemblies = AppDomain.CurrentDomain.GetAssemblies();
    var assembly = assemblies
      .FirstOrDefault( a => a.FullName == assemblyName );

    if( assembly == null )
    {
      return Result.Fail<Type>( Error.Unexpected( $"The '{assemblyName}' assembly was not found" ) );
    }

    var type = assembly.GetType( typeName );
    if( type is null )
    {
      return Result.Fail<Type>( Error.Unexpected( $"The '{typeName}' type was not found" ) );
    }

    return Result.Ok( type );
  }

  #endregion
}
