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
        bool foundPassenger = false;
        
        var characters = _characterService.GetAllCharacters();
        
        foreach (var character in characters)
        {
            if (!character.Health.IsAlive) continue;
            
            var characterView = _characterService.GetCharacterView(character);
            if (characterView?.HitTarget == null) continue;
            
            bool isDriver = _characterService.IsDriver(character);
            
            if (foundPassenger && isDriver) continue;
            
            var sqrDistance = (characterView.HitTarget.position - enemyPosition).sqrMagnitude;
            
            if (!foundPassenger && !isDriver)
            {
                foundPassenger = true;
                bestTarget = characterView.HitTarget;
                closestSqrDistance = sqrDistance;
            }
            else if (foundPassenger == isDriver && sqrDistance < closestSqrDistance)
            {
                bestTarget = characterView.HitTarget;
                closestSqrDistance = sqrDistance;
            }
        }
        
        if (bestTarget == null && _carView?.HitTarget != null)
        {
            bestTarget = _carView.HitTarget;
        }
        
        return bestTarget;
    }
} 