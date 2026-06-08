import { test, expect } from '@playwright/test';

test.use({ storageState: 'storageState.json' });//login saved 

test('details back returns to pet list', async ({ page }) => {

    await page.goto('/Pets?page=2');

    await page.locator('.pet-row').first().click();

    await page.getByRole('link', { name: /back/i }).click();

    await expect(page).toHaveURL(/\/Pets\?page=2/);
});
