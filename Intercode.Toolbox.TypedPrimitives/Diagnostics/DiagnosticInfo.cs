// Module Name: DiagnosticInfo.cs
// Author:      Eduardo Velasquez
// Copyright (c) 2024, Intercode Consulting, Inc.

namespace Intercode.Toolbox.TypedPrimitives.Diagnostics;

using System.Collections.Immutable;
using Microsoft.CodeAnalysis;

// Cache-friendly collection of diagnostic information to used to generate a
// Diagnostic instance, which may not be cache friendly.
internal sealed record DiagnosticInfo(
  DiagnosticDescriptor Descriptor,
  Location? Location,
  string? MessageArg = null,
  EquatableArray<StringPair> Properties = default )
{
  #region Public Methods

  public Diagnostic ToDiagnostic()
  {
    var messageArgs = MessageArg != null ? new object[] { MessageArg } : null;

    ImmutableDictionary<string, string?>? properties = null;
    if( Properties is { Length: > 0 } props )
    {
      var builder = ImmutableDictionary.CreateBuilder<string, string>();
      foreach( var pair in props )
      {
        builder.Add( pair.Key, pair.Value );
      }

      properties = builder.ToImmutable()!;
    }

    var diagnostic = Diagnostic.Create( Descriptor, Location ?? Location.None, messageArgs, properties );
    return diagnostic;
  }

  #endregion
}
