using UnityEngine;

public class CharacterTargetSelector : ICharacterTargetSelector
{
    private readonly ICharacterService _characterService;
    private readonly ICarView _carView;
    
    public CharacterTargetSelector(ICharacterService characterService, ICarView carView)
    {
        _characterService = characterService;
        _carView = carView;
    }
    
    public Transform GetBestCharacterTarget(Vector3 enemyPosition)
    {
        Transform bestTarget = null;
        float closestSqrDistance = float.MaxValue;
        Transform driverTarget = null;
        float driverSqrDistance = float.MaxValue;
        
        var characters = _characterService.GetAllCharacters();
        Debug.Log($"CharacterTargetSelector: проверяем {characters.Count} персонажей");
        
        foreach (var character in characters)
        {
            if (!character.Health.IsAlive) 
            {
                Debug.Log($"Персонаж {character.Name} мертв, пропускаем");
                continue;
            }
            
            var characterView = _characterService.GetCharacterView(character);
            if (characterView?.HitTarget == null) 
            {
                Debug.Log($"У персонажа {character.Name} нет HitTarget");
                continue;
            }
            
            bool isDriver = _characterService.IsDriver(character);
            var sqrDistance = (characterView.HitTarget.position - enemyPosition).sqrMagnitude;
            
            Debug.Log($"Персонаж {character.Name}: водитель={isDriver}, расстояние={Mathf.Sqrt(sqrDistance):F1}");
            
            if (isDriver)
            {
                if (driverTarget == null || sqrDistance < driverSqrDistance)
                {
                    driverTarget = characterView.HitTarget;
                    driverSqrDistance = sqrDistance;
                }
            }
            else
            {
                if (bestTarget == null || sqrDistance < closestSqrDistance)
                {
                    bestTarget = characterView.HitTarget;
                    closestSqrDistance = sqrDistance;
                }
            }
        }
        
        if (bestTarget != null)
        {
            Debug.Log($"Выбрана цель: пассажир на расстоянии {Mathf.Sqrt(closestSqrDistance):F1}");
            return bestTarget;
        }
        
        if (driverTarget != null)
        {
            Debug.Log($"Выбрана цель: водитель на расстоянии {Mathf.Sqrt(driverSqrDistance):F1}");
            return driverTarget;
        }
        
        if (_carView?.HitTarget != null)
        {
            Debug.Log("Выбрана цель: машина");
            return _carView.HitTarget;
        }
        
        Debug.Log("Цель не найдена");
        return null;
    }
} 