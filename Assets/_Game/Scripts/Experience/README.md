# Система опыта и уровней

## Обзор
Универсальная система опыта и уровней с умной формулой расчета, визуальными эффектами и подготовкой к системе апгрейдов.

## Основные компоненты

### 1. Конфигурация (Data)
- `ExperienceConfig` - ScriptableObject с настройками системы опыта
- Умная формула: `baseExp * exponentialMultiplier^(level-1) + linearMultiplier * (level-1)`
- Настройки бонусов, штрафов и визуальных эффектов

### 2. Модель (Model)
- `ExperienceSystem` - POCO класс с логикой опыта и уровней
- `ExperienceSystemData` - данные для сохранения/загрузки

### 3. Сервис (Service)
- `IExperienceService` - интерфейс управления опытом
- `ExperienceService` - реализация с автоматическим начислением опыта за врагов

### 4. UI
- `ExperienceUI` - компонент отображения опыта с анимациями
- Полоска прогресса, уведомления о повышении уровня и получении опыта

### 5. DI
- `ExperienceInstaller` - MonoInstaller для регистрации зависимостей

## Умная формула расчета

### Опыт для уровня
```
requiredExp = baseExp * exponentialMultiplier^(level-1) + linearMultiplier * (level-1)
```

**Параметры по умолчанию:**
- `baseExp = 100` - базовый опыт для 2 уровня
- `exponentialMultiplier = 1.2` - экспоненциальный рост
- `linearMultiplier = 50` - линейная добавка

**Примеры:**
- Уровень 2: 100 * 1.2^1 + 50 * 1 = 170 опыта
- Уровень 5: 100 * 1.2^4 + 50 * 4 = 307 опыта
- Уровень 10: 100 * 1.2^9 + 50 * 9 = 965 опыта

### Корректировка опыта за врагов
```
adjustedExp = baseExp * levelDifferenceMultiplier * playerLevelBonus
```

**Бонусы и штрафы:**
- Враг выше уровня: +10% за каждый уровень разницы
- Враг ниже уровня: -15% за каждый уровень разницы (с ограничением)
- Бонус за уровень игрока: +5% за каждый уровень

**Система убывающей отдачи:**
- При штрафе > 80% применяется формула: `80% + (штраф - 80%) * 50%`
- Минимальный опыт: 10% от базового

## Интеграция с врагами

### Автоматическое начисление
- Система автоматически подписывается на событие `EnemyHealthHandler.EnemyDied`
- Опыт начисляется из `Enemy.RewardExperience`
- Уровень врага рассчитывается по его характеристикам

### Расчет уровня врага
```csharp
float healthRatio = enemy.Health / 100f;
float damageRatio = enemy.AttackDamage / 50f;
float speedRatio = enemy.MoveSpeed / 5f;
int enemyLevel = (healthRatio + damageRatio + speedRatio) / 3f * 10f;
```

## Настройка

### 1. Создание конфигурации
1. Создать `ExperienceConfig` через меню "Game/Experience Config"
2. Настроить параметры формулы и эффекты
3. Назначить префабы эффектов (опционально)

### 2. Установка на сцену
1. Добавить `ExperienceInstaller` на сцену
2. Привязать созданный `ExperienceConfig`
3. Добавить `ExperienceUI` на Canvas

### 3. Настройка UI
- `levelText` - текст уровня
- `experienceText` - текст прогресса
- `experienceBar` - полоска опыта
- `levelUpNotification` - уведомление о повышении уровня
- `experienceGainNotification` - уведомление о получении опыта

## События системы

### IExperienceService события
- `LevelChanged` - изменение уровня
- `ExperienceChanged` - изменение опыта
- `LevelUp` - повышение уровня (с эффектами)
- `ExperienceGained` - получение опыта (с эффектами)

### Использование
```csharp
[Inject] IExperienceService _experienceService;

void Start()
{
    _experienceService.LevelUp += OnLevelUp;
    _experienceService.ExperienceGained += OnExperienceGained;
}

void OnLevelUp(int newLevel)
{
    Debug.Log($"Новый уровень: {newLevel}!");
}
```

## Подготовка к апгрейдам

### Архитектура
- Система событий готова для интеграции с апгрейдами
- `LevelUp` событие - идеальная точка для предложения апгрейдов
- Уровень игрока доступен через `IExperienceService.CurrentLevel`

### Будущие возможности
- Очки навыков за уровень
- Разблокировка апгрейдов по уровню
- Престиж система для бесконечного прогресса
- Множители опыта как апгрейды

## Сохранение/Загрузка

### Данные для сохранения
```csharp
var saveData = _experienceService.GetSaveData();
// saveData.CurrentLevel
// saveData.TotalExperience
```

### Загрузка данных
```csharp
_experienceService.LoadSaveData(saveData);
```

## Принципы реализации
- ✅ Никаких `GameObject.Find`
- ✅ Использование `sqrMagnitude` где возможно
- ✅ Dependency Injection через Zenject
- ✅ Разделение ответственности (MVC)
- ✅ Интерфейсы для всех сервисов
- ✅ ScriptableObject для конфигурации
- ✅ Система событий для расширяемости
- ✅ Готовность к системе апгрейдов 