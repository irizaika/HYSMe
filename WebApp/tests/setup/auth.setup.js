import { chromium } from '@playwright/test';

export default async () => {
    const browser = await chromium.launch();
    const page = await browser.newPage();

    await page.goto('https://localhost:7084/auth/login');

    await page.fill('#UserName', 'email@email.com');
    await page.fill('#Password', 'Admin01!');
    await page.click('button[type=submit]');

    await page.waitForURL('**/');

    await page.context().storageState({ path: 'storageState.json' });

    await browser.close();
};
