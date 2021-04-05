using System.IO;
using System.Threading.Tasks;
using AzureFunctions.Commands.IsItPrime;
using AzureFunctions.Infrastructure.Commands;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace AzureFunctions.Functions
{
    public class ActionFunctions
    {
        private readonly ICommandHandler<IsItPrimeCommand, PrimeResult> _isItPrime;

        public ActionFunctions(ICommandHandler<IsItPrimeCommand, PrimeResult> isItPrime)
        {
            _isItPrime = isItPrime;
        }


        [FunctionName(nameof(IsItPrime))]
        public async Task<IActionResult> IsItPrime([HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = "math/prime")] HttpRequest req, 
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

            if (!long.TryParse(numberStr, out var number))
            {
                return new BadRequestObjectResult("Pass a number in the query string or in the request body for calculation.");
            }

            var result = await _isItPrime.Execute(new IsItPrimeCommand
            {
                Number = number
            });

            return 
                new OkObjectResult(result);
        }
    }
}

