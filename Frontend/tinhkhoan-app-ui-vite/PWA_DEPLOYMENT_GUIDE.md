# PWA Deployment Guide - TinhKhoan Application

## Overview
This guide provides comprehensive instructions for deploying the TinhKhoan Progressive Web App (PWA) to production environments.

## Pre-Deployment Checklist

### ✅ PWA Requirements Met
- [x] HTTPS enabled (required for PWA in production)
- [x] Web App Manifest configured
- [x] Service Worker implemented
- [x] Responsive design
- [x] Offline functionality
- [x] Install prompts
- [x] Background sync
- [x] Caching strategies

### ✅ Files and Configuration
- [x] `manifest.webmanifest` - Web app manifest
- [x] `sw.js` - Service worker
- [x] PWA icons (64x64, 192x192, 512x512, maskable, Apple touch)
- [x] `browserconfig.xml` - Microsoft tile configuration
- [x] Meta tags in `index.html`
- [x] Vite PWA plugin configuration

## Build Process

### 1. Production Build
```bash
# Navigate to project directory
cd /path/to/tinhkhoan-app-ui-vite

# Install dependencies
npm install

# Run production build
npm run build

# Preview build (optional)
npm run preview
```

### 2. Build Output Verification
After running `npm run build`, verify these files are generated:
```
dist/
├── index.html
├── manifest.webmanifest
├── sw.js
├── workbox-*.js
├── registerSW.js
├── assets/
│   ├── index-*.css
│   └── index-*.js
└── icons/
    ├── pwa-64x64.png
    ├── pwa-192x192.png
    ├── pwa-512x512.png
    └── ...
```

## Deployment Options

### Option 1: Static Hosting (Recommended)

#### Vercel Deployment
```bash
# Install Vercel CLI
npm install -g vercel

# Deploy
vercel --prod

# Follow prompts to configure deployment
```

#### Netlify Deployment
```bash
# Install Netlify CLI
npm install -g netlify-cli

# Deploy
netlify deploy --prod --dir=dist
```

#### GitHub Pages
1. Push code to GitHub repository
2. Go to repository Settings → Pages
3. Select source as GitHub Actions
4. Create `.github/workflows/deploy.yml`:

```yaml
name: Deploy PWA
on:
  push:
    branches: [ main ]
jobs:
  deploy:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v3
      - uses: actions/setup-node@v3
        with:
          node-version: '18'
      - run: npm install
      - run: npm run build
      - uses: peaceiris/actions-gh-pages@v3
        with:
          github_token: ${{ secrets.GITHUB_TOKEN }}
          publish_dir: ./dist
```

### Option 2: Server Deployment

#### Nginx Configuration
```nginx
server {
    listen 443 ssl http2;
    server_name your-domain.com;
    
    ssl_certificate /path/to/ssl/cert.pem;
    ssl_certificate_key /path/to/ssl/private.key;
    
    root /var/www/tinhkhoan-app/dist;
    index index.html;
    
    # PWA specific headers
    location / {
        try_files $uri $uri/ /index.html;
        
        # Cache static assets
        location ~* \.(js|css|png|jpg|jpeg|gif|ico|svg|woff|woff2)$ {
            expires 1y;
            add_header Cache-Control "public, immutable";
        }
        
        # PWA files - no cache
        location ~* \.(webmanifest|sw\.js)$ {
            expires 0;
            add_header Cache-Control "no-cache, no-store, must-revalidate";
        }
    }
    
    # Security headers
    add_header X-Frame-Options "SAMEORIGIN" always;
    add_header X-Content-Type-Options "nosniff" always;
    add_header Strict-Transport-Security "max-age=31536000; includeSubDomains" always;
}

# Redirect HTTP to HTTPS
server {
    listen 80;
    server_name your-domain.com;
    return 301 https://$server_name$request_uri;
}
```

#### Apache Configuration
```apache
<VirtualHost *:443>
    ServerName your-domain.com
    DocumentRoot /var/www/tinhkhoan-app/dist
    
    SSLEngine on
    SSLCertificateFile /path/to/ssl/cert.pem
    SSLCertificateKeyFile /path/to/ssl/private.key
    
    # PWA support
    <IfModule mod_rewrite.c>
        RewriteEngine On
        RewriteBase /
        RewriteRule ^index\.html$ - [L]
        RewriteCond %{REQUEST_FILENAME} !-f
        RewriteCond %{REQUEST_FILENAME} !-d
        RewriteRule . /index.html [L]
    </IfModule>
    
    # Cache static assets
    <LocationMatch "\.(css|js|png|jpg|jpeg|gif|ico|svg|woff|woff2)$">
        ExpiresActive On
        ExpiresDefault "access plus 1 year"
        Header set Cache-Control "public, immutable"
    </LocationMatch>
    
    # PWA files - no cache
    <LocationMatch "\.(webmanifest|sw\.js)$">
        ExpiresActive On
        ExpiresDefault "access plus 0 seconds"
        Header set Cache-Control "no-cache, no-store, must-revalidate"
    </LocationMatch>
    
    # Security headers
    Header always set X-Frame-Options SAMEORIGIN
    Header always set X-Content-Type-Options nosniff
    Header always set Strict-Transport-Security "max-age=31536000; includeSubDomains"
</VirtualHost>
```

## Environment Configuration

### Production Environment Variables
Create `.env.production`:
```env
# API Configuration
VITE_API_BASE_URL=https://your-api-domain.com/api
VITE_APP_NAME=TinhKhoan - Agribank Lai Châu Center
VITE_APP_VERSION=1.0.0

# PWA Configuration
VITE_PWA_NAME=TinhKhoan
VITE_PWA_SHORT_NAME=TinhKhoan
VITE_PWA_DESCRIPTION=Hệ thống tính khoán Agribank Lai Châu Center

# Security
VITE_ENABLE_HTTPS=true
VITE_SECURE_COOKIES=true
```

### Backend API CORS Configuration
Ensure your backend API allows requests from your PWA domain:
```csharp
// In Program.cs or Startup.cs
app.UseCors(options => 
{
    options.WithOrigins("https://your-pwa-domain.com")
           .AllowAnyMethod()
           .AllowAnyHeader()
           .AllowCredentials();
});
```

## SSL/TLS Certificate Setup

### Let's Encrypt (Free SSL)
```bash
# Install Certbot
sudo apt install certbot python3-certbot-nginx

# Obtain certificate
sudo certbot --nginx -d your-domain.com

# Auto-renewal (add to crontab)
0 12 * * * /usr/bin/certbot renew --quiet
```

### Cloudflare (Easy SSL)
1. Add your domain to Cloudflare
2. Change nameservers to Cloudflare
3. Enable SSL/TLS → Full (strict)
4. Enable Always Use HTTPS

## Post-Deployment Verification

### 1. PWA Audit with Lighthouse
```bash
# Install Lighthouse CLI
npm install -g lighthouse

# Run PWA audit
lighthouse https://your-domain.com --only-categories=pwa --output=html --output-path=./pwa-audit.html
```

### 2. Manual Testing Checklist
- [ ] HTTPS working correctly
- [ ] Install prompt appears
- [ ] App installs successfully
- [ ] Offline mode functions
- [ ] Service worker registers
- [ ] Caching works properly
- [ ] Background sync operational
- [ ] Responsive on mobile devices
- [ ] Performance acceptable

### 3. Browser Compatibility Testing
Test on:
- [ ] Chrome (Android/Desktop)
- [ ] Safari (iOS/macOS)
- [ ] Firefox (Android/Desktop)
- [ ] Edge (Desktop)
- [ ] Samsung Internet (Android)

## Monitoring and Analytics

### Service Worker Analytics
Add to your service worker:
```javascript
// Track PWA usage
self.addEventListener('install', event => {
    // Track installation
    if (typeof gtag !== 'undefined') {
        gtag('event', 'pwa_install');
    }
});

self.addEventListener('fetch', event => {
    // Track offline usage
    if (!navigator.onLine) {
        if (typeof gtag !== 'undefined') {
            gtag('event', 'pwa_offline_usage');
        }
    }
});
```

### Performance Monitoring
Consider implementing:
- Google Analytics 4 for PWA tracking
- Web Vitals monitoring
- Error tracking (Sentry, etc.)
- Uptime monitoring

## Maintenance and Updates

### 1. PWA Updates
When deploying updates:
```javascript
// In your main app, handle updates
if ('serviceWorker' in navigator) {
    navigator.serviceWorker.addEventListener('controllerchange', () => {
        // Show update notification to user
        window.location.reload();
    });
}
```

### 2. Cache Management
```javascript
// Clear old caches during updates
self.addEventListener('activate', event => {
    event.waitUntil(
        caches.keys().then(cacheNames => {
            return Promise.all(
                cacheNames.map(cacheName => {
                    if (cacheName !== CURRENT_CACHE_VERSION) {
                        return caches.delete(cacheName);
                    }
                })
            );
        })
    );
});
```

## Security Considerations

### Content Security Policy
Add to `index.html`:
```html
<meta http-equiv="Content-Security-Policy" content="
    default-src 'self';
    script-src 'self' 'unsafe-inline' 'unsafe-eval';
    style-src 'self' 'unsafe-inline';
    img-src 'self' data: https:;
    connect-src 'self' https://your-api-domain.com;
    font-src 'self';
    object-src 'none';
    media-src 'self';
    worker-src 'self';
">
```

### Additional Security Headers
```
X-Frame-Options: SAMEORIGIN
X-Content-Type-Options: nosniff
X-XSS-Protection: 1; mode=block
Strict-Transport-Security: max-age=31536000; includeSubDomains
Referrer-Policy: strict-origin-when-cross-origin
```

## Troubleshooting Common Issues

### Service Worker Not Updating
- Check browser cache settings
- Verify service worker cache versioning
- Test in incognito mode

### Install Prompt Not Showing
- Verify HTTPS is enabled
- Check PWA criteria with Lighthouse
- Test on different browsers

### Offline Mode Not Working
- Verify service worker registration
- Check cache strategies
- Test network interception

### Performance Issues
- Optimize images and assets
- Review caching strategies
- Minimize JavaScript bundles
- Use code splitting

## Rollback Strategy

### Quick Rollback
1. Keep previous build artifacts
2. Use deployment platforms' rollback features
3. Test rollback in staging first

### Emergency Rollback
```bash
# If using Vercel
vercel rollback [deployment-url]

# If using Netlify
netlify deploy --prod --dir=./previous-dist

# If using custom server
mv /var/www/current /var/www/backup-$(date +%Y%m%d)
mv /var/www/previous /var/www/current
sudo systemctl reload nginx
```

## Success Metrics

Track these KPIs post-deployment:
- PWA install rate
- Offline usage statistics
- Page load performance
- User engagement metrics
- Error rates
- Cache hit ratios

## Support and Documentation

- PWA Documentation: https://web.dev/progressive-web-apps/
- Workbox Documentation: https://developers.google.com/web/tools/workbox
- Vite PWA Plugin: https://vite-plugin-pwa.netlify.app/

---

**Last Updated:** June 10, 2025  
**Version:** 1.0.0  
**Deployment Status:** ✅ Ready for Production

---

# Hướng Dẫn Triển Khai PWA - Ứng Dụng TinhKhoan

## Tổng Quan
Hướng dẫn này cung cấp các hướng dẫn toàn diện để triển khai Progressive Web App (PWA) TinhKhoan lên môi trường production.

## Danh Sách Kiểm Tra Trước Triển Khai

### ✅ Yêu Cầu PWA Đã Đáp Ứng
- [x] HTTPS được kích hoạt (bắt buộc cho PWA trong production)
- [x] Web App Manifest đã cấu hình
- [x] Service Worker đã triển khai
- [x] Thiết kế responsive
- [x] Chức năng offline
- [x] Install prompts
- [x] Background sync
- [x] Caching strategies

### ✅ Files và Cấu Hình
- [x] `manifest.webmanifest` - Web app manifest
- [x] `sw.js` - Service worker
- [x] PWA icons (64x64, 192x192, 512x512, maskable, Apple touch)
- [x] `browserconfig.xml` - Cấu hình Microsoft tile
- [x] Meta tags trong `index.html`
- [x] Cấu hình Vite PWA plugin

## Quy Trình Build

### 1. Production Build
```bash
# Điều hướng đến thư mục dự án
cd /path/to/tinhkhoan-app-ui-vite

# Cài đặt dependencies
npm install

# Chạy production build
npm run build

# Xem trước build (tùy chọn)
npm run preview
```

### 2. Xác Minh Đầu Ra Build
Sau khi chạy `npm run build`, xác minh các file này được tạo ra:
```
dist/
├── index.html
├── manifest.webmanifest
├── sw.js
├── workbox-*.js
├── registerSW.js
├── assets/
│   ├── index-*.css
│   └── index-*.js
└── icons/
    ├── pwa-64x64.png
    ├── pwa-192x192.png
    ├── pwa-512x512.png
    └── ...
```

## Tùy Chọn Triển Khai

### Tùy Chọn 1: Static Hosting (Khuyến Nghị)

#### Triển Khai Vercel
```bash
# Cài đặt Vercel CLI
npm install -g vercel

# Triển khai
vercel --prod

# Làm theo hướng dẫn để cấu hình triển khai
```

#### Triển Khai Netlify
```bash
# Cài đặt Netlify CLI
npm install -g netlify-cli

# Triển khai
netlify deploy --prod --dir=dist
```

#### GitHub Pages
1. Push code lên GitHub repository
2. Vào repository Settings → Pages
3. Chọn source là GitHub Actions
4. Tạo `.github/workflows/deploy.yml`:

```yaml
name: Deploy PWA
on:
  push:
    branches: [ main ]
jobs:
  deploy:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v3
      - uses: actions/setup-node@v3
        with:
          node-version: '18'
      - run: npm install
      - run: npm run build
      - uses: peaceiris/actions-gh-pages@v3
        with:
          github_token: ${{ secrets.GITHUB_TOKEN }}
          publish_dir: ./dist
```

### Tùy Chọn 2: Triển Khai Server

#### Cấu Hình Nginx
```nginx
server {
    listen 443 ssl http2;
    server_name your-domain.com;
    
    ssl_certificate /path/to/ssl/cert.pem;
    ssl_certificate_key /path/to/ssl/private.key;
    
    root /var/www/tinhkhoan-app/dist;
    index index.html;
    
    # Headers đặc biệt cho PWA
    location / {
        try_files $uri $uri/ /index.html;
        
        # Cache static assets
        location ~* \.(js|css|png|jpg|jpeg|gif|ico|svg|woff|woff2)$ {
            expires 1y;
            add_header Cache-Control "public, immutable";
        }
        
        # PWA files - không cache
        location ~* \.(webmanifest|sw\.js)$ {
            expires 0;
            add_header Cache-Control "no-cache, no-store, must-revalidate";
        }
    }
    
    # Security headers
    add_header X-Frame-Options "SAMEORIGIN" always;
    add_header X-Content-Type-Options "nosniff" always;
    add_header Strict-Transport-Security "max-age=31536000; includeSubDomains" always;
}

# Chuyển hướng HTTP sang HTTPS
server {
    listen 80;
    server_name your-domain.com;
    return 301 https://$server_name$request_uri;
}
```

#### Cấu Hình Apache
```apache
<VirtualHost *:443>
    ServerName your-domain.com
    DocumentRoot /var/www/tinhkhoan-app/dist
    
    SSLEngine on
    SSLCertificateFile /path/to/ssl/cert.pem
    SSLCertificateKeyFile /path/to/ssl/private.key
    
    # Hỗ trợ PWA
    <IfModule mod_rewrite.c>
        RewriteEngine On
        RewriteBase /
        RewriteRule ^index\.html$ - [L]
        RewriteCond %{REQUEST_FILENAME} !-f
        RewriteCond %{REQUEST_FILENAME} !-d
        RewriteRule . /index.html [L]
    </IfModule>
    
    # Cache static assets
    <LocationMatch "\.(css|js|png|jpg|jpeg|gif|ico|svg|woff|woff2)$">
        ExpiresActive On
        ExpiresDefault "access plus 1 year"
        Header set Cache-Control "public, immutable"
    </LocationMatch>
    
    # PWA files - không cache
    <LocationMatch "\.(webmanifest|sw\.js)$">
        ExpiresActive On
        ExpiresDefault "access plus 0 seconds"
        Header set Cache-Control "no-cache, no-store, must-revalidate"
    </LocationMatch>
    
    # Security headers
    Header always set X-Frame-Options SAMEORIGIN
    Header always set X-Content-Type-Options nosniff
    Header always set Strict-Transport-Security "max-age=31536000; includeSubDomains"
</VirtualHost>
```

## Cấu Hình Môi Trường

### Biến Môi Trường Production
Tạo `.env.production`:
```env
# Cấu hình API
VITE_API_BASE_URL=https://your-api-domain.com/api
VITE_APP_NAME=TinhKhoan - Agribank Lai Châu Center
VITE_APP_VERSION=1.0.0

# Cấu hình PWA
VITE_PWA_NAME=TinhKhoan
VITE_PWA_SHORT_NAME=TinhKhoan
VITE_PWA_DESCRIPTION=Hệ thống tính khoán Agribank Lai Châu Center

# Bảo mật
VITE_ENABLE_HTTPS=true
VITE_SECURE_COOKIES=true
```

### Cấu Hình CORS Backend API
Đảm bảo backend API cho phép requests từ domain PWA của bạn:
```csharp
// Trong Program.cs hoặc Startup.cs
app.UseCors(options => 
{
    options.WithOrigins("https://your-pwa-domain.com")
           .AllowAnyMethod()
           .AllowAnyHeader()
           .AllowCredentials();
});
```

## Cài Đặt Chứng Chỉ SSL/TLS

### Let's Encrypt (SSL Miễn Phí)
```bash
# Cài đặt Certbot
sudo apt install certbot python3-certbot-nginx

# Lấy chứng chỉ
sudo certbot --nginx -d your-domain.com

# Tự động gia hạn (thêm vào crontab)
0 12 * * * /usr/bin/certbot renew --quiet
```

### Cloudflare (SSL Dễ Dàng)
1. Thêm domain vào Cloudflare
2. Đổi nameservers sang Cloudflare
3. Kích hoạt SSL/TLS → Full (strict)
4. Kích hoạt Always Use HTTPS

## Xác Minh Sau Triển Khai

### 1. Kiểm Tra PWA với Lighthouse
```bash
# Cài đặt Lighthouse CLI
npm install -g lighthouse

# Chạy kiểm tra PWA
lighthouse https://your-domain.com --only-categories=pwa --output=html --output-path=./pwa-audit.html
```

### 2. Danh Sách Kiểm Tra Thủ Công
- [ ] HTTPS hoạt động chính xác
- [ ] Install prompt xuất hiện
- [ ] App cài đặt thành công
- [ ] Chế độ offline hoạt động
- [ ] Service worker đăng ký
- [ ] Caching hoạt động đúng
- [ ] Background sync hoạt động
- [ ] Responsive trên thiết bị di động
- [ ] Hiệu suất chấp nhận được

### 3. Kiểm Tra Tương Thích Trình Duyệt
Kiểm tra trên:
- [ ] Chrome (Android/Desktop)
- [ ] Safari (iOS/macOS)
- [ ] Firefox (Android/Desktop)
- [ ] Edge (Desktop)
- [ ] Samsung Internet (Android)

## Giám Sát và Phân Tích

### Phân Tích Service Worker
Thêm vào service worker của bạn:
```javascript
// Theo dõi sử dụng PWA
self.addEventListener('install', event => {
    // Theo dõi cài đặt
    if (typeof gtag !== 'undefined') {
        gtag('event', 'pwa_install');
    }
});

self.addEventListener('fetch', event => {
    // Theo dõi sử dụng offline
    if (!navigator.onLine) {
        if (typeof gtag !== 'undefined') {
            gtag('event', 'pwa_offline_usage');
        }
    }
});
```

### Giám Sát Hiệu Suất
Cân nhác triển khai:
- Google Analytics 4 cho theo dõi PWA
- Giám sát Web Vitals
- Theo dõi lỗi (Sentry, v.v.)
- Giám sát uptime

## Bảo Trì và Cập Nhật

### 1. Cập Nhật PWA
Khi triển khai cập nhật:
```javascript
// Trong app chính, xử lý cập nhật
if ('serviceWorker' in navigator) {
    navigator.serviceWorker.addEventListener('controllerchange', () => {
        // Hiển thị thông báo cập nhật cho người dùng
        window.location.reload();
    });
}
```

### 2. Quản Lý Cache
```javascript
// Xóa cache cũ trong quá trình cập nhật
self.addEventListener('activate', event => {
    event.waitUntil(
        caches.keys().then(cacheNames => {
            return Promise.all(
                cacheNames.map(cacheName => {
                    if (cacheName !== CURRENT_CACHE_VERSION) {
                        return caches.delete(cacheName);
                    }
                })
            );
        })
    );
});
```

## Cân Nhắc Bảo Mật

### Content Security Policy
Thêm vào `index.html`:
```html
<meta http-equiv="Content-Security-Policy" content="
    default-src 'self';
    script-src 'self' 'unsafe-inline' 'unsafe-eval';
    style-src 'self' 'unsafe-inline';
    img-src 'self' data: https:;
    connect-src 'self' https://your-api-domain.com;
    font-src 'self';
    object-src 'none';
    media-src 'self';
    worker-src 'self';
">
```

### Headers Bảo Mật Bổ Sung
```
X-Frame-Options: SAMEORIGIN
X-Content-Type-Options: nosniff
X-XSS-Protection: 1; mode=block
Strict-Transport-Security: max-age=31536000; includeSubDomains
Referrer-Policy: strict-origin-when-cross-origin
```

## Khắc Phục Sự Cố Thường Gặp

### Service Worker Không Cập Nhật
- Kiểm tra cài đặt cache trình duyệt
- Xác minh versioning cache service worker
- Kiểm tra trong chế độ incognito

### Install Prompt Không Hiển Thị
- Xác minh HTTPS được kích hoạt
- Kiểm tra tiêu chí PWA với Lighthouse
- Kiểm tra trên các trình duyệt khác nhau

### Chế độ Offline Không Hoạt Động
- Xác minh đăng ký service worker
- Kiểm tra cache strategies
- Kiểm tra network interception

### Vấn Đề Hiệu Suất
- Tối ưu hóa hình ảnh và assets
- Xem lại caching strategies
- Giảm thiểu JavaScript bundles
- Sử dụng code splitting

## Chiến Lược Rollback

### Rollback Nhanh
1. Giữ artifacts build trước đó
2. Sử dụng tính năng rollback của platforms triển khai
3. Kiểm tra rollback trong staging trước

### Rollback Khẩn Cấp
```bash
# Nếu sử dụng Vercel
vercel rollback [deployment-url]

# Nếu sử dụng Netlify
netlify deploy --prod --dir=./previous-dist

# Nếu sử dụng custom server
mv /var/www/current /var/www/backup-$(date +%Y%m%d)
mv /var/www/previous /var/www/current
sudo systemctl reload nginx
```

## Chỉ Số Thành Công

Theo dõi các KPI sau triển khai:
- Tỷ lệ cài đặt PWA
- Thống kê sử dụng offline
- Hiệu suất tải trang
- Chỉ số tương tác người dùng
- Tỷ lệ lỗi
- Tỷ lệ cache hit

## Hỗ Trợ và Tài Liệu

- Tài liệu PWA: https://web.dev/progressive-web-apps/
- Tài liệu Workbox: https://developers.google.com/web/tools/workbox
- Vite PWA Plugin: https://vite-plugin-pwa.netlify.app/

---

**Cập nhật lần cuối:** 10 tháng 6, 2025  
**Phiên bản:** 1.0.0  
**Trạng thái triển khai:** ✅ Sẵn sàng cho Production
