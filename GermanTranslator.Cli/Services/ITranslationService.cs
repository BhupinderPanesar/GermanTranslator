namespace GermanTranslator.Cli.Services;

public interface ITranslationService
{
    Task<TranslationResponse> TranslateAsync(TranslationRequest request);
}