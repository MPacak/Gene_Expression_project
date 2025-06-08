import { fileURLToPath, URL } from 'node:url';

import { defineConfig, loadEnv } from 'vite';
import react from '@vitejs/plugin-react';


export default defineConfig(({ mode }) => {
    // loadEnv will pick up .env, .env.local, .env.[mode], etc.
    const env = loadEnv(mode, process.cwd());

    return {
        plugins: [react()],
        resolve: {
            alias: {
                '@': fileURLToPath(new URL('./src', import.meta.url)),
            },
        },
        server: {
            port: 56864,
            // No HTTPS section at all
            proxy: {
                // proxies any request starting with /api/visualization
                '^/api/visualization': {
                    // use your env var here
                    target: env.VITE_API_URL,
                    changeOrigin: true,
                    secure: false,
                    logLevel: 'debug',
                },
            },
        },
        define: {
            __APP_NAME__: JSON.stringify(env.VITE_APP_NAME || 'GeneExpression')
        }
    };
});