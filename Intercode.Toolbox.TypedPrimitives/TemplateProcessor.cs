// Module Name: TemplateProcessor.cs
// Author:      Eduardo Velasquez
// Copyright (c) 2024, Intercode Consulting, Inc.

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

  #endregion

  #region Public Methods

  public IEnumerable<GeneratedType> ProcessTemplate(
    GeneratorModel model,
    SupportedTypeInfo typeInfo )
  {
    var context = new TemplateContext( model, typeInfo );

    // Set the macros needed for the primitive type
    var builder = new MacroProcessorBuilder();

    // Set the common macros for all templates
    builder.AddMacro( Macros.TypeKeyword, typeInfo.Keyword );
    builder.AddMacro( Macros.Namespace, context.Model.Namespace );
    builder.AddMacro( Macros.TypeName, context.Model.TypeName );
    builder.AddMacro( Macros.TypeQualifiedName, $"{context.Model.Namespace}.{context.Model.TypeName}" );

    // If the template uses a non-default StringComparison, set the StringComparison macro
    if( context.Model.StringComparison is not null )
    {
      builder.AddMacro( Macros.StringComparison, context.Model.StringComparison! );
    }

    if( context.Model.HasTypeConverter )
    {
      builder.AddConverterMacros( TypedPrimitiveConverter.TypeConverter, typeInfo );
    }

    if( context.Model.HasSystemTextJsonConverter )
    {
      builder.AddConverterMacros( TypedPrimitiveConverter.SystemTextJson, typeInfo );
    }

    if( context.Model.HasNewtonsoftJsonConverter )
    {
      builder.AddConverterMacros( TypedPrimitiveConverter.NewtonsoftJson, typeInfo );
    }

    var macroProcessor = builder.Build();

    // Create the EFCore ValueConverter if requested
    SourceText? sourceText;
    if( context.Model.HasEfCoreConverter )
    {
      sourceText = GenerateContent( EF_CORE_VALUE_CONVERTER_TEMPLATE_NAME );
      yield return new GeneratedType( $"{model.Namespace}.{model.TypeName}ValueConverter", sourceText );
    }

    // Create the TypeConverter if requested
    if( context.Model.HasTypeConverter )
    {
      sourceText = GenerateContent( TYPE_CONVERTER_TEMPLATE_NAME );
      yield return new GeneratedType( $"{model.Namespace}.{model.TypeName}TypeConverter", sourceText );
    }

    // Create the System.Text.Json converter if requested
    if( context.Model.HasSystemTextJsonConverter )
    {
      sourceText = GenerateContent( SYSTEM_TEXT_JSON_CONVERTER_TEMPLATE_NAME );
      yield return new GeneratedType( $"{model.Namespace}.{model.TypeName}SystemTextJsonConverter", sourceText );
    }

    // Create the Newtonsoft.Json converter if requested
    if( context.Model.HasNewtonsoftJsonConverter )
    {
      sourceText = GenerateContent( NEWTONSOFT_JSON_CONVERTER_TEMPLATE_NAME );
      yield return new GeneratedType( $"{model.Namespace}.{model.TypeName}NewtonsoftJsonConverter", sourceText );
    }

    // See if we have already composed the main template
    var compiledTemplate = _templateCache.GetOrAdd(
      context.TemplateKey,
      _ =>
      {
        // Nope! Compose the template and cache it
        var mainTemplate = context.LoadTemplate( context.MainTemplateName );
        return ComposeMainTemplate( mainTemplate, macroProcessor );
      }
    );

    // Apply the macro values to the context's content
    var processed = GenerateSource( macroProcessor, compiledTemplate );
    yield return new GeneratedType( $"{model.Namespace}.{model.TypeName}", SourceText.From( processed, Encoding.UTF8 ) );

    yield break;

    SourceText GenerateContent(
      string templateKey )
    {
      var compiled = _templateCache.GetOrAdd(
        templateKey,
        key =>
        {
          var template = context.LoadTemplate( key );
          return new TemplateCompiler().Compile( template );
        }
      );

      var processedTemplate = GenerateSource( macroProcessor, compiled );
      return SourceText.From( processedTemplate, Encoding.UTF8 );
    }
  }

  #endregion

  #region Implementation

  // NOTE: The main template is special and needs to be composed because it will
  // contain macros (e.g. attributes) that in turn, contain other macros that need
  // to be evaluated after the template is composed.
  private static Template ComposeMainTemplate(
    string mainTemplate,
    MacroProcessor macroProcessor )
  {
    var compiler = new TemplateCompiler();
    var compiledTemplate = compiler.Compile( mainTemplate );
    var composedTemplate = GenerateSource( macroProcessor, compiledTemplate );

    // Compile and return the composed template
    return compiler.Compile( composedTemplate );
  }

  private static string GenerateSource(
    MacroProcessor macroProcessor,
    Template template )
  {
    var sb = StringBuilderPool.Default.Get();

    try
    {
      using var writer = new StringWriter( sb );
      macroProcessor.ProcessMacros( template, writer );
      return writer.ToString();
    }
    finally
    {
      StringBuilderPool.Default.Return( sb );
    }
  }

  #endregion
}
