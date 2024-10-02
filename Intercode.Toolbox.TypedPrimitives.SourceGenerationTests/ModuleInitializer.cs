// Module Name: ModuleInitializer.cs
// Author:      Eduardo Velasquez
// Copyright (c) 2024, Intercode Consulting, Inc.

namespace Intercode.Toolbox.TypedPrimitives.SourceGenerationTests;

using System.Runtime.CompilerServices;
using FluentResults;

internal static class ModuleInitializer
{
  #region Public Methods

  [ModuleInitializer]
  public static void Init()
  {
    VerifySourceGenerators.Initialize();
    VerifyDiffPlex.Initialize();

    // Force loading of the Intercode.Toolbox.TypePrimitives.Annotations and FluentResults assemblies.
    var x = typeof( TypedPrimitiveAttribute );
    var y = typeof( Result );
  }

  #endregion
}
