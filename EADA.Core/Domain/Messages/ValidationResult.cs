using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json;
using fluentValidationresult = FluentValidation.Results.ValidationResult;
namespace EADA.Core.Domain.Messages;

public class ValidationResult
{
    public ValidationResult(bool isValid = false)
    {
        IsValid = isValid;
    }

    public ValidationResult(fluentValidationresult fluentValidationResult)
    {
        var vr = fluentValidationResult;
        if (vr.IsValid)
        {
            IsValid = IsValid;
            return;
        }

        var errors = vr.Errors
            .GroupBy(x => x.PropertyName,
                x => x, (key, failures) => new
                {
                    Key = key,
                    Failures = failures
                        .Where(x => x.PropertyName == key)
                        .Select(x => x.ErrorMessage)
                        .ToArray()
                });

        foreach (var e in errors)
        {
            AddErrors(e.Key, e.Failures);
        }
    }

    public ValidationResult(params (string name, string[] messages)[] errors)
    {
        IsValid = false;
        foreach (var (name, messages) in errors)
        {
            AddErrors(name,messages);
        }
    }

    public void AddErrors<T>(T keyFlag, params string[] errors) where T : Enum =>
        AddErrors(Enum.GetName(typeof(T), keyFlag), errors);

    public void AddErrors(string key, params string[] errors)
    {
        IsValid = false;
        if(!Errors.ContainsKey(key)) Errors.Add(key, new HashSet<string>());
        if (!errors.Any() && !Errors[key].Any())
        {
            ((HashSet<string>)Errors[key]).Add("Invalid");
            return;
        }

        foreach (var error in errors)
        {
            ((HashSet<string>)Errors[key]).Add(error);
        }
    }

    public bool IsValid { get; set; }
    [JsonIgnore]
    public bool IsSuccessful { get; set; } = true;
    public Dictionary<string, IEnumerable<string>> Errors { get; private set; } =
        new Dictionary<string, IEnumerable<string>>();
    [JsonIgnore]
    public Exception Exception { get; set; } = null;

    public void WithException(Exception e)
    {
        IsSuccessful = false;
        IsValid = false;
        Exception = e;
    }
    public static implicit operator Dictionary<string, string[]>(ValidationResult result) =>
        result.Errors.ToDictionary(k => k.Key, k => k.Value.ToArray());
}