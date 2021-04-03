﻿using System.IO;
using System.Reflection;
using AzureFunctions;
using AzureFunctions.Commands.Cosmos;
using AzureFunctions.Commands.IsItPrime;
using AzureFunctions.Configuration.Options;
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
            builder.Services.Configure<CosmosOptions>(config.GetSection("Cosmos"));
            builder.Services.AddOptions();

            ConfigureServices(builder.Services).BuildServiceProvider(true);

            Init(builder.Services.BuildServiceProvider());
        }

        private IServiceCollection ConfigureServices(IServiceCollection services)
        {
            services
                .AddLogging()
                .AddSingleton<ICommandHandler<IsItPrimeCommand, bool>, IsItPrimeCommandHandler>()
                .AddSingleton<ICommandHandler<InitCosmosCommand, bool>, InitCosmosCommandHandler>();

            return services;
        }

        private void Init(ServiceProvider serviceProvider)
        {
            var initCosmos = serviceProvider.GetRequiredService<ICommandHandler<InitCosmosCommand, bool>>();
            
            initCosmos.Execute(new InitCosmosCommand()).GetAwaiter().GetResult();
        }
    }
}
