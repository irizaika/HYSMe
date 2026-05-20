import { test, expect } from '@playwright/test';

test.use({ storageState: 'storageState.json' });//login saved 

test('shows validation error if required fields missing', async ({ page }) => {
    await page.goto('/');
    await page.getByRole('button', { name: '+ Report Lost' }).click();

    await page.click('#submit');
    await expect(page.getByText('Fix validation errors')).toBeVisible(); // toast error displsayes

    await expect(page.locator('#petName')).toBeVisible(); // modal still visible
});