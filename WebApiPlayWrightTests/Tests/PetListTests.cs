using Microsoft.Playwright;
using WebApiPlayWrightTests.Fixture;


namespace WebApiPlayWrightTests.Tests;


[TestFixture]
public class PetListTests : AuthenticatedPageTest
{

    [Test]
    public async Task PetListPageLoads()
    {
        await Page.GotoAsync("/Pets");


        await Expect(
            Page.GetByRole(
                AriaRole.Heading,
                new() { Name = "Lost Pets" })
        )
        .ToBeVisibleAsync();


        await Expect(
            Page.Locator(".pet-row").First
        )
        .ToBeVisibleAsync();
    }



    [Test]
    public async Task PaginationChangesPage()
    {
        await Page.GotoAsync("/Pets");


        await Page
            .GetByRole(
                AriaRole.Button,
                new() { Name = "2" })
            .ClickAsync();


        await Expect(Page)
            .ToHaveURLAsync(
                new Regex("page=2")
            );
    }



    [Test]
    public async Task SearchFiltersPets()
    {
        await Page.GotoAsync("/Pets");


        await Page
            .Locator("#petSearch")
            .FillAsync("dog");


        await Page.WaitForTimeoutAsync(500);


        await Expect(Page)
            .ToHaveURLAsync(
                new Regex("search=dog|Pets")
            );


        var count =
            await Page
                .Locator(".pet-row")
                .CountAsync();


        Assert.That(count, Is.GreaterThan(0));
    }
}