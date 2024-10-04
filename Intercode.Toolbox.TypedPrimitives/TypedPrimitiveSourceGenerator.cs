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
    IncrementalGeneratorInitializationContext initializationContext )
  {
    initializationContext.RegisterPostInitializationOutput(
      static context =>
      {
        AddSource( typeof( TypedPrimitiveAttribute ) );
        return;

        void AddSource(
          Type type )
        {
          var source = EmbeddedResourceManager.LoadTextResource( $"{type.Name}.cs" );
          var hintName = $"{type.FullName!}.g.cs";

          context.AddSource( hintName, source );
        }
      }
    );

    // Get all the structs that are tagged with the marker attribute
    var markerPipeline = initializationContext.SyntaxProvider.ForAttributeWithMetadataName(
      Parser.MarkerAttributeFullName,
      Parser.IsGenerationTarget,
      Parser.GetTypedPrimitiveToGenerate
    );

    // Get all the structs that are tagged with the generic marker attribute
    var genericMarkerPipeline = initializationContext.SyntaxProvider.ForAttributeWithMetadataName(
      Parser.GenericMarkerAttributeFullName,
      Parser.IsGenerationTarget,
      Parser.GetTypedPrimitiveToGenerate
    );

    // Combine the results of both pipelines
    var pipeline = markerPipeline.Collect()
                                 .Combine( genericMarkerPipeline.Collect() )
                                 .SelectMany(
                                   (
                                     tuple,
                                     _ ) => tuple.Left.Concat( tuple.Right )
                                 );

    // Report diagnostics from failed results
    initializationContext.RegisterSourceOutput(
      pipeline.SelectMany(
        static (
          result,
          _ ) => result.Errors
      ),
      static (
        context,
        diagnosticInfo ) =>
      {
        var diagnostic = diagnosticInfo.ToDiagnostic();
        context.ReportDiagnostic( diagnostic );
      }
    );

    // Generate source code from successful results
    var toGenerate = pipeline.Where( result => result.IsSuccess )
                             .Select(
                               static (
                                 result,
                                 _ ) => result.Value
                             );

    initializationContext.RegisterSourceOutput(
      toGenerate,
      static (
        context,
        model ) => GenerateSource( model, context )
    );

    return;
  }

  #endregion

  #region Implementation

  private static void GenerateSource(
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
