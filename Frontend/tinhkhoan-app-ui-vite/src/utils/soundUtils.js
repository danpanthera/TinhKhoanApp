/**
 * Âm thanh thông báo upload hoàn tất
 */
const notificationSound = new Audio('/sounds/notification-sound.mp3')

export const playNotificationSound = () => {
  // Tăng âm lượng lên 100%
  notificationSound.volume = 1.0

  // Phát âm thanh
  notificationSound.play().catch(error => {
    console.warn('Không thể phát âm thanh thông báo:', error)
  })
}

export default {
  playNotificationSound,
}
