using JetBrains.Annotations;

namespace GermanTranslator.Cli.ExternalApi;

[PublicAPI]
public class GoogleTranslationResponse
{
    public Data Data { get; init; }
}
[PublicAPI]
public class Data
{
    public Data()
    {
        Translations = new List<Translation>();
    }
    public List<Translation> Translations { get; }
}
[PublicAPI]
public class Translation
{
    public string TranslatedText { get; init; }
}