// Module Name: Program.cs
// Author:      Eduardo Velasquez
// Copyright (c) 2024, Intercode Consulting, Inc.

using System.Runtime.CompilerServices;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using Intercode.Toolbox.Core;

BenchmarkSwitcher.FromAssembly( typeof( DisposerTests ).Assembly )
                 .Run( args );

[HideColumns( "Error", "StdDev", "Median", "RatioSD" )]
[DisassemblyDiagnoser( 0 )]
public class DisposerTests
{
  #region Public Methods

  [Benchmark( Baseline = true )]
  public void CallingWithDisposable()
  {
    using var disposer = WithDisposable();
  }

  [Benchmark]
  public void CallingWithDisposer()
  {
    using var disposer = WithDispose();
  }

  #endregion

  #region Implementation

  [MethodImpl( MethodImplOptions.NoInlining )]
  internal Disposer WithDispose()
  {
    return new Disposer( () => { } );
  }

  [MethodImpl( MethodImplOptions.NoInlining )]
  internal IDisposable WithDisposable()
  {
    return new Disposer( () => { } );
  }

  #endregion
}
