/**
 * Audio Service - Quản lý âm thanh thông báo
 * Sử dụng để phát âm thanh khi có sự kiện quan trọng
 */
class AudioService {
  constructor() {
    // Preload audio files để giảm delay khi play
    this.sounds = {
      success: null,
      error: null,
      notification: null
    }

    this.isEnabled = true // Cho phép user tắt/bật âm thanh
    this.volume = 0.7 // Volume mặc định

    this.initializeSounds()
  }

  /**
   * Khởi tạo và preload các file âm thanh
   */
  initializeSounds() {
    try {
      // Load success sound
      this.sounds.success = new Audio('/sounds/notification-sound.mp3')
      this.sounds.success.volume = this.volume
      this.sounds.success.preload = 'auto'

      // Load notification sound (same file for now)
      this.sounds.notification = new Audio('/sounds/notification-sound.mp3')
      this.sounds.notification.volume = this.volume
      this.sounds.notification.preload = 'auto'

      console.log('🔊 Audio Service initialized successfully')
    } catch (error) {
      console.warn('⚠️ Audio Service initialization failed:', error)
    }
  }

  /**
   * Phát âm thanh thành công
   */
  playSuccess() {
    this.playSound('success')
  }

  /**
   * Phát âm thanh thông báo
   */
  playNotification() {
    this.playSound('notification')
  }

  /**
   * Phát âm thanh lỗi
   */
  playError() {
    // Có thể thêm error sound riêng sau
    console.log('🔊 Error sound (placeholder)')
  }

  /**
   * Phát một âm thanh cụ thể
   * @param {string} soundType - Loại âm thanh (success, error, notification)
   */
  playSound(soundType) {
    if (!this.isEnabled) {
      console.log('🔇 Audio disabled')
      return
    }

    try {
      const sound = this.sounds[soundType]
      if (sound) {
        // Reset time về 0 để có thể play lại ngay lập tức
        sound.currentTime = 0

        // Play sound
        sound.play().then(() => {
          console.log(`🔊 Played ${soundType} sound`)
        }).catch(error => {
          console.warn(`⚠️ Failed to play ${soundType} sound:`, error)
        })
      } else {
        console.warn(`⚠️ Sound ${soundType} not found`)
      }
    } catch (error) {
      console.warn('⚠️ Error playing sound:', error)
    }
  }

  /**
   * Bật/tắt âm thanh
   * @param {boolean} enabled - true để bật, false để tắt
   */
  setEnabled(enabled) {
    this.isEnabled = enabled
    console.log(`🔊 Audio ${enabled ? 'enabled' : 'disabled'}`)
  }

  /**
   * Điều chỉnh volume
   * @param {number} volume - Volume từ 0.0 đến 1.0
   */
  setVolume(volume) {
    this.volume = Math.max(0, Math.min(1, volume))

    // Cập nhật volume cho tất cả sounds
    Object.values(this.sounds).forEach(sound => {
      if (sound) {
        sound.volume = this.volume
      }
    })

    console.log(`🔊 Volume set to ${this.volume}`)
  }

  /**
   * Check xem audio có được support không
   */
  isSupported() {
    return typeof Audio !== 'undefined'
  }
}

// Export singleton instance
export default new AudioService()
