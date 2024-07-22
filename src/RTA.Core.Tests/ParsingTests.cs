using RTA.Core.Interpreters;
using RTA.Core.Parser;
using Xunit.Abstractions;

namespace RTA.Core.Tests;

public class ParsingTests(ITestOutputHelper output)
{
    private const string ResourcesPath = "RTA.Core.Tests.res";
    private readonly XLogger<BasicInterpreter> _logger = new(output);

    [Fact]
    public async Task MinimumValidYaml_ShouldBeParsed()
    {
        //arrange
        var parser = new YamlParser();
        var yml = await Resources.Helper.GetFileAsync("min_valid_test.yml");
        const string expectedTestName = "minimum valid test case";

        //act
        var test = parser.Parse(yml);
        
        //assert
        Assert.NotNull(test);
        Assert.Equal(expectedTestName, test.Name);
        Assert.Null(test.Arrange);
        Assert.NotNull(test.Act);
        Assert.NotNull(test.Assert);
    }

    
}