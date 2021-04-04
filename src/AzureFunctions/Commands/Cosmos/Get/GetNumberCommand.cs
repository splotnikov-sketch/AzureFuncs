using AzureFunctions.Commands.Cosmos.Entities;
using AzureFunctions.Infrastructure.Commands;

namespace AzureFunctions.Commands.Cosmos.Get
{
    public class GetNumberCommand : ICommand<NumberEntity>
    {
        public long Number { get; set; }
    }
}