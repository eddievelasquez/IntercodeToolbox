// Module Name: TemplateContext.cs
// Author:      Eduardo Velasquez
// Copyright (c) 2024, Intercode Consulting, Inc.

namespace Intercode.Toolbox.TypedPrimitives;

internal class TemplateContext
{
  #region Constants

  private const string MAIN_TEMPLATE_NAME = "PrimitiveType";
  private const string COMMON_DIRECTORY = "Common";

  #endregion

  #region Fields

  private readonly string _resourceDirectory;

  #endregion

  #region Constructors

  public TemplateContext(
    GeneratorModel model )
  {
    Model = model;
    TypeInfo = TypeManager.GetSupportedTypeInfo( model.PrimitiveType );

    // Use the common main template if no specialization exists
    var useCommonTemplates = !EmbeddedResourceManager.DoesTemplateExist(
      Model.PrimitiveType.FullName!,
      MAIN_TEMPLATE_NAME
    );

    if( useCommonTemplates )
    {
      _resourceDirectory = COMMON_DIRECTORY;
      TemplateKey = Model.Converters.ToString();
    }
    else
    {
      _resourceDirectory = model.PrimitiveType.FullName!;
      TemplateKey = $"{Model.PrimitiveType.FullName}_{Model.Converters}";
    }
  }

  #endregion

  #region Properties

  public string TemplateKey { get; }
  public GeneratorModel Model { get; }
  public SupportedTypeInfo TypeInfo { get; }

  #endregion

  #region Public Methods

  public string LoadTemplate(
    string templateName,
    bool useCommonOverride = false )
  {
    var resourceDir = useCommonOverride ? COMMON_DIRECTORY : _resourceDirectory;
    var template = EmbeddedResourceManager.LoadTemplate( resourceDir, templateName );
    return template;
  }

  #endregion
}
