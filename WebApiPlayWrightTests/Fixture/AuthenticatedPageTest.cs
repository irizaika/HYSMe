using Microsoft.Playwright;

namespace WebApiPlayWrightTests.Fixture;

public abstract class AuthenticatedPageTest : BasePageTest
{
    private static bool _initialized;
    private static readonly SemaphoreSlim Semaphore = new(1, 1);

    [OneTimeSetUp]
    public async Task CreateStorageState()
    {
        if (_initialized)
            return;

        await Semaphore.WaitAsync();

        try
        {
            if (_initialized)
                return;

            using var playwright =
                await Microsoft.Playwright.Playwright.CreateAsync();

            await using var browser =
                await playwright.Chromium.LaunchAsync(new()
                {
                    Headless = true
                });

            var context = await browser.NewContextAsync();
            var page = await context.NewPageAsync();

            Console.WriteLine($"BASE_URL = {BaseUrl}");

            await page.GotoAsync($"{BaseUrl}/auth/login");

            await page.FillAsync(
                "#UserName",
                "email@email.com");

            await page.FillAsync(
                "#Password",
                "Admin01!");

            await page.ClickAsync("button[type=submit]");

            await page.WaitForURLAsync("**/");

            await context.StorageStateAsync(new()
            {
                Path = "storageState.json"
            });

            await context.CloseAsync();

            _initialized = true;
        }
        finally
        {
            Semaphore.Release();
        }
    }

    public override BrowserNewContextOptions ContextOptions()
    {
        return new BrowserNewContextOptions
        {
            BaseURL = BaseUrl,
            StorageStatePath = "storageState.json"
        };
    }
}