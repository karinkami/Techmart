<template>
  <div class="auth-page">
    <div class="container">
      <div class="auth-card">
        <div class="auth-tabs">
          <button 
            @click="isLogin = true" 
            :class="['tab', { active: isLogin }]"
          >
            Вход
          </button>
          <button 
            @click="isLogin = false" 
            :class="['tab', { active: !isLogin }]"
          >
            Регистрация
          </button>
        </div>

        <h1>{{ isLogin ? '🔐 Вход' : '✨ Регистрация' }}</h1>

        <!-- Форма входа -->
        <form v-if="isLogin" @submit.prevent="handleLogin">
          <div class="form-group">
            <label>📧 Email</label>
            <input 
              type="email" 
              v-model="loginForm.email" 
              required 
              placeholder="Введите email"
            />
          </div>
          <div class="form-group">
            <label>🔒 Пароль</label>
            <input 
              type="password" 
              v-model="loginForm.password" 
              required 
              placeholder="Введите пароль"
            />
          </div>
          <div v-if="error" class="error-message">{{ error }}</div>
          <div v-if="success" class="success-message">{{ success }}</div>
          <button type="submit" :disabled="loading" class="btn-primary">
            {{ loading ? 'Вход...' : 'Войти' }}
          </button>
        </form>

        <!-- Форма регистрации -->
        <form v-else @submit.prevent="handleRegister">
          <div class="form-group">
            <label>👤 Имя пользователя</label>
            <input 
              type="text" 
              v-model="registerForm.userName" 
              required 
              placeholder="Введите имя"
            />
          </div>
          <div class="form-group">
            <label>📧 Email</label>
            <input 
              type="email" 
              v-model="registerForm.email" 
              required 
              placeholder="Введите email"
            />
          </div>
          <div class="form-group">
            <label>🔒 Пароль</label>
            <input 
              type="password" 
              v-model="registerForm.password" 
              required 
              placeholder="Минимум 6 символов"
              minlength="6"
            />
          </div>
          <div v-if="error" class="error-message">{{ error }}</div>
          <button type="submit" :disabled="loading" class="btn-primary">
            {{ loading ? 'Регистрация...' : 'Зарегистрироваться' }}
          </button>
        </form>
      </div>
    </div>
  </div>
</template>

<script>
import { api } from '../services/api'

export default {
  name: 'Auth',
  data() {
    return {
      isLogin: true,
      loginForm: {
        email: '',
        password: ''
      },
      registerForm: {
        userName: '',
        email: '',
        password: ''
      },
      error: '',
      success: '',
      loading: false
    }
  },
  watch: {
    isLogin() {
      // Очищаем ошибки при переключении
      this.error = ''
      this.success = ''
    }
  },
  mounted() {
    // Проверяем, есть ли сообщение об успешной регистрации
    const registered = sessionStorage.getItem('registered')
    if (registered) {
      this.isLogin = true
      this.success = 'Регистрация успешна! Войдите в систему.'
      sessionStorage.removeItem('registered')
    }
  },
  methods: {
    async handleLogin() {
      this.error = ''
      this.success = ''
      this.loading = true

      if (!this.loginForm.email || !this.loginForm.password) {
        this.error = 'Заполните все поля'
        this.loading = false
        return
      }

      try {
        const response = await api.post('/auth/login', {
          email: this.loginForm.email.trim(),
          password: this.loginForm.password
        })

        if (response.data && response.data.token && response.data.user) {
          localStorage.setItem('token', response.data.token)
          localStorage.setItem('user', JSON.stringify(response.data.user))
          // Обновляем состояние в App.vue
          if (this.$root.$checkAuth) {
            this.$root.$checkAuth()
          } else {
            // Fallback: используем кастомное событие
            window.dispatchEvent(new Event('auth-changed'))
          }
          // Небольшая задержка для обновления состояния
          await this.$nextTick()
          this.$router.push('/profile')
        } else {
          this.error = 'Неверный формат ответа от сервера'
        }
      } catch (error) {
        console.error('Login error:', error)
        
        if (error.response) {
          this.error = error.response.data?.message || 'Ошибка входа. Проверьте email и пароль.'
        } else if (error.request) {
          this.error = 'Не удалось подключиться к серверу. Проверьте, что backend запущен.'
        } else {
          this.error = 'Ошибка при отправке запроса. Попробуйте снова.'
        }
      } finally {
        this.loading = false
      }
    },
    async handleRegister() {
      this.error = ''
      this.loading = true

      if (!this.registerForm.userName || !this.registerForm.email || !this.registerForm.password) {
        this.error = 'Заполните все поля'
        this.loading = false
        return
      }

      if (this.registerForm.password.length < 6) {
        this.error = 'Пароль должен содержать минимум 6 символов'
        this.loading = false
        return
      }

      if (!this.registerForm.email.includes('@')) {
        this.error = 'Введите корректный email'
        this.loading = false
        return
      }

      try {
        const response = await api.post('/auth/register', {
          userName: this.registerForm.userName.trim(),
          email: this.registerForm.email.trim(),
          password: this.registerForm.password
        })

        if (response.data) {
          sessionStorage.setItem('registered', 'true')
          this.isLogin = true
          this.success = 'Регистрация успешна! Войдите в систему.'
          // Очищаем форму регистрации
          this.registerForm = {
            userName: '',
            email: '',
            password: ''
          }
        } else {
          this.error = 'Ошибка регистрации. Попробуйте снова.'
        }
      } catch (error) {
        console.error('Register error:', error)
        
        if (error.response) {
          this.error = error.response.data?.message || 'Ошибка регистрации. Попробуйте снова.'
        } else if (error.request) {
          this.error = 'Не удалось подключиться к серверу. Проверьте, что backend запущен.'
        } else {
          this.error = 'Ошибка при отправке запроса. Попробуйте снова.'
        }
      } finally {
        this.loading = false
      }
    }
  }
}
</script>

<style scoped>
.auth-page {
  min-height: calc(100vh - 140px);
  display: flex;
  align-items: center;
  justify-content: center;
  padding: 2rem 0;
}

.auth-card {
  background: white;
  padding: 2.5rem;
  border-radius: 20px;
  box-shadow: 0 8px 30px rgba(0,0,0,0.12);
  max-width: 450px;
  width: 100%;
  border: 1px solid rgba(102, 126, 234, 0.1);
}

.auth-tabs {
  display: flex;
  gap: 0.5rem;
  margin-bottom: 2rem;
  border-bottom: 2px solid #f0f0f0;
}

.tab {
  flex: 1;
  padding: 1rem;
  background: none;
  border: none;
  border-bottom: 3px solid transparent;
  cursor: pointer;
  font-size: 1rem;
  font-weight: 600;
  color: #999;
  transition: all 0.3s;
  position: relative;
}

.tab:hover {
  color: #667eea;
  background: rgba(102, 126, 234, 0.05);
}

.tab.active {
  color: #667eea;
  border-bottom-color: #667eea;
  background: rgba(102, 126, 234, 0.08);
}

.auth-card h1 {
  text-align: center;
  margin-bottom: 2rem;
  color: #667eea;
}

.form-group {
  margin-bottom: 1.5rem;
}

.form-group label {
  display: block;
  margin-bottom: 0.5rem;
  color: #333;
  font-weight: 500;
}

.form-group input {
  width: 100%;
  padding: 0.9rem;
  border: 2px solid #e0e0e0;
  border-radius: 10px;
  font-size: 1rem;
  box-sizing: border-box;
  transition: all 0.3s;
}

.form-group input:focus {
  outline: none;
  border-color: #667eea;
  box-shadow: 0 0 0 3px rgba(102, 126, 234, 0.1);
  transform: translateY(-1px);
}

.btn-primary {
  width: 100%;
  padding: 1rem;
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
  color: white;
  border: none;
  border-radius: 10px;
  font-size: 1rem;
  font-weight: bold;
  cursor: pointer;
  transition: all 0.3s;
  box-shadow: 0 4px 15px rgba(102, 126, 234, 0.3);
}

.btn-primary:hover:not(:disabled) {
  transform: translateY(-2px);
  box-shadow: 0 6px 20px rgba(102, 126, 234, 0.4);
}

.btn-primary:disabled {
  opacity: 0.6;
  cursor: not-allowed;
}

.error-message {
  color: #e74c3c;
  margin-bottom: 1rem;
  padding: 0.75rem;
  background: #fee;
  border-radius: 5px;
  text-align: center;
}

.success-message {
  color: #27ae60;
  margin-bottom: 1rem;
  padding: 0.75rem;
  background: #efe;
  border-radius: 5px;
  text-align: center;
}
</style>

