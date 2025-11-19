<template>
  <div class="checkout-page">
    <div class="container">
      <h1>📋 Оформление заказа</h1>
      
      <div v-if="loading" class="loading">
        <div class="spinner"></div>
        <p>Загрузка...</p>
      </div>

      <div v-else-if="cartItems.length === 0" class="empty-cart">
        <div class="empty-icon">🛒</div>
        <p>Ваша корзина пуста</p>
        <router-link to="/catalog" class="btn-primary">Перейти в каталог</router-link>
      </div>

      <div v-else class="checkout-content">
        <div class="checkout-form-section">
          <h2>📝 Данные для доставки</h2>
          <form @submit.prevent="submitOrder" class="checkout-form">
            <div class="form-group">
              <label>👤 Имя получателя *</label>
              <input 
                type="text" 
                v-model="orderForm.name" 
                required 
                placeholder="Введите ваше имя"
              />
            </div>
            <div class="form-group">
              <label>📧 Email *</label>
              <input 
                type="email" 
                v-model="orderForm.email" 
                required 
                placeholder="Введите email"
              />
            </div>
            <div class="form-group">
              <label>📱 Телефон *</label>
              <input 
                type="tel" 
                v-model="orderForm.phone" 
                required 
                placeholder="+7 (999) 123-45-67"
              />
            </div>
            <div class="form-group">
              <label>📍 Адрес доставки *</label>
              <textarea 
                v-model="orderForm.address" 
                required 
                placeholder="Введите полный адрес доставки"
                rows="3"
              ></textarea>
            </div>
            <div v-if="error" class="error-message">{{ error }}</div>
            <button type="submit" :disabled="submitting" class="btn-submit">
              {{ submitting ? '⏳ Оформление...' : '✅ Оформить заказ' }}
            </button>
          </form>
        </div>

        <div class="checkout-summary-section">
          <h2>🛒 Состав заказа</h2>
          <div class="order-items">
            <div v-for="item in cartItems" :key="item.id" class="order-item">
              <div class="item-image">
                <img 
                  v-if="item.product?.imageUrl && !isImageError(item.id)" 
                  :src="item.product.imageUrl" 
                  :alt="item.product.title"
                  @error="setImageError(item.id)"
                />
                <div v-else class="no-image">
                  <div class="image-placeholder">
                    <span>{{ item.product?.title?.charAt(0) || '?' }}</span>
                  </div>
                </div>
              </div>
              <div class="item-details">
                <h3>{{ item.product?.title }}</h3>
                <div class="item-quantity-price">
                  <span>Количество: {{ item.quantity }} шт.</span>
                  <span class="item-price">{{ formatPrice(item.product?.price || 0) }} ₽</span>
                </div>
              </div>
              <div class="item-total">
                {{ formatPrice((item.product?.price || 0) * item.quantity) }} ₽
              </div>
            </div>
          </div>
          <div class="order-summary">
            <div class="summary-row">
              <span>Товаров:</span>
              <span>{{ totalItems }} шт.</span>
            </div>
            <div class="summary-row total">
              <span>Итого:</span>
              <span>{{ formatPrice(totalAmount) }} ₽</span>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script>
import { api } from '../services/api'

export default {
  name: 'Checkout',
  data() {
    return {
      cartItems: [],
      loading: true,
      submitting: false,
      error: '',
      orderForm: {
        name: '',
        email: '',
        phone: '',
        address: ''
      },
      imageErrors: new Set()
    }
  },
  computed: {
    totalAmount() {
      return this.cartItems.reduce((sum, item) => {
        return sum + (item.product?.price || 0) * item.quantity
      }, 0)
    },
    totalItems() {
      return this.cartItems.reduce((sum, item) => sum + item.quantity, 0)
    }
  },
  async mounted() {
    await this.loadCart()
    this.loadUserData()
  },
  methods: {
    async loadCart() {
      this.loading = true
      try {
        const response = await api.get('/cart')
        this.cartItems = response.data
      } catch (error) {
        console.error('Ошибка загрузки корзины:', error)
        if (error.response?.status === 401) {
          this.$router.push('/auth')
        }
      } finally {
        this.loading = false
      }
    },
    loadUserData() {
      const userStr = localStorage.getItem('user')
      if (userStr) {
        try {
          const user = JSON.parse(userStr)
          this.orderForm.name = user.userName || ''
          this.orderForm.email = user.email || ''
        } catch (e) {
          console.error('Ошибка загрузки данных пользователя:', e)
        }
      }
    },
    async submitOrder() {
      this.error = ''
      this.submitting = true

      try {
        const response = await api.post('/orders', this.orderForm)
        
        if (response.data && response.data.id) {
          this.$root.$toast?.success(`Заказ успешно оформлен! Номер заказа: #${response.data.id}`)
          this.$router.push('/cart')
        } else {
          this.error = 'Неверный формат ответа от сервера'
        }
      } catch (error) {
        console.error('Ошибка оформления заказа:', error)
        if (error.response?.data?.message) {
          this.error = error.response.data.message
        } else {
          this.error = 'Ошибка оформления заказа. Попробуйте снова.'
        }
      } finally {
        this.submitting = false
      }
    },
    formatPrice(price) {
      return new Intl.NumberFormat('ru-RU').format(price)
    },
    setImageError(itemId) {
      this.imageErrors.add(itemId)
    },
    isImageError(itemId) {
      return this.imageErrors.has(itemId)
    }
  }
}
</script>

<style scoped>
.checkout-page {
  min-height: calc(100vh - 140px);
  padding: 2rem 0;
}

.checkout-page h1 {
  text-align: center;
  margin-bottom: 2.5rem;
  font-size: 2.8rem;
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
  -webkit-background-clip: text;
  -webkit-text-fill-color: transparent;
  background-clip: text;
  font-weight: 700;
}

.loading {
  text-align: center;
  padding: 4rem;
}

.spinner {
  width: 50px;
  height: 50px;
  border: 4px solid rgba(102, 126, 234, 0.1);
  border-top-color: #667eea;
  border-radius: 50%;
  animation: spin 1s linear infinite;
  margin: 0 auto 1rem;
}

@keyframes spin {
  to { transform: rotate(360deg); }
}

.empty-cart {
  text-align: center;
  padding: 4rem;
}

.empty-icon {
  font-size: 5rem;
  margin-bottom: 1rem;
  opacity: 0.5;
}

.empty-cart p {
  font-size: 1.2rem;
  color: #666;
  margin-bottom: 1.5rem;
}

.checkout-content {
  display: grid;
  grid-template-columns: 1fr 1fr;
  gap: 2rem;
}

.checkout-form-section,
.checkout-summary-section {
  background: white;
  padding: 2.5rem;
  border-radius: 20px;
  box-shadow: 0 8px 30px rgba(0,0,0,0.12);
  border: 1px solid rgba(102, 126, 234, 0.1);
}

.checkout-form-section h2,
.checkout-summary-section h2 {
  color: #667eea;
  margin-bottom: 1.5rem;
  font-size: 1.5rem;
}

.checkout-form {
  display: flex;
  flex-direction: column;
  gap: 1.5rem;
}

.form-group {
  display: flex;
  flex-direction: column;
}

.form-group label {
  margin-bottom: 0.5rem;
  font-weight: 600;
  color: #333;
}

.form-group input,
.form-group textarea {
  padding: 0.9rem;
  border: 2px solid #e0e0e0;
  border-radius: 10px;
  font-size: 1rem;
  transition: all 0.3s;
  font-family: inherit;
}

.form-group input:focus,
.form-group textarea:focus {
  outline: none;
  border-color: #667eea;
  box-shadow: 0 0 0 3px rgba(102, 126, 234, 0.1);
}

.form-group textarea {
  resize: vertical;
  min-height: 80px;
}

.btn-submit {
  padding: 1.2rem;
  background: linear-gradient(135deg, #27ae60 0%, #229954 100%);
  color: white;
  border: none;
  border-radius: 12px;
  font-size: 1.1rem;
  font-weight: bold;
  cursor: pointer;
  transition: all 0.3s;
  box-shadow: 0 4px 15px rgba(39, 174, 96, 0.3);
  margin-top: 0.5rem;
}

.btn-submit:hover:not(:disabled) {
  transform: translateY(-2px);
  box-shadow: 0 6px 20px rgba(39, 174, 96, 0.4);
}

.btn-submit:disabled {
  opacity: 0.6;
  cursor: not-allowed;
}

.error-message {
  color: #e74c3c;
  padding: 0.75rem;
  background: #fee;
  border-radius: 5px;
  text-align: center;
}

.order-items {
  display: flex;
  flex-direction: column;
  gap: 1rem;
  margin-bottom: 1.5rem;
  max-height: 400px;
  overflow-y: auto;
}

.order-item {
  display: flex;
  gap: 1rem;
  padding: 1rem;
  background: linear-gradient(135deg, #f8f9fa 0%, #ffffff 100%);
  border-radius: 10px;
  border: 1px solid rgba(102, 126, 234, 0.1);
}

.item-image {
  width: 80px;
  height: 80px;
  border-radius: 8px;
  overflow: hidden;
  background: #f5f5f5;
  flex-shrink: 0;
}

.item-image img {
  width: 100%;
  height: 100%;
  object-fit: cover;
}

.no-image {
  width: 100%;
  height: 100%;
  display: flex;
  align-items: center;
  justify-content: center;
}

.image-placeholder {
  width: 100%;
  height: 100%;
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
  display: flex;
  align-items: center;
  justify-content: center;
  color: white;
  font-size: 1.5rem;
  font-weight: bold;
}

.item-details {
  flex: 1;
}

.item-details h3 {
  font-size: 1rem;
  margin-bottom: 0.5rem;
  color: #333;
}

.item-quantity-price {
  display: flex;
  flex-direction: column;
  gap: 0.25rem;
  font-size: 0.9rem;
  color: #666;
}

.item-price {
  color: #667eea;
  font-weight: 600;
}

.item-total {
  font-size: 1.2rem;
  font-weight: bold;
  color: #667eea;
  align-self: center;
}

.order-summary {
  padding-top: 1.5rem;
  border-top: 2px solid #eee;
}

.summary-row {
  display: flex;
  justify-content: space-between;
  padding: 0.75rem 0;
  font-size: 1rem;
}

.summary-row.total {
  font-size: 1.3rem;
  font-weight: bold;
  color: #667eea;
  border-top: 2px solid #667eea;
  margin-top: 0.5rem;
  padding-top: 1rem;
}

.btn-primary {
  display: inline-block;
  padding: 0.75rem 2rem;
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
  color: white;
  text-decoration: none;
  border-radius: 10px;
  font-weight: 500;
  transition: all 0.3s;
  box-shadow: 0 2px 8px rgba(102, 126, 234, 0.3);
}

.btn-primary:hover {
  transform: translateY(-2px);
  box-shadow: 0 4px 12px rgba(102, 126, 234, 0.4);
}

@media (max-width: 968px) {
  .checkout-content {
    grid-template-columns: 1fr;
  }
}
</style>

