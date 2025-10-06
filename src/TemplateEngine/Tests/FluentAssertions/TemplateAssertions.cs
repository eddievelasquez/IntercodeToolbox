// Module Name: TemplateAssertions.cs
// Author:      Eduardo Velasquez
// Copyright (c) 2025, Intercode Consulting, Inc.

namespace Intercode.Toolbox.TemplateEngine.Tests.FluentAssertions;

using System.Diagnostics;
using global::FluentAssertions.Execution;
using global::FluentAssertions.Primitives;

[DebuggerNonUserCode]
internal sealed class TemplateAssertions: ReferenceTypeAssertions<Template, TemplateAssertions>
{
  #region Constructors

  public TemplateAssertions(
    Template template )
    : base( template )
  {
  }

  #endregion

  #region Properties

  protected override string Identifier => "template";

  #endregion

  #region Public Methods

  [CustomAssertion]
  public AndConstraint<TemplateAssertions> HaveText(
    string expected,
    string because = "",
    params object[] becauseArgs )
  {
    Execute.Assertion
           .BecauseOf( because, becauseArgs )
           .ForCondition( Subject.Text == expected )
           .FailWith(
             "Expected {context:template} text to be {0}{reason}, but found {1}.",
             expected,
             Subject.Text
           );

    return new AndConstraint<TemplateAssertions>( this );
  }

  public AndWhichConstraint<TemplateAssertions, SegmentAssertions> HaveSingleSegment(
    string because = "",
    params object[] becauseArgs )
  {
    using var _ = new AssertionScope();

    Execute.Assertion
           .BecauseOf( because, becauseArgs )
           .ForCondition( Subject.Segments.Length == 1 )
           .FailWith(
             "Expected {context:template} to contain a single segment{reason}, but found {0}.",
             Subject.Segments.Length
           );

    var seg = Subject.Segments[0];

    return new AndWhichConstraint<TemplateAssertions, SegmentAssertions>(
      this,
      new SegmentAssertions( Subject, seg )
    );
  }

  public AndWhichConstraint<TemplateAssertions, SegmentAssertions> HaveSegmentAt(
    int index,
    string because = "",
    params object[] becauseArgs )
  {
    using var _ = new AssertionScope();

    Execute.Assertion
           .BecauseOf( because, becauseArgs )
           .ForCondition( index >= 0 && index < Subject.Segments.Length )
           .FailWith(
             "Expected {context:template} to have a segment at index {0}{reason}, but valid range is [0..{1}).",
             index,
             Subject.Segments.Length
           );

    var seg = Subject.Segments[index];

    return new AndWhichConstraint<TemplateAssertions, SegmentAssertions>(
      this,
      new SegmentAssertions( Subject, seg )
    );
  }

  public AndConstraint<TemplateAssertions> HaveSegmentCount(
    int expected,
    string because = "",
    params object[] becauseArgs )
  {
    Execute.Assertion
           .BecauseOf( because, becauseArgs )
           .ForCondition( Subject.Segments.Length == expected )
           .FailWith(
             "Expected {context:template} to have {0} segments{reason}, but found {1}.",
             expected,
             Subject.Segments.Length
           );

    return new AndConstraint<TemplateAssertions>( this );
  }

  #endregion
}
