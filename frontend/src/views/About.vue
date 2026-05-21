<template>
  <div class="about">
    <div class="about-hero">
      <div class="container">
        <h1>О нас</h1>
        <p class="hero-subtitle">Ваш надежный партнер в мире техники</p>
      </div>
    </div>
    
    <div class="container" style="position: relative; z-index: 1;">
      <section class="about-section">
        <div class="section-content">
          <div class="section-text">
            <h2>TechnMart - ваш надежный партнер в мире техники</h2>
            <p>
              TechnMart - это современный интернет-магазин техники, который предлагает широкий 
              ассортимент качественных товаров от ведущих производителей. Мы работаем с 2020 года 
              и за это время заслужили доверие тысяч клиентов по всей стране.
            </p>
            <p>
              Наша команда профессионалов всегда готова помочь вам выбрать именно то, что нужно. 
              Мы тщательно отбираем каждый товар, чтобы предложить вам только лучшее.
            </p>
          </div>
          <div class="section-image">
            <div class="image-placeholder">
              <svg viewBox="0 0 300 300" xmlns="http://www.w3.org/2000/svg">
                <defs>
                  <linearGradient id="aboutGrad" x1="0%" y1="0%" x2="100%" y2="100%">
                    <stop offset="0%" style="stop-color:#667eea;stop-opacity:1" />
                    <stop offset="100%" style="stop-color:#764ba2;stop-opacity:1" />
                  </linearGradient>
                </defs>
                <rect width="300" height="300" fill="url(#aboutGrad)" rx="20"/>
                <circle cx="150" cy="100" r="40" fill="white" opacity="0.3"/>
                <rect x="80" y="160" width="140" height="60" rx="10" fill="white" opacity="0.3"/>
                <rect x="100" y="175" width="100" height="30" rx="5" fill="white" opacity="0.5"/>
              </svg>
            </div>
          </div>
        </div>
      </section>

      <section class="about-section">
        <h2>Наши преимущества</h2>
        <div class="advantages-grid">
          <div class="advantage-item">
            <div class="advantage-icon">📦</div>
            <h3>Огромный выбор</h3>
            <p>Техника различных категорий от ведущих производителей</p>
          </div>
          <div class="advantage-item">
            <div class="advantage-icon">✅</div>
            <h3>Оригинальная продукция</h3>
            <p>Только оригинальная продукция с официальной гарантией</p>
          </div>
          <div class="advantage-item">
            <div class="advantage-icon">🚚</div>
            <h3>Быстрая доставка</h3>
            <p>Доставка по всей стране в кратчайшие сроки</p>
          </div>
          <div class="advantage-item">
            <div class="advantage-icon">💬</div>
            <h3>Профессиональная поддержка</h3>
            <p>Консультация и поддержка на всех этапах покупки</p>
          </div>
          <div class="advantage-item">
            <div class="advantage-icon">💰</div>
            <h3>Выгодные цены</h3>
            <p>Специальные предложения и регулярные акции</p>
          </div>
          <div class="advantage-item">
            <div class="advantage-icon">💳</div>
            <h3>Удобная оплата</h3>
            <p>Различные способы оплаты для вашего удобства</p>
          </div>
        </div>
      </section>

      <section class="about-section mission-section">
        <div class="mission-content">
          <div class="mission-image">
            <div class="image-placeholder">
              <svg viewBox="0 0 250 250" xmlns="http://www.w3.org/2000/svg">
                <defs>
                  <linearGradient id="missionGrad" x1="0%" y1="0%" x2="100%" y2="100%">
                    <stop offset="0%" style="stop-color:#27ae60;stop-opacity:1" />
                    <stop offset="100%" style="stop-color:#229954;stop-opacity:1" />
                  </linearGradient>
                </defs>
                <rect width="250" height="250" fill="url(#missionGrad)" rx="20"/>
                <circle cx="125" cy="80" r="35" fill="white" opacity="0.3"/>
                <path d="M 80 140 L 125 180 L 170 120" stroke="white" stroke-width="8" fill="none" stroke-linecap="round" stroke-linejoin="round" opacity="0.5"/>
              </svg>
            </div>
          </div>
          <div class="mission-text">
            <h2>Наша миссия</h2>
            <p>
              Мы стремимся сделать покупку техники максимально простой и удобной для каждого 
              клиента. Наша цель - предоставить качественные товары по доступным ценам с 
              отличным сервисом.
            </p>
            <p>
              Каждый день мы работаем над тем, чтобы улучшить ваш опыт покупок и сделать 
              процесс выбора техники максимально комфортным.
            </p>
          </div>
        </div>
      </section>

      <section class="about-section contacts-section">
        <h2>Свяжитесь с нами</h2>
        <div class="contacts-grid">
          <div class="contact-card">
            <div class="contact-icon">📧</div>
            <h3>Email</h3>
            <p>info@techmart.ru</p>
          </div>
          <div class="contact-card">
            <div class="contact-icon">📞</div>
            <h3>Телефон</h3>
            <p>+7 (800) 123-45-67</p>
          </div>
          <div class="contact-card">
            <div class="contact-icon">📍</div>
            <h3>Адрес</h3>
            <p>г. Москва, ул. Примерная, д. 1</p>
          </div>
        </div>
      </section>
    </div>
  </div>
</template>

<script>
export default {
  name: 'About'
}
</script>

<style scoped>
.about {
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
.snowflake:nth-child(16) { left: 75%; animation-duration: 11s; animation-delay: 0.4s; font-size: 1.4rem; }
.snowflake:nth-child(17) { left: 85%; animation-duration: 13s; animation-delay: 1.4s; font-size: 1.2rem; }
.snowflake:nth-child(18) { left: 5%; animation-duration: 15s; animation-delay: 2.4s; font-size: 1.3rem; }
.snowflake:nth-child(19) { left: 95%; animation-duration: 10s; animation-delay: 0.6s; font-size: 1.1rem; }
.snowflake:nth-child(20) { left: 8%; animation-duration: 12s; animation-delay: 1.6s; font-size: 1.5rem; }
.snowflake:nth-child(21) { left: 18%; animation-duration: 14s; animation-delay: 2.6s; font-size: 1.2rem; }
.snowflake:nth-child(22) { left: 28%; animation-duration: 11s; animation-delay: 0.8s; font-size: 1.3rem; }
.snowflake:nth-child(23) { left: 38%; animation-duration: 13s; animation-delay: 1.8s; font-size: 1.1rem; }
.snowflake:nth-child(24) { left: 48%; animation-duration: 15s; animation-delay: 2.8s; font-size: 1.4rem; }

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

.about-hero {
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
  color: white;
  padding: 5rem 0;
  text-align: center;
  margin-bottom: 4rem;
  position: relative;
  overflow: hidden;
}


.about-hero .container {
  position: relative;
  z-index: 1;
}


.about-hero h1 {
  font-size: 3.5rem;
  margin-bottom: 1rem;
  text-shadow: 0 2px 10px rgba(0,0,0,0.2);
  font-weight: 700;
}

.hero-subtitle {
  font-size: 1.5rem;
  opacity: 0.95;
  font-weight: 500;
}

.about-section {
  margin-bottom: 4rem;
  background: transparent;
  padding: 3.5rem;
  border-radius: 25px;
  box-shadow: 0 4px 15px rgba(0,0,0,0.05);
  transition: all 0.4s cubic-bezier(0.175, 0.885, 0.32, 1.275);
  border: 2px solid rgba(102, 126, 234, 0.15);
  position: relative;
  overflow: hidden;
}

.about-section::before {
  content: '';
  position: absolute;
  top: 0;
  left: 0;
  right: 0;
  height: 5px;
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
  transform: scaleX(0);
  transition: transform 0.4s;
}

.about-section:hover::before {
  transform: scaleX(1);
}

.about-section:hover {
  box-shadow: 0 15px 40px rgba(102, 126, 234, 0.15);
  transform: translateY(-8px);
  border-color: rgba(102, 126, 234, 0.3);
  background: rgba(255, 255, 255, 0.3);
}

.section-content {
  display: grid;
  grid-template-columns: 1fr 1fr;
  gap: 3rem;
  align-items: center;
}

.section-image {
  display: flex;
  justify-content: center;
  align-items: center;
}

.image-placeholder {
  width: 100%;
  max-width: 350px;
  height: 350px;
  border-radius: 25px;
  overflow: hidden;
  box-shadow: 0 15px 40px rgba(102, 126, 234, 0.25);
  transition: transform 0.3s;
  animation: floatImage 3s ease-in-out infinite;
}

.image-placeholder:hover {
  transform: scale(1.05) rotate(2deg);
}

@keyframes floatImage {
  0%, 100% { transform: translateY(0); }
  50% { transform: translateY(-15px); }
}

.image-placeholder svg {
  width: 100%;
  height: 100%;
}

.section-text p {
  margin-bottom: 1rem;
}

.about-section h2 {
  color: #667eea;
  margin-bottom: 1.5rem;
  font-size: 2.2rem;
  font-weight: 700;
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
  -webkit-background-clip: text;
  -webkit-text-fill-color: transparent;
  background-clip: text;
}

.about-section p {
  color: #666;
  line-height: 1.8;
  font-size: 1.1rem;
}

.advantages-grid {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(280px, 1fr));
  gap: 2rem;
  margin-top: 2rem;
}

.advantage-item {
  text-align: center;
  padding: 2.5rem;
  background: linear-gradient(135deg, #ffffff 0%, #f8f9ff 100%);
  border-radius: 20px;
  border: 2px solid rgba(102, 126, 234, 0.1);
  transition: all 0.4s cubic-bezier(0.175, 0.885, 0.32, 1.275);
  position: relative;
  overflow: hidden;
}


.advantage-item:hover {
  transform: translateY(-10px) scale(1.03);
  box-shadow: 0 12px 35px rgba(102, 126, 234, 0.15);
  border-color: rgba(102, 126, 234, 0.4);
  background: rgba(255, 255, 255, 0.3);
}

.advantage-icon {
  font-size: 3.5rem;
  margin-bottom: 1.2rem;
  display: block;
  transition: transform 0.2s;
  filter: drop-shadow(0 4px 8px rgba(102, 126, 234, 0.2));
  animation: float 2s ease-in-out infinite;
}

.advantage-item:hover .advantage-icon {
  transform: scale(1.2) rotate(10deg);
}

@keyframes float {
  0%, 100% { transform: translateY(0); }
  50% { transform: translateY(-8px); }
}

.advantage-item h3 {
  color: #667eea;
  margin-bottom: 0.5rem;
  font-size: 1.2rem;
  font-weight: 600;
}

.advantage-item p {
  color: #666;
  font-size: 0.95rem;
  line-height: 1.6;
  margin: 0;
}

.mission-section {
  background: transparent;
}

.mission-content {
  display: grid;
  grid-template-columns: 1fr 1fr;
  gap: 3rem;
  align-items: center;
}

.mission-image {
  order: 2;
}

.mission-text {
  order: 1;
}

.mission-text p {
  margin-bottom: 1rem;
}

.contacts-section {
  text-align: center;
}

.contacts-grid {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(250px, 1fr));
  gap: 2rem;
  margin-top: 2rem;
}

.contact-card {
  background: linear-gradient(135deg, #ffffff 0%, #f8f9ff 100%);
  padding: 3rem 2.5rem;
  border-radius: 20px;
  border: 2px solid rgba(102, 126, 234, 0.1);
  transition: all 0.4s cubic-bezier(0.175, 0.885, 0.32, 1.275);
  position: relative;
  overflow: hidden;
}

.contact-card::before {
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

.contact-card:hover::before {
  transform: scaleX(1);
}

.contact-card:hover {
  transform: translateY(-10px) scale(1.03);
  box-shadow: 0 12px 35px rgba(102, 126, 234, 0.15);
  border-color: rgba(102, 126, 234, 0.4);
  background: rgba(255, 255, 255, 0.3);
}

.contact-icon {
  font-size: 3.5rem;
  margin-bottom: 1.2rem;
  display: block;
  transition: transform 0.3s;
  filter: drop-shadow(0 4px 8px rgba(102, 126, 234, 0.2));
}

.contact-card:hover .contact-icon {
  transform: scale(1.2) rotate(10deg);
}

.contact-card h3 {
  color: #667eea;
  margin-bottom: 0.5rem;
  font-size: 1.3rem;
  font-weight: 600;
}

.contact-card p {
  color: #666;
  font-size: 1.1rem;
  margin: 0;
}

@media (max-width: 768px) {
  .section-content,
  .mission-content {
    grid-template-columns: 1fr;
  }
  
  .mission-image {
    order: 1;
  }
  
  .mission-text {
    order: 2;
  }
}
</style>

