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
        
        foreach (var character in characters)
        {
            if (!character.Health.IsAlive) continue;
            
            var characterView = _characterService.GetCharacterView(character);
            if (characterView?.HitTarget == null) continue;
            
            bool isDriver = _characterService.IsDriver(character);
            var sqrDistance = (characterView.HitTarget.position - enemyPosition).sqrMagnitude;
            
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
            return bestTarget;
        }
        
        if (driverTarget != null)
        {
            return driverTarget;
        }
        
        if (_carView?.HitTarget != null)
        {
            return _carView.HitTarget;
        }
        
        return null;
    }
} 