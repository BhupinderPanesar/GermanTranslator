using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using GermanTranslator.Cli.ExternalApi;
using GermanTranslator.Cli.Services;
using Newtonsoft.Json;
using NSubstitute;
using Refit;
using Xunit;
using ValidationResult = FluentValidation.Results.ValidationResult;

namespace GermanTranslator.Cli.Tests;

public class TranslationServiceTests
{
    private readonly IGoogleTranslationApi _googleTranslationApi = Substitute.For<IGoogleTranslationApi>();
    private readonly IValidator<TranslationRequest> _validator = Substitute.For<IValidator<TranslationRequest>>();

    private readonly TranslationService _translationService;
    public TranslationServiceTests()
    {
        _translationService = new TranslationService(_validator,_googleTranslationApi);
    }
    
    [Fact]
    public async Task ReturnErrorResponse_When_QueryIsEmpty()
    {
        const string validationErrorMessage = "Cannot be empty.";
        var request = new TranslationRequest("");

        var validationResult = new ValidationResult(new List<ValidationFailure>
        {
            new ValidationFailure("Query", validationErrorMessage)
        });
        
        _validator.ValidateAsync(Arg.Any<TranslationRequest>()).Returns(validationResult);
        
        TranslationResponse response = await _translationService.TranslateAsync(request);

        response.HasError.Should().BeTrue();
        response.ErrorMessage.Should().Be(validationErrorMessage);
    }
    
    [Fact]
    public async Task ReturnsMultipleErrorsInASentence()
    {
        const string validationErrorMessage = "Cannot be empty.";
        const string validationErrorMessage2 = "Sentence Cannot contain Numeric value.";
        var request = new TranslationRequest("");

        var validationResult = new ValidationResult(new List<ValidationFailure>
        {
            new ValidationFailure("Query", validationErrorMessage),
            new ValidationFailure("Query", validationErrorMessage2)
        });
        
        _validator.ValidateAsync(Arg.Any<TranslationRequest>()).Returns(validationResult);
        
        TranslationResponse response = await _translationService.TranslateAsync(request);
        
        response.HasError.Should().BeTrue();
        response.ErrorMessage.Should().Be($"{validationErrorMessage} {validationErrorMessage2}");
    }
    
    [Fact]
    public async Task ShouldConvertSingleNumericValueToWord()
    {
        var request = new TranslationRequest("2");

        var googleResponse = JsonConvert.DeserializeObject<GoogleTranslationResponse>("{\"data\":{translations:[{\"translatedText\":\"zwei\"}]}}");
        var httpResponseMessage = new HttpResponseMessage(HttpStatusCode.OK);
        
        CreateApiSuccessResponse(httpResponseMessage, googleResponse);

        TranslationResponse response = await _translationService.TranslateAsync(request);
        
        response.HasError.Should().BeFalse();
        response.ErrorMessage.Should().BeEmpty();
        response.Translations.FirstOrDefault()?.TranslatedText.Should()
            .Be(googleResponse?.Data?.Translations.FirstOrDefault()?.TranslatedText);
    }
    
    [Fact]
    public async Task ReturnsSuccessResponseWithTranslation()
    {
        var request = new TranslationRequest("I like German");

        var googleResponse = JsonConvert.DeserializeObject<GoogleTranslationResponse>("{\"data\":{translations:[{\"translatedText\":\"Ich mag Deutsch\"}]}}");
        var httpResponseMessage = new HttpResponseMessage(HttpStatusCode.OK);
        
        CreateApiSuccessResponse(httpResponseMessage, googleResponse);

        TranslationResponse response = await _translationService.TranslateAsync(request);
        
        response.HasError.Should().BeFalse();
        response.ErrorMessage.Should().BeEmpty();
        response.Translations.FirstOrDefault()?.TranslatedText.Should()
            .Be(googleResponse?.Data?.Translations.FirstOrDefault()?.TranslatedText);
    }
    
    [Fact]
    public async Task ReturnsErrorResponse()
    {
        var request = new TranslationRequest("I like German");

        var googleResponse = JsonConvert.DeserializeObject<GoogleTranslationResponse>("{\"data\":{translations:[{\"translatedText\":\"Ich mag Deutsch\"}]}}");
        var httpResponseMessage = new HttpResponseMessage(HttpStatusCode.Forbidden);
        CreateApiErrorResponse(httpResponseMessage, googleResponse, CreateApiException(HttpStatusCode.Forbidden, "Forbidden Test"));

        TranslationResponse response = await _translationService.TranslateAsync(request);
        
        response.HasError.Should().BeTrue();
        response.ErrorMessage.Should().NotBeNull();
    }

    private void CreateApiSuccessResponse(HttpResponseMessage httpResponseMessage, GoogleTranslationResponse googleResponse)
    {
        RefitSettings refitSettings = RefitSettings();
        _googleTranslationApi.GetTranslationAsync(Arg.Any<HttpContent>()).Returns(
            new ApiResponse<GoogleTranslationResponse>(httpResponseMessage, googleResponse, refitSettings));
    }
    
    private void CreateApiErrorResponse(HttpResponseMessage httpResponseMessage, GoogleTranslationResponse googleResponse, ApiException exception)
    {
        RefitSettings refitSettings = RefitSettings();
        _googleTranslationApi.GetTranslationAsync(Arg.Any<HttpContent>()).Returns(
            new ApiResponse<GoogleTranslationResponse>(httpResponseMessage, googleResponse, refitSettings, exception));
    }

    private RefitSettings RefitSettings()
    {
        var refitSettings = new RefitSettings
        {
            ContentSerializer = new NewtonsoftJsonContentSerializer()
        };
        var validationResult = new ValidationResult(new List<ValidationFailure>());
        _validator.ValidateAsync(Arg.Any<TranslationRequest>()).Returns(validationResult);
        return refitSettings;
    }

    private static ApiException CreateApiException(HttpStatusCode statusCode, string content)
    {
        var refitSettings = new RefitSettings();
        return ApiException.Create(null!, null!, 
            new HttpResponseMessage
            {
                StatusCode = statusCode,
                Content = refitSettings.ContentSerializer.ToHttpContent(content)
            }, refitSettings).Result;
    }
}