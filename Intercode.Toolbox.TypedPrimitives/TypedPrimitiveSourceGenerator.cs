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
    // Filter for all readonly record structs that the maker attribute
    var primitivesToGenerate = context.SyntaxProvider.ForAttributeWithMetadataName(
      Parser.MarkerAttributeFullName,
      Parser.IsGenerationTarget,
      Parser.GetTypedPrimitiveToGenerate
    );

    context.RegisterSourceOutput(
      primitivesToGenerate,
      static (
        context,
        result ) => Generate( result, context )
    );
  }

  #endregion

  #region Implementation

  private static void Generate(
    Result<GeneratorModel> primitiveToGenerate,
    SourceProductionContext context )
  {
    if( primitiveToGenerate.IsFailed )
    {
      foreach( var diagnosticInfo in primitiveToGenerate.Errors )
      {
        var diagnostic = diagnosticInfo.ToDiagnostic();
        context.ReportDiagnostic( diagnostic );
      }

      return;
    }

    var model = primitiveToGenerate.Value;
    var processor = new TemplateProcessor();
    var content = processor.ProcessTemplate( model );
    context.AddSource( $"{model.Namespace}.{model.Name}.g.cs", content );
  }

  #endregion
}
