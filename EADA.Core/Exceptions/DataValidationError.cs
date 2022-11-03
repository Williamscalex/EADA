using EADA.Core.Domain.Messages;
using EADA.Core.Exceptions.JsonObjects;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Rbit.Exceptions;

namespace EADA.Core.Exceptions;

/// <summary>
/// Represents an error that occurred during a validation setup in preparation for a database transaction
/// </summary>
public class DataValidationError : HandledException
{
    public ValidationResult ValidationResult { get; protected set; }
    public bool UserExceptionMessage { get; protected set; }
    public string InvalidModelName { get; protected set; }

    /// <summary>
    /// Initializes the exception with the given information. This also sets the <see cref="UserExceptionMessage"/> to true.
    /// </summary>
    /// <param name="invalidModelName">A name describing the model that was invalidated.</param>
    /// <param name="friendlyMessage">A friendly message that will be shown to the user.</param>
    public DataValidationError(string invalidModelName, string friendlyMessage) : base(friendlyMessage)
    {
        ValidationResult = new ValidationResult(("", new string[] { }));
        UserExceptionMessage = false;
        InvalidModelName = invalidModelName;
    }

    public DataValidationError(string invalidModelName, ValidationResult validationResult) : base(
        "The input was invalid.")
    {
        ValidationResult = validationResult;
        UserExceptionMessage = false;
        InvalidModelName = invalidModelName;
    }

    /// <summary>
    /// Initializes a data validation exception.
    /// This is a shot=hand overload of the constructor method that accepts a <see cref="EADA.Core.Domain.Messages.ValidationResult"/>.
    /// This will instantiate a new validation result using the given <paramref name="invalidPropertyOrKeyName"/> and <paramref name="messages"/> properties,
    /// bu only for a single invalid property. If you have multiple invalid properties use the constructor that accepts an instance of <see cref="EADA.Core.Domain.Messages.ValidationResult"/>.
    /// </summary>
    /// <param name="invalidModelName">A name describing the model that ws invalidated.</param>
    /// <param name="invalidPropertyOrKeyName">
    /// The value to use as the key for the given error <paramref name="messages"/> collection.
    /// </param>
    /// <param name="messages">A collection of user-friendly validation error messages for the corresponding property.</param>
    public DataValidationError(string invalidModelName, string invalidPropertyOrKeyName, params string[] messages) :
        base("The input was invalid.")
    {
        ValidationResult = new ValidationResult((name: invalidPropertyOrKeyName, messages));
        UserExceptionMessage = false;
        InvalidModelName = invalidModelName;
    }
    /// <summary>
    /// Parses the given <see cref="FluentValidation.Results.ValidationResult"/> object into a
    /// custom validation object and stores it in this error for further processing.
    /// </summary>
    /// <param name="invalidModelName">A name describing the model that ws invalidated.</param>
    /// <param name="validationResult">The result of a FluentValidation.Net validator.</param>
    public DataValidationError(string invalidModelName, FluentValidation.Results.ValidationResult validationResult) :
        base("The input was invalid.")
    {
        ValidationResult = new ValidationResult(validationResult);
        UserExceptionMessage = false;
        InvalidModelName = invalidModelName;
    }

    /// <summary>
    /// Initializes the exception to describe this validation error with a <see cref="EADA.Core.Domain.Messages.ValidationResult"/>.
    /// This <see cref="EADA.Core.Domain.Messages.ValidationResult"/> instance can contain more complex validation errors if necessary.
    /// </summary>
    /// <param name="invalidModelName">A name describing the model that ws invalidated.</param>
    /// <param name="friendlyMessage">A message that can be shown to the user. Although this should always be populated,
    /// use the <paramref name="userExceptionMessage"/> argument to indicate which message is best to display to the user.</param>
    /// <param name="validationResult">An object available if more complex validation error info should be returned.</param>
    /// <param name="userExceptionMessage">Indicates if the exception message <paramref name="friendlyMessage"/> should
    /// be used instead of the <see cref="EADA.Core.Domain.Messages.ValidationResult.Errors"/>property.</param>
    public DataValidationError(string invalidModelName,
        string friendlyMessage,
        ValidationResult validationResult,
        bool userExceptionMessage) : base(friendlyMessage)
    {
        ValidationResult = validationResult;
        UserExceptionMessage = userExceptionMessage;
        InvalidModelName = invalidModelName;
    }

    public class DataValidationErrorJson
    {
        public bool UserExceptionMessage { get; set; }
        public string InvalidModelName { get; set; }
        public string Message { get; set; }
        public Dictionary<string, string[]>ValidationErrors { get; set; }
    }
    public string ToJson()
    {
        var data = new DataValidationErrorJson()
        {
            UserExceptionMessage = UserExceptionMessage,
            InvalidModelName = InvalidModelName,
            Message = Message,
            ValidationErrors = ValidationResult
        };

        var newValidationErrors = new Dictionary<string, string[]>();

        foreach (var error in data.ValidationErrors)
        {
            var (key, value) = error;

            if (key.Contains("."))
            {
                var splitKey = error.Key.Split(".");
                var newKey = new List<string>() { splitKey[0] };
                for (int i = 1; i < splitKey.Length; i++)
                {
                    var part = splitKey[i];
                    newKey.Add(part.Substring(0,1).ToLower() + part.Substring(1, part.Length -1));
                }

                key = string.Join(".", newKey);
            }

            newValidationErrors.Add(key,value);
        }

        data.ValidationErrors = newValidationErrors;

        return JsonConvert.SerializeObject(data, new JsonSerializerSettings()
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver()
        });
    }
}