using RTA.Core.Functions;
using RTA.Core.Interpreters;
using RTA.Core.Parser;
using Xunit.Abstractions;

namespace RTA.Core.Tests;

public class InterpreterTests(ITestOutputHelper output)
{
    private const string ResourcesPath = "RTA.Core.Tests.res";
    private readonly XLogger<BasicInterpreter> _logger = new(output);

    [Fact]
    public async Task TestCaseWithInvalidMethods_ShouldFail() {
        //arrange
        var parser = new YamlParser();
        var yml = await Resources.Helper.GetFileAsync("min_valid_test.yml");        
        var interpreter = new BasicInterpreter(_logger);
        var expectedError = "Function some_function is unknown";

        //act
        var test = parser.Parse(yml);
        test.Arrange?.Add("some_method", new Arguments(){});
        var arrangeResult =  interpreter.IsValidSession(Test.Section.Arrange, test);
        var actResult =  interpreter.IsValidSession(Test.Section.Act, test);
        var assertResult = interpreter.IsValidSession(Test.Section.Assert, test);
        
        //assert
        Assert.NotNull(test);
        Assert.False(arrangeResult.result);
        Assert.False(actResult.result);
        Assert.False(assertResult.result);
                
        Assert.NotNull(arrangeResult.errors);
        Assert.Single(arrangeResult.errors);
        Assert.Equal(expectedError, arrangeResult.errors[0]);

        Assert.NotNull(actResult.errors);
        Assert.Single(actResult.errors);
        Assert.Equal(expectedError, actResult.errors[0]);

        Assert.NotNull(assertResult.errors);
        Assert.Single(assertResult.errors);
        Assert.Equal(expectedError, assertResult.errors[0]);
        
        
    }

    [Fact]
    public async Task TestCaseWithValidActMethod_ShouldBeValidated() {
        //arrange
        var parser = new YamlParser();
        var yml = await Resources.Helper.GetFileAsync("min_valid_test.yml");        
        var interpreter = new BasicInterpreter(_logger);
        var function = new Function() { Name = "some_function" };
        
        interpreter.Register(Test.Section.Act, function);

        //act
        var test = parser.Parse(yml);
        var actResult =  interpreter.IsValidSession(Test.Section.Act, test);
        
        //assert
        Assert.NotNull(test);
        Assert.True(actResult.result);
        Assert.Null(actResult.errors);                
    }

}
