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
    context.RegisterPostInitializationOutput(
      static initializationContext =>
      {
        var source = EmbeddedResourceManager.LoadTextResource( "TypedPrimitiveAttribute.cs" );
        var hintName = $"{typeof( TypedPrimitiveAttribute ).FullName!}.g.cs";

        initializationContext.AddSource( hintName, source );
      }
    );

    // Get all the readonly structs that are tagged with the marker attribute
    var pipeline = context.SyntaxProvider.ForAttributeWithMetadataName(
      Parser.MarkerAttributeFullName,
      Parser.IsGenerationTarget,
      Parser.GetTypedPrimitiveToGenerate
    );

    // Get all the readonly structs that are tagged with the generic marker attribute
    var genericPipeline = context.SyntaxProvider.ForAttributeWithMetadataName(
      Parser.GenericMarkerAttributeFullName,
      Parser.IsGenerationTarget,
      Parser.GetTypedPrimitiveToGenerate
    );

    // Report errors
    RegisterErrorOutput( pipeline );
    RegisterErrorOutput( genericPipeline );

    // Generate source code
    RegisterSourceOutput( pipeline );
    RegisterSourceOutput( genericPipeline );

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
      var hintName = $"{typeName}.g.cs";
      context.AddSource( hintName, sourceText );
    }
  }

  #endregion
}
