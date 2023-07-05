using FluentValidation;
using GermanTranslator.Cli.Services;

namespace GermanTranslator.Cli.Validators;

public class QueryValidation : AbstractValidator<TranslationRequest>
{
    public QueryValidation()
    {
        RuleFor(request => request.Query)
            .NotEmpty()
            .WithMessage($"Cannot be empty.")
            .Matches(@"^[\sA-Za-z]+$|^[\s0-9]+$")
            .WithMessage("Provide either letters or numeric characters. " +
                         "If letters, can be a word or a sentence with surrounding double quotes. " +
                         "The sentence cannot contain numeric characters (e.g. 12), only as a word is acceptable e.g. twelve.");
    }
}