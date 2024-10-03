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
  public static readonly string GenericMarkerAttributeFullName = typeof( TypedPrimitiveAttribute<> ).FullName!;
  public static readonly string ConvertersKey = nameof( TypedPrimitiveAttribute.Converters );
  public static readonly string StringComparisonKey = nameof( TypedPrimitiveAttribute.StringComparison );

  #endregion

  #region Public Methods

  // Only target structs as this method is called by SyntaxProvider.ForAttributeWithMetadataName,
  // which ensures that the syntax node has the marker attribute, which is only applicable
  // to structs.
  //
  // The partial and readonly modifiers are validated in GetTypedPrimitiveToGenerate
  // method to ensure the user gets an error message that is more specific.
  public static bool IsGenerationTarget(
    SyntaxNode syntaxNode,
    CancellationToken _ )
  {
    return syntaxNode is StructDeclarationSyntax;
  }

  // Obtain the generation model for the target type
  public static Result<GeneratorModel> GetTypedPrimitiveToGenerate(
    GeneratorAttributeSyntaxContext context,
    CancellationToken _ )
  {
    // Ensure the target symbol is a named type symbol, but we should never get here because
    // syntax nodes are filtered to only include structs .
    if( context.TargetSymbol is not INamedTypeSymbol structSymbol )
    {
      return Error.UnexpectedFailure(
        $"'{context.TargetSymbol.QualifiedName()}' is not a named type symbol",
        context.TargetNode
      );
    }

    var structSyntax = ( StructDeclarationSyntax ) context.TargetNode;
    if( !IsPartial( structSyntax ) )
    {
      return Error.NotPartialFailure( structSymbol, structSyntax );
    }

    if( !IsReadOnly( structSyntax ) )
    {
      return Error.NotReadOnlyFailure( structSymbol, structSyntax );
    }

    // Attribute data should never be null because the source generator's syntax provider
    // filters out by the attribute name, so this should have the marker attribute.
    var attributeData = structSymbol.GetAttributes()
                                    .Single( data => data.AttributeClass?.Name == MarkerAttributeName );

    var attrSyntax = attributeData.ApplicationSyntaxReference?.GetSyntax();
    var result = CreatePrimitiveToGenerate( structSymbol, attributeData, attrSyntax );
    if( result.IsFailed )
    {
      return Result.Fail<GeneratorModel>( result.Errors );
    }

    return Result.Ok( result.Value );

    static bool IsPartial(
      StructDeclarationSyntax syntax )
    {
      return syntax.Modifiers.Any( modifier => modifier.IsKind( SyntaxKind.PartialKeyword ) );
    }

    static bool IsReadOnly(
      StructDeclarationSyntax syntax )
    {
      return syntax.Modifiers.Any( modifier => modifier.IsKind( SyntaxKind.ReadOnlyKeyword ) );
    }
  }

  #endregion

  #region Implementation

  private static Result<GeneratorModel> CreatePrimitiveToGenerate(
    INamedTypeSymbol recordSymbol,
    AttributeData attributeData,
    SyntaxNode? attributeSyntax )
  {
    var result = GetPrimitiveType( attributeData );

    // Ensure that the primitive type was found
    if( result.IsFailed )
    {
      return Result.Fail<GeneratorModel>( result.Errors );
    }

    var primitiveType = result.Value!;
    if( !TypeManager.IsTypeSupported( primitiveType ) )
    {
      return Error.UnsupportedTypeFailure( primitiveType, attributeSyntax );
    }

    var namedArguments = attributeData.NamedArguments;
    var converters = namedArguments.GetEnumValue<TypedPrimitiveConverter>( ConvertersKey ) ??
                     TypedPrimitiveConverter.None;
    var @namespace = recordSymbol.ContainingNamespace.ToDisplayString();
    var typeName = recordSymbol.Name;

    string? stringComparison = null;
    if( primitiveType == typeof( string ) )
    {
      var stringComparisonValue = namedArguments.GetEnumValue<StringComparison>( StringComparisonKey );
      var comparison = stringComparisonValue ?? StringComparison.OrdinalIgnoreCase;
      stringComparison = $"{typeof( StringComparison ).FullName}.{comparison}";
    }

    var model = new GeneratorModel(
      primitiveType,
      typeName,
      @namespace,
      converters,
      stringComparison
    );

    return Result.Ok( model );
  }

  private static Result<Type> GetPrimitiveType(
    AttributeData attributeData )
  {
    if( attributeData.AttributeClass!.IsGenericType )
    {
      var typeArg = attributeData.AttributeClass.TypeArguments.First();
      return typeArg.GetTypeValue();
    }

    var args = attributeData.ConstructorArguments;
    Debug.Assert( args.Length > 0 );

    // The first argument should always be the primitive type
    return args[0].GetTypeValue();
  }

  #endregion
}
