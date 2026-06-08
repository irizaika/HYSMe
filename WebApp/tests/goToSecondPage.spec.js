import { test, expect } from '@playwright/test';

test.use({ storageState: 'storageState.json' });//login saved 

test('pagination changes page', async ({ page }) => {
    await page.goto('/Pets');

    await page.getByRole('button', { name: '2' }).click();

    await expect(page).toHaveURL(/page=2/);
});
