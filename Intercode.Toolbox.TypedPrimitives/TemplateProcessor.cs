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
  #region Constants

  // The macro table used for processing templates
  private static readonly MacroTable s_macroTable = new MacroTableBuilder().DeclareTemplateProcessorMacros().Build();

  // Caches compiled templates by key for reuse and performance
  private static readonly ConcurrentDictionary<string, Template> s_templateCache = new ();

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

    // Load main template
    var template = LoadAndCompileTemplate(
      descriptor.TemplateKey,
      descriptor,
      includes =>
      {
        // Add includes for each enabled converter type
        includes.AddIncludes( model.TypeConverter, typeInfo )
                .AddIncludes( model.SystemTextJsonConverter, typeInfo )
                .AddIncludes( model.NewtonsoftJsonConverter, typeInfo );
      },
      descriptor.MainTemplateName
    );

    // Add macro values
    var macroValues = descriptor.CreateMacroValues( s_macroTable );

    // Yield the main struct as a generated type
    yield return new GeneratedType( model, GenerateSourceText( template, macroValues ) );

    // Generate enabled converters for the model
    foreach( var generatedType in GenerateEnabledConverters( descriptor, macroValues ) )
    {
      yield return generatedType;
    }
  }

  #endregion

  #region Implementation

  private IEnumerable<GeneratedType> GenerateEnabledConverters(
    TemplateDescriptor descriptor,
    MacroValues macroValues )
  {
    // Generate source text for each enabled converter
    foreach( var converter in descriptor.Model.GetEnabledConverters() )
    {
      var resourcePath = descriptor.GetTemplateResourcePath( converter );
      var template = LoadAndCompileTemplate( resourcePath, descriptor, null );
      yield return new GeneratedType( descriptor.Model, converter, GenerateSourceText( template, macroValues ) );
    }
  }

  private Template LoadAndCompileTemplate(
    string resourcePath,
    TemplateDescriptor descriptor,
    Action<IncludesCollection>? processIncludes = null )
  {
    // Retrieve or compile the template, adding includes if needed
    var template = s_templateCache.GetOrAdd(
      resourcePath,
      key =>
      {
        var templateText = descriptor.LoadTemplateByPath( resourcePath );
        IncludesCollection? includes = null;

        if( processIncludes != null )
        {
          includes = new IncludesCollection();
          processIncludes( includes );
        }

        // Compile the template with the macro context and includes
        return TemplateCompiler.Compile( s_macroTable, templateText, includes );
      }
    );

    return template;
  }

  private Template LoadAndCompileTemplate(
    string templateKey,
    TemplateDescriptor descriptor,
    Action<IncludesCollection>? processIncludes,
    string? templateResourceId )
  {
    // Retrieve or compile the template, adding includes if needed
    var template = s_templateCache.GetOrAdd(
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
        return TemplateCompiler.Compile( s_macroTable, templateText, includes );
      }
    );

    return template;
  }

  private static SourceText GenerateSourceText(
    Template template,
    MacroValues macroValues )
  {
    // Process macros in the compiled template and return as SourceText
    string? text;

    // Use a pooled StringBuilder for efficiency
    var sb = StringBuilderPool.Default.Get();

    try
    {
      MacroProcessor.ProcessMacros( template, macroValues, sb );
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
