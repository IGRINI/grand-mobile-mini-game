# Checklist разработчика

1. Подготовить префаб машинки с 4 пустыми дочерними `Transform` (pivots) и выставить их в `CarView`.
2. Добавить в `ICarView` и `CarView` свойство `IReadOnlyList<Transform> CharacterPivots`.
3. Создать класс `Character` с методом `Seat`.
4. Создать `ICharacterService` и `CharacterService`, реализовать логику списка и посадки.
5. Сделать `CharacterInstaller`, привязать массив префабов персонажей.
6. Спавнер бонусов (`PickupSpawner`):
   - подготовить префабы бонусов (копии `characterPrefabs` или отдельные маркеры);
   - написать логику Spawn/Despawn.
7. Обработчик столкновений:
   - получить `ICharacterService` из контейнера, при подборе бонуса:
     - если есть пустой слот – `Select` с добавлением;
     - иначе – открыть `ReplacePanel`.
8. UI:
   - `ReplacePanel` с выбором замены.
9. Локальные тесты:
   - Юнит-тесты для `CharacterService`.
   - Интеграционные тесты посадки.
10. Оптимизация:
    - Проверить в Update вызовы `Vector3` операций, заменить на `sqrMagnitude`.
    - Убедиться, что нет `Find*`, `Reflection`.
11. Code review и merge в основную ветку. 