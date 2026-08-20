using Microsoft.Playwright;
using WebApiPlayWrightTests.Fixture;

namespace WebApiPlayWrightTests.Tests;

[TestFixture]
public class UnauthenticatedTests : BasePageTest
{
    [Test]
    public async Task GoToLogin()
    {
        await Page.GotoAsync("/");

        await Page
            .GetByRole(
                AriaRole.Button,
                new() { Name = "+ Report Lost" })
            .ClickAsync();

        await Expect(Page)
            .ToHaveURLAsync(new Regex(".*Login.*"));
    }
}