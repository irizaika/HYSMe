using Microsoft.Playwright;

namespace WebApiPlayWrightTests.Tests
{
    [Parallelizable(ParallelScope.Self)]
    [TestFixture]
    public class UnauthenticatedTests : PageTest
    {
        public override BrowserNewContextOptions ContextOptions()
        {
            return new()
            {
                BaseURL = "https://localhost:7084"
            };
        }

        [Test]
        public async Task GoToLogin()
        {
           await Page.GotoAsync("/");
         //   await Page.GotoAsync("https://localhost:7084/");

            await Page.GetByRole(AriaRole.Button, new PageGetByRoleOptions { Name = "+ Report Lost" })
                .ClickAsync();

            await Expect(Page).ToHaveURLAsync(new Regex(".*Login.*"));
        }
    }
}
