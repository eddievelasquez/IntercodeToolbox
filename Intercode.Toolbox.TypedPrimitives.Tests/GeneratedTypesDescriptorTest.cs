// Module Name: GeneratedTypesDescriptorTest.cs
// Author:      GitHub Copilot
//
// Tests for Intercode.Toolbox.TypedPrimitives.GeneratedTypesDescriptor

namespace Intercode.Toolbox.TypedPrimitives.Tests;

using FluentAssertions;
using Intercode.Toolbox.TemplateEngine;

public class GeneratedTypesDescriptorTest
{
  [Fact]
  public void Ctor_ShouldThrow_WhenPrimitiveTypeUnsupported()
  {
    var model = new GeneratorModel(
      typeof(object),
      "MyObject",
      "Test.Namespace",
      TypedPrimitiveConverter.None,
      "StringComparison.Ordinal"
    );

    var act = () => new GeneratedTypesDescriptor( model );

    act.Should()
       .Throw<InvalidOperationException>()
       .WithMessage("*is not a supported type*");
  }

  [Fact]
  public void GetTemplateTypeDescriptor_ShouldReturnDifferentDescriptors_ForMain_WhenConvertersDiffer()
  {
    var modelA = new GeneratorModel(
      typeof(int),
      "MyInt",
      "Test.Namespace",
      TypedPrimitiveConverter.SystemTextJson,
      "StringComparison.Ordinal"
    );

    var modelB = new GeneratorModel(
      typeof(int),
      "MyInt",
      "Test.Namespace",
      TypedPrimitiveConverter.TypeConverter,
      "StringComparison.Ordinal"
    );

    var descriptorA = new GeneratedTypesDescriptor( modelA );
    var descriptorB = new GeneratedTypesDescriptor( modelB );

    var tdA = descriptorA.GetTemplateTypeDescriptor( TemplateType.Main );
    var tdB = descriptorB.GetTemplateTypeDescriptor( TemplateType.Main );

    tdA.Equals( tdB ).Should().BeFalse("Main template descriptors are specialized by the converters set");
  }

  [Fact]
  public void GetTemplateTypeDescriptor_ShouldReturnEqualDescriptors_ForNonMain_WhenConvertersDiffer()
  {
    var modelA = new GeneratorModel(
      typeof(int),
      "MyInt",
      "Test.Namespace",
      TypedPrimitiveConverter.SystemTextJson,
      "StringComparison.Ordinal"
    );

    var modelB = new GeneratorModel(
      typeof(int),
      "MyInt",
      "Test.Namespace",
      TypedPrimitiveConverter.TypeConverter,
      "StringComparison.Ordinal"
    );

    var descriptorA = new GeneratedTypesDescriptor( modelA );
    var descriptorB = new GeneratedTypesDescriptor( modelB );

    var tdA = descriptorA.GetTemplateTypeDescriptor( TemplateType.SystemTextJsonConverter );
    var tdB = descriptorB.GetTemplateTypeDescriptor( TemplateType.SystemTextJsonConverter );

    tdA.Equals( tdB ).Should().BeTrue("converter templates are not specialized by the converters set");
  }

  [Fact]
  public void AddIncludes_ShouldAddOnlyEnabledIncludes()
  {
    var model = new GeneratorModel(
      typeof(int),
      "MyInt",
      "Test.Namespace",
      // Enable only TypeConverter
      TypedPrimitiveConverter.TypeConverter,
      "StringComparison.Ordinal"
    );

    var descriptor = new GeneratedTypesDescriptor( model );

    var includes = new IncludesCollection();
    descriptor.AddIncludes( includes );

    includes.TryGetIncludeContent( MacroNames.TypeConverterAttribute.AsSpan(), out var typeConv ).Should().BeTrue();
    typeConv.Should().NotBeNull().And.Contain("TypeConverter(");

    includes.TryGetIncludeContent( MacroNames.SystemTextJsonConverterAttribute.AsSpan(), out var stjAttr ).Should().BeTrue();
    stjAttr.Should().BeNull("STJ converter is disabled");

    includes.TryGetIncludeContent( MacroNames.NewtonsoftJsonConverterAttribute.AsSpan(), out var nsjAttr ).Should().BeTrue();
    nsjAttr.Should().BeNull("Newtonsoft converter is disabled");
  }

  [Fact]
  public void CreateMacroValues_ShouldPopulateCommonAndConverterMacros()
  {
    var model = new GeneratorModel(
      typeof(int),
      "MyInt",
      "Test.Namespace",
      // Enable only System.Text.Json converter
      TypedPrimitiveConverter.SystemTextJson,
      "StringComparison.Ordinal"
    );

    var descriptor = new GeneratedTypesDescriptor( model );

    var macroTable = new MacroTableBuilder().DeclareTemplateProcessorMacros().Build();
    var values = descriptor.CreateMacroValues( macroTable );

    values.GetValue( MacroNames.PrimitiveName ).Should().Be("int");
    values.GetValue( MacroNames.TypedPrimitiveNamespace ).Should().Be("Test.Namespace");
    values.GetValue( MacroNames.TypedPrimitiveName ).Should().Be("MyInt");
    values.GetValue( MacroNames.TypedPrimitiveQualifiedName ).Should().Be("Test.Namespace.MyInt");
    values.GetValue( MacroNames.StringComparison ).Should().Be("StringComparison.Ordinal");

    // STJ macros should be set for int
    values.GetValue( MacroNames.SystemTextJsonTokenType ).Should().Be("Number");
    values.GetValue( MacroNames.SystemTextJsonReader ).Should().Be("reader.GetInt32()");
    values.GetValue( MacroNames.SystemTextJsonWriter ).Should().Contain("WriteNumberValue");

    // Newtonsoft macros should be null when converter is disabled
    values.GetValue( MacroNames.NewtonsoftJsonTokenType ).Should().BeNull();
    values.GetValue( MacroNames.NewtonsoftJsonReader ).Should().BeNull();
    values.GetValue( MacroNames.NewtonsoftJsonWriter ).Should().BeNull();
  }
}
