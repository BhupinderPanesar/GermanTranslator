namespace GermanTranslator.Cli.Services;

public class TranslationRequest
{
    public TranslationRequest(string query)
    {
        Query = query;
    }
    public string Query { get; set; }
}