// // Module Name: Program.cs
// // Author:      Eddie Velasquez
// // Copyright (c) 2025, Intercode Consulting, Inc.

using BenchmarkDotNet.Running;
using Intercode.Toolbox.TemplateEngine.Benchmarks;

BenchmarkSwitcher.FromAssembly(typeof(MacroProcessingBenchmarks).Assembly).Run(args);
