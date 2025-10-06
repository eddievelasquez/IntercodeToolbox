// Module Name: GeneratedTypesDescriptor.cs
// Author:      Eduardo Velasquez
// Copyright (c) 2024, Intercode Consulting, Inc.

namespace Intercode.Toolbox.TypedPrimitives;

using Intercode.Toolbox.TemplateEngine;

internal class GeneratedTypesDescriptor
{
  #region Constructors

  public GeneratedTypesDescriptor(
    GeneratorModel model )
  {
    Model = model;

    if( !TypeManager.TryGetSupportedTypeInfo( Model.PrimitiveType, out var typeInfo ) )
    {
      throw new InvalidOperationException( $"{Model.PrimitiveType} is not a supported type" );
    }

    TypeInfo = typeInfo;
  }

  #endregion

  #region Properties

  public GeneratorModel Model { get; }
  private SupportedTypeInfo TypeInfo { get; }

  #endregion

  #region Public Methods

  public TemplateDescriptor GetTemplateTypeDescriptor(
    TemplateType templateType )
  {
    return TemplateDescriptor.Create(
      templateType,
      Model.PrimitiveType,
      templateType == TemplateType.Main ? Model.Converters : null
    );
  }

  public void AddIncludes(
    IncludesCollection includes )
  {
    includes.AddIncludes( Model.TypeConverter, TypeInfo )
            .AddIncludes( Model.SystemTextJsonConverter, TypeInfo )
            .AddIncludes( Model.NewtonsoftJsonConverter, TypeInfo );
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
