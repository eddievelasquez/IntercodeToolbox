// Module Name: Program.cs
// Author:      Eduardo Velasquez
// Copyright (c) 2024, Intercode Consulting, Inc.

using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Exporters.Csv;
using BenchmarkDotNet.Reports;
using BenchmarkDotNet.Running;
using Intercode.Toolbox.TemplateEngine.Benchmarks;
using Perfolizer.Metrology;

#if PROFILING
var tests = new MacroProcessingTests();

for( int i = 0; i < 100; ++i )
{
  tests.UsingTemplateEngine();
}

return;
#else

var exporter = new CsvExporter(
  CsvSeparator.CurrentCulture,
  new SummaryStyle(
    cultureInfo: System.Globalization.CultureInfo.CurrentCulture,
    printUnitsInHeader: true,
    printUnitsInContent: false,
    timeUnit: Perfolizer.Horology.TimeUnit.Nanosecond,
    sizeUnit: SizeUnit.B
  ));

var config = ManualConfig.CreateMinimumViable().AddExporter(exporter);

BenchmarkSwitcher.FromAssembly( typeof( MacroProcessingTests ).Assembly )
                 .Run( args );

#endif
