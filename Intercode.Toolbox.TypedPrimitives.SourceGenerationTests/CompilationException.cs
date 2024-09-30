// Module Name: CompilationException.cs
// Author:      Eduardo Velasquez
// Copyright (c) 2024, Intercode Consulting, Inc.

namespace Intercode.Toolbox.TypedPrimitives.SourceGenerationTests;

using System.Runtime.CompilerServices;
using Microsoft.CodeAnalysis;

public class CompilationException: Exception
{
  #region Constructors

  public CompilationException(
    IEnumerable<Diagnostic> diagnostics )
  {
    Diagnostics = diagnostics.ToArray();
  }

  public CompilationException(
    string? message,
    IEnumerable<Diagnostic> diagnostics )
    : base( message )
  {
    Diagnostics = diagnostics.ToArray();
  }

  public CompilationException(
    string? message,
    IEnumerable<Diagnostic> diagnostics,
    Exception? innerException )
    : base( message, innerException )
  {
    Diagnostics = diagnostics.ToArray();
  }

  #endregion

  #region Properties

  public Diagnostic[] Diagnostics { get; }

  #endregion

  #region Public Methods

  public static void ThrowIfErrors(
    IEnumerable<Diagnostic> diagnostics,
    [CallerArgumentExpression( nameof( diagnostics ) )] string? paramName = null )
  {
    List<Diagnostic>? errorList = null;
    foreach( var diagnostic in diagnostics )
    {
      if( diagnostic.Severity != DiagnosticSeverity.Error )
      {
        continue;
      }

      errorList ??= [];
      errorList.Add( diagnostic );
    }

    if( errorList is not null )
    {
      throw new CompilationException( errorList[0].GetMessage(), errorList );
    }
  }

  #endregion
}
