using System.Threading.Tasks;
using AzureFunctions.Configuration.Options;
using AzureFunctions.Constants;
using AzureFunctions.Infrastructure.Commands;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Options;

namespace AzureFunctions.Commands.Cosmos
{
    public abstract class BaseCosmosCommandHandler<TCommand, TResult>: ICommandHandler<TCommand, TResult>
        where TCommand: ICommand<TResult>
    {
        public readonly CosmosOptions CosmosOptions;
        public CosmosClient CosmosClient { private set; get; }
        public Database Database => CosmosClient?.GetDatabase(CosmosOptions.DatabaseId);
        public Container Container => Database?.GetContainer(CosmosOptions.ContainerId);

        protected BaseCosmosCommandHandler(IOptions<CosmosOptions> cosmosOptions)
        {
            CosmosOptions = cosmosOptions.Value;
            CosmosClient =
                new CosmosClient(CosmosOptions.EndpointUri, CosmosOptions.PrimaryKey,
                                 new CosmosClientOptions
                                 {
                                     ApplicationName = Global.ApplicationName
                                 });
        }

        public abstract Task<TResult> Execute(TCommand command);
    }
}