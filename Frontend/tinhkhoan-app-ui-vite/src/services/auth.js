// src/services/auth.js
import api from './api'

export async function login(username, password) {
  try {
    const res = await api.post('/auth/login', { username, password })
    if (res.data && res.data.token) {
      // Lưu token
      localStorage.setItem('token', res.data.token)

      // Lưu thông tin user và thời gian login
      localStorage.setItem('username', username)
      localStorage.setItem('loginTime', new Date().toISOString())

      // Lưu thông tin user chi tiết nếu có
      if (res.data.user) {
        localStorage.setItem('user', JSON.stringify(res.data.user))
      }

      return res.data
    } else {
      throw new Error('Sai thông tin đăng nhập')
    }
  } catch (err) {
    throw new Error(err.response?.data?.message || 'Đăng nhập thất bại')
  }
}

export function logout() {
  // Xóa tất cả thông tin liên quan đến session
  localStorage.removeItem('token')
  localStorage.removeItem('username')
  localStorage.removeItem('loginTime')
  localStorage.removeItem('user')
}

export function getToken() {
  return localStorage.getItem('token')
}

export function isAuthenticated() {
  // 🔍 TEMPORARY: Always return true for debugging
  console.log('Auth check: Temporarily bypassed for debugging')
  return true

  /*
  const token = getToken();
  if (!token) {
    console.log('Auth check: No token found');
    return false;
  }

  try {
    // Kiểm tra format JWT
    const parts = token.split('.');
    if (parts.length !== 3) {
      console.log('Auth check: Invalid JWT format');
      localStorage.removeItem('token'); // Remove invalid token
      return false;
    }

    // Decode và kiểm tra expiration
    const payload = JSON.parse(atob(parts[1]));
    const now = Math.floor(Date.now() / 1000);

    if (payload.exp && payload.exp < now) {
      console.log('Auth check: Token expired', {
        exp: new Date(payload.exp * 1000),
        now: new Date()
      });
      localStorage.removeItem('token'); // Remove expired token
      return false;
    }

    console.log('Auth check: Token valid', {
      username: payload.username || payload.sub,
      exp: payload.exp ? new Date(payload.exp * 1000) : 'no expiration'
    });
    return true;
  } catch (error) {
    console.log('Auth check: Error validating token', error);
    localStorage.removeItem('token'); // Remove invalid token
    return false;
  }
  */
}

// Validate token with backend
export async function validateTokenWithBackend() {
  const token = getToken()
  if (!token) return false

  try {
    const response = await api.get('/auth/validate', {
      headers: { Authorization: `Bearer ${token}` },
    })
    return response.status === 200
  } catch (error) {
    console.log('Backend token validation failed:', error)
    return false
  }
}

// Get user info from token
export function getUserInfo() {
  const token = getToken()
  if (!token) return null

  try {
    const parts = token.split('.')
    if (parts.length !== 3) return null

    const payload = JSON.parse(atob(parts[1]))
    return {
      username: payload.username || payload.sub,
      role: payload.role,
      exp: payload.exp ? new Date(payload.exp * 1000) : null,
      iat: payload.iat ? new Date(payload.iat * 1000) : null,
    }
  } catch (error) {
    console.log('Error getting user info:', error)
    return null
  }
}
