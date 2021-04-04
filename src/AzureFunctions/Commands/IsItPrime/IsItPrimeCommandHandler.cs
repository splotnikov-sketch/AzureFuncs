using System;
using System.Diagnostics;
using System.Threading.Tasks;
using AzureFunctions.Commands.Cosmos.Add;
using AzureFunctions.Commands.Cosmos.Entities;
using AzureFunctions.Commands.Cosmos.Get;
using AzureFunctions.Infrastructure.Commands;

namespace AzureFunctions.Commands.IsItPrime
{
    public class IsItPrimeCommandHandler : ICommandHandler<IsItPrimeCommand, PrimeResult>
    {
        private readonly ICommandHandler<GetNumberCommand, NumberEntity> _getFromCache;
        private readonly ICommandHandler<AddNumberCommand, NumberEntity> _addToCache;

        public IsItPrimeCommandHandler(ICommandHandler<GetNumberCommand, NumberEntity> getFromCache, 
                                       ICommandHandler<AddNumberCommand, NumberEntity> addToCache)
        {
            _getFromCache = getFromCache;
            _addToCache = addToCache;
        }


        public async Task<PrimeResult> Execute(IsItPrimeCommand command)
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();

            var (isSimple, isPrime) = IsSimple(command.Number);
            if (isSimple)
            {
                stopwatch.Stop();
                return new PrimeResult
                       {
                           Number = command.Number,
                           IsPrime = isPrime.Value,
                           IsCached = false,
                           ExecutionTime = GetTime(stopwatch.Elapsed)
                       };
            }
            
            var cached = await _getFromCache.Execute(new GetNumberCommand
                                                    {
                                                        Number = command.Number
                                                    });
            if (cached != null)
            {
                stopwatch.Stop();
                return new PrimeResult
                       {
                           Number = command.Number,
                           IsPrime = cached.IsPrime,
                           IsCached = true,
                           ExecutionTime = GetTime(stopwatch.Elapsed)
                       };
            }

            var calculated = GetCalculated(command.Number);
            
            stopwatch.Stop();

            var result = new PrimeResult
                         {
                             Number = command.Number,
                             IsPrime = calculated,
                             IsCached = false,
                             ExecutionTime = GetTime(stopwatch.Elapsed)
                         };

            await _addToCache.Execute(new AddNumberCommand
                                      {
                                          Number = result.Number,
                                          IsPrime = result.IsPrime,
                                          CalculationTime = result.ExecutionTime
                                      });

            return result;
        }


        private (bool isSimple, bool? isPrime) IsSimple(long number)
        {
            if (number < 2)
            {
                return (true, false);
            }

            if (number == 2)
            {
                return (true, true);
            }

            return (false, null);
        }

        private bool GetCalculated(long number)
        {
            for (var i = number / 2; i >= 2; i -= 1)
            {
                if (number % i == 0)
                {
                    return false;
                }
            }

            return true;
        }


        private string GetTime(TimeSpan timeSpan)
        {
            return timeSpan.ToString(@"mm\:ss");
        }
    }
}