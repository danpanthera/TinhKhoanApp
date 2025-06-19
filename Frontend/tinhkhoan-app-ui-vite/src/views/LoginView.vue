<template>
  <div class="login-container">
    <form @submit.prevent="handleLogin" class="login-form">
      <h2>Đăng nhập hệ thống</h2>
      <div>
        <label for="username">Tên đăng nhập</label>
        <input v-model="username" id="username" type="text" required />
      </div>
      <div>
        <label for="password">Mật khẩu</label>
        <input v-model="password" id="password" type="password" required />
      </div>
      <div v-if="error" class="error">{{ error }}</div>
      <button type="submit" :disabled="loading">Đăng nhập</button>
    </form>
  </div>
</template>

<script setup>
import { ref } from 'vue';
import { useRouter } from 'vue-router';
import { login } from '../services/auth';

const username = ref('');
const password = ref('');
const error = ref('');
const loading = ref(false);
const router = useRouter();

const handleLogin = async () => {
  error.value = '';
  loading.value = true;
  try {
    await login(username.value, password.value);
    router.push('/');
  } catch (e) {
    error.value = e.message || 'Đăng nhập thất bại';
  } finally {
    loading.value = false;
  }
};
</script>

<style scoped>
.login-container {
  max-width: 350px;
  margin: 80px auto;
  padding: 32px 24px;
  background: #fff;
  border-radius: 8px;
  box-shadow: 0 2px 12px rgba(0,0,0,0.08);
}
.login-form h2 {
  margin-bottom: 18px;
  text-align: center;
}
.login-form label {
  display: block;
  margin-bottom: 4px;
}
.login-form input {
  width: 100%;
  padding: 8px;
  margin-bottom: 16px;
  border: 1px solid #ccc;
  border-radius: 4px;
}
.login-form button {
  width: 100%;
  padding: 10px;
  background: #2d8cf0;
  color: #fff;
  border: none;
  border-radius: 4px;
  font-weight: bold;
  cursor: pointer;
}
.login-form button:disabled {
  background: #b3d8fd;
}
.error {
  color: #e74c3c;
  margin-bottom: 12px;
  text-align: center;
}
</style>
