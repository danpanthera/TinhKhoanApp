#!/usr/bin/env node

import { execSync } from 'child_process';
import fs from 'fs';
import path from 'path';
import { fileURLToPath } from 'url';

const __filename = fileURLToPath(import.meta.url);
const __dirname = path.dirname(__filename);

console.log('Starting type checking with debug info...');

try {
  // Kiểm tra xem tsconfig.json có tồn tại không
  const tsconfigPath = path.join(process.cwd(), 'tsconfig.json');
  if (fs.existsSync(tsconfigPath)) {
    console.log('tsconfig.json found');
    const tsconfigContent = fs.readFileSync(tsconfigPath, 'utf8');
    console.log('tsconfig.json content:', tsconfigContent);
  } else {
    console.log('tsconfig.json not found');
  }

  // Đếm số lượng file Vue
  const result = execSync('find src -name "*.vue" | wc -l').toString().trim();
  console.log(`Number of Vue files: ${result}`);

  // Liệt kê các file TypeScript có vấn đề
  console.log('Looking for potential problematic files...');
  const tsFiles = execSync('find src -name "*.ts" -o -name "*.tsx"').toString().trim().split('\n');
  console.log(`Found ${tsFiles.length} TypeScript files`);

  // Kiểm tra từng file TS
  for (const file of tsFiles) {
    if (!file) continue;
    try {
      console.log(`Checking ${file}...`);
      execSync(`npx tsc --noEmit ${file}`, { timeout: 5000 });
      console.log(`✅ ${file} - OK`);
    } catch (e) {
      console.log(`❌ ${file} - Error: ${e.message}`);
    }
  }

  // Thử kiểm tra một file Vue đơn giản
  try {
    console.log('Testing a simple Vue file...');
    execSync('npx vue-tsc --noEmit src/components/dashboard/LoadingOverlay.vue', { timeout: 10000 });
    console.log('✅ Vue file test passed');
  } catch (e) {
    console.log('❌ Vue file test failed:', e.message);
  }

  console.log('Debug checks completed');
} catch (error) {
  console.error('Error during checking:', error.message);
  if (error.stdout) console.log('stdout:', error.stdout.toString());
  if (error.stderr) console.log('stderr:', error.stderr.toString());
  process.exit(1);
}
