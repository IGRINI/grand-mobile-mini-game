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
            
            if (isDriver)
            {
                Debug.Log($"Персонаж {character.Name} - водитель, пропускаем (водителя не трогаем)");
                continue;
            }
            
            var sqrDistance = (characterView.HitTarget.position - enemyPosition).sqrMagnitude;
            Debug.Log($"Персонаж {character.Name}: пассажир, расстояние={Mathf.Sqrt(sqrDistance):F1}");
            
            if (bestTarget == null || sqrDistance < closestSqrDistance)
            {
                bestTarget = characterView.HitTarget;
                closestSqrDistance = sqrDistance;
            }
        }
        
        if (bestTarget != null)
        {
            Debug.Log($"Выбрана цель: пассажир на расстоянии {Mathf.Sqrt(closestSqrDistance):F1}");
            return bestTarget;
        }
        
        if (_carView?.HitTarget != null)
        {
            Debug.Log("Нет живых пассажиров, выбрана цель: машина");
            return _carView.HitTarget;
        }
        
        Debug.Log("Цель не найдена");
        return null;
    }
} 