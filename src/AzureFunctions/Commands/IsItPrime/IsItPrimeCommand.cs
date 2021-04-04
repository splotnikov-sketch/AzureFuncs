using AzureFunctions.Infrastructure.Commands;

namespace AzureFunctions.Commands.IsItPrime
{
    public class IsItPrimeCommand : ICommand<PrimeResult>
    {
        public long Number { get; set; }
    }
}
