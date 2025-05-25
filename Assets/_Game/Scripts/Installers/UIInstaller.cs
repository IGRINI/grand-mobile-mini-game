using UnityEngine;
using Zenject;

public class UIInstaller : MonoInstaller
{
    [SerializeField] private TeamHealthUI _teamHealthUI;
    
    public override void InstallBindings()
    {
        Container.BindInstance(_teamHealthUI).AsSingle();
        Container.Bind<GameOverManager>().FromNewComponentOnNewGameObject().AsSingle().NonLazy();
    }
} 