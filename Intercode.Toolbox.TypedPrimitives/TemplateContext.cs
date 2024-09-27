// Module Name: TemplateContext.cs
// Author:      Eduardo Velasquez
// Copyright (c) 2024, Intercode Consulting, Inc.

namespace Intercode.Toolbox.TypedPrimitives;

using System.Text;
using Intercode.Toolbox.TypedPrimitives.TemplateEngine;

internal class TemplateContext: IDisposable
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
    UseCommonTemplates = UsesCommonTemplates();
    TemplateKey = GetTemplateKey();

    _resourceDirectory = GetResourceDirectory( model.PrimitiveType );
    ContentBuilder = StringBuilderPool.Default.Get();
    TypeInfo = TypeManager.GetSupportedTypeInfo( model.PrimitiveType );
    return;

    string GetResourceDirectory(
      Type type )
    {
      return UseCommonTemplates ? COMMON_DIRECTORY : type.FullName!;
    }

    string GetTemplateKey()
    {
      if( UseCommonTemplates )
      {
        return $"{Model.Converters}";
      }

      return $"{Model.PrimitiveType.FullName}_{Model.Converters}";
    }

    bool UsesCommonTemplates()
    {
      return !EmbeddedResourceManager.DoesResourceExist(
        Model.PrimitiveType.FullName!,
        MAIN_TEMPLATE_NAME
      );
    }
  }

  #endregion

  #region Properties

  public string TemplateKey { get; }
  public GeneratorModel Model { get; }
  public StringBuilder ContentBuilder { get; }
  public bool UseCommonTemplates { get; }
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

  public void Dispose()
  {
    StringBuilderPool.Default.Return( ContentBuilder );
    GC.SuppressFinalize( this );
  }

  #endregion
}
