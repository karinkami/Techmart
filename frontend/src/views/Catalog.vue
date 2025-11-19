<template>
  <div class="catalog">
    <div class="container">
      <h1>Каталог товаров</h1>
      
      <div class="filters-wrapper" v-if="categories.length > 0 || manufacturers.length > 0">
        <div class="filters">
          <div class="filter-group">
            <label>📂 Категория</label>
            <select v-model="selectedCategory" @change="loadProducts">
              <option value="">Все категории</option>
              <option v-for="cat in categories" :key="cat.id" :value="cat.id">
                {{ cat.name }}
              </option>
            </select>
          </div>
          
          <div class="filter-group">
            <label>🏭 Производитель</label>
            <select v-model="selectedManufacturer" @change="loadProducts">
              <option value="">Все производители</option>
              <option v-for="man in manufacturers" :key="man.id" :value="man.id">
                {{ man.name }}
              </option>
            </select>
          </div>
        </div>
      </div>

      <div v-if="loading" class="loading">
        <div class="spinner"></div>
        <p>Загрузка товаров...</p>
      </div>
      
      <div v-else-if="products.length === 0" class="no-products">
        <div class="empty-icon">📦</div>
        <p>Товары не найдены</p>
        <p class="empty-hint">Попробуйте изменить фильтры</p>
      </div>
      
      <div v-else class="products-grid">
        <div v-for="product in products" :key="product.id" class="product-card">
          <router-link :to="`/product/${product.id}`" class="product-link">
            <div class="product-image">
              <img 
                v-if="product.imageUrl && !isImageError(product.id)" 
                :src="product.imageUrl" 
                :alt="product.title"
                @error="setImageError(product.id)"
                @load="onImageLoad(product.id)"
                loading="lazy"
              />
              <div v-else class="no-image">
                <div class="image-placeholder">
                  <span>{{ product.title.charAt(0) }}</span>
                </div>
              </div>
            </div>
            <div class="product-info">
              <h3>{{ product.title }}</h3>
              <p class="product-description">{{ product.description }}</p>
              <div class="product-meta">
                <span class="category">{{ product.category?.name }}</span>
                <span class="manufacturer">{{ product.manufacturer?.name }}</span>
              </div>
            </div>
          </router-link>
          <div class="product-footer">
            <span class="price">{{ formatPrice(product.price) }} ₽</span>
            <button 
              v-if="isAuthenticated" 
              @click.stop="addToCart(product.id)" 
              class="btn-add-cart"
              :disabled="addingToCart === product.id"
            >
              <span v-if="addingToCart !== product.id">🛒 В корзину</span>
              <span v-else>⏳ Добавление...</span>
            </button>
            <router-link v-else to="/auth" class="btn-add-cart btn-login" @click.stop>
              🔐 Войти для покупки
            </router-link>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script>
import { api } from '../services/api'

export default {
  name: 'Catalog',
  data() {
    return {
      products: [],
      categories: [],
      manufacturers: [],
      selectedCategory: '',
      selectedManufacturer: '',
      loading: true,
      imageErrors: new Set(),
      addingToCart: null
    }
  },
  computed: {
    isAuthenticated() {
      return !!localStorage.getItem('token')
    }
  },
  async mounted() {
    await Promise.all([
      this.loadCategories(),
      this.loadManufacturers(),
      this.loadProducts()
    ])
  },
  methods: {
    async loadProducts() {
      this.loading = true
      try {
        const response = await api.get('/products')
        let filtered = response.data || []
        
        if (this.selectedCategory) {
          filtered = filtered.filter(p => p.categoryId === parseInt(this.selectedCategory))
        }
        
        if (this.selectedManufacturer) {
          filtered = filtered.filter(p => p.manufacturerId === parseInt(this.selectedManufacturer))
        }
        
        this.products = filtered
      } catch (error) {
        console.error('Ошибка загрузки товаров:', error)
        this.products = []
        this.$root.$toast?.error('Ошибка загрузки товаров. Проверьте консоль браузера и убедитесь, что backend запущен на http://localhost:5001')
      } finally {
        this.loading = false
      }
    },
    async loadCategories() {
      try {
        const response = await api.get('/categories')
        this.categories = response.data
      } catch (error) {
        console.error('Ошибка загрузки категорий:', error)
      }
    },
    async loadManufacturers() {
      try {
        const response = await api.get('/manufacturers')
        this.manufacturers = response.data
      } catch (error) {
        console.error('Ошибка загрузки производителей:', error)
      }
    },
    formatPrice(price) {
      return new Intl.NumberFormat('ru-RU').format(price)
    },
    setImageError(productId) {
      console.error(`Ошибка загрузки изображения для товара ${productId}`)
      this.imageErrors.add(productId)
    },
    isImageError(productId) {
      return this.imageErrors.has(productId)
    },
    onImageLoad(productId) {
      // Убираем из списка ошибок, если изображение загрузилось
      this.imageErrors.delete(productId)
    },
    async addToCart(productId) {
      if (!this.isAuthenticated) {
        this.$router.push('/auth')
        return
      }

      this.addingToCart = productId
      try {
        await api.post('/cart', {
          productId: productId,
          quantity: 1
        })
        this.$root.$toast?.success('Товар добавлен в корзину!')
      } catch (error) {
        console.error('Ошибка добавления в корзину:', error)
        this.$root.$toast?.error('Ошибка добавления в корзину. Попробуйте снова.')
      } finally {
        this.addingToCart = null
      }
    }
  }
}
</script>

<style scoped>
.catalog {
  position: relative;
  width: 100%;
  min-height: calc(100vh - 140px);
  padding: 3rem 0;
}

.catalog::before {
  content: '';
  position: absolute;
  top: 0;
  left: 50%;
  transform: translateX(-50%);
  width: 100vw;
  height: 100%;
  background: white;
  z-index: 0;
}

.catalog .container {
  position: relative;
  z-index: 1;
  background: white;
  max-width: 1200px;
  margin: 0 auto;
  padding: 0 20px;
}

.catalog h1 {
  font-size: 2.8rem;
  margin-bottom: 2rem;
  text-align: center;
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
  -webkit-background-clip: text;
  -webkit-text-fill-color: transparent;
  background-clip: text;
  font-weight: 700;
}

.filters-wrapper {
  margin-bottom: 2.5rem;
}

.filters {
  display: flex;
  gap: 1.5rem;
  flex-wrap: wrap;
  background: white;
  padding: 1.5rem;
  border-radius: 15px;
  box-shadow: 0 4px 15px rgba(0,0,0,0.08);
  border: 1px solid rgba(102, 126, 234, 0.1);
}

.filter-group {
  flex: 1;
  min-width: 200px;
}

.filter-group label {
  display: block;
  margin-bottom: 0.5rem;
  color: #667eea;
  font-weight: 600;
  font-size: 0.9rem;
}

.filters select {
  width: 100%;
  padding: 0.9rem 1rem;
  border: 2px solid #e0e0e0;
  border-radius: 10px;
  font-size: 1rem;
  background: white;
  transition: all 0.3s;
  cursor: pointer;
  appearance: none;
  background-image: url("data:image/svg+xml,%3Csvg xmlns='http://www.w3.org/2000/svg' width='12' height='12' viewBox='0 0 12 12'%3E%3Cpath fill='%23667eea' d='M6 9L1 4h10z'/%3E%3C/svg%3E");
  background-repeat: no-repeat;
  background-position: right 1rem center;
  padding-right: 2.5rem;
}

.filters select:focus {
  outline: none;
  border-color: #667eea;
  box-shadow: 0 0 0 3px rgba(102, 126, 234, 0.1);
  transform: translateY(-1px);
}

.loading {
  text-align: center;
  padding: 4rem;
  font-size: 1.2rem;
  color: #666;
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

.no-products {
  text-align: center;
  padding: 4rem;
}

.empty-icon {
  font-size: 5rem;
  margin-bottom: 1rem;
  opacity: 0.5;
}

.no-products p {
  font-size: 1.2rem;
  color: #666;
  margin-bottom: 0.5rem;
}

.empty-hint {
  font-size: 1rem;
  color: #999;
}

.products-grid {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(300px, 1fr));
  gap: 2rem;
}

.product-card {
  background: white;
  border-radius: 20px;
  overflow: hidden;
  box-shadow: 0 4px 15px rgba(0,0,0,0.08);
  transition: all 0.4s cubic-bezier(0.175, 0.885, 0.32, 1.275);
  border: 1px solid rgba(102, 126, 234, 0.1);
  position: relative;
  display: flex;
  flex-direction: column;
}

.product-link {
  text-decoration: none;
  color: inherit;
  display: flex;
  flex-direction: column;
  cursor: pointer;
  flex: 1;
}

.product-card::before {
  content: '';
  position: absolute;
  top: 0;
  left: 0;
  right: 0;
  height: 4px;
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
  transform: scaleX(0);
  transition: transform 0.4s;
}

.product-card:hover::before {
  transform: scaleX(1);
}

.product-card:hover {
  transform: translateY(-10px) scale(1.02);
  box-shadow: 0 12px 40px rgba(102, 126, 234, 0.25);
  border-color: rgba(102, 126, 234, 0.4);
}

.product-image {
  width: 100%;
  height: 180px;
  background: white;
  display: flex;
  align-items: center;
  justify-content: center;
  overflow: hidden;
  position: relative;
}

.product-image::after {
  content: '';
  position: absolute;
  top: 0;
  left: 0;
  right: 0;
  bottom: 0;
  background: transparent;
  pointer-events: none;
}

.product-image img {
  width: 100%;
  height: 100%;
  object-fit: contain;
  transition: transform 0.4s;
  padding: 10px;
}

.product-card:hover .product-image img {
  transform: scale(1.05);
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
  font-size: 3rem;
  font-weight: bold;
}

.product-info {
  padding: 1.5rem;
  flex: 1;
  display: flex;
  flex-direction: column;
}

.product-info h3 {
  font-size: 1.3rem;
  margin-bottom: 0.5rem;
  color: #333;
}

.product-description {
  color: #666;
  font-size: 0.9rem;
  margin-bottom: 1rem;
  display: -webkit-box;
  -webkit-line-clamp: 2;
  line-clamp: 2;
  -webkit-box-orient: vertical;
  overflow: hidden;
}

.product-meta {
  display: flex;
  gap: 1rem;
  margin-bottom: 1rem;
  font-size: 0.85rem;
}

.category, .manufacturer {
  padding: 0.25rem 0.75rem;
  background: #f0f0f0;
  border-radius: 15px;
  color: #666;
}

.product-footer {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 1rem 1.5rem;
  border-top: 1px solid #eee;
  gap: 1rem;
  flex-wrap: wrap;
  margin-top: auto;
}

.price {
  font-size: 1.5rem;
  font-weight: bold;
  color: #667eea;
}

.btn-add-cart {
  padding: 0.7rem 1.5rem;
  background: linear-gradient(135deg, #27ae60 0%, #229954 100%);
  color: white;
  border: none;
  border-radius: 10px;
  cursor: pointer;
  font-size: 0.9rem;
  font-weight: 600;
  transition: all 0.3s;
  white-space: nowrap;
  box-shadow: 0 2px 8px rgba(39, 174, 96, 0.3);
  text-decoration: none;
  display: inline-block;
}

.btn-add-cart:hover:not(:disabled) {
  transform: translateY(-2px) scale(1.05);
  box-shadow: 0 4px 15px rgba(39, 174, 96, 0.4);
}

.btn-add-cart:disabled {
  opacity: 0.6;
  cursor: not-allowed;
  transform: none;
}

.btn-login {
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
  box-shadow: 0 2px 8px rgba(102, 126, 234, 0.3);
}

.btn-login:hover {
  box-shadow: 0 4px 15px rgba(102, 126, 234, 0.4);
}
</style>

