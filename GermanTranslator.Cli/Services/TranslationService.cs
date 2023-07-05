using FluentValidation;
using FluentValidation.Results;
using GermanTranslator.Cli.Constants;
using GermanTranslator.Cli.ExternalApi;
using Humanizer;
using Refit;

namespace GermanTranslator.Cli.Services;
using static TranslationResponse.Factory;
using static ApiConstants;
public class TranslationService : ITranslationService
{
    private readonly IValidator<TranslationRequest> _validator;
    private readonly IGoogleTranslationApi _googleTranslationApi;

    public TranslationService(IValidator<TranslationRequest> validator,
        IGoogleTranslationApi googleTranslationApi)
    {
        _validator = validator;
        _googleTranslationApi = googleTranslationApi;
    }
    public async Task<TranslationResponse> TranslateAsync(TranslationRequest request)
    {
        ValidationResult validationResult = await _validator.ValidateAsync(request);

        if (!validationResult.IsValid)
        {
            string errorMessage = GetErrorMessagesAsSingleString(validationResult);
            return ErrorResponse(errorMessage);
        }

        IfNumericConvertToWord(request);

        var content = new FormUrlEncodedContent(new Dictionary<string, string>
        {
            {"target", Target},
            {"source", Source},
            {"q", request.Query}
        });
        
        ApiResponse<GoogleTranslationResponse> response = await _googleTranslationApi.GetTranslationAsync(content);

        return !response.IsSuccessStatusCode ? ErrorResponse(response.Error?.Message) : SuccessResponse(response.Content?.Data?.Translations);
    }

    private static string GetErrorMessagesAsSingleString(ValidationResult validationResult)
    {
        return string.Join(' ', validationResult.Errors.Select(error => error.ErrorMessage));
    }

    private static void IfNumericConvertToWord(TranslationRequest request)
    {
        if (int.TryParse(request.Query, out int queryAsNumber))
        {
            request.Query = queryAsNumber.ToWords();
        }
    }
}