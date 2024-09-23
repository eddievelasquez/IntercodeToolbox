namespace Intercode.Toolbox.TypedPrimitives.SourceGenerationTests;

using System.Runtime.CompilerServices;
using FluentResults;
using Intercode.Toolbox.TypedPrimitives;
using VerifyTests;

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
