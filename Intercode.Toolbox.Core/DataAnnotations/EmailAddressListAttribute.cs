// Module Name: EmailAddressListAttribute.cs
// Author:      Eduardo Velasquez
// Copyright (c) 2024, Intercode Consulting, Inc.

namespace Intercode.Toolbox.Core.DataAnnotations;

using System.ComponentModel.DataAnnotations;

/// <summary>
///   Represents a custom validation attribute that validates a list of email addresses.
/// </summary>
/// <remarks>
///   This attribute is used to validate a property that contains a list of email addresses.
///   It checks if each email in the list is valid according to the <see cref="EmailAddressAttribute.IsValid(object)" />.
/// </remarks>
[AttributeUsage( AttributeTargets.Property )]
public sealed class EmailAddressListAttribute: ValidationAttribute
{
  #region Constants

  private const string DEFAULT_ERROR_MESSAGE = "'{0}' contains an invalid email address.";
  private static readonly EmailAddressAttribute s_emailAddressAttr = new ();

  #endregion

  #region Constructors

  /// <summary>
  ///   Initializes a new instance of the <see cref="EmailAddressListAttribute" /> class.
  /// </summary>
  public EmailAddressListAttribute()
    : base( DEFAULT_ERROR_MESSAGE )
  {
  }

  /// <summary>
  ///   Initializes a new instance of the <see cref="EmailAddressListAttribute" /> class with a custom error message.
  /// </summary>
  /// <param name="errorMessage">The error message to display.</param>
  public EmailAddressListAttribute(
    string errorMessage )
    : base( errorMessage )
  {
  }

  /// <summary>
  ///   Initializes a new instance of the <see cref="EmailAddressListAttribute" /> class with a custom error message
  ///   accessor.
  /// </summary>
  /// <param name="errorMessageAccessor">A function that returns the error message to display.</param>
  public EmailAddressListAttribute(
    Func<string> errorMessageAccessor )
    : base( errorMessageAccessor )
  {
  }

  #endregion

  #region Public Methods

  /// <summary>
  ///   Determines whether the specified value is valid.
  /// </summary>
  /// <param name="value">The value to validate.</param>
  /// <returns><c>true</c> if the value is valid; otherwise, <c>false</c>.</returns>
  public override bool IsValid(
    object? value )
  {
    return value is IEnumerable<string> emailList && emailList.All( s_emailAddressAttr.IsValid );
  }

  /// <summary>
  ///   Formats the error message.
  /// </summary>
  /// <param name="name">The name of the property being validated.</param>
  /// <returns>The formatted error message.</returns>
  public override string FormatErrorMessage(
    string name )
  {
    return string.Format( ErrorMessageString, name );
  }

  #endregion
}
