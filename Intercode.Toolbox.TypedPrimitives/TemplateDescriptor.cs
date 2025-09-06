// Module Name: TemplateDescriptor.cs
// Author:      Eduardo Velasquez
// Copyright (c) 2024, Intercode Consulting, Inc.

namespace Intercode.Toolbox.TypedPrimitives;

using Intercode.Toolbox.TemplateEngine;

internal class TemplateDescriptor
{
  #region Constants

  private const string MAIN_TEMPLATE_NAME_VALUE_TYPE = "PrimitiveType.ValueType";
  private const string MAIN_TEMPLATE_NAME_REFERENCE_TYPE = "PrimitiveType.ReferenceType";
  private const string COMMON_DIRECTORY = "Common";

  #endregion

  #region Constructors

  public TemplateDescriptor(
    GeneratorModel model,
    SupportedTypeInfo typeInfo )
  {
    Model = model;
    TypeInfo = typeInfo;
    MainTemplateName = model.IsValueType ? MAIN_TEMPLATE_NAME_VALUE_TYPE : MAIN_TEMPLATE_NAME_REFERENCE_TYPE;

    // Use the common main template if no specialization exists
    var useCommonTemplates = !EmbeddedResourceManager.DoesTemplateExist(
      Model.PrimitiveTypeName,
      MainTemplateName
    );

    TemplateKey = useCommonTemplates
      ? $"{MainTemplateName}.{Model.Converters}"
      : $"{MainTemplateName}.{Model.PrimitiveTypeName}.{Model.Converters}";
  }

  #endregion

  #region Properties

  public string TemplateKey { get; }
  public string MainTemplateName { get; }
  public GeneratorModel Model { get; }
  public SupportedTypeInfo TypeInfo { get; }

  #endregion

  #region Public Methods

  public string LoadTemplate(
    string templateName )
  {
    // Try to load specialized type template
    if( EmbeddedResourceManager.TryLoadTemplate( Model.PrimitiveTypeName, templateName, out var template ) )
    {
      return template;
    }

    // Load common template
    template = EmbeddedResourceManager.LoadTemplate( COMMON_DIRECTORY, templateName );
    return template;
  }

  public void AddMacrosToContext(
    MacroProcessorContext context )
  {
    // Set the common macros for all templates
    context.AddMacro( MacroNames.PrimitiveName, TypeInfo.Keyword );
    context.AddMacro( MacroNames.TypedPrimitiveNamespace, Model.Namespace );
    context.AddMacro( MacroNames.TypedPrimitiveName, Model.TypeName );
    context.AddMacro( MacroNames.TypedPrimitiveQualifiedName, $"{Model.Namespace}.{Model.TypeName}" );
    context.AddMacro( MacroNames.StringComparison, Model.StringComparison );

    // Add/update conditional macros for the requested converters
    context.AddConverterMacros( Model.TypeConverter.IsEnabled, TypedPrimitiveConverter.TypeConverter, TypeInfo );
    context.AddConverterMacros( Model.SystemTextJsonConverter.IsEnabled, TypedPrimitiveConverter.SystemTextJson, TypeInfo );
    context.AddConverterMacros( Model.NewtonsoftJsonConverter.IsEnabled, TypedPrimitiveConverter.NewtonsoftJson, TypeInfo );
  }

  #endregion
}
