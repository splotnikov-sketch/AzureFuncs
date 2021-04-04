using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using AzureFunctions.Configuration.Options;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Enums;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;

namespace AzureFunctions.Functions
{
    public class TestFunctions
    {
        private readonly ConfigurationItems _configurationItems;

        public TestFunctions(IOptions<ConfigurationItems> configurationItems)
        {
            _configurationItems = configurationItems.Value;
        }

        [FunctionName(nameof(Configuration))]
        [OpenApiOperation(operationId: nameof(Configuration), tags: new[] { "example" })]
        [OpenApiSecurity("function_key", SecuritySchemeType.ApiKey, Name = "code", In = OpenApiSecurityLocationType.Query)]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "text/plain", bodyType: typeof(string), Description = "The OK response")]
        public async Task<IActionResult> Configuration([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = "example/config")] HttpRequest req,
         ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request to get configuration.");

            var localSettings = Environment.GetEnvironmentVariable("LocalSettingValue");
            var commonValue = _configurationItems.CommonValue;
            var secretValue = _configurationItems.SecretValue;

            return
                new OkObjectResult($"Local Value : '{localSettings}' | Global Value : '{commonValue}' | Secret Value : '{secretValue}'");
        }


        [FunctionName(nameof(SayHello))]
        [OpenApiOperation(operationId: nameof(SayHello), tags: new[] { "example" })]
        [OpenApiSecurity("function_key", SecuritySchemeType.ApiKey, Name = "code", In = OpenApiSecurityLocationType.Query)]
        [OpenApiParameter(name: "name", In = ParameterLocation.Query, Required = true, Type = typeof(string), Description = "The **Name** parameter")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "text/plain", bodyType: typeof(string), Description = "The OK response")]

        public async Task<IActionResult> SayHello([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = "example/hello")] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            string name = req.Query["name"];

            var requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            name ??= data?.name;

            var responseMessage = string.IsNullOrEmpty(name)
                ? "This HTTP triggered function executed successfully. Pass a name in the query string or in the request body for a personalized response."
                : $"Hello, {name}. This HTTP triggered function executed successfully.";

            return
                new OkObjectResult(responseMessage);
        }
    }
}
