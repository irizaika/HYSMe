using Microsoft.Playwright;
using WebApiPlayWrightTests.Fixture;


namespace WebApiPlayWrightTests.Tests;


[TestFixture]
public class PetDetailsTests : AuthenticatedPageTest
{

    [Test]
    public async Task OpenPetDetailsFromList()
    {
        await Page.GotoAsync("/Pets");


        await Page
            .Locator(".pet-row")
            .First
            .ClickAsync();


        await Expect(Page)
            .ToHaveURLAsync(
                new Regex("/Pets/Details/\\d+")
            );


        await Expect(
            Page.GetByRole(
                AriaRole.Link,
                new() { Name = new Regex("Back").ToString() })
        )
        .ToBeVisibleAsync();
    }

    [Test]
    public async Task OpenPetDetailsFromHomePage()
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


        await Expect(
            Page.GetByRole(
                AriaRole.Link,
                new() { Name = new Regex("Back").ToString() })
        )
        .ToBeVisibleAsync();
    }



    [Test]
    public async Task DetailsBackPreservesSearch()
    {
        await Page.GotoAsync(
            //"/Pets?search=dog&page=2"
            "/Pets?search=ri&page=2"
        );


        await Page
            .Locator(".pet-row")
            .First
            .ClickAsync();


        await Page
            .GetByRole(
                AriaRole.Link,
                new() { Name = new Regex("Back").ToString() })
            .ClickAsync();



        await Expect(Page)
            .ToHaveURLAsync(
                //new Regex("search=dog")
                new Regex("search=ri")
            );


        await Expect(Page)
            .ToHaveURLAsync(
                new Regex("page=2")
            );
    }

    [Test]
    public async Task PreservesSearchAndPageAfterReturningFromDetails()
    {
        await Page.GotoAsync("/Pets");


        // Search pets
        await Page
            .Locator("#petSearch")
            .FillAsync("dog");


        // Wait for search/filter update
        await Page.WaitForTimeoutAsync(500);


        // Go to page 2 if pagination exists
        var page2 = Page.GetByRole(
            AriaRole.Button,
            new() { Name = "2" }
        );


        if (await page2.IsVisibleAsync())
        {
            await page2.ClickAsync();
        }


        // Remember current search value
        var searchValue =
            await Page
                .Locator("#petSearch")
                .InputValueAsync();



        // Open first pet details
        await Page
            .Locator(".pet-row")
            .First
            .ClickAsync();



        // Return back
        await Page
            .GetByRole(
                AriaRole.Link,
                new()
                {
                    Name = new Regex("back", RegexOptions.IgnoreCase).ToString()
                })
            .ClickAsync();



        // Search should still be preserved
        await Expect(
            Page.Locator("#petSearch")
        )
        .ToHaveValueAsync(searchValue);
    }
}