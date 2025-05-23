# Техническое задание

1. **Стек и зависимости**
   - Unity 2021+ (или актуальная LTS).
   - Zenject для DI.
   - UniTask (опционально) для асинхронных задач.
   - Никаких вызовов `GameObject.Find`, `FindObjectOfType`, никакой рефлексии – только внедрение зависимостей.

2. **Архитектура**
   - Модель–Контроллер–View (MVC/MVP):
     - `CarModel`, `CarController`, `CarView` (MonoBehaviour).
     - `Character` (POCO), `CharacterService` (сервис), `CharacterInstaller`.
   - DI (Zenject): все зависимости через конструктор/модули.

3. **Слоты посадки**
   - В `CarView` добавить 4 `Transform[] characterPivots` (настраиваются в инспекторе).
   - Позиции задать вручную в префабе машинки.

4. **Классы и интерфейсы**
   - `Character`:
     - свойства: `Name`, `Prefab`;
     - метод `Seat(ICarView, int slotIndex)` – через `Instantiate`, `transform.parent = pivot`, сброс `localPosition` и `localRotation`.
   - `ICharacterService`:
     - `IReadOnlyList<Character> Characters`, `Character Selected`;
     - `void Select(Character)`.
   - `CharacterService` (реализация `IInitializable`):
     - в `Initialize()` сразу сажает первый персонажа в слот 0;
     - при `Select` меняет модель и вызывает `Seat`.
   - `CharacterInstaller` (MonoInstaller):
     - привязать массив `GameObject[] characterPrefabs`;
     - биндинг `CharacterService` + массив префабов.

5. **Сбор бонусов**
   - Спавнер бонусов (`PickupSpawner`): периодически или по событиям выпускает предзагруженные «бонусные» префабы на пространство карты.
   - При столкновении с машинкой – получить ссылку на `ICharacterService`, вызвать `Select` или автоматическую посадку в свободный слот.

6. **UI замены**
   - Панель замены (`ReplacePanel`): выводит список текущих персонажей и новый бонус;
   - Клик/Тач по слоту меняет выбранного;
   - Rx-подписки или UniTask для асинхронного ожидания выбора.

7. **Производительность**
   - Для проверки расстояний использовать `sqrMagnitude`, а не `Vector3.Distance`.
   - Минимизировать аллокации в Update/Tick.

8. **Тестирование**
   - Юнит-тесты для `CharacterService` (выбор, замена).
   - Интеграционные тесты для посадки (Validate: объект в иерархии привязан к нужному `pivot`). 