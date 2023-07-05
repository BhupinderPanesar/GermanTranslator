using GermanTranslator.Cli.Configuration;
using GermanTranslator.Cli.ExternalApi;
using Microsoft.Extensions.DependencyInjection;
using Refit;

namespace GermanTranslator.Cli;

public static class Bootstrapper
{
    public static IServiceCollection RegisterAndSetupRefitClient(this IServiceCollection services, ApiConfiguration apiConfiguration)
    {
        IHttpClientBuilder service = services.AddRefitClient<IGoogleTranslationApi>(_ => new RefitSettings
                                                                                         {
                                                                                             ContentSerializer = new NewtonsoftJsonContentSerializer()
                                                                                         })
                                             .ConfigureHttpClient(client =>
                                                                  {
                                                                      client.BaseAddress = new Uri(apiConfiguration.Google.BaseAddress);
                                                                      client.DefaultRequestHeaders.Add(apiConfiguration.Google.ApiHostName, apiConfiguration.Google.ApiHostValue);
                                                                      client.DefaultRequestHeaders.Add(apiConfiguration.Google.ApiKeyName, apiConfiguration.Google.ApiKeyValue);
                                                                  });

        return service.Services;
    }

    public static async Task ExecuteConsoleWriterAsync(this IServiceCollection services, string[] args)
    {
        ServiceProvider serviceProvider = services.BuildServiceProvider();
        var consoleWriter = serviceProvider.GetRequiredService<ConsoleWriter>();

        if (args.Length == 0)
        {
            args = new string[2];
            args[0] = "-q";
            args[1] = "43";
        }

        await consoleWriter.RunAsync(args);
    }
}