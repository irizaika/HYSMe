import { test, expect } from '@playwright/test';

test.use({ storageState: 'storageState.json' });//login saved 

test('preserves search and page after returning from details', async ({ page }) => {

    await page.goto('/Pets');

    await page.locator('#petSearch').fill('dog');

    await page.waitForTimeout(500);

    const page2 = page.getByRole('button', { name: '2' });

    if (await page2.isVisible()) {
        await page2.click();
    }

    const searchValue =
        await page.locator('#petSearch').inputValue();

    await page.locator('.pet-row').first().click();

    await page.getByRole('link', { name: /back/i }).click();

    await expect(page.locator('#petSearch'))
        .toHaveValue(searchValue);
});
