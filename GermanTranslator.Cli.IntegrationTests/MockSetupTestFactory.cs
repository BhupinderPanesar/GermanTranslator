using System;
using System.Threading.Tasks;
using FluentValidation;
using GermanTranslator.Cli.Configuration;
using GermanTranslator.Cli.ExternalApi;
using GermanTranslator.Cli.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Refit;

namespace GermanTranslator.Cli.IntegrationTests;

public sealed class MockSetupTestFactory
{
    private readonly IServiceCollection _services;
    public MockSetupTestFactory()
    {
        IConfigurationRoot configurationBuilder = new ConfigurationBuilder()
                                                  .AddJsonFile("appsettings.json")
                                                  .Build();
        
        var apiConfiguration = configurationBuilder.GetSection(ApiConfiguration.SectionName).Get<ApiConfiguration>();
        
        _services = new ServiceCollection();

        RegisterAndSetupRefitClient(apiConfiguration,_services);
        _services.AddTransient<ITranslationService, TranslationService>();
        _services.AddTransient<ConsoleWriter>();
        _services.AddValidatorsFromAssemblyContaining<ConsoleWriter>();
    }
    
    private static void RegisterAndSetupRefitClient(ApiConfiguration apiConfiguration, IServiceCollection services)
    {
        services.AddRefitClient<IGoogleTranslationApi>(_ => new RefitSettings
                                                            {
                                                                ContentSerializer = new NewtonsoftJsonContentSerializer()
                                                            })
                .ConfigureHttpClient(client =>
                                     {
                                         client.BaseAddress = new Uri(apiConfiguration.Google.BaseAddress);
                                         client.DefaultRequestHeaders.Add(apiConfiguration.Google.ApiHostName, apiConfiguration.Google.ApiHostValue);
                                         client.DefaultRequestHeaders.Add(apiConfiguration.Google.ApiKeyName, apiConfiguration.Google.ApiKeyValue);
                                     });
    }

    public async Task ExecuteAsync(string[] args)
    {
        await _services.ExecuteConsoleWriterAsync(args);
    }
}