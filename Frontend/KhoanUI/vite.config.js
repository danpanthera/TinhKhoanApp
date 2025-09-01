import vue from '@vitejs/plugin-vue'
import path from 'path'
import { defineConfig } from 'vite'
import { VitePWA } from 'vite-plugin-pwa'

// https://vite.dev/config/
export default defineConfig({
  // 🇻🇳 Cấu hình UTF-8 cho tiếng Việt
  build: {
    target: 'esnext',
    // Đảm bảo UTF-8 encoding trong build
    assetsInlineLimit: 0,
    rollupOptions: {
      output: {
  // Simplify manualChunks until dashboard components exist
  manualChunks: undefined,
      },
    },
  },
  plugins: [
    vue(),
    VitePWA({
      registerType: 'autoUpdate',
      workbox: {
        clientsClaim: true,
        skipWaiting: true,
        globPatterns: ['**/*.{js,css,html,ico,png,svg,json,vue,txt,woff2}'],
      },
      includeAssets: ['favicon.svg', 'Logo-Agribank-2.png', 'agribank-logo.svg'],
      manifest: {
        name: 'Agribank Lai Châu Center - Tính khoán',
        short_name: 'Agribank TK',
        description: 'Hệ thống quản lý tính khoán và lương Agribank Lai Châu Center',
        theme_color: '#8B1538',
        background_color: '#ffffff',
        display: 'standalone',
        scope: '/',
        start_url: '/',
        orientation: 'portrait-primary',
        categories: ['business', 'finance', 'productivity'],
        lang: 'vi-VN',
        icons: [
          {
            src: 'pwa-64x64.png',
            sizes: '64x64',
            type: 'image/png',
          },
          {
            src: 'pwa-192x192.png',
            sizes: '192x192',
            type: 'image/png',
          },
          {
            src: 'pwa-512x512.png',
            sizes: '512x512',
            type: 'image/png',
            purpose: 'any',
          },
          {
            src: 'maskable-icon-512x512.png',
            sizes: '512x512',
            type: 'image/png',
            purpose: 'maskable',
          },
        ],
      },
      devOptions: {
        enabled: false, // Disable PWA in development to avoid service worker errors
        type: 'module',
      },
    }),
  ],
  resolve: {
    alias: {
      '@': path.resolve(__dirname, './src'),
    },
  },
  server: {
    port: 3000,
    host: true, // Bind to all network interfaces
    strictPort: true, // Fail if port is already in use
    cors: true,
    proxy: {
      '/api': {
        target: 'http://localhost:5055',
        changeOrigin: true,
        secure: false,
        timeout: 30000,
        proxyTimeout: 30000,
      },
    },
  },
})
