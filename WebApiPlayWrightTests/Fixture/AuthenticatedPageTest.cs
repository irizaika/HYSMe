using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;
using NUnit.Framework;

namespace WebApiPlayWrightTests.Fixture;

public abstract class AuthenticatedPageTest : PageTest
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

            using var playwright = await Microsoft.Playwright.Playwright.CreateAsync();
           // using var playwright = await Playwright.CreateAsync();

            await using var browser = await playwright.Chromium.LaunchAsync(
                new BrowserTypeLaunchOptions
                {
                    Headless = true
                });

            var context = await browser.NewContextAsync();

            var page = await context.NewPageAsync();

            await page.GotoAsync("https://localhost:7084/auth/login");

            await page.FillAsync("#UserName", "email@email.com");
            await page.FillAsync("#Password", "Admin01!");

            await page.ClickAsync("button[type=submit]");

            await page.WaitForURLAsync("**/");

            await context.StorageStateAsync(new()
            {
                Path = "storageState.json"
            });

            _initialized = true;
        }
        finally
        {
            Semaphore.Release();
        }
    }

    public override BrowserNewContextOptions ContextOptions()
    {
        return new()
        {
            BaseURL = "https://localhost:7084",
            StorageStatePath = "storageState.json"
        };
    }
}