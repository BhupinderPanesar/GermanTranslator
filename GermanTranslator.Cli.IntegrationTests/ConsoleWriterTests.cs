using System;
using System.IO;
using System.Threading.Tasks;
using FluentAssertions;
using Xunit;

namespace GermanTranslator.Cli.IntegrationTests;

public class ConsoleWriterTests
{
    private readonly MockSetupTestFactory _factory;
    public ConsoleWriterTests()
    {
        _factory = new MockSetupTestFactory();
    }
    
    [Fact]
    public async Task ShouldReturnTranslatedSentenceInGerman()
    {
        const string SuccessResponseTranslation = "Translation 1: Wie geht es dir\r\n";
        
        string[] args = new string[2];
        args[0] = "-q";
        args[1] = "How are you";
        
        await using var stringWriter = new StringWriter();
        Console.SetOut(stringWriter);
        await _factory.ExecuteAsync(args);
        
        string consoleOutput = stringWriter.ToString();

        consoleOutput.Should().Be(SuccessResponseTranslation);
    }
}