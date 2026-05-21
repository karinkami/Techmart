from __future__ import annotations

import json
import logging
import os
import threading
from collections import Counter, defaultdict
from datetime import datetime, timezone
from pathlib import Path
from typing import Any

from fastapi import FastAPI
from pydantic import BaseModel
from sklearn.feature_extraction.text import TfidfVectorizer
from sklearn.linear_model import LogisticRegression
from sklearn.pipeline import Pipeline
from sklearn.preprocessing import OneHotEncoder
from sklearn.compose import ColumnTransformer

try:
    import redis
except Exception:
    redis = None


DATA_DIR = Path(__file__).parent / "data"
EVENTS_PATH = DATA_DIR / "events.json"
CHAT_DATASET_PATH = DATA_DIR / "chat_intents.json"

DATA_DIR.mkdir(parents=True, exist_ok=True)

app = FastAPI(title="TechMart ML Service")


INTENT_MODEL: Pipeline | None = None
RECOMMENDER_CACHE: Any = None
EVENTS_LOCK = threading.Lock()
REDIS_STOP_EVENT = threading.Event()
REDIS_THREAD: threading.Thread | None = None
LOGGER = logging.getLogger("techmart.ml")


class EventPayload(BaseModel):
    user_id: int
    event_type: str
    product_id: int | None = None
    quantity: int | None = None
    timestamp: datetime | None = None


class RecommendPayload(BaseModel):
    user_id: int
    limit: int = 5


class ChatPayload(BaseModel):
    text: str


def _load_events() -> list[dict[str, Any]]:
    if not EVENTS_PATH.exists():
        return []
    return json.loads(EVENTS_PATH.read_text(encoding="utf-8"))


def _save_events(events: list[dict[str, Any]]) -> None:
    EVENTS_PATH.write_text(
        json.dumps(events, ensure_ascii=False, indent=2),
        encoding="utf-8"
    )


def _event_to_record(payload: EventPayload) -> dict[str, Any]:
    return {
        "user_id": payload.user_id,
        "event_type": payload.event_type,
        "product_id": payload.product_id,
        "quantity": payload.quantity,
        "timestamp": (payload.timestamp or datetime.now(timezone.utc)).isoformat(),
    }


def _append_event_record(record: dict[str, Any]) -> None:
    global RECOMMENDER_CACHE
    with EVENTS_LOCK:
        events = _load_events()
        events.append(record)
        _save_events(events)
        RECOMMENDER_CACHE = None


def _is_redis_enabled() -> bool:
    return os.getenv("REDIS_ENABLED", "false").strip().lower() in {"1", "true", "yes", "on"}


def _redis_consumer_loop() -> None:
    if redis is None:
        LOGGER.warning("Redis is enabled, but redis package is not installed.")
        return

    redis_url = os.getenv("REDIS_URL", "redis://localhost:6379/0")
    queue_key = os.getenv("REDIS_QUEUE_KEY", "user-events")

    try:
        client = redis.from_url(redis_url, decode_responses=True)
        client.ping()
    except Exception as ex:
        LOGGER.warning("Redis consumer init failed: %s", ex)
        return

    LOGGER.info("Redis consumer started for key '%s'", queue_key)
    try:
        while not REDIS_STOP_EVENT.is_set():
            try:
                item = client.brpop(queue_key, timeout=1)
                if not item:
                    continue
                _, raw_payload = item
                payload = json.loads(raw_payload) if raw_payload else {}
                record = {
                    "user_id": int(payload.get("user_id")),
                    "event_type": str(payload.get("event_type")),
                    "product_id": payload.get("product_id"),
                    "quantity": payload.get("quantity"),
                    "timestamp": payload.get("timestamp") or datetime.now(timezone.utc).isoformat(),
                }
                _append_event_record(record)
            except Exception as ex:
                LOGGER.warning("Redis consume error: %s", ex)
    finally:
        LOGGER.info("Redis consumer stopped")


def _load_chat_dataset():
    if not CHAT_DATASET_PATH.exists():
        raise RuntimeError("chat_intents.json не найден")
    return json.loads(CHAT_DATASET_PATH.read_text(encoding="utf-8"))


def _train_intent_model() -> Pipeline:
    dataset = _load_chat_dataset()
    train_samples: list[tuple[str, str]] = []
    for intent in dataset.get("intents", []):
        intent_name = intent.get("name")
        for sample in intent.get("samples", []):
            if intent_name and sample:
                train_samples.append((str(sample), str(intent_name)))

    if not train_samples:
        train_samples = [("где мой заказ", "order_status"), ("как вернуть товар", "return")]
    x_train = [x for x, _ in train_samples]
    y_train = [y for _, y in train_samples]
    model = Pipeline(
        [
            ("tfidf", TfidfVectorizer(ngram_range=(1, 2), min_df=1)),
            ("clf", LogisticRegression(max_iter=500)),
        ]
    )
    model.fit(x_train, y_train)
    return model

INTENT_MODEL = _train_intent_model()


def _chat_reply(intent: str) -> str:
    replies = {
        "order_status": "Проверить статус заказа можно в личном кабинете в разделе «Мои заказы».",
        "return": "Возврат можно оформить в личном кабинете: откройте «Мои заказы», выберите доставленный заказ и нажмите «Оформить возврат».",
        "payment": "Мы принимаем карты и онлайн-оплату. На этапе оформления вы увидите все способы.",
        "warranty": "На товары действует официальная гарантия производителя.",
    }
    return replies.get(intent, "Извините, я не понял. Попробуйте переформулировать")


def _chat_by_rules(text: str) -> tuple[str, float, str] | None:
    t = text.lower()

    if any(word in t for word in ["возврат", "вернуть", "отмен", "деньги обратно"]):
        return _chat_reply("return"), 0.9, "return"

    if any(word in t for word in ["заказ", "достав", "посыл", "трек"]):
        return _chat_reply("order_status"), 0.9, "order_status"

    if any(word in t for word in ["оплат", "карт", "рассроч", "платеж"]):
        return _chat_reply("payment"), 0.9, "payment"

    if any(word in t for word in ["гарант", "гарантия"]):
        return _chat_reply("warranty"), 0.9, "warranty"

    return None



def _build_user_item(events):
    user_item = defaultdict(set)

    for e in events:
        if e["event_type"] == "purchase":
            user_item[int(e["user_id"])].add(int(e["product_id"]))

    return user_item


def _popular(events, limit):
    c = Counter()

    for e in events:
        pid = e.get("product_id")
        if pid:
            c[int(pid)] += 3 if e["event_type"] == "purchase" else 1

    return [p for p, _ in c.most_common(limit)]


def _popular_excluding(events, limit, excluded: set[int] | None = None):
    excluded = excluded or set()
    ranked = _popular(events, max(limit + len(excluded), limit))
    return [pid for pid in ranked if pid not in excluded][:limit]


def _user_interactions(events, user_id: int) -> tuple[set[int], set[int]]:
    viewed = set()
    purchased = set()

    for e in events:
        if int(e.get("user_id", -1)) != user_id:
            continue

        pid = e.get("product_id")
        if not pid:
            continue

        pid = int(pid)
        if e.get("event_type") == "view":
            viewed.add(pid)
        if e.get("event_type") == "purchase":
            purchased.add(pid)

    return viewed, purchased


def _reason_for_recommendation(product_id: int, viewed: set[int], purchased: set[int]) -> str:
    if product_id in viewed:
        return "Вы уже смотрели этот товар"
    if product_id in purchased:
        return "Похоже на ваши предыдущие покупки"
    if viewed:
        return "Похоже на товары, которые вы недавно смотрели"
    if purchased:
        return "Похоже на ваши прошлые покупки"
    return "Популярный товар среди похожих пользователей"


def _build_recommendation_response(
    ml_ids: list[int],
    events: list[dict[str, Any]],
    limit: int,
    viewed: set[int],
    purchased: set[int],
) -> list[dict[str, Any]]:
    selected: list[tuple[int, str]] = []
    used = set()

    for pid in ml_ids:
        if pid in used:
            continue
        selected.append((pid, "ml"))
        used.add(pid)
        if len(selected) >= limit:
            break

    if len(selected) < limit:
        unseen_popular = _popular_excluding(events, limit, excluded=(viewed | purchased | used))
        for pid in unseen_popular:
            if pid in used:
                continue
            selected.append((pid, "popular_unseen"))
            used.add(pid)
            if len(selected) >= limit:
                break

    if len(selected) < limit:
        for pid in _popular(events, limit * 3):
            if pid in used:
                continue
            selected.append((pid, "popular_fallback"))
            used.add(pid)
            if len(selected) >= limit:
                break

    return [
        {
            "product_id": pid,
            "reason": _reason_for_recommendation(pid, viewed, purchased),
            "source": source,
        }
        for pid, source in selected
    ]


def _train_recommender(events):
    user_item = _build_user_item(events)

    users = list(user_item.keys())
    items = list({e["product_id"] for e in events if e.get("product_id")})

    X, y = [], []

    for u in users:
        for i in items:

            label = 1 if i in user_item[u] else 0

            X.append([
                len(user_item[u]),
                u,
                i,
            ])

            y.append(label)

    preprocessor = ColumnTransformer(
        transformers=[
            ('cat', OneHotEncoder(handle_unknown='ignore'), [1, 2])
        ],
        remainder='passthrough'
    )
    
    model = Pipeline([
        ('preprocessor', preprocessor),
        ('clf', LogisticRegression(max_iter=500))
    ])
    model.fit(X, y)

    return model, user_item, items


def _get_recommender(events):
    global RECOMMENDER_CACHE

    if RECOMMENDER_CACHE:
        return RECOMMENDER_CACHE

    RECOMMENDER_CACHE = _train_recommender(events)
    return RECOMMENDER_CACHE


@app.get("/health")
def health():
    return {"status": "ok"}


@app.on_event("startup")
def startup_event() -> None:
    global REDIS_THREAD
    if not _is_redis_enabled():
        return
    if REDIS_THREAD and REDIS_THREAD.is_alive():
        return
    REDIS_STOP_EVENT.clear()
    REDIS_THREAD = threading.Thread(target=_redis_consumer_loop, daemon=True, name="redis-consumer")
    REDIS_THREAD.start()


@app.on_event("shutdown")
def shutdown_event() -> None:
    REDIS_STOP_EVENT.set()


@app.post("/events")
def track_event(payload: EventPayload):
    _append_event_record(_event_to_record(payload))
    return {"status": "ok"}


@app.post("/recommend")
def recommend(payload: RecommendPayload):

    events = _load_events()
    viewed, purchased = _user_interactions(events, payload.user_id)
    user_seen = viewed | purchased

    if len(events) < 5:
        return {"recommendations": _build_recommendation_response([], events, payload.limit, viewed, purchased)}

    try:
        model, user_item, items = _get_recommender(events)
        user_items = user_item.get(payload.user_id, set())
        scored = []

        for i in items:
            if int(i) in user_seen:
                continue

            features = [[
                len(user_items),
                payload.user_id,
                i,
            ]]

            score = model.predict_proba(features)[0][1]
            scored.append((i, score))

        scored.sort(key=lambda x: x[1], reverse=True)
        top = scored[:payload.limit]
    except Exception:
        top = []
    ml_ids = [int(pid) for pid, _ in top]
    return {"recommendations": _build_recommendation_response(ml_ids, events, payload.limit, viewed, purchased)}

@app.post("/chat")
def chat(payload: ChatPayload) -> dict[str, Any]:
    text = payload.text.lower().strip()

    if not text:
        return {
            "reply": "Извините, я не понял. Попробуйте переформулировать",
            "confidence": 0.0,
            "intent": "unknown",
            "source": "none",
        }

    # ML
    probs = INTENT_MODEL.predict_proba([text])[0]
    classes = INTENT_MODEL.classes_
    max_index = int(probs.argmax())
    confidence = float(probs[max_index])
    intent = str(classes[max_index])

    if confidence >= 0.5:
        return {
            "reply": _chat_reply(intent),
            "confidence": confidence,
            "intent": intent,
            "source": "ml",
        }

    # RULES
    rule_result = _chat_by_rules(text)
    if rule_result:
        reply, rule_confidence, rule_intent = rule_result
        return {
            "reply": reply,
            "confidence": rule_confidence,
            "intent": rule_intent,
            "source": "rules",
        }

    # FALLBACK
    return {
        "reply": "Я не до конца уверен в ответе. Хотите связаться с оператором?",
        "confidence": confidence,
        "intent": intent,
        "source": "fallback",
    }


@app.post("/train")
def train():

    global INTENT_MODEL, RECOMMENDER_CACHE

    events = _load_events()

    INTENT_MODEL = _train_intent_model()
    RECOMMENDER_CACHE = _train_recommender(events)

    return {
        "status": "trained",
        "events": len(events),
        "ml_recommender": True
    }