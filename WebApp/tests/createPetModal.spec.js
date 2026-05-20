import { test, expect } from '@playwright/test';

test.use({ storageState: 'storageState.json' });//login saved 

test('opens create pet modal when clicking Report Lost', async ({ page }) => {

    await page.goto('/');

    // Click "Report Lost" button
    await page.getByRole('button', { name: '+ Report Lost' }).click();

    // Expect modal to be visible
    const modal = page.locator('#createPetModal');

    await expect(modal).toBeVisible();

    // Check form exists
    await expect(page.locator('#createPetForm')).toBeVisible();

    // Check important fields
    await expect(page.locator('#petName')).toBeVisible();
    await expect(page.locator('#petLatitude')).toBeVisible();
    await expect(page.locator('#petLongitude')).toBeVisible();
});