// Module Name: DisposerTests.cs
// Author:      Eduardo Velasquez
// Copyright (c) 2024, Intercode Consulting, Inc.

namespace Intercode.Toolbox.Core.Tests;

using FluentAssertions;

public class DisposerTests
{
  #region Tests

  [Fact]
  public void Disposer_ShouldInvokeAction()
  {
    var invoked = false;
    var disposer = new Disposer( () => invoked = true );
    disposer.Dispose();

    invoked.Should()
           .BeTrue();
  }

  [Fact]
  public void Disposer_ShouldNotThrow_WhenActionIsNull()
  {
    var disposer = new Disposer( null );
    var act = () => disposer.Dispose();
    act.Should()
       .NotThrow();
  }

  [Fact]
  public void Disposer_ShouldOnlyCallTheActionOnce_WhenDisposeIsCalledMultipleTimes()
  {
    var count = 0;
    var disposer = new Disposer( () => ++count );
    disposer.Dispose();
    disposer.Dispose();
    disposer.Dispose();

    count.Should()
         .Be( 1 );
  }

  #endregion
}
