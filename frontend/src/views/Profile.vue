<template>
  <div class="profile-page">
    <div class="container">
      <h1>Личный кабинет</h1>
      
      <div v-if="user" class="profile-card">
        <div class="profile-header">
          <h2>Добро пожаловать, {{ user.userName }}!</h2>
        </div>
        
        <div v-if="!isEditing" class="profile-info">
          <div class="info-item">
            <span class="label">Имя пользователя:</span>
            <span class="value">{{ user.userName }}</span>
          </div>
          <div class="info-item">
            <span class="label">Email:</span>
            <span class="value">{{ user.email }}</span>
          </div>
          <div class="info-item">
            <span class="label">Дата регистрации:</span>
            <span class="value">{{ formatDate(user.createdAt) }}</span>
          </div>
          <div class="profile-actions">
            <router-link to="/orders" class="btn-orders">📦 Мои заказы</router-link>
            <button @click="startEdit" class="btn-edit">Редактировать профиль</button>
            <button @click="handleLogout" class="btn-logout">Выйти из аккаунта</button>
          </div>
        </div>

        <div v-else class="edit-form">
          <h3>Редактирование профиля</h3>
          <div class="form-group">
            <label>Имя пользователя</label>
            <input type="text" v-model="editForm.userName" />
          </div>
          <div class="form-group">
            <label>Email</label>
            <input type="email" v-model="editForm.email" />
          </div>
          <div v-if="error" class="error-message">{{ error }}</div>
          <div v-if="success" class="success-message">{{ success }}</div>
          <div class="form-actions">
            <button @click="saveProfile" :disabled="loading" class="btn-save">
              {{ loading ? 'Сохранение...' : 'Сохранить' }}
            </button>
            <button @click="cancelEdit" class="btn-cancel">Отмена</button>
          </div>
        </div>
      </div>

      <div v-else class="loading">Загрузка...</div>
    </div>
  </div>
</template>

<script>
import { api } from '../services/api'

export default {
  name: 'Profile',
  data() {
    return {
      user: null,
      isEditing: false,
      editForm: {
        userName: '',
        email: ''
      },
      error: '',
      success: '',
      loading: false
    }
  },
  async mounted() {
    await this.loadUser()
  },
  methods: {
    async loadUser() {
      const userStr = localStorage.getItem('user')
      if (userStr) {
        this.user = JSON.parse(userStr)
        // Загружаем актуальные данные с сервера
        try {
          const response = await api.get('/users/me')
          this.user = { ...response.data, createdAt: this.user.createdAt }
          localStorage.setItem('user', JSON.stringify(this.user))
        } catch (error) {
          console.error('Ошибка загрузки данных пользователя:', error)
        }
      } else {
        this.$router.push('/auth')
      }
    },
    startEdit() {
      this.editForm.userName = this.user.userName
      this.editForm.email = this.user.email
      this.isEditing = true
      this.error = ''
      this.success = ''
    },
    cancelEdit() {
      this.isEditing = false
      this.error = ''
      this.success = ''
    },
    async saveProfile() {
      this.error = ''
      this.success = ''
      
      // Валидация
      if (!this.editForm.userName || !this.editForm.email) {
        this.error = 'Все поля обязательны для заполнения'
        return
      }

      if (!this.editForm.email.includes('@')) {
        this.error = 'Введите корректный email'
        return
      }

      this.loading = true

      try {
        const response = await api.put('/users/me', {
          userName: this.editForm.userName.trim(),
          email: this.editForm.email.trim()
        })

        // Обновляем данные пользователя
        this.user = { ...this.user, ...response.data }
        localStorage.setItem('user', JSON.stringify(this.user))
        
        this.success = 'Профиль успешно обновлен!'
        this.isEditing = false
        
        // Обновляем данные на сервере еще раз для уверенности
        try {
          const updatedUser = await api.get('/users/me')
          this.user = { ...updatedUser.data, createdAt: this.user.createdAt }
          localStorage.setItem('user', JSON.stringify(this.user))
        } catch (e) {
          console.error('Ошибка обновления данных:', e)
        }
        
        setTimeout(() => {
          this.success = ''
        }, 3000)
      } catch (error) {
        this.error = error.response?.data?.message || 'Ошибка обновления профиля. Попробуйте снова.'
      } finally {
        this.loading = false
      }
    },
    handleLogout() {
      localStorage.removeItem('token')
      localStorage.removeItem('user')
      this.$router.push('/')
    },
    formatDate(dateString) {
      if (!dateString) return 'Не указано'
      const date = new Date(dateString)
      return date.toLocaleDateString('ru-RU', {
        year: 'numeric',
        month: 'long',
        day: 'numeric'
      })
    }
  }
}
</script>

<style scoped>
.profile-page {
  min-height: calc(100vh - 140px);
  padding: 2rem 0;
}

.profile-page h1 {
  text-align: center;
  margin-bottom: 2.5rem;
  font-size: 2.8rem;
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
  -webkit-background-clip: text;
  -webkit-text-fill-color: transparent;
  background-clip: text;
  font-weight: 700;
}

.profile-card {
  background: white;
  padding: 2.5rem;
  border-radius: 20px;
  box-shadow: 0 8px 30px rgba(0,0,0,0.12);
  max-width: 700px;
  margin: 0 auto;
  border: 1px solid rgba(102, 126, 234, 0.1);
}

.profile-header {
  margin-bottom: 2rem;
  padding-bottom: 1rem;
  border-bottom: 2px solid #f0f0f0;
}

.profile-header h2 {
  color: #667eea;
  margin: 0;
}

.profile-actions {
  display: flex;
  gap: 1rem;
  margin-top: 2rem;
  flex-wrap: wrap;
}

.btn-logout {
  padding: 0.75rem 2rem;
  background: linear-gradient(135deg, #e74c3c 0%, #c0392b 100%);
  color: white;
  border: none;
  border-radius: 10px;
  cursor: pointer;
  font-size: 1rem;
  font-weight: 600;
  transition: all 0.3s;
  flex: 1;
  min-width: 200px;
  box-shadow: 0 4px 15px rgba(231, 76, 60, 0.3);
}

.btn-logout:hover {
  transform: translateY(-2px);
  box-shadow: 0 6px 20px rgba(231, 76, 60, 0.4);
}

.profile-info {
  display: flex;
  flex-direction: column;
  gap: 1.5rem;
}

.info-item {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 1.2rem;
  background: linear-gradient(135deg, #f8f9fa 0%, #ffffff 100%);
  border-radius: 10px;
  border: 1px solid rgba(102, 126, 234, 0.1);
  transition: all 0.3s;
}

.info-item:hover {
  transform: translateX(5px);
  box-shadow: 0 2px 10px rgba(102, 126, 234, 0.1);
}

.label {
  font-weight: 600;
  color: #666;
}

.value {
  color: #333;
  font-size: 1.1rem;
}

.loading {
  text-align: center;
  padding: 3rem;
  font-size: 1.2rem;
  color: #666;
}

.btn-orders {
  display: inline-block;
  padding: 0.75rem 2rem;
  background: linear-gradient(135deg, #27ae60 0%, #229954 100%);
  color: white;
  text-decoration: none;
  border-radius: 10px;
  font-size: 1rem;
  font-weight: 600;
  transition: all 0.3s;
  flex: 1;
  min-width: 200px;
  box-shadow: 0 4px 15px rgba(39, 174, 96, 0.3);
  text-align: center;
}

.btn-orders:hover {
  transform: translateY(-2px);
  box-shadow: 0 6px 20px rgba(39, 174, 96, 0.4);
}

.btn-edit {
  padding: 0.75rem 2rem;
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
  color: white;
  border: none;
  border-radius: 10px;
  cursor: pointer;
  font-size: 1rem;
  font-weight: 600;
  transition: all 0.3s;
  flex: 1;
  min-width: 200px;
  box-shadow: 0 4px 15px rgba(102, 126, 234, 0.3);
}

.btn-edit:hover {
  transform: translateY(-2px);
  box-shadow: 0 6px 20px rgba(102, 126, 234, 0.4);
}

.edit-form {
  margin-top: 1rem;
}

.edit-form h3 {
  margin-bottom: 1.5rem;
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

.form-actions {
  display: flex;
  gap: 1rem;
  margin-top: 1.5rem;
}

.btn-save {
  padding: 0.75rem 2rem;
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
  color: white;
  border: none;
  border-radius: 10px;
  cursor: pointer;
  font-size: 1rem;
  font-weight: 600;
  transition: all 0.3s;
  box-shadow: 0 4px 15px rgba(102, 126, 234, 0.3);
}

.btn-save:hover:not(:disabled) {
  transform: translateY(-2px);
  box-shadow: 0 6px 20px rgba(102, 126, 234, 0.4);
}

.btn-save:disabled {
  opacity: 0.6;
  cursor: not-allowed;
}

.btn-cancel {
  padding: 0.75rem 2rem;
  background: #95a5a6;
  color: white;
  border: none;
  border-radius: 10px;
  cursor: pointer;
  font-size: 1rem;
  font-weight: 600;
  transition: all 0.3s;
  box-shadow: 0 2px 8px rgba(149, 165, 166, 0.3);
}

.btn-cancel:hover {
  background: #7f8c8d;
  transform: translateY(-2px);
  box-shadow: 0 4px 12px rgba(149, 165, 166, 0.4);
}

.error-message {
  color: #e74c3c;
  margin-bottom: 1rem;
  padding: 0.75rem;
  background: #fee;
  border-radius: 5px;
}

.success-message {
  color: #27ae60;
  margin-bottom: 1rem;
  padding: 0.75rem;
  background: #efe;
  border-radius: 5px;
}
</style>

