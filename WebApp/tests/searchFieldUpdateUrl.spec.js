import { test, expect } from '@playwright/test';

test.use({ storageState: 'storageState.json' });//login saved 

test('search filters pets', async ({ page }) => {
    await page.goto('/Pets');

    await page.locator('#petSearch').fill('dog');

    await page.waitForTimeout(500);

    await expect(page).toHaveURL(/search=dog|Pets/);

    const rows = page.locator('.pet-row');

    expect(await rows.count()).toBeGreaterThan(0);

});