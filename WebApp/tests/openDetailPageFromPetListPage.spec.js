import { test, expect } from '@playwright/test';

test.use({ storageState: 'storageState.json' });//login saved

test('open pet details from list', async ({ page }) => {
    await page.goto('/Pets');

    await page.locator('.pet-row').first().click();

    await expect(page).toHaveURL(/\/Pets\/Details\/\d+/);

    await expect(
        page.getByRole('link', { name: /Back/i })
    ).toBeVisible();
});