// Module Name: TypedPrimitiveSourceGenerator.cs
// Author:      Eduardo Velasquez
// Copyright (c) 2024, Intercode Consulting, Inc.

namespace Intercode.Toolbox.TypedPrimitives;

using Intercode.Toolbox.TypedPrimitives.Diagnostics;
using Microsoft.CodeAnalysis;

[Generator]
public sealed class TypedPrimitiveSourceGenerator: IIncrementalGenerator
{
  #region Public Methods

  public void Initialize(
    IncrementalGeneratorInitializationContext initializationContext )
  {
    AddPostInitializationSources( initializationContext );

    // Collect types with marker attributes
    var modelResults = GatherStructsToGenerate( initializationContext );

    // Get errors from failed results
    var failures = modelResults.Where( result => result.IsFailed )
                               .SelectMany(
                                 static (
                                   modelResult,
                                   _ ) => modelResult.Errors
                               );

    // Report diagnostics for failures
    initializationContext.RegisterSourceOutput(
      failures,
      static (
        productionContext,
        diagnosticInfo ) =>
      {
        productionContext.ReportDiagnostic( diagnosticInfo );
      }
    );

    // Get models and check if they are supported
    var successes = modelResults.Where( result => result.IsSuccess )
                                .Select(
                                  static (
                                    result,
                                    _ ) =>
                                  {
                                    var model = result.Value;
                                    TypeManager.TryGetSupportedTypeInfo( model.PrimitiveTypeName, out var typeInfo );
                                    return ( Model: model, TypeInfo: typeInfo );
                                  }
                                );

    // Get errors for unsupported types
    var unsupported = successes.Where( tuple => tuple.TypeInfo is null )
                               .SelectMany(
                                 static (
                                   tuple,
                                   _ ) => Error.UnsupportedTypeFailure( tuple.Model.PrimitiveTypeName ).Errors
                               );

    // Report diagnostics for unsupported types
    initializationContext.RegisterSourceOutput(
      unsupported,
      static (
        productionContext,
        diagnosticInfo ) =>
      {
        productionContext.ReportDiagnostic( diagnosticInfo );
      }
    );

    // Get models for supported types
    var supported = successes.Where( tuple => tuple.TypeInfo is not null )
                             .Select(
                               (
                                 tuple,
                                 _ ) => ( tuple.Model, TypeInfo: tuple.TypeInfo! )
                             );

    // Generate source code for supported type
    initializationContext.RegisterSourceOutput(
      supported,
      static (
        context,
        tuple ) => GenerateSource( context, tuple.Model, tuple.TypeInfo )
    );
  }

  #endregion

  #region Implementation

  private static IncrementalValuesProvider<Result<GeneratorModel>>
    GatherStructsToGenerate(
      IncrementalGeneratorInitializationContext initializationContext )
  {
    // Get all the structs that are tagged with the marker attribute
    var markerPipeline =
      initializationContext.SyntaxProvider.ForAttributeWithMetadataName(
        Parser.MarkerAttributeFullName,
        Parser.IsGenerationTarget,
        Parser.GetTypedPrimitiveToGenerate
      );

    // Get all the structs that are tagged with the generic marker attribute
    var genericMarkerPipeline =
      initializationContext.SyntaxProvider.ForAttributeWithMetadataName(
        Parser.GenericMarkerAttributeFullName,
        Parser.IsGenerationTarget,
        Parser.GetTypedPrimitiveToGenerate
      );

    // Merge the results of both pipelines
    var pipeline = markerPipeline.Merge( genericMarkerPipeline );
    return pipeline;
  }

  private static void AddPostInitializationSources(
    IncrementalGeneratorInitializationContext context )
  {
    context.RegisterPostInitializationOutput(
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
  }

  private static void GenerateSource(
    SourceProductionContext context,
    GeneratorModel model,
    SupportedTypeInfo typeInfo )
  {
    var processor = new TemplateProcessor();

    foreach( var (typeName, sourceText) in
            processor.ProcessTemplate( model, typeInfo ) )
    {
      var hintName = $"{typeName}.g.cs";
      context.AddSource( hintName, sourceText );
    }
  }

  #endregion
}
