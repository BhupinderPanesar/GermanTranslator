using JetBrains.Annotations;

namespace GermanTranslator.Cli.Configuration;

[UsedImplicitly(ImplicitUseTargetFlags.Members)]
public class ApiConfiguration
{
    public const string SectionName = "API";
    public Google Google { get; init; }
}

[UsedImplicitly(ImplicitUseTargetFlags.Members)]
public struct Google
{
    public string BaseAddress { get; init; }
    public string ApiHostName { get; init; }
    public string ApiHostValue { get; init; }
    public string ApiKeyName { get; init; }
    public string ApiKeyValue { get; init; }
}