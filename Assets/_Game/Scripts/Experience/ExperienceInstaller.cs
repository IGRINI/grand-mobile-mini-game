using UnityEngine;
using Zenject;

public class ExperienceInstaller : MonoInstaller
{
    [Header("Experience Configuration")]
    [SerializeField] private ExperienceConfig experienceConfig;
    
    public override void InstallBindings()
    {
        if (experienceConfig == null)
        {
            Debug.LogError("ExperienceConfig не назначен в ExperienceInstaller!");
            return;
        }
        
        Container.Bind<ExperienceConfig>().FromInstance(experienceConfig).AsSingle();
        Container.BindInterfacesAndSelfTo<ExperienceService>().AsSingle().NonLazy();
    }
} 