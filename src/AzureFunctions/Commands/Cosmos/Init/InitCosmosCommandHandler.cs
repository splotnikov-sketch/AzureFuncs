using System;
using System.Threading.Tasks;
using AzureFunctions.Configuration.Options;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Options;

namespace AzureFunctions.Commands.Cosmos.Init
{
    public class InitCosmosCommandHandler : BaseCosmosCommandHandler<InitCosmosCommand, bool>
    {
        public InitCosmosCommandHandler(IOptions<CosmosOptions> cosmosOptions): base(cosmosOptions)
        {
        }

        public override async Task<bool> Execute(InitCosmosCommand command)
        {
            try
            {
                Database database = await CosmosClient.CreateDatabaseIfNotExistsAsync(CosmosOptions.DatabaseId, 400);

                Container container =
                    await database.CreateContainerIfNotExistsAsync(CosmosOptions.ContainerId, "/Number");

                return true;
            }
            catch (Exception e)
            {
                //  _logger.LogError(e.Message);
                return false;
            }
        }
    }
}