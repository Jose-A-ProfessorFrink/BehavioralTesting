using System.Net;
using System.Reflection;
using FluentAssertions;
using FluentAssertions.Equivalency;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace SimpleOrderingSystem.Tests.Common;

/// <summary>
/// Extensions for <see cref="HttpResponseMessage"/>.
/// </summary>
public static class HttpResponseMessageExtensions
{
    /// <summary>
    /// Reads the content into the specified type
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="response"></param>
    /// <returns></returns>
    public static async Task<T> ReadContentAsAsync<T>(this HttpResponseMessage response)
    {
        return await response.Content.ReadAsAsync<T>();
    }

    /// <summary>
    /// Checks for equivalency
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="response"></param>
    /// <param name="expectedResult"></param>
    /// <returns></returns>
    public static Task ShouldBeOkWithResponseAsync<T>(this HttpResponseMessage response, T expectedResult)
    {
        return response.ShouldBeOkWithResponseAsync(expectedResult, a => a);
    }

    /// <summary>
    /// Checks for equivalency with config options
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="response"></param>
    /// <param name="expectedResult"></param>
    /// <param name="config"></param>
    /// <returns></returns>
    public static Task ShouldBeOkWithResponseAsync<T>(this HttpResponseMessage response, T expectedResult, 
        Func<EquivalencyAssertionOptions<T>, EquivalencyAssertionOptions<T>> config)
    {
        return response.ShouldBeWithResponseAsync(HttpStatusCode.OK, expectedResult, config);
    }

    /// <summary>
    /// Checks for equivalency
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="response"></param>
    /// <param name="expectedResult"></param>
    /// <returns></returns>
    public static Task ShouldBeCreatedWithResponseAsync<T>(this HttpResponseMessage response, T expectedResult)
    {
        return response.ShouldBeCreatedWithResponseAsync(expectedResult, a => a);
    }

    /// <summary>
    /// Checks for equivalency with config options
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="response"></param>
    /// <param name="expectedResult"></param>
    /// <param name="config"></param>
    /// <returns></returns>
    public static Task ShouldBeCreatedWithResponseAsync<T>(this HttpResponseMessage response, T expectedResult,
        Func<EquivalencyAssertionOptions<T>, EquivalencyAssertionOptions<T>> config)
    {
        return response.ShouldBeWithResponseAsync(HttpStatusCode.Created, expectedResult, config);
    }

    /// <summary>
    /// Checks for equivalency with config options
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="response"></param>
    /// <param name="statusCode"></param>
    /// <param name="expectedResult"></param>
    /// <param name="config"></param>
    /// <returns></returns>
    public static async Task ShouldBeWithResponseAsync<T>(this HttpResponseMessage response, HttpStatusCode statusCode, T expectedResult,
        Func<EquivalencyAssertionOptions<T>, EquivalencyAssertionOptions<T>> config)
    {
        var because = response.Because();

        response.StatusCode.Should().Be(statusCode, because);

        var actualResult = await response.Content.ReadAsAsync<T>();

        actualResult.Should().BeEquivalentTo(expectedResult, config, because);
    }


    public static void ShouldBeOk(this HttpResponseMessage response) 
        => response.ShouldHaveStatusCode(HttpStatusCode.OK);

    public static void ShouldBeNotFound(this HttpResponseMessage response) 
        => response.ShouldHaveStatusCode(HttpStatusCode.NotFound);

    public static void ShouldBeInternalServerError(this HttpResponseMessage response) 
        => response.ShouldHaveStatusCode(HttpStatusCode.InternalServerError);

    /// <summary>
    /// Asserts an HttpResponseMessage contains the status code.
    /// </summary>
    /// <param name="response"></param>
    /// <param name="statusCode"></param>
    public static void ShouldHaveStatusCode(this HttpResponseMessage response,
        HttpStatusCode statusCode)
    {
        response.StatusCode
            .Should()
            .Be(statusCode, response.Because());
    }

    /// <summary>
    /// Ensures a problem details response for a service exception with the provided information.
    /// </summary>
    /// <param name="httpResponseMessage"></param>
    /// <param name="code"></param>
    /// <param name="description"></param>
    /// <param name="detail"></param>
    /// <returns></returns>
    public static async Task ShouldBeTheFollowingServiceExceptionBadRequestAsync(
        this HttpResponseMessage httpResponseMessage, string code, string description, string? detail = null)
    {
        var contentString = await httpResponseMessage.Content.ReadAsStringAsync();

        httpResponseMessage.StatusCode.Should().Be(HttpStatusCode.BadRequest, httpResponseMessage.Because());

        var problemDetails = JsonConvert.DeserializeObject<ProblemDetailsDataContract>(contentString);

        problemDetails.Should().BeEquivalentTo(new ProblemDetailsDataContract
        {
            Type = code,
            Title = description,
            Detail = detail,
            Status = StatusCodes.Status400BadRequest
        });
    }

    #region ShouldBeTheFollowingServiceExceptionBadRequest Helpers

    private class ProblemDetailsDataContract
    {
        public string? Type { get; set; }

        public string? Title { get; set; }

        public int? Status { get; set; }

        public string? Detail { get; set; }
    }

    #endregion

    /// <summary>
    /// Ensures a validation problem details response for a model state validation failure with the provided information.
    /// </summary>
    /// <param name="httpResponseMessage"></param>
    /// <param name="errors"></param>
    /// <returns></returns>
    public static async Task ShouldBeTheFollowingModelStateValidationBadRequestAsync(
        this HttpResponseMessage httpResponseMessage,
        params ValidationError[] errors)
    {
        var contentString = await httpResponseMessage.Content.ReadAsStringAsync();

        httpResponseMessage.StatusCode.Should().Be(HttpStatusCode.BadRequest, httpResponseMessage.Because());

        var validationProblemDetails =
            JsonConvert.DeserializeObject<ValidationProblemDetailsDataContract>(contentString);

        validationProblemDetails.Should().BeEquivalentTo(
            new ValidationProblemDetailsDataContract
            {
                Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1",
                Title = "One or more validation errors occurred.",
                Status = StatusCodes.Status400BadRequest,
                //Detail = "Please refer to the errors property for additional details."
            },
            o => o.Excluding(x => x.Errors));

        static string ToPascalCase(string s)
        {
            if (string.IsNullOrEmpty(s) || char.IsLower(s, 0))
            {
                return s;
            }

            return char.ToLowerInvariant(s[0]) + s[1..];
        }

        validationProblemDetails?.Errors.Should()
            .BeEquivalentTo(errors.ToDictionary(a => ToPascalCase(a.Key), a => a.Values));
    }

    #region ShouldBeTheFollowingModelStateValidationBadRequest Helpers

    private class ValidationProblemDetailsDataContract
    {
        public Dictionary<string, string[]> Errors { get; set; } = new Dictionary<string, string[]>();

        public string? Type { get; set; }

        public string? Title { get; set; }

        public int? Status { get; set; }

        public string? Detail { get; set; }
    }

    #endregion

    /// <summary>
    /// Converts an <see cref="HttpResponseMessage"/> to an informative because message for use with fluentassertions.
    /// </summary>
    /// <param name="hrm"></param>
    /// <returns></returns>
    public static string Because(this HttpResponseMessage hrm)
    {
        return JsonConvert.SerializeObject(hrm, new JsonSerializerSettings
        {
            ContractResolver = new HttpContentAwareContractResolver(),
            Formatting = Formatting.Indented
        });
    }

    #region Because Helpers

    /// <summary>
    /// Ensures <see cref="HttpContent"/> members are serialized as their human readable values and not their stream values.
    /// </summary>
    private class HttpContentAwareContractResolver : DefaultContractResolver
    {
        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
        {
            var property = base.CreateProperty(member, memberSerialization);

            if (typeof(HttpContent).IsAssignableFrom(property.PropertyType))
            {
                property.Converter = new HttpContentJsonConverter();
            }

            return property;
        }

        private class HttpContentJsonConverter : JsonConverter
        {
            public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
            {
                var httpContent = (HttpContent?)value;
                var contentString = httpContent?.ReadAsStringAsync().GetAwaiter().GetResult();

                try
                {
                    serializer.Serialize(writer, JsonConvert.DeserializeObject(contentString ?? string.Empty));
                }
                catch (JsonReaderException)
                {
                    writer.WriteValue(contentString);
                }
            }

            public override object ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
            {
                throw new InvalidOperationException();
            }

            public override bool CanConvert(Type objectType)
            {
                return typeof(HttpContent).IsAssignableFrom(objectType);
            }

            public override bool CanRead => false;
        }
    }

    #endregion
}
