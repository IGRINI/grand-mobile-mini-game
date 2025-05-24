using UnityEngine;
using Zenject;

public class CombatInstaller : MonoInstaller
{
    [SerializeField] private EnemyView enemyPrefab;

    public override void InstallBindings()
    {
        Container.BindInterfacesAndSelfTo<AttackSystem>().AsSingle();
        Container.BindInterfacesAndSelfTo<ProjectileService>().AsSingle();
        Container.BindInterfacesAndSelfTo<ProjectilePool>().AsSingle().NonLazy();
        Container.BindInterfacesAndSelfTo<ProjectileFactory>().AsSingle();
                
        Container.BindMemoryPool<EnemyView, EnemyPool>()
                .WithInitialSize(5)
                .FromComponentInNewPrefab(enemyPrefab)
                .UnderTransformGroup("EnemyPool");
    }
} 