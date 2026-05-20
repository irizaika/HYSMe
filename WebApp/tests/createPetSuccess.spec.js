import { test, expect } from '@playwright/test';

test.use({ storageState: 'storageState.json' });//login saved 

test('user can create a lost pet', async ({ page }) => {
    await page.goto('/');

    await page.getByRole('button', { name: '+ Report Lost' }).click();

    await page.fill('#petName', 'Test Dog');
    await page.fill('#petType', 'Dog');
    await page.fill('#petDescription', 'Lost near park');
    await page.fill('#petLatitude', '56.95');
    await page.fill('#petLongitude', '24.10');
    await page.fill('#petDateLost', '2023-05-15');

    await page.click('#submit');

    await expect(page).toHaveURL(/\/$/); 

    await expect(page.getByText('Pet created!')).toBeVisible();
});