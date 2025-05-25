# Система врагов

## Обзор
Система врагов реализована по принципам SOLID, DRY, KISS и следует архитектуре проекта с использованием Zenject для DI.

## Основные компоненты

### 1. Данные (Data)
- `EnemyData` - ScriptableObject с настройками врага (здоровье, скорость, урон, дальность атаки)

### 2. Модель (Model)
- `Enemy` - POCO класс, представляющий врага
- Методы: `TakeDamage()`, `Heal()`, свойства для доступа к данным

### 3. Сервис (Service)
- `IEnemyService` - интерфейс управления врагами
- `EnemyService` - реализация сервиса
- Функции: спавн, деспавн, нанесение урона, поиск ближайшего врага

### 4. Представление (View)
- `IEnemyView` - интерфейс отображения врага
- `EnemyView` - MonoBehaviour компонент для отображения врага с полоской здоровья

### 5. Поведение (Behavior)
- `IEnemyBehavior` - интерфейс поведения врага
- `FollowPlayerBehavior` - базовое поведение следования за игроком

### 6. Спавнер
- `EnemySpawner` - автоматический спавн врагов вокруг игрока
- `EnemyTestController` - тестовый контроллер для ручного спавна

### 7. DI
- `EnemyInstaller` - MonoInstaller для регистрации зависимостей

## Интеграция с оружием
- Снаряды (`Projectile`) автоматически наносят урон врагам при столкновении
- Оружие (`WeaponController`) автоматически нацеливается на ближайшего врага

## Принципы реализации
- ✅ Никаких `GameObject.Find`
- ✅ Использование `sqrMagnitude` вместо `Vector3.Distance`
- ✅ Dependency Injection через Zenject
- ✅ Разделение ответственности (MVC)
- ✅ Интерфейсы для всех сервисов
- ✅ ScriptableObject для данных

## Настройка
1. Создать `EnemyData` через меню "Game/Enemy Data"
2. Добавить `EnemyInstaller` на сцену и привязать массив `EnemyData`
3. Добавить `EnemySpawner` для автоспавна или `EnemyTestController` для тестов
4. Настроить префабы врагов с компонентом `EnemyView` 

# Система здоровья врагов

## Описание
Единая система управления здоровьем врагов, следующая принципам SOLID, KISS и DRY.

## Архитектура

### Принципы
- **Single Responsibility**: Каждый класс отвечает за одну задачу
- **Open/Closed**: Система открыта для расширения, закрыта для модификации
- **Dependency Inversion**: Зависимости от абстракций, не от конкретных классов
- **DRY**: Логика здоровья не дублируется
- **KISS**: Простая и понятная архитектура

### Компоненты

#### Интерфейсы
- `IEnemyHealthHandler` - единая точка управления здоровьем врагов
- `IEnemyComponent` - связь между Enemy моделью и MonoBehaviour компонентами

#### Классы
- `EnemyHealthHandler` - реализация управления здоровьем
- `Enemy` - модель врага (убраны дублирующие методы TakeDamage/Heal)
- `HittableEnemy` - компонент для взаимодействия с машиной

## Поток данных

### Получение урона
1. `CarDamageDealer` → `HittableEnemy.TakeDamageFromCar()`
2. `HittableEnemy` → `IEnemyHealthHandler.TakeDamage()`
3. `EnemyHealthHandler` → `Enemy.Health.TakeDamage()`
4. При смерти: `EnemyHealthHandler.EnemyDied` → `HittableEnemy.OnEnemyDied()`
5. `HittableEnemy` → анимация смерти и возврат в пул

### Восстановление здоровья
1. `HittableEnemy.ReturnToPool()` → `IEnemyHealthHandler.Heal()`
2. `EnemyHealthHandler` → `Enemy.Health.Heal()`

## Преимущества рефакторинга

### До рефакторинга
- Дублирование логики в Enemy.cs и HittableEnemy.cs
- Прямая работа с IHealth компонентом
- Нет единой точки управления здоровьем
- Сложно отслеживать смерть врагов

### После рефакторинга
- Единая точка управления здоровьем (`IEnemyHealthHandler`)
- Нет дублирования кода
- Четкое разделение ответственности
- Легко расширяемая архитектура
- Централизованное событие смерти

## Использование

### Нанесение урона
```csharp
// Через IEnemyHealthHandler
_healthHandler.TakeDamage(enemy, damage);

// Через HittableEnemy (для машины)
hittableEnemy.TakeDamageFromCar(damage);
```

### Отслеживание смерти
```csharp
_healthHandler.EnemyDied += OnEnemyDied;
```

### Связь Enemy с компонентами
```csharp
// В EnemyView.Initialize()
var enemyComponent = GetComponent<IEnemyComponent>();
enemyComponent?.SetEnemy(enemy);
```

## Интеграция

### DI регистрация
```csharp
// В EnemyInstaller
Container.Bind<IEnemyHealthHandler>().To<EnemyHealthHandler>().AsSingle();
```

### Компоненты врага
- `EnemyView` - основной компонент врага
- `HittableEnemy` - компонент для столкновений с машиной
- Оба реализуют `IEnemyComponent` для связи с `Enemy` моделью

## Расширяемость

Система легко расширяется для:
- Новых типов урона
- Дополнительных эффектов при смерти
- Различных способов восстановления здоровья
- Статистики и аналитики

Все изменения происходят через единую точку входа - `IEnemyHealthHandler`. 