// Module Name: TemplateDescriptor.cs
// Author:      Eduardo Velasquez
// Copyright (c) 2024, Intercode Consulting, Inc.

namespace Intercode.Toolbox.TypedPrimitives;

using System.Collections.Concurrent;
using Intercode.Toolbox.TemplateEngine;

internal class TemplateDescriptor
{
  #region Constants

  private static readonly ConcurrentDictionary<string, string> s_templatePathCache = new ();

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

    var converterFlags = ( byte ) Model.Converters;

    TemplateKey = useCommonTemplates
      ? $"{MainTemplateName}.{converterFlags:X2}"
      : $"{MainTemplateName}.{Model.PrimitiveTypeName}.{converterFlags:X2}";
  }

  #endregion

  #region Properties

  public string TemplateKey { get; }
  public string MainTemplateName { get; }
  public GeneratorModel Model { get; }
  public SupportedTypeInfo TypeInfo { get; }

  #endregion

  #region Public Methods

  public string GetTemplateResourcePath(
    ConverterModel converter )
  {
    var key = $"{converter.Name}.{Model.PrimitiveTypeName}";

    return s_templatePathCache.GetOrAdd(
      key,
      _ =>
      {
        // Use the common main template if no specialization exists
        var usesCommonTemplate = !EmbeddedResourceManager.DoesTemplateExist(
          Model.PrimitiveTypeName,
          converter.Name
        );

        var directory = usesCommonTemplate ? COMMON_DIRECTORY : Model.PrimitiveTypeName;
        var resourcePath = EmbeddedResourceManager.GetTemplateResourcePath( directory, converter.Name );
        return resourcePath;
      }
    );
  }

  public string LoadTemplateByPath(
    string resourcePath )
  {
    return EmbeddedResourceManager.LoadTemplate( resourcePath );
  }

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

  public MacroValues CreateMacroValues(
    MacroTable macroTable )
  {
    var macroValues = macroTable.CreateValues();

    // Set the common macros for all templates
    macroValues.SetValue( MacroNames.PrimitiveName, TypeInfo.Keyword )
               .SetValue( MacroNames.TypedPrimitiveNamespace, Model.Namespace )
               .SetValue( MacroNames.TypedPrimitiveName, Model.TypeName )
               .SetValue( MacroNames.TypedPrimitiveQualifiedName, $"{Model.Namespace}.{Model.TypeName}" )
               .SetValue( MacroNames.StringComparison, Model.StringComparison )
               .AddConverterMacros( Model.TypeConverter, TypeInfo )
               .AddConverterMacros( Model.SystemTextJsonConverter, TypeInfo )
               .AddConverterMacros( Model.NewtonsoftJsonConverter, TypeInfo );

    return macroValues;
  }

  #endregion
}
