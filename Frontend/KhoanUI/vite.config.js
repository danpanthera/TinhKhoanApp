import { defineConfig } from 'vite';

// Minimal Vite config to serve public/ and src/ on port 3000
export default defineConfig({
  server: {
    port: 3000,
    strictPort: true,
    host: 'localhost',
    proxy: {
      // Proxy API calls to backend to avoid CORS and configure single origin
      '/api': {
        target: 'http://localhost:5055',
        changeOrigin: true,
        secure: false,
        // leave path as-is (already starts with /api)
        // rewrite: path => path
      }
    }
  },
  preview: {
    port: 3000,
    strictPort: true,
    host: 'localhost'
  },
  publicDir: 'public',
  build: {
    outDir: 'dist',
    emptyOutDir: false // keep existing dist assets for now
  }
});
