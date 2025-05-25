# Система апгрейдов

## Обзор
Универсальная система апгрейдов с карточным интерфейсом, автоматической паузой игры и легким добавлением новых апгрейдов.

## Основные компоненты

### 1. Данные (Data)
- `UpgradeData` - ScriptableObject с настройками апгрейда
- `UpgradeType` - enum с типами апгрейдов
- Поддержка уровней, требований и конфликтов

### 2. Модель (Model)
- `PlayerUpgrade` - POCO класс для апгрейда игрока
- Отслеживание текущего уровня и значений

### 3. Сервис (Service)
- `IUpgradeService` - интерфейс управления апгрейдами
- `UpgradeService` - реализация с генерацией случайных вариантов
- `UpgradeEffectApplier` - применение эффектов апгрейдов

### 4. UI
- `UpgradeSelectionUI` - основной UI с паузой игры
- `UpgradeCard` - карточка апгрейда с анимациями
- Автоматическое появление при повышении уровня

### 5. DI
- `UpgradeInstaller` - MonoInstaller для регистрации зависимостей

## Типы апгрейдов

### Боевые
- `IncreaseCharacterDamage` - увеличение урона персонажей
- `IncreaseRamDamage` - увеличение урона от сбивания
- `IncreaseFireRate` - увеличение скорости стрельбы
- `IncreaseRange` - увеличение дальности атаки
- `IncreaseCriticalChance` - увеличение шанса критического удара
- `IncreaseCriticalDamage` - увеличение критического урона

### Защитные
- `ReduceIncomingDamage` - уменьшение входящего урона
- `ReduceDamageToCharacters` - уменьшение урона по персонажам
- `AddArmor` - добавление брони
- `IncreaseHealthRegeneration` - увеличение регенерации здоровья

### Утилитарные
- `HealthRestore` - восстановление здоровья
- `AddPassenger` - добавление нового пассажира
- `IncreaseMaxSpeed` - увеличение максимальной скорости
- `IncreaseMovementSpeed` - увеличение скорости движения
- `ReduceWeaponCooldown` - уменьшение перезарядки оружия
- `IncreaseExperienceGain` - увеличение получения опыта

## Интеграция с системой опыта

### Автоматическое появление
- При повышении уровня автоматически показываются 3 случайных апгрейда
- Игра ставится на паузу (`Time.timeScale = 0`)
- Игрок должен выбрать один из предложенных апгрейдов

### События
```csharp
_upgradeService.UpgradeOptionsAvailable += OnUpgradeOptionsAvailable;
_upgradeService.UpgradeSelected += OnUpgradeSelected;
_upgradeService.UpgradeSelectionClosed += OnUpgradeSelectionClosed;
```

## Настройка апгрейдов

### Создание UpgradeData
1. Создать через меню "Game/Upgrade Data"
2. Настроить основную информацию:
   - `upgradeName` - название апгрейда
   - `description` - описание
   - `icon` - иконка
   - `upgradeType` - тип из enum

3. Настроить значения:
   - `value` - базовое значение
   - `isPercentage` - процентное значение или абсолютное
   - `maxLevel` - максимальный уровень
   - `valuePerLevel` - прирост за уровень

4. Настроить требования:
   - `minPlayerLevel` / `maxPlayerLevel` - уровни игрока
   - `requiredUpgrades` - необходимые апгрейды
   - `conflictingUpgrades` - конфликтующие апгрейды

### Форматированное описание
```csharp
formattedDescription = "Увеличивает урон на {value}";
// Автоматически заменит {value} на текущее значение
```

## Логика выбора апгрейдов

### Фильтрация доступных
1. Проверка уровня игрока
2. Проверка максимального уровня апгрейда
3. Проверка требуемых апгрейдов
4. Проверка конфликтующих апгрейдов

### Генерация вариантов
```csharp
var options = _upgradeService.GenerateUpgradeOptions(playerLevel, 3);
```

## Применение эффектов

### UpgradeEffectApplier
- Автоматически применяет эффекты при выборе апгрейда
- Содержит логику для каждого типа апгрейда
- Предоставляет методы для получения множителей

### Использование в других системах
```csharp
[Inject] UpgradeEffectApplier _upgradeEffects;

float damageMultiplier = _upgradeEffects.GetDamageMultiplier(UpgradeType.IncreaseCharacterDamage);
float speedMultiplier = _upgradeEffects.GetSpeedMultiplier(UpgradeType.IncreaseMaxSpeed);
```

## UI и анимации

### UpgradeSelectionUI
- Автоматическая пауза игры
- Анимированное появление панели
- Создание карточек с задержкой
- Кнопка пропуска (опционально)

### UpgradeCard
- Hover эффекты с масштабированием
- Анимация выбора
- Отображение текущего уровня
- Цветовая кастомизация

### Анимации с паузой
- Все анимации используют `.SetUpdate(true)` для работы при `Time.timeScale = 0`
- DOTween последовательности для плавных переходов

## Расширяемость

### Добавление нового апгрейда
1. Добавить новый тип в `UpgradeType` enum
2. Создать `UpgradeData` ScriptableObject
3. Добавить логику в `UpgradeEffectApplier.ApplyUpgradeEffect()`
4. Добавить в массив `availableUpgrades` в `UpgradeInstaller`

### Пример нового апгрейда
```csharp
// В UpgradeType enum
IncreaseJumpHeight,

// В UpgradeEffectApplier
case UpgradeType.IncreaseJumpHeight:
    ApplyJumpHeightIncrease(upgrade.CurrentValue);
    break;
```

## Настройка на сцене

### 1. Создание апгрейдов
1. Создать несколько `UpgradeData` через меню
2. Настроить их параметры и иконки

### 2. Установка инсталлера
1. Добавить `UpgradeInstaller` на сцену
2. Привязать массив созданных `UpgradeData`

### 3. Настройка UI
1. Добавить `UpgradeSelectionUI` на Canvas
2. Создать префаб `UpgradeCard`
3. Настроить ссылки в UI компоненте

## Принципы реализации
- ✅ Никаких `GameObject.Find`
- ✅ Dependency Injection через Zenject
- ✅ Разделение ответственности (MVC)
- ✅ Интерфейсы для всех сервисов
- ✅ ScriptableObject для данных
- ✅ Система событий для расширяемости
- ✅ Автоматическая пауза игры
- ✅ Легкое добавление новых апгрейдов
- ✅ Анимации с поддержкой паузы 