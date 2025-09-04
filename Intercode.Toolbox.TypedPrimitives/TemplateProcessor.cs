// Module Name: TemplateProcessor.cs
// Author:      Eduardo Velasquez
// Copyright (c) 2025, Intercode Consulting, Inc.

namespace Intercode.Toolbox.TypedPrimitives;

using System.Collections.Concurrent;
using System.Text;
using Intercode.Toolbox.TemplateEngine;
using Microsoft.CodeAnalysis.Text;

internal class TemplateProcessor
{
  #region Constants

  private const string TYPE_CONVERTER_TEMPLATE_NAME = "TypeConverter";
  private const string SYSTEM_TEXT_JSON_CONVERTER_TEMPLATE_NAME = "SystemTextJsonConverter";
  private const string NEWTONSOFT_JSON_CONVERTER_TEMPLATE_NAME = "NewtonsoftJsonConverter";
  private const string EF_CORE_VALUE_CONVERTER_TEMPLATE_NAME = "EFCoreValueConverter";

  #endregion

  #region Fields

  private readonly ConcurrentDictionary<string, Template> _templateCache = new ();
  private readonly MacroProcessorContext _processorContext = new ();

  #endregion

  #region Public Methods

  public IEnumerable<GeneratedType> ProcessTemplate(
    GeneratorModel model,
    SupportedTypeInfo typeInfo )
  {
    var context = new TemplateContext( model, typeInfo );

    // Set the common macros for all templates
    _processorContext.AddMacro( MacroNames.PrimitiveName, typeInfo.Keyword );
    _processorContext.AddMacro( MacroNames.TypedPrimitiveNamespace, context.Model.Namespace );
    _processorContext.AddMacro( MacroNames.TypedPrimitiveName, context.Model.TypeName );

    _processorContext.AddMacro(
      MacroNames.TypedPrimitiveQualifiedName,
      $"{context.Model.Namespace}.{context.Model.TypeName}"
    );
    _processorContext.AddMacro( MacroNames.StringComparison, context.Model.StringComparison );

    // Add/update conditional macros for the requested converters
    _processorContext.AddConverterMacros(
      context.Model.HasTypeConverter,
      TypedPrimitiveConverter.TypeConverter,
      typeInfo
    );

    _processorContext.AddConverterMacros(
      context.Model.HasSystemTextJsonConverter,
      TypedPrimitiveConverter.SystemTextJson,
      typeInfo
    );

    _processorContext.AddConverterMacros(
      context.Model.HasNewtonsoftJsonConverter,
      TypedPrimitiveConverter.NewtonsoftJson,
      typeInfo
    );

    SourceText? sourceText;

    // Create the EFCore ValueConverter if requested
    if( context.Model.HasEfCoreConverter )
    {
      sourceText = GenerateSourceText( EF_CORE_VALUE_CONVERTER_TEMPLATE_NAME );
      yield return new GeneratedType( $"{model.Namespace}.{model.TypeName}ValueConverter", sourceText );
    }

    // Create the TypeConverter if requested
    if( context.Model.HasTypeConverter )
    {
      sourceText = GenerateSourceText( TYPE_CONVERTER_TEMPLATE_NAME );
      yield return new GeneratedType( $"{model.Namespace}.{model.TypeName}TypeConverter", sourceText );
    }

    // Create the System.Text.Json converter if requested
    if( context.Model.HasSystemTextJsonConverter )
    {
      sourceText = GenerateSourceText( SYSTEM_TEXT_JSON_CONVERTER_TEMPLATE_NAME );
      yield return new GeneratedType( $"{model.Namespace}.{model.TypeName}SystemTextJsonConverter", sourceText );
    }

    // Create the Newtonsoft.Json converter if requested
    if( context.Model.HasNewtonsoftJsonConverter )
    {
      sourceText = GenerateSourceText( NEWTONSOFT_JSON_CONVERTER_TEMPLATE_NAME );
      yield return new GeneratedType( $"{model.Namespace}.{model.TypeName}NewtonsoftJsonConverter", sourceText );
    }

    // Create the main typed primitive struct
    sourceText = GenerateSourceText(
      context.TemplateKey,
      includes =>
      {
        // Add includes for the requested converters
        includes.AddIncludes( model.HasTypeConverter, TypedPrimitiveConverter.TypeConverter, typeInfo );
        includes.AddIncludes( model.HasSystemTextJsonConverter, TypedPrimitiveConverter.SystemTextJson, typeInfo );
        includes.AddIncludes( model.HasNewtonsoftJsonConverter, TypedPrimitiveConverter.NewtonsoftJson, typeInfo );
      },
      context.MainTemplateName
    );

    yield return new GeneratedType( $"{model.Namespace}.{model.TypeName}", sourceText );

    yield break;

    SourceText GenerateSourceText(
      string templateKey,
      Action<IncludesCollection>? processIncludes = null,
      string? templateResourceId = null )
    {
      var compiled = _templateCache.GetOrAdd(
        templateKey,
        key =>
        {
          templateResourceId ??= key;
          var template = context.LoadTemplate( templateResourceId );
          IncludesCollection? includes = null;

          if( processIncludes != null )
          {
            includes = new IncludesCollection();
            processIncludes( includes );
          }

          return TemplateCompiler.Compile( _processorContext, template, includes );
        }
      );

      var processedTemplate = ProcessMacros( compiled );
      return SourceText.From( processedTemplate, Encoding.UTF8 );
    }
  }

  #endregion

  #region Implementation

  // NOTE: The main template is special and needs to be composed because it will
  // contain macros (e.g. attributes) that in turn, contain other macros that need
  // to be evaluated after the template is composed.

  private static string ProcessMacros(
    Template template )
  {
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
