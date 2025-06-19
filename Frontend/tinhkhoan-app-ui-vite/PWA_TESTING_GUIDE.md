# PWA Functionality Testing Guide

## Overview
This guide provides step-by-step instructions to test all Progressive Web App (PWA) features implemented in the TinhKhoan application.

## Testing Environment Setup

### Prerequisites
1. The application should be running on `http://localhost:4173/` (preview server)
2. Use Chrome/Edge for best PWA support testing
3. Open Developer Tools (F12) for advanced testing

## PWA Features to Test

### 1. Service Worker Installation
**What to check:**
- Service worker registers successfully
- Caching strategies are working

**How to test:**
1. Open Developer Tools → Application tab → Service Workers
2. Verify `sw.js` is registered and running
3. Check Console for service worker logs
4. Go to Application → Storage → Cache Storage
5. Verify caches are created: `api-cache`, `static-cache`, `image-cache`, etc.

### 2. Web App Manifest
**What to check:**
- App can be installed as PWA
- Proper app metadata and icons

**How to test:**
1. Open Developer Tools → Application tab → Manifest
2. Verify manifest properties:
   - Name: "TinhKhoan - Agribank Lai Châu Center"
   - Short name: "TinhKhoan"
   - Theme color: "#1B5E20"
   - Background color: "#f5f5f5"
   - Display mode: "standalone"
3. Check that all icon sizes are present (64x64, 192x192, 512x512)

### 3. Install Prompt
**What to check:**
- Install banner appears in supported browsers
- Custom install prompt works

**How to test:**
1. Look for the install button in the address bar (Chrome/Edge)
2. Check if custom PWA install prompt component appears
3. Click install and verify app installs to desktop/home screen
4. Open installed app and verify it runs in standalone mode

### 4. Offline Functionality
**What to check:**
- App works when offline
- Offline indicator appears
- Data is cached and available offline

**How to test:**
1. Load the application normally (online)
2. Navigate through different pages to cache content
3. Open Developer Tools → Network tab
4. Check "Offline" checkbox to simulate offline mode
5. Refresh the page - it should still work
6. Verify offline indicator appears in the UI
7. Try navigating to cached pages - they should load from cache

### 5. Background Sync
**What to check:**
- Actions are queued when offline
- Queued actions execute when back online

**How to test:**
1. Go offline (Network tab → Offline checkbox)
2. Try to perform create/update/delete operations
3. Verify actions are queued (check OfflineIndicator component)
4. Go back online
5. Verify queued actions are automatically executed
6. Check that data was successfully synced

### 6. Offline Store and Sync Status
**What to check:**
- Offline status is properly detected
- Sync progress is shown
- Manual sync works

**How to test:**
1. Monitor the OfflineIndicator component
2. Go offline/online and verify status changes immediately
3. When offline with queued actions, verify:
   - Number of pending actions is shown
   - Sync button appears when back online
   - Manual sync can be triggered
4. Check cache management features

### 7. Update Notifications
**What to check:**
- App prompts for updates when new version is available
- Update process works smoothly

**How to test:**
1. Make a small change to the app code
2. Build again: `npm run build`
3. The running PWA should show an update notification
4. Click update and verify new version loads

### 8. Responsive Design & Mobile Features
**What to check:**
- App works well on mobile devices
- Touch interactions work properly

**How to test:**
1. Open Developer Tools → Toggle device toolbar
2. Test various mobile device sizes
3. Verify touch interactions work
4. Check that the app is mobile-friendly

## Advanced Testing Scenarios

### Test Offline-First Workflow
1. Start the app offline
2. Verify cached content loads
3. Verify offline indicator shows appropriate message
4. Go online and verify sync happens automatically

### Test Network Reliability
1. Use Developer Tools → Network tab → Throttling
2. Set to "Slow 3G" or "Fast 3G"
3. Verify app remains responsive
4. Check that appropriate loading states are shown

### Test Cross-Browser Compatibility
1. Test in Chrome, Firefox, Safari, Edge
2. Verify core PWA features work across browsers
3. Note: Safari has limited PWA support compared to Chrome/Edge

## Expected Results

### ✅ Successful PWA Implementation Should Show:
- ✅ Service worker registered without errors
- ✅ Install prompt appears and works
- ✅ App works offline after initial cache
- ✅ Offline indicator shows current status
- ✅ Actions are queued when offline and sync when online
- ✅ Update notifications appear for new versions
- ✅ Smooth mobile experience
- ✅ Fast loading times due to caching

### ❌ Issues to Watch For:
- ❌ Service worker registration errors
- ❌ Missing cache entries
- ❌ App fails to load offline
- ❌ Sync failures when coming back online
- ❌ Missing update notifications
- ❌ Poor mobile experience

## Developer Tools & Debugging

### Useful Chrome DevTools Features:
1. **Application Tab:**
   - Service Workers: Check registration status
   - Storage: View cached data and clear cache
   - Manifest: Verify PWA manifest

2. **Network Tab:**
   - Offline simulation
   - View cached vs network requests
   - Throttling for slow network testing

3. **Console:**
   - Service worker logs
   - Error messages
   - PWA-related messages

### Lighthouse PWA Audit
1. Open Developer Tools → Lighthouse tab
2. Select "Progressive Web App" category
3. Run audit
4. Aim for score of 90+ for production PWA

## Production Deployment Notes

When deploying to production:
1. Ensure HTTPS is enabled (PWA requirement)
2. Verify all icons are properly sized and optimized
3. Test install process on actual mobile devices
4. Monitor service worker updates in production
5. Set up proper cache invalidation strategies

## Troubleshooting Common Issues

### Service Worker Not Registering
- Check console for registration errors
- Verify `sw.js` is accessible at root level
- Ensure HTTPS in production

### Install Prompt Not Showing
- Verify manifest.json is valid
- Check that all PWA criteria are met
- Some browsers have different trigger conditions

### Offline Mode Not Working
- Check that resources are properly cached
- Verify network intercepting in service worker
- Clear cache and re-test caching

### Sync Not Working
- Check browser console for sync errors
- Verify background sync is supported
- Test with different network conditions
