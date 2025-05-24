using UnityEngine;
using Zenject;

public class EnemyInstaller : MonoInstaller
{
    [SerializeField] private EnemyData[] enemyConfigs;

    public override void InstallBindings()
    {
        Container.BindInstance(enemyConfigs).AsSingle();
        Container.BindInterfacesAndSelfTo<EnemyService>().AsSingle().NonLazy();
    }
} 