<template>
  <section class="recommendations">
    <h2>Вам может понравиться</h2>
    <p class="subtitle">Подборка на основе ваших просмотров и покупок</p>

    <div v-if="loading" class="loading">Загружаем рекомендации...</div>
    <div v-else-if="error" class="error">{{ error }}</div>

    <div v-else class="grid">
      <router-link
        v-for="item in items"
        :key="item.id"
        :to="`/product/${item.id}`"
        class="card"
      >
        <img v-if="item.imageUrl" :src="item.imageUrl" :alt="item.title" class="image" />
        <div v-else class="image placeholder">{{ item.title?.charAt(0) }}</div>
        <h3>{{ item.title }}</h3>
        <p class="price">{{ formatPrice(item.price) }} ₽</p>
        <p class="reason">{{ item.reason }}</p>
      </router-link>
    </div>
  </section>
</template>

<script>
import { api } from '../services/api'

export default {
  name: 'RecommendationsBlock',
  data() {
    return {
      loading: false,
      error: '',
      items: []
    }
  },
  async mounted() {
    if (!localStorage.getItem('token')) {
      this.error = 'Войдите в аккаунт, чтобы увидеть персональные рекомендации'
      return
    }
    this.loading = true
    try {
      const response = await api.post('/recommendations', { limit: 5 })
      this.items = response.data?.items || []
      if (!this.items.length) {
        this.error = 'Пока нет рекомендаций. Посмотрите несколько товаров.'
      }
    } catch (e) {
      console.error(e)
      this.error = 'Не удалось загрузить рекомендации'
    } finally {
      this.loading = false
    }
  },
  methods: {
    formatPrice(price) {
      return new Intl.NumberFormat('ru-RU').format(price || 0)
    }
  }
}
</script>

<style scoped>
.recommendations { margin-top: 4rem; }
.subtitle { color: #666; margin-bottom: 1rem; }
.grid { display: grid; grid-template-columns: repeat(auto-fit, minmax(220px, 1fr)); gap: 1rem; }
.card { background: #fff; border-radius: 12px; padding: 1rem; text-decoration: none; color: inherit; border: 1px solid #ececec; }
.image { width: 100%; height: 150px; object-fit: contain; border-radius: 8px; background: #f6f7fb; }
.placeholder { display: flex; align-items: center; justify-content: center; font-size: 2rem; color: #667eea; }
.price { margin-top: 0.6rem; font-weight: 700; color: #667eea; }
.reason { margin-top: 0.4rem; color: #666; font-size: 0.9rem; }
.loading, .error { margin-top: 1rem; color: #555; }
</style>
