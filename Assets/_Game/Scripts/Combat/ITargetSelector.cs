using UnityEngine;

public interface ITargetSelector
{
    Enemy GetClosestAliveEnemy(Vector3 position);
    Enemy GetBestEnemyTarget(Vector3 position, Enemy currentTarget);
} 