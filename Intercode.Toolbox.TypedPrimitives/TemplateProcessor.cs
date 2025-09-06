// Module Name: TemplateProcessor.cs
// Author:      Eduardo Velasquez
// Copyright (c) 2025, Intercode Consulting, Inc.

namespace Intercode.Toolbox.TypedPrimitives;

using System.Collections.Concurrent;
using System.Text;
using Intercode.Toolbox.TemplateEngine;
using Microsoft.CodeAnalysis.Text;

/// <summary>
///   Processes templates for generating typed primitive source code, including converters and main struct definitions.
/// </summary>
internal class TemplateProcessor
{
  #region Fields

  // Caches compiled templates by key for reuse and performance
  private readonly ConcurrentDictionary<string, Template> _templateCache = new ();

  // Context for macro processing, shared across template processing
  private readonly MacroProcessorContext _processorContext = new ();

  #endregion

  #region Public Methods

  /// <summary>
  ///   Processes the template for a given model and type info, yielding generated types (converters and main struct).
  /// </summary>
  /// <param name="model">The generator model describing the primitive type and its converters.</param>
  /// <param name="typeInfo">Supported type information for macros and includes.</param>
  /// <returns>An IEnumerable of generated types (converters and main struct).</returns>
  public IEnumerable<GeneratedType> ProcessTemplate(
    GeneratorModel model,
    SupportedTypeInfo typeInfo )
  {
    // Create a descriptor for the template, which manages macros and template keys
    var descriptor = new TemplateDescriptor( model, typeInfo );

    // Add macros from the descriptor to the macro processor context
    descriptor.AddMacrosToContext( _processorContext );

    // Generate and yield all enabled converters for the model
    foreach( var generatedType in GenerateConverters() )
    {
      yield return generatedType;
    }

    // Generate the main typed primitive struct source text, including requested converters
    var sourceText = GenerateSourceText(
      descriptor.TemplateKey,
      includes =>
      {
        // Add includes for each enabled converter type
        includes.AddIncludes( model.TypeConverter.IsEnabled, TypedPrimitiveConverter.TypeConverter, typeInfo );
        includes.AddIncludes( model.SystemTextJsonConverter.IsEnabled, TypedPrimitiveConverter.SystemTextJson, typeInfo );
        includes.AddIncludes( model.NewtonsoftJsonConverter.IsEnabled, TypedPrimitiveConverter.NewtonsoftJson, typeInfo );
      },
      descriptor.MainTemplateName
    );

    // Yield the main struct as a generated type
    yield return new GeneratedType( $"{model.Namespace}.{model.TypeName}", sourceText );

    yield break;

    // Generate source text for each enabled converter
    IEnumerable<GeneratedType> GenerateConverters()
    {
      foreach( var converter in model.GetEnabledConverters() )
      {
        sourceText = GenerateSourceText( converter.Name );
        yield return new GeneratedType( model, converter, sourceText );
      }
    }

    SourceText GenerateSourceText(
      string templateKey,
      Action<IncludesCollection>? processIncludes = null,
      string? templateResourceId = null )
    {
      // Retrieve or compile the template, adding includes if needed
      var template = _templateCache.GetOrAdd(
        templateKey,
        key =>
        {
          templateResourceId ??= key;
          var templateText = descriptor.LoadTemplate( templateResourceId );
          IncludesCollection? includes = null;

          if( processIncludes != null )
          {
            includes = new IncludesCollection();
            processIncludes( includes );
          }

          // Compile the template with the macro context and includes
          return TemplateCompiler.Compile( _processorContext, templateText, includes );
        }
      );

      // Process macros in the compiled template and return as SourceText
      var processedTemplate = ProcessMacros( template );
      return SourceText.From( processedTemplate, Encoding.UTF8 );
    }
  }

  #endregion

  #region Implementation

  private static string ProcessMacros(
    Template template )
  {
    // Use a pooled StringBuilder for efficiency
    var sb = StringBuilderPool.Default.Get();

    try
    {
      MacroProcessor.ProcessMacros( template, sb );
      return sb.ToString();
    }
    finally
    {
      StringBuilderPool.Default.Return( sb );
    }
  }

  #endregion
}
