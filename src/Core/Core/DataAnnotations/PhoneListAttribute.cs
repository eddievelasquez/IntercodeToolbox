// Module Name: PhoneListAttribute.cs
// Author:      Eduardo Velasquez
// Copyright (c) 2024, Intercode Consulting, Inc.

namespace Intercode.Toolbox.Core.DataAnnotations;

using System.ComponentModel.DataAnnotations;

/// <summary>
///   Represents a custom validation attribute that validates a list of phone numbers.
/// </summary>
/// <remarks>
///   This attribute is used to validate a property that contains a list of phone numbers.
///   It checks if each phone number in the list is valid according to the <see cref="PhoneAttribute.IsValid(object)" />.
/// </remarks>
[AttributeUsage( AttributeTargets.Property )]
public sealed class PhoneListAttribute: ValidationAttribute
{
  #region Constants

  private const string DEFAULT_ERROR_MESSAGE = "'{0}' contains an invalid phone number.";
  private static readonly PhoneAttribute s_phoneAttr = new ();

  #endregion

  #region Constructors

  /// <summary>
  ///   Initializes a new instance of the <see cref="PhoneListAttribute" /> class.
  /// </summary>
  public PhoneListAttribute()
    : base( DEFAULT_ERROR_MESSAGE )
  {
  }

  /// <summary>
  ///   Initializes a new instance of the <see cref="PhoneListAttribute" /> class with a custom error message.
  /// </summary>
  /// <param name="errorMessage">The error message to display if the validation fails.</param>
  public PhoneListAttribute(
    string errorMessage )
    : base( errorMessage )
  {
  }

  /// <summary>
  ///   Initializes a new instance of the <see cref="PhoneListAttribute" /> class with a custom error message accessor.
  /// </summary>
  /// <param name="errorMessageAccessor">A function that returns the error message to display if the validation fails.</param>
  public PhoneListAttribute(
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
  /// <returns><c>true</c> if the value is a valid list of phone numbers; otherwise, <c>false</c>.</returns>
  public override bool IsValid(
    object? value )
  {
    return value is IEnumerable<string> phoneList && phoneList.All( s_phoneAttr.IsValid );
  }

  /// <summary>
  ///   Formats the error message to display if the validation fails.
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
