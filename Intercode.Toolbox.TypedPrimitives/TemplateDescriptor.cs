// Module Name: TemplateDescriptor.cs
// Author:      Eduardo Velasquez
// Copyright (c) 2025, Intercode Consulting, Inc.

namespace Intercode.Toolbox.TypedPrimitives;

using System.Collections.Concurrent;
using Intercode.Toolbox.TemplateEngine;

internal class TemplateDescriptor
{
  #region Constants

  private static readonly ConcurrentDictionary<int, TemplateDescriptor> s_cache = new ();

  #endregion

  #region Constructors

  private TemplateDescriptor(
    TemplateType templateType,
    Type primitiveType,
    object? specializationDiscriminator = null )
  {
    PrimitiveType = primitiveType;
    SpecializationDiscriminator = specializationDiscriminator;
    TemplateType = templateType;
    PrimitiveTypeName = primitiveType.FullName!;
    TemplateName = GetTemplateName();

    var isSpecialized = EmbeddedResourceManager.TemplateExists( PrimitiveTypeName, TemplateName );

    TemplateKey = GenerateTemplateKey( isSpecialized, SpecializationDiscriminator );

    ResourcePath = EmbeddedResourceManager.GetTemplateResourcePath(
      isSpecialized ? PrimitiveTypeName : ResourceNames.CommonDirectory,
      TemplateName
    );
  }

  #endregion

  #region Properties

  public Type PrimitiveType { get; }
  public TemplateType TemplateType { get; }
  public object? SpecializationDiscriminator { get; }
  public string PrimitiveTypeName { get; }
  public string TemplateName { get; }
  public string TemplateKey { get; }
  public string ResourcePath { get; }

  #endregion

  #region Public Methods

  public static TemplateDescriptor Create(
    TemplateType templateType,
    Type primitiveType,
    object? specializationDiscriminator = null )
  {
    var key = GenerateKey( primitiveType, templateType, specializationDiscriminator );

    return s_cache.GetOrAdd(
      key,
      _ => new TemplateDescriptor( templateType, primitiveType, specializationDiscriminator )
    );
  }

  public string LoadTemplateText()
  {
    return EmbeddedResourceManager.LoadTemplateResource( ResourcePath );
  }

  public override bool Equals(
    object? obj )
  {
    if( obj is null )
    {
      return false;
    }

    if( ReferenceEquals( this, obj ) )
    {
      return true;
    }

    return obj.GetType() == GetType() && Equals( ( TemplateDescriptor ) obj );
  }

  public override int GetHashCode()
  {
    return GenerateKey( PrimitiveType, TemplateType, SpecializationDiscriminator );
  }

  #endregion

  #region Implementation

  private static int GenerateKey(
    Type primitiveType,
    TemplateType templateType,
    object? discriminator )
  {
    // NOTE: netstandard 2.0 doesn't support HashCode.Combine
    unchecked
    {
      var hash = 17;
      hash = hash * 31 + primitiveType.GetHashCode();
      hash = hash * 31 + ( int ) templateType;
      hash = hash * 31 + ( discriminator?.GetHashCode() ?? 0 );
      return hash;
    }
  }

  protected bool Equals(
    TemplateDescriptor other )
  {
    return PrimitiveType == other.PrimitiveType &&
           TemplateType == other.TemplateType &&
           Equals( SpecializationDiscriminator, other.SpecializationDiscriminator );
  }

  private string GetTemplateName()
  {
    return TemplateType switch
    {
      TemplateType.Main => PrimitiveType.IsValueType
        ? ResourceNames.MainValueTypeTemplate
        : ResourceNames.MainReferenceTypeTemplate,
      TemplateType.TypeConverter           => ResourceNames.TypeConverterTemplate,
      TemplateType.SystemTextJsonConverter => ResourceNames.SystemTextJsonConverterTemplate,
      TemplateType.NewtonsoftJsonConverter => ResourceNames.NewtonsoftJsonConverterTemplate,
      TemplateType.EfCoreValueConverter    => ResourceNames.EfCoreValueConverterTemplate,
      _                                    => throw new ArgumentOutOfRangeException()
    };
  }

  private string GenerateTemplateKey(
    bool isSpecialized,
    object? specializationDiscriminator )
  {
    var builder = StringBuilderPool.Default.Get();

    try
    {
      builder.Append( TemplateName );

      if( isSpecialized )
      {
        Append( PrimitiveTypeName );
      }

      if( specializationDiscriminator != null )
      {
        Append( specializationDiscriminator.ToString() );
      }

      return builder.ToString();
    }
    finally
    {
      StringBuilderPool.Default.Return( builder );
    }

    void Append(
      string value )
    {
      builder.Append( '.' );
      builder.Append( value );
    }
  }

  #endregion
}
