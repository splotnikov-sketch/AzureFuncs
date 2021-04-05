using System;
using System.IO;
using System.Threading.Tasks;
using AzureFunctions.Configuration.Options;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace AzureFunctions.Functions
{
    public class TestFunctions
    {
        private readonly ConfigurationItems _configurationItems;
        private readonly CosmosOptions _cosmos;

        public TestFunctions(IOptions<ConfigurationItems> configurationItems,
                             IOptions<CosmosOptions> cosmos)
        {
            _cosmos = cosmos.Value;
            _configurationItems = configurationItems.Value;
        }

        [FunctionName(nameof(Configuration))]
        public async Task<IActionResult> Configuration([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = "example/config")] HttpRequest req,
         ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request to get configuration.");

            var localSettings = Environment.GetEnvironmentVariable("LocalSettingValue");
            var commonValue = _configurationItems.CommonValue;
            var secretValue = _configurationItems.SecretValue;

            var result = new
            {
                ConfigurationItems = new
                {
                    LocalSettingValue = localSettings,
                    CommonValue = commonValue,
                    SecretValue = secretValue
                },

                Cosmos = new
                {
                    ConnectionString = _cosmos.ConnectionString,
                    DatabaseId = _cosmos.DatabaseId,
                    ContainerId = _cosmos.ContainerId
                }
            };

            return
                new OkObjectResult(result);
        }


        [FunctionName(nameof(SayHello))]
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
