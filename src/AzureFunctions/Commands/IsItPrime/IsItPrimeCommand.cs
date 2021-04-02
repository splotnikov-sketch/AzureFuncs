using System.Threading.Tasks;
using AzureFunctions.Infrastructure.Commands;

namespace AzureFunctions.Commands.IsItPrime
{
    public class IsItPrimeCommand : ICommand<bool>
    {
        public int Number { get; set; }
    }

    public class IsItPrimeCommandHandler : ICommandHandler<IsItPrimeCommand, bool>
    {
        public async Task<bool> Execute(IsItPrimeCommand command)
        {
            if (command.Number < 2)
            {
                return await Task.FromResult(false);
            }

            if (command.Number == 2)
            {
                return await Task.FromResult(true);
            }

            for (var i = (command.Number / 2); i >= 2; i-=1)
            {
                if (command.Number % i == 0)
                {
                    await Task.FromResult(false);
                }
            }

            return await Task.FromResult(true);
        }
    }
}
