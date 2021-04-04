using System;
using System.Threading.Tasks;
using AzureFunctions.Commands.Cosmos.Entities;
using AzureFunctions.Commands.Cosmos.Get;
using AzureFunctions.Configuration.Options;
using AzureFunctions.Infrastructure.Commands;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Options;

namespace AzureFunctions.Commands.Cosmos.Add
{
    public class AddNumberCommandHandler : BaseCosmosCommandHandler<AddNumberCommand, NumberEntity>
    {
        private readonly ICommandHandler<GetNumberCommand, NumberEntity> _get;

        public AddNumberCommandHandler(IOptions<CosmosOptions> cosmosOptions, 
                                       ICommandHandler<GetNumberCommand, NumberEntity> get): base(cosmosOptions)
        {
            _get = get;
        }

        public override async Task<NumberEntity> Execute(AddNumberCommand command)
        {
            var existingEntity = await _get.Execute(new GetNumberCommand
                                              {
                                                  Number = command.Number
                                              });

            if (existingEntity != null)
            {
                return existingEntity;
            }

            var id = command.Number.ToString();

            var newEntity = new NumberEntity
                            {
                                Id = id,
                                Number = command.Number,
                                IsPrime = command.IsPrime,
                                CalculationTime = command.CalculationTime,
                                CreatedDate = DateTime.Now
                            };

            var result = 
                await Container.CreateItemAsync<NumberEntity>(newEntity, new PartitionKey(command.Number));

            return 
                result.Resource;
        }
    }
}