using Zenject;

public class HealthInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        Container.BindInterfacesAndSelfTo<HealthService>().AsSingle().NonLazy();
    }
} 