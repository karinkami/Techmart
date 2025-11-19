<template>
  <TransitionGroup name="toast" tag="div" class="toast-container">
    <div
      v-for="toast in toasts"
      :key="toast.id"
      :class="['toast', `toast-${toast.type}`]"
    >
      <span class="toast-icon">{{ toast.icon }}</span>
      <span class="toast-message">{{ toast.message }}</span>
      <button @click="removeToast(toast.id)" class="toast-close">×</button>
    </div>
  </TransitionGroup>
</template>

<script>
export default {
  name: 'Toast',
  data() {
    return {
      toasts: []
    }
  },
  methods: {
    show(message, type = 'info', duration = 3000) {
      const icons = {
        success: '✅',
        error: '❌',
        warning: '⚠️',
        info: 'ℹ️'
      }
      
      const toast = {
        id: Date.now() + Math.random(),
        message,
        type,
        icon: icons[type] || icons.info
      }
      
      this.toasts.push(toast)
      
      if (duration > 0) {
        setTimeout(() => {
          this.removeToast(toast.id)
        }, duration)
      }
      
      return toast.id
    },
    removeToast(id) {
      const index = this.toasts.findIndex(t => t.id === id)
      if (index > -1) {
        this.toasts.splice(index, 1)
      }
    },
    success(message, duration) {
      return this.show(message, 'success', duration)
    },
    error(message, duration) {
      return this.show(message, 'error', duration)
    },
    warning(message, duration) {
      return this.show(message, 'warning', duration)
    },
    info(message, duration) {
      return this.show(message, 'info', duration)
    }
  }
}
</script>

<style scoped>
.toast-container {
  position: fixed;
  top: 20px;
  right: 20px;
  z-index: 10000;
  display: flex;
  flex-direction: column;
  gap: 0.75rem;
  max-width: 400px;
  pointer-events: none;
}

.toast {
  display: flex;
  align-items: center;
  gap: 0.75rem;
  padding: 1rem 1.25rem;
  background: white;
  border-radius: 12px;
  box-shadow: 0 4px 20px rgba(0, 0, 0, 0.15);
  pointer-events: auto;
  animation: slideIn 0.3s ease-out;
  min-width: 300px;
}

.toast-icon {
  font-size: 1.5rem;
  flex-shrink: 0;
}

.toast-message {
  flex: 1;
  font-size: 0.95rem;
  color: #333;
  line-height: 1.4;
}

.toast-close {
  background: none;
  border: none;
  font-size: 1.5rem;
  color: #999;
  cursor: pointer;
  padding: 0;
  width: 24px;
  height: 24px;
  display: flex;
  align-items: center;
  justify-content: center;
  border-radius: 50%;
  transition: all 0.2s;
  flex-shrink: 0;
}

.toast-close:hover {
  background: rgba(0, 0, 0, 0.1);
  color: #333;
}

.toast-success {
  border-left: 4px solid #27ae60;
  background: linear-gradient(135deg, #ffffff 0%, #f0fdf4 100%);
}

.toast-error {
  border-left: 4px solid #e74c3c;
  background: linear-gradient(135deg, #ffffff 0%, #fef2f2 100%);
}

.toast-warning {
  border-left: 4px solid #f39c12;
  background: linear-gradient(135deg, #ffffff 0%, #fffbeb 100%);
}

.toast-info {
  border-left: 4px solid #3498db;
  background: linear-gradient(135deg, #ffffff 0%, #eff6ff 100%);
}

@keyframes slideIn {
  from {
    transform: translateX(400px);
    opacity: 0;
  }
  to {
    transform: translateX(0);
    opacity: 1;
  }
}

.toast-leave-active {
  transition: all 0.3s ease-in;
}

.toast-leave-to {
  transform: translateX(400px);
  opacity: 0;
}

@media (max-width: 768px) {
  .toast-container {
    right: 10px;
    left: 10px;
    max-width: none;
  }
  
  .toast {
    min-width: auto;
  }
}
</style>

