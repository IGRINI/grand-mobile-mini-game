using Zenject;

public class CameraInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        Container.Bind<IBuildingOcclusionDetector>()
            .To<BuildingOcclusionDetector>()
            .AsSingle();
            
        Container.BindInterfacesTo<CameraOcclusionController>()
            .FromComponentInHierarchy()
            .AsSingle();
    }
} 