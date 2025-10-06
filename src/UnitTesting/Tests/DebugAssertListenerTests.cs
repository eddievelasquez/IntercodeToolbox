// Module Name: DebugAssertListenerTests.cs
// Author:      Eduardo Velasquez
// Copyright (c) 2024, Intercode Consulting, Inc.

namespace Intercode.Toolbox.UnitTesting.Tests;

using System.Diagnostics;
using FluentAssertions;
using Xunit;

public class DebugAssertListenerTests
{
  #region Tests

  [Fact]
  public void Constructor_ShouldInterceptAssertFailure()
  {
    using var listener = new DebugAssertListener();
    var act = () => Debug.Assert( false, "This is a test" );

    act.Should()
       .Throw<DebugAssertException>();
  }

  [Fact]
  public void Constructor_ShouldReplaceFirstListener()
  {
    var originalFirstListener = Trace.Listeners.Count > 0 ? Trace.Listeners[0] : null;

    using( var listener = new DebugAssertListener() )
    {
      GetFirstListener()
        .Should()
        .BeSameAs( listener.InnerListener );
    }

    GetFirstListener()
      .Should()
      .BeSameAs( originalFirstListener );

    static TraceListener? GetFirstListener()
    {
      return Trace.Listeners.Count > 0 ? Trace.Listeners[0] : null;
    }
  }

  [Fact]
  public void GetTrace_ShouldReturnTrace_WhenAssertionFails()
  {
    using var listener = new DebugAssertListener();
    var act = () => Debug.Assert( false, "This is a test" );

    act.Should()
       .Throw<DebugAssertException>();

    var trace = listener.GetTrace();

    trace.Should()
         .Be( $"This is a test{Environment.NewLine}" );
  }

  #endregion
}
