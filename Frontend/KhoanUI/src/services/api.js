import axios from 'axios'

// Prefer runtime-defined base URL via global, then env, then Vite proxy '/api'
const runtimeBase = (typeof window !== 'undefined' && window.__API_BASE_URL__) ? window.__API_BASE_URL__ : null
const envBase = (typeof import.meta !== 'undefined' && import.meta.env && import.meta.env.VITE_API_BASE) ? import.meta.env.VITE_API_BASE : null
const baseURL = runtimeBase || envBase || '/api'

const api = axios.create({ baseURL, timeout: 120000 })
export default api
