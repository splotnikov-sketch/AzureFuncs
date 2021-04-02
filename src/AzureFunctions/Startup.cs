using System.IO;
using System.Reflection;
using AzureFunctions;
using AzureFunctions.Commands.IsItPrime;
using AzureFunctions.Configuration;
using AzureFunctions.Infrastructure.Commands;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

[assembly: FunctionsStartup(typeof(Startup))]
namespace AzureFunctions
{
    
    public class Startup: FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", false)
                .AddUserSecrets(Assembly.GetExecutingAssembly(), false)
                .AddEnvironmentVariables()
                .Build();

            builder.Services.Configure<ConfigurationItems>(config.GetSection("ConfigurationItems"));
            builder.Services.AddOptions();


            builder.Services.AddSingleton<ICommandHandler<IsItPrimeCommand, bool>, IsItPrimeCommandHandler>();
            builder.Services.AddLogging();
        }
    }
}
