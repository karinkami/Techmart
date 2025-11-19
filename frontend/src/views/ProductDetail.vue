<template>
  <div class="product-detail">
    <div class="container">
      <div v-if="loading" class="loading">
        <div class="spinner"></div>
        <p>Загрузка товара...</p>
      </div>

      <div v-else-if="error" class="error">
        <p>{{ error }}</p>
        <router-link to="/catalog" class="btn-back">← Вернуться в каталог</router-link>
      </div>

      <div v-else-if="product" class="product-content">
        <button @click="$router.back()" class="btn-back-top">← Назад</button>
        
        <div class="product-main">
          <div class="product-image-section">
            <div class="product-image-large">
              <img 
                v-if="product.imageUrl && !imageError" 
                :src="product.imageUrl" 
                :alt="product.title"
                @error="imageError = true"
                loading="lazy"
              />
              <div v-else class="no-image-large">
                <div class="image-placeholder-large">
                  <span>{{ product.title.charAt(0) }}</span>
                </div>
              </div>
            </div>
          </div>

          <div class="product-info-section">
            <div class="product-header">
              <div class="product-badges">
                <span class="badge category-badge">{{ product.category?.name }}</span>
                <span class="badge manufacturer-badge">{{ product.manufacturer?.name }}</span>
              </div>
              <h1>{{ product.title }}</h1>
              <div class="product-price-large">
                {{ formatPrice(product.price) }} ₽
              </div>
            </div>

            <div class="product-description-full">
              <h2>📝 Описание</h2>
              <p>{{ product.description }}</p>
            </div>

            <div class="product-specs">
              <h2>📋 Характеристики</h2>
              <div class="specs-grid">
                <div class="spec-item">
                  <span class="spec-label">Категория:</span>
                  <span class="spec-value">{{ product.category?.name }}</span>
                </div>
                <div class="spec-item">
                  <span class="spec-label">Производитель:</span>
                  <span class="spec-value">{{ product.manufacturer?.name }}</span>
                </div>
                <div class="spec-item">
                  <span class="spec-label">Цена:</span>
                  <span class="spec-value price-value">{{ formatPrice(product.price) }} ₽</span>
                </div>
              </div>
            </div>

            <div class="product-actions">
              <div class="quantity-selector" v-if="isAuthenticated">
                <label>Количество:</label>
                <div class="quantity-controls">
                  <button @click="decreaseQuantity" :disabled="quantity <= 1">−</button>
                  <input type="number" v-model.number="quantity" min="1" max="99" />
                  <button @click="increaseQuantity">+</button>
                </div>
              </div>

              <button 
                v-if="isAuthenticated" 
                @click="addToCart" 
                class="btn-add-cart-large"
                :disabled="addingToCart"
              >
                <span v-if="!addingToCart">🛒 Добавить в корзину</span>
                <span v-else>⏳ Добавление...</span>
              </button>
              <router-link v-else to="/auth" class="btn-add-cart-large btn-login-large">
                🔐 Войти для покупки
              </router-link>
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
  name: 'ProductDetail',
  data() {
    return {
      product: null,
      loading: true,
      error: '',
      imageError: false,
      quantity: 1,
      addingToCart: false
    }
  },
  computed: {
    isAuthenticated() {
      return !!localStorage.getItem('token')
    }
  },
  async mounted() {
    await this.loadProduct()
  },
  watch: {
    '$route'(to, from) {
      if (to.params.id !== from.params.id) {
        this.loadProduct()
      }
    }
  },
  methods: {
    async loadProduct() {
      this.loading = true
      this.error = ''
      this.imageError = false
      this.quantity = 1

      try {
        const productId = this.$route.params.id
        const response = await api.get(`/products/${productId}`)
        this.product = response.data
      } catch (error) {
        console.error('Ошибка загрузки товара:', error)
        if (error.response?.status === 404) {
          this.error = 'Товар не найден'
        } else {
          this.error = 'Ошибка загрузки товара. Попробуйте позже.'
        }
      } finally {
        this.loading = false
      }
    },
    formatPrice(price) {
      return new Intl.NumberFormat('ru-RU').format(price)
    },
    increaseQuantity() {
      if (this.quantity < 99) {
        this.quantity++
      }
    },
    decreaseQuantity() {
      if (this.quantity > 1) {
        this.quantity--
      }
    },
    async addToCart() {
      if (!this.isAuthenticated) {
        this.$router.push('/auth')
        return
      }

      this.addingToCart = true
      try {
        await api.post('/cart', {
          productId: this.product.id,
          quantity: this.quantity
        })
        this.$root.$toast?.success(`Товар добавлен в корзину! (${this.quantity} шт.)`)
      } catch (error) {
        console.error('Ошибка добавления в корзину:', error)
        this.$root.$toast?.error('Ошибка добавления в корзину. Попробуйте снова.')
      } finally {
        this.addingToCart = false
      }
    }
  }
}
</script>

<style scoped>
.product-detail {
  min-height: calc(100vh - 140px);
  padding: 2rem 0;
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

.error {
  text-align: center;
  padding: 4rem;
}

.error p {
  font-size: 1.2rem;
  color: #e74c3c;
  margin-bottom: 1.5rem;
}

.btn-back-top {
  margin-bottom: 2rem;
  padding: 0.75rem 1.5rem;
  background: white;
  border: 2px solid #e0e0e0;
  border-radius: 10px;
  cursor: pointer;
  font-size: 1rem;
  font-weight: 500;
  color: #667eea;
  transition: all 0.3s;
  box-shadow: 0 2px 8px rgba(0,0,0,0.05);
}

.btn-back-top:hover {
  border-color: #667eea;
  background: rgba(102, 126, 234, 0.05);
  transform: translateX(-5px);
}

.btn-back {
  display: inline-block;
  padding: 0.75rem 1.5rem;
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
  color: white;
  text-decoration: none;
  border-radius: 10px;
  font-weight: 500;
  transition: all 0.3s;
  box-shadow: 0 2px 8px rgba(102, 126, 234, 0.3);
}

.btn-back:hover {
  transform: translateY(-2px);
  box-shadow: 0 4px 12px rgba(102, 126, 234, 0.4);
}

.product-main {
  display: grid;
  grid-template-columns: 1fr 1fr;
  gap: 3rem;
  background: white;
  padding: 2.5rem;
  border-radius: 20px;
  box-shadow: 0 8px 30px rgba(0,0,0,0.12);
  border: 1px solid rgba(102, 126, 234, 0.1);
}

.product-image-section {
  position: static;
  height: fit-content;
  align-self: start;
}

.product-image-large {
  width: 100%;
  max-height: 500px;
  border-radius: 20px;
  overflow: hidden;
  background: linear-gradient(135deg, #f5f5f5 0%, #e8e8e8 100%);
  box-shadow: 0 4px 20px rgba(0,0,0,0.1);
  display: flex;
  align-items: center;
  justify-content: center;
  position: relative;
}

.product-image-large img {
  width: 100%;
  height: auto;
  max-height: 500px;
  object-fit: contain;
  display: block;
  position: relative;
}

.no-image-large {
  width: 100%;
  height: 100%;
  display: flex;
  align-items: center;
  justify-content: center;
}

.image-placeholder-large {
  width: 100%;
  height: 100%;
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
  display: flex;
  align-items: center;
  justify-content: center;
  color: white;
  font-size: 5rem;
  font-weight: bold;
}

.product-info-section {
  display: flex;
  flex-direction: column;
  gap: 2rem;
}

.product-header {
  border-bottom: 2px solid #f0f0f0;
  padding-bottom: 1.5rem;
}

.product-badges {
  display: flex;
  gap: 0.75rem;
  margin-bottom: 1rem;
}

.badge {
  padding: 0.5rem 1rem;
  border-radius: 20px;
  font-size: 0.85rem;
  font-weight: 600;
}

.category-badge {
  background: linear-gradient(135deg, rgba(102, 126, 234, 0.1) 0%, rgba(118, 75, 162, 0.1) 100%);
  color: #667eea;
  border: 1px solid rgba(102, 126, 234, 0.2);
}

.manufacturer-badge {
  background: linear-gradient(135deg, rgba(39, 174, 96, 0.1) 0%, rgba(34, 153, 84, 0.1) 100%);
  color: #27ae60;
  border: 1px solid rgba(39, 174, 96, 0.2);
}

.product-header h1 {
  font-size: 2.5rem;
  margin-bottom: 1rem;
  color: #333;
  line-height: 1.2;
}

.product-price-large {
  font-size: 2.5rem;
  font-weight: bold;
  color: #667eea;
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
  -webkit-background-clip: text;
  -webkit-text-fill-color: transparent;
  background-clip: text;
}

.product-description-full {
  padding: 1.5rem;
  background: linear-gradient(135deg, #f8f9fa 0%, #ffffff 100%);
  border-radius: 15px;
  border: 1px solid rgba(102, 126, 234, 0.1);
}

.product-description-full h2 {
  font-size: 1.3rem;
  margin-bottom: 1rem;
  color: #667eea;
}

.product-description-full p {
  font-size: 1.1rem;
  line-height: 1.8;
  color: #666;
}

.product-specs {
  padding: 1.5rem;
  background: linear-gradient(135deg, #f8f9fa 0%, #ffffff 100%);
  border-radius: 15px;
  border: 1px solid rgba(102, 126, 234, 0.1);
}

.product-specs h2 {
  font-size: 1.3rem;
  margin-bottom: 1rem;
  color: #667eea;
}

.specs-grid {
  display: flex;
  flex-direction: column;
  gap: 1rem;
}

.spec-item {
  display: flex;
  justify-content: space-between;
  padding: 0.75rem;
  background: white;
  border-radius: 10px;
  border: 1px solid rgba(102, 126, 234, 0.1);
}

.spec-label {
  font-weight: 600;
  color: #666;
}

.spec-value {
  color: #333;
  font-weight: 500;
}

.price-value {
  color: #667eea;
  font-weight: bold;
  font-size: 1.1rem;
}

.product-actions {
  padding: 1.5rem;
  background: linear-gradient(135deg, rgba(102, 126, 234, 0.05) 0%, rgba(118, 75, 162, 0.05) 100%);
  border-radius: 15px;
  border: 1px solid rgba(102, 126, 234, 0.1);
}

.quantity-selector {
  margin-bottom: 1.5rem;
}

.quantity-selector label {
  display: block;
  margin-bottom: 0.5rem;
  font-weight: 600;
  color: #333;
}

.quantity-controls {
  display: flex;
  align-items: center;
  gap: 0.5rem;
}

.quantity-controls button {
  width: 45px;
  height: 45px;
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

.quantity-controls button:hover:not(:disabled) {
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
  color: white;
  border-color: #667eea;
  transform: scale(1.1);
  box-shadow: 0 2px 8px rgba(102, 126, 234, 0.3);
}

.quantity-controls button:disabled {
  opacity: 0.5;
  cursor: not-allowed;
}

.quantity-controls input {
  width: 80px;
  height: 45px;
  border: 2px solid #e0e0e0;
  border-radius: 10px;
  text-align: center;
  font-size: 1.2rem;
  font-weight: 600;
  color: #333;
  transition: all 0.3s;
}

.quantity-controls input:focus {
  outline: none;
  border-color: #667eea;
  box-shadow: 0 0 0 3px rgba(102, 126, 234, 0.1);
}

.btn-add-cart-large {
  width: 100%;
  padding: 1.2rem;
  background: linear-gradient(135deg, #27ae60 0%, #229954 100%);
  color: white;
  border: none;
  border-radius: 12px;
  cursor: pointer;
  font-size: 1.1rem;
  font-weight: bold;
  transition: all 0.3s;
  box-shadow: 0 4px 15px rgba(39, 174, 96, 0.3);
  text-decoration: none;
  display: flex;
  align-items: center;
  justify-content: center;
}

.btn-add-cart-large:hover:not(:disabled) {
  transform: translateY(-2px);
  box-shadow: 0 6px 20px rgba(39, 174, 96, 0.4);
}

.btn-add-cart-large:disabled {
  opacity: 0.6;
  cursor: not-allowed;
}

.btn-login-large {
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
  box-shadow: 0 4px 15px rgba(102, 126, 234, 0.3);
}

.btn-login-large:hover {
  box-shadow: 0 6px 20px rgba(102, 126, 234, 0.4);
}

@media (max-width: 968px) {
  .product-main {
    grid-template-columns: 1fr;
    gap: 2rem;
  }

  .product-image-section {
    position: static;
  }

  .product-header h1 {
    font-size: 2rem;
  }

  .product-price-large {
    font-size: 2rem;
  }
}
</style>

