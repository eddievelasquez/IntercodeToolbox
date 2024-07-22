// Module Name: TestTraceListener.cs
// Author:      Eduardo Velasquez
// Copyright (c) 2024, Intercode Consulting, Inc.

namespace Intercode.Toolbox.UnitTesting;

using System.Diagnostics;
using System.Text;

internal class TestTraceListener: TraceListener
{
  #region Fields

  private readonly StringBuilder _buffer = new ();

  #endregion

  #region Public Methods

  public void ClearTrace()
  {
    _buffer.Clear();
  }

  public string GetTrace()
  {
    return _buffer.ToString();
  }

  public override void Write(
    string? message )
  {
    if( message is null )
    {
      return;
    }

    if( NeedIndent )
    {
      WriteIndent();
    }

    _buffer.Append( message );
  }

  public override void WriteLine(
    string? message )
  {
    Write( message );

    _buffer.AppendLine();
  }

  public override void Fail(
    string? message )
  {
    WriteLine( message );
    throw new DebugAssertException( message );
  }

  public override void Fail(
    string? message,
    string? detailMessage )
  {
    WriteLine( $"{message}{detailMessage}" );
    throw new DebugAssertException( message, detailMessage );
  }

  #endregion
}
