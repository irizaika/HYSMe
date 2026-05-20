import { test, expect } from '@playwright/test';

test.use({ storageState: undefined });

test('redirects to login if not authenticated', async ({ page }) => {

    await page.goto('/');

    await page.getByRole('button', { name: '+ Report Lost' }).click();

    await expect(page).toHaveURL(/login/i);
});