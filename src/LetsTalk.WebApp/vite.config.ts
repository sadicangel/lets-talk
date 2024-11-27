import { sveltekit } from '@sveltejs/kit/vite';
import { defineConfig } from 'vite';
import process from 'process';

export default defineConfig({
	plugins: [sveltekit()],
	server: {
		port: parseInt(process.env.PORT!),
		proxy: {
			'/api': {
				target: process.env['services__letstalk-webapi__https__0'] || process.env['services__letstalk-webapi__http__0']!,
				changeOrigin: true,
				secure: false
			},
			'/hub': {
				target: (process.env['services__letstalk-webapi__https__0'] || process.env['services__letstalk-webapi__http__0']!).replace('http', 'ws'),
				changeOrigin: true,
				secure: false,
				ws: true,
			},
		}
	}
});
