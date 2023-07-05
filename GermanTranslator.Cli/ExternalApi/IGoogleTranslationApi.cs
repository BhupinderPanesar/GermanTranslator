using Refit;

namespace GermanTranslator.Cli.ExternalApi;

public interface IGoogleTranslationApi
{
    [Post("")]
    Task<ApiResponse<GoogleTranslationResponse>> GetTranslationAsync([Body(BodySerializationMethod.UrlEncoded)] HttpContent content);
}