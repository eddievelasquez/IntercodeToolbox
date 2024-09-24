// Module Name: TemplateProcessor.cs
// Author:      Eduardo Velasquez
// Copyright (c) 2024, Intercode Consulting, Inc.

namespace Intercode.Toolbox.TypedPrimitives;

internal class TemplateProcessor
{
  #region Constants

  private const string MAIN_TEMPLATE_NAME = "PrimitiveType";
  private const string TYPE_CONVERTER_TEMPLATE_NAME = "TypeConverter";
  private const string SYSTEM_TEXT_JSON_CONVERTER_UNVALIDATED_TEMPLATE_NAME = "SystemTextJsonConverter_Unvalidated";
  private const string SYSTEM_TEXT_JSON_CONVERTER_VALIDATED_TEMPLATE_NAME = "SystemTextJsonConverter_Validated";
  private const string EF_CORE_VALUE_CONVERTER_TEMPLATE_NAME = "EFCoreValueConverter";
  private const string FACTORY_UNVALIDATED_TEMPLATE_NAME = "Factory_Unvalidated";
  private const string OPERATORS_UNVALIDATED_TEMPLATE_NAME = "Operators_Unvalidated";
  private const string FACTORY_VALIDATED_TEMPLATE_NAME = "Factory_Validated";
  private const string FACTORY_VALIDATED_WITH_FLAGS_TEMPLATE_NAME = "Factory_ValidatedWithFlags";
  private const string OPERATORS_VALIDATED_TEMPLATE_NAME = "Operators_Validated";

  private const string TYPE_CONVERTER_ATTRIBUTE_TEMPLATE =
    $"[global::System.ComponentModel.TypeConverter( typeof( ${Macros.FullName}$.TypeConverter ) )]";

  private const string SYSTEM_TEXT_JSON_CONVERTER_ATTRIBUTE_TEMPLATE =
    $"[global::System.Text.Json.Serialization.JsonConverter( typeof( ${Macros.FullName}$.SystemTextJsonConverter ) )]";

  private static readonly TemplateCache s_templateCache = new ();

  #endregion

  #region Public Methods

  public string ProcessTemplate(
    GeneratorModel model )
  {
    using var context = new TemplateContext( model );

    // See if we have already composed and compiled this template
    var compiledTemplate = s_templateCache.GetOrAddTemplate(
      context,
      ctx =>
      {
        // Nope! Compose the template and cache it
        var mainTemplate = ctx.LoadTemplate( MAIN_TEMPLATE_NAME );

        var attributeBlock = GenerateAttributeBlock( ctx );
        var converterBlock = GenerateConverterBlock( ctx );
        return ComposeTemplate( ctx, mainTemplate, attributeBlock, converterBlock );
      }
    );

    // Set the macros needed for the primitive type
    var builder = new MacroProcessorBuilder();
    var typeInfo = context.TypeInfo;

    // If the template uses the System.Text.Json converter, set the JSON macros
    if( context.Model.HasConverter( TypedPrimitiveConverter.SystemTextJson ) )
    {
      builder.AddMacro( Macros.JsonTokenType, typeInfo.JsonTokenType );
      builder.AddMacro( Macros.JsonReader, typeInfo.JsonReader );
      builder.AddMacro( Macros.JsonWriter, typeInfo.JsonWriter );
    }

    // If the template uses a validator, set the validator macros
    if( context.Model.ValidatorTypeName is not null )
    {
      builder.AddMacro( Macros.ValidatorType, context.Model.ValidatorTypeName );
      if( context.Model.ValidatorFlagsTypeName is not null )
      {
        builder.AddMacro( Macros.ValidationFlagType, context.Model.ValidatorFlagsTypeName );
        builder.AddMacro( Macros.ValidationFlagDefaultValue, context.Model.ValidatorFlagsDefaultValue! );
      }
    }

    // If the template uses a non-default StringComparison, set the StringComparison macro
    if( context.Model.StringComparison is not null )
    {
      builder.AddMacro( Macros.StringComparison, context.Model.StringComparison! );
    }

    // Set the common macros for all templates
    builder.AddMacro( Macros.TypeKeyword, typeInfo.Keyword );
    builder.AddMacro( Macros.TypeName, typeInfo.Name );
    builder.AddMacro( Macros.Namespace, context.Model.Namespace );
    builder.AddMacro( Macros.Name, context.Model.Name );
    builder.AddMacro( Macros.FullName, $"{context.Model.Namespace}.{context.Model.Name}" );

    // Apply the macro values to the context's content
    var macroProcessor = builder.Build();
    var processed = macroProcessor.ProcessMacros( compiledTemplate );
    return processed;
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

    return builder.ToString();
  }

  private static string GenerateConverterBlock(
    TemplateContext context )
  {
    var builder = context.ContentBuilder;
    builder.Clear();

    if( context.Model.HasConverter( TypedPrimitiveConverter.TypeConverter ) )
    {
      ContentLoadAndAppendTemplate( TYPE_CONVERTER_TEMPLATE_NAME );
    }

    if( context.Model.HasConverter( TypedPrimitiveConverter.SystemTextJson ) )
    {
      ContentLoadAndAppendTemplate(
        context.ValidationType == ValidationType.Unvalidated
          ? SYSTEM_TEXT_JSON_CONVERTER_UNVALIDATED_TEMPLATE_NAME
          : SYSTEM_TEXT_JSON_CONVERTER_VALIDATED_TEMPLATE_NAME
      );
    }

    if( context.Model.HasConverter( TypedPrimitiveConverter.EfCoreValueConverter ) )
    {
      ContentLoadAndAppendTemplate( EF_CORE_VALUE_CONVERTER_TEMPLATE_NAME );
    }

    return builder.ToString();

    void ContentLoadAndAppendTemplate(
      string templateName )
    {
      var template = context.LoadTemplate( templateName, true );
      builder.AppendLine( template );
    }
  }

  private static CompiledTemplate ComposeTemplate(
    TemplateContext context,
    string mainTemplate,
    string attributeBlock,
    string converterBlock )
  {
    // Compose the template and preprocess to evaluate the initial macros
    var contentBuilder = context.ContentBuilder;
    contentBuilder.EnsureCapacity( mainTemplate.Length + attributeBlock.Length + converterBlock.Length );
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
      builder.AddMacro( Macros.Converters, converterBlock );

      if( context.Model.ValidatorTypeName is null )
      {
        var factoryTemplate = context.LoadTemplate( FACTORY_UNVALIDATED_TEMPLATE_NAME );
        builder.AddMacro( Macros.Factory, factoryTemplate );

        var operatorTemplate = context.LoadTemplate( OPERATORS_UNVALIDATED_TEMPLATE_NAME );
        builder.AddMacro( Macros.Operators, operatorTemplate );
      }
      else
      {
        var factoryTemplate = context.LoadTemplate(
          context.Model.ValidatorFlagsTypeName is null
            ? FACTORY_VALIDATED_TEMPLATE_NAME
            : FACTORY_VALIDATED_WITH_FLAGS_TEMPLATE_NAME
        );

        var operatorTemplate = context.LoadTemplate( OPERATORS_VALIDATED_TEMPLATE_NAME );
        builder.AddMacro( Macros.Factory, factoryTemplate );
        builder.AddMacro( Macros.Operators, operatorTemplate );
      }

      var processor = builder.Build();
      return processor;
    }
  }

  #endregion
}
