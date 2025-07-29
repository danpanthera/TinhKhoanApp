import { precacheAndRoute, cleanupOutdatedCaches } from 'workbox-precaching'
import { registerRoute } from 'workbox-routing'
import { StaleWhileRevalidate, CacheFirst, NetworkFirst } from 'workbox-strategies'
import { CacheableResponsePlugin } from 'workbox-cacheable-response'
import { ExpirationPlugin } from 'workbox-expiration'

// Tự động precache tất cả assets được build bởi Vite
precacheAndRoute(self.__WB_MANIFEST)

// Dọn dẹp cache cũ
cleanupOutdatedCaches()

// Cache cho API responses - Network First strategy
registerRoute(
  ({ url }) => url.pathname.startsWith('/api/'),
  new NetworkFirst({
    cacheName: 'api-cache',
    plugins: [
      new CacheableResponsePlugin({
        statuses: [0, 200],
      }),
      new ExpirationPlugin({
        maxEntries: 50,
        maxAgeSeconds: 5 * 60, // 5 phút cho API cache
      }),
    ],
  })
)

// Cache cho static assets - Cache First strategy
registerRoute(
  ({ request }) =>
    request.destination === 'style' || request.destination === 'script' || request.destination === 'worker',
  new CacheFirst({
    cacheName: 'static-assets',
    plugins: [
      new CacheableResponsePlugin({
        statuses: [0, 200],
      }),
      new ExpirationPlugin({
        maxEntries: 60,
        maxAgeSeconds: 30 * 24 * 60 * 60, // 30 ngày
      }),
    ],
  })
)

// Cache cho images - Cache First strategy
registerRoute(
  ({ request }) => request.destination === 'image',
  new CacheFirst({
    cacheName: 'images',
    plugins: [
      new CacheableResponsePlugin({
        statuses: [0, 200],
      }),
      new ExpirationPlugin({
        maxEntries: 60,
        maxAgeSeconds: 30 * 24 * 60 * 60, // 30 ngày
      }),
    ],
  })
)

// Cache cho fonts - Cache First strategy
registerRoute(
  ({ request }) => request.destination === 'font',
  new CacheFirst({
    cacheName: 'fonts',
    plugins: [
      new CacheableResponsePlugin({
        statuses: [0, 200],
      }),
      new ExpirationPlugin({
        maxEntries: 10,
        maxAgeSeconds: 365 * 24 * 60 * 60, // 1 năm
      }),
    ],
  })
)

// Cache cho external resources (CDN) - Stale While Revalidate
registerRoute(
  ({ url }) => url.origin === 'https://cdnjs.cloudflare.com' || url.origin === 'https://images.unsplash.com',
  new StaleWhileRevalidate({
    cacheName: 'external-resources',
    plugins: [
      new CacheableResponsePlugin({
        statuses: [0, 200],
      }),
      new ExpirationPlugin({
        maxEntries: 30,
        maxAgeSeconds: 7 * 24 * 60 * 60, // 7 ngày
      }),
    ],
  })
)

// Offline fallback cho navigation requests
registerRoute(
  ({ request }) => request.mode === 'navigate',
  new NetworkFirst({
    cacheName: 'pages',
    plugins: [
      new CacheableResponsePlugin({
        statuses: [0, 200],
      }),
    ],
  })
)

// Lắng nghe message để skip waiting
self.addEventListener('message', event => {
  if (event.data && event.data.type === 'SKIP_WAITING') {
    console.log('🔧 SW: Received SKIP_WAITING message')
    self.skipWaiting()
  }
})

// Thông báo khi Service Worker được activate
self.addEventListener('activate', event => {
  console.log('🔧 SW: Service Worker activated')

  // Claim tất cả clients
  event.waitUntil(
    self.clients.claim().then(() => {
      console.log('🔧 SW: Claimed all clients')
    })
  )
})

// Xử lý background sync cho offline actions
self.addEventListener('sync', event => {
  console.log('🔧 SW: Background sync triggered:', event.tag)

  if (event.tag === 'background-sync') {
    event.waitUntil(doBackgroundSync())
  }
})

// Hàm thực hiện background sync
async function doBackgroundSync() {
  try {
    // Lấy dữ liệu pending từ IndexedDB hoặc localStorage
    const pendingActions = JSON.parse(localStorage.getItem('pendingActions') || '[]')

    for (const action of pendingActions) {
      try {
        // Thực hiện action
        await fetch(action.url, {
          method: action.method,
          headers: action.headers,
          body: action.body,
        })

        console.log('🔧 SW: Background sync completed for:', action.url)
      } catch (error) {
        console.error('🔧 SW: Background sync failed for:', action.url, error)
      }
    }

    // Xóa pending actions sau khi hoàn thành
    localStorage.removeItem('pendingActions')
  } catch (error) {
    console.error('🔧 SW: Background sync error:', error)
  }
}

// Push notifications handler
self.addEventListener('push', event => {
  console.log('🔧 SW: Push event received')

  const options = {
    body: event.data ? event.data.text() : 'Có thông báo mới từ Agribank TK',
    icon: '/pwa-192x192.png',
    badge: '/pwa-64x64.png',
    vibrate: [200, 100, 200],
    data: {
      dateOfArrival: Date.now(),
      primaryKey: 1,
    },
    actions: [
      {
        action: 'explore',
        title: 'Xem chi tiết',
        icon: '/pwa-64x64.png',
      },
      {
        action: 'close',
        title: 'Đóng',
        icon: '/pwa-64x64.png',
      },
    ],
  }

  event.waitUntil(self.registration.showNotification('Agribank Lai Châu (7800) Center', options))
})

// Notification click handler
self.addEventListener('notificationclick', event => {
  console.log('🔧 SW: Notification click received:', event.action)

  event.notification.close()

  if (event.action === 'explore') {
    // Mở ứng dụng
    event.waitUntil(self.clients.openWindow('/'))
  }
})

// Install event
self.addEventListener('install', event => {
  console.log('🔧 SW: Service Worker installed')

  // Force activation
  self.skipWaiting()
})

// Error handling
self.addEventListener('error', event => {
  console.error('🔧 SW: Service Worker error:', event.error)
})

self.addEventListener('unhandledrejection', event => {
  console.error('🔧 SW: Unhandled promise rejection:', event.reason)
})
