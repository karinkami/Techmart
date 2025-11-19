<template>
  <div class="home">
    <div class="snowflakes">
      <div class="snowflake">❄</div>
      <div class="snowflake">❅</div>
      <div class="snowflake">❆</div>
      <div class="snowflake">❄</div>
      <div class="snowflake">❅</div>
      <div class="snowflake">❆</div>
      <div class="snowflake">❄</div>
      <div class="snowflake">❅</div>
      <div class="snowflake">❆</div>
      <div class="snowflake">❄</div>
      <div class="snowflake">❅</div>
      <div class="snowflake">❆</div>
      <div class="snowflake">❄</div>
      <div class="snowflake">❅</div>
      <div class="snowflake">❆</div>
      <div class="snowflake">❄</div>
      <div class="snowflake">❅</div>
      <div class="snowflake">❆</div>
      <div class="snowflake">❄</div>
      <div class="snowflake">❅</div>
      <div class="snowflake">❆</div>
      <div class="snowflake">❄</div>
      <div class="snowflake">❅</div>
      <div class="snowflake">❆</div>
    </div>
    <div class="hero">
      <div class="container">
        <div class="hero-content">
          <div class="hero-text">
            <h1>Добро пожаловать в TechMart</h1>
            <p class="subtitle">Лучший интернет-магазин техники с гарантией качества</p>
            <p class="description">Тысячи товаров от ведущих производителей. Быстрая доставка. Официальная гарантия.</p>
            <router-link to="/catalog" class="btn-primary">Перейти в каталог</router-link>
          </div>
          <div class="hero-image">
            <div class="hero-placeholder">
              <svg viewBox="0 0 200 200" xmlns="http://www.w3.org/2000/svg">
                <defs>
                  <linearGradient id="grad1" x1="0%" y1="0%" x2="100%" y2="100%">
                    <stop offset="0%" style="stop-color:#667eea;stop-opacity:1" />
                    <stop offset="100%" style="stop-color:#764ba2;stop-opacity:1" />
                  </linearGradient>
                </defs>
                <rect width="200" height="200" fill="url(#grad1)" rx="20"/>
                <circle cx="100" cy="80" r="30" fill="white" opacity="0.3"/>
                <rect x="60" y="120" width="80" height="40" rx="5" fill="white" opacity="0.3"/>
                <rect x="70" y="130" width="60" height="20" rx="3" fill="white" opacity="0.5"/>
              </svg>
            </div>
          </div>
        </div>
      </div>
    </div>
    
    <div class="container" style="position: relative; z-index: 1;">
      <section class="features">
        <h2>Почему выбирают нас?</h2>
        <div class="features-grid">
          <div class="feature-card">
            <div class="feature-icon">📦</div>
            <h3>Широкий ассортимент</h3>
            <p>Тысячи товаров от ведущих производителей. Ноутбуки, смартфоны, планшеты и многое другое</p>
          </div>
          <div class="feature-card">
            <div class="feature-icon">🚚</div>
            <h3>Доставка по всей стране</h3>
            <p>Быстрая и надежная доставка. Получите заказ уже на следующий день в большинстве городов</p>
          </div>
          <div class="feature-card">
            <div class="feature-icon">✅</div>
            <h3>Гарантия качества</h3>
            <p>Официальная гарантия на все товары. Профессиональная поддержка и сервис</p>
          </div>
          <div class="feature-card">
            <div class="feature-icon">💰</div>
            <h3>Выгодные цены</h3>
            <p>Лучшие цены на рынке. Регулярные акции и специальные предложения для наших клиентов</p>
          </div>
          <div class="feature-card">
            <div class="feature-icon">🔒</div>
            <h3>Безопасные покупки</h3>
            <p>Защищенные платежи. Ваши данные в безопасности. Удобные способы оплаты</p>
          </div>
          <div class="feature-card">
            <div class="feature-icon">🎁</div>
            <h3>Бонусная программа</h3>
            <p>Копите бонусы за покупки и получайте скидки. Специальные предложения для постоянных клиентов</p>
          </div>
        </div>
      </section>

      <section class="categories-preview">
        <h2>Популярные категории</h2>
        <div class="categories-grid">
          <router-link to="/catalog" class="category-card">
            <div class="category-icon">💻</div>
            <h3>Ноутбуки</h3>
          </router-link>
          <router-link to="/catalog" class="category-card">
            <div class="category-icon">📱</div>
            <h3>Смартфоны</h3>
          </router-link>
          <router-link to="/catalog" class="category-card">
            <div class="category-icon">📱</div>
            <h3>Планшеты</h3>
          </router-link>
          <router-link to="/catalog" class="category-card">
            <div class="category-icon">🎧</div>
            <h3>Наушники</h3>
          </router-link>
          <router-link to="/catalog" class="category-card">
            <div class="category-icon">🖥️</div>
            <h3>Мониторы</h3>
          </router-link>
        </div>
      </section>
    </div>
  </div>
</template>

<script>
export default {
  name: 'Home'
}
</script>

<style scoped>
.home {
  width: 100%;
  position: relative;
  min-height: 100vh;
}

.snowflakes {
  position: fixed;
  top: 0;
  left: 0;
  width: 100vw;
  height: 100vh;
  pointer-events: none;
  z-index: 1000;
  overflow: visible;
}

.hero {
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
  color: white;
  padding: 6rem 0;
  position: relative;
  overflow: hidden;
}

.snowflake {
  position: absolute;
  top: -50px;
  color: rgba(255, 255, 255, 0.9);
  font-size: 1.5rem;
  animation: snowfall linear infinite;
  text-shadow: 0 0 8px rgba(255, 255, 255, 0.8), 0 0 15px rgba(255, 255, 255, 0.5);
  filter: drop-shadow(0 0 3px rgba(255, 255, 255, 0.6));
}

.snowflake:nth-child(1) { left: 10%; animation-duration: 10s; animation-delay: 0s; font-size: 1.2rem; }
.snowflake:nth-child(2) { left: 20%; animation-duration: 12s; animation-delay: 1s; font-size: 1.5rem; }
.snowflake:nth-child(3) { left: 30%; animation-duration: 14s; animation-delay: 2s; font-size: 1rem; }
.snowflake:nth-child(4) { left: 40%; animation-duration: 11s; animation-delay: 0.5s; font-size: 1.3rem; }
.snowflake:nth-child(5) { left: 50%; animation-duration: 13s; animation-delay: 1.5s; font-size: 1.4rem; }
.snowflake:nth-child(6) { left: 60%; animation-duration: 15s; animation-delay: 2.5s; font-size: 1.1rem; }
.snowflake:nth-child(7) { left: 70%; animation-duration: 10s; animation-delay: 0.3s; font-size: 1.6rem; }
.snowflake:nth-child(8) { left: 80%; animation-duration: 12s; animation-delay: 1.2s; font-size: 1.2rem; }
.snowflake:nth-child(9) { left: 90%; animation-duration: 14s; animation-delay: 2.2s; font-size: 1.3rem; }
.snowflake:nth-child(10) { left: 15%; animation-duration: 11s; animation-delay: 0.7s; font-size: 1.4rem; }
.snowflake:nth-child(11) { left: 25%; animation-duration: 13s; animation-delay: 1.7s; font-size: 1rem; }
.snowflake:nth-child(12) { left: 35%; animation-duration: 15s; animation-delay: 2.7s; font-size: 1.5rem; }
.snowflake:nth-child(13) { left: 45%; animation-duration: 10s; animation-delay: 0.2s; font-size: 1.2rem; }
.snowflake:nth-child(14) { left: 55%; animation-duration: 12s; animation-delay: 1.3s; font-size: 1.3rem; }
.snowflake:nth-child(15) { left: 65%; animation-duration: 14s; animation-delay: 2.3s; font-size: 1.1rem; }

@keyframes snowfall {
  0% {
    transform: translateY(-50px) translateX(0) rotate(0deg);
    opacity: 0;
  }
  5% {
    opacity: 1;
  }
  95% {
    opacity: 1;
  }
  100% {
    transform: translateY(100vh) translateX(30px) rotate(360deg);
    opacity: 0;
  }
}


.hero .container {
  position: relative;
  z-index: 1;
}

.hero-content {
  display: grid;
  grid-template-columns: 1fr 1fr;
  gap: 3rem;
  align-items: center;
}

.hero-text {
  text-align: left;
}

.hero h1 {
  font-size: 3.5rem;
  margin-bottom: 1rem;
  text-shadow: 0 2px 10px rgba(0,0,0,0.2);
  animation: fadeInUp 0.8s ease-out;
  line-height: 1.2;
}

.subtitle {
  font-size: 1.5rem;
  margin-bottom: 1rem;
  opacity: 0.95;
  animation: fadeInUp 0.8s ease-out 0.2s both;
  font-weight: 600;
}

.description {
  font-size: 1.1rem;
  margin-bottom: 2rem;
  opacity: 0.9;
  line-height: 1.6;
  animation: fadeInUp 0.8s ease-out 0.3s both;
}

.hero-image {
  display: flex;
  justify-content: center;
  align-items: center;
  animation: fadeInUp 0.8s ease-out 0.4s both;
}

.hero-placeholder {
  width: 100%;
  max-width: 450px;
  height: 450px;
  border-radius: 30px;
  overflow: hidden;
  box-shadow: 0 25px 70px rgba(0,0,0,0.4);
  background: rgba(255,255,255,0.15);
  backdrop-filter: blur(15px);
  animation: floatImage 3s ease-in-out infinite;
  position: relative;
}

@keyframes floatImage {
  0%, 100% { transform: translateY(0) rotate(0deg); }
  50% { transform: translateY(-20px) rotate(2deg); }
}

.hero-placeholder svg {
  width: 100%;
  height: 100%;
}

.btn-primary {
  display: inline-block;
  padding: 1.2rem 2.5rem;
  background: white;
  color: #667eea;
  text-decoration: none;
  border-radius: 50px;
  font-weight: bold;
  font-size: 1.1rem;
  transition: all 0.3s;
  box-shadow: 0 4px 15px rgba(0,0,0,0.2);
  animation: fadeInUp 0.8s ease-out 0.4s both;
}

.btn-primary:hover {
  transform: translateY(-3px) scale(1.05);
  box-shadow: 0 6px 20px rgba(0,0,0,0.3);
}

@keyframes fadeInUp {
  from {
    opacity: 0;
    transform: translateY(30px);
  }
  to {
    opacity: 1;
    transform: translateY(0);
  }
}

.features {
  margin-top: 4rem;
}

.features h2 {
  text-align: center;
  font-size: 2.8rem;
  margin-bottom: 3rem;
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
  -webkit-background-clip: text;
  -webkit-text-fill-color: transparent;
  background-clip: text;
  font-weight: 700;
}

.features-grid {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(300px, 1fr));
  gap: 2rem;
}

.feature-card {
  background: transparent;
  padding: 2.5rem;
  border-radius: 25px;
  box-shadow: 0 4px 15px rgba(0,0,0,0.05);
  text-align: center;
  transition: all 0.4s cubic-bezier(0.175, 0.885, 0.32, 1.275);
  border: 2px solid rgba(102, 126, 234, 0.15);
  position: relative;
  overflow: hidden;
}

.feature-card::before {
  content: '';
  position: absolute;
  top: 0;
  left: 0;
  right: 0;
  height: 4px;
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
  transform: scaleX(0);
  transition: transform 0.3s;
}

.feature-card:hover::before {
  transform: scaleX(1);
}

.feature-card:hover {
  transform: translateY(-15px) scale(1.02);
  box-shadow: 0 15px 40px rgba(102, 126, 234, 0.2);
  border-color: rgba(102, 126, 234, 0.4);
  background: rgba(255, 255, 255, 0.3);
}

.feature-icon {
  font-size: 4.5rem;
  margin-bottom: 1.5rem;
  display: block;
  animation: float 2s ease-in-out infinite;
  filter: drop-shadow(0 4px 8px rgba(102, 126, 234, 0.2));
  transition: transform 0.2s;
}

.feature-card:hover .feature-icon {
  transform: scale(1.1) rotate(5deg);
}

.feature-card:nth-child(2) .feature-icon {
  animation-delay: 0.2s;
}

.feature-card:nth-child(3) .feature-icon {
  animation-delay: 0.4s;
}

.feature-card:nth-child(4) .feature-icon {
  animation-delay: 0.6s;
}

.feature-card:nth-child(5) .feature-icon {
  animation-delay: 0.8s;
}

.feature-card:nth-child(6) .feature-icon {
  animation-delay: 1s;
}

@keyframes float {
  0%, 100% {
    transform: translateY(0);
  }
  50% {
    transform: translateY(-10px);
  }
}

.feature-card h3 {
  color: #667eea;
  margin-bottom: 1rem;
  font-size: 1.5rem;
  font-weight: 600;
}

.feature-card p {
  color: #666;
  line-height: 1.8;
  font-size: 1rem;
}

.categories-preview {
  margin-top: 5rem;
  padding-top: 4rem;
  border-top: 2px solid rgba(102, 126, 234, 0.1);
}

.categories-preview h2 {
  text-align: center;
  font-size: 2.8rem;
  margin-bottom: 3rem;
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
  -webkit-background-clip: text;
  -webkit-text-fill-color: transparent;
  background-clip: text;
  font-weight: 700;
}

.categories-grid {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(180px, 1fr));
  gap: 1.5rem;
}

.category-card {
  background: transparent;
  padding: 2.5rem 2rem;
  border-radius: 20px;
  box-shadow: 0 4px 15px rgba(0,0,0,0.05);
  text-align: center;
  text-decoration: none;
  transition: all 0.4s cubic-bezier(0.175, 0.885, 0.32, 1.275);
  border: 2px solid rgba(102, 126, 234, 0.15);
  position: relative;
  overflow: hidden;
}


.category-card:hover {
  transform: translateY(-12px) scale(1.05);
  box-shadow: 0 12px 35px rgba(102, 126, 234, 0.2);
  border-color: #667eea;
  background: rgba(255, 255, 255, 0.3);
}

.category-icon {
  font-size: 4rem;
  margin-bottom: 1.2rem;
  display: block;
  transition: transform 0.3s;
  filter: drop-shadow(0 4px 8px rgba(102, 126, 234, 0.2));
}

.category-card:hover .category-icon {
  transform: scale(1.2) rotate(10deg);
}

.category-card h3 {
  color: #667eea;
  font-size: 1.2rem;
  font-weight: 600;
  margin: 0;
}

@media (max-width: 768px) {
  .hero-content {
    grid-template-columns: 1fr;
    text-align: center;
  }
  
  .hero-text {
    text-align: center;
  }
  
  .hero h1 {
    font-size: 2.5rem;
  }
}
</style>

