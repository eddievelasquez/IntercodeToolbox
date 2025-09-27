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
  /// <returns>An IEnumerable of generated types (converters and main struct).</returns>
  public IEnumerable<GeneratedType> GenerateTypes(
    GeneratorModel model )
  {
    var descriptor = new GeneratedTypesDescriptor( model );
    var macroValues = descriptor.CreateMacroValues( s_macroTable );

    // Generate the main struct
    yield return GenerateMainTemplate( descriptor, macroValues );

    // Generate enabled converters
    foreach( var generatedType in GenerateEnabledConverters( descriptor, macroValues ) )
    {
      yield return generatedType;
    }
  }

  #endregion

  #region Implementation

  private static GeneratedType GenerateMainTemplate(
    GeneratedTypesDescriptor descriptor,
    MacroValues macroValues )
  {
    // Load main template
    var template = LoadAndCompileTemplate(
      descriptor.GetTemplateTypeDescriptor( TemplateType.Main ),
      descriptor.AddIncludes
    );

    return new GeneratedType( descriptor.Model, GenerateSourceText( template, macroValues ) );
  }

  private static IEnumerable<GeneratedType> GenerateEnabledConverters(
    GeneratedTypesDescriptor descriptor,
    MacroValues macroValues )
  {
    // Generate source text for each enabled converter
    foreach( var converter in descriptor.Model.GetEnabledConverters() )
    {
      var template = LoadAndCompileTemplate( descriptor.GetTemplateTypeDescriptor( converter.TemplateType ) );
      yield return new GeneratedType( descriptor.Model, converter, GenerateSourceText( template, macroValues ) );
    }
  }

  private static Template LoadAndCompileTemplate(
    TemplateDescriptor templateDescriptor,
    Action<IncludesCollection>? processIncludes = null )
  {
    // Retrieve or compile the template, adding includes if needed
    var template = s_templateCache.GetOrAdd(
      templateDescriptor.TemplateKey,
      _ =>
      {
        var templateText = templateDescriptor.LoadTemplateText();
        IncludesCollection? includes = null;

        if( processIncludes != null )
        {
          includes = new IncludesCollection();
          processIncludes( includes );
        }

        return TemplateCompiler.Compile( s_macroTable, templateText, includes );
      }
    );

    return template;
  }

  private static SourceText GenerateSourceText(
    Template template,
    MacroValues macroValues )
  {
    var text = template.ProcessMacros( macroValues );
    return SourceText.From( text, Encoding.UTF8 );
  }

  #endregion
}
