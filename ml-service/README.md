# ML-сервис TechMart

FastAPI-сервис: рекомендации товаров, чат-бот с классификацией интентов, приём событий поведения и опциональный consumer Redis-очереди.

Общая настройка проекта и первый запуск — в [корневом README](../README.md).

## Эндпоинты

| Метод | Путь | Описание |
|-------|------|----------|
| GET | `/health` | Статус сервиса |
| POST | `/events` | Событие пользователя |
| POST | `/recommend` | Рекомендации (`user_id`, `limit`) |
| POST | `/chat` | Ответ бота (`text`) |
| POST | `/train` | Переобучение на накопленных событиях |

## Запуск

```powershell
cd ml-service
python -m venv .venv
.venv\Scripts\Activate.ps1
pip install -r requirements.txt
copy .env.example .env
uvicorn app:app --host 0.0.0.0 --port 8000 --env-file .env
```

## Переменные окружения (`.env`)

| Переменная | По умолчанию | Описание |
|------------|--------------|----------|
| `REDIS_ENABLED` | `false` | Включить consumer очереди |
| `REDIS_URL` | `redis://localhost:6379/0` | Подключение к Redis |
| `REDIS_QUEUE_KEY` | `user-events` | Ключ списка событий |

Значения должны совпадать с `Redis` в `TechMart.Api/appsettings.json`.

## Пакетное обучение

Для обновления модели рекомендаций и интентов чата:

```powershell
curl -X POST http://localhost:8000/train
```

## Данные

- `data/events.json` — накопленные события поведения
- `data/chat_intents.json` — обучающая выборка для чат-бота
