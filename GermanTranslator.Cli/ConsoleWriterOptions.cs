using CommandLine;
using JetBrains.Annotations;

namespace GermanTranslator.Cli;

[UsedImplicitly(ImplicitUseTargetFlags.Members)]
public class ConsoleWriterOptions
{
    [Option('q',"query", Required = true, HelpText = "Provides the word or sentence to translate")]
    public string Query { get; init; }
    
    [Option('t',"translateTo", Required = true, HelpText = "Provides target to translate to, 'g' for german or 'e' for english")]
    public string TranslateTo { get; init; }
}