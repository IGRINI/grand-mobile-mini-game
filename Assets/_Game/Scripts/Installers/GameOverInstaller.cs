using UnityEngine;
using Zenject;

public class GameOverInstaller : MonoInstaller
{
    [SerializeField] private GameOverManager gameOverManagerPrefab;
    
    public override void InstallBindings()
    {
        var gameOverManager = Container.InstantiatePrefabForComponent<GameOverManager>(gameOverManagerPrefab);
        Container.Bind<GameOverManager>().FromInstance(gameOverManager).AsSingle();
    }
} 