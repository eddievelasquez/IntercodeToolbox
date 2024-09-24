// Module Name: TypedConstantExtensions.cs
// Author:      Eduardo Velasquez
// Copyright (c) 2024, Intercode Consulting, Inc.

namespace Intercode.Toolbox.TypedPrimitives;

using System.Collections.Concurrent;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;

internal static class TypedConstantExtensions
{
  #region Constants

  // We use a nullable Type value so we don't have to lookup unknown types multiple times
  private static readonly ConcurrentDictionary<string, Type?> s_cachedTypes;

  #endregion

  #region Constructors

  static TypedConstantExtensions()
  {
    // Preload well-known types
    s_cachedTypes = new ConcurrentDictionary<string, Type?>();
    s_cachedTypes.TryAdd( typeof( Guid ).FullName!, typeof( Guid ) );
    s_cachedTypes.TryAdd( typeof( DateTimeOffset ).FullName!, typeof( DateTimeOffset ) );
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
    this ImmutableArray<KeyValuePair<string, TypedConstant>> arguments,
    string name )
  {
    var typedConstant = arguments.FirstOrDefault( pair => pair.Key == name )
                                 .Value;
    return typedConstant.GetTypeName();
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

  public static Type? GetTypeValue(
    this ImmutableArray<KeyValuePair<string, TypedConstant>> arguments,
    string name )
  {
    var typedConstant = arguments.FirstOrDefault( pair => pair.Key == name )
                                 .Value;
    return typedConstant.GetTypeValue();
  }

  public static Type? GetTypeValue(
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
      var type = namedTypeSymbol.SpecialType switch
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
        _                           => GetTypeFromSymbol( namedTypeSymbol )
      };

      return type;
    }

    return null;
  }

  public static object? GetValue(
    this ImmutableArray<KeyValuePair<string, TypedConstant>> arguments,
    string name )
  {
    var typedConstant = arguments.FirstOrDefault( pair => pair.Key == name )
                                 .Value;
    return typedConstant.GetValue();
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

  private static Type? GetTypeFromSymbol(
    INamedTypeSymbol namedTypeSymbol )
  {
    var typeName = namedTypeSymbol.ToDisplayString();
    var type = s_cachedTypes.GetOrAdd( typeName, s => GetTypeFromSymbolCore( namedTypeSymbol, s ) );
    return type;
  }

  private static Type? GetTypeFromSymbolCore(
    INamedTypeSymbol namedTypeSymbol,
    string typeName )
  {
    // Get metadata reference for the named type symbol
    var containingAssembly = namedTypeSymbol.ContainingAssembly;
    var assemblyName = containingAssembly.Identity.GetDisplayName();

    var assemblies = AppDomain.CurrentDomain.GetAssemblies();
    var assembly = assemblies
      .FirstOrDefault( a => a.FullName == assemblyName );

    if( assembly == null )
    {
      return null;
    }

    return assembly.GetType( typeName );
  }

  #endregion
}
