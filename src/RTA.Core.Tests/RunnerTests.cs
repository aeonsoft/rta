using RTA.Core.WebDriver;
using RTA.Core.WebDriver.Commands;

namespace RTA.Core.Tests;

public class RunnerTests
{
    const string SauceDemoUrl = "https://www.saucedemo.com/";
    private readonly Settings _settings = new Settings { Port = 9515 };    

    [Fact]
    public async Task StartSession_WithInValidPort_ThrowsHttpRequestException()
    {
        //arrange
        var runner = new Runner(new Settings(){Port = 100});
        
        Task<string?> StartSessionOnInvalidPort() => runner.StartSession(); 

        //act and assert
        await Assert.ThrowsAsync<HttpRequestException>(StartSessionOnInvalidPort);
    }
    

    [Fact]
    public async Task StartSession_WithValidConfig_SetsInSession()
    {
        //arrange
        var runner = new Runner(_settings);

        //act
        var sessionId = await runner.StartSession();
        
        //assert
        Assert.True(runner.InSession);
        Assert.Equal(sessionId, runner.SessionId);
        
        //cleanup
        await runner.CloseSession();
        Assert.False(runner.InSession);
    }

    [Fact]
    public async Task NavigateTo_RealWebSite_OpensRightPage()
    {
        //arrange
        var runner = new Runner(_settings);
        var expectedUrl = SauceDemoUrl;
        
        //act
        await runner.StartSession();
        await runner.NavigateTo(expectedUrl);
        var currentUtl = await runner.GetCurrentUrl();
        await runner.CloseSession();
        
        //assert
        Assert.Equal(expectedUrl, currentUtl);
    }

    [Fact]
    public async Task TakeScreenshot_OnValidPage_ReturnsPageImage()
    {
        string? image;
        var imagePath = Path.Combine(Directory.GetCurrentDirectory(),
            "TakeScreenshot_OnValidPage_ReturnsPageImage.png");
            
        using (var runner = new Runner(_settings))
        {
            await runner.StartSession();
            await runner.NavigateTo(SauceDemoUrl);
            image = await runner.ScreenShot();
            await runner.CloseSession();
        }

        Assert.NotNull(image);
        Assert.True(image.Length > 100);
        
        await File.WriteAllBytesAsync(imagePath, Convert.FromBase64String(image));
        Assert.True(File.Exists(imagePath));
        
        File.Delete(imagePath);
    }
    
    [Fact]
    public async Task TakeElementScreenshot_OnValidElement_ReturnsPageImage()
    {
        string? image;
        var imagePath = Path.Combine(Directory.GetCurrentDirectory(),
            "TakeElementScreenshot_OnValidElement_ReturnsPageImage.png");
            
        using (var runner = new Runner(_settings))
        {
            await runner.StartSession();
            await runner.NavigateTo(SauceDemoUrl);
            image = await runner.ElementScreenShot("#login_button_container");
            await runner.CloseSession();
        }

        Assert.NotNull(image);
        Assert.True(image.Length > 100);
        await File.WriteAllBytesAsync(imagePath, Convert.FromBase64String(image));
        Assert.True(File.Exists(imagePath));
        
        File.Delete(imagePath);
    }
    

    [Fact]
    public async Task ClickOnLogin_WithInvalidUserName_ShouldDisplayError()
    {
        //arrange
        const string userName = "non_existing_user_name";
        const string password = "secret_sauce";
        const string exceptedMessage = "Epic sadface: Username and password do not match any user in this service";
        const string errorMessageSelector = "h3[data-test='error']";
        const string userNameSelector = "#user-name";
        const string passwordSelector = "#password";
        const string loginButtonSelector = "#login-button";
        string? foundMessage;

        //act
        using (var runner = new Runner(_settings))
        {
            await runner.StartSession();
            await runner.NavigateTo(SauceDemoUrl);
            await runner.SendKeys(userNameSelector, userName);
            await runner.SendKeys(passwordSelector, password);
            await runner.Click(loginButtonSelector);
            foundMessage = await runner.GetText(errorMessageSelector);
            await runner.CloseSession();
        }

        // assert
        Assert.NotNull(foundMessage);
        Assert.Equal(exceptedMessage, foundMessage);
    }
}