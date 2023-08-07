using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Net;
using SimpleOrderingSystem.Models;

namespace SimpleOrderingSystem;

/// <summary>
/// A global exception filter that will handle all unhandled exceptions for the application.
/// (swiped from http://www.talkingdotnet.com/global-exception-handling-in-aspnet-core-webapi/)
/// </summary>
internal class ExceptionFilter : IExceptionFilter
{
    /// <summary>
    /// Handler for the exception
    /// </summary>
    /// <param name="context"></param>
    public void OnException(ExceptionContext context)
    {
        var response = context.HttpContext.Response;

        //All payment exceptions can be interpreted as 400s
        //we might want to add to this later but for now this is all we need
        if (context.Exception is SimpleOrderingSystemException)
        {
            response.StatusCode = (int)HttpStatusCode.BadRequest;
            response.ContentType = "application/json";
            response.WriteAsync(SerializeJson(context.Exception.Message));

            context.ExceptionHandled = true;
        }
        else
        {
            response.StatusCode = (int)HttpStatusCode.InternalServerError;
            response.ContentType = "application/json";
            //TODO: Write error here
            response.WriteAsync(SerializeJson("An unexpected error has occurred."));
            context.ExceptionHandled = true;
        }
    }

    private string SerializeJson(object obj)
    {
        return JsonConvert.SerializeObject(obj, new JsonSerializerSettings
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver()
        });
    }
}

