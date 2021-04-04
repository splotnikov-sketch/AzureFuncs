using AzureFunctions.Commands.Cosmos.Entities;
using AzureFunctions.Infrastructure.Commands;

namespace AzureFunctions.Commands.Cosmos.Add
{
    public class AddNumberCommand : ICommand<NumberEntity>
    {
        public long Number { get; set; }
        public bool IsPrime { get; set; }
        public string CalculationTime { get; set; }
    }
}