using System.Net;
using System.Threading.Tasks;
using AzureFunctions.Commands.Cosmos.Entities;
using AzureFunctions.Configuration.Options;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Options;

namespace AzureFunctions.Commands.Cosmos.Get
{
    public class GetNumberCommandHandler : BaseCosmosCommandHandler<GetNumberCommand, NumberEntity>
    {
        public GetNumberCommandHandler(IOptions<CosmosOptions> cosmosOptions) : base(cosmosOptions)
        {
        }

        public override async Task<NumberEntity> Execute(GetNumberCommand command)
        {
            try
            {
                var response =
                    await Container.ReadItemAsync<NumberEntity>(command.Number.ToString(), new PartitionKey(command.Number));

                return response.Resource;
            }
            catch (CosmosException ex) 
                when (ex.StatusCode == HttpStatusCode.NotFound)
            {
                return null;
            }
        }
    }
}