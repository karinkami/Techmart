<template>
  <div id="app">
    <nav class="navbar">
      <div class="container">
        <router-link to="/" class="logo">TechMart</router-link>
        <div class="nav-links">
          <div class="main-nav">
            <router-link to="/">🏠 Главная</router-link>
            <router-link to="/catalog">📦 Каталог</router-link>
            <router-link to="/about">ℹ️ О нас</router-link>
          </div>
          <div class="user-nav">
            <template v-if="isAuthenticated">
              <router-link to="/cart" class="nav-link cart-link">
                <span class="nav-icon">🛒</span>
                <span>Корзина</span>
              </router-link>
              <router-link to="/profile" class="nav-link profile-link">
                <span class="nav-icon">👤</span>
                <span>Личный кабинет</span>
              </router-link>
            </template>
            <template v-else>
              <router-link to="/auth" class="nav-link login-link">
                <span class="nav-icon">🔐</span>
                <span>Войти / Регистрация</span>
              </router-link>
            </template>
          </div>
        </div>
      </div>
    </nav>
    <main class="main-content">
      <router-view />
    </main>
    <Toast ref="toast" />
    <ConfirmDialog ref="confirmDialog" />
    <footer class="footer">
      <div class="container">
        <p>&copy; 2025 TechMart. Все права защищены.</p>
      </div>
    </footer>
  </div>
</template>

<script>
import Toast from './components/Toast.vue'
import ConfirmDialog from './components/ConfirmDialog.vue'

export default {
  name: 'App',
  components: {
    Toast,
    ConfirmDialog
  },
  data() {
    return {
      isAuthenticated: !!localStorage.getItem('token')
    }
  },
  methods: {
    handleLogout() {
      localStorage.removeItem('token')
      localStorage.removeItem('user')
      this.isAuthenticated = false
      this.$router.push('/')
    },
    checkAuth() {
      this.isAuthenticated = !!localStorage.getItem('token')
    }
  },
  mounted() {
    // Делаем toast и confirmDialog доступными глобально
    this.$nextTick(() => {
      this.$root.$toast = this.$refs.toast
      this.$root.$confirm = this.$refs.confirmDialog
      this.$root.$checkAuth = this.checkAuth
      console.log('Компоненты инициализированы:', {
        toast: !!this.$root.$toast,
        confirm: !!this.$root.$confirm
      })
    })
    
    // Отслеживаем изменения в localStorage
    window.addEventListener('storage', this.checkAuth)
    // Отслеживаем кастомное событие для обновления аутентификации
    window.addEventListener('auth-changed', this.checkAuth)
    
    // Проверяем аутентификацию при монтировании
    this.checkAuth()
  },
  beforeUnmount() {
    window.removeEventListener('storage', this.checkAuth)
    window.removeEventListener('auth-changed', this.checkAuth)
  },
  watch: {
    '$route'() {
      // Обновляем состояние при изменении маршрута
      this.checkAuth()
    }
  }
}
</script>

<style>
* {
  margin: 0;
  padding: 0;
  box-sizing: border-box;
}

body {
  font-family: -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, Oxygen, Ubuntu, Cantarell, sans-serif;
  line-height: 1.6;
  color: #333;
  background: linear-gradient(135deg, #f5f7fa 0%, #c3cfe2 100%);
  min-height: 100vh;
}

.container {
  max-width: 1200px;
  margin: 0 auto;
  padding: 0 20px;
}

.navbar {
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
  color: white;
  padding: 1.2rem 0;
  box-shadow: 0 4px 6px rgba(0,0,0,0.1);
  position: sticky;
  top: 0;
  z-index: 1000;
}

.navbar .container {
  display: flex;
  justify-content: space-between;
  align-items: center;
}

.logo {
  font-size: 1.8rem;
  font-weight: bold;
  color: white;
  text-decoration: none;
  transition: transform 0.3s;
}

.logo:hover {
  transform: scale(1.05);
}

.nav-links {
  display: flex;
  align-items: center;
  gap: 1.5rem;
}

.main-nav {
  display: flex;
  gap: 1rem;
  align-items: center;
  padding-right: 1.5rem;
  border-right: 2px solid rgba(255, 255, 255, 0.3);
}

.user-nav {
  display: flex;
  gap: 1rem;
  align-items: center;
}

.nav-links a {
  color: white;
  text-decoration: none;
  transition: all 0.3s;
  padding: 0.6rem 1.2rem;
  border-radius: 8px;
  font-weight: 500;
  display: flex;
  align-items: center;
  gap: 0.5rem;
  white-space: nowrap;
}

.nav-links a:hover,
.nav-links a.router-link-active {
  background: rgba(255, 255, 255, 0.2);
  transform: translateY(-2px);
  box-shadow: 0 2px 8px rgba(0,0,0,0.15);
}

.nav-icon {
  font-size: 1.2rem;
  display: inline-flex;
  align-items: center;
  justify-content: center;
}

.cart-link {
  background: rgba(255, 193, 7, 0.2);
  border: 2px solid rgba(255, 193, 7, 0.4);
}

.cart-link:hover {
  background: rgba(255, 193, 7, 0.3);
  box-shadow: 0 4px 12px rgba(255, 193, 7, 0.3);
}

.profile-link {
  background: rgba(76, 175, 80, 0.2);
  border: 2px solid rgba(76, 175, 80, 0.4);
}

.profile-link:hover {
  background: rgba(76, 175, 80, 0.3);
  box-shadow: 0 4px 12px rgba(76, 175, 80, 0.3);
}

.login-link {
  background: rgba(255, 255, 255, 0.2);
  border: 2px solid rgba(255, 255, 255, 0.4);
  backdrop-filter: blur(10px);
}

.login-link:hover {
  background: rgba(255, 255, 255, 0.3);
  transform: translateY(-2px);
  box-shadow: 0 4px 8px rgba(0,0,0,0.2);
}

@media (max-width: 768px) {
  .nav-links {
    flex-direction: column;
    gap: 0.5rem;
    width: 100%;
  }
  
  .main-nav {
    border-right: none;
    border-bottom: 2px solid rgba(255, 255, 255, 0.3);
    padding-right: 0;
    padding-bottom: 1rem;
    width: 100%;
    justify-content: center;
  }
  
  .user-nav {
    width: 100%;
    justify-content: center;
  }
  
  .nav-links a {
    width: 100%;
    justify-content: center;
  }
}

.main-content {
  min-height: calc(100vh - 140px);
  padding: 3rem 0;
  background: transparent;
}

.footer {
  background: linear-gradient(135deg, #2c3e50 0%, #34495e 100%);
  color: white;
  text-align: center;
  padding: 2rem 0;
  margin-top: auto;
  box-shadow: 0 -2px 10px rgba(0,0,0,0.1);
}

#app {
  display: flex;
  flex-direction: column;
  min-height: 100vh;
}
</style>

