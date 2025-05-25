using UnityEngine;
using Zenject;

public class UIInstaller : MonoInstaller
{
    [SerializeField] private TeamHealthUI _teamHealthUI;
    [SerializeField] private UpgradeSelectionUI _upgradeSelectionUI;
    
    public override void InstallBindings()
    {
        Container.BindInstance(_teamHealthUI).AsSingle();
        
        if (_upgradeSelectionUI != null)
        {
            Container.BindInstance(_upgradeSelectionUI).AsSingle();
        }
        else
        {
            Debug.LogError("UpgradeSelectionUI не назначен в UIInstaller!");
        }
        
        Container.Bind<GameOverManager>().FromNewComponentOnNewGameObject().AsSingle().NonLazy();
        Container.Bind<TestExperienceGain>().FromNewComponentOnNewGameObject().AsSingle().NonLazy();
    }
} 