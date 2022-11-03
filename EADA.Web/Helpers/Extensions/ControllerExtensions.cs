using EADA.Core.Constants;
using EADA.Core.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Rbit.Exceptions;

namespace EADA.Web.Helpers.Extensions;

public static class ControllerExtensions
{
    private static class ContentTypes
    {
        public const string ApplicationJson = "application/json";
        public const string TextHtml = "text/html";
    }

    public static IActionResult handledError(this ControllerBase controller, Exception ex,
        string defaultMessage = "An unexpected error occurred.")
    {
        try
        {
            var logger = controller.HttpContext.RequestServices.GetRequiredService<ILogger<ControllerBase>>();
            logger.LogError(ex,defaultMessage);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }

        switch (ex)
        {
            case DataValidationError de:
                return new DataValidationErrorResult(de);
            case HandledException hex:
                return new SimpleErrorMessageResult(hex);
            default:
                return new UnhandledErrorResult(defaultMessage);
        }
    }

    public class UnhandledErrorResult : ContentResult
    {
        public UnhandledErrorResult(string error)
        {
            StatusCode = ResponseStatusCode;
            ContentType = ContentTypes.TextHtml;
            Content = error;
        }

        public static Type Response = typeof(string);
        public static int ResponseStatusCode = CustomStatusCodes.UnhandedError;
    }

    public class SimpleErrorMessageResult : ContentResult, IStatusCodeActionResult
    {
        public SimpleErrorMessageResult(string error)
        {
            StatusCode = CustomStatusCodes.SimpleErrorMessage;
            ContentType = ContentTypes.TextHtml;
            Content = error;
        }

        public SimpleErrorMessageResult(HandledException hex)
        {
            StatusCode = ResponseStatusCode;
            ContentType = ContentTypes.TextHtml;
            Content = hex.Message;
        }

        public static Type Response = typeof(string);
        public static int ResponseStatusCode = CustomStatusCodes.SimpleErrorMessage;
    }

    public class DataValidationErrorResult : ContentResult, IStatusCodeActionResult
    {
        public DataValidationErrorResult(DataValidationError e)
        {
            StatusCode = ResponseStatusCode;
            ContentType = ContentTypes.ApplicationJson;
            Content = e.ToJson();
        }

        public static Type Response = typeof(DataValidationError.DataValidationErrorJson);
        public static int ResponseStatusCode = CustomStatusCodes.ModelValidationError;
    }
}