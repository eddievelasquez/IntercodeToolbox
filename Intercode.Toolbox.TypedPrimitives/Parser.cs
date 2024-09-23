// Module Name: Parser.cs
// Author:      Eduardo Velasquez
// Copyright (c) 2024, Intercode Consulting, Inc.

namespace Intercode.Toolbox.TypedPrimitives;

using System.Diagnostics;
using Intercode.Toolbox.TypedPrimitives.Diagnostics;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

internal static class Parser
{
  #region Constants

  public static readonly string MarkerAttributeName = nameof( TypedPrimitiveAttribute )!;
  public static readonly string MarkerAttributeFullName = typeof( TypedPrimitiveAttribute ).FullName!;
  public static readonly string ConvertersKey = nameof( TypedPrimitiveAttribute.Converters );
  public static readonly string ValidatorTypeKey = nameof( TypedPrimitiveAttribute.ValidatorType );
  public static readonly string ValidatorFlagsTypeKey = nameof( TypedPrimitiveAttribute.ValidatorFlagsType );
  public static readonly string ValidatorFlagsDefaultValueKey = nameof( TypedPrimitiveAttribute.ValidatorFlagsDefaultValue );
  public static readonly string StringComparisonKey = nameof( TypedPrimitiveAttribute.StringComparison );

  #endregion

  #region Public Methods

  // Select readonly record structs that have attributes
  public static bool IsGenerationTarget(
    SyntaxNode syntaxNode,
    CancellationToken _ )
  {
    return syntaxNode is RecordDeclarationSyntax { Modifiers: { } modifiers } recordSyntax &&
           modifiers.Any( modifier => modifier.IsKind( SyntaxKind.ReadOnlyKeyword ) ) &&
           recordSyntax.AttributeLists.Count > 0;
  }

  // Select readonly records that have the marker attribute
  public static Result<GeneratorModel> GetTypedPrimitiveToGenerate(
    GeneratorAttributeSyntaxContext context,
    CancellationToken _ )
  {
    if( context.TargetSymbol is not INamedTypeSymbol recordSymbol )
    {
      return Result.Fail<GeneratorModel>( UnexpectedDiagnostic.Create( "Expected a named type symbol", null ) );
    }

    var recordSyntax = ( RecordDeclarationSyntax ) context.TargetNode;

    // Ensure it's a partial record
    if( !IsPartial( recordSyntax ) )
    {
      return Result.Fail<GeneratorModel>( NotPartialDiagnostic.Create( recordSyntax ) );
    }

    // Get attribute values
    GeneratorModel? model = null;
    List<DiagnosticInfo>? diagnostics = null;

    foreach( var attributeData in recordSymbol.GetAttributes() )
    {
      if( attributeData.AttributeClass?.Name != MarkerAttributeName )
      {
        continue;
      }

      var attrSyntax = attributeData.ApplicationSyntaxReference?.GetSyntax();
      var result = CreatePrimitiveToGenerate( recordSymbol, attributeData, attrSyntax );
      if( result.IsFailed )
      {
        diagnostics ??= [];
        diagnostics.AddRange( result.Errors );
      }

      model = result.Value;
    }

    if( diagnostics is not null && diagnostics.Count > 0 )
    {
      return Result.Fail<GeneratorModel>( diagnostics );
    }

    Debug.Assert( model != null );
    return Result.Ok( model!.Value );

    static bool IsPartial(
      RecordDeclarationSyntax syntax )
    {
      return syntax.Modifiers.Any( modifier => modifier.IsKind( SyntaxKind.PartialKeyword ) );
    }
  }

  #endregion

  #region Implementation

  private static Result<GeneratorModel> CreatePrimitiveToGenerate(
    INamedTypeSymbol recordSymbol,
    AttributeData attributeData,
    SyntaxNode? attributeSyntax )
  {
    var primitiveType = GetPrimitiveType( attributeData );
    if( primitiveType is null )
    {
      return Result.Fail<GeneratorModel>( MissingPrimitiveTypeDiagnostic.Create( attributeSyntax ) );
    }

    if( !TypeManager.IsTypeSupported( primitiveType ) )
    {
      return Result.Fail<GeneratorModel>( UnsupportedTypeDiagnostic.Create( attributeSyntax, primitiveType ) );
    }

    var namedArguments = attributeData.NamedArguments;
    var converters = namedArguments.GetEnumValue<TypedPrimitiveConverter>( ConvertersKey ) ??
                     TypedPrimitiveConverter.Default;
    var validatorTypeName = namedArguments.GetTypeName( ValidatorTypeKey );
    var validatorFlagsType = namedArguments.GetTypeValue( ValidatorFlagsTypeKey );
    var validatorFlagsDefaultValue = namedArguments.GetValue( ValidatorFlagsDefaultValueKey );
    var nameSpace = recordSymbol.ContainingNamespace.ToDisplayString();
    var name = recordSymbol.Name;
    string? validatorFlagsTypeName;

    string? defaultValue = null;
    if( validatorFlagsType is not null )
    {
      // The validator flags must be an enum
      if( !validatorFlagsType.IsEnum )
      {
        return Result.Fail<GeneratorModel>( ValidatorFlagsMustBeEnum.Create( attributeSyntax ) );
      }

      validatorFlagsTypeName = validatorFlagsType.FullName;

      // Find the default value if one was defined; otherwise use "default"
      if( validatorFlagsDefaultValue is not null )
      {
        var value = Enum.GetName( validatorFlagsType, validatorFlagsDefaultValue );
        defaultValue = $"{validatorFlagsType.FullName}.{value}";
      }
      else
      {
        defaultValue = "default";
      }
    }
    else
    {
      // The validator flags type assembly is not loaded in the current AppDomain,
      // We just use the type name directly and cast the default value if present
      validatorFlagsTypeName = namedArguments.GetTypeName( ValidatorFlagsTypeKey );

      // NO need for a default value if not using the validation flags
      if( validatorFlagsTypeName is not null )
      {
        defaultValue = validatorFlagsDefaultValue is not null
          ? $"({validatorFlagsTypeName}) {validatorFlagsDefaultValue}"
          : "default";
      }
    }

    string? stringComparison = null;
    if( primitiveType == typeof( string ) )
    {
      var comparison = StringComparison.OrdinalIgnoreCase;
      var stringComparisonValue = namedArguments.GetEnumValue<StringComparison>( StringComparisonKey );
      if( stringComparisonValue is not null )
      {
        comparison = stringComparisonValue.Value;
      }

      stringComparison = $"{typeof( StringComparison ).FullName}.{comparison}";
    }

    var model = new GeneratorModel(
      primitiveType,
      name,
      nameSpace,
      converters,
      validatorTypeName,
      validatorFlagsTypeName,
      defaultValue,
      stringComparison
    );

    return Result.Ok( model );
  }

  private static Type? GetPrimitiveType(
    AttributeData attributeData )
  {
    var args = attributeData.ConstructorArguments;

    // The first argument should always be the primitive type
    if( args.Length > 0 )
    {
      return args[0]
        .GetTypeValue();
    }

    return null;
  }

  #endregion
}
