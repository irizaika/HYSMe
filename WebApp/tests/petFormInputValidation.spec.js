import { test, expect } from '@playwright/test';

test.use({ storageState: 'storageState.json' });

test('validation shows and clears correctly', async ({ page }) => {

    await page.goto('/');

    await page.getByRole('button', { name: '+ Report Lost' }).click();

    await page.click('#submit');

    const form = page.locator('#createPetForm');

    // Required fields becomes invalid
    await expect(form.locator('#petName')).toHaveClass(/is-invalid/);
    await expect(form.locator('#petType')).toHaveClass(/is-invalid/);
    await expect(form.locator('#petDescription')).toHaveClass(/is-invalid/);
    await expect(form.locator('#petLatitude')).toHaveClass(/is-invalid/); 
    await expect(form.locator('#petLongitude')).toHaveClass(/is-invalid/);

    // Error messages appears
    const nameError = form.locator('[data-valmsg-for="Name"]');
    await expect(nameError).not.toHaveText('');

    const typeError = form.locator('[data-valmsg-for="Type"]');
    await expect(typeError).not.toHaveText('');

    const descriptionError = form.locator('[data-valmsg-for="Description"]');
    await expect(descriptionError).not.toHaveText('');

    const latitudeError = form.locator('[data-valmsg-for="Latitude"]');
    await expect(latitudeError).not.toHaveText(''); 

    const longitudeError = form.locator('[data-valmsg-for="Longitude"]');
    await expect(longitudeError).not.toHaveText(''); 

    // Fix field
    await page.fill('#petName', 'Buddy');
    await page.fill('#petType', 'Dog');
    await page.fill('#petDescription', 'Lost near park');
    await page.fill('#petLatitude', '56.95');
    await page.fill('#petLongitude', '24.10');

    // Error disappears
    await expect(form.locator('#petName')).not.toHaveClass(/is-invalid/);
    await expect(nameError).toHaveText('');

    await expect(form.locator('#petType')).not.toHaveClass(/is-invalid/);
    await expect(typeError).toHaveText('');

    await expect(form.locator('#petDescription')).not.toHaveClass(/is-invalid/);
    await expect(descriptionError).toHaveText('');

    await expect(form.locator('#petLatitude')).not.toHaveClass(/is-invalid/);
    await expect(latitudeError).toHaveText('');

    await expect(form.locator('#petLongitude')).not.toHaveClass(/is-invalid/);
    await expect(longitudeError).toHaveText('');
});