using Microsoft.Playwright;

namespace WebApiPlayWrightTests.Setup;

[SetUpFixture]
public class PlaywrightSetup
{
    [OneTimeSetUp]
    public async Task Setup()
    {
        using var playwright = await Playwright.CreateAsync();

        await using var browser =
            await playwright.Chromium.LaunchAsync();

        var context = await browser.NewContextAsync();

        var page = await context.NewPageAsync();

        await page.GotoAsync(
            "https://localhost:7084/auth/login");

        await page.FillAsync(
            "#UserName",
            "email@email.com");

        await page.FillAsync(
            "#Password",
            "Admin01!");

        await page.ClickAsync(
            "button[type=submit]");

        await page.WaitForURLAsync("**/");

        await context.StorageStateAsync(new()
        {
            Path = "storageState.json"
        });
    }
}