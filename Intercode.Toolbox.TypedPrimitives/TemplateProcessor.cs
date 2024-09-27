// Module Name: TemplateProcessor.cs
// Author:      Eduardo Velasquez
// Copyright (c) 2024, Intercode Consulting, Inc.

namespace Intercode.Toolbox.TypedPrimitives;

using System.Collections.Concurrent;
using Intercode.Toolbox.TypedPrimitives.TemplateEngine;

internal class TemplateProcessor
{
  #region Constants

  private const string MAIN_TEMPLATE_NAME = "PrimitiveType";
  private const string TYPE_CONVERTER_TEMPLATE_NAME = "TypeConverter";
  private const string SYSTEM_TEXT_JSON_CONVERTER_TEMPLATE_NAME = "SystemTextJsonConverter";
  private const string NEWTONSOFT_JSON_CONVERTER_TEMPLATE_NAME = "NewtonsoftJsonConverter";
  private const string EF_CORE_VALUE_CONVERTER_TEMPLATE_NAME = "EFCoreValueConverter";

  private const string TYPE_CONVERTER_ATTRIBUTE_TEMPLATE =
    $"[global::System.ComponentModel.TypeConverter( typeof( ${Macros.FullName}$TypeConverter ) )]";

  private const string SYSTEM_TEXT_JSON_CONVERTER_ATTRIBUTE_TEMPLATE =
    $"[global::System.Text.Json.Serialization.JsonConverter( typeof( ${Macros.FullName}$SystemTextJsonConverter ) )]";

  private const string NEWTONSOFT_JSON_CONVERTER_ATTRIBUTE_TEMPLATE =
    $"[global::Newtonsoft.Json.JsonConverter( typeof( ${Macros.FullName}$NewtonsoftJsonConverter ) )]";

  #endregion

  #region Fields

  private readonly ConcurrentDictionary<string, CompiledTemplate> s_templateCache = new ();

  #endregion

  #region Public Methods

  public IEnumerable<(string TypeName, string Content)> ProcessTemplate(
    GeneratorModel model )
  {
    using var context = new TemplateContext( model );

    // Set the macros needed for the primitive type
    var builder = new MacroProcessorBuilder();
    var typeInfo = context.TypeInfo;

    // Set the common macros for all templates
    builder.AddMacro( Macros.TypeKeyword, typeInfo.Keyword );
    builder.AddMacro( Macros.Namespace, context.Model.Namespace );
    builder.AddMacro( Macros.Name, context.Model.Name );
    builder.AddMacro( Macros.FullName, $"{context.Model.Namespace}.{context.Model.Name}" );

    // If the template uses a non-default StringComparison, set the StringComparison macro
    if( context.Model.StringComparison is not null )
    {
      builder.AddMacro( Macros.StringComparison, context.Model.StringComparison! );
    }

    // If the template uses a System.Text.Json converter, set the JSON macros
    if( context.Model.HasConverter( TypedPrimitiveConverter.SystemTextJson ) )
    {
      builder.AddMacro( Macros.JsonTokenType, typeInfo.JsonTokenType );
      builder.AddMacro( Macros.JsonReader, typeInfo.JsonReader );
      builder.AddMacro( Macros.JsonWriter, typeInfo.JsonWriter );
    }

    if( context.Model.HasConverter( TypedPrimitiveConverter.NewtonsoftJson ) )
    {
      builder.AddMacro( Macros.NewtonsoftJsonTokenType, typeInfo.NewtonsoftJsonTokenType );
    }

    var macroProcessor = builder.Build();

    // Create the EFCore ValueConverter if requested
    if( context.Model.HasConverter( TypedPrimitiveConverter.EfCoreValueConverter ) )
    {
      var content = GenerateContent( EF_CORE_VALUE_CONVERTER_TEMPLATE_NAME );
      yield return ( $"{model.Namespace}.{model.Name}ValueConverter", content );
    }

    // Create the TypeConverter if requested
    if( context.Model.HasConverter( TypedPrimitiveConverter.TypeConverter ) )
    {
      var content = GenerateContent( TYPE_CONVERTER_TEMPLATE_NAME );
      yield return ( $"{model.Namespace}.{model.Name}TypeConverter", content );
    }

    // Create the System.Text.Json converter if requested
    if( context.Model.HasConverter( TypedPrimitiveConverter.SystemTextJson ) )
    {
      var content = GenerateContent( SYSTEM_TEXT_JSON_CONVERTER_TEMPLATE_NAME );
      yield return ( $"{model.Namespace}.{model.Name}SystemTextJsonConverter", content );
    }

    // Create the Newtonsoft.Json converter if requested
    if( context.Model.HasConverter( TypedPrimitiveConverter.NewtonsoftJson ) )
    {
      var content = GenerateContent( NEWTONSOFT_JSON_CONVERTER_TEMPLATE_NAME );
      yield return ( $"{model.Namespace}.{model.Name}NewtonsoftJsonConverter", content );
    }

    // See if we have already composed and compiled this template
    var compiledTemplate = s_templateCache.GetOrAdd(
      context.TemplateKey,
      _ =>
      {
        // Nope! Compose the template and cache it
        var mainTemplate = context.LoadTemplate( MAIN_TEMPLATE_NAME );

        var attributeBlock = GenerateAttributeBlock( context );
        return ComposeTemplate( context, mainTemplate, attributeBlock );
      }
    );

    // Apply the macro values to the context's content
    var processed = macroProcessor.ProcessMacros( compiledTemplate );
    yield return ( $"{model.Namespace}.{model.Name}", processed );

    yield break;

    string GenerateContent(
      string templateKey )
    {
      var compiled = s_templateCache.GetOrAdd(
        templateKey,
        key =>
        {
          var template = context.LoadTemplate( key, true );
          return new TemplateCompiler().Compile( template );
        }
      );

      var content = macroProcessor.ProcessMacros( compiled );
      return content;
    }
  }

  #endregion

  #region Implementation

  private static string GenerateAttributeBlock(
    TemplateContext context )
  {
    var builder = context.ContentBuilder;
    builder.Clear();

    if( context.Model.HasConverter( TypedPrimitiveConverter.TypeConverter ) )
    {
      builder.AppendLine( TYPE_CONVERTER_ATTRIBUTE_TEMPLATE );
    }

    if( context.Model.HasConverter( TypedPrimitiveConverter.SystemTextJson ) )
    {
      builder.AppendLine( SYSTEM_TEXT_JSON_CONVERTER_ATTRIBUTE_TEMPLATE );
    }

    if( context.Model.HasConverter( TypedPrimitiveConverter.NewtonsoftJson ) )
    {
      builder.AppendLine( NEWTONSOFT_JSON_CONVERTER_ATTRIBUTE_TEMPLATE );
    }

    return builder.ToString();
  }

  private static CompiledTemplate ComposeTemplate(
    TemplateContext context,
    string mainTemplate,
    string attributeBlock )
  {
    // Compose the template and preprocess to evaluate the initial macros
    var contentBuilder = context.ContentBuilder;
    contentBuilder.EnsureCapacity( mainTemplate.Length + attributeBlock.Length );
    contentBuilder.Clear();
    contentBuilder.Append( mainTemplate );

    var compiler = new TemplateCompiler();
    var compiledTemplate = compiler.Compile( contentBuilder.ToString() );

    var macroProcessor = CreateMacroProcessor();
    var composedTemplate = macroProcessor.ProcessMacros( compiledTemplate );

    // Compile and return the composed template
    return compiler.Compile( composedTemplate );

    MacroProcessor CreateMacroProcessor()
    {
      var builder = new MacroProcessorBuilder();
      builder.AddMacro( Macros.Attributes, attributeBlock );

      var processor = builder.Build();
      return processor;
    }
  }

  #endregion
}
