import { test, expect } from '@playwright/test';

test.use({ storageState: 'storageState.json' });//login saved 

test('pet list page loads', async ({ page }) => {
    await page.goto('/Pets');

    await expect(
        page.getByRole('heading', { name: 'Lost Pets' }) //h1-h6
    ).toBeVisible();

    await expect(page.locator('.pet-row').first()).toBeVisible();
});