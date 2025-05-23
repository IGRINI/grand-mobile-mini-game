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

    public override void InstallBindings()
    {
        Container.BindInstance(inputAsset).AsSingle();
        Container.Bind<ICarModel>().To<CarModel>().AsSingle().WithArguments(acceleration, brakeForce, maxSpeed, maxReverseSpeed, turnSpeed);
        Container.Bind<ICarView>().To<CarView>().FromComponentInHierarchy().AsSingle();
        Container.BindInterfacesAndSelfTo<InputService>().AsSingle().NonLazy();
        Container.BindInterfacesTo<CarController>().AsSingle().NonLazy();
    }
} 