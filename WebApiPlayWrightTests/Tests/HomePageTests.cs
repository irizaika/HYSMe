using Microsoft.Playwright;
using WebApiPlayWrightTests.Fixture;


namespace WebApiPlayWrightTests.Tests;


[TestFixture]
public class HomePageTests : AuthenticatedPageTest
{

    [Test]
    public async Task DetailsOpenedFromHomeReturnsHome()
    {
        await Page.GotoAsync("/");

        await Page
            .Locator(".pet-row")
            .First
            .ClickAsync();


        await Expect(Page)
            .ToHaveURLAsync(
                new Regex("/Pets/Details/\\d+")
            );


        await Page
            .GetByRole(
                AriaRole.Link,
                new() { Name = new Regex("back", RegexOptions.IgnoreCase).ToString() })
            .ClickAsync();


        await Expect(Page)
            .ToHaveURLAsync(
                new Regex("^.*/$")
            );
    }
}