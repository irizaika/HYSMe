using Microsoft.Playwright;
using WebApiPlayWrightTests.Fixture;

namespace WebApiPlayWrightTests.Tests
{
   // [Parallelizable(ParallelScope.Self)]
    [NonParallelizable]
    [TestFixture]
    public class CreatePetTests : AuthenticatedPageTest
    {

        [Test]
        public async Task CreatePetFail()
        {
            await Page.GotoAsync("/");

            await Page
                .GetByRole(
                    AriaRole.Button,
                    new() { Name = "+ Report Lost" })
                .ClickAsync();

            await Page.ClickAsync("#submit");

            await Expect(Page.GetByText("Fix validation errors"))
                .ToBeVisibleAsync();

            await Expect(Page.Locator("#petName")).ToBeVisibleAsync(); // modal still visible
        }
        
        [Test]
        public async Task OpensCreatePetModalWhenClickingReportLost()
        {
            await Page.GotoAsync("/");

            // Click "+ Report Lost"
            await Page
                .GetByRole(
                    AriaRole.Button,
                    new() { Name = "+ Report Lost" })
                .ClickAsync();

            // Modal visible
            var modal = Page.Locator("#createPetModal");

            await Expect(modal).ToBeVisibleAsync();

            // Form exists
            await Expect(Page.Locator("#createPetForm"))
                .ToBeVisibleAsync();

            // Important fields
            await Expect(Page.Locator("#petName"))
                .ToBeVisibleAsync();

            await Expect(Page.Locator("#petLatitude"))
                .ToBeVisibleAsync();

            await Expect(Page.Locator("#petLongitude"))
                .ToBeVisibleAsync();
        }


        [Test]
        public async Task UserCanCreateLostPet()
        {

            //clear db
            using var client = new HttpClient();


            if (string.IsNullOrEmpty(Environment.GetEnvironmentVariable("PETS_API")))
            {

                var response = await client.PostAsync("https://localhost:7001/test/reset", null);
                Assert.That(response.IsSuccessStatusCode, Is.True);

            }
            else
            {
                var response = await client.PostAsync(Environment.GetEnvironmentVariable("PETS_API") + "/test/reset", null);
                Assert.That(response.IsSuccessStatusCode, Is.True);
            }

            //Console.WriteLine(response.StatusCode);
            //Console.WriteLine(await response.Content.ReadAsStringAsync());

            await Page.GotoAsync("/");

            await Page
                .GetByRole(
                    AriaRole.Button,
                    new() { Name = "+ Report Lost" })
                .ClickAsync();


            await Page.FillAsync(
                "#petName",
                "Test Dog");

            await Page.FillAsync(
                "#petType",
                "Dog");

            await Page.FillAsync(
                "#petDescription",
                "Lost near park");

            await Page.FillAsync(
                "#petLatitude",
                "56.95");

            await Page.FillAsync(
                "#petLongitude",
                "24.10");

            await Page.FillAsync(
                "#petDateLost",
                "2023-05-15");


            await Page.ClickAsync("#submit");


            // stays on home page
            await Expect(Page)
                .ToHaveURLAsync(new Regex(".*/$"));


            // success toast
            await Expect(
                Page.GetByText("Pet created!"))
                .ToBeVisibleAsync();
        }


        [Test]
        public async Task ValidationShowsAndClearsCorrectly()
        {
            await Page.GotoAsync("/");


            await Page
                .GetByRole(
                    AriaRole.Button,
                    new() { Name = "+ Report Lost" })
                .ClickAsync();


            await Page.ClickAsync("#submit");


            var form = Page.Locator("#createPetForm");


            // Required fields become invalid

            await Expect(form.Locator("#petName"))
                .ToHaveClassAsync(new Regex("is-invalid"));

            await Expect(form.Locator("#petType"))
                .ToHaveClassAsync(new Regex("is-invalid"));

            await Expect(form.Locator("#petDescription"))
                .ToHaveClassAsync(new Regex("is-invalid"));

            await Expect(form.Locator("#petLatitude"))
                .ToHaveClassAsync(new Regex("is-invalid"));

            await Expect(form.Locator("#petLongitude"))
                .ToHaveClassAsync(new Regex("is-invalid"));



            // Error messages appear

            await Expect(
                form.Locator("[data-valmsg-for='Name']")
            )
            .Not.ToHaveTextAsync("");


            await Expect(
                form.Locator("[data-valmsg-for='Type']")
            )
            .Not.ToHaveTextAsync("");


            await Expect(
                form.Locator("[data-valmsg-for='Description']")
            )
            .Not.ToHaveTextAsync("");


            await Expect(
                form.Locator("[data-valmsg-for='Latitude']")
            )
            .Not.ToHaveTextAsync("");


            await Expect(
                form.Locator("[data-valmsg-for='Longitude']")
            )
            .Not.ToHaveTextAsync("");



            // Fill required fields

            await Page.FillAsync("#petName", "Buddy");

            await Page.FillAsync("#petType", "Dog");

            await Page.FillAsync(
                "#petDescription",
                "Lost near park"
            );

            await Page.FillAsync(
                "#petLatitude",
                "56.95"
            );

            await Page.FillAsync(
                "#petLongitude",
                "24.10"
            );



            // Validation clears

            await Expect(form.Locator("#petName"))
                .Not.ToHaveClassAsync(new Regex("is-invalid"));

            await Expect(
                form.Locator("[data-valmsg-for='Name']")
            )
            .ToHaveTextAsync("");



            await Expect(form.Locator("#petType"))
                .Not.ToHaveClassAsync(new Regex("is-invalid"));

            await Expect(
                form.Locator("[data-valmsg-for='Type']")
            )
            .ToHaveTextAsync("");



            await Expect(form.Locator("#petDescription"))
                .Not.ToHaveClassAsync(new Regex("is-invalid"));

            await Expect(
                form.Locator("[data-valmsg-for='Description']")
            )
            .ToHaveTextAsync("");



            await Expect(form.Locator("#petLatitude"))
                .Not.ToHaveClassAsync(new Regex("is-invalid"));

            await Expect(
                form.Locator("[data-valmsg-for='Latitude']")
            )
            .ToHaveTextAsync("");



            await Expect(form.Locator("#petLongitude"))
                .Not.ToHaveClassAsync(new Regex("is-invalid"));

            await Expect(
                form.Locator("[data-valmsg-for='Longitude']")
            )
            .ToHaveTextAsync("");
        }

    }
}
