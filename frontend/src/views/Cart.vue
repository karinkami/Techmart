<template>
  <div class="cart-page">
    <div class="container">
      <h1>Корзина</h1>
      
      <div v-if="loading" class="loading">
        <div class="spinner"></div>
        <p>Загрузка корзины...</p>
      </div>
      
      <div v-else-if="cartItems.length === 0" class="empty-cart">
        <div class="empty-icon">🛒</div>
        <p>Ваша корзина пуста</p>
        <p class="empty-hint">Добавьте товары из каталога</p>
        <router-link to="/catalog" class="btn-primary">Перейти в каталог</router-link>
      </div>
      
      <div v-else class="cart-content">
        <div class="cart-items">
          <div v-for="item in cartItems" :key="item.id" class="cart-item">
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
            <div class="item-info">
              <h3>{{ item.product?.title }}</h3>
              <p class="item-description">{{ item.product?.description }}</p>
              <div class="item-price">{{ formatPrice(item.product?.price || 0) }} ₽</div>
            </div>
            <div class="item-quantity">
              <button @click="updateQuantity(item.id, item.quantity - 1)" :disabled="item.quantity <= 1" title="Уменьшить">−</button>
              <span>{{ item.quantity }}</span>
              <button @click="updateQuantity(item.id, item.quantity + 1)" title="Увеличить">+</button>
            </div>
            <div class="item-total">
              {{ formatPrice((item.product?.price || 0) * item.quantity) }} ₽
            </div>
            <button @click="removeItem(item.id)" class="btn-remove" title="Удалить">🗑️</button>
          </div>
        </div>
        
        <div class="cart-summary">
          <h2>📋 Итого</h2>
          <div class="summary-row">
            <span>Товаров:</span>
            <span class="summary-value">{{ totalItems }} шт.</span>
          </div>
          <div class="summary-row total">
            <span>Сумма:</span>
            <span class="summary-value">{{ formatPrice(totalAmount) }} ₽</span>
          </div>
          <button @click="clearCart" class="btn-clear">🗑️ Очистить корзину</button>
          <button @click="checkout" class="btn-checkout">✅ Оформить заказ</button>
        </div>
      </div>
    </div>
  </div>
</template>

<script>
import { api } from '../services/api'

export default {
  name: 'Cart',
  data() {
    return {
      cartItems: [],
      loading: true,
      imageErrors: new Set()
    }
  },
  computed: {
    totalItems() {
      return this.cartItems.reduce((sum, item) => sum + item.quantity, 0)
    },
    totalAmount() {
      return this.cartItems.reduce((sum, item) => {
        return sum + (item.product?.price || 0) * item.quantity
      }, 0)
    }
  },
  async mounted() {
    await this.loadCart()
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
    async updateQuantity(itemId, newQuantity) {
      if (newQuantity < 1) return
      
      try {
        await api.put(`/cart/${itemId}`, {
          productId: 0,
          quantity: newQuantity
        })
        await this.loadCart()
      } catch (error) {
        console.error('Ошибка обновления количества:', error)
        this.$root.$toast?.error('Ошибка обновления количества')
      }
    },
    async removeItem(itemId) {
      try {
        // Проверяем наличие компонента подтверждения
        if (!this.$root.$confirm) {
          console.error('Компонент подтверждения не найден')
          // Fallback на стандартный confirm
          if (!window.confirm('Удалить товар из корзины?')) return
        } else {
          const confirmed = await this.$root.$confirm.show(
            'Удаление товара',
            'Удалить товар из корзины?',
            'Удалить'
          )
          if (!confirmed) return
        }
        
        const response = await api.delete(`/cart/${itemId}`)
        console.log('Товар удален, ответ сервера:', response.status)
        await this.loadCart()
        
        if (this.$root.$toast) {
          this.$root.$toast.success('Товар удален из корзины')
        }
      } catch (error) {
        console.error('Ошибка удаления товара:', error)
        console.error('Детали ошибки:', error.response?.data || error.message)
        if (this.$root.$toast) {
          this.$root.$toast.error('Ошибка удаления товара')
        }
      }
    },
    async clearCart() {
      try {
        // Проверяем наличие компонента подтверждения
        if (!this.$root.$confirm) {
          console.error('Компонент подтверждения не найден')
          // Fallback на стандартный confirm
          if (!window.confirm('Очистить всю корзину?')) return
        } else {
          const confirmed = await this.$root.$confirm.show(
            'Очистка корзины',
            'Очистить всю корзину?',
            'Очистить'
          )
          if (!confirmed) return
        }
        
        const response = await api.delete('/cart')
        console.log('Корзина очищена, ответ сервера:', response.status)
        await this.loadCart()
        
        if (this.$root.$toast) {
          this.$root.$toast.success('Корзина очищена')
        }
      } catch (error) {
        console.error('Ошибка очистки корзины:', error)
        console.error('Детали ошибки:', error.response?.data || error.message)
        if (this.$root.$toast) {
          this.$root.$toast.error('Ошибка очистки корзины')
        }
      }
    },
    checkout() {
      this.$router.push('/checkout')
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
.cart-page {
  min-height: calc(100vh - 140px);
  padding: 2rem 0;
}

.cart-page h1 {
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
  margin-bottom: 0.5rem;
}

.empty-hint {
  font-size: 1rem;
  color: #999;
  margin-bottom: 1.5rem;
}

.cart-content {
  display: grid;
  grid-template-columns: 1fr 350px;
  gap: 2rem;
}

.cart-items {
  display: flex;
  flex-direction: column;
  gap: 1rem;
}

.cart-item {
  background: white;
  padding: 1.5rem;
  border-radius: 20px;
  box-shadow: 0 4px 15px rgba(0,0,0,0.08);
  display: grid;
  grid-template-columns: 120px 1fr auto auto auto;
  gap: 1.5rem;
  align-items: center;
  border: 1px solid rgba(102, 126, 234, 0.1);
  transition: all 0.4s cubic-bezier(0.175, 0.885, 0.32, 1.275);
  position: relative;
  overflow: hidden;
}

.cart-item::before {
  content: '';
  position: absolute;
  left: 0;
  top: 0;
  bottom: 0;
  width: 4px;
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
  transform: scaleY(0);
  transition: transform 0.4s;
}

.cart-item:hover::before {
  transform: scaleY(1);
}

.cart-item:hover {
  box-shadow: 0 8px 30px rgba(102, 126, 234, 0.2);
  transform: translateY(-4px);
  border-color: rgba(102, 126, 234, 0.3);
}

.item-image {
  width: 120px;
  height: 120px;
  border-radius: 5px;
  overflow: hidden;
  background: #f5f5f5;
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
  font-size: 2rem;
  font-weight: bold;
}

.item-info {
  flex: 1;
}

.item-info h3 {
  margin-bottom: 0.5rem;
  color: #333;
}

.item-description {
  color: #666;
  font-size: 0.9rem;
  margin-bottom: 0.5rem;
  display: -webkit-box;
  -webkit-line-clamp: 2;
  line-clamp: 2;
  -webkit-box-orient: vertical;
  overflow: hidden;
}

.item-price {
  color: #667eea;
  font-weight: bold;
  font-size: 1.1rem;
}

.item-quantity {
  display: flex;
  align-items: center;
  gap: 0.5rem;
}

.item-quantity button {
  width: 38px;
  height: 38px;
  border: 2px solid #e0e0e0;
  background: white;
  border-radius: 10px;
  cursor: pointer;
  font-size: 1.5rem;
  display: flex;
  align-items: center;
  justify-content: center;
  transition: all 0.3s;
  font-weight: bold;
  color: #667eea;
  line-height: 1;
}

.item-quantity button:hover:not(:disabled) {
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
  color: white;
  border-color: #667eea;
  transform: scale(1.15);
  box-shadow: 0 2px 8px rgba(102, 126, 234, 0.3);
}

.item-quantity button:disabled {
  opacity: 0.5;
  cursor: not-allowed;
}

.item-quantity span {
  min-width: 30px;
  text-align: center;
  font-weight: 500;
}

.item-total {
  font-size: 1.3rem;
  font-weight: bold;
  color: #667eea;
  min-width: 120px;
  text-align: right;
}

.btn-remove {
  width: 45px;
  height: 45px;
  border: none;
  background: linear-gradient(135deg, #e74c3c 0%, #c0392b 100%);
  color: white;
  border-radius: 12px;
  cursor: pointer;
  font-size: 1.3rem;
  line-height: 1;
  transition: all 0.3s;
  box-shadow: 0 2px 8px rgba(231, 76, 60, 0.3);
  display: flex;
  align-items: center;
  justify-content: center;
}

.btn-remove:hover {
  transform: scale(1.15) rotate(5deg);
  box-shadow: 0 4px 15px rgba(231, 76, 60, 0.4);
}

.cart-summary {
  background: white;
  padding: 2.5rem;
  border-radius: 20px;
  box-shadow: 0 8px 30px rgba(0,0,0,0.12);
  height: fit-content;
  position: sticky;
  top: 2rem;
  border: 1px solid rgba(102, 126, 234, 0.1);
}

.cart-summary h2 {
  margin-bottom: 1.5rem;
  color: #667eea;
}

.summary-row {
  display: flex;
  justify-content: space-between;
  padding: 0.75rem 0;
  border-bottom: 1px solid #eee;
  align-items: center;
}

.summary-value {
  font-weight: 600;
  color: #667eea;
}

.summary-row.total {
  font-size: 1.4rem;
  font-weight: bold;
  color: #667eea;
  border-bottom: none;
  margin-top: 0.5rem;
  padding-top: 1rem;
  border-top: 3px solid #667eea;
}

.summary-row.total .summary-value {
  font-size: 1.5rem;
}

.btn-clear {
  width: 100%;
  padding: 0.75rem;
  background: #95a5a6;
  color: white;
  border: none;
  border-radius: 10px;
  cursor: pointer;
  font-size: 1rem;
  font-weight: 600;
  margin-top: 1rem;
  transition: all 0.3s;
  box-shadow: 0 2px 8px rgba(149, 165, 166, 0.3);
}

.btn-clear:hover {
  background: #7f8c8d;
  transform: translateY(-2px);
  box-shadow: 0 4px 12px rgba(149, 165, 166, 0.4);
}

.btn-checkout {
  width: 100%;
  padding: 1rem;
  background: linear-gradient(135deg, #27ae60 0%, #229954 100%);
  color: white;
  border: none;
  border-radius: 10px;
  cursor: pointer;
  font-size: 1rem;
  font-weight: bold;
  margin-top: 0.5rem;
  transition: all 0.3s;
  box-shadow: 0 4px 15px rgba(39, 174, 96, 0.3);
}

.btn-checkout:hover {
  transform: translateY(-2px);
  box-shadow: 0 6px 20px rgba(39, 174, 96, 0.4);
}

.btn-primary {
  display: inline-block;
  padding: 0.75rem 2rem;
  background: #667eea;
  color: white;
  text-decoration: none;
  border-radius: 5px;
  font-weight: 500;
  transition: background 0.3s;
}

.btn-primary:hover {
  background: #5568d3;
}

@media (max-width: 968px) {
  .cart-content {
    grid-template-columns: 1fr;
  }
  
  .cart-item {
    grid-template-columns: 100px 1fr;
    gap: 1rem;
  }
  
  .item-quantity,
  .item-total,
  .btn-remove {
    grid-column: 2;
  }
  
  .item-total {
    text-align: left;
  }
}
</style>

