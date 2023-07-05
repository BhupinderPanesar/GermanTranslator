using CommandLine;
using GermanTranslator.Cli.Services;

namespace GermanTranslator.Cli;

public class ConsoleWriter
{
    private readonly ITranslationService _translationService;

    public ConsoleWriter(ITranslationService translationService)
    {
        _translationService = translationService;
    }
    
    public async Task RunAsync(string[] args)
    {
        await Parser.Default
            .ParseArguments<ConsoleWriterOptions>(args)
            .WithParsedAsync(HandleOptions);
    }

    private async Task HandleOptions(ConsoleWriterOptions options)
    {
        var request = new TranslationRequest(options.Query);
        TranslationResponse response = await _translationService.TranslateAsync(request);

        if (response.HasError)
        {
           Console.WriteLine(response.ErrorMessage);
           return;
        }

        int count = 1;
        response.Translations.ForEach(translation => Console.WriteLine($"Translation {count++}: {translation.TranslatedText}"));
    }
    
    
}