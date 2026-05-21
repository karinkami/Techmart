<template>
  <div class="chat-widget">
    <button v-if="!isOpen" class="toggle" @click="isOpen = true">💬</button>
    <div v-else class="window">
      <div class="header">
        <span>Поддержка TechnMart</span>
        <button @click="isOpen = false">✕</button>
      </div>
      <div class="messages">
        <div v-for="(msg, idx) in messages" :key="idx" :class="['msg', msg.role]">
          {{ msg.text }}
        </div>
      </div>
      <form class="input-row" @submit.prevent="sendMessage">
        <input v-model.trim="input" placeholder="Напишите вопрос..." />
        <button :disabled="sending || !input">Отправить</button>
      </form>
    </div>
  </div>
</template>

<script>
import { api } from '../services/api'

export default {
  name: 'ChatWidget',
  data() {
    return {
      isOpen: false,
      input: '',
      sending: false,
      messages: [
        { role: 'bot', text: 'Здравствуйте! Я помогу со статусом заказа, возвратом, оплатой и гарантией.' }
      ]
    }
  },
  methods: {
    async sendMessage() {
      if (!this.input || this.sending) return
      const text = this.input
      this.messages.push({ role: 'user', text })
      this.input = ''
      this.sending = true
      try {
        const response = await api.post('/chat', { text })
        const reply = response.data?.reply || 'Извините, я не понял. Попробуйте переформулировать'
        this.messages.push({ role: 'bot', text: reply })
      } catch (e) {
        console.error(e)
        this.messages.push({ role: 'bot', text: 'Извините, я не понял. Попробуйте переформулировать' })
      } finally {
        this.sending = false
      }
    }
  }
}
</script>

<style scoped>
.chat-widget { position: fixed; right: 20px; bottom: 20px; z-index: 1200; }
.toggle { width: 56px; height: 56px; border-radius: 50%; border: none; background: #667eea; color: #fff; font-size: 1.4rem; cursor: pointer; }
.window { width: 320px; height: 420px; background: #fff; border-radius: 12px; border: 1px solid #e6e6e6; display: flex; flex-direction: column; overflow: hidden; box-shadow: 0 10px 28px rgba(0,0,0,0.15); }
.header { padding: 0.8rem 1rem; background: #667eea; color: #fff; display: flex; justify-content: space-between; align-items: center; }
.header button { border: none; background: transparent; color: #fff; cursor: pointer; }
.messages { flex: 1; padding: 0.8rem; overflow-y: auto; background: #fafbff; }
.msg { max-width: 88%; margin-bottom: 0.6rem; padding: 0.6rem 0.8rem; border-radius: 10px; line-height: 1.3; }
.msg.user { margin-left: auto; background: #667eea; color: #fff; }
.msg.bot { margin-right: auto; background: #f0f1f5; color: #333; }
.input-row { display: flex; gap: 0.5rem; padding: 0.8rem; border-top: 1px solid #ececec; }
.input-row input { flex: 1; padding: 0.55rem; border: 1px solid #d8d8d8; border-radius: 8px; }
.input-row button { padding: 0.55rem 0.75rem; border: none; border-radius: 8px; background: #667eea; color: #fff; cursor: pointer; }
.input-row button:disabled { opacity: 0.6; cursor: default; }
</style>
