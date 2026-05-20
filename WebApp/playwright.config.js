import { defineConfig } from '@playwright/test';

export default defineConfig({
    globalSetup: './tests/setup/auth.setup.js',
    use: {
        baseURL: 'https://localhost:7084', //  app URL
        headless: true,
        //storageState: 'storageState.json'
    }
});