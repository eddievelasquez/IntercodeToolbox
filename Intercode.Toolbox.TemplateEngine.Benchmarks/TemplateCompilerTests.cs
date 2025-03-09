// Module Name: TemplateCompilerTests.cs
// Author:      Eduardo Velasquez
// Copyright (c) 2024, Intercode Consulting, Inc.

namespace Intercode.Toolbox.TemplateEngine.Benchmarks;

using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;

[HideColumns( "Error", "StdDev", "Median", "RatioSD" )]
[MemoryDiagnoser]
[SimpleJob( RuntimeMoniker.Net80, baseline: true )]
[SimpleJob( RuntimeMoniker.Net90 )]
public partial class TemplateCompilerTests
{
  #region Fields

  private readonly TemplateCompiler _compiler = new ();

  #endregion

  #region Public Methods

  [Benchmark]
  public void UsingTemplateCompiler()
  {
    var template = _compiler.Compile( TemplateEngineHelper.TemplateText );
  }

  #endregion
}
