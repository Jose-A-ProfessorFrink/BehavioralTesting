using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using SimpleOrderingSystem.Domain.Models;
using SimpleOrderingSystem.Domain.Extensions;

namespace SimpleOrderingSystem;

/// <summary>
/// A global exception filter that will handle all unhandled exceptions for the application.
/// (swiped from http://www.talkingdotnet.com/global-exception-handling-in-aspnet-core-webapi/)
/// </summary>
internal class ExceptionFilter : IExceptionFilter
{
    private readonly IHostEnvironment _hostEnvironment;

    /// <summary>
    /// Initializes a new instance of the <see cref="ExceptionFilter"/> class.
    /// </summary>
    /// <param name="hostEnvironment"></param>
    public ExceptionFilter(IHostEnvironment hostEnvironment)
    {
        _hostEnvironment = hostEnvironment;
    }

    /// <summary>
    /// Handler for the exception
    /// </summary>
    /// <param name="context"></param>
    public void OnException(ExceptionContext context)
    {
        var response = context.HttpContext.Response;
        context.Result = GetProblemDetailsActionResult(context.Exception);
        context.ExceptionHandled = true;
    }


    private IActionResult GetProblemDetailsActionResult(Exception exception)
    {
        var serviceException = exception as SimpleOrderingSystemException;
        var problemDetails = new ProblemDetails();

        if (serviceException != null)
        {
            problemDetails.Type = serviceException.ErrorCode.ToString();
            problemDetails.Title = serviceException.ErrorCode.GetDescription();
            problemDetails.Status = StatusCodes.Status400BadRequest;
            problemDetails.Detail = serviceException.Details;
        }
        else
        {
            problemDetails.Type = "InternalServerError";
            problemDetails.Title = "The application has experienced an unexpected error. Please contact the site administrator.";
            problemDetails.Status = StatusCodes.Status500InternalServerError;
        }

        if (_hostEnvironment.IsDevelopment())
        {
            problemDetails.Extensions.Add("stackTrace", exception.ToString());
        }

        return new ObjectResult(problemDetails)
        {
            StatusCode = problemDetails.Status,
            ContentTypes = { "application/problem+json", "application/problem+xml" }
        };
    }
}

