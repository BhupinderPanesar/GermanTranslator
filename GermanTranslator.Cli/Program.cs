using GermanTranslator.Cli;
using GermanTranslator.Cli.Configuration;
using GermanTranslator.Cli.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using FluentValidation;

IConfigurationRoot configurationBuilder = new ConfigurationBuilder()
                                          .AddJsonFile("appsettings.json")
                                          .Build();

var apiConfiguration = configurationBuilder.GetSection(ApiConfiguration.SectionName).Get<ApiConfiguration>();

var services = new ServiceCollection();

services.RegisterAndSetupRefitClient(apiConfiguration);
services.AddTransient<ITranslationService, TranslationService>();
services.AddTransient<ConsoleWriter>();
services.AddValidatorsFromAssemblyContaining<Program>();

await services.ExecuteConsoleWriterAsync(args);