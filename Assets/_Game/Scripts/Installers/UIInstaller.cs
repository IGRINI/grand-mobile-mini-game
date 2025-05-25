using UnityEngine;
using Zenject;

public class UIInstaller : MonoInstaller
{
    [SerializeField] private TeamHealthUI _teamHealthUI;
    [SerializeField] private GameOverManager _gameOverManager;
    
    public override void InstallBindings()
    {
        Container.BindInstance(_teamHealthUI).AsSingle();
        
        if (_gameOverManager != null)
        {
            Container.BindInstance(_gameOverManager).AsSingle();
        }
        else
        {
            // Создаем GameOverManager если не назначен в инспекторе
            var gameOverManagerGO = new GameObject("GameOverManager");
            var gameOverManager = gameOverManagerGO.AddComponent<GameOverManager>();
            Container.BindInstance(gameOverManager).AsSingle();
        }
    }
} 