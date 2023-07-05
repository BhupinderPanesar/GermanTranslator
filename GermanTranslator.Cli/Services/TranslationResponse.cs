using GermanTranslator.Cli.ExternalApi;

namespace GermanTranslator.Cli.Services;

public class TranslationResponse
{
    private TranslationResponse(List<Translation> translations)
    {
        Translations = translations;
        ErrorMessage = string.Empty;
    }

    private TranslationResponse(string errorMessage)
    {
        ErrorMessage = errorMessage;
        Translations = new List<Translation>();
    }
    
    public List<Translation> Translations { get; }
    public string ErrorMessage { get; }
    public bool HasError => !string.IsNullOrWhiteSpace(ErrorMessage);

    public static class Factory
    {
        public static TranslationResponse SuccessResponse(List<Translation> translations)
        {
            return new TranslationResponse(translations);
        }

        public static TranslationResponse ErrorResponse(string errorMessage)
        {
            return new TranslationResponse(errorMessage);
        }
    }
}