using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using AzureFunctions.Commands.IsItPrime;
using AzureFunctions.Configuration;
using AzureFunctions.Infrastructure.Commands;
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
    public class ActionFunctions
    {
        private readonly ConfigurationItems _configurationItems;
        private readonly ICommandHandler<IsItPrimeCommand, bool> _isItPrime;

        public ActionFunctions(IOptions<ConfigurationItems> configurationItems, 
            ICommandHandler<IsItPrimeCommand, bool> isItPrime)
        {
            _isItPrime = isItPrime;
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
                new OkObjectResult($"Local Value : '{localSettings}' | Common Value : '{commonValue}' | Secret Value : '{secretValue}'");
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


        [FunctionName(nameof(IsItPrime))]
        [OpenApiOperation(operationId: nameof(IsItPrime), tags: new[] { "math" })]
        [OpenApiSecurity("function_key", SecuritySchemeType.ApiKey, Name = "code", In = OpenApiSecurityLocationType.Query)]
        [OpenApiParameter(name: "number", In = ParameterLocation.Query, Required = true, Type = typeof(int), Description = "The **Number** parameter")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(int), Description = "The OK response")]
        public async Task<IActionResult> IsItPrime([HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req, 
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request to calculate if passing number is a prime.");

            string numberStr = req.Query["number"];

            var requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            numberStr = !string.IsNullOrWhiteSpace(numberStr) ? numberStr : data?.number;

            if (string.IsNullOrEmpty(numberStr))
            {
                return new BadRequestObjectResult("Pass a number in the query string or in the request body for calculation.");
            }

            if (!int.TryParse(numberStr, out var number))
            {
                return new BadRequestObjectResult("Pass a number in the query string or in the request body for calculation.");
            }

            var isItPrime = await _isItPrime.Execute(new IsItPrimeCommand
            {
                Number = number
            });

            
            return 
                new OkObjectResult(new
                {
                    Number = number,
                    IsPrime = isItPrime,
                    IsCached = false
                });
        }
    }
}

