using System.Threading.Tasks;
using AzureFunctions.Configuration.Options;
using AzureFunctions.Infrastructure.Commands;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Options;

namespace AzureFunctions.Commands.Cosmos
{
    public class InitCosmosCommand : ICommand<bool>
    {
    }

    public class InitCosmosCommandHandler : ICommandHandler<InitCosmosCommand, bool>
    {
        private readonly CosmosOptions _cosmosOptions;

        public InitCosmosCommandHandler(IOptions<CosmosOptions> cosmosOptions)
        {
            _cosmosOptions = cosmosOptions.Value;
        }

        public async Task<bool> Execute(InitCosmosCommand command)
        {
            var cosmosClient =
                new CosmosClient(_cosmosOptions.EndpointUri, _cosmosOptions.PrimaryKey,
                    new CosmosClientOptions
                    {
                        ApplicationName = "AzureFunctions"
                    });

            Database database = await cosmosClient.CreateDatabaseIfNotExistsAsync(_cosmosOptions.DatabaseId, 400);


            Container container =
                await database.CreateContainerIfNotExistsAsync(_cosmosOptions.ContainerId, "/number");


            return await Task.FromResult(true);
        }
    }
}
