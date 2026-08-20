using Microsoft.Extensions.Configuration;
using Microsoft.Playwright;

namespace WebApiPlayWrightTests.Fixture;

public abstract class BasePageTest : PageTest
{
    protected static readonly IConfiguration Configuration =
        new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: false)
            .AddJsonFile(
                $"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}.json",
                optional: true)
            .AddEnvironmentVariables()
            .Build();

    protected static bool Headless =>
        bool.TryParse(Configuration["Playwright:Headless"], out var value)
            ? value
            : true;

    protected static float SlowMo =>
        float.TryParse(Configuration["Playwright:SlowMo"], out var value)
            ? value
            : 0;

    protected static string BaseUrl
    {
        get
        {
            var baseUrl = Environment.GetEnvironmentVariable("BASE_URL");

            return string.IsNullOrWhiteSpace(baseUrl)
                ? "https://localhost:7084"
                : baseUrl;
        }
    }

    public override Task<BrowserTypeLaunchOptions?> LaunchOptionsAsync()
    {
        return Task.FromResult<BrowserTypeLaunchOptions?>(
            new BrowserTypeLaunchOptions
            {
                Headless = Headless,
                SlowMo = SlowMo
            });
    }

    public override BrowserNewContextOptions ContextOptions()
    {
        return new BrowserNewContextOptions
        {
            BaseURL = BaseUrl,
            
        };
    }

    [SetUp]
    public async Task StartTracing()
    {
        await Context.Tracing.StartAsync(new()
        {
            Screenshots = true,
            Snapshots = true,
            Sources = true
        });
    }

    [TearDown]
    public async Task StopTracing()
    {
        if (TestContext.CurrentContext.Result.Outcome.Status != NUnit.Framework.Interfaces.TestStatus.Failed)
        {
            await Context.Tracing.StopAsync();
            return;
        }

        var testName = TestContext.CurrentContext.Test.Name;

        foreach (var c in Path.GetInvalidFileNameChars())
            testName = testName.Replace(c, '_');

        var directory =
            Environment.GetEnvironmentVariable("TEST_RESULTS_DIR")
            ?? Path.Combine(
                TestContext.CurrentContext.WorkDirectory,
                "traces");

        Directory.CreateDirectory(directory);

        var tracePath = Path.Combine(
            directory,
            $"{testName}.zip");

        await Context.Tracing.StopAsync(new()
        {
            Path = tracePath
        });

        TestContext.WriteLine($"Playwright trace: {tracePath}");
    }

}