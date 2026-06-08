import { test, expect } from '@playwright/test';

test.use({ storageState: 'storageState.json' });//login saved 

test('details opened from home returns to home', async ({ page }) => {

    await page.goto('/');

    await page.locator('.pet-row').first().click();

    await expect(page).toHaveURL(/\/Pets\/Details\/\d+/);

    await page.getByRole('link', { name: /back/i }).click();

    await expect(page).toHaveURL('/');
});