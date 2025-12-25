-- Обновление изображений для услуг настройки Windows Server и других услуг настройки
-- Устанавливаем изображение setupWinserv.png для всех услуг настройки

-- Обновляем услуги настройки Windows Server (по slug)
UPDATE Products 
SET ImageUrl = '/images/setupWinserv.png'
WHERE Slug LIKE 'windows-server-setup-%'
   OR (Type = 2 AND (Name LIKE '%Setup%' OR Name LIKE '%Настройка%' OR Name LIKE '%настройка%' OR Name LIKE '%установка%' OR Name LIKE '%Установка%'))
   OR (Type = 2 AND ImageUrl IS NULL);

-- Обновляем другие услуги настройки (по slug config-*)
UPDATE Products 
SET ImageUrl = '/images/setupWinserv.png'
WHERE Slug LIKE 'config-%'
   AND ImageUrl IS NULL;

GO

