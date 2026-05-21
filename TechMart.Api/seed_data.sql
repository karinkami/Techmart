-- Добавление тестовых данных

-- Категории
INSERT INTO "Categories" ("name") VALUES 
('Ноутбуки'),
('Смартфоны'),
('Планшеты'),
('Наушники'),
('Мониторы')
ON CONFLICT ("name") DO NOTHING;

-- Производители
INSERT INTO "Manufacturers" ("name") VALUES 
('Apple'),
('Samsung'),
('Lenovo'),
('HP'),
('Sony')
ON CONFLICT ("name") DO NOTHING;

-- Товары
INSERT INTO "Products" ("title", "description", "price", "catigory_id", "manufacturer_id", "image_url") VALUES 
('MacBook Pro 14"', 'Мощный ноутбук для профессионалов с процессором M3', 199999.00, 
 (SELECT "id" FROM "Categories" WHERE "name" = 'Ноутбуки'),
 (SELECT "id" FROM "Manufacturers" WHERE "name" = 'Apple'),
 'https://c.dns-shop.ru/thumb/st1/fit/0/0/0532d25b7de2547be7c23364cedf43bf/47f7e630d05e7f5fa016300cfbc4c23c7774510cf7b7275d8d32785bb3a31f1d.jpg.webp'),

('iPhone 15 Pro', 'Флагманский смартфон с титановым корпусом', 99999.00,
 (SELECT "id" FROM "Categories" WHERE "name" = 'Смартфоны'),
 (SELECT "id" FROM "Manufacturers" WHERE "name" = 'Apple'),
 'https://c.dns-shop.ru/thumb/st1/fit/0/0/0532d25b7de2547be7c23364cedf43bf/47f7e630d05e7f5fa016300cfbc4c23c7774510cf7b7275d8d32785bb3a31f1d.jpg.webp'),

('Samsung Galaxy S24', 'Современный смартфон с AI функциями', 79999.00,
 (SELECT "id" FROM "Categories" WHERE "name" = 'Смартфоны'),
 (SELECT "id" FROM "Manufacturers" WHERE "name" = 'Samsung'),
 'https://c.dns-shop.ru/thumb/st1/fit/0/0/0532d25b7de2547be7c23364cedf43bf/47f7e630d05e7f5fa016300cfbc4c23c7774510cf7b7275d8d32785bb3a31f1d.jpg.webp'),

('iPad Air', 'Планшет для работы и творчества', 59999.00,
 (SELECT "id" FROM "Categories" WHERE "name" = 'Планшеты'),
 (SELECT "id" FROM "Manufacturers" WHERE "name" = 'Apple'),
 'https://c.dns-shop.ru/thumb/st1/fit/0/0/0532d25b7de2547be7c23364cedf43bf/47f7e630d05e7f5fa016300cfbc4c23c7774510cf7b7275d8d32785bb3a31f1d.jpg.webp'),

('Sony WH-1000XM5', 'Беспроводные наушники с шумоподавлением', 29999.00,
 (SELECT "id" FROM "Categories" WHERE "name" = 'Наушники'),
 (SELECT "id" FROM "Manufacturers" WHERE "name" = 'Sony'),
 'https://c.dns-shop.ru/thumb/st1/fit/0/0/0532d25b7de2547be7c23364cedf43bf/47f7e630d05e7f5fa016300cfbc4c23c7774510cf7b7275d8d32785bb3a31f1d.jpg.webp'),

('Lenovo ThinkPad X1', 'Бизнес ноутбук с отличной клавиатурой', 129999.00,
 (SELECT "id" FROM "Categories" WHERE "name" = 'Ноутбуки'),
 (SELECT "id" FROM "Manufacturers" WHERE "name" = 'Lenovo'),
 'https://c.dns-shop.ru/thumb/st1/fit/0/0/0532d25b7de2547be7c23364cedf43bf/47f7e630d05e7f5fa016300cfbc4c23c7774510cf7b7275d8d32785bb3a31f1d.jpg.webp'),

('HP EliteDisplay', '27-дюймовый монитор 4K', 34999.00,
 (SELECT "id" FROM "Categories" WHERE "name" = 'Мониторы'),
 (SELECT "id" FROM "Manufacturers" WHERE "name" = 'HP'),
 'https://c.dns-shop.ru/thumb/st1/fit/0/0/0532d25b7de2547be7c23364cedf43bf/47f7e630d05e7f5fa016300cfbc4c23c7774510cf7b7275d8d32785bb3a31f1d.jpg.webp'),

('Samsung Galaxy Tab', 'Планшет для продуктивности', 39999.00,
 (SELECT "id" FROM "Categories" WHERE "name" = 'Планшеты'),
 (SELECT "id" FROM "Manufacturers" WHERE "name" = 'Samsung'),
 'https://c.dns-shop.ru/thumb/st1/fit/0/0/0532d25b7de2547be7c23364cedf43bf/47f7e630d05e7f5fa016300cfbc4c23c7774510cf7b7275d8d32785bb3a31f1d.jpg.webp')
ON CONFLICT DO NOTHING;

