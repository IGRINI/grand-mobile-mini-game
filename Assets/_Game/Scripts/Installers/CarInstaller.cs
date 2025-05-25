using UnityEngine;
using Zenject;
using UnityEngine.InputSystem;

public class CarInstaller : MonoInstaller
{
    [SerializeField] private InputActionAsset inputAsset;
    [SerializeField] private float acceleration = 5f;
    [SerializeField] private float brakeForce = 8f;
    [SerializeField] private float maxSpeed = 10f;
    [SerializeField] private float maxReverseSpeed = 5f;
    [SerializeField] private float turnSpeed = 180f;
    [SerializeField] private float minSpeedToTurn = 1f;
    [SerializeField] private float maxTurnRate = 270f;
    [SerializeField] private float turnRateSpeedFactor = 0.5f;
    [SerializeField] private float baseCollisionDamage = 25f;
    [SerializeField] private float selfDamageMultiplier = 0.3f;
    [SerializeField] private float speedReductionPerEnemy = 2f;
    [SerializeField] private float initialHealth = 100f;

    public override void InstallBindings()
    {
        Container.BindInstance(inputAsset).AsSingle();
        Container.Bind<ICarModel>().To<CarModel>().AsSingle().WithArguments(new object[]{acceleration, brakeForce, maxSpeed, maxReverseSpeed, turnSpeed, minSpeedToTurn, maxTurnRate, turnRateSpeedFactor, baseCollisionDamage, selfDamageMultiplier, speedReductionPerEnemy});
        Container.BindInstance(initialHealth).WhenInjectedInto<CarController>();
        Container.Bind<ICarView>().To<CarView>().FromComponentInHierarchy().AsSingle();
        Container.Bind<IInputService>().To<MobileInputService>().FromComponentInHierarchy().AsSingle().NonLazy();
        Container.Bind<IObstacleService>().To<ObstacleService>().AsSingle();
        Container.Bind<ICollisionDetector>().FromMethod(ctx => ctx.Container.Resolve<IObstacleService>().GetCollisionDetector()).AsSingle();
        Container.Bind<IEnemyDetector>().To<EnemyDetector>().AsSingle();
        Container.Bind<ICarDamageDealer>().To<CarDamageDealer>().AsSingle();
        Container.Bind<ITargetSelector>().To<TargetSelector>().AsSingle();
        Container.Bind<ICharacterTargetSelector>().To<CharacterTargetSelector>().AsSingle();
        Container.BindInterfacesAndSelfTo<CarController>().AsSingle().NonLazy();
    }
} 