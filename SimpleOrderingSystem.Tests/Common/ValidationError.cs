namespace SimpleOrderingSystem.Tests.Common;

/// <summary>
/// Can use with <see cref="HttpResponseMessageExtensions.ShouldBeTheFollowingModelStateValidationBadRequestAsync"/>
///
/// Can be implicitly assigned:
///     string: for a general error
///     (string key, string value): for single error tied to a key
///     (string key, string[] values) for multiple errors tied to a key
/// </summary>
public readonly struct ValidationError
{
    private ValidationError(string key, string[] values)
    {
        Key = key;
        Values = values;
    }

    private ValidationError(string key, string value)
        : this(key, new[] { value })
    {
    }

    private ValidationError(string value)
        : this(string.Empty, value)
    {
    }

    /// <summary>
    /// Gets validation error key.
    /// </summary>
    public string Key { get; }

    /// <summary>
    /// Gets validation error values.
    /// </summary>
    public string[] Values { get; }

    /// <summary>
    /// Validation error operator.
    /// </summary>
    /// <param name="genericError"></param>
    public static implicit operator ValidationError(string genericError)
    {
        return new ValidationError(genericError);
    }

    /// <summary>
    /// Validation error operator.
    /// </summary>
    /// <param name="error"></param>
    public static implicit operator ValidationError((string Key, string Value) error)
    {
        var (key, value) = error;
        return new ValidationError(key, value);
    }

    /// <summary>
    /// Validation error operator.
    /// </summary>
    /// <param name="error"></param>
    public static implicit operator ValidationError((string Key, string[] Values) error)
    {
        var (key, values) = error;
        return new ValidationError(key, values);
    }

    /// <summary>
    /// Creates model state validation error for a required property.
    /// </summary>
    /// <param name="requiredField"></param>
    /// <returns></returns>
    public static ValidationError CreateRequiredFieldError(string requiredField)
    {
        return (requiredField, $"The {requiredField} field is required.");
    }

    /// <summary>
    /// "A non-empty request body is required.".
    /// </summary>
    /// <returns></returns>
    public static ValidationError CreateNonEmptyRequestBodyRequiredError()
    {
        return "A non-empty request body is required.";
    }
}
