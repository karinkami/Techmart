<template>
  <div class="orders-page">
    <div class="container">
      <h1>📦 Мои заказы</h1>
      
      <div v-if="loading" class="loading">
        <div class="spinner"></div>
        <p>Загрузка заказов...</p>
      </div>

      <div v-else-if="error" class="error">
        <p>{{ error }}</p>
      </div>

      <div v-else-if="orders.length === 0" class="empty-orders">
        <div class="empty-icon">📦</div>
        <p>У вас пока нет заказов</p>
        <router-link to="/catalog" class="btn-primary">Перейти в каталог</router-link>
      </div>

      <div v-else class="orders-list">
        <div v-for="order in orders" :key="order.id" class="order-card">
          <div class="order-header">
            <div class="order-info">
              <h2>Заказ #{{ order.id }}</h2>
              <div class="order-meta">
                <span class="order-date">📅 {{ formatDate(order.createdAt) }}</span>
                <span class="order-status" :class="getStatusClass(order.status)">
                  {{ getStatusText(order.status) }}
                </span>
              </div>
            </div>
            <div class="order-total">
              <span class="total-label">Итого:</span>
              <span class="total-amount">{{ formatPrice(order.totalAmount) }} ₽</span>
              <button
                v-if="canRequestReturn(order)"
                class="btn-return"
                @click="openReturnModal(order)"
              >
                Оформить возврат
              </button>
              <span v-else-if="hasActiveReturn(order.id)" class="return-badge">Заявка на возврат создана</span>
            </div>
          </div>

          <div class="order-details">
            <div class="delivery-info">
              <h3>📋 Данные доставки</h3>
              <div class="info-row">
                <span class="info-label">Получатель:</span>
                <span>{{ order.name }}</span>
              </div>
              <div class="info-row">
                <span class="info-label">Email:</span>
                <span>{{ order.email }}</span>
              </div>
              <div class="info-row">
                <span class="info-label">Телефон:</span>
                <span>{{ order.phone }}</span>
              </div>
              <div class="info-row">
                <span class="info-label">Адрес:</span>
                <span>{{ order.address }}</span>
              </div>
            </div>

            <div class="order-items-section">
              <h3>🛒 Товары в заказе</h3>
              <div v-if="order.orderItems && order.orderItems.length > 0" class="order-items">
                <div v-for="item in order.orderItems" :key="item.id" class="order-item">
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
                    <h4>{{ item.product?.title }}</h4>
                    <div class="item-specs">
                      <span>Количество: {{ item.quantity }} шт.</span>
                      <span>Цена: {{ formatPrice(item.price) }} ₽</span>
                    </div>
                  </div>
                  <div class="item-total">
                    {{ formatPrice(item.price * item.quantity) }} ₽
                  </div>
                </div>
              </div>
              <div v-else class="no-items">
                <p>Товары не найдены</p>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>

    <div v-if="returnModalOpen" class="modal-backdrop" @click.self="closeReturnModal">
      <div class="modal">
        <h3>Оформление возврата</h3>
        <p class="modal-subtitle">Заказ #{{ returnForm.orderId }}</p>

        <div class="form-group">
          <label>Причина возврата *</label>
          <select v-model="returnForm.reason">
            <option value="">Выберите причину</option>
            <option value="Не подошел товар">Не подошел товар</option>
            <option value="Товар с дефектом">Товар с дефектом</option>
            <option value="Пришел не тот товар">Пришел не тот товар</option>
            <option value="Другое">Другое</option>
          </select>
        </div>

        <div class="form-group">
          <label>Что именно не так / комментарий</label>
          <textarea
            v-model.trim="returnForm.details"
            rows="4"
            placeholder="Опишите проблему, это поможет быстрее обработать заявку"
          />
        </div>

        <div class="form-group">
          <label>Предпочтительное решение *</label>
          <select v-model="returnForm.preferredResolution">
            <option value="refund">Возврат денег</option>
            <option value="exchange">Обмен товара</option>
            <option value="service">Диагностика/ремонт</option>
          </select>
        </div>

        <div class="form-group">
          <label>Контактный телефон *</label>
          <input
            v-model.trim="returnForm.contactPhone"
            type="text"
            placeholder="+7 (___) ___-__-__"
          />
        </div>

        <div v-if="returnError" class="error-text">{{ returnError }}</div>
        <div class="modal-actions">
          <button class="btn-secondary" :disabled="returnSubmitting" @click="closeReturnModal">Отмена</button>
          <button class="btn-primary" :disabled="returnSubmitting" @click="submitReturnRequest">
            {{ returnSubmitting ? 'Отправка...' : 'Отправить заявку' }}
          </button>
        </div>
      </div>
    </div>
  </div>
</template>

<script>
import { api } from '../services/api'

export default {
  name: 'Orders',
  data() {
    return {
      orders: [],
      loading: true,
      error: '',
      imageErrors: new Set(),
      returnRequestsByOrder: {},
      returnModalOpen: false,
      returnSubmitting: false,
      returnError: '',
      returnForm: {
        orderId: null,
        reason: '',
        details: '',
        preferredResolution: 'refund',
        contactPhone: ''
      }
    }
  },
  async mounted() {
    await Promise.all([this.loadOrders(), this.loadReturnRequests()])
  },
  methods: {
    async loadOrders() {
      this.loading = true
      this.error = ''
      try {
        const response = await api.get('/orders')
        this.orders = response.data || []
      } catch (error) {
        console.error('Ошибка загрузки заказов:', error)
        if (error.response?.status === 401) {
          this.$router.push('/auth')
        } else if (error.response?.data?.message) {
          this.error = error.response.data.message
        } else if (error.response?.data?.error) {
          this.error = error.response.data.error
        } else {
          this.error = 'Ошибка загрузки заказов. Попробуйте позже.'
        }
      } finally {
        this.loading = false
      }
    },
    formatPrice(price) {
      return new Intl.NumberFormat('ru-RU').format(price)
    },
    formatDate(dateString) {
      if (!dateString) return ''
      const date = new Date(dateString)
      return date.toLocaleDateString('ru-RU', {
        year: 'numeric',
        month: 'long',
        day: 'numeric',
        hour: '2-digit',
        minute: '2-digit'
      })
    },
    getStatusText(status) {
      const statusMap = {
        'pending': '⏳ Ожидает обработки',
        'processing': '🔄 В обработке',
        'shipped': '🚚 Отправлен',
        'delivered': '✅ Доставлен',
        'cancelled': '❌ Отменен'
      }
      return statusMap[status] || status
    },
    getStatusClass(status) {
      const classMap = {
        'pending': 'status-pending',
        'processing': 'status-processing',
        'shipped': 'status-shipped',
        'delivered': 'status-delivered',
        'cancelled': 'status-cancelled'
      }
      return classMap[status] || ''
    },
    canRequestReturn(order) {
      const normalized = String(order?.status || '').trim().toLowerCase()
      const eligible = normalized === 'delivered' || normalized === 'completed' || normalized === 'done'
      return eligible && !this.hasActiveReturn(order.id)
    },
    hasActiveReturn(orderId) {
      return Boolean(this.returnRequestsByOrder[orderId])
    },
    async loadReturnRequests() {
      try {
        const response = await api.get('/returns/mine')
        const map = {}
        for (const req of response.data || []) {
          if (!map[req.orderId] && ['pending', 'in_review', 'approved'].includes(req.status)) {
            map[req.orderId] = true
          }
        }
        this.returnRequestsByOrder = map
      } catch (e) {
        console.error('Ошибка загрузки заявок на возврат:', e)
      }
    },
    openReturnModal(order) {
      this.returnForm.orderId = order.id
      this.returnForm.reason = ''
      this.returnForm.details = ''
      this.returnForm.preferredResolution = 'refund'
      this.returnForm.contactPhone = order.phone || ''
      this.returnError = ''
      this.returnModalOpen = true
    },
    closeReturnModal() {
      this.returnModalOpen = false
      this.returnError = ''
    },
    async submitReturnRequest() {
      this.returnError = ''
      if (!this.returnForm.orderId || !this.returnForm.reason || !this.returnForm.contactPhone) {
        this.returnError = 'Заполните обязательные поля: причина и контактный телефон.'
        return
      }
      this.returnSubmitting = true
      try {
        await api.post('/returns', {
          orderId: this.returnForm.orderId,
          reason: this.returnForm.reason,
          details: this.returnForm.details || null,
          preferredResolution: this.returnForm.preferredResolution,
          contactPhone: this.returnForm.contactPhone
        })
        this.$root.$toast?.success('Заявка на возврат отправлена')
        this.returnRequestsByOrder = {
          ...this.returnRequestsByOrder,
          [this.returnForm.orderId]: true
        }
        this.closeReturnModal()
      } catch (e) {
        console.error('Ошибка отправки заявки на возврат:', e)
        this.returnError = e.response?.data?.message || 'Не удалось отправить заявку. Попробуйте позже.'
      } finally {
        this.returnSubmitting = false
      }
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
.orders-page {
  min-height: calc(100vh - 140px);
  padding: 2rem 0;
}

.orders-page h1 {
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

.error {
  text-align: center;
  padding: 2rem;
  color: #e74c3c;
}

.empty-orders {
  text-align: center;
  padding: 4rem;
}

.empty-icon {
  font-size: 5rem;
  margin-bottom: 1rem;
  opacity: 0.5;
}

.empty-orders p {
  font-size: 1.2rem;
  color: #666;
  margin-bottom: 1.5rem;
}

.orders-list {
  display: flex;
  flex-direction: column;
  gap: 2rem;
}

.order-card {
  background: white;
  border-radius: 20px;
  padding: 2rem;
  box-shadow: 0 8px 30px rgba(0,0,0,0.12);
  border: 1px solid rgba(102, 126, 234, 0.1);
  transition: all 0.3s;
}

.order-card:hover {
  box-shadow: 0 12px 40px rgba(102, 126, 234, 0.15);
  transform: translateY(-2px);
}

.order-header {
  display: flex;
  justify-content: space-between;
  align-items: flex-start;
  margin-bottom: 2rem;
  padding-bottom: 1.5rem;
  border-bottom: 2px solid #f0f0f0;
}

.order-info h2 {
  font-size: 1.8rem;
  color: #333;
  margin-bottom: 0.5rem;
}

.order-meta {
  display: flex;
  gap: 1.5rem;
  flex-wrap: wrap;
}

.order-date {
  color: #666;
  font-size: 0.95rem;
}

.order-status {
  padding: 0.4rem 1rem;
  border-radius: 20px;
  font-size: 0.9rem;
  font-weight: 600;
}

.status-pending {
  background: rgba(255, 193, 7, 0.1);
  color: #ff9800;
  border: 1px solid rgba(255, 193, 7, 0.3);
}

.status-processing {
  background: rgba(33, 150, 243, 0.1);
  color: #2196f3;
  border: 1px solid rgba(33, 150, 243, 0.3);
}

.status-shipped {
  background: rgba(156, 39, 176, 0.1);
  color: #9c27b0;
  border: 1px solid rgba(156, 39, 176, 0.3);
}

.status-delivered {
  background: rgba(76, 175, 80, 0.1);
  color: #4caf50;
  border: 1px solid rgba(76, 175, 80, 0.3);
}

.status-cancelled {
  background: rgba(244, 67, 54, 0.1);
  color: #f44336;
  border: 1px solid rgba(244, 67, 54, 0.3);
}

.order-total {
  text-align: right;
  display: flex;
  flex-direction: column;
  align-items: flex-end;
  gap: 0.6rem;
}

.total-label {
  display: block;
  font-size: 0.9rem;
  color: #666;
  margin-bottom: 0.25rem;
}

.total-amount {
  font-size: 1.8rem;
  font-weight: bold;
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
  -webkit-background-clip: text;
  -webkit-text-fill-color: transparent;
  background-clip: text;
}

.order-details {
  display: grid;
  grid-template-columns: 1fr 1fr;
  gap: 2rem;
}

.delivery-info h3,
.order-items-section h3 {
  font-size: 1.2rem;
  color: #667eea;
  margin-bottom: 1rem;
}

.info-row {
  display: flex;
  justify-content: space-between;
  padding: 0.5rem 0;
  border-bottom: 1px solid #f0f0f0;
}

.info-label {
  font-weight: 600;
  color: #666;
}

.order-items {
  display: flex;
  flex-direction: column;
  gap: 1rem;
  max-height: 400px;
  overflow-y: auto;
}

.no-items {
  text-align: center;
  padding: 2rem;
  color: #999;
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

.item-details h4 {
  font-size: 1rem;
  margin-bottom: 0.5rem;
  color: #333;
}

.item-specs {
  display: flex;
  flex-direction: column;
  gap: 0.25rem;
  font-size: 0.9rem;
  color: #666;
}

.item-total {
  font-size: 1.2rem;
  font-weight: bold;
  color: #667eea;
  align-self: center;
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

.btn-return {
  padding: 0.5rem 0.9rem;
  border-radius: 8px;
  border: none;
  cursor: pointer;
  color: #fff;
  font-weight: 600;
  background: linear-gradient(135deg, #ff9800 0%, #f57c00 100%);
}

.return-badge {
  font-size: 0.85rem;
  color: #ff9800;
  font-weight: 600;
}

.modal-backdrop {
  position: fixed;
  inset: 0;
  background: rgba(0, 0, 0, 0.45);
  display: flex;
  justify-content: center;
  align-items: center;
  z-index: 1300;
}

.modal {
  width: min(560px, calc(100vw - 2rem));
  background: #fff;
  border-radius: 14px;
  padding: 1.2rem;
}

.modal-subtitle {
  color: #666;
  margin-bottom: 1rem;
}

.form-group {
  margin-bottom: 0.9rem;
}

.form-group label {
  display: block;
  margin-bottom: 0.4rem;
  font-weight: 600;
}

.form-group input,
.form-group select,
.form-group textarea {
  width: 100%;
  border: 1px solid #d8d8d8;
  border-radius: 8px;
  padding: 0.55rem 0.65rem;
}

.error-text {
  color: #d32f2f;
  margin-bottom: 0.7rem;
}

.modal-actions {
  display: flex;
  justify-content: flex-end;
  gap: 0.6rem;
}

.btn-secondary {
  padding: 0.55rem 0.9rem;
  border-radius: 8px;
  border: 1px solid #ccc;
  background: #fff;
  cursor: pointer;
}

@media (max-width: 968px) {
  .order-header {
    flex-direction: column;
    gap: 1rem;
  }

  .order-total {
    text-align: left;
  }

  .order-details {
    grid-template-columns: 1fr;
  }
}
</style>

