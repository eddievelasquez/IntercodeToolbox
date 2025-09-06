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

    var template = LoadAndCompileTemplate(
      descriptor.TemplateKey,
      descriptor,
      includes =>
      {
        // Add includes for each enabled converter type
        includes.AddIncludes( model.TypeConverter.IsEnabled, TypedPrimitiveConverter.TypeConverter, typeInfo );
        includes.AddIncludes( model.SystemTextJsonConverter.IsEnabled, TypedPrimitiveConverter.SystemTextJson, typeInfo );
        includes.AddIncludes( model.NewtonsoftJsonConverter.IsEnabled, TypedPrimitiveConverter.NewtonsoftJson, typeInfo );
      },
      descriptor.MainTemplateName
    );

    // Add macros from the descriptor to the macro processor context
    descriptor.AddMacrosToContext( template.Context );

    // Yield the main struct as a generated type
    yield return new GeneratedType( model, GenerateSourceText( template ) );

    // Generate enabled converters for the model
    foreach( var generatedType in GenerateEnabledConverters( descriptor, template.Context ) )
    {
      yield return generatedType;
    }
  }

  #endregion

  #region Implementation

  private IEnumerable<GeneratedType> GenerateEnabledConverters(
    TemplateDescriptor descriptor,
    MacroProcessorContext processorContext )
  {
    // Generate source text for each enabled converter
    foreach( var converter in descriptor.Model.GetEnabledConverters() )
    {
      var template = LoadAndCompileTemplate( converter.Name, descriptor, processorContext: processorContext );
      yield return new GeneratedType( descriptor.Model, converter, GenerateSourceText( template ) );
    }
  }

  private Template LoadAndCompileTemplate(
    string templateKey,
    TemplateDescriptor descriptor,
    Action<IncludesCollection>? processIncludes = null,
    string? templateResourceId = null,
    MacroProcessorContext? processorContext = null )
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
        processorContext ??= new MacroProcessorContext();
        return TemplateCompiler.Compile( processorContext, templateText, includes );
      }
    );

    return template;
  }

  private static SourceText GenerateSourceText(
    Template template )
  {
    // Process macros in the compiled template and return as SourceText
    string? text;

    // Use a pooled StringBuilder for efficiency
    var sb = StringBuilderPool.Default.Get();

    try
    {
      MacroProcessor.ProcessMacros( template, sb );
      text = sb.ToString();
    }
    finally
    {
      StringBuilderPool.Default.Return( sb );
    }

    return SourceText.From( text, Encoding.UTF8 );
  }

  #endregion
}
