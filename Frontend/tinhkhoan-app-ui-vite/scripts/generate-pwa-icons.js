#!/usr/bin/env node

/**
 * Script tạo PWA icons từ logo Agribank
 * Đây là script mô phỏng - trong thực tế cần dùng tools như ImageMagick hoặc Sharp
 */

import fs from 'fs';
import path from 'path';
import { fileURLToPath } from 'url';

const __filename = fileURLToPath(import.meta.url);
const __dirname = path.dirname(__filename);

const publicDir = path.join(__dirname, '../public');

// Tạo template cho browserconfig.xml
const browserConfig = `<?xml version="1.0" encoding="utf-8"?>
<browserconfig>
    <msapplication>
        <tile>
            <square150x150logo src="/mstile-150x150.png"/>
            <TileColor>#8B1538</TileColor>
        </tile>
    </msapplication>
</browserconfig>`;

// Tạo file browserconfig.xml
fs.writeFileSync(path.join(publicDir, 'browserconfig.xml'), browserConfig);

console.log('✅ Đã tạo browserconfig.xml');
console.log('📝 Lưu ý: Cần tạo các PWA icons thủ công từ Logo-Agribank-2.png:');
console.log('   - pwa-64x64.png (64x64px)');
console.log('   - pwa-192x192.png (192x192px)');
console.log('   - pwa-512x512.png (512x512px)');
console.log('   - maskable-icon-512x512.png (512x512px với padding)');
console.log('   - apple-touch-icon.png (180x180px)');
console.log('   - apple-touch-icon-152x152.png (152x152px)');
console.log('   - apple-touch-icon-167x167.png (167x167px)');
console.log('   - apple-touch-icon-180x180.png (180x180px)');
console.log('   - mstile-150x150.png (150x150px)');
