namespace RTA.Core.Tests;
using RTA.Core.WebDriver;
using RTA.Core.WebDriver.Commands;
using System;
using static System.Net.WebRequestMethods;

public class RainbowPOC
{
    private readonly Settings _settings = new Settings { Port = 9515 };
    private readonly HttpClient _httpClient = new HttpClient();


    [Fact]
    public async Task LoginWithValidCredentials_DisplayHome()
    {
        const string companySelector = "#MainContentPlaceHolder_cphLeftColumn_lblUsuario";
        const string url = "https://demonstracao.rainbowtec.com/User.Login.aspx";
        const string expected = "RAINBOW TECNOLOGIA E CONSULTORIA LTDA.";

        string? found;
        using (var runner = new Runner(_settings))
        {
            await runner.StartSession();
            await runner.NavigateTo(url);
            await runner.SendKeys("#MainContentPlaceHolder_tbxUsuario_tbxEdit", "RTABOT");
            await runner.SendKeys("#MainContentPlaceHolder_tbxSenha_tbxEdit", "123456");
            await runner.Click("#MainContentPlaceHolder_btnLogin");

            var companyLoaded = await runner.WaitForElement(companySelector);
            Assert.True(companyLoaded);

            found = await runner.GetText(companySelector);
            await runner.CloseSession();
        }


        Assert.Equal(expected, found);

    }

}
