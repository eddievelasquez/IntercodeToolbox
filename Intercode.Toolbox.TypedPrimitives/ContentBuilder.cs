// Module Name: ContentBuilder.cs
// Author:      Eduardo Velasquez
// Copyright (c) 2024, Intercode Consulting, Inc.

namespace Intercode.Toolbox.TypedPrimitives;

using System.Text;

internal class ContentBuilder: IDisposable
{
  #region Constants

  private const int DEFAULT_BUILDER_CAPACITY = 1204;

  public static readonly StringBuilderPool StringBuilderPool = new ();

  #endregion

  #region Fields

  private readonly bool _builderOwned;

  #endregion

  #region Constructors

  public ContentBuilder(
    int capacity = 0 )
  {
    StringBuilder = StringBuilderPool.Get();
    StringBuilder.EnsureCapacity( Math.Max( capacity, DEFAULT_BUILDER_CAPACITY ) );
    _builderOwned = true;
  }

  public ContentBuilder(
    StringBuilder builder )
  {
    StringBuilder = builder;
    _builderOwned = false;
  }

  #endregion

  #region Properties

  public StringBuilder StringBuilder { get; }

  #endregion

  #region Public Methods

  public void EnsureCapacity(
    int capacity )
  {
    StringBuilder.EnsureCapacity( capacity );
  }

  public void Clear()
  {
    StringBuilder.Clear();
  }

  public void AppendLine(
    string text )
  {
    StringBuilder.AppendLine( text );
  }

  public void Append(
    string text )
  {
    StringBuilder.Append( text );
  }

  public override string ToString()
  {
    return StringBuilder.ToString();
  }

  public void Dispose()
  {
    if( _builderOwned )
    {
      StringBuilderPool.Return( StringBuilder );
    }

    GC.SuppressFinalize( this );
  }

  #endregion
}
