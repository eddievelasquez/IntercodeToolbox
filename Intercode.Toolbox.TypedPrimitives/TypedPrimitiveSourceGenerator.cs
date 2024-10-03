// Module Name: TypedPrimitiveSourceGenerator.cs
// Author:      Eduardo Velasquez
// Copyright (c) 2024, Intercode Consulting, Inc.

namespace Intercode.Toolbox.TypedPrimitives;

using Microsoft.CodeAnalysis;

[Generator]
public sealed class TypedPrimitiveSourceGenerator: IIncrementalGenerator
{
  #region Public Methods

  public void Initialize(
    IncrementalGeneratorInitializationContext context )
  {
    // Get all the readonly structs that are tagged with the marker attribute
    var primitivesToGenerate = context.SyntaxProvider.ForAttributeWithMetadataName(
      Parser.MarkerAttributeFullName,
      Parser.IsGenerationTarget,
      Parser.GetTypedPrimitiveToGenerate
    );

    // Get all the readonly structs that are tagged with the generic marker attribute
    var primitivesToGenerateFromGeneric = context.SyntaxProvider.ForAttributeWithMetadataName(
      Parser.GenericMarkerAttributeFullName,
      Parser.IsGenerationTarget,
      Parser.GetTypedPrimitiveToGenerate
    );

    // Report errors
    RegisterErrorOutput( primitivesToGenerate );
    RegisterErrorOutput( primitivesToGenerateFromGeneric );

    // Generate source code
    RegisterSourceOutput( primitivesToGenerate );
    RegisterSourceOutput( primitivesToGenerateFromGeneric );

    return;

    void RegisterSourceOutput(
      IncrementalValuesProvider<Result<GeneratorModel>> results )
    {
      var values = results.Where( result => result.IsSuccess )
                          .Select(
                            static (
                              result,
                              _ ) => result.Value
                          );

      context.RegisterSourceOutput(
        values,
        static (
          context,
          model ) => Generate( model, context )
      );
    }

    void RegisterErrorOutput(
      IncrementalValuesProvider<Result<GeneratorModel>> results )
    {
      context.RegisterSourceOutput(
        results.SelectMany(
          static (
            result,
            _ ) => result.Errors
        ),
        static (
          context,
          info ) =>
        {
          var diagnostic = info.ToDiagnostic();
          context.ReportDiagnostic( diagnostic );
        }
      );
    }
  }

  #endregion

  #region Implementation

  private static void Generate(
    GeneratorModel model,
    SourceProductionContext context )
  {
    var processor = new TemplateProcessor();

    foreach( var (typeName, sourceText) in processor.ProcessTemplate( model ) )
    {
      context.AddSource( $"{typeName}.g.cs", sourceText );
    }
  }

  #endregion
}
