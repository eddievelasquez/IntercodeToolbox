// Module Name: Program.cs
// Author:      Eduardo Velasquez
// Copyright (c) 2024, Intercode Consulting, Inc.

using System.Text;
using System.Text.RegularExpressions;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using Intercode.Toolbox.TemplateEngine;
using Intercode.Toolbox.TemplateEngine.Benchmarks;

#if PROFILING
var tests = new MacroProcessingTests();

for( int i = 0; i < 100; ++i )
{
  tests.UsingTemplateEngine();
}

return;
#else

BenchmarkSwitcher.FromAssembly( typeof( MacroProcessingTests ).Assembly )
                 .Run( args );

#endif

[HideColumns( "Error", "StdDev", "Median", "RatioSD" )]
[MemoryDiagnoser]
public partial class MacroProcessingTests
{
  #region Fields

  private readonly Template _template;
  private readonly MacroProcessor _macroProcessor;
  private readonly IReadOnlyDictionary<string, string> _macros;

  #endregion

  #region Constructors

  public MacroProcessingTests()
  {
    var helper = new TemplateEngineHelper();
    _template = helper.Compile();
    _macroProcessor = helper.CreateMacroProcessor();
    _macros = _macroProcessor.GetMacros();
  }

  #endregion

  #region Public Methods

  [Benchmark( OperationsPerInvoke = 3 )]
  public void UsingMacroProcessor()
  {
    var writer = new StringWriter();
    _macroProcessor.ProcessMacros( _template, writer );
  }

  [Benchmark( Baseline = true, OperationsPerInvoke = 3 )]
  public void UsingStringBuilderReplace()
  {
    var sb = new StringBuilder( _template.Text );
    foreach( var (macro, value) in _macros )
    {
      sb.Replace( macro, value );
    }

    var processed = sb.ToString();
  }

  [Benchmark( OperationsPerInvoke = 3 )]
  public void UsingRegularExpressions()
  {
    var result = CreateMacroNameRegex()
      .Replace(
        _template.Text,
        match =>
        {
          var key = match.Groups[1].Value;
          return _macros.TryGetValue( key, out var value ) ? value : match.Value;
        }
      );
  }

  #endregion

  #region Implementation

  [GeneratedRegex( @"\$([^$]+)\$" )]
  private static partial Regex CreateMacroNameRegex();

  #endregion
}
