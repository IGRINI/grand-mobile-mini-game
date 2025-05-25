using UnityEngine;

public interface ICharacterTargetSelector
{
    Transform GetBestCharacterTarget(Vector3 enemyPosition);
} 