using UnityEngine;
using Zenject;

public class CharacterInstaller : MonoInstaller
{
    [SerializeField] private CharacterData[] characterConfigs;

    public override void InstallBindings()
    {
        Container.BindInstance(characterConfigs).AsSingle();
        Container.BindInterfacesAndSelfTo<CharacterService>().AsSingle().NonLazy();
    }
} 