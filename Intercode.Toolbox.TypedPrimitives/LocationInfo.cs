// Module Name: LocationInfo.cs
// Author:      Eduardo Velasquez
// Copyright (c) 2024, Intercode Consulting, Inc.

namespace Intercode.Toolbox.TypedPrimitives;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

internal record LocationInfo(
  string FilePath,
  TextSpan TextSpan,
  LinePositionSpan LinePositionSpan )
{
  #region Public Methods

  public static LocationInfo Create(
    SyntaxNode syntaxNode )
  {
    var location = syntaxNode.GetLocation();
    return new LocationInfo(
      location.SourceTree!.FilePath,
      location.SourceSpan,
      location.GetLineSpan()
              .Span
    );
  }

  public Location ToLocation()
  {
    return Location.Create( FilePath, TextSpan, LinePositionSpan );
  }

  #endregion
}
