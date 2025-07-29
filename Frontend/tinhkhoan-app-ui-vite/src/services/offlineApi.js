// Offline-aware API wrapper
import apiClient from './api.js'
import { useOfflineStore } from '../stores/offlineStore.js'

/**
 * Offline-aware API service that handles network failures gracefully
 * and queues actions for when the connection is restored
 */
class OfflineApiService {
  constructor() {
    this.offlineStore = null
  }

  /**
   * Initialize the offline store reference
   */
  init() {
    this.offlineStore = useOfflineStore()
  }

  /**
   * Make an offline-aware GET request
   * @param {string} url - API endpoint
   * @param {Object} config - Axios config object
   * @param {Object} options - Offline handling options
   * @returns {Promise} - API response or cached data
   */
  async get(url, config = {}, options = {}) {
    try {
      const response = await apiClient.get(url, config)

      // Cache successful responses if caching is enabled
      if (options.cache && this.offlineStore) {
        await this.offlineStore.cacheResponse(url, response.data)
      }

      return response
    } catch (error) {
      if (this.offlineStore && !this.offlineStore.isOnline) {
        // Try to return cached data when offline
        const cachedData = await this.offlineStore.getCachedResponse(url)
        if (cachedData) {
          return {
            data: cachedData,
            status: 200,
            statusText: 'OK (from cache)',
            headers: {},
            config: config,
            fromCache: true,
          }
        }
      }
      throw error
    }
  }

  /**
   * Make an offline-aware POST request
   * @param {string} url - API endpoint
   * @param {Object} data - Request payload
   * @param {Object} config - Axios config object
   * @param {Object} options - Offline handling options
   * @returns {Promise} - API response or queued action
   */
  async post(url, data = {}, config = {}, options = {}) {
    try {
      const response = await apiClient.post(url, data, config)
      return response
    } catch (error) {
      if (this.offlineStore && !this.offlineStore.isOnline && options.queue !== false) {
        // Queue the action for later execution
        await this.offlineStore.queueAction({
          type: 'POST',
          url,
          data,
          config,
          options,
          timestamp: Date.now(),
        })

        // Return a mock successful response
        return {
          data: { message: 'Action queued for sync when online', queued: true },
          status: 202,
          statusText: 'Accepted (queued)',
          headers: {},
          config: config,
          queued: true,
        }
      }
      throw error
    }
  }

  /**
   * Make an offline-aware PUT request
   * @param {string} url - API endpoint
   * @param {Object} data - Request payload
   * @param {Object} config - Axios config object
   * @param {Object} options - Offline handling options
   * @returns {Promise} - API response or queued action
   */
  async put(url, data = {}, config = {}, options = {}) {
    try {
      const response = await apiClient.put(url, data, config)
      return response
    } catch (error) {
      if (this.offlineStore && !this.offlineStore.isOnline && options.queue !== false) {
        // Queue the action for later execution
        await this.offlineStore.queueAction({
          type: 'PUT',
          url,
          data,
          config,
          options,
          timestamp: Date.now(),
        })

        return {
          data: { message: 'Action queued for sync when online', queued: true },
          status: 202,
          statusText: 'Accepted (queued)',
          headers: {},
          config: config,
          queued: true,
        }
      }
      throw error
    }
  }

  /**
   * Make an offline-aware DELETE request
   * @param {string} url - API endpoint
   * @param {Object} config - Axios config object
   * @param {Object} options - Offline handling options
   * @returns {Promise} - API response or queued action
   */
  async delete(url, config = {}, options = {}) {
    try {
      const response = await apiClient.delete(url, config)
      return response
    } catch (error) {
      if (this.offlineStore && !this.offlineStore.isOnline && options.queue !== false) {
        // Queue the action for later execution
        await this.offlineStore.queueAction({
          type: 'DELETE',
          url,
          config,
          options,
          timestamp: Date.now(),
        })

        return {
          data: { message: 'Action queued for sync when online', queued: true },
          status: 202,
          statusText: 'Accepted (queued)',
          headers: {},
          config: config,
          queued: true,
        }
      }
      throw error
    }
  }

  /**
   * Make an offline-aware PATCH request
   * @param {string} url - API endpoint
   * @param {Object} data - Request payload
   * @param {Object} config - Axios config object
   * @param {Object} options - Offline handling options
   * @returns {Promise} - API response or queued action
   */
  async patch(url, data = {}, config = {}, options = {}) {
    try {
      const response = await apiClient.patch(url, data, config)
      return response
    } catch (error) {
      if (this.offlineStore && !this.offlineStore.isOnline && options.queue !== false) {
        // Queue the action for later execution
        await this.offlineStore.queueAction({
          type: 'PATCH',
          url,
          data,
          config,
          options,
          timestamp: Date.now(),
        })

        return {
          data: { message: 'Action queued for sync when online', queued: true },
          status: 202,
          statusText: 'Accepted (queued)',
          headers: {},
          config: config,
          queued: true,
        }
      }
      throw error
    }
  }

  /**
   * Direct access to the original API client for cases where offline handling is not needed
   */
  get direct() {
    return apiClient
  }
}

// Create and export a singleton instance
const offlineApi = new OfflineApiService()

export default offlineApi
export { OfflineApiService }
