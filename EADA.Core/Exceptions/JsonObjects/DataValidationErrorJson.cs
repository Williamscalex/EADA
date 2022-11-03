namespace EADA.Core.Exceptions.JsonObjects;

public class DataValidationErrorJson
{
    public bool UserExceptionMessage { get; set; }
    public string InvalidModelName { get; set; }
    public string Message { get; set; }
    public Dictionary<string, string[]>ValidationErrors { get; set; }
}