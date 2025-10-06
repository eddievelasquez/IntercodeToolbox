// Module Name: TemplateDescriptorTest.cs
// Author:      Eduardo Velasquez
// Copyright (c) 2025, Intercode Consulting, Inc.

namespace Intercode.Toolbox.TypedPrimitives.Tests;

using FluentAssertions;

public class TemplateDescriptorTest
{
  #region Tests

  [Fact]
  public void Create_ShouldReturnDifferentInstances_WhenDiscriminatorDiffers()
  {
    var a = TemplateDescriptor.Create( TemplateType.Main, typeof( int ), "A" );
    var b = TemplateDescriptor.Create( TemplateType.Main, typeof( int ), "B" );
    a.Should().NotBeSameAs( b );
  }

  [Fact]
  public void Create_ShouldReturnDifferentInstances_WhenPrimitiveTypeDiffers()
  {
    var a = TemplateDescriptor.Create( TemplateType.Main, typeof( int ) );
    var b = TemplateDescriptor.Create( TemplateType.Main, typeof( long ) );
    a.Should().NotBeSameAs( b );
  }

  [Fact]
  public void Create_ShouldReturnDifferentInstances_WhenTemplateTypeDiffers()
  {
    var a = TemplateDescriptor.Create( TemplateType.Main, typeof( int ) );
    var b = TemplateDescriptor.Create( TemplateType.TypeConverter, typeof( int ) );
    a.Should().NotBeSameAs( b );
  }

  [Fact]
  public void Create_ShouldReturnSameInstance_WhenCalledWithSameArguments()
  {
    var a = TemplateDescriptor.Create( TemplateType.Main, typeof( int ) );
    var b = TemplateDescriptor.Create( TemplateType.Main, typeof( int ) );
    a.Should().BeSameAs( b );
  }

  [Fact]
  public void Equals_ShouldReturnFalse_ForDifferentArguments()
  {
    var a = TemplateDescriptor.Create( TemplateType.Main, typeof( int ) );
    var b = TemplateDescriptor.Create( TemplateType.TypeConverter, typeof( int ) );
    a.Equals( b ).Should().BeFalse();
  }

  [Fact]
  public void Equals_ShouldReturnTrue_ForSameArguments()
  {
    var a = TemplateDescriptor.Create( TemplateType.Main, typeof( int ) );
    var b = TemplateDescriptor.Create( TemplateType.Main, typeof( int ) );
    a.Equals( b ).Should().BeTrue();
    a.GetHashCode().Should().Be( b.GetHashCode() );
  }

  [Fact]
  public void GetTemplateName_ShouldReturnExpectedName_ForEfCoreValueConverter()
  {
    TemplateDescriptor.Create( TemplateType.EfCoreValueConverter, typeof( int ) )
                      .TemplateName.Should()
                      .Be( ResourceNames.EfCoreValueConverterTemplate );
  }

  [Fact]
  public void GetTemplateName_ShouldReturnExpectedName_ForNewtonsoftJsonConverter()
  {
    TemplateDescriptor.Create( TemplateType.NewtonsoftJsonConverter, typeof( int ) )
                      .TemplateName.Should()
                      .Be( ResourceNames.NewtonsoftJsonConverterTemplate );
  }

  [Fact]
  public void GetTemplateName_ShouldReturnExpectedName_ForSystemTextJsonConverter()
  {
    TemplateDescriptor.Create( TemplateType.SystemTextJsonConverter, typeof( int ) )
                      .TemplateName.Should()
                      .Be( ResourceNames.SystemTextJsonConverterTemplate );
  }

  [Fact]
  public void GetTemplateName_ShouldReturnExpectedName_ForTypeConverter()
  {
    TemplateDescriptor.Create( TemplateType.TypeConverter, typeof( int ) )
                      .TemplateName.Should()
                      .Be( ResourceNames.TypeConverterTemplate );
  }

  [Fact]
  public void GetTemplateName_ShouldReturnReferenceTypeTemplate_ForReferenceType()
  {
    var descriptor = TemplateDescriptor.Create( TemplateType.Main, typeof( string ) );
    descriptor.TemplateName.Should().Be( ResourceNames.MainReferenceTypeTemplate );
  }

  [Fact]
  public void GetTemplateName_ShouldReturnValueTypeTemplate_ForValueType()
  {
    var descriptor = TemplateDescriptor.Create( TemplateType.Main, typeof( int ) );
    descriptor.TemplateName.Should().Be( ResourceNames.MainValueTypeTemplate );
  }

  [Fact]
  public void LoadTemplateText_ShouldReturnTemplateContent()
  {
    var descriptor = TemplateDescriptor.Create( TemplateType.TypeConverter, typeof( int ) );
    var text = descriptor.LoadTemplateText();
    text.Should().NotBeNullOrWhiteSpace();
  }

  [Fact]
  public void TemplateKey_ShouldAppendDiscriminator_WhenProvided()
  {
    var descriptor = TemplateDescriptor.Create( TemplateType.Main, typeof( int ), "Spec" );
    descriptor.TemplateKey.Should().EndWith( ".Spec" );
  }

  [Fact]
  public void TemplateKey_ShouldStartWithTemplateName()
  {
    var descriptor = TemplateDescriptor.Create( TemplateType.Main, typeof( int ) );
    descriptor.TemplateKey.Should().StartWith( descriptor.TemplateName );
  }

  #endregion
}
