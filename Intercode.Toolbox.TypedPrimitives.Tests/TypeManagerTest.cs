// Module Name: TypeManagerTest.cs
// Author:      Eduardo Velasquez
// Copyright (c) 2025, Intercode Consulting, Inc.

namespace Intercode.Toolbox.TypedPrimitives.Tests;

using FluentAssertions;

public class TypeManagerTest
{
  #region Tests

  [Fact]
  public void IsSupported_ShouldReturnFalse_WhenTypeIsNotSupported()
  {
    TypeManager.IsSupported( typeof( bool ) ).Should().BeFalse();
    TypeManager.IsSupported( typeof( object ) ).Should().BeFalse();
  }

  [Theory]
  [MemberData( nameof( SupportedTypesData ) )]
  public void IsSupported_ShouldReturnTrue_WhenTypeIsSupported(
    Type type,
    string _ )
  {
    TypeManager.IsSupported( type ).Should().BeTrue();
  }

  [Fact]
  public void TryGetSupportedTypeInfo_ShouldProvideConverterMacrosAndIncludes_WhenTypeIsSupported()
  {
    TypeManager.TryGetSupportedTypeInfo( typeof( int ), out var info ).Should().BeTrue();
    info.Should().NotBeNull();

    // System.Text.Json macros
    var stjMacros = info!.GetConverterMacros( TypedPrimitiveConverter.SystemTextJson )
                         .ToDictionary( kv => kv.Key, kv => kv.Value );
    stjMacros.Should().ContainKeys( "SystemTextJsonTokenType", "SystemTextJsonReader", "SystemTextJsonWriter" );

    // Newtonsoft.Json macros
    var nsjMacros = info.GetConverterMacros( TypedPrimitiveConverter.NewtonsoftJson )
                        .ToDictionary( kv => kv.Key, kv => kv.Value );
    nsjMacros.Should().ContainKeys( "NewtonsoftJsonTokenType", "NewtonsoftJsonReader", "NewtonsoftJsonWriter" );

    // Includes
    var typeConverterIncludes = info.GetIncludes( TypedPrimitiveConverter.TypeConverter )
                                    .ToDictionary( kv => kv.Key, kv => kv.Value );
    typeConverterIncludes.Should().ContainKey( "TypeConverterAttribute" );

    var stjIncludes = info.GetIncludes( TypedPrimitiveConverter.SystemTextJson )
                          .ToDictionary( kv => kv.Key, kv => kv.Value );
    stjIncludes.Should().ContainKey( "SystemTextJsonConverterAttribute" );

    var nsjIncludes = info.GetIncludes( TypedPrimitiveConverter.NewtonsoftJson )
                          .ToDictionary( kv => kv.Key, kv => kv.Value );
    nsjIncludes.Should().ContainKey( "NewtonsoftJsonConverterAttribute" );
  }

  [Fact]
  public void TryGetSupportedTypeInfo_ShouldReturnFalseAndNullInfo_WhenTypeIsNotSupported()
  {
    var result = TypeManager.TryGetSupportedTypeInfo( typeof( bool ), out var info );

    result.Should().BeFalse();
    info.Should().BeNull();
  }

  [Theory]
  [MemberData( nameof( SupportedTypesData ) )]
  public void TryGetSupportedTypeInfo_ShouldReturnInfo_WhenTypeIsSupported(
    Type type,
    string expectedKeyword )
  {
    var result = TypeManager.TryGetSupportedTypeInfo( type, out var info );

    result.Should().BeTrue();
    info.Should().NotBeNull();
    info!.Keyword.Should().Be( expectedKeyword );
  }

  #endregion

  #region Implementation

  public static TheoryData<Type, string> SupportedTypesData { get; } = new ()
  {
    { typeof( byte ), "byte" },
    { typeof( sbyte ), "sbyte" },
    { typeof( short ), "short" },
    { typeof( ushort ), "ushort" },
    { typeof( int ), "int" },
    { typeof( uint ), "uint" },
    { typeof( long ), "long" },
    { typeof( ulong ), "ulong" },
    { typeof( float ), "float" },
    { typeof( double ), "double" },
    { typeof( decimal ), "decimal" },
    { typeof( string ), "string" },
    { typeof( Guid ), "global::System.Guid" },
    { typeof( DateTime ), "global::System.DateTime" },
    { typeof( DateTimeOffset ), "global::System.DateTimeOffset" },
    { typeof( TimeSpan ), "global::System.TimeSpan" },
    { typeof( Uri ), "global::System.Uri" }
  };

  #endregion
}
