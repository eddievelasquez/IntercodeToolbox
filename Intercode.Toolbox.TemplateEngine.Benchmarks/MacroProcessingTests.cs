// Module Name: MacroProcessingTests.cs
// Author:      Eduardo Velasquez
// Copyright (c) 2024, Intercode Consulting, Inc.

#pragma warning disable CS0618 // Type or member is obsolete

namespace Intercode.Toolbox.TemplateEngine.Benchmarks;

using System.Text;
using System.Text.RegularExpressions;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;

[HideColumns( "Error", "StdDev", "Median", "RatioSD" )]
[MemoryDiagnoser]
[SimpleJob( RuntimeMoniker.Net80, baseline: true )]
[SimpleJob( RuntimeMoniker.Net90 )]
public partial class MacroProcessingTests
{
  #region Fields

  private readonly Template _template;
  private readonly MacroProcessor _macroProcessor;
  private readonly IReadOnlyDictionary<string, string> _macros;
  private readonly TemplateMacroValues _macroValues;

  #endregion

  #region Constructors

  public MacroProcessingTests()
  {
    var helper = new TemplateEngineHelper();
    _template = helper.Compile();
    _macroProcessor = helper.CreateMacroProcessor();
    _macros = helper.Macros;
    _macroValues = helper.CreateMacroValues( _template );
  }

  #endregion

  #region Public Methods

  [Benchmark( OperationsPerInvoke = 3 )]
  public void UsingMacroProcessorWithPooledStringBuilder()
  {
    var builder = StringBuilderPool.Default.Get();

    try
    {
      _macroProcessor.ProcessMacros( _template, builder );
    }
    finally
    {
      StringBuilderPool.Default.Return( builder );
    }
  }

  [Benchmark( OperationsPerInvoke = 3 )]
  public void UsingOptimizedMacroProcessorWithPooledStringBuilder()
  {
    var builder = StringBuilderPool.Default.Get();

    try
    {
      _macroProcessor.ProcessMacros( _macroValues, builder );
    }
    finally
    {
      StringBuilderPool.Default.Return( builder );
    }
  }

  [Benchmark( OperationsPerInvoke = 3 )]
  public void UsingMacroProcessorWithStringBuilder()
  {
    var builder = new StringBuilder();
    _macroProcessor.ProcessMacros( _template, builder );
  }

  [Benchmark( OperationsPerInvoke = 3 )]
  public void UsingOptimizedMacroProcessorWithStringBuilder()
  {
    var builder = new StringBuilder();
    _macroProcessor.ProcessMacros( _macroValues, builder );
  }

  [Benchmark( OperationsPerInvoke = 3 )]
  public void UsingMacroProcessorWithTextWriter()
  {
    var writer = new StringWriter();
    _macroProcessor.ProcessMacros( _template, writer );
  }

  [Benchmark( OperationsPerInvoke = 3 )]
  public void UsingOptimizedMacroProcessorWithTextWriter()
  {
    var writer = new StringWriter();
    _macroProcessor.ProcessMacros( _macroValues, writer );
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
