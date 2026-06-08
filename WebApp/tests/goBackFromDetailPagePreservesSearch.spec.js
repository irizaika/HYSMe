import { test, expect } from '@playwright/test';

test.use({ storageState: 'storageState.json' });//login saved 

test('details back preserves search', async ({ page }) => {

    await page.goto('/Pets?search=dog&page=2');

    await page.locator('.pet-row').first().click();

    await page.getByRole('link', { name: /back/i }).click();

    await expect(page).toHaveURL(/search=dog/);
    await expect(page).toHaveURL(/page=2/);

    await expect(page.locator('#petSearch')).toHaveValue('dog');
});
